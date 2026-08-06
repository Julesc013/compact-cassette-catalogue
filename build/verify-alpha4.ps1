[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release',
    [switch]$SkipBuildOutputs,
    [string]$ToolchainLockPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha4-contract.ps1')
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
Assert-C3Alpha4Manifest -Manifest $manifest
if ((& git -C $repositoryRoot rev-parse legacy/1.x).Trim() -cne 'c4115b82ea43fdd763685d862a08fe5c61db6dff') { throw 'legacy/1.x moved before Alpha 4.' }
foreach ($forbiddenTag in @('v1.3.0a3', 'v1.3.0a4', 'v1.3.0b1')) {
    & git -C $repositoryRoot show-ref --verify --quiet "refs/tags/$forbiddenTag"
    if ($LASTEXITCODE -eq 0) { throw "Alpha 4 source verification requires absent tag '$forbiddenTag'." }
}
& (Join-Path $PSScriptRoot 'verify-preparation.ps1') -Configuration $Configuration -SkipBuildOutputs:$SkipBuildOutputs
& (Join-Path $PSScriptRoot 'verify-release-identity.ps1') -ExpectedProductVersion '1.3.0' -ExpectedStage 'Alpha 4' `
    -ExpectedReleaseLabel '1.3.0a4' -ExpectedTag 'v1.3.0a4' -ExpectedChannel alpha -ExpectedDate ([datetime]'2026-08-06') `
    -Configuration $Configuration -VerifyBuildOutputs:(-not $SkipBuildOutputs)
& (Join-Path $PSScriptRoot 'test-alpha4-tag-message.ps1')
& (Join-Path $PSScriptRoot 'test-package-evidence-set.ps1')
& (Join-Path $PSScriptRoot 'test-source-reproducibility.ps1') -SelfTest
& (Join-Path $PSScriptRoot 'test-target-tooling-ps2.ps1')
if (-not $SkipBuildOutputs) {
    if ([string]::IsNullOrWhiteSpace($ToolchainLockPath)) { throw 'Full Alpha 4 verification requires -ToolchainLockPath.' }
    & (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'verify-setup-builds.ps1') -Configuration $Configuration
    & (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'verify-alpha3-assets.ps1') -Configuration $Configuration -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'test-source-reproducibility.ps1') -Configuration $Configuration -ToolchainLockPath $ToolchainLockPath -IncludeSetup
}
Write-Host 'C3 1.3.0 Alpha 4 source/distribution controls passed; no publication, Beta, feed, or legacy claim is implied.'
