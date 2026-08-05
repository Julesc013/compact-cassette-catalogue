[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$SkipBuildOutputs,
    [switch]$IdentityProjectionOnly
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$tagName = 'v1.3.0a3'
$alpha2PostTagCommit = 'a566b05014c5bac472be307c4a8328657bc47a6d'
$alpha2TagObject = '0e8633671e55bbb7ce5e692f8e0f5c4201a62627'
$alpha2TagCommit = '0aad46c6ad1d241caa70ceb74ccfe9bbfa12165b'
$legacyCheckpoint = 'c4115b82ea43fdd763685d862a08fe5c61db6dff'

& git -C $repositoryRoot merge-base --is-ancestor $alpha2PostTagCommit HEAD
if ($LASTEXITCODE -ne 0) {
    throw "Alpha 3 does not descend from Alpha 2 post-tag commit '$alpha2PostTagCommit'."
}

$resolvedAlpha2Object = (& git -C $repositoryRoot rev-parse refs/tags/v1.3.0a2).Trim()
$resolvedAlpha2Commit = (& git -C $repositoryRoot rev-parse 'v1.3.0a2^{commit}').Trim()
if ($resolvedAlpha2Object -cne $alpha2TagObject -or $resolvedAlpha2Commit -cne $alpha2TagCommit) {
    throw "Immutable Alpha 2 identity changed: $resolvedAlpha2Object / $resolvedAlpha2Commit."
}

$resolvedLegacy = (& git -C $repositoryRoot rev-parse legacy/1.x).Trim()
if ($resolvedLegacy -cne $legacyCheckpoint) {
    throw "legacy/1.x moved during Alpha 3 work: expected '$legacyCheckpoint', found '$resolvedLegacy'."
}

& git -C $repositoryRoot show-ref --verify --quiet "refs/tags/$tagName"
if ($LASTEXITCODE -eq 0) {
    throw "Alpha 3 source/candidate verification requires absent tag '$tagName'."
}

& (Join-Path $PSScriptRoot 'validate-setup-genome.ps1')

if ($IdentityProjectionOnly) {
    $allowedProductionChanges = @(
        'Compact Cassette Catalogue/My Project/AssemblyInfo.vb',
        'Compact Cassette Catalogue/varGlobals.vb'
    )
    $productionChanges = @(& git -C $repositoryRoot diff --name-only $alpha2PostTagCommit -- 'Compact Cassette Catalogue')
    $unexpectedProductionChanges = @($productionChanges | Where-Object { $allowedProductionChanges -notcontains $_ })
    $missingIdentityChanges = @($allowedProductionChanges | Where-Object { $productionChanges -notcontains $_ })
    if ($unexpectedProductionChanges.Count -gt 0) {
        throw "Alpha 3 identity tranche contains application changes outside identity projection: $($unexpectedProductionChanges -join ', ')"
    }
    if ($missingIdentityChanges.Count -gt 0) {
        throw "Alpha 3 identity tranche is missing projection(s): $($missingIdentityChanges -join ', ')"
    }
}

& (Join-Path $PSScriptRoot 'verify-preparation.ps1') `
    -Configuration $Configuration `
    -SkipBuildOutputs:$SkipBuildOutputs
& (Join-Path $PSScriptRoot 'verify-release-identity.ps1') `
    -ExpectedProductVersion '1.3.0' `
    -ExpectedStage 'Alpha 3' `
    -ExpectedReleaseLabel '1.3.0a3' `
    -ExpectedTag $tagName `
    -ExpectedDate ([datetime]'2026-08-05') `
    -Configuration $Configuration `
    -VerifyBuildOutputs:(-not $SkipBuildOutputs)
& (Join-Path $PSScriptRoot 'test-package-evidence-set.ps1')
& (Join-Path $PSScriptRoot 'test-source-reproducibility.ps1') -SelfTest
& (Join-Path $PSScriptRoot 'test-target-tooling-ps2.ps1')

if (-not $SkipBuildOutputs) {
    & (Join-Path $PSScriptRoot 'verify-packages.ps1') `
        -Configuration $Configuration `
        -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'verify-setup-builds.ps1') `
        -Configuration $Configuration
}

Write-Host 'C3 1.3.0 Alpha 3 source/candidate controls passed; no Alpha 3 tag, publication, feed, or legacy movement is claimed.'
