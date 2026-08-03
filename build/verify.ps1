[CmdletBinding()]
param(
    [switch]$Rebuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot 'verify-metadata.ps1')
& (Join-Path $PSScriptRoot 'build.ps1') -Configuration Release -Rebuild:$Rebuild

Push-Location (Split-Path -Parent $PSScriptRoot)
try {
    & git diff --check
    if ($LASTEXITCODE -ne 0) {
        throw 'git diff --check failed.'
    }
}
finally {
    Pop-Location
}

Write-Host 'C3 repository verification passed.'

