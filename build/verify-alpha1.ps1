[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('Candidate', 'CorrectionCandidate', 'Tagged')]
    [string]$TagState = 'Candidate',
    [switch]$SkipBuildOutputs,
    [switch]$RunLaunchSmoke,
    [switch]$AllowDirty
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$tagName = 'v1.3.0a1'
$runtimeSource = '509c9ec29679e30dcdcb1f57d8874b850cee310c'
$releaseCheckpoint = '2413e9139a098f3321385f2f946e743012a447f5'
$developmentBaseline = '58a5b7d21daf19e1b6112d44efb887c7d8ea9500'
$archiveTip = 'f27c1d0c6798ea68b81ac0b0889ef770ad19d2d9'
$legacyCheckpoint = 'c4115b82ea43fdd763685d862a08fe5c61db6dff'
$originalTagObject = 'ac723441f6391177e2885b53837cd394ceed8f48'
$originalTagCommit = 'dad42b2b76c3f469b20416b2788317e630913ae1'
$originalTagArchive = 'refs/tags/archive/v1.3.0a1-original-tag-object-2026-08-05'

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
if ($RunLaunchSmoke) {
    if ($SkipBuildOutputs) {
        throw '-RunLaunchSmoke cannot be combined with -SkipBuildOutputs.'
    }
    & (Join-Path $PSScriptRoot 'smoke-launch.ps1') -Configuration $Configuration
}

$resolvedBaseline = (& git -C $repositoryRoot rev-parse 'v1.2.0b1^{commit}').Trim()
if ($resolvedBaseline -cne $releaseCheckpoint) {
    throw "v1.2.0b1 resolves to '$resolvedBaseline', expected '$releaseCheckpoint'."
}
& git -C $repositoryRoot merge-base --is-ancestor $developmentBaseline HEAD
if ($LASTEXITCODE -ne 0) {
    throw 'Alpha 1 candidate does not descend from the 58a5b7d development baseline.'
}

$resolvedArchive = (& git -C $repositoryRoot rev-list -n 1 archive/1x-refactor-attempt-2026-08-03).Trim()
if ($resolvedArchive -cne $archiveTip) {
    throw "Archived refactor tip resolves to '$resolvedArchive', expected '$archiveTip'."
}
$postReleaseArchive = 'refs/tags/archive/1.2-postrelease-tip'
if ((& git -C $repositoryRoot cat-file -t $postReleaseArchive).Trim() -cne 'tag') {
    throw 'archive/1.2-postrelease-tip is missing or is not annotated.'
}
$resolvedDevelopment = (& git -C $repositoryRoot rev-parse "$postReleaseArchive^{commit}").Trim()
if ($resolvedDevelopment -cne $developmentBaseline) {
    throw "Post-release archive resolves to '$resolvedDevelopment', expected '$developmentBaseline'."
}
$resolvedLegacy = (& git -C $repositoryRoot rev-parse legacy/1.x).Trim()
if ($resolvedLegacy -cne $legacyCheckpoint) {
    throw "legacy/1.x moved during Alpha preparation: expected '$legacyCheckpoint', found '$resolvedLegacy'."
}
if ((& git -C $repositoryRoot cat-file -t $originalTagArchive).Trim() -cne 'tag') {
    throw 'The original Alpha tag-object archive is missing or is not annotated.'
}
$archiveHeader = @(& git -C $repositoryRoot cat-file -p $originalTagArchive | Select-Object -First 1)
if ($archiveHeader.Count -ne 1 -or $archiveHeader[0] -cne "object $originalTagObject") {
    throw "The original Alpha archive does not preserve tag object '$originalTagObject'."
}

$allowedProductionChanges = @(
    'Compact Cassette Catalogue/Compact Cassette Catalogue.vbproj',
    'Compact Cassette Catalogue/My Project/AssemblyInfo.vb',
    'Compact Cassette Catalogue/varGlobals.vb'
)
$productionChanges = @(& git -C $repositoryRoot diff --name-only $runtimeSource HEAD -- 'Compact Cassette Catalogue')
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
if ($TagState -ceq 'CorrectionCandidate') {
    if (-not $tagExists) {
        throw "Correction-candidate validation requires the original '$tagName' tag."
    }
    $currentTagObject = (& git -C $repositoryRoot rev-parse $tagRef).Trim()
    $currentTagCommit = (& git -C $repositoryRoot rev-parse "$tagName^{commit}").Trim()
    if ($currentTagObject -cne $originalTagObject -or $currentTagCommit -cne $originalTagCommit) {
        throw "Correction candidate found unexpected existing tag object/commit: $currentTagObject / $currentTagCommit."
    }
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
