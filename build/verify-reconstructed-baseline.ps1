[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$SkipBuildOutputs,
    [switch]$AllowDirty
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$runtimeSource = '509c9ec29679e30dcdcb1f57d8874b850cee310c'
$releaseCheckpoint = '2413e9139a098f3321385f2f946e743012a447f5'
$developmentBaseline = '58a5b7d21daf19e1b6112d44efb887c7d8ea9500'
$refactorTip = 'f27c1d0c6798ea68b81ac0b0889ef770ad19d2d9'

& (Join-Path $PSScriptRoot 'verify-preparation.ps1') `
    -Configuration $Configuration `
    -SkipBuildOutputs:$SkipBuildOutputs

$resolvedRelease = (& git -C $repositoryRoot rev-parse 'v1.2.0b1^{commit}').Trim()
if ($resolvedRelease -cne $releaseCheckpoint) {
    throw "v1.2.0b1 resolves to '$resolvedRelease', expected '$releaseCheckpoint'."
}

$postReleaseTag = 'refs/tags/archive/1.2-postrelease-tip'
if ((& git -C $repositoryRoot cat-file -t $postReleaseTag).Trim() -cne 'tag') {
    throw 'archive/1.2-postrelease-tip is missing or is not annotated.'
}
$resolvedDevelopment = (& git -C $repositoryRoot rev-parse "$postReleaseTag^{commit}").Trim()
if ($resolvedDevelopment -cne $developmentBaseline) {
    throw "Post-release archive resolves to '$resolvedDevelopment', expected '$developmentBaseline'."
}

$resolvedRefactor = (& git -C $repositoryRoot rev-parse 'archive/1x-refactor-attempt-2026-08-03^{commit}').Trim()
if ($resolvedRefactor -cne $refactorTip) {
    throw "Refactor archive resolves to '$resolvedRefactor', expected '$refactorTip'."
}

& git -C $repositoryRoot merge-base --is-ancestor $developmentBaseline HEAD
if ($LASTEXITCODE -ne 0) {
    throw "Candidate HEAD does not descend from development baseline $developmentBaseline."
}

foreach ($comparison in @(
        @($runtimeSource, $releaseCheckpoint, 'runtime source to release checkpoint'),
        @($releaseCheckpoint, $developmentBaseline, 'release checkpoint to development baseline'),
        @($runtimeSource, 'HEAD', 'runtime source to reconstructed checkpoint'))) {
    $changes = @(& git -C $repositoryRoot diff --name-only $comparison[0] $comparison[1] -- 'Compact Cassette Catalogue')
    if ($changes.Count -ne 0) {
        throw "Unexpected production difference ($($comparison[2])): $($changes -join ', ')"
    }
}

$versionLines = @(Get-Content -LiteralPath (Join-Path $repositoryRoot 'VERSION'))
if (($versionLines -join "`n") -cne (@('1.2.0', 'Release', '14/05/2026') -join "`n")) {
    throw 'The public VERSION feed changed during baseline reconstruction.'
}

if (-not $AllowDirty) {
    $status = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
    if ($status.Count -ne 0) {
        throw "Baseline qualification requires a clean worktree:`n$($status -join "`n")"
    }
}

Write-Host 'Corrected C3 1.2 development baseline verified: three immutable anchors, 58a ancestry, zero production difference, preparation suite, and feed isolation passed.'
