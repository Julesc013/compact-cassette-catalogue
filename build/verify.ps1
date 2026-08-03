[CmdletBinding()]
param(
    [switch]$Rebuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot 'verify-metadata.ps1')
& (Join-Path $PSScriptRoot 'test-json-validator.ps1')
& (Join-Path $PSScriptRoot 'validate-release-contract.ps1')
& (Join-Path $PSScriptRoot 'test-release-contract.ps1')
& (Join-Path $PSScriptRoot 'test-release-ref-transaction.ps1')
& (Join-Path $PSScriptRoot 'test-trusted-release-target.ps1')
& (Join-Path $PSScriptRoot 'test-update-feed-contract.ps1')
& (Join-Path $PSScriptRoot 'validate-build-contract.ps1')
& (Join-Path $PSScriptRoot 'validate-dependencies.ps1')
& (Join-Path $PSScriptRoot 'validate-ui-boundaries.ps1')
& (Join-Path $PSScriptRoot 'validate-project-parity.ps1')
& (Join-Path $PSScriptRoot 'validate-docs.ps1')
& (Join-Path $PSScriptRoot 'test-doc-validation.ps1')
& (Join-Path $PSScriptRoot 'test.ps1') -Configuration Release
& (Join-Path $PSScriptRoot 'build.ps1') -Configuration Release -Rebuild:$Rebuild
& (Join-Path $PSScriptRoot 'verify-binary-metadata.ps1') -Configuration Release
& (Join-Path $PSScriptRoot 'verify-pe.ps1') -Configuration Release

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$gitMetadataPath = Join-Path $repositoryRoot '.git'
if (Test-Path -LiteralPath $gitMetadataPath) {
    Push-Location $repositoryRoot
    try {
        & git diff --check
        if ($LASTEXITCODE -ne 0) {
            throw 'git diff --check failed.'
        }
    }
    finally {
        Pop-Location
    }
}
else {
    Write-Host 'Git metadata is unavailable; skipped worktree diff validation for this source snapshot.'
}

Write-Host 'C3 repository verification passed.'
