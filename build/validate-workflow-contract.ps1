[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$workflowRoot = Join-Path $repositoryRoot '.github\workflows'
$repositoryChecksPath = Join-Path $workflowRoot 'repository-checks.yml'
$failures = New-Object Collections.Generic.List[String]

if (-not (Test-Path -LiteralPath $repositoryChecksPath -PathType Leaf)) {
    throw "Hosted repository workflow is missing: $repositoryChecksPath"
}

$repositoryChecks = [IO.File]::ReadAllText($repositoryChecksPath)
$checkoutPattern = '(?ms)^\s*- name: Check out repository\s*\r?\n' +
    '\s*uses: actions/checkout@(?<sha>[0-9a-f]{40})[^\r\n]*\r?\n' +
    '\s*with:\s*\r?\n(?<inputs>(?:\s{10}[^\r\n]+\r?\n)+)'
$checkout = [regex]::Match($repositoryChecks, $checkoutPattern)
if (-not $checkout.Success) {
    $failures.Add('repository-checks.yml must contain one pinned checkout block.')
}
else {
    $inputs = [string]$checkout.Groups['inputs'].Value
    $requiredInputs = @(
        'ref: ${{ github.sha }}',
        'fetch-depth: 0',
        'fetch-tags: true',
        'persist-credentials: false'
    )
    foreach ($requiredInput in $requiredInputs) {
        if ($inputs -cnotmatch ('(?m)^\s{10}' + [regex]::Escape($requiredInput) + '\s*$')) {
            $failures.Add(
                "repository-checks.yml checkout must set '$requiredInput'.")
        }
    }
}

if ($repositoryChecks -cnotmatch
    "(?m)^\s*if: startsWith\(github\.ref, 'refs/tags/v2\.'\)\s*$" -or
    $repositoryChecks -cnotmatch
    '(?m)^\s*run: \./build/validate-release-contract\.ps1 -Mode Tag\s*$') {
    $failures.Add('repository-checks.yml must retain the exact v2 tag contract step.')
}

$workflowFiles = @(Get-ChildItem -LiteralPath $workflowRoot -Filter '*.yml' -File)
foreach ($workflowFile in $workflowFiles) {
    $workflowText = [IO.File]::ReadAllText($workflowFile.FullName)
    $usesMatches = [regex]::Matches($workflowText, '(?m)^\s*uses:\s*(?<target>\S+)')
    foreach ($usesMatch in $usesMatches) {
        $target = [string]$usesMatch.Groups['target'].Value
        if ($target.StartsWith('./', [StringComparison]::Ordinal)) {
            continue
        }
        if ($target -cnotmatch '^[A-Za-z0-9_.-]+/[A-Za-z0-9_.-]+@[0-9a-f]{40}$') {
            $relativePath = $workflowFile.FullName.Substring($repositoryRoot.Length + 1)
            $failures.Add("$relativePath uses an unpinned action '$target'.")
        }
    }
}

if ($failures.Count -gt 0) {
    throw ("Workflow-contract validation failed:`n - " + ($failures -join "`n - "))
}

Write-Host (
    "Workflow security and tag-checkout contract verified across " +
    "$($workflowFiles.Count) workflow(s).")
