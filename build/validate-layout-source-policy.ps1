[CmdletBinding()]
param(
    [ValidateSet('Discovery', 'Qualification')]
    [string]$Mode = 'Qualification',
    [string]$OutputPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$sourceRoot = Join-Path $repositoryRoot 'Compact Cassette Catalogue'
$patterns = [ordered]@{
    'runtime-layout-helper' = 'CatalogueUx\.(?:ConfigureMainForm|ConfigureTapeForm|ConfigureListForm|AddCancelButton|AddActionButton)'
    'runtime-form-scroll' = '(?<!\.)\bAutoScroll\s*='
    'runtime-client-size' = '(?<!\.)\bClientSize\s*='
    'runtime-bounds' = '\.(?:Bounds|Left|Top|Width|Height)\s*(?:\+|-)?='
    'runtime-visible-control' = '\.Controls\.Add\('
    'runtime-literal-rectangle' = '\bNew\s+Rectangle\('
}

$violations = @()
foreach ($file in Get-ChildItem -LiteralPath $sourceRoot -Filter '*.vb' -File |
        Where-Object { $_.Name -notlike '*.Designer.vb' } | Sort-Object Name) {
    $lines = Get-Content -LiteralPath $file.FullName
    for ($lineIndex = 0; $lineIndex -lt $lines.Count; $lineIndex++) {
        $line = [string]$lines[$lineIndex]
        foreach ($entry in $patterns.GetEnumerator()) {
            if ($line -match [string]$entry.Value) {
                $violations += [PSCustomObject][ordered]@{
                    rule = [string]$entry.Key
                    path = $file.FullName.Substring($repositoryRoot.Length + 1).Replace('\', '/')
                    line = $lineIndex + 1
                    text = $line.Trim()
                }
            }
        }
    }
}

$result = [PSCustomObject][ordered]@{
    schemaVersion = 1
    mode = $Mode
    sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    violationCount = $violations.Count
    violations = @($violations)
}

if (-not [string]::IsNullOrWhiteSpace($OutputPath)) {
    $parent = Split-Path -Parent $OutputPath
    if (-not [string]::IsNullOrWhiteSpace($parent)) {
        New-Item -ItemType Directory -Force -Path $parent | Out-Null
    }
    $result | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $OutputPath -Encoding UTF8
}

if ($Mode -eq 'Discovery') {
    $expectedFiles = @(
        'CatalogueWorkflow.vb', 'frmBrandNew.vb', 'frmBrands.vb', 'frmDeckNew.vb',
        'frmDecks.vb', 'frmMain.vb', 'frmModelNew.vb', 'frmModels.vb',
        'frmTapeNew.vb', 'frmTapes.vb'
    )
    $actualFiles = @($violations | ForEach-Object { [IO.Path]::GetFileName($_.path) } | Sort-Object -Unique)
    $missing = @($expectedFiles | Where-Object { $_ -notin $actualFiles })
    if ($missing.Count -ne 0) {
        throw "Alpha 4 source-policy discovery did not reproduce expected files: $($missing -join ', ')."
    }
    Write-Host "Alpha 4 layout source-policy failures reproduced: $($violations.Count) violation(s) across $($actualFiles.Count) files."
    return
}

if ($violations.Count -ne 0) {
    $summary = @($violations | ForEach-Object { "$($_.path):$($_.line) [$($_.rule)]" })
    throw "Runtime layout source policy failed:`n$($summary -join "`n")"
}

Write-Host 'Runtime layout source policy verified: no static-control construction or fixed geometry remains in production behaviour files.'
