[CmdletBinding()]
param(
    [switch]$SkipBuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = Join-Path $repositoryRoot 'artifacts'
$stagingRoot = Join-Path $artifactsRoot 'staging'
$packagesRoot = Join-Path $artifactsRoot 'packages'

function Assert-UnderArtifacts {
    param([string]$Path)
    $fullPath = [IO.Path]::GetFullPath($Path)
    $fullArtifacts = [IO.Path]::GetFullPath($artifactsRoot) + [IO.Path]::DirectorySeparatorChar
    if (-not $fullPath.StartsWith($fullArtifacts, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to modify a path outside artifacts: $fullPath"
    }
}

if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'build.ps1') -Configuration Release -Rebuild
}

[xml]$versionProps = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'Version.props') -Raw
$versionValues = $versionProps.Project.PropertyGroup
$productVersion = [string]$versionValues.C3ProductVersion
$releaseStage = [string]$versionValues.C3ReleaseStage
$releaseDate = [DateTime]::ParseExact(
    [string]$versionValues.C3ReleaseDate,
    'yyyy-MM-dd',
    [Globalization.CultureInfo]::InvariantCulture)
$versionLabel = $productVersion
if (-not [string]::Equals($releaseStage, 'Release', [StringComparison]::OrdinalIgnoreCase)) {
    $stageSlug = ($releaseStage.Trim().ToLowerInvariant() -replace '[^a-z0-9]+', '.').Trim('.')
    $versionLabel += '-' + $stageSlug
}

$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json

foreach ($path in @($stagingRoot, $packagesRoot)) {
    Assert-UnderArtifacts $path
    if (Test-Path -LiteralPath $path) {
        Remove-Item -LiteralPath $path -Recurse -Force
    }
    New-Item -ItemType Directory -Path $path -Force | Out-Null
}

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$fixedTimestamp = New-Object DateTimeOffset(
    $releaseDate.Year,
    $releaseDate.Month,
    $releaseDate.Day,
    0,
    0,
    0,
    [TimeSpan]::Zero)
$packagePaths = New-Object Collections.Generic.List[String]

foreach ($lane in @($manifest.lanes)) {
    if ([string]$lane.distribution -ne 'portable') {
        continue
    }

    $laneId = [string]$lane.id
    $outputDirectory = Join-Path $repositoryRoot ([string]$lane.outputDirectory)
    $stageDirectory = Join-Path $stagingRoot $laneId
    Assert-UnderArtifacts $stageDirectory
    New-Item -ItemType Directory -Path $stageDirectory -Force | Out-Null

    $payload = @(
        @{ Source = Join-Path $outputDirectory 'Compact Cassette Catalogue.exe'; Name = 'Compact Cassette Catalogue.exe' },
        @{ Source = Join-Path $outputDirectory 'Compact Cassette Catalogue.exe.config'; Name = 'Compact Cassette Catalogue.exe.config' },
        @{ Source = Join-Path $outputDirectory 'C3.Catalogue.dll'; Name = 'C3.Catalogue.dll' },
        @{ Source = Join-Path $outputDirectory 'C3.Infrastructure.dll'; Name = 'C3.Infrastructure.dll' },
        @{ Source = Join-Path $repositoryRoot 'README.md'; Name = 'README.md' },
        @{ Source = Join-Path $repositoryRoot 'RELEASE_NOTES.md'; Name = 'RELEASE_NOTES.md' }
    )

    foreach ($item in $payload) {
        if (-not (Test-Path -LiteralPath $item.Source -PathType Leaf)) {
            throw "Missing package input for ${laneId}: $($item.Source)"
        }
        Copy-Item -LiteralPath $item.Source -Destination (Join-Path $stageDirectory $item.Name)
    }

    $buildText = @(
        'Product: Compact Cassette Catalogue (C3)'
        "Version: $productVersion"
        "Stage: $releaseStage"
        "Lane: $laneId"
        "Target framework: $($lane.targetFramework)"
        "Runtime claim: $($lane.runtimeClaim)"
    ) -join [Environment]::NewLine
    [IO.File]::WriteAllText(
        (Join-Path $stageDirectory 'BUILD.txt'),
        $buildText + [Environment]::NewLine,
        (New-Object Text.UTF8Encoding($false)))

    $packageName = "C3-v$versionLabel-$laneId-portable.zip"
    $packagePath = Join-Path $packagesRoot $packageName
    $stream = [IO.File]::Open($packagePath, [IO.FileMode]::CreateNew, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
    $archive = New-Object IO.Compression.ZipArchive($stream, [IO.Compression.ZipArchiveMode]::Create, $false)
    try {
        foreach ($file in Get-ChildItem -LiteralPath $stageDirectory -File | Sort-Object Name) {
            $entry = $archive.CreateEntry($file.Name, [IO.Compression.CompressionLevel]::Optimal)
            $entry.LastWriteTime = $fixedTimestamp
            $entryStream = $entry.Open()
            $sourceStream = [IO.File]::OpenRead($file.FullName)
            try {
                $sourceStream.CopyTo($entryStream)
            }
            finally {
                $sourceStream.Dispose()
                $entryStream.Dispose()
            }
        }
    }
    finally {
        $archive.Dispose()
        $stream.Dispose()
    }

    $packagePaths.Add($packagePath)
    Write-Host "Packaged $packageName"
}

$hashLines = @($packagePaths | Sort-Object | ForEach-Object {
    $hash = (Get-FileHash -LiteralPath $_ -Algorithm SHA256).Hash.ToLowerInvariant()
    "$hash  $([IO.Path]::GetFileName($_))"
})
$hashPath = Join-Path $packagesRoot 'SHA256SUMS.txt'
[IO.File]::WriteAllText(
    $hashPath,
    ($hashLines -join [Environment]::NewLine) + [Environment]::NewLine,
    (New-Object Text.UTF8Encoding($false)))

& (Join-Path $PSScriptRoot 'verify-packages.ps1')

