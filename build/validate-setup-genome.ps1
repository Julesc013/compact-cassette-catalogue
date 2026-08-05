[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifestPath = Join-Path $PSScriptRoot 'setup-genome.v1.json'
$allowListPath = Join-Path $PSScriptRoot 'setup-genome-allowlist.json'
$expected = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$allowList = Get-Content -LiteralPath $allowListPath -Raw | ConvertFrom-Json
if ([int]$expected.schemaVersion -ne 1 -or [int]$allowList.schemaVersion -ne 1) {
    throw 'Unsupported setup genome or allow-list schema version.'
}

$resolvedBaseline = (& git -C $repositoryRoot rev-parse "$($expected.baseline.ref)^{commit}").Trim()
if ($LASTEXITCODE -ne 0 -or $resolvedBaseline -cne [string]$expected.baseline.commit) {
    throw "Setup genome baseline moved or cannot be resolved: $($expected.baseline.ref)."
}
& git -C $repositoryRoot merge-base --is-ancestor $resolvedBaseline HEAD
if ($LASTEXITCODE -ne 0) {
    throw 'Current source does not descend from the setup genome baseline.'
}

$allowed = @{}
foreach ($entry in @($allowList.entries)) {
    foreach ($field in @('surface', 'reason', 'regression')) {
        if ([string]::IsNullOrWhiteSpace([string]$entry.$field)) {
            throw "Every setup genome allow-list entry requires $field."
        }
    }
    if ($allowed.ContainsKey([string]$entry.surface)) {
        throw "Duplicate setup genome allow-list surface '$($entry.surface)'."
    }
    $allowed[[string]$entry.surface] = $entry
}

$actual = & (Join-Path $PSScriptRoot 'get-setup-genome.ps1') -RepositoryRoot $repositoryRoot
$failures = New-Object Collections.Generic.List[String]
$usedAllowances = New-Object Collections.Generic.List[String]

function Compare-Surface {
    param([string]$Name, [object]$ExpectedValue, [object]$ActualValue)
    $expectedJson = $ExpectedValue | ConvertTo-Json -Depth 30 -Compress
    $actualJson = $ActualValue | ConvertTo-Json -Depth 30 -Compress
    if ($expectedJson -cne $actualJson) {
        if ($allowed.ContainsKey($Name)) {
            $usedAllowances.Add($Name)
            Write-Warning "Approved setup genome difference: $Name -- $($allowed[$Name].reason)"
        }
        else {
            $failures.Add($Name)
        }
    }
}

Compare-Surface 'identity' $expected.identity $actual.identity
Compare-Surface 'formClasses' $expected.formClasses $actual.formClasses

foreach ($collectionName in @('applicationManifests', 'controls', 'resourceKeys', 'artwork')) {
    $expectedByPath = @{}
    foreach ($item in @($expected.$collectionName)) { $expectedByPath[[string]$item.path] = $item }
    $actualByPath = @{}
    foreach ($item in @($actual.$collectionName)) { $actualByPath[[string]$item.path] = $item }
    $paths = @($expectedByPath.Keys + $actualByPath.Keys | Sort-Object -Unique)
    foreach ($path in $paths) {
        Compare-Surface "$collectionName`:$path" $expectedByPath[$path] $actualByPath[$path]
    }
}

$unusedAllowances = @($allowed.Keys | Where-Object { $usedAllowances -notcontains $_ })
if ($unusedAllowances.Count -gt 0) {
    $failures.Add("unused allow-list entries: $($unusedAllowances -join ', ')")
}
if ($failures.Count -gt 0) {
    throw "Setup genome validation failed: $($failures -join ', ')"
}

Write-Host "Setup genome verified against $resolvedBaseline; $($usedAllowances.Count) reviewed difference(s)."
