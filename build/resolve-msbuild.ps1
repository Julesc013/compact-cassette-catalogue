[CmdletBinding()]
param(
    [ValidateSet('14', '15', '17')]
    [string]$Toolset,
    [string]$MSBuildPath,
    [switch]$AllowCompatibleFallback
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if (-not [string]::IsNullOrWhiteSpace($MSBuildPath)) {
    if (-not (Test-Path -LiteralPath $MSBuildPath -PathType Leaf)) {
        throw "The requested MSBuild executable does not exist: $MSBuildPath"
    }
    Write-Output ([IO.Path]::GetFullPath($MSBuildPath))
    return
}

$msbuild14 = Join-Path ${env:ProgramFiles(x86)} 'MSBuild\14.0\Bin\MSBuild.exe'
if ($Toolset -eq '14' -and (Test-Path -LiteralPath $msbuild14 -PathType Leaf)) {
    Write-Output $msbuild14
    return
}

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere -PathType Leaf)) {
    throw 'Visual Studio Installer vswhere.exe was not found.'
}

$range = switch ($Toolset) {
    '14' { if ($AllowCompatibleFallback) { '[15.0,17.0)' } else { $null } }
    '15' { '[15.0,17.0)' }
    '17' { '[17.0,18.0)' }
    default { '[15.0,18.0)' }
}

if ($Toolset -eq '14' -and $null -eq $range) {
    throw 'Authoritative net40 builds require VS2015/MSBuild 14. Use -AllowCompatibleFallback only for diagnostic builds.'
}

$installationPath = & $vswhere -latest -products '*' -version $range -requires Microsoft.Component.MSBuild -property installationPath
if ([string]::IsNullOrWhiteSpace($installationPath)) {
    throw "No compatible Visual Studio installation was found for toolset '$Toolset'."
}

$candidates = @(
    (Join-Path $installationPath 'MSBuild\Current\Bin\MSBuild.exe'),
    (Join-Path $installationPath 'MSBuild\15.0\Bin\MSBuild.exe')
)
$resolved = $candidates | Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } | Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($resolved)) {
    throw "MSBuild was not found below '$installationPath'."
}

if ($Toolset -eq '14') {
    Write-Warning "Using '$resolved' as a diagnostic fallback. This does not satisfy the authoritative MSBuild 14 gate."
}
Write-Output $resolved
