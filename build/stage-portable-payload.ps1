[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = Join-Path $repositoryRoot 'artifacts'
$stagingRoot = Join-Path $artifactsRoot 'staging'
$profilesRoot = Join-Path $repositoryRoot 'release\profiles'
$payloadPath = Join-Path $profilesRoot 'portable-payload.v1.json'
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$packages = @(& (Join-Path $PSScriptRoot 'get-release-packages.ps1') -Identity $identity)

function Assert-UnderArtifacts {
    param([string]$Path)
    $fullPath = [IO.Path]::GetFullPath($Path)
    $fullRoot = [IO.Path]::GetFullPath($artifactsRoot)
    if (-not $fullPath.StartsWith(
            $fullRoot + [IO.Path]::DirectorySeparatorChar,
            [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to modify a path outside artifacts: $fullPath"
    }
    $current = $fullPath
    while ($true) {
        if (Test-Path -LiteralPath $current) {
            $item = Get-Item -LiteralPath $current -Force
            if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw "Refusing to modify a reparse-point path: $current"
            }
        }
        if ($current.Equals($fullRoot, [StringComparison]::OrdinalIgnoreCase)) { break }
        $parent = [IO.Directory]::GetParent($current)
        if ($null -eq $parent) { throw "Could not validate artifacts ancestry: $fullPath" }
        $current = $parent.FullName
    }
}

& (Join-Path $PSScriptRoot 'validate-distribution-contract.ps1')
Assert-UnderArtifacts $stagingRoot
if (Test-Path -LiteralPath $stagingRoot) {
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}
[IO.Directory]::CreateDirectory($stagingRoot) | Out-Null
$payload = Get-Content -LiteralPath $payloadPath -Raw | ConvertFrom-Json
$results = New-Object Collections.Generic.List[Object]

foreach ($package in $packages) {
    $profilePath = Join-Path $profilesRoot ($package.LaneId + '-portable.toml')
    $profile = & (Join-Path $PSScriptRoot 'read-distribution-profile.ps1') -Path $profilePath
    $archiveRoot = $profile.ArchiveRoot.Replace('{release-label}', $identity.ReleaseLabel).Replace('{lane}', $package.LaneId)
    if ($archiveRoot -cnotmatch '^C3-v[0-9A-Za-z.-]+-win-(?:x86|x64)-net(?:40|48)-portable$') {
        throw "Resolved archive root is not canonical: $archiveRoot"
    }
    $laneStageRoot = Join-Path $stagingRoot $package.LaneId
    $payloadRoot = Join-Path $laneStageRoot $archiveRoot
    Assert-UnderArtifacts $payloadRoot
    [IO.Directory]::CreateDirectory($payloadRoot) | Out-Null

    $configuredOutput = Join-Path (Split-Path -Parent (Join-Path $repositoryRoot $package.OutputDirectory)) $Configuration
    $sourceRoots = @{
        'lane-output' = $configuredOutput
        'cli-output' = Join-Path $repositoryRoot "artifacts\bin\cli\$Configuration"
        'repository' = $repositoryRoot
    }
    foreach ($entry in @($payload.entries | Sort-Object target)) {
        $target = Join-Path $payloadRoot ([string]$entry.target)
        if ([string]$entry.sourceRoot -ceq 'generated') {
            if ([string]$entry.role -cne 'build-metadata' -or [string]$entry.source -cne 'build-metadata') {
                throw "Unsupported generated payload entry: $($entry.role)"
            }
            $buildText = @(
                'Product: Compact Cassette Catalogue (C3)'
                'Product ID: c3'
                "Version: $($identity.ProductVersion)"
                "Stage: $($identity.ReleaseStage)"
                "Lane: $($package.LaneId)"
                "Target framework: $($package.TargetFramework)"
                "Runtime claim: $($package.RuntimeClaim)"
                "Payload profile: $($payload.id)"
            ) -join [Environment]::NewLine
            [IO.File]::WriteAllText(
                $target,
                $buildText + [Environment]::NewLine,
                (New-Object Text.UTF8Encoding($false)))
            continue
        }
        $sourceRoot = $sourceRoots[[string]$entry.sourceRoot]
        if ([string]::IsNullOrWhiteSpace($sourceRoot)) {
            throw "Unsupported payload source root: $($entry.sourceRoot)"
        }
        $source = Join-Path $sourceRoot ([string]$entry.source)
        if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
            throw "Missing payload source for $($package.LaneId)/$($entry.role): $source"
        }
        Copy-Item -LiteralPath $source -Destination $target
    }

    $actualTargets = @(Get-ChildItem -LiteralPath $payloadRoot -File | ForEach-Object { $_.Name } | Sort-Object)
    $expectedTargets = @($payload.entries | ForEach-Object { [string]$_.target } | Sort-Object)
    if (@(Compare-Object $expectedTargets $actualTargets -CaseSensitive).Count -ne 0) {
        throw "Staged payload does not match profile for lane '$($package.LaneId)'."
    }
    $results.Add([PSCustomObject]@{
        LaneId = $package.LaneId
        ArchiveRoot = $archiveRoot
        LaneStageRoot = $laneStageRoot
        PayloadRoot = $payloadRoot
        PackageFileName = $package.FileName
    })
    Write-Host "Staged canonical payload: $($package.LaneId)/$archiveRoot"
}

$results | Sort-Object LaneId
