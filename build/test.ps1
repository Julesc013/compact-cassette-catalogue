[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repositoryRoot 'tests\C3.Characterization\C3.Characterization.vbproj'
$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1')

& $msbuild `
    $project `
    '/t:Build' `
    "/p:Configuration=$Configuration" `
    '/p:Platform=AnyCPU' `
    '/m' `
    '/v:minimal' `
    '/nologo'

if ($LASTEXITCODE -ne 0) {
    throw "Characterization test build failed with exit code $LASTEXITCODE."
}

& (Join-Path $PSScriptRoot 'validate-catalogue-api.ps1') `
    -Configuration $Configuration
& (Join-Path $PSScriptRoot 'test-catalogue-csharp-candidate.ps1') `
    -Configuration $Configuration

$testExecutable = Join-Path $repositoryRoot "artifacts\tests\characterization\$Configuration\C3.CharacterizationTests.exe"
& $testExecutable
if ($LASTEXITCODE -ne 0) {
    throw "Characterization tests failed with exit code $LASTEXITCODE."
}
