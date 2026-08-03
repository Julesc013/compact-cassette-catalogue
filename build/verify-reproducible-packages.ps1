[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = Join-Path $repositoryRoot 'artifacts'
$reproductionRoot = Join-Path $artifactsRoot 'reproducibility'
$sourceArchive = Join-Path $reproductionRoot 'source.zip'
$firstRoot = Join-Path $reproductionRoot 'candidate-a'
$secondRoot = Join-Path $reproductionRoot 'candidate-with-a-different-path-b'
$retainedPackagesRoot = Join-Path $artifactsRoot 'packages'
$windowsPowerShell = Join-Path $env:SystemRoot 'System32\WindowsPowerShell\v1.0\powershell.exe'

function Assert-SafeArtifactsPath {
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

function Reset-ArtifactsDirectory {
    param([string]$Path)

    Assert-SafeArtifactsPath $Path
    if (Test-Path -LiteralPath $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }
    New-Item -ItemType Directory -Path $Path -Force | Out-Null
}

function Get-PackageSnapshot {
    param([string]$CandidateRoot)

    $packagesRoot = Join-Path $CandidateRoot 'artifacts\packages'
    if (-not (Test-Path -LiteralPath $packagesRoot -PathType Container)) {
        throw "Package output directory is missing: $packagesRoot"
    }

    $files = @(Get-ChildItem -LiteralPath $packagesRoot -File | Sort-Object Name)
    if ($files.Count -ne 3) {
        throw "Expected two portable ZIPs and SHA256SUMS.txt, found $($files.Count) files."
    }
    if (@($files | Where-Object { $_.Extension -ceq '.zip' }).Count -ne 2) {
        throw 'Expected exactly two portable ZIPs.'
    }
    if (@($files | Where-Object { $_.Name -ceq 'SHA256SUMS.txt' }).Count -ne 1) {
        throw 'Expected exactly one SHA256SUMS.txt.'
    }

    return @($files | ForEach-Object {
        [PSCustomObject]@{
            Name = $_.Name
            Length = $_.Length
            Sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
            FullName = $_.FullName
        }
    })
}

function Assert-MatchingSnapshots {
    param(
        [object[]]$First,
        [object[]]$Second,
        [string]$Description
    )

    $leftRecords = @($First)
    $rightRecords = @($Second)
    $mismatches = New-Object Collections.Generic.List[String]
    if ($leftRecords.Count -ne $rightRecords.Count) {
        $mismatches.Add("file count changed from $($leftRecords.Count) to $($rightRecords.Count)")
    }

    $comparisonCount = [Math]::Min($leftRecords.Count, $rightRecords.Count)
    for ($index = 0; $index -lt $comparisonCount; $index++) {
        $left = $leftRecords[$index]
        $right = $rightRecords[$index]
        if ($left.Name -cne $right.Name) {
            $mismatches.Add("file name changed from '$($left.Name)' to '$($right.Name)'")
            continue
        }
        if ($left.Length -ne $right.Length) {
            $mismatches.Add("$($left.Name) length changed from $($left.Length) to $($right.Length)")
        }
        if ($left.Sha256 -cne $right.Sha256) {
            $mismatches.Add("$($left.Name) SHA-256 changed from $($left.Sha256) to $($right.Sha256)")
        }
    }

    if ($mismatches.Count -gt 0) {
        throw "${Description}:`n- $($mismatches -join "`n- ")"
    }
}

function Invoke-PackagePass {
    param(
        [string]$CandidateRoot,
        [string]$Label
    )

    Write-Host "${Label}: full Release rebuild and package in $CandidateRoot"
    $packageScript = Join-Path $CandidateRoot 'build\package.ps1'
    & $windowsPowerShell `
        -NoLogo `
        -NoProfile `
        -NonInteractive `
        -ExecutionPolicy Bypass `
        -File $packageScript
    if ($LASTEXITCODE -ne 0) {
        throw "$Label failed with exit code $LASTEXITCODE."
    }
    return @(Get-PackageSnapshot -CandidateRoot $CandidateRoot)
}

if (-not (Test-Path -LiteralPath $windowsPowerShell -PathType Leaf)) {
    throw 'Windows PowerShell 5.1 was not found.'
}

$gitStatus = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
if ($LASTEXITCODE -ne 0) {
    throw 'Could not inspect the Git worktree.'
}
if ($gitStatus.Count -gt 0) {
    throw "Release reproduction requires a clean committed worktree:`n$($gitStatus -join "`n")"
}

$headCommit = ([string](& git -C $repositoryRoot rev-parse --verify HEAD)).Trim()
if ($LASTEXITCODE -ne 0 -or $headCommit -notmatch '^[0-9a-f]{40}$') {
    throw 'Could not resolve the exact candidate commit.'
}

$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1')
$msbuildVersion = (Get-Item -LiteralPath $msbuild).VersionInfo.FileVersion
$runtimeInfo = [string](& $windowsPowerShell -NoLogo -NoProfile -NonInteractive -Command `
    '$PSVersionTable.PSVersion.ToString() + '' / CLR '' + [Environment]::Version.ToString()')
if ($LASTEXITCODE -ne 0) {
    throw 'Could not inspect the Windows PowerShell release host.'
}

Write-Host "Candidate commit: $headCommit"
Write-Host "MSBuild: $msbuild ($msbuildVersion)"
Write-Host "Packaging host: Windows PowerShell $($runtimeInfo.Trim())"
Write-Host "Operating system: $([Environment]::OSVersion.VersionString)"

Reset-ArtifactsDirectory $reproductionRoot
& git -C $repositoryRoot archive --format=zip "--output=$sourceArchive" $headCommit
if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $sourceArchive -PathType Leaf)) {
    throw 'Could not create the committed source snapshot.'
}

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::ExtractToDirectory($sourceArchive, $firstRoot)
[IO.Compression.ZipFile]::ExtractToDirectory($sourceArchive, $secondRoot)

$first = @(Invoke-PackagePass -CandidateRoot $firstRoot -Label 'Reproducibility pass 1/2')
$second = @(Invoke-PackagePass -CandidateRoot $secondRoot -Label 'Reproducibility pass 2/2')
Assert-MatchingSnapshots $first $second 'Packages are not reproducible across clean source roots'

Reset-ArtifactsDirectory $retainedPackagesRoot
$secondPackagesRoot = Join-Path $secondRoot 'artifacts\packages'
foreach ($file in Get-ChildItem -LiteralPath $secondPackagesRoot -File) {
    Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $retainedPackagesRoot $file.Name)
}
& (Join-Path $PSScriptRoot 'verify-packages.ps1')
$retained = @(Get-PackageSnapshot -CandidateRoot $repositoryRoot)
Assert-MatchingSnapshots $second $retained 'Retained candidate differs from the proven package set'

Write-Host 'Reproducible package evidence:'
foreach ($record in $retained) {
    Write-Host ("{0}  {1,10}  {2}" -f $record.Sha256, $record.Length, $record.Name)
}

Assert-SafeArtifactsPath $reproductionRoot
Remove-Item -LiteralPath $reproductionRoot -Recurse -Force
Write-Host 'Portable packages are byte-identical across two clean, path-distinct Release builds.'
Write-Host "The proven candidate is retained in $retainedPackagesRoot"
