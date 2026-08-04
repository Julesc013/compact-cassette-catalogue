[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$ContractPath,
    [string]$SchemaPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$arguments = @{ RepositoryRoot = $RepositoryRoot }
if (-not [string]::IsNullOrWhiteSpace($ContractPath)) {
    $arguments.ContractPath = $ContractPath
}
if (-not [string]::IsNullOrWhiteSpace($SchemaPath)) {
    $arguments.SchemaPath = $SchemaPath
}
$contract = & (Join-Path $PSScriptRoot 'get-branch-contract.ps1') @arguments

Write-Host (
    'Permanent branch contract verified: ' +
    "$($contract.CurrentQualified), $($contract.CurrentIntegration), " +
    "$($contract.LegacyQualified), $($contract.LegacyIntegration).")
