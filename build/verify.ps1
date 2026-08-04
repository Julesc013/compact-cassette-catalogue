[CmdletBinding()]
param(
    [switch]$Rebuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Invoke-RepositoryScript {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,
        [hashtable]$Parameters = @{}
    )

    # Run every component with the same native-exit baseline as an independent
    # GitHub Actions PowerShell step, then reject a leaked failure code locally.
    $global:LASTEXITCODE = 0
    & (Join-Path $PSScriptRoot $Name) @Parameters
    if ($global:LASTEXITCODE -ne 0) {
        throw "$Name completed with native exit code $global:LASTEXITCODE."
    }
}

Invoke-RepositoryScript 'verify-metadata.ps1'
Invoke-RepositoryScript 'test-json-validator.ps1'
Invoke-RepositoryScript 'validate-branch-contract.ps1'
Invoke-RepositoryScript 'test-branch-contract.ps1'
Invoke-RepositoryScript 'validate-compatibility-corpus.ps1'
Invoke-RepositoryScript 'test-compatibility-corpus.ps1'
Invoke-RepositoryScript 'validate-release-train.ps1'
Invoke-RepositoryScript 'test-release-train.ps1'
Invoke-RepositoryScript 'validate-release-contract.ps1'
Invoke-RepositoryScript 'test-release-contract.ps1'
Invoke-RepositoryScript 'test-release-ref-transaction.ps1'
Invoke-RepositoryScript 'test-trusted-release-target.ps1'
Invoke-RepositoryScript 'test-update-feed-contract.ps1'
Invoke-RepositoryScript 'validate-workflow-contract.ps1'
Invoke-RepositoryScript 'validate-build-contract.ps1'
Invoke-RepositoryScript 'validate-dependencies.ps1'
Invoke-RepositoryScript 'validate-ui-boundaries.ps1'
Invoke-RepositoryScript 'validate-project-parity.ps1'
Invoke-RepositoryScript 'validate-docs.ps1'
Invoke-RepositoryScript 'test-doc-validation.ps1'
Invoke-RepositoryScript 'test.ps1' @{ Configuration = 'Release' }
Invoke-RepositoryScript 'build.ps1' @{
    Configuration = 'Release'
    Rebuild = [bool]$Rebuild
}
Invoke-RepositoryScript 'verify-binary-metadata.ps1' @{ Configuration = 'Release' }
Invoke-RepositoryScript 'verify-pe.ps1' @{ Configuration = 'Release' }

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
