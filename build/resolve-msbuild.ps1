[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere -PathType Leaf)) {
    throw 'Visual Studio Installer vswhere.exe was not found.'
}

# Visual Studio 2017 and 2019 can build the legacy .NET Framework 4.0 project.
# Visual Studio 2022 and newer are intentionally excluded from this resolver.
$installationPath = & $vswhere `
    -latest `
    -products '*' `
    -version '[15.0,17.0)' `
    -requires Microsoft.Component.MSBuild `
    -property installationPath

if ([string]::IsNullOrWhiteSpace($installationPath)) {
    throw 'No compatible Visual Studio 2017 or 2019 MSBuild installation was found.'
}

$msbuild = Join-Path $installationPath 'MSBuild\Current\Bin\MSBuild.exe'
if (-not (Test-Path -LiteralPath $msbuild -PathType Leaf)) {
    $msbuild = Join-Path $installationPath 'MSBuild\15.0\Bin\MSBuild.exe'
}

if (-not (Test-Path -LiteralPath $msbuild -PathType Leaf)) {
    throw "MSBuild was not found below '$installationPath'."
}

Write-Output $msbuild

