[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$SkipBuildOutputs,
    [string]$ExpectedBuildSourceCommit
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot 'validate-baseline-genome.ps1')
& (Join-Path $PSScriptRoot 'validate-compatibility-corpus.ps1')
& (Join-Path $PSScriptRoot 'validate-docs.ps1')
& (Join-Path $PSScriptRoot 'validate-lanes.ps1')
& (Join-Path $PSScriptRoot 'validate-setup-manifests.ps1')
& (Join-Path $PSScriptRoot 'validate-setup-offline.ps1')
& (Join-Path $PSScriptRoot 'validate-setup-artwork.ps1')
& (Join-Path $PSScriptRoot 'test.ps1') -Configuration $Configuration
if (-not $SkipBuildOutputs) {
    & (Join-Path $PSScriptRoot 'verify-builds.ps1') `
        -Configuration $Configuration `
        -ExpectedSourceCommit $ExpectedBuildSourceCommit
}

Write-Host 'C3 1.3.0 non-runtime preparation checks passed.'
