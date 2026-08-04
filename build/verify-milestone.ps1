[CmdletBinding()]
param(
    [string]$ExpectedMilestone,
    [switch]$Rebuild,
    [switch]$Reproduce
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$trainPath = Join-Path (Split-Path -Parent $PSScriptRoot) `
    'release\train\2.0.0.json'
$train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
if (-not [string]::IsNullOrWhiteSpace($ExpectedMilestone) -and
    [string]$train.currentMilestone -cne $ExpectedMilestone) {
    throw "Expected milestone '$ExpectedMilestone', found '$($train.currentMilestone)'."
}

& (Join-Path $PSScriptRoot 'validate-release-train.ps1')
& (Join-Path $PSScriptRoot 'verify.ps1') -Rebuild:$Rebuild
if ($Reproduce) {
    & (Join-Path $PSScriptRoot 'verify-reproducible-packages.ps1')
}

Write-Host "Milestone '$($train.currentMilestone)' verification passed."
