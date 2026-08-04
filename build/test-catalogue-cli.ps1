[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$tool = Join-Path $repositoryRoot "artifacts\bin\cli\$Configuration\c3.exe"
$source = Join-Path $repositoryRoot 'fixtures\catalogues\v1.1.0\valid\populated.xml'
$invalid = Join-Path $repositoryRoot 'fixtures\catalogues\v1.1.0\invalid\malformed.xml'
$workRoot = Join-Path $repositoryRoot 'artifacts\cli-tests'
if (Test-Path -LiteralPath $workRoot) {
    Remove-Item -LiteralPath $workRoot -Recurse -Force
}
[IO.Directory]::CreateDirectory($workRoot) | Out-Null
$native = Join-Path $workRoot 'converted.c3catalogue'
$legacy = Join-Path $workRoot 'exported-v1.1.xml'

function Invoke-Cli {
    param([string[]]$Arguments, [int]$ExpectedExit)
    $global:LASTEXITCODE = 0
    & $tool @Arguments
    if ($LASTEXITCODE -ne $ExpectedExit) {
        throw "c3 $($Arguments -join ' ') returned $LASTEXITCODE; expected $ExpectedExit."
    }
}

Invoke-Cli -Arguments @('validate', $source) -ExpectedExit 0
Invoke-Cli -Arguments @('migrate', '--dry-run', $source) -ExpectedExit 0
Invoke-Cli -Arguments @('migrate', $source, $native) -ExpectedExit 0
Invoke-Cli -Arguments @('validate', $native) -ExpectedExit 0
Invoke-Cli -Arguments @('export-legacy', $native, $legacy) -ExpectedExit 0
Invoke-Cli -Arguments @('validate', $legacy) -ExpectedExit 0
Invoke-Cli -Arguments @('validate', $invalid) -ExpectedExit 2
Invoke-Cli -Arguments @('migrate', $source, $native) -ExpectedExit 2

Write-Host 'Catalogue CLI tests passed: 8 scenarios.'
$global:LASTEXITCODE = 0
