[CmdletBinding()]
param(
    [string]$TagName,
    [switch]$RequireArtifacts
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot 'validate-release-train.ps1')
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
if ([string]::IsNullOrWhiteSpace($TagName)) {
    $TagName = [string]$identity.TagName
}
if ($TagName -cne [string]$identity.TagName) {
    throw "Tag '$TagName' does not match current identity '$($identity.TagName)'."
}

& (Join-Path $PSScriptRoot 'validate-release-contract.ps1') `
    -Mode Tag `
    -TagName $TagName `
    -RequireArtifacts:$RequireArtifacts

Write-Host "Qualified annotated tag verified: $TagName."
