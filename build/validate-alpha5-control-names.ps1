[CmdletBinding()]
param(
    [string]$ManifestPath = (Join-Path $PSScriptRoot 'baseline-genome.v1.json'),
    [string]$AllowListPath = (Join-Path $PSScriptRoot 'alpha5-control-name-allowlist.json')
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$baseline = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
$allowList = Get-Content -LiteralPath $AllowListPath -Raw | ConvertFrom-Json
if ([int]$allowList.schemaVersion -ne 1) { throw 'Unsupported Alpha 5 control-name allow-list schema.' }
$current = & (Join-Path $PSScriptRoot 'get-baseline-genome.ps1') -RepositoryRoot $repositoryRoot -BaselineRef ([string]$baseline.baseline.ref)

function Get-ControlKey($entry) {
    return '{0}|{1}|{2}' -f ([string]$entry.path), ([string]$entry.name), ([string]$entry.type)
}

$baselineKeys = @{}
foreach ($entry in @($baseline.controlNames)) { $baselineKeys[(Get-ControlKey $entry)] = $true }
$currentKeys = @{}
foreach ($entry in @($current.controlNames)) { $currentKeys[(Get-ControlKey $entry)] = $true }
$allowedKeys = @{}
foreach ($entry in @($allowList.entries)) {
    if ([string]::IsNullOrWhiteSpace([string]$entry.reason)) {
        throw 'Every Alpha 5 control-name entry requires a review reason.'
    }
    $key = Get-ControlKey $entry
    if ($allowedKeys.ContainsKey($key)) { throw "Duplicate Alpha 5 control-name entry: $key" }
    $allowedKeys[$key] = $true
}

$removed = @($baselineKeys.Keys | Where-Object { -not $currentKeys.ContainsKey($_) } | Sort-Object)
if ($removed.Count -ne 0) {
    throw "Alpha 5 may not remove or rename baseline controls: $($removed -join ', ')"
}
$added = @($currentKeys.Keys | Where-Object { -not $baselineKeys.ContainsKey($_) } | Sort-Object)
$unapproved = @($added | Where-Object { -not $allowedKeys.ContainsKey($_) })
$unused = @($allowedKeys.Keys | Where-Object { -not $currentKeys.ContainsKey($_) } | Sort-Object)
if ($unapproved.Count -ne 0 -or $unused.Count -ne 0) {
    throw "Alpha 5 control-name closure failed. Unapproved: $($unapproved -join ', '); unused: $($unused -join ', ')."
}

Write-Host "Alpha 5 control-name closure verified: $($added.Count) reviewed addition(s), zero baseline removals."
