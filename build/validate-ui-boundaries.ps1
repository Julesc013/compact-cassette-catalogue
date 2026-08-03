[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$winFormsRoot = Join-Path $repositoryRoot 'src\C3.WinForms'
$legacyState = [IO.Path]::GetFullPath(
    (Join-Path $winFormsRoot 'State\LegacyGlobalState.vb'))
$settingsAdapter = [IO.Path]::GetFullPath(
    (Join-Path $winFormsRoot 'Configuration\MySettingsStore.vb'))
$generatedSettings = [IO.Path]::GetFullPath(
    (Join-Path $winFormsRoot 'My Project\Settings.Designer.vb'))
$failures = New-Object Collections.Generic.List[String]

function Test-SamePath {
    param([string]$Left, [string]$Right)
    return [string]::Equals(
        [IO.Path]::GetFullPath($Left),
        [IO.Path]::GetFullPath($Right),
        [StringComparison]::OrdinalIgnoreCase)
}

foreach ($source in Get-ChildItem -LiteralPath $winFormsRoot -Recurse -File -Filter '*.vb') {
    $content = Get-Content -LiteralPath $source.FullName -Raw

    if (($content -match '\bDataSet\b' -or $content -match '\.Tables\s*\(') -and
            -not (Test-SamePath $source.FullName $legacyState)) {
        $failures.Add(
            "WinForms source '$($source.FullName)' crosses the legacy DataSet composition boundary.")
    }

    if ($content -match '\bDataTable\b|\bDataRow\b|\.Rows\s*\(') {
        $failures.Add(
            "WinForms source '$($source.FullName)' directly accesses the legacy row/table model.")
    }

    if ($content -match '\bMy\.Settings\b' -and
            -not (Test-SamePath $source.FullName $settingsAdapter) -and
            -not (Test-SamePath $source.FullName $generatedSettings)) {
        $failures.Add(
            "WinForms source '$($source.FullName)' bypasses the settings adapter.")
    }
}

if ($failures.Count -gt 0) {
    throw ("WinForms boundary validation failed:`n - " + ($failures -join "`n - "))
}

Write-Host 'WinForms boundaries verified: typed services, one DataSet composition seam, one settings adapter.'
