[CmdletBinding()]
param(
    [string]$Lane,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal',
    [string]$MSBuildPath,
    [switch]$AllowCompatibleFallback,
    [switch]$Rebuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if (-not [string]::IsNullOrWhiteSpace($Lane)) {
    $lanes = @($lanes | Where-Object { $_.id -ceq $Lane })
    if ($lanes.Count -eq 0) {
        $available = @($manifest.lanes | ForEach-Object { $_.id }) -join ', '
        throw "Unknown build lane '$Lane'. Available lanes: $available"
    }
}

$projectPath = Join-Path $repositoryRoot ([string]$manifest.sourceProject)
if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
    throw "The original C3 project was not found: $projectPath"
}

$target = if ($Rebuild) { 'Rebuild' } else { 'Build' }
foreach ($buildLane in $lanes) {
    $resolveArguments = @{
        Toolset = [string]$buildLane.toolset
        AllowCompatibleFallback = $AllowCompatibleFallback
    }
    if (-not [string]::IsNullOrWhiteSpace($MSBuildPath)) {
        $resolveArguments.MSBuildPath = $MSBuildPath
    }
    $msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1') @resolveArguments

    $outputPath = Join-Path $repositoryRoot "artifacts\bin\$($buildLane.id)\$Configuration"
    $intermediatePath = Join-Path $repositoryRoot "artifacts\obj\$($buildLane.id)\$Configuration"
    $appConfigPath = Join-Path $repositoryRoot ([string]$buildLane.appConfig)
    if (-not (Test-Path -LiteralPath $appConfigPath -PathType Leaf)) {
        throw "Build lane '$($buildLane.id)' references missing AppConfig '$appConfigPath'."
    }
    New-Item -ItemType Directory -Path $outputPath -Force | Out-Null
    New-Item -ItemType Directory -Path $intermediatePath -Force | Out-Null

    Write-Host "Building $($buildLane.id) from the original project with $msbuild"
    & $msbuild `
        $projectPath `
        "/t:$target" `
        "/p:Configuration=$Configuration" `
        "/p:Platform=$($buildLane.platform)" `
        "/p:PlatformTarget=$($buildLane.platformTarget)" `
        "/p:TargetFrameworkVersion=$($buildLane.targetFramework)" `
        "/p:AppConfig=$appConfigPath" `
        "/p:OutputPath=$outputPath\" `
        "/p:IntermediateOutputPath=$intermediatePath\" `
        '/p:UseSharedCompilation=false' `
        "/v:$Verbosity" `
        '/nologo'

    if ($LASTEXITCODE -ne 0) {
        throw "Build lane '$($buildLane.id)' failed with exit code $LASTEXITCODE."
    }
}

Write-Host "Built $($lanes.Count) source-identical C3 lane(s)."
