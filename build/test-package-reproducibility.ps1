[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$testRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts\package-reproducibility'))
$allowedRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts')).TrimEnd('\') + '\'
if (-not $testRoot.StartsWith($allowedRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Package reproducibility root is outside repository artifacts: $testRoot"
}
$first = Join-Path $testRoot 'path-a'
$second = Join-Path $testRoot 'different-absolute-path-b'
foreach ($path in @($first, $second)) {
    $resolvedPath = [IO.Path]::GetFullPath($path)
    if (-not $resolvedPath.StartsWith($testRoot.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to replace package test path outside '$testRoot': $resolvedPath"
    }
    if (Test-Path -LiteralPath $path) {
        Remove-Item -LiteralPath $path -Recurse -Force
    }
    New-Item -ItemType Directory -Path $path -Force | Out-Null
}

& (Join-Path $PSScriptRoot 'package.ps1') -Configuration $Configuration -OutputDirectory $first
& (Join-Path $PSScriptRoot 'package.ps1') -Configuration $Configuration -OutputDirectory $second
& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -PackageDirectory $first
& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -PackageDirectory $second

$firstFiles = @(Get-ChildItem -LiteralPath $first -File | Sort-Object Name)
$secondFiles = @(Get-ChildItem -LiteralPath $second -File | Sort-Object Name)
if (($firstFiles.Name -join "`n") -cne ($secondFiles.Name -join "`n")) {
    throw 'Path-distinct package runs produced different asset names.'
}
for ($index = 0; $index -lt $firstFiles.Count; $index++) {
    $firstHash = (Get-FileHash -LiteralPath $firstFiles[$index].FullName -Algorithm SHA256).Hash
    $secondHash = (Get-FileHash -LiteralPath $secondFiles[$index].FullName -Algorithm SHA256).Hash
    if ($firstHash -cne $secondHash) {
        throw "Path-distinct package output differs for '$($firstFiles[$index].Name)': $firstHash / $secondHash"
    }
}

Write-Host "Deterministic package reproduction passed across '$first' and '$second'."
