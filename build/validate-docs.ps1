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
    'docs/governance/1.3.0-three-lane-matrix-2026-08-05.md',
    'docs/governance/1.3.0-release-authorization-2026-08-05.md',
    'docs/governance/1.3.0-alpha3-classic-setup-2026-08-05.md',
    'docs/governance/1.3.0-beta1-authorization-2026-08-06.md',
    'docs/planning/1.3.0-recovery-plan.md',
    'docs/planning/1.3.0-salvage-ledger.md',
    'docs/planning/1.3.0-milestones.md',
    'docs/planning/1.3.0-alpha.1.md',
    'docs/planning/1.3.0-alpha.2.md',
    'docs/planning/1.3.0-alpha.3.md',
    'docs/planning/1.3.0-beta.1.md',
    'docs/planning/1.3.0-stable.md',
    'docs/testing/1.3.0-qualification-matrix.md',
    'docs/testing/1.3.0-alpha3-defect-ledger.md',
    'docs/testing/1.3.0-historical-gate1-record.md',
    'docs/testing/1.3.0-target-runtime-record.md',
    'docs/testing/1.3.0-target-setup-record.md',
    'docs/setup/1.3.0-manifest-contracts.md',
    'docs/compatibility/1x-evidence-matrix.md',
    'release/validation/1.3.0-preparation-2026-08-05.md',
    'release/validation/1.3.0-reconstructed-baseline.md',
    'release/validation/1.3.0-alpha.1.md',
    'release/validation/1.3.0-alpha.1-post-correction.md',
    'release/validation/1.3.0-three-lane-preparation-2026-08-05.md',
    'release/validation/1.3.0-release-control-hardening-2026-08-05.md',
    'release/validation/1.3.0-candidate-freeze-assertions-2026-08-05.md',
    'release/validation/1.3.0-builder-and-gate1-preparation-2026-08-05.md',
    'release/validation/1.3.0-alpha.2-preparation-2026-08-05.md',
    'release/validation/1.3.0-alpha.3-preparation-2026-08-05.md',
    'release/validation/1.3.0-alpha.3-qualified.md'
)

$failures = New-Object Collections.Generic.List[String]

foreach ($relativePath in @(
        'build/historical-toolchain.json',
        'build/prepare-historical-toolchain.ps1',
        'build/run-historical-gate1-builds.ps1',
        'build/prepare-historical-gate1-runtime-kit.ps1',
        'build/package-content/HISTORICAL_GATE1_README.txt',
        'build/alpha2-tag-message.ps1',
        'build/alpha2-qualified-evidence.ps1',
        'build/new-alpha2-qualified-record.ps1',
        'build/new-alpha2-post-tag-record.ps1',
        'build/test-alpha2-tag-message.ps1',
        'build/test-source-reproducibility.ps1',
        'build/verify-alpha3.ps1',
        'build/alpha3-tag-message.ps1',
        'build/test-alpha3-tag-message.ps1',
        'build/alpha3-external-evidence.ps1',
        'build/test-alpha3-external-evidence.ps1',
        'build/alpha3-qualified-evidence.ps1',
        'build/new-alpha3-qualified-record.ps1',
        'build/verify-alpha3-tagged.ps1',
        'build/new-alpha3-post-tag-record.ps1',
        'build/evidence-templates/alpha3-historical-gate1.json',
        'build/evidence-templates/alpha3-target-qualification.json',
        'build/verify-target-setup.ps1',
        'build/get-setup-genome.ps1',
        'build/setup-genome.v1.json',
        'build/setup-genome-allowlist.json',
        'build/validate-setup-genome.ps1',
        'release/validation/1.3.0-alpha.2-qualified.json',
        'release/validation/1.3.0-alpha.2-post-tag.json',
        'release/validation/1.3.0-alpha.3-qualified.json',
        'release/validation/1.3.0-alpha.3-post-tag.json')) {
    if (-not (Test-Path -LiteralPath (Join-Path $repositoryRoot $relativePath) -PathType Leaf)) {
        throw "Required release control is missing: $relativePath"
    }
}

$alpha3QualifiedTemplate = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release/validation/1.3.0-alpha.3-qualified.json') -Raw | ConvertFrom-Json
$alpha3PostTagTemplate = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release/validation/1.3.0-alpha.3-post-tag.json') -Raw | ConvertFrom-Json
if ([int]$alpha3QualifiedTemplate.schemaVersion -ne 1 -or
        [string]$alpha3QualifiedTemplate.status -notin @('template', 'pass') -or
        [string]$alpha3QualifiedTemplate.releaseLabel -cne '1.3.0a3' -or
        @($alpha3QualifiedTemplate.assets).Count -ne 6 -or
        @($alpha3QualifiedTemplate.applicationBuildEvidence).Count -ne 3 -or
        @($alpha3QualifiedTemplate.setupBuildEvidence).Count -ne 3) {
    $failures.Add('Alpha 3 qualification template does not preserve the six-asset and nine-executable evidence contract.')
}
if ([int]$alpha3PostTagTemplate.schemaVersion -ne 1 -or
        [string]$alpha3PostTagTemplate.status -notin @('template', 'pass') -or
        [string]$alpha3PostTagTemplate.releaseLabel -cne '1.3.0a3' -or
        [string]$alpha3PostTagTemplate.tagName -cne 'v1.3.0a3' -or
        @($alpha3PostTagTemplate.assets).Count -ne 6 -or
        [string]$alpha3PostTagTemplate.publicationStatus -cne 'retained-unpublished' -or
        [bool]$alpha3PostTagTemplate.publicReleaseCreated -or
        [bool]$alpha3PostTagTemplate.feedChanged -or
        [bool]$alpha3PostTagTemplate.legacyMoved -or
        ([string]$alpha3PostTagTemplate.status -ceq 'template' -and [bool]$alpha3PostTagTemplate.packagesRetained) -or
        ([string]$alpha3PostTagTemplate.status -ceq 'pass' -and -not [bool]$alpha3PostTagTemplate.packagesRetained) -or
        [bool]$alpha3PostTagTemplate.betaAuthorized) {
    $failures.Add('Alpha 3 post-tag template does not preserve tag, retention, publication, feed, legacy, and Beta authority boundaries.')
}

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
        '## Post-Alpha three-lane correction',
        '## Beta 1 entry — finish historical baseline reconstruction',
        '## Release-control hardening before Gate 1',
        '## Alpha 2 — three-lane control checkpoint',
        '## Alpha 3 — legacy reliability and classic setup',
        '## Alpha 3 — lifecycle and data safety',
        '## Alpha 3 — referential and counter integrity',
        '## Alpha 3 — settings, diagnostics, lanes, and packages',
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

$matrixDecision = Get-Content -LiteralPath (Join-Path $repositoryRoot 'docs/governance/1.3.0-three-lane-matrix-2026-08-05.md') -Raw
foreach ($laneId in @('win-x86-net40', 'win-x64-net48', 'win-arm64-net481')) {
    if (-not $matrixDecision.Contains($laneId)) {
        $failures.Add("Three-lane decision is missing release lane: $laneId")
    }
}
foreach ($statement in @(
        'exactly three public release lanes',
        'VS2017 15.9.81',
        'VS2022 17.14.37',
        'VS2026 18.8.2',
        'VS2015/MSBuild 14 = historical 1.2 reconstruction oracle',
        'portable classic executable packages',
        '0xaa64',
        'The candidate lock is external to source and immutable',
        'authenticated',
        'five-entry manifest',
        'Universal Setup')) {
    if (-not $matrixDecision.Contains($statement)) {
        $failures.Add("Three-lane decision is missing required authority: $statement")
    }
}

$releaseAuthorization = Get-Content -LiteralPath (Join-Path $repositoryRoot 'docs/governance/1.3.0-release-authorization-2026-08-05.md') -Raw
foreach ($statement in @(
        'Codex is authorized to create annotated C3 1.3.0 Alpha tags',
        'an Alpha distribution does not by itself authorize',
        'authorize publishing it publicly',
        'Explicit human approval is required before any C3 1.3.0 Beta version is created',
        'producing or retaining a Beta-labelled distribution')) {
    if (-not $releaseAuthorization.Contains($statement)) {
        $failures.Add("Release authorization is missing required owner decision: $statement")
    }
}

$betaAuthorization = Get-Content -LiteralPath (Join-Path $repositoryRoot 'docs/governance/1.3.0-beta1-authorization-2026-08-06.md') -Raw
foreach ($statement in @(
        'build and retain the three Beta-labelled portable Candidate ZIPs',
        'create annotated tag `v1.3.0b1` only after every technical',
        'fast-forward `legacy/1.x`',
        'does not authorize',
        'any change to `master` or `dev/2.x`',
        'any change to the root `VERSION` feed',
        'NO-GO retains maximum truthful focused repairs')) {
    if (-not $betaAuthorization.Contains($statement)) {
        $failures.Add("Beta authorization is missing required owner boundary: $statement")
    }
}

$freezeAssertions = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release/validation/1.3.0-candidate-freeze-assertions-2026-08-05.md') -Raw
foreach ($statement in @(
        'one source and one lock across the complete three-package set',
        'Provider-ref capture and offline build boundary',
        'Final source closure',
        'Candidate freeze, Beta qualification')) {
    if (-not $freezeAssertions.Contains($statement)) {
        $failures.Add("Candidate-freeze assertion record is missing required boundary: $statement")
    }
}

$activePlanPaths = @(
    'docs/planning/1.3.0-recovery-plan.md',
    'docs/planning/1.3.0-alpha.3.md',
    'docs/planning/1.3.0-beta.1.md',
    'docs/planning/1.3.0-stable.md',
    'docs/testing/1.3.0-qualification-matrix.md'
)
foreach ($relativePath in $activePlanPaths) {
    $activeContent = Get-Content -LiteralPath (Join-Path $repositoryRoot $relativePath) -Raw
    foreach ($laneId in @('win-x86-net40', 'win-x64-net48', 'win-arm64-net481')) {
        if (-not $activeContent.Contains($laneId)) {
            $failures.Add("Active authority $relativePath is missing release lane: $laneId")
        }
    }
}

$releaseNotes = Get-Content -LiteralPath (Join-Path $repositoryRoot 'RELEASE_NOTES.md') -Raw
foreach ($statement in @(
        '# Compact Cassette Catalogue 1.3.0 Alpha 3',
        '1.3.0a3 / Alpha 3 / v1.3.0a3',
        'intentionally unpublished',
        'runtime repairs, Candidate reproduction',
        'Forty-six shared-engine regressions and four',
        'No standalone uninstaller is published',
        'v1.3.0b1',
        'v1.3.0')) {
    if (-not $releaseNotes.Contains($statement)) {
        $failures.Add("Release notes are missing required Alpha disclosure: $statement")
    }
}

$alpha3Decision = Get-Content -LiteralPath (Join-Path $repositoryRoot 'docs/governance/1.3.0-alpha3-classic-setup-2026-08-05.md') -Raw
foreach ($statement in @(
        'portable ZIPs remain the canonical product payloads',
        'optional, secondary distribution',
        'exact already-qualified bytes',
        'One source-identical shared',
        'No MSI, MSIX, ClickOnce publication, network bootstrapper',
        'Every Beta-labelled tag or retained portable/setup byte requires explicit')) {
    if (-not $alpha3Decision.Contains($statement)) {
        $failures.Add("Alpha 3 decision is missing required authority: $statement")
    }
}

$alpha3Plan = Get-Content -LiteralPath (Join-Path $repositoryRoot 'docs/planning/1.3.0-alpha.3.md') -Raw
foreach ($statement in @(
        'Planned tag: `v1.3.0a3`',
        'C3-v1.3.0a3-win-x86-net40-portable.zip',
        'C3-v1.3.0a3-win-x64-net48-setup.zip',
        'Historical Gate 1 remains mandatory',
        'without a DLL',
        'post-tag attestation',
        'Stop before any Beta-labelled tag')) {
    if (-not $alpha3Plan.Contains($statement)) {
        $failures.Add("Alpha 3 plan is missing required gate or boundary: $statement")
    }
}

$alpha2Plan = Get-Content -LiteralPath (Join-Path $repositoryRoot 'docs/planning/1.3.0-alpha.2.md') -Raw
foreach ($statement in @(
        'Planned tag: `v1.3.0a2`',
        'C3-v1.3.0a2-win-x86-net40-portable.zip',
        'C3-v1.3.0a2-win-x64-net48-portable.zip',
        'C3-v1.3.0a2-win-arm64-net481-portable.zip',
        'one external immutable lock bound to C',
        'post-tag descendant P',
        'build/test-source-reproducibility.ps1',
        'Public publication: not authorized',
        'Explicit human approval is required before even retaining a package whose name',
        'minimum operating system')) {
    if (-not $alpha2Plan.Contains($statement)) {
        $failures.Add("Alpha 2 plan is missing required gate or boundary: $statement")
    }
}

$postTagTemplate = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release/validation/1.3.0-alpha.2-post-tag.json') -Raw | ConvertFrom-Json
if ([int]$postTagTemplate.schemaVersion -ne 1 -or
        [string]$postTagTemplate.releaseLabel -cne '1.3.0a2' -or
        [string]$postTagTemplate.tagName -cne 'v1.3.0a2' -or
        [string]$postTagTemplate.publicationStatus -cne 'retained-unpublished' -or
        [bool]$postTagTemplate.publicReleaseCreated -or
        [bool]$postTagTemplate.feedChanged -or
        [bool]$postTagTemplate.legacyMoved -or
        [string]$postTagTemplate.legacyCommit -cne 'c4115b82ea43fdd763685d862a08fe5c61db6dff' -or
        @($postTagTemplate.packages).Count -ne 3) {
    $failures.Add('Alpha 2 post-tag record/template does not preserve the release, publication, feed, package, and legacy boundary.')
}

$qualifiedTemplate = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release/validation/1.3.0-alpha.2-qualified.json') -Raw | ConvertFrom-Json
if ([int]$qualifiedTemplate.schemaVersion -ne 1 -or
        [string]$qualifiedTemplate.releaseLabel -cne '1.3.0a2' -or
        @($qualifiedTemplate.packages).Count -ne 3 -or
        @($qualifiedTemplate.buildEvidence).Count -ne 3) {
    $failures.Add('Alpha 2 qualified evidence record/template does not preserve the three-package and three-builder evidence contract.')
}

$alpha2Record = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release/validation/1.3.0-alpha.2-preparation-2026-08-05.md') -Raw
foreach ($statement in @(
        'repository identity preparation in progress',
        'releaseLabel=1.3.0a2',
        'does not claim that the three packages',
        'Administrator action is required')) {
    if (-not $alpha2Record.Contains($statement)) {
        $failures.Add("Alpha 2 preparation record is missing required evidence boundary: $statement")
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
