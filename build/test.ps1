[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$MSBuildPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repositoryRoot 'tests\C3.Characterization\C3.Characterization.vbproj'
$resolveArguments = @{}
if (-not [string]::IsNullOrWhiteSpace($MSBuildPath)) {
    $resolveArguments.MSBuildPath = $MSBuildPath
}
$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1') @resolveArguments

& $msbuild $project '/t:Build' "/p:Configuration=$Configuration" '/p:Platform=AnyCPU' '/v:minimal' '/nologo'
if ($LASTEXITCODE -ne 0) {
    throw "Characterization test build failed with exit code $LASTEXITCODE."
}

$testExecutable = Join-Path $repositoryRoot "artifacts\tests\characterization\$Configuration\C3.CharacterizationTests.exe"
& $testExecutable
if ($LASTEXITCODE -ne 0) {
    throw "Characterization tests failed with exit code $LASTEXITCODE."
}
