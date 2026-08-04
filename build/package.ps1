[CmdletBinding()]
param(
    [switch]$SkipBuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = Join-Path $repositoryRoot 'artifacts'
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

Assert-UnderArtifacts $packagesRoot
if (Test-Path -LiteralPath $packagesRoot) {
    Remove-Item -LiteralPath $packagesRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $packagesRoot -Force | Out-Null

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
$stagedPayloads = @(& (Join-Path $PSScriptRoot 'stage-portable-payload.ps1') -Configuration Release)

foreach ($packageDefinition in $packageDefinitions) {
    $laneId = $packageDefinition.LaneId
    $staged = @($stagedPayloads | Where-Object { $_.LaneId -ceq $laneId })
    if ($staged.Count -ne 1) {
        throw "Expected one staged payload for lane '$laneId'."
    }
    $stageDirectory = $staged[0].LaneStageRoot

    $packageName = $packageDefinition.FileName
    $packagePath = Join-Path $packagesRoot $packageName
    $stream = [IO.File]::Open($packagePath, [IO.FileMode]::CreateNew, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
    $archive = New-Object IO.Compression.ZipArchive($stream, [IO.Compression.ZipArchiveMode]::Create, $false)
    try {
        $files = @(Get-ChildItem -LiteralPath $stageDirectory -File -Recurse | ForEach-Object {
                [PSCustomObject]@{
                    File = $_
                    EntryName = $_.FullName.Substring($stageDirectory.Length + 1).Replace('\', '/')
                }
            } | Sort-Object EntryName)
        foreach ($item in $files) {
            $file = $item.File
            $entry = $archive.CreateEntry($item.EntryName, [IO.Compression.CompressionLevel]::Optimal)
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
