[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$winFormsRoot = Join-Path $repositoryRoot 'src\C3.WinForms'
$compositionRoot = [IO.Path]::GetFullPath(
    (Join-Path $winFormsRoot 'Bootstrap\ApplicationComposition.vb'))
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

$retiredGlobalState = Join-Path $winFormsRoot 'State\LegacyGlobalState.vb'
if (Test-Path -LiteralPath $retiredGlobalState) {
    $failures.Add("Retired global document seam '$retiredGlobalState' has been reintroduced.")
}

$retiredBrandForms = @(
    'Features\Brands\frmBrands.vb',
    'Features\Brands\frmBrandNew.vb',
    'Features\Brands\frmBrandEdit.vb')
foreach ($relativePath in $retiredBrandForms) {
    $retiredPath = Join-Path $winFormsRoot $relativePath
    if (Test-Path -LiteralPath $retiredPath) {
        $failures.Add("Retired duplicate Brand form '$retiredPath' has been reintroduced.")
    }
}

$presentationRoot = Join-Path $repositoryRoot 'src\C3.Presentation.WinForms'
$brandFormPath = Join-Path $presentationRoot 'Features\Brands\BrandWorkspaceForm.cs'
$brandDesignerPath = Join-Path $presentationRoot 'Features\Brands\BrandWorkspaceForm.Designer.cs'
foreach ($requiredPath in @($brandFormPath, $brandDesignerPath)) {
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
        $failures.Add("Shared Brand workspace file '$requiredPath' is missing.")
    }
}
if (Test-Path -LiteralPath $brandFormPath -PathType Leaf) {
    $brandFormText = Get-Content -LiteralPath $brandFormPath -Raw
    foreach ($requiredFragment in @(
            'BrandWorkspacePresenter',
            'WorkspaceController',
            'CatalogueChanged',
            'ProcessCmdKey')) {
        if ($brandFormText -notmatch [regex]::Escape($requiredFragment)) {
            $failures.Add("Shared Brand workspace does not contain '$requiredFragment'.")
        }
    }
}
if (Test-Path -LiteralPath $brandDesignerPath -PathType Leaf) {
    $brandDesignerText = Get-Content -LiteralPath $brandDesignerPath -Raw
    foreach ($requiredFragment in @(
            'AutoScaleMode.Dpi',
            'MinimumSize = new System.Drawing.Size(720, 450)',
            'AccessibleName = "Brands"',
            'AccessibleName = "Brand name"',
            'AccessibleName = "Brand code"',
            'AccessibleName = "Brand notes"')) {
        if ($brandDesignerText -notmatch [regex]::Escape($requiredFragment)) {
            $failures.Add("Shared Brand designer does not contain '$requiredFragment'.")
        }
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
            -not (Test-SamePath $source.FullName $compositionRoot)) {
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

$compositionText = Get-Content -LiteralPath $compositionRoot -Raw
if ($compositionText -match '(?im)^\s*(Friend|Public)?\s*Module\b') {
    $failures.Add('ApplicationComposition must be an explicit instance, not a module-level global.')
}
if ($compositionText -notmatch 'NotInheritable Class ApplicationComposition') {
    $failures.Add('The WinForms composition root must be the explicit ApplicationComposition class.')
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

Write-Host 'WinForms boundaries verified: explicit application composition, one DataSet adapter seam, shared workspace, and C3-owned preferences.'
