[CmdletBinding()]
param(
    [string]$ManifestPath = (Join-Path $PSScriptRoot 'baseline-genome.v1.json'),
    [string]$AllowListPath = (Join-Path $PSScriptRoot 'baseline-genome-allowlist.json')
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
foreach ($requiredPath in @($ManifestPath, $AllowListPath)) {
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
        throw "Required genome file is missing: $requiredPath"
    }
}

$expected = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
$allowList = Get-Content -LiteralPath $AllowListPath -Raw | ConvertFrom-Json
if ([int]$expected.schemaVersion -ne 1 -or [int]$allowList.schemaVersion -ne 1) {
    throw 'Unsupported baseline genome or allow-list schema version.'
}

$baselineRef = [string]$expected.baseline.ref
$resolvedBaseline = (& git -C $repositoryRoot rev-parse "$baselineRef^{commit}").Trim()
if ($LASTEXITCODE -ne 0) {
    throw "Cannot resolve baseline ref '$baselineRef'."
}
if ($resolvedBaseline -cne [string]$expected.baseline.commit) {
    throw "Baseline ref '$baselineRef' moved: expected $($expected.baseline.commit), found $resolvedBaseline."
}

& git -C $repositoryRoot merge-base --is-ancestor $resolvedBaseline HEAD
if ($LASTEXITCODE -ne 0) {
    throw "Candidate HEAD does not descend from baseline $baselineRef ($resolvedBaseline)."
}

$allowed = @{}
foreach ($entry in @($allowList.entries)) {
    $surface = [string]$entry.surface
    $reason = [string]$entry.reason
    $regression = [string]$entry.regression
    if ([string]::IsNullOrWhiteSpace($surface) -or
            [string]::IsNullOrWhiteSpace($reason) -or
            [string]::IsNullOrWhiteSpace($regression)) {
        throw 'Every genome allow-list entry requires surface, reason, and regression fields.'
    }
    if ($allowed.ContainsKey($surface)) {
        throw "Duplicate genome allow-list surface: $surface"
    }
    $allowed[$surface] = $entry
}

$current = & (Join-Path $PSScriptRoot 'get-baseline-genome.ps1') -RepositoryRoot $repositoryRoot -BaselineRef $baselineRef
$surfaces = @(
    'productionFiles',
    'typeNames',
    'controlNames',
    'resourceKeys',
    'designerHashes',
    'resxHashes',
    'identity',
    'frameworkReferences',
    'settings',
    'dataSet',
    'catalogue',
    'principalAssets',
    'sourcePolicy'
)

$failures = New-Object Collections.Generic.List[String]
$approvedDifferences = New-Object Collections.Generic.List[String]
foreach ($surface in $surfaces) {
    $expectedJson = $expected.$surface | ConvertTo-Json -Depth 20 -Compress
    $currentJson = $current.$surface | ConvertTo-Json -Depth 20 -Compress
    if ($expectedJson -cne $currentJson) {
        if ($allowed.ContainsKey($surface)) {
            $approvedDifferences.Add($surface)
            Write-Warning "Approved baseline genome difference: $surface -- $($allowed[$surface].reason)"
        }
        else {
            $failures.Add($surface)
            Write-Error "Unapproved baseline genome difference: $surface" -ErrorAction Continue
        }
    }
}

$unknownAllowListEntries = @($allowed.Keys | Where-Object { $surfaces -notcontains $_ })
if ($unknownAllowListEntries.Count -gt 0) {
    throw "Unknown genome allow-list surface(s): $($unknownAllowListEntries -join ', ')"
}

if ($failures.Count -gt 0) {
    throw "Baseline genome validation failed for $($failures.Count) surface(s): $($failures -join ', ')"
}

Write-Host (
    "Baseline genome verified against {0} ({1}); {2} frozen surfaces, {3} approved difference(s)." -f
    $baselineRef,
    $resolvedBaseline,
    $surfaces.Count,
    $approvedDifferences.Count)
