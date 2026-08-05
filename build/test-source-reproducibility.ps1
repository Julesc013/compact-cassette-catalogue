[CmdletBinding()]
param(
    [string]$ToolchainLockPath,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$IncludeSetup,
    [switch]$SelfTest
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Get-FileHashMap {
    param(
        [Parameter(Mandatory = $true)][string]$Directory,
        [Parameter(Mandatory = $true)][string[]]$Names
    )

    $map = [ordered]@{}
    foreach ($name in $Names) {
        $path = Join-Path $Directory $name
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Reproducibility input is missing '$path'."
        }
        $map[$name] = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    return $map
}

function Assert-HashMapsEqual {
    param(
        [Parameter(Mandatory = $true)]$Expected,
        [Parameter(Mandatory = $true)]$Actual,
        [Parameter(Mandatory = $true)][string]$Context
    )

    if (($Expected.Keys -join "`n") -cne ($Actual.Keys -join "`n")) {
        throw "$Context contains a different file-name set."
    }
    foreach ($name in $Expected.Keys) {
        if ([string]$Expected[$name] -cne [string]$Actual[$name]) {
            throw "$Context differs for '$name': $($Expected[$name]) / $($Actual[$name])"
        }
    }
}

if ($SelfTest) {
    $left = [ordered]@{ 'a.bin' = 'aa'; 'b.bin' = 'bb' }
    $right = [ordered]@{ 'a.bin' = 'aa'; 'b.bin' = 'bb' }
    Assert-HashMapsEqual -Expected $left -Actual $right -Context 'self-test equal maps'
    $right['b.bin'] = 'cc'
    try {
        Assert-HashMapsEqual -Expected $left -Actual $right -Context 'self-test differing maps'
        throw 'Source reproducibility self-test accepted differing hashes.'
    }
    catch {
        if ($_.Exception.Message -notmatch "differs for 'b.bin'") { throw }
    }
    Write-Host 'Source reproducibility helper self-test passed equal and differing hash-map cases.'
    return
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) {
    throw 'Source reproducibility requires a clean controlling worktree.'
}
if ([string]::IsNullOrWhiteSpace($ToolchainLockPath) -or -not [IO.Path]::IsPathRooted($ToolchainLockPath)) {
    throw '-ToolchainLockPath must be an absolute external lock path.'
}
$ToolchainLockPath = [IO.Path]::GetFullPath($ToolchainLockPath)
if (-not (Test-Path -LiteralPath $ToolchainLockPath -PathType Leaf)) {
    throw "External toolchain lock is missing: $ToolchainLockPath"
}
$repositoryPrefix = [IO.Path]::GetFullPath($repositoryRoot).TrimEnd('\') + '\'
if ($ToolchainLockPath.StartsWith($repositoryPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Source reproducibility requires the immutable lock to remain outside the repository.'
}
$lock = Get-Content -LiteralPath $ToolchainLockPath -Raw | ConvertFrom-Json
if ([string]$lock.status -cne 'locked' -or [string]$lock.sourceCommit -cne $sourceCommit) {
    throw "External lock is not locked to current source '$sourceCommit'."
}
$lockHash = (Get-FileHash -LiteralPath $ToolchainLockPath -Algorithm SHA256).Hash.ToLowerInvariant()

$authoritativePackages = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)"
$authoritativeEntryEvidence = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)"
& (Join-Path $PSScriptRoot 'verify-packages.ps1') `
    -Configuration $Configuration `
    -PackageDirectory $authoritativePackages `
    -EvidenceDirectory $authoritativeEntryEvidence `
    -RequireCandidateEvidence
if ($IncludeSetup) {
    & (Join-Path $PSScriptRoot 'verify-setup-builds.ps1') -Configuration $Configuration -ExpectedSourceCommit $sourceCommit
    & (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'verify-alpha3-assets.ps1') -Configuration $Configuration -RequireCandidateEvidence
}

$artifactRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts'))
$temporaryParent = [IO.Path]::GetFullPath((Join-Path ([IO.Path]::GetTempPath()) 'c3-source-reproducibility'))
$worktreeRoot = [IO.Path]::GetFullPath((Join-Path $temporaryParent ([Guid]::NewGuid().ToString('N'))))
$retainedRoot = [IO.Path]::GetFullPath((Join-Path $artifactRoot "evidence\source-reproducibility\$($manifest.releaseLabel)"))
$artifactPrefix = $artifactRoot.TrimEnd('\') + '\'
if (-not $retainedRoot.StartsWith($artifactPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Retained reproducibility path escaped artifacts: $retainedRoot"
}
$temporaryPrefix = $temporaryParent.TrimEnd('\') + '\'
if (-not $worktreeRoot.StartsWith($temporaryPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Temporary reproducibility worktree path escaped its dedicated root: $worktreeRoot"
}
foreach ($governedPath in @($worktreeRoot, $retainedRoot)) {
    if (Test-Path -LiteralPath $governedPath) {
        throw "Stale reproducibility path exists; remove it through governed cleanup before retrying: $governedPath"
    }
}
New-Item -ItemType Directory -Path $worktreeRoot -Force | Out-Null

$runs = @(
    [PSCustomObject]@{ name = 'path-a'; path = (Join-Path $worktreeRoot 'path-a') },
    [PSCustomObject]@{ name = 'different-absolute-path-b'; path = (Join-Path $worktreeRoot 'different-absolute-path-b') }
)
$addedWorktrees = New-Object Collections.Generic.List[String]
try {
    foreach ($run in $runs) {
        & git -C $repositoryRoot worktree add --detach $run.path $sourceCommit
        if ($LASTEXITCODE -ne 0) {
            throw "Could not create clean detached reproducibility worktree '$($run.path)'."
        }
        $addedWorktrees.Add([string]$run.path)

        $runBuild = Join-Path $run.path 'build\build.ps1'
        & $runBuild `
            -Configuration $Configuration `
            -ToolchainMode Candidate `
            -ToolchainLockPath $ToolchainLockPath `
            -Rebuild
        & (Join-Path $run.path 'build\package.ps1') `
            -Configuration $Configuration `
            -RequireCandidateEvidence
        & (Join-Path $run.path 'build\verify-packages.ps1') `
            -Configuration $Configuration `
            -RequireCandidateEvidence
        if ($IncludeSetup) {
            & (Join-Path $run.path 'build\build-setup.ps1') `
                -Configuration $Configuration `
                -ToolchainMode Candidate `
                -ToolchainLockPath $ToolchainLockPath
            & (Join-Path $run.path 'build\verify-setup-builds.ps1') `
                -Configuration $Configuration `
                -ExpectedSourceCommit $sourceCommit
            & (Join-Path $run.path 'build\package-setup.ps1') `
                -Configuration $Configuration `
                -RequireCandidateEvidence
            & (Join-Path $run.path 'build\verify-setup-packages.ps1') `
                -Configuration $Configuration `
                -RequireCandidateEvidence
            & (Join-Path $run.path 'build\assemble-alpha3-assets.ps1') `
                -Configuration $Configuration `
                -RequireCandidateEvidence
            & (Join-Path $run.path 'build\verify-alpha3-assets.ps1') `
                -Configuration $Configuration `
                -RequireCandidateEvidence
        }
    }

    $assetNames = @($manifest.lanes | ForEach-Object { [string]$_.packageName }) + @('SHA256SUMS.txt')
    $entryEvidenceNames = @($manifest.lanes | ForEach-Object { "$($_.packageName).entries.json" }) + @('ENTRY_MANIFEST_SHA256SUMS.txt')
    $authoritativeAssetMap = Get-FileHashMap -Directory $authoritativePackages -Names $assetNames
    $authoritativeEntryMap = Get-FileHashMap -Directory $authoritativeEntryEvidence -Names $entryEvidenceNames
    $setupAssetNames = @()
    $setupEntryEvidenceNames = @()
    $distributionAssetNames = @()
    $releaseAssetEvidenceNames = @()
    $authoritativeSetupAssetMap = [ordered]@{}
    $authoritativeSetupEntryMap = [ordered]@{}
    $authoritativeDistributionMap = [ordered]@{}
    $authoritativeReleaseAssetEvidenceMap = [ordered]@{}
    if ($IncludeSetup) {
        $setupAssetNames = @($manifest.lanes | ForEach-Object { [string]$_.setupPackageName }) + @('SHA256SUMS.txt')
        $setupEntryEvidenceNames = @($manifest.lanes | ForEach-Object { "$($_.setupPackageName).entries.json" }) + @('ENTRY_MANIFEST_SHA256SUMS.txt')
        $distributionAssetNames = @($manifest.lanes | ForEach-Object { [string]$_.packageName }) +
            @($manifest.lanes | ForEach-Object { [string]$_.setupPackageName }) + @('SHA256SUMS.txt')
        $releaseAssetEvidenceNames = @('release-assets.json', 'RELEASE_ASSETS_SHA256.txt')
        $authoritativeSetupAssetMap = Get-FileHashMap -Directory (Join-Path $repositoryRoot "artifacts\setup\packages\$($manifest.releaseLabel)") -Names $setupAssetNames
        $authoritativeSetupEntryMap = Get-FileHashMap -Directory (Join-Path $repositoryRoot "artifacts\evidence\setup-packages\$($manifest.releaseLabel)") -Names $setupEntryEvidenceNames
        $authoritativeDistributionMap = Get-FileHashMap -Directory (Join-Path $repositoryRoot "artifacts\distributions\$($manifest.releaseLabel)") -Names $distributionAssetNames
        $authoritativeReleaseAssetEvidenceMap = Get-FileHashMap -Directory (Join-Path $repositoryRoot "artifacts\evidence\release-assets\$($manifest.releaseLabel)") -Names $releaseAssetEvidenceNames
    }
    $runSummaries = New-Object Collections.Generic.List[Object]
    foreach ($run in $runs) {
        $runPackages = Join-Path $run.path "artifacts\packages\$($manifest.releaseLabel)"
        $runEntryEvidence = Join-Path $run.path "artifacts\evidence\packages\$($manifest.releaseLabel)"
        $assetMap = Get-FileHashMap -Directory $runPackages -Names $assetNames
        $entryMap = Get-FileHashMap -Directory $runEntryEvidence -Names $entryEvidenceNames
        Assert-HashMapsEqual -Expected $authoritativeAssetMap -Actual $assetMap -Context "$($run.name) package set"
        Assert-HashMapsEqual -Expected $authoritativeEntryMap -Actual $entryMap -Context "$($run.name) entry-manifest set"

        $setupAssetMap = [ordered]@{}
        $setupEntryMap = [ordered]@{}
        $distributionMap = [ordered]@{}
        $releaseAssetEvidenceMap = [ordered]@{}
        if ($IncludeSetup) {
            $setupAssetMap = Get-FileHashMap -Directory (Join-Path $run.path "artifacts\setup\packages\$($manifest.releaseLabel)") -Names $setupAssetNames
            $setupEntryMap = Get-FileHashMap -Directory (Join-Path $run.path "artifacts\evidence\setup-packages\$($manifest.releaseLabel)") -Names $setupEntryEvidenceNames
            $distributionMap = Get-FileHashMap -Directory (Join-Path $run.path "artifacts\distributions\$($manifest.releaseLabel)") -Names $distributionAssetNames
            $releaseAssetEvidenceMap = Get-FileHashMap -Directory (Join-Path $run.path "artifacts\evidence\release-assets\$($manifest.releaseLabel)") -Names $releaseAssetEvidenceNames
            Assert-HashMapsEqual -Expected $authoritativeSetupAssetMap -Actual $setupAssetMap -Context "$($run.name) setup package set"
            Assert-HashMapsEqual -Expected $authoritativeSetupEntryMap -Actual $setupEntryMap -Context "$($run.name) setup entry-manifest set"
            Assert-HashMapsEqual -Expected $authoritativeDistributionMap -Actual $distributionMap -Context "$($run.name) six-asset distribution"
            Assert-HashMapsEqual -Expected $authoritativeReleaseAssetEvidenceMap -Actual $releaseAssetEvidenceMap -Context "$($run.name) release-asset evidence"
        }

        foreach ($lane in @($manifest.lanes)) {
            foreach ($fileName in @('Compact Cassette Catalogue.exe', 'Compact Cassette Catalogue.exe.config')) {
                $authoritativeOutput = Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\$fileName"
                $runOutput = Join-Path $run.path "artifacts\bin\$($lane.id)\$Configuration\$fileName"
                $authoritativeHash = (Get-FileHash -LiteralPath $authoritativeOutput -Algorithm SHA256).Hash.ToLowerInvariant()
                $runHash = (Get-FileHash -LiteralPath $runOutput -Algorithm SHA256).Hash.ToLowerInvariant()
                if ($authoritativeHash -cne $runHash) {
                    throw "$($run.name) source build differs for $($lane.id)/$fileName."
                }
            }
            if ($IncludeSetup) {
                foreach ($setupOutput in @(
                        'installer\Compact Cassette Catalogue Installer.exe',
                        'installer\Compact Cassette Catalogue Installer.exe.config',
                        'uninstaller\Compact Cassette Catalogue Uninstaller.exe',
                        'uninstaller\Compact Cassette Catalogue Uninstaller.exe.config')) {
                    $authoritativeOutput = Join-Path $repositoryRoot "artifacts\setup\bin\$($lane.id)\$Configuration\$setupOutput"
                    $runOutput = Join-Path $run.path "artifacts\setup\bin\$($lane.id)\$Configuration\$setupOutput"
                    $authoritativeHash = (Get-FileHash -LiteralPath $authoritativeOutput -Algorithm SHA256).Hash.ToLowerInvariant()
                    $runHash = (Get-FileHash -LiteralPath $runOutput -Algorithm SHA256).Hash.ToLowerInvariant()
                    if ($authoritativeHash -cne $runHash) {
                        throw "$($run.name) source build differs for setup $($lane.id)/$setupOutput."
                    }
                }
            }
        }

        $retainedRun = Join-Path $retainedRoot $run.name
        New-Item -ItemType Directory -Path $retainedRun -Force | Out-Null
        Copy-Item -LiteralPath (Join-Path $run.path 'artifacts\evidence\build') -Destination (Join-Path $retainedRun 'build') -Recurse
        Copy-Item -LiteralPath $runPackages -Destination (Join-Path $retainedRun 'packages') -Recurse
        Copy-Item -LiteralPath $runEntryEvidence -Destination (Join-Path $retainedRun 'package-evidence') -Recurse
        if ($IncludeSetup) {
            Copy-Item -LiteralPath (Join-Path $run.path 'artifacts\evidence\setup-build') -Destination (Join-Path $retainedRun 'setup-build') -Recurse
            Copy-Item -LiteralPath (Join-Path $run.path "artifacts\setup\packages\$($manifest.releaseLabel)") -Destination (Join-Path $retainedRun 'setup-packages') -Recurse
            Copy-Item -LiteralPath (Join-Path $run.path "artifacts\evidence\setup-packages\$($manifest.releaseLabel)") -Destination (Join-Path $retainedRun 'setup-package-evidence') -Recurse
            Copy-Item -LiteralPath (Join-Path $run.path "artifacts\distributions\$($manifest.releaseLabel)") -Destination (Join-Path $retainedRun 'distribution') -Recurse
            Copy-Item -LiteralPath (Join-Path $run.path "artifacts\evidence\release-assets\$($manifest.releaseLabel)") -Destination (Join-Path $retainedRun 'release-asset-evidence') -Recurse
        }
        $runSummaries.Add([ordered]@{
                name = $run.name
                sourcePath = [string]$run.path
                retainedEvidence = $retainedRun
                packages = $assetMap
                entryEvidence = $entryMap
                setupPackages = $setupAssetMap
                setupEntryEvidence = $setupEntryMap
                distribution = $distributionMap
                releaseAssetEvidence = $releaseAssetEvidenceMap
            })
    }

    $summary = [ordered]@{
        schemaVersion = 1
        status = 'pass'
        releaseLabel = [string]$manifest.releaseLabel
        sourceCommit = $sourceCommit
        toolchainLockSha256 = $lockHash
        configuration = $Configuration
        includesSetup = [bool]$IncludeSetup
        pathDistinct = ([string]$runs[0].path -cne [string]$runs[1].path)
        authoritativePackages = $authoritativeAssetMap
        authoritativeEntryEvidence = $authoritativeEntryMap
        runs = @($runSummaries.ToArray())
        recordedAtUtc = [DateTime]::UtcNow.ToString('o')
    }
    $summaryPath = Join-Path $retainedRoot 'source-reproducibility.json'
    $summaryJson = ($summary | ConvertTo-Json -Depth 10) + "`n"
    [IO.File]::WriteAllText($summaryPath, $summaryJson, (New-Object Text.UTF8Encoding($false)))
    $reproducedScope = if ($IncludeSetup) { 'portable/setup binaries, six packages, entry manifests, and assembled distribution' } else { 'portable package and entry-manifest bytes' }
    Write-Host "Two clean path-distinct Candidate source builds reproduced the authoritative $reproducedScope. Evidence: $summaryPath"
}
finally {
    for ($index = $addedWorktrees.Count - 1; $index -ge 0; $index--) {
        & git -C $repositoryRoot worktree remove --force $addedWorktrees[$index]
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "Could not remove temporary reproducibility worktree '$($addedWorktrees[$index])'."
        }
    }
    & git -C $repositoryRoot worktree prune
    if ((Test-Path -LiteralPath $worktreeRoot -PathType Container) -and
            @(Get-ChildItem -LiteralPath $worktreeRoot -Force).Count -eq 0) {
        Remove-Item -LiteralPath $worktreeRoot -Force
    }
}
