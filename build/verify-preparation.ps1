[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$SkipBuildOutputs
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot 'validate-baseline-genome.ps1')
& (Join-Path $PSScriptRoot 'validate-compatibility-corpus.ps1')
& (Join-Path $PSScriptRoot 'test.ps1') -Configuration $Configuration
if (-not $SkipBuildOutputs) {
    & (Join-Path $PSScriptRoot 'verify-builds.ps1') -Configuration $Configuration
}

Write-Host 'C3 1.3.0 non-runtime preparation checks passed.'
