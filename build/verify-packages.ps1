[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$packagesRoot = Join-Path $repositoryRoot 'artifacts\packages'
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$packageDefinitions = @(& (Join-Path $PSScriptRoot 'get-release-packages.ps1') -Identity $identity)
$expectedPackageNames = @($packageDefinitions | ForEach-Object { $_.FileName } | Sort-Object)
$hashPath = Join-Path $packagesRoot 'SHA256SUMS.txt'
if (-not (Test-Path -LiteralPath $hashPath -PathType Leaf)) {
    throw 'SHA256SUMS.txt is missing from artifacts/packages.'
}

$requiredEntries = @(
    'BUILD.txt',
    'C3.Catalogue.dll',
    'C3.Domain.dll',
    'C3.Infrastructure.dll',
    'Compact Cassette Catalogue.exe',
    'Compact Cassette Catalogue.exe.config',
    'README.md',
    'RELEASE_NOTES.md'
)

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$hashRecords = @(Get-Content -LiteralPath $hashPath | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
$listedPackages = New-Object Collections.Generic.List[String]
foreach ($record in $hashRecords) {
    if ($record -notmatch '^([0-9a-f]{64})  (.+\.zip)$') {
        throw "Invalid SHA256SUMS record: $record"
    }

    $expectedHash = $matches[1]
    $fileName = $matches[2]
    if ([IO.Path]::GetFileName($fileName) -cne $fileName) {
        throw "Package name must not contain a path: $fileName"
    }
    if (@($listedPackages | Where-Object {
            $_.Equals($fileName, [StringComparison]::OrdinalIgnoreCase)
        }).Count -gt 0) {
        throw "Duplicate package in SHA256SUMS: $fileName"
    }
    $listedPackages.Add($fileName)
    $matchingDefinitions = @($packageDefinitions | Where-Object { $_.FileName -ceq $fileName })
    if ($matchingDefinitions.Count -ne 1) {
        throw "Package is not defined by build/lanes.json and Version.props: $fileName"
    }
    $packageDefinition = $matchingDefinitions[0]
    $packagePath = Join-Path $packagesRoot $fileName
    if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
        throw "Package listed in SHA256SUMS is missing: $fileName"
    }

    $actualHash = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($actualHash -cne $expectedHash) {
        throw "SHA-256 mismatch for $fileName"
    }

    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entryNames = @($archive.Entries | ForEach-Object { $_.FullName })
        if ($entryNames.Count -ne $requiredEntries.Count) {
            throw "$fileName has $($entryNames.Count) entries; expected $($requiredEntries.Count)."
        }
        for ($index = 0; $index -lt $requiredEntries.Count; $index++) {
            if ($entryNames[$index] -cne $requiredEntries[$index]) {
                throw "$fileName entry $index is '$($entryNames[$index])'; expected '$($requiredEntries[$index])'."
            }
        }

        $expectedTimestamp = $identity.ReleaseDate.Date
        foreach ($entry in $archive.Entries) {
            # ZIP stores a DOS wall-clock timestamp without an offset. Compare
            # the encoded wall-clock value, not the reader machine's local zone.
            if ($entry.LastWriteTime.DateTime -ne $expectedTimestamp) {
                throw "$fileName entry '$($entry.FullName)' has a noncanonical timestamp."
            }
        }

        $buildEntry = $archive.GetEntry('BUILD.txt')
        $buildReader = New-Object IO.StreamReader($buildEntry.Open(), [Text.Encoding]::UTF8, $true)
        try {
            $actualBuildText = $buildReader.ReadToEnd().Replace("`r`n", "`n")
        }
        finally {
            $buildReader.Dispose()
        }
        $expectedBuildText = @(
            'Product: Compact Cassette Catalogue (C3)'
            "Version: $($identity.ProductVersion)"
            "Stage: $($identity.ReleaseStage)"
            "Lane: $($packageDefinition.LaneId)"
            "Target framework: $($packageDefinition.TargetFramework)"
            "Runtime claim: $($packageDefinition.RuntimeClaim)"
        ) -join "`n"
        $expectedBuildText += "`n"
        if ($actualBuildText -cne $expectedBuildText) {
            throw "$fileName BUILD.txt does not match the canonical identity and lane contract."
        }
    }
    finally {
        $archive.Dispose()
    }

    Write-Host "Package verified: $fileName"
}

if ($hashRecords.Count -ne $expectedPackageNames.Count) {
    throw "Expected $($expectedPackageNames.Count) portable packages, found $($hashRecords.Count)."
}

$listedDifference = @(Compare-Object $expectedPackageNames @($listedPackages | Sort-Object) -CaseSensitive)
if ($listedDifference.Count -gt 0) {
    throw "SHA256SUMS does not match the lane-defined package set:`n$($listedDifference | Out-String)"
}

$expectedFiles = @($listedPackages) + @('SHA256SUMS.txt') | Sort-Object
$actualFiles = @(Get-ChildItem -LiteralPath $packagesRoot -File |
    ForEach-Object { $_.Name } |
    Sort-Object)
$unexpectedDifference = @(Compare-Object $expectedFiles $actualFiles -CaseSensitive)
if ($unexpectedDifference.Count -gt 0) {
    throw "Package directory contains a missing or unexpected file:`n$($unexpectedDifference | Out-String)"
}

Write-Host 'All portable packages and SHA-256 records verified.'
