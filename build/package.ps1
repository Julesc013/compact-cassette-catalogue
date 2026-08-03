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
    $fullArtifactsRoot = [IO.Path]::GetFullPath($artifactsRoot)
    $fullArtifactsPrefix = $fullArtifactsRoot + [IO.Path]::DirectorySeparatorChar
    if (-not $fullPath.StartsWith($fullArtifactsPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to modify a path outside artifacts: $fullPath"
    }

    $currentPath = $fullPath
    while ($true) {
        if (Test-Path -LiteralPath $currentPath) {
            $item = Get-Item -LiteralPath $currentPath -Force
            if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw "Refusing to modify a reparse-point path: $currentPath"
            }
        }
        if ($currentPath.Equals($fullArtifactsRoot, [StringComparison]::OrdinalIgnoreCase)) {
            break
        }
        $parent = [IO.Directory]::GetParent($currentPath)
        if ($null -eq $parent) {
            throw "Could not validate artifacts ancestry for: $fullPath"
        }
        $currentPath = $parent.FullName
    }
}

if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'build.ps1') -Configuration Release -Rebuild
}

& (Join-Path $PSScriptRoot 'verify-binary-metadata.ps1') -Configuration Release

$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$productVersion = $identity.ProductVersion
$releaseStage = $identity.ReleaseStage
$releaseDate = $identity.ReleaseDate
if ($releaseDate.Year -lt 1980 -or $releaseDate.Year -gt 2107) {
    throw "Release date $($releaseDate.ToString('yyyy-MM-dd')) is outside the ZIP timestamp range."
}
$versionLabel = $identity.ReleaseLabel

$packageDefinitions = @(& (Join-Path $PSScriptRoot 'get-release-packages.ps1') -Identity $identity)

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

foreach ($packageDefinition in $packageDefinitions) {
    $laneId = $packageDefinition.LaneId
    $outputDirectory = Join-Path $repositoryRoot $packageDefinition.OutputDirectory
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
        "Target framework: $($packageDefinition.TargetFramework)"
        "Runtime claim: $($packageDefinition.RuntimeClaim)"
    ) -join [Environment]::NewLine
    [IO.File]::WriteAllText(
        (Join-Path $stageDirectory 'BUILD.txt'),
        $buildText + [Environment]::NewLine,
        (New-Object Text.UTF8Encoding($false)))

    $packageName = $packageDefinition.FileName
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
