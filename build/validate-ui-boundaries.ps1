[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$winFormsRoot = Join-Path $repositoryRoot 'src\C3.WinForms'
$legacyState = [IO.Path]::GetFullPath(
    (Join-Path $winFormsRoot 'State\LegacyGlobalState.vb'))
$failures = New-Object Collections.Generic.List[String]

$retiredSettingsPaths = @(
    (Join-Path $winFormsRoot 'Configuration\MySettingsStore.vb'),
    (Join-Path $winFormsRoot 'My Project\Settings.Designer.vb'),
    (Join-Path $winFormsRoot 'My Project\Settings.settings')
)
foreach ($retiredPath in $retiredSettingsPaths) {
    if (Test-Path -LiteralPath $retiredPath) {
        $failures.Add("Retired My.Settings artifact '$retiredPath' has been reintroduced.")
    }
}

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

    if ($content -match '\bMy\.Settings\b') {
        $failures.Add(
            "WinForms source '$($source.FullName)' uses the retired My.Settings provider.")
    }
}

foreach ($configurationPath in @(
        (Join-Path $winFormsRoot 'Configuration\Net40\App.config'),
        (Join-Path $winFormsRoot 'Configuration\Net48\App.config'))) {
    $configuration = Get-Content -LiteralPath $configurationPath -Raw
    if ($configuration -match '<userSettings\b|My\.MySettings') {
        $failures.Add(
            "Application configuration '$configurationPath' reintroduces My.Settings ownership.")
    }
}

if ($failures.Count -gt 0) {
    throw ("WinForms boundary validation failed:`n - " + ($failures -join "`n - "))
}

Write-Host 'WinForms boundaries verified: typed services, one DataSet composition seam, C3-owned preferences.'
