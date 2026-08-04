[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$SkipBuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'test.ps1') -Configuration $Configuration
}

$testExecutable = Join-Path $repositoryRoot `
    "artifacts\tests\characterization\$Configuration\C3.CharacterizationTests.exe"
if (-not (Test-Path -LiteralPath $testExecutable -PathType Leaf)) {
    throw "Characterization executable is missing: $testExecutable"
}

$output = & $testExecutable '--measure-brand-workspace'
if ($LASTEXITCODE -ne 0) {
    throw "Brand workspace measurement failed with exit code $LASTEXITCODE."
}

$line = @($output | Where-Object {
        $_ -match '^BRAND_WORKSPACE_PERFORMANCE\|brands=676\|iterations=20\|'
    })
if ($line.Count -ne 1) {
    throw 'Brand workspace measurement did not return its one canonical result line.'
}

Write-Host $line[0]
