[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$packagesRoot = Join-Path $repositoryRoot 'artifacts\packages'
$hashPath = Join-Path $packagesRoot 'SHA256SUMS.txt'
if (-not (Test-Path -LiteralPath $hashPath -PathType Leaf)) {
    throw 'SHA256SUMS.txt is missing from artifacts/packages.'
}

$requiredEntries = @(
    'BUILD.txt',
    'C3.Catalogue.dll',
    'C3.Infrastructure.dll',
    'Compact Cassette Catalogue.exe',
    'Compact Cassette Catalogue.exe.config',
    'README.md',
    'RELEASE_NOTES.md'
)

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$hashRecords = @(Get-Content -LiteralPath $hashPath | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
foreach ($record in $hashRecords) {
    if ($record -notmatch '^([0-9a-f]{64})  (.+\.zip)$') {
        throw "Invalid SHA256SUMS record: $record"
    }

    $expectedHash = $matches[1]
    $fileName = $matches[2]
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
        foreach ($requiredEntry in $requiredEntries) {
            if ($entryNames -notcontains $requiredEntry) {
                throw "$fileName is missing required entry '$requiredEntry'."
            }
        }
        if ($entryNames.Count -ne $requiredEntries.Count) {
            throw "$fileName contains unexpected entries: $($entryNames -join ', ')"
        }
    }
    finally {
        $archive.Dispose()
    }

    Write-Host "Package verified: $fileName"
}

if ($hashRecords.Count -ne 2) {
    throw "Expected two portable packages, found $($hashRecords.Count)."
}

Write-Host 'All portable packages and SHA-256 records verified.'

