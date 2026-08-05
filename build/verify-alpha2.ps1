[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('Candidate', 'Tagged')]
    [string]$TagState = 'Candidate',
    [switch]$SkipBuildOutputs,
    [switch]$RunLaunchSmoke,
    [switch]$AllowDirty
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$tagName = 'v1.3.0a2'
$identityBase = '576c6ecb0b65f97899b9abbe4cf84063151091c1'
$developmentBaseline = '58a5b7d21daf19e1b6112d44efb887c7d8ea9500'
$legacyCheckpoint = 'c4115b82ea43fdd763685d862a08fe5c61db6dff'
$alpha1TagObject = '95b530f4f726fb67b3b002b47bf1d4061e71ce3c'
$alpha1TagCommit = '8caa155103879cf41dc6ada753c0927180929059'
$alpha2Record = 'release/validation/1.3.0-alpha.2-preparation-2026-08-05.md'
$headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$expectedBuildSource = $headCommit
$packageSource = $null
if ($TagState -ceq 'Tagged' -and $SkipBuildOutputs) {
    throw 'Tagged Alpha 2 verification requires the retained Candidate build and package evidence.'
}
if ($TagState -ceq 'Tagged') {
    $manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
    $firstEntryManifest = Join-Path $repositoryRoot (
        "artifacts\evidence\packages\{0}\{1}.entries.json" -f
        $manifest.releaseLabel, $manifest.lanes[0].packageName)
    if (-not (Test-Path -LiteralPath $firstEntryManifest -PathType Leaf)) {
        throw "Tagged Alpha 2 verification requires retained package evidence: $firstEntryManifest"
    }
    $packageSource = [string](Get-Content -LiteralPath $firstEntryManifest -Raw | ConvertFrom-Json).sourceCommit
    $expectedBuildSource = $packageSource
}

& (Join-Path $PSScriptRoot 'verify-preparation.ps1') `
    -Configuration $Configuration `
    -SkipBuildOutputs:$SkipBuildOutputs `
    -ExpectedBuildSourceCommit $expectedBuildSource
& (Join-Path $PSScriptRoot 'verify-release-identity.ps1') `
    -ExpectedProductVersion '1.3.0' `
    -ExpectedStage 'Alpha 2' `
    -ExpectedReleaseLabel '1.3.0a2' `
    -ExpectedTag $tagName `
    -ExpectedDate ([datetime]'2026-08-05') `
    -Configuration $Configuration `
    -VerifyBuildOutputs:(-not $SkipBuildOutputs)
& (Join-Path $PSScriptRoot 'test-package-evidence-set.ps1')
& (Join-Path $PSScriptRoot 'test-target-tooling-ps2.ps1')

if ($RunLaunchSmoke -and $SkipBuildOutputs) {
    throw '-RunLaunchSmoke cannot be combined with -SkipBuildOutputs.'
}

& git -C $repositoryRoot merge-base --is-ancestor $developmentBaseline HEAD
if ($LASTEXITCODE -ne 0) {
    throw 'Alpha 2 candidate does not descend from the recovered development baseline.'
}
$resolvedLegacy = (& git -C $repositoryRoot rev-parse legacy/1.x).Trim()
if ($resolvedLegacy -cne $legacyCheckpoint) {
    throw "legacy/1.x moved during Alpha 2 preparation: expected '$legacyCheckpoint', found '$resolvedLegacy'."
}
$resolvedAlpha1Object = (& git -C $repositoryRoot rev-parse refs/tags/v1.3.0a1).Trim()
$resolvedAlpha1Commit = (& git -C $repositoryRoot rev-parse 'v1.3.0a1^{commit}').Trim()
if ($resolvedAlpha1Object -cne $alpha1TagObject -or $resolvedAlpha1Commit -cne $alpha1TagCommit) {
    throw "Immutable Alpha 1 identity changed: $resolvedAlpha1Object / $resolvedAlpha1Commit."
}

$allowedProductionChanges = @(
    'Compact Cassette Catalogue/My Project/AssemblyInfo.vb',
    'Compact Cassette Catalogue/varGlobals.vb'
)
$productionChanges = @(& git -C $repositoryRoot diff --name-only $identityBase -- 'Compact Cassette Catalogue')
$unexpectedProductionChanges = @($productionChanges | Where-Object { $allowedProductionChanges -notcontains $_ })
$missingIdentityChanges = @($allowedProductionChanges | Where-Object { $productionChanges -notcontains $_ })
if ($unexpectedProductionChanges.Count -gt 0) {
    throw "Alpha 2 contains application changes outside the identity projection: $($unexpectedProductionChanges -join ', ')"
}
if ($missingIdentityChanges.Count -gt 0) {
    throw "Alpha 2 is missing source identity projection(s): $($missingIdentityChanges -join ', ')"
}

$tagRef = "refs/tags/$tagName"
& git -C $repositoryRoot show-ref --verify --quiet $tagRef
$tagExists = $LASTEXITCODE -eq 0
if ($TagState -ceq 'Candidate' -and $tagExists) {
    throw "Candidate validation requires absent tag '$tagName'."
}
if ($TagState -ceq 'Tagged') {
    if (-not $tagExists) {
        throw "Tagged validation requires annotated tag '$tagName'."
    }
    if ((& git -C $repositoryRoot cat-file -t $tagRef).Trim() -cne 'tag') {
        throw "'$tagName' is not an annotated tag."
    }
    $tagCommit = (& git -C $repositoryRoot rev-parse "$tagName^{commit}").Trim()
    $headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    if ($tagCommit -cne $headCommit) {
        throw "'$tagName' points to '$tagCommit', expected current HEAD '$headCommit'."
    }
}

if (-not $SkipBuildOutputs) {
    & (Join-Path $PSScriptRoot 'verify-packages.ps1') `
        -Configuration $Configuration `
        -RequireCandidateEvidence
    if ($TagState -ceq 'Candidate') {
        & (Join-Path $PSScriptRoot 'test-package-reproducibility.ps1') `
            -Configuration $Configuration
        & (Join-Path $PSScriptRoot 'test-release-controls.ps1') `
            -Configuration $Configuration
    }
    if ([string]::IsNullOrWhiteSpace($packageSource)) {
        $manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
        $firstEntryManifest = Join-Path $repositoryRoot (
            "artifacts\evidence\packages\{0}\{1}.entries.json" -f
            $manifest.releaseLabel, $manifest.lanes[0].packageName)
        $packageSource = [string](Get-Content -LiteralPath $firstEntryManifest -Raw | ConvertFrom-Json).sourceCommit
    }
    & git -C $repositoryRoot merge-base --is-ancestor $packageSource $headCommit
    if ($LASTEXITCODE -ne 0) {
        throw "Alpha 2 package source '$packageSource' is not an ancestor of HEAD '$headCommit'."
    }
    if ($TagState -ceq 'Candidate' -and $packageSource -cne $headCommit) {
        throw "Candidate Alpha 2 packages must be built from current HEAD '$headCommit', found '$packageSource'."
    }
    if ($TagState -ceq 'Tagged') {
        $postSourceChanges = @(& git -C $repositoryRoot diff --name-only $packageSource $headCommit)
        $unexpectedEvidenceChanges = @($postSourceChanges | Where-Object { $_ -cne $alpha2Record })
        if ($unexpectedEvidenceChanges.Count -gt 0) {
            throw "Tagged Alpha 2 has non-evidence changes after package source: $($unexpectedEvidenceChanges -join ', ')"
        }
    }
    if ($RunLaunchSmoke) {
        & (Join-Path $PSScriptRoot 'smoke-launch.ps1') `
            -Configuration $Configuration `
            -AllowKnownCloseTimeout
    }
}

if (-not $AllowDirty) {
    $status = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
    if ($status.Count -ne 0) {
        throw "Alpha 2 validation requires a clean worktree:`n$($status -join "`n")"
    }
}

$scope = if ($SkipBuildOutputs) { 'source-only preparation' } else { 'Candidate packages and controls' }
Write-Host "C3 1.3.0 Alpha 2 $($TagState.ToLowerInvariant()) verified for $scope; public feed and legacy ledger remain unchanged."
