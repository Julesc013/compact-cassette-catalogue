[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Get-BytesSha256 {
    param([byte[]]$Bytes)
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { return ([BitConverter]::ToString($algorithm.ComputeHash($Bytes))).Replace('-', '').ToLowerInvariant() }
    finally { $algorithm.Dispose() }
}

function Get-EntryBytes {
    param($Entry)
    $input = $Entry.Open()
    $memory = New-Object IO.MemoryStream
    try { $input.CopyTo($memory); return ,$memory.ToArray() }
    finally { $memory.Dispose(); $input.Dispose() }
}

function Rewrite-SetupPackage {
    param([string]$Path, [string]$TargetEntry, [ValidateSet('append', 'identity', 'extra')][string]$Mutation)
    $archive = [IO.Compression.ZipFile]::OpenRead($Path)
    try {
        $records = New-Object Collections.Generic.List[Object]
        foreach ($entry in $archive.Entries) {
            $bytes = Get-EntryBytes -Entry $entry
            if ($entry.FullName -ceq $TargetEntry) {
                if ($Mutation -ceq 'append') { $bytes = [byte[]]($bytes + [byte]0x21) }
                elseif ($Mutation -ceq 'identity') {
                    $text = [Text.Encoding]::UTF8.GetString($bytes)
                    if (-not $text.Contains('stage="Alpha 3"')) { throw 'Synthetic payload identity target was not found.' }
                    $bytes = [Text.Encoding]::UTF8.GetBytes($text.Replace('stage="Alpha 3"', 'stage="Alpha 4"'))
                }
            }
            $records.Add([PSCustomObject]@{ name = $entry.FullName; bytes = $bytes; timestamp = $entry.LastWriteTime })
        }
    }
    finally { $archive.Dispose() }
    if ($Mutation -ceq 'extra') {
        $records.Add([PSCustomObject]@{ name = 'unexpected.txt'; bytes = [Text.Encoding]::UTF8.GetBytes('unexpected'); timestamp = [DateTimeOffset]::Parse('2000-01-01T00:00:00Z') })
    }
    $stream = [IO.File]::Open($Path, [IO.FileMode]::Create, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
    $archive = New-Object IO.Compression.ZipArchive($stream, [IO.Compression.ZipArchiveMode]::Create, $false)
    try {
        foreach ($record in $records) {
            $entry = $archive.CreateEntry([string]$record.name, [IO.Compression.CompressionLevel]::Optimal)
            $entry.LastWriteTime = [DateTimeOffset]$record.timestamp
            $output = $entry.Open()
            try { $output.Write([byte[]]$record.bytes, 0, ([byte[]]$record.bytes).Length) }
            finally { $output.Dispose() }
        }
    }
    finally { $archive.Dispose(); $stream.Dispose() }
}

function Update-AuthenticatedSidecars {
    param([string]$PackageDirectory, [string]$EvidenceDirectory, [string]$PackageName)
    $packagePath = Join-Path $PackageDirectory $PackageName
    $entryManifestName = "$PackageName.entries.json"
    $entryManifestPath = Join-Path $EvidenceDirectory $entryManifestName
    $entryManifest = Get-Content -LiteralPath $entryManifestPath -Raw | ConvertFrom-Json
    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entryManifest.entries = @($archive.Entries | ForEach-Object {
                $bytes = Get-EntryBytes -Entry $_
                [PSCustomObject]@{ name = $_.FullName; size = [long]$bytes.Length; sha256 = Get-BytesSha256 -Bytes $bytes }
            })
    }
    finally { $archive.Dispose() }
    $entryManifest.packageSha256 = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash.ToLowerInvariant()
    [IO.File]::WriteAllText($entryManifestPath, (($entryManifest | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))

    $checksumPath = Join-Path $PackageDirectory 'SHA256SUMS.txt'
    $lines = @(Get-Content -LiteralPath $checksumPath)
    for ($index = 0; $index -lt $lines.Count; $index++) {
        if ($lines[$index].EndsWith("  $PackageName", [StringComparison]::Ordinal)) { $lines[$index] = "$($entryManifest.packageSha256)  $PackageName" }
    }
    [IO.File]::WriteAllText($checksumPath, (($lines -join "`n") + "`n"), (New-Object Text.UTF8Encoding($false)))

    $entryChecksumPath = Join-Path $EvidenceDirectory 'ENTRY_MANIFEST_SHA256SUMS.txt'
    $lines = @(Get-Content -LiteralPath $entryChecksumPath)
    $manifestHash = (Get-FileHash -LiteralPath $entryManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()
    for ($index = 0; $index -lt $lines.Count; $index++) {
        if ($lines[$index].EndsWith("  $entryManifestName", [StringComparison]::Ordinal)) { $lines[$index] = "$manifestHash  $entryManifestName" }
    }
    [IO.File]::WriteAllText($entryChecksumPath, (($lines -join "`n") + "`n"), (New-Object Text.UTF8Encoding($false)))
}

function Assert-Rejected {
    param([string]$Name, [string]$TargetEntry, [string]$Mutation, [string]$ExpectedFragment)
    $caseRoot = Join-Path $testRoot $Name.Replace(' ', '-')
    $casePackages = Join-Path $caseRoot 'packages'
    $caseEvidence = Join-Path $caseRoot 'evidence'
    [IO.Directory]::CreateDirectory($casePackages) | Out-Null
    [IO.Directory]::CreateDirectory($caseEvidence) | Out-Null
    Copy-Item -Path (Join-Path $sourcePackageDirectory '*') -Destination $casePackages -Recurse -Force
    Copy-Item -Path (Join-Path $sourceEvidenceDirectory '*') -Destination $caseEvidence -Recurse -Force
    $packagePath = Join-Path $casePackages $lanePackageName
    Rewrite-SetupPackage -Path $packagePath -TargetEntry $TargetEntry -Mutation $Mutation
    Update-AuthenticatedSidecars -PackageDirectory $casePackages -EvidenceDirectory $caseEvidence -PackageName $lanePackageName
    try {
        & (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration `
            -PackageDirectory $casePackages -EvidenceDirectory $caseEvidence `
            -PortablePackageDirectory $portablePackageDirectory -PortableEvidenceDirectory $portableEvidenceDirectory *> $null
        throw "Negative case '$Name' was accepted."
    }
    catch {
        if ($_.Exception.Message -notlike "*$ExpectedFragment*") { throw "Negative case '$Name' failed for the wrong reason: $($_.Exception.Message)" }
        Write-Host "PASS: $Name rejected: $($_.Exception.Message)"
    }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$sourcePackageDirectory = Join-Path $repositoryRoot "artifacts\setup\packages\$($manifest.releaseLabel)"
$sourceEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\setup-packages\$($manifest.releaseLabel)"
$portablePackageDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)"
$portableEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)"
$lanePackageName = [string]$manifest.lanes[0].setupPackageName
$testParent = Join-Path $repositoryRoot 'artifacts\tests\setup-package-controls'
[IO.Directory]::CreateDirectory($testParent) | Out-Null
$testRoot = Join-Path $testParent ([Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($testRoot) | Out-Null
try {
    Assert-Rejected -Name 'altered portable payload with rewritten sidecars' -TargetEntry 'payload/README.txt' -Mutation append -ExpectedFragment 'not byte-identical'
    Assert-Rejected -Name 'altered setup executable with rewritten sidecars' -TargetEntry 'SETUP.exe' -Mutation append -ExpectedFragment 'does not match its verified build output'
    Assert-Rejected -Name 'altered payload identity with rewritten sidecars' -TargetEntry 'payload.xml' -Mutation identity -ExpectedFragment 'payload.xml product identity is not exact'
    Assert-Rejected -Name 'unexpected bundle entry with rewritten sidecars' -TargetEntry '' -Mutation extra -ExpectedFragment 'not the exact ten-entry bundle'
}
finally {
    if (Test-Path -LiteralPath $testRoot) { [IO.Directory]::Delete($testRoot, $true) }
}
Write-Host 'Setup package controls rejected authenticated-sidecar tampering of application, setup, identity, and bundle closure.'
