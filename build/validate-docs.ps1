[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$requiredFiles = @(
    'README.md',
    'CHANGELOG.md',
    'RELEASE_NOTES.md',
    'TODO.md',
    'docs/README.md',
    'docs/governance/1x-branch-recovery-2026-08-05.md',
    'docs/governance/legacy-maintenance.md',
    'docs/planning/1.3.0-recovery-plan.md',
    'docs/planning/1.3.0-salvage-ledger.md',
    'docs/planning/1.3.0-milestones.md',
    'docs/planning/1.3.0-alpha.1.md',
    'docs/planning/1.3.0-beta.1.md',
    'docs/planning/1.3.0-stable.md',
    'docs/testing/1.3.0-qualification-matrix.md',
    'docs/compatibility/1x-evidence-matrix.md',
    'release/validation/1.3.0-preparation-2026-08-05.md',
    'release/validation/1.3.0-reconstructed-baseline.md',
    'release/validation/1.3.0-alpha.1.md',
    'release/validation/1.3.0-alpha.1-post-correction.md'
)

$failures = New-Object Collections.Generic.List[String]
foreach ($relativePath in $requiredFiles) {
    if (-not (Test-Path -LiteralPath (Join-Path $repositoryRoot $relativePath) -PathType Leaf)) {
        $failures.Add("Required maintenance document is missing: $relativePath")
    }
}

$trackedMarkdown = @(& git -C $repositoryRoot ls-files '*.md')
if ($LASTEXITCODE -ne 0) {
    throw 'Could not enumerate tracked Markdown files.'
}
foreach ($relativePath in $trackedMarkdown) {
    $fullPath = Join-Path $repositoryRoot $relativePath
    $content = Get-Content -LiteralPath $fullPath -Raw
    foreach ($match in [regex]::Matches($content, '\[[^\]]+\]\((?<target>[^\)]+)\)')) {
        $target = $match.Groups['target'].Value.Trim()
        if ($target.StartsWith('<') -and $target.EndsWith('>')) {
            $target = $target.Substring(1, $target.Length - 2)
        }
        $target = ($target -split '\s+"')[0]
        $target = ($target -split '#')[0]
        if ([string]::IsNullOrWhiteSpace($target) -or
                $target -match '^(?i:https?|mailto):' -or
                $target.StartsWith('#')) {
            continue
        }

        $decodedTarget = [Uri]::UnescapeDataString($target) -replace '/', '\'
        $resolvedTarget = [IO.Path]::GetFullPath((Join-Path (Split-Path -Parent $fullPath) $decodedTarget))
        if (-not (Test-Path -LiteralPath $resolvedTarget)) {
            $failures.Add("Broken local link in ${relativePath}: $target")
        }
    }
}

$todo = Get-Content -LiteralPath (Join-Path $repositoryRoot 'TODO.md') -Raw
foreach ($heading in @(
        '## Alpha 1 — maintenance foundation',
        '## Beta 1 entry — finish baseline reconstruction',
        '## Beta 1 — lifecycle and data safety',
        '## Beta 1 — referential and counter integrity',
        '## Beta 1 — settings, diagnostics, lanes, and packages',
        '## Stable 1.3.0',
        '## Explicitly outside C3 1.3')) {
    if (-not $todo.Contains($heading)) {
        $failures.Add("TODO is missing required workstream: $heading")
    }
}
foreach ($staleHeading in @('## 1.2.x Maintenance', '## 1.3 Usability', '## 3.0 UI Rebuild')) {
    if ($todo.Contains($staleHeading)) {
        $failures.Add("TODO still contains superseded legacy scope: $staleHeading")
    }
}
foreach ($baselineStatement in @('58a5b7d', '509c9ec', '2413e913', 'archive/1.2-postrelease-tip')) {
    if (-not $todo.Contains($baselineStatement)) {
        $failures.Add("TODO is missing corrected baseline identity: $baselineStatement")
    }
}

$releaseNotes = Get-Content -LiteralPath (Join-Path $repositoryRoot 'RELEASE_NOTES.md') -Raw
foreach ($statement in @(
        '# Compact Cassette Catalogue 1.3.0 Alpha 1',
        'intentionally unpublished',
        'No runtime lifecycle',
        'v1.3.0b1',
        'v1.3.0')) {
    if (-not $releaseNotes.Contains($statement)) {
        $failures.Add("Release notes are missing required Alpha disclosure: $statement")
    }
}

$versionLines = @(Get-Content -LiteralPath (Join-Path $repositoryRoot 'VERSION'))
$expectedFeed = @('1.2.0', 'Release', '14/05/2026')
if (($versionLines -join "`n") -cne ($expectedFeed -join "`n")) {
    $failures.Add('The public three-line VERSION feed changed before stable publication.')
}

& git -C $repositoryRoot diff --check
if ($LASTEXITCODE -ne 0) {
    $failures.Add('git diff --check reported whitespace errors.')
}

if ($failures.Count -gt 0) {
    throw "Documentation validation failed:`n - $($failures -join "`n - ")"
}

Write-Host "Legacy documentation verified: $($trackedMarkdown.Count) tracked Markdown files, required plans present, local links valid, public feed isolated."
