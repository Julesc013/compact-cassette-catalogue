[CmdletBinding()]
param(
    [string]$OutputPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$sourceRoots = @(
    (Join-Path $repositoryRoot 'Compact Cassette Catalogue Installer'),
    (Join-Path $repositoryRoot 'Compact Cassette Catalogue Uninstaller')
)
$patterns = [ordered]@{
    'runtime-form-scroll' = '(?<!\.)\bAutoScroll\s*='
    'runtime-client-size' = '(?<!\.)\bClientSize\s*='
    'runtime-bounds' = '\.(?:Bounds|Left|Top|Width|Height)\s*(?:\+|-)?='
    'runtime-visible-control' = '\.Controls\.Add\('
    'runtime-button-construction' = '\bNew\s+(?:System\.Windows\.Forms\.)?Button\s*\('
    'runtime-literal-rectangle' = '\bNew\s+(?:System\.Drawing\.)?Rectangle\s*\('
}

$violations = @()
foreach ($sourceRoot in $sourceRoots) {
    foreach ($file in Get-ChildItem -LiteralPath $sourceRoot -Filter 'frm*.vb' -File |
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
}

$result = [PSCustomObject][ordered]@{
    schemaVersion = 1
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

if ($violations.Count -ne 0) {
    $summary = @($violations | ForEach-Object { "$($_.path):$($_.line) [$($_.rule)]" })
    throw "Setup runtime layout source policy failed:`n$($summary -join "`n")"
}

Write-Host 'Setup runtime layout source policy verified: wizard form behaviour files contain no geometry mutation or visible-control construction.'
