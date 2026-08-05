[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release',
    [switch]$SkipBuildOutputs,
    [string]$CandidateRoot,
    [string]$ToolchainLockPath,
    [switch]$VerifyIdentityTransition,
    [string]$IdentityCommit
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-contract.ps1')
. (Join-Path $PSScriptRoot 'beta1-identity-transition.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
[void](Assert-C3Beta1ManifestPath -Path (Join-Path $PSScriptRoot 'lanes.json'))
$headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw 'Beta 1 verification requires clean frozen C-beta.' }
if ((& git -C $repositoryRoot rev-parse refs/tags/v1.3.0a2).Trim() -cne '0e8633671e55bbb7ce5e692f8e0f5c4201a62627' -or
        (& git -C $repositoryRoot rev-parse 'v1.3.0a2^{commit}').Trim() -cne '0aad46c6ad1d241caa70ceb74ccfe9bbfa12165b') {
    throw 'Immutable Alpha 2 tag object or target changed.'
}
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0a3
if ($LASTEXITCODE -eq 0) { throw 'Superseded Alpha 3 must remain untagged before Beta 1.' }
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0b1
if ($LASTEXITCODE -eq 0) { throw 'Beta 1 source/Candidate verification requires absent v1.3.0b1.' }
if ((& git -C $repositoryRoot rev-parse legacy/1.x).Trim() -cne 'c4115b82ea43fdd763685d862a08fe5c61db6dff') {
    throw 'legacy/1.x moved before complete Beta GO.'
}

if ($VerifyIdentityTransition) {
    if ([string]::IsNullOrWhiteSpace($IdentityCommit)) { throw '-VerifyIdentityTransition requires explicit -IdentityCommit.' }
    [void](Assert-C3Beta1IdentityTransition -RepositoryRoot $repositoryRoot -IdentityCommit $IdentityCommit -HeadCommit $headCommit)
}

& (Join-Path $PSScriptRoot 'verify-preparation.ps1') -Configuration $Configuration -SkipBuildOutputs:$SkipBuildOutputs
& (Join-Path $PSScriptRoot 'verify-release-identity.ps1') -ExpectedProductVersion '1.3.0' -ExpectedStage 'Beta 1' `
    -ExpectedReleaseLabel '1.3.0b1' -ExpectedTag 'v1.3.0b1' -ExpectedChannel beta -ExpectedDate ([datetime]'2026-08-06') `
    -Configuration $Configuration -VerifyBuildOutputs:(-not $SkipBuildOutputs)
& (Join-Path $PSScriptRoot 'test-beta1-assets.ps1')
& (Join-Path $PSScriptRoot 'test-beta1-verdict.ps1')
& (Join-Path $PSScriptRoot 'test-beta1-tag-message.ps1')
& (Join-Path $PSScriptRoot 'test-beta1-topology.ps1')

if (-not $SkipBuildOutputs) {
    if ([string]::IsNullOrWhiteSpace($CandidateRoot) -or [string]::IsNullOrWhiteSpace($ToolchainLockPath)) {
        throw 'Full Beta 1 Candidate verification requires -CandidateRoot and -ToolchainLockPath.'
    }
    & (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'verify-setup-builds.ps1') -Configuration $Configuration -ExpectedSourceCommit $headCommit
    & (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'verify-beta1-assets.ps1') -CandidateRoot $CandidateRoot -ExpectedSourceCommit $headCommit `
        -ExpectedToolchainLockSha256 (Get-FileHash -LiteralPath $ToolchainLockPath -Algorithm SHA256).Hash.ToLowerInvariant()
    & (Join-Path $PSScriptRoot 'test-source-reproducibility.ps1') -Configuration $Configuration `
        -ToolchainLockPath $ToolchainLockPath -IncludeSetup
}

Write-Host 'C3 1.3.0 Beta 1 source/Candidate controls passed; this does not itself authorize a tag, ledger movement, or publication.'
