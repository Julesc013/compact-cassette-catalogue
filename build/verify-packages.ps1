[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$PackageDirectory
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if ([string]::IsNullOrWhiteSpace($PackageDirectory)) {
    $PackageDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseVersion)"
}
$PackageDirectory = [IO.Path]::GetFullPath($PackageDirectory)
$expectedAssetNames = @($lanes.packageName) + @('SHA256SUMS.txt')
$actualAssetNames = @(Get-ChildItem -LiteralPath $PackageDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualAssetNames -join "`n") -cne (($expectedAssetNames | Sort-Object) -join "`n")) {
    throw "Package directory must contain exactly the three ZIPs and SHA256SUMS.txt. Found: $($actualAssetNames -join ', ')"
}

$checksumLines = @(Get-Content -LiteralPath (Join-Path $PackageDirectory 'SHA256SUMS.txt'))
if ($checksumLines.Count -ne $lanes.Count) {
    throw "SHA256SUMS.txt contains $($checksumLines.Count) lines; expected $($lanes.Count)."
}
$expectedEntries = @(
    'Compact Cassette Catalogue.exe',
    'Compact Cassette Catalogue.exe.config',
    'README.txt',
    'RELEASE_NOTES.txt',
    'BUILD.txt'
)

for ($index = 0; $index -lt $lanes.Count; $index++) {
    $lane = $lanes[$index]
    $packagePath = Join-Path $PackageDirectory ([string]$lane.packageName)
    $packageHash = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash.ToLowerInvariant()
    $expectedChecksumLine = "$packageHash  $($lane.packageName)"
    if ($checksumLines[$index] -cne $expectedChecksumLine) {
        throw "Checksum line $($index + 1) does not match '$expectedChecksumLine'."
    }

    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entryNames = @($archive.Entries | ForEach-Object { $_.FullName })
        if (($entryNames -join "`n") -cne ($expectedEntries -join "`n")) {
            throw "$($lane.id) ZIP payload differs from the exact portable allow-list: $($entryNames -join ', ')"
        }
        foreach ($entry in $archive.Entries) {
            if ($entry.LastWriteTime.DateTime -ne [DateTime]::new(2000, 1, 1, 0, 0, 0, [DateTimeKind]::Unspecified)) {
                throw "$($lane.id) entry '$($entry.FullName)' has a non-deterministic timestamp '$($entry.LastWriteTime)'."
            }
            if ($entry.FullName -match '(?i)(\.dll$|\.msi$|\.msix$|\.application$|setup\.exe$|uninstall|bootstrap|clickonce|updater?)') {
                throw "$($lane.id) contains prohibited release content '$($entry.FullName)'."
            }
        }

        foreach ($mapping in @(
                @('Compact Cassette Catalogue.exe', (Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\Compact Cassette Catalogue.exe")),
                @('Compact Cassette Catalogue.exe.config', (Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\Compact Cassette Catalogue.exe.config")))) {
            $entry = $archive.GetEntry([string]$mapping[0])
            $entryStream = $entry.Open()
            $algorithm = [Security.Cryptography.SHA256]::Create()
            try {
                $entryHash = ([BitConverter]::ToString($algorithm.ComputeHash($entryStream))).Replace('-', '').ToLowerInvariant()
            }
            finally {
                $algorithm.Dispose()
                $entryStream.Dispose()
            }
            $sourceHash = (Get-FileHash -LiteralPath ([string]$mapping[1]) -Algorithm SHA256).Hash.ToLowerInvariant()
            if ($entryHash -cne $sourceHash) {
                throw "$($lane.id) ZIP entry '$($mapping[0])' does not match its verified build output."
            }
        }

        $buildEntry = $archive.GetEntry('BUILD.txt')
        $reader = New-Object IO.StreamReader($buildEntry.Open(), (New-Object Text.UTF8Encoding($false)), $true)
        try {
            $buildData = ConvertFrom-StringData ($reader.ReadToEnd())
        }
        finally {
            $reader.Dispose()
        }
        if ([string]$buildData.lane -cne [string]$lane.id -or
                [string]$buildData.targetFramework -cne [string]$lane.targetFramework -or
                [string]$buildData.peMachine -cne [string]$lane.peMachine -or
                [string]$buildData.runtimeDllCount -cne '0' -or
                [string]$buildData.distribution -cne 'portable-classic-winforms') {
            throw "$($lane.id) BUILD.txt does not match its lane and portable payload contract."
        }
    }
    finally {
        $archive.Dispose()
    }
    Write-Host "Verified portable package: $($lane.packageName) ($packageHash)"
}

Write-Host 'Verified exactly three deterministic portable ZIPs, checksum manifest, matching EXE/config bytes, and prohibited-output exclusion.'
