[CmdletBinding()]
param(
    [string]$Lane,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal',
    [switch]$Rebuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifestPath = Join-Path $PSScriptRoot 'lanes.json'
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)

if (-not [string]::IsNullOrWhiteSpace($Lane)) {
    $lanes = @($lanes | Where-Object { $_.id -ceq $Lane })
    if ($lanes.Count -eq 0) {
        $available = @($manifest.lanes | ForEach-Object { $_.id }) -join ', '
        throw "Unknown build lane '$Lane'. Available lanes: $available"
    }
}

$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1')
$target = if ($Rebuild) { 'Rebuild' } else { 'Build' }

foreach ($buildLane in $lanes) {
    $projectPath = Join-Path $repositoryRoot ([string]$buildLane.project)
    if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
        throw "Build lane '$($buildLane.id)' references missing project '$projectPath'."
    }

    Write-Host "Building $($buildLane.id): $Configuration|$($buildLane.platform)"
    & $msbuild `
        $projectPath `
        "/t:$target" `
        "/p:Configuration=$Configuration" `
        "/p:Platform=$($buildLane.platform)" `
        '/m' `
        "/v:$Verbosity" `
        '/nologo'

    if ($LASTEXITCODE -ne 0) {
        throw "Build lane '$($buildLane.id)' failed with exit code $LASTEXITCODE."
    }
}

Write-Host "Built $($lanes.Count) C3 lane(s)."

