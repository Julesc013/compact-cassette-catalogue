[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$PortablePackageDirectory,
    [string]$PortableEvidenceDirectory,
    [string]$OutputDirectory,
    [string]$EvidenceDirectory,
    [switch]$RequireCandidateEvidence
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
. (Join-Path $PSScriptRoot 'package-evidence-set.ps1')

function Get-StreamSha256 {
    param([Parameter(Mandatory = $true)]$Stream)
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { return ([BitConverter]::ToString($algorithm.ComputeHash($Stream))).Replace('-', '').ToLowerInvariant() }
    finally { $algorithm.Dispose() }
}

function Get-ZipEntryBytes {
    param([Parameter(Mandatory = $true)]$Archive, [Parameter(Mandatory = $true)][string]$Name)
    $entry = $Archive.GetEntry($Name)
    if ($null -eq $entry) { throw "Portable package entry is missing: $Name" }
    $input = $entry.Open()
    $memory = New-Object IO.MemoryStream
    try { $input.CopyTo($memory); return ,$memory.ToArray() }
    finally { $memory.Dispose(); $input.Dispose() }
}

function Add-ZipBytesEntry {
    param(
        [Parameter(Mandatory = $true)]$Archive,
        [Parameter(Mandatory = $true)][string]$EntryName,
        [Parameter(Mandatory = $true)][byte[]]$Bytes,
        [Parameter(Mandatory = $true)][DateTimeOffset]$Timestamp
    )
    $entry = $Archive.CreateEntry($EntryName, [IO.Compression.CompressionLevel]::Optimal)
    $entry.LastWriteTime = $Timestamp
    $output = $entry.Open()
    try { $output.Write($Bytes, 0, $Bytes.Length) }
    finally { $output.Dispose() }
}

function Get-BytesSha256 {
    param([Parameter(Mandatory = $true)][byte[]]$Bytes)
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { return ([BitConverter]::ToString($algorithm.ComputeHash($Bytes))).Replace('-', '').ToLowerInvariant() }
    finally { $algorithm.Dispose() }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if ([string]::IsNullOrWhiteSpace($PortablePackageDirectory)) { $PortablePackageDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)" }
if ([string]::IsNullOrWhiteSpace($PortableEvidenceDirectory)) { $PortableEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)" }
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) { $OutputDirectory = Join-Path $repositoryRoot "artifacts\setup\packages\$($manifest.releaseLabel)" }
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) { $EvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\setup-packages\$($manifest.releaseLabel)" }
$PortablePackageDirectory = [IO.Path]::GetFullPath($PortablePackageDirectory)
$PortableEvidenceDirectory = [IO.Path]::GetFullPath($PortableEvidenceDirectory)
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)
New-Item -ItemType Directory -Path $OutputDirectory,$EvidenceDirectory -Force | Out-Null

& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration `
    -PackageDirectory $PortablePackageDirectory -EvidenceDirectory $PortableEvidenceDirectory `
    -RequireCandidateEvidence:$RequireCandidateEvidence

$portableNames = @('Compact Cassette Catalogue.exe', 'Compact Cassette Catalogue.exe.config', 'README.txt', 'RELEASE_NOTES.txt', 'BUILD.txt')
$payloadNames = @('BUILD.txt', 'Compact Cassette Catalogue.exe', 'Compact Cassette Catalogue.exe.config', 'README.txt', 'RELEASE_NOTES.txt', 'UNINSTALL.exe', 'UNINSTALL.exe.config')
$fixedTimestamp = [DateTimeOffset]::Parse('2000-01-01T00:00:00Z')
$utf8 = New-Object Text.UTF8Encoding($false)
$checksums = New-Object Collections.Generic.List[String]
$entryChecksums = New-Object Collections.Generic.List[String]
$setRecords = New-Object Collections.Generic.List[Object]

foreach ($lane in $lanes) {
    $portablePath = Join-Path $PortablePackageDirectory ([string]$lane.packageName)
    $portableEntryManifestPath = Join-Path $PortableEvidenceDirectory "$($lane.packageName).entries.json"
    $portableEntryManifest = Get-Content -LiteralPath $portableEntryManifestPath -Raw | ConvertFrom-Json
    $sourceCommit = [string]$portableEntryManifest.sourceCommit
    $lockSha256 = [string]$portableEntryManifest.toolchainLockSha256
    $setupEvidencePath = Join-Path $repositoryRoot "artifacts\evidence\setup-build\$($lane.id)\$Configuration\toolchain.json"
    $setupEvidence = Get-Content -LiteralPath $setupEvidencePath -Raw | ConvertFrom-Json
    if ([string]$setupEvidence.sourceCommit -cne $sourceCommit -or [string]$setupEvidence.toolchainLockSha256 -cne $lockSha256 -or
            [string]$setupEvidence.releaseLabel -cne [string]$manifest.releaseLabel -or [string]$setupEvidence.lane -cne [string]$lane.id) {
        throw "$($lane.id) setup build and portable package do not share one source/release/toolchain lock."
    }
    if ($RequireCandidateEvidence -and [string]$setupEvidence.classification -cne 'Candidate') {
        throw "$($lane.id) setup package requires Candidate setup-build evidence."
    }
    if (-not $RequireCandidateEvidence -and [string]$setupEvidence.classification -notin @('Preparation', 'Candidate')) {
        throw "$($lane.id) setup build has unknown classification '$($setupEvidence.classification)'."
    }

    $setupOutput = Join-Path $repositoryRoot "artifacts\setup\bin\$($lane.id)\$Configuration"
    $installerOutput = Join-Path $setupOutput 'installer\Compact Cassette Catalogue Installer.exe'
    $uninstallerOutput = Join-Path $setupOutput 'uninstaller\Compact Cassette Catalogue Uninstaller.exe'
    $sourceMappings = @(
        @('SETUP.exe', $installerOutput),
        @('SETUP.exe.config', "$installerOutput.config"),
        @('UNINSTALL.exe', $uninstallerOutput),
        @('UNINSTALL.exe.config', "$uninstallerOutput.config")
    )
    foreach ($mapping in $sourceMappings) {
        if (-not (Test-Path -LiteralPath ([string]$mapping[1]) -PathType Leaf)) { throw "$($lane.id) setup source is missing: $($mapping[1])" }
    }

    $portableArchive = [IO.Compression.ZipFile]::OpenRead($portablePath)
    try {
        $payloadBytes = @{}
        foreach ($name in $portableNames) { $payloadBytes[$name] = Get-ZipEntryBytes -Archive $portableArchive -Name $name }
    }
    finally { $portableArchive.Dispose() }
    $payloadBytes['UNINSTALL.exe'] = [IO.File]::ReadAllBytes($uninstallerOutput)
    $payloadBytes['UNINSTALL.exe.config'] = [IO.File]::ReadAllBytes("$uninstallerOutput.config")

    $payloadRecords = @($payloadNames | ForEach-Object {
            [ordered]@{ name = $_; size = [long]$payloadBytes[$_].Length; sha256 = Get-BytesSha256 -Bytes $payloadBytes[$_] }
        })
    $escape = { param([string]$value) [Security.SecurityElement]::Escape($value) }
    $xmlLines = New-Object Collections.Generic.List[String]
    $xmlLines.Add('<?xml version="1.0" encoding="utf-8"?>')
    $xmlLines.Add('<C3SetupPayload schemaVersion="1">')
    $xmlLines.Add(('  <Product version="{0}" stage="{1}" label="{2}" lane="{3}" architecture="{4}" framework="{5}" sourceCommit="{6}" />' -f `
                (& $escape ([string]$manifest.releaseVersion)), (& $escape ([string]$manifest.releaseStage)), (& $escape ([string]$manifest.releaseLabel)),
                (& $escape ([string]$lane.id)), (& $escape ([string]$lane.platformTarget)), (& $escape ([string]$lane.targetFramework)), $sourceCommit))
    $xmlLines.Add('  <Files>')
    foreach ($record in $payloadRecords) {
        $xmlLines.Add(('    <File path="{0}" size="{1}" sha256="{2}" />' -f (& $escape ([string]$record.name)), $record.size, $record.sha256))
    }
    $xmlLines.Add('  </Files>')
    $xmlLines.Add('</C3SetupPayload>')
    $payloadManifestBytes = $utf8.GetBytes(($xmlLines -join "`n") + "`n")

    $rootBytes = @{
        'SETUP.exe' = [IO.File]::ReadAllBytes($installerOutput)
        'SETUP.exe.config' = [IO.File]::ReadAllBytes("$installerOutput.config")
        'payload.xml' = $payloadManifestBytes
    }
    $setupPackagePath = Join-Path $OutputDirectory ([string]$lane.setupPackageName)
    $stream = [IO.File]::Open($setupPackagePath, [IO.FileMode]::Create, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
    $archive = New-Object IO.Compression.ZipArchive($stream, [IO.Compression.ZipArchiveMode]::Create, $false)
    try {
        foreach ($name in @('SETUP.exe', 'SETUP.exe.config', 'payload.xml')) { Add-ZipBytesEntry -Archive $archive -EntryName $name -Bytes $rootBytes[$name] -Timestamp $fixedTimestamp }
        foreach ($name in $payloadNames) { Add-ZipBytesEntry -Archive $archive -EntryName "payload/$name" -Bytes $payloadBytes[$name] -Timestamp $fixedTimestamp }
    }
    finally { $archive.Dispose(); $stream.Dispose() }

    $packageHash = (Get-FileHash -LiteralPath $setupPackagePath -Algorithm SHA256).Hash.ToLowerInvariant()
    $checksums.Add("$packageHash  $($lane.setupPackageName)")
    $archive = [IO.Compression.ZipFile]::OpenRead($setupPackagePath)
    try {
        $entryRecords = @($archive.Entries | ForEach-Object {
                $input = $_.Open()
                try { $hash = Get-StreamSha256 -Stream $input }
                finally { $input.Dispose() }
                [ordered]@{ name = $_.FullName; size = [long]$_.Length; sha256 = $hash }
            })
    }
    finally { $archive.Dispose() }
    $entryManifest = [ordered]@{
        schemaVersion = 1
        packageName = [string]$lane.setupPackageName
        packageSha256 = $packageHash
        portablePackageName = [string]$lane.packageName
        portablePackageSha256 = (Get-FileHash -LiteralPath $portablePath -Algorithm SHA256).Hash.ToLowerInvariant()
        portableEntryManifestSha256 = (Get-FileHash -LiteralPath $portableEntryManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()
        releaseVersion = [string]$manifest.releaseVersion
        releaseStage = [string]$manifest.releaseStage
        releaseLabel = [string]$manifest.releaseLabel
        releaseTag = [string]$manifest.releaseTag
        releaseChannel = [string]$manifest.releaseChannel
        publicationStatus = [string]$manifest.publicationStatus
        lane = [string]$lane.id
        sourceCommit = $sourceCommit
        toolchainMode = [string]$setupEvidence.classification
        toolchainLockSha256 = $lockSha256
        entries = $entryRecords
    }
    $entryManifestName = "$($lane.setupPackageName).entries.json"
    $entryManifestPath = Join-Path $EvidenceDirectory $entryManifestName
    [IO.File]::WriteAllText($entryManifestPath, (($entryManifest | ConvertTo-Json -Depth 8) + "`n"), $utf8)
    $entryManifestHash = (Get-FileHash -LiteralPath $entryManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $entryChecksums.Add("$entryManifestHash  $entryManifestName")
    $setRecords.Add([PSCustomObject]@{
            lane = [string]$lane.id; releaseVersion = [string]$manifest.releaseVersion; releaseStage = [string]$manifest.releaseStage
            releaseLabel = [string]$manifest.releaseLabel; releaseTag = [string]$manifest.releaseTag; releaseChannel = [string]$manifest.releaseChannel
            publicationStatus = [string]$manifest.publicationStatus; sourceCommit = $sourceCommit; toolchainMode = [string]$setupEvidence.classification
            toolchainLockStatus = $(if ([string]$setupEvidence.classification -ceq 'Candidate') { 'locked' } else { 'template' }); toolchainLockSha256 = $lockSha256
        })
    Write-Host "Packaged setup $($lane.id): $setupPackagePath"
}

Assert-C3PackageEvidenceSet -Records @($setRecords.ToArray()) -RequireCandidate:$RequireCandidateEvidence | Out-Null
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'SHA256SUMS.txt'), (($checksums.ToArray() -join "`n") + "`n"), $utf8)
[IO.File]::WriteAllText((Join-Path $EvidenceDirectory 'ENTRY_MANIFEST_SHA256SUMS.txt'), (($entryChecksums.ToArray() -join "`n") + "`n"), $utf8)
Write-Host 'Produced three deterministic offline setup bundles from exact portable payload bytes and verified setup outputs.'
