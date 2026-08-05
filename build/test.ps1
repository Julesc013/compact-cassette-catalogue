[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$MSBuildPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projects = @(
    [PSCustomObject]@{
        Name = 'Characterization'
        Project = Join-Path $repositoryRoot 'tests\C3.Characterization\C3.Characterization.vbproj'
        Executable = Join-Path $repositoryRoot "artifacts\tests\characterization\$Configuration\C3.CharacterizationTests.exe"
    },
    [PSCustomObject]@{
        Name = 'Integrity characterization'
        Project = Join-Path $repositoryRoot 'tests\C3.Integrity.Characterization\C3.Integrity.Characterization.vbproj'
        Executable = Join-Path $repositoryRoot "artifacts\tests\integrity\$Configuration\C3.IntegrityCharacterization.exe"
    },
    [PSCustomObject]@{
        Name = 'Settings characterization'
        Project = Join-Path $repositoryRoot 'tests\C3.Settings.Characterization\C3.Settings.Characterization.vbproj'
        Executable = Join-Path $repositoryRoot "artifacts\tests\settings\$Configuration\C3.SettingsCharacterizationTests.exe"
    },
    [PSCustomObject]@{
        Name = 'Setup characterization'
        Project = Join-Path $repositoryRoot 'tests\C3.Setup.Characterization\C3.Setup.Characterization.vbproj'
        Executable = Join-Path $repositoryRoot "artifacts\tests\setup\$Configuration\C3.SetupTests.exe"
    }
)
$resolveArguments = @{}
if (-not [string]::IsNullOrWhiteSpace($MSBuildPath)) {
    $resolveArguments.MSBuildPath = $MSBuildPath
}
$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1') @resolveArguments

foreach ($test in $projects) {
    & $msbuild $test.Project '/t:Build' "/p:Configuration=$Configuration" '/p:Platform=AnyCPU' '/v:minimal' '/nologo'
    if ($LASTEXITCODE -ne 0) {
        throw "$($test.Name) test build failed with exit code $LASTEXITCODE."
    }
    & $test.Executable
    if ($LASTEXITCODE -ne 0) {
        throw "$($test.Name) tests failed with exit code $LASTEXITCODE."
    }
}
