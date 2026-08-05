[CmdletBinding()]
param(
    [ValidateSet('15', '17', '18')]
    [string]$Toolset = '15',
    [string]$MSBuildPath,
    [string]$ExpectedProductVersion,
    [string]$ExpectedInstallationVersion,
    [switch]$Detailed,
    [switch]$AllowCompatibleFallback
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if ($AllowCompatibleFallback) {
    throw 'Compatible MSBuild fallback is not permitted by the C3 1.3 three-lane release contract.'
}

$toolsetRanges = @{
    '15' = '[15.0,16.0)'
    '17' = '[17.0,18.0)'
    '18' = '[18.0,19.0)'
}
$range = [string]$toolsetRanges[$Toolset]
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere -PathType Leaf)) {
    throw 'Visual Studio Installer vswhere.exe was not found.'
}

$instance = $null
if ([string]::IsNullOrWhiteSpace($MSBuildPath)) {
    $instanceJson = & $vswhere -latest -products '*' -version $range -requires Microsoft.Component.MSBuild -format json
    if ($LASTEXITCODE -ne 0) {
        throw "vswhere failed while resolving Visual Studio range $range."
    }
    $instances = @($instanceJson | ConvertFrom-Json)
    if ($instances.Count -eq 0) {
        throw "No complete stable Visual Studio installation was found in range $range for toolset '$Toolset'."
    }
    $instance = $instances[0]
    if (-not [bool]$instance.isComplete -or [bool]$instance.isPrerelease) {
        throw "Visual Studio '$($instance.installationPath)' is not a complete stable installation."
    }

    $candidateRelativePaths = if ($Toolset -eq '15') {
        @('MSBuild\15.0\Bin\MSBuild.exe')
    }
    else {
        @('MSBuild\Current\Bin\MSBuild.exe')
    }
    $resolved = $candidateRelativePaths |
        ForEach-Object { Join-Path ([string]$instance.installationPath) $_ } |
        Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } |
        Select-Object -First 1
    if ([string]::IsNullOrWhiteSpace($resolved)) {
        throw "MSBuild was not found below '$($instance.installationPath)'."
    }
}
else {
    if (-not (Test-Path -LiteralPath $MSBuildPath -PathType Leaf)) {
        throw "The requested MSBuild executable does not exist: $MSBuildPath"
    }
    $resolved = [IO.Path]::GetFullPath($MSBuildPath)

    $allJson = & $vswhere -all -products '*' -version $range -requires Microsoft.Component.MSBuild -format json
    if ($LASTEXITCODE -eq 0) {
        $allInstances = @($allJson | ConvertFrom-Json)
        $instance = $allInstances |
            Where-Object { $resolved.StartsWith(([string]$_.installationPath).TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase) } |
            Select-Object -First 1
    }
}

$versionInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($resolved)
$versionMatch = [regex]::Match([string]$versionInfo.ProductVersion, '^(?<major>\d+)')
if (-not $versionMatch.Success -or $versionMatch.Groups['major'].Value -cne $Toolset) {
    throw "MSBuild '$resolved' reports product version '$($versionInfo.ProductVersion)', not required family $Toolset.x."
}

$productVersion = $null
$installationVersion = $null
$installationPath = $null
$displayName = $null
if ($null -ne $instance) {
    $productVersion = [string]$instance.catalog.productDisplayVersion
    $installationVersion = [string]$instance.installationVersion
    $installationPath = [string]$instance.installationPath
    $displayName = [string]$instance.displayName
}

if (-not [string]::IsNullOrWhiteSpace($ExpectedProductVersion) -and
        $productVersion -cne $ExpectedProductVersion) {
    throw "Visual Studio product version '$productVersion' does not match locked version '$ExpectedProductVersion'."
}
if (-not [string]::IsNullOrWhiteSpace($ExpectedInstallationVersion) -and
        $installationVersion -cne $ExpectedInstallationVersion) {
    throw "Visual Studio installation version '$installationVersion' does not match locked version '$ExpectedInstallationVersion'."
}

$details = [PSCustomObject]@{
    toolset = $Toolset
    visualStudioRange = $range
    visualStudioDisplayName = $displayName
    visualStudioProductVersion = $productVersion
    visualStudioInstallationVersion = $installationVersion
    visualStudioInstallationPath = $installationPath
    msbuildPath = [IO.Path]::GetFullPath($resolved)
    msbuildFileVersion = [string]$versionInfo.FileVersion
    msbuildProductVersion = [string]$versionInfo.ProductVersion
}

if ($Detailed) {
    Write-Output $details
}
else {
    Write-Output $details.msbuildPath
}
