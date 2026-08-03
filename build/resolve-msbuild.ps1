[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere -PathType Leaf)) {
    throw 'Visual Studio Installer vswhere.exe was not found.'
}

# Visual Studio 2017 15.9 is the one canonical compiler for release evidence.
# Newer installations remain useful editors but must not silently change output.
$installationPath = & $vswhere `
    -latest `
    -products '*' `
    -version '[15.9,16.0)' `
    -requires Microsoft.Component.MSBuild `
    -property installationPath

if ([string]::IsNullOrWhiteSpace($installationPath)) {
    throw 'Visual Studio 2017 15.9 MSBuild was not found.'
}

$msbuild = Join-Path $installationPath 'MSBuild\Current\Bin\MSBuild.exe'
if (-not (Test-Path -LiteralPath $msbuild -PathType Leaf)) {
    $msbuild = Join-Path $installationPath 'MSBuild\15.0\Bin\MSBuild.exe'
}

if (-not (Test-Path -LiteralPath $msbuild -PathType Leaf)) {
    throw "MSBuild was not found below '$installationPath'."
}

Write-Output $msbuild
