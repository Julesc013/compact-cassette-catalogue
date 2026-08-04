[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('Candidate', 'Tagged')]
    [string]$TagState = 'Candidate',
    [switch]$SkipBuildOutputs,
    [switch]$AllowDirty
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$tagName = 'v1.3.0a1'
$baseline = '2413e9139a098f3321385f2f946e743012a447f5'
$archiveTip = 'f27c1d0c6798ea68b81ac0b0889ef770ad19d2d9'
$legacyCheckpoint = '99ee814f0632fc1af99610cdf40cee7ca26bb896'

& (Join-Path $PSScriptRoot 'verify-preparation.ps1') `
    -Configuration $Configuration `
    -SkipBuildOutputs:$SkipBuildOutputs
& (Join-Path $PSScriptRoot 'verify-release-identity.ps1') `
    -ExpectedProductVersion '1.3.0' `
    -ExpectedStage 'Alpha 1' `
    -ExpectedDate ([datetime]'2026-08-05') `
    -Configuration $Configuration `
    -VerifyBuildOutputs:(-not $SkipBuildOutputs)
& (Join-Path $PSScriptRoot 'download-baseline-assets.ps1')

$resolvedBaseline = (& git -C $repositoryRoot rev-parse 'v1.2.0b1^{commit}').Trim()
if ($resolvedBaseline -cne $baseline) {
    throw "v1.2.0b1 resolves to '$resolvedBaseline', expected '$baseline'."
}
& git -C $repositoryRoot merge-base --is-ancestor $baseline HEAD
if ($LASTEXITCODE -ne 0) {
    throw 'Alpha 1 candidate does not descend from v1.2.0b1.'
}

$resolvedArchive = (& git -C $repositoryRoot rev-list -n 1 archive/1x-refactor-attempt-2026-08-03).Trim()
if ($resolvedArchive -cne $archiveTip) {
    throw "Archived refactor tip resolves to '$resolvedArchive', expected '$archiveTip'."
}
$resolvedLegacy = (& git -C $repositoryRoot rev-parse legacy/1.x).Trim()
if ($resolvedLegacy -cne $legacyCheckpoint) {
    throw "legacy/1.x moved during Alpha preparation: expected '$legacyCheckpoint', found '$resolvedLegacy'."
}

$allowedProductionChanges = @(
    'Compact Cassette Catalogue/Compact Cassette Catalogue.vbproj',
    'Compact Cassette Catalogue/My Project/AssemblyInfo.vb',
    'Compact Cassette Catalogue/varGlobals.vb'
)
$productionChanges = @(& git -C $repositoryRoot diff --name-only v1.2.0b1 -- 'Compact Cassette Catalogue')
$unexpectedProductionChanges = @($productionChanges | Where-Object { $allowedProductionChanges -notcontains $_ })
$missingIdentityChanges = @($allowedProductionChanges | Where-Object { $productionChanges -notcontains $_ })
if ($unexpectedProductionChanges.Count -gt 0) {
    throw "Alpha 1 contains unexpected production changes: $($unexpectedProductionChanges -join ', ')"
}
if ($missingIdentityChanges.Count -gt 0) {
    throw "Alpha 1 is missing identity projection(s): $($missingIdentityChanges -join ', ')"
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
    $tagType = (& git -C $repositoryRoot cat-file -t $tagRef).Trim()
    if ($tagType -cne 'tag') {
        throw "'$tagName' is not an annotated tag."
    }
    $tagCommit = (& git -C $repositoryRoot rev-parse "$tagName^{commit}").Trim()
    $headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    if ($tagCommit -cne $headCommit) {
        throw "'$tagName' points to '$tagCommit', expected current HEAD '$headCommit'."
    }
}

if (-not $AllowDirty) {
    $status = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
    if ($status.Count -ne 0) {
        throw "Alpha validation requires a clean worktree:`n$($status -join "`n")"
    }
}

Write-Host "C3 1.3.0 Alpha 1 $($TagState.ToLowerInvariant()) verified: ancestry, archive, legacy checkpoint, identity-only production diff, feed isolation, and automated evidence passed."
