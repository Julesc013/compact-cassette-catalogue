[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$DistributionDirectory,
    [string]$RemoteName = 'origin'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha5-contract.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($DistributionDirectory)) {
    $DistributionDirectory = Join-Path $repositoryRoot 'artifacts\distributions\1.3.0a5'
}
$DistributionDirectory = [IO.Path]::GetFullPath($DistributionDirectory)
$jsonPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.5-qualified.json'
$markdownPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.5-qualified.md'
$template = Get-Content -LiteralPath $jsonPath -Raw | ConvertFrom-Json
if ([string]$template.status -cne 'template') { throw 'Alpha 5 qualified record is not an unpopulated template.' }
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) {
    throw 'Alpha 5 evidence generation requires clean frozen package source C.'
}
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0a5
if ($LASTEXITCODE -eq 0) { throw 'Alpha 5 evidence E must be created before v1.3.0a5.' }

$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
Assert-C3Alpha5Manifest -Manifest $manifest
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-alpha3-assets.ps1') -Configuration $Configuration -RequireCandidateEvidence
Assert-C3Alpha5Distribution -Directory $DistributionDirectory -Record $null

$portableEvidence = Join-Path $repositoryRoot 'artifacts\evidence\packages\1.3.0a5'
$setupEvidence = Join-Path $repositoryRoot 'artifacts\evidence\setup-packages\1.3.0a5'
$firstManifest = Get-Content -LiteralPath (Join-Path $portableEvidence "$($manifest.lanes[0].packageName).entries.json") -Raw | ConvertFrom-Json
if ([string]$firstManifest.sourceCommit -cne $sourceCommit) { throw 'Alpha 5 package bytes are not bound to current source C.' }
$lockHash = [string]$firstManifest.toolchainLockSha256
$closurePath = Join-Path $repositoryRoot 'artifacts\evidence\build\candidate-source-closure.json'
$reproPath = Join-Path $repositoryRoot 'artifacts\evidence\source-reproducibility\1.3.0a5\source-reproducibility.json'
$startupPath = Join-Path $repositoryRoot "artifacts\evidence\startup\$sourceCommit\startup-lifecycle-summary.json"
foreach ($path in @($closurePath, $reproPath, $startupPath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Alpha 5 retained evidence is missing: $path" }
}
$startup = Get-Content -LiteralPath $startupPath -Raw | ConvertFrom-Json
if ([string]$startup.sourceCommit -cne $sourceCommit -or [int]$startup.total -lt 40 -or [int]$startup.failed -ne 0) {
    throw 'Alpha 5 startup evidence must bind source C and pass at least 40 x86/x64 runs.'
}

$assets = @($script:C3Alpha5AssetNames | ForEach-Object {
    $name = $_
    $lane = @($script:C3Alpha5Lanes | Where-Object { $name.Contains($_) })[0]
    $kind = if ($name.EndsWith('-portable.zip', [StringComparison]::Ordinal)) { 'portable' } else { 'setup' }
    $entryRoot = if ($kind -ceq 'portable') { $portableEvidence } else { $setupEvidence }
    $entryPath = Join-Path $entryRoot "$name.entries.json"
    [ordered]@{
        name = $name
        kind = $kind
        lane = $lane
        size = [long](Get-Item -LiteralPath (Join-Path $DistributionDirectory $name)).Length
        sha256 = (Get-FileHash -LiteralPath (Join-Path $DistributionDirectory $name) -Algorithm SHA256).Hash.ToLowerInvariant()
        entryManifestSha256 = (Get-FileHash -LiteralPath $entryPath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
})

function Get-RemoteHead([string]$Ref) {
    $lines = @(& git -C $repositoryRoot ls-remote --heads $RemoteName $Ref)
    if ($LASTEXITCODE -ne 0 -or $lines.Count -ne 1) { throw "Could not bind remote protected ref '$Ref'." }
    return [string](@($lines[0] -split "`t")[0])
}
$record = [ordered]@{
    schemaVersion = 1
    status = 'pass'
    classification = 'retained-alpha-test-distribution'
    releaseVersion = '1.3.0'
    releaseStage = 'Alpha 5'
    releaseLabel = '1.3.0a5'
    releaseTag = 'v1.3.0a5'
    releaseChannel = 'alpha'
    publicationStatus = 'retained-unpublished'
    sourceCommit = $sourceCommit
    toolchainLockSha256 = $lockHash
    sourceClosureSha256 = (Get-FileHash -LiteralPath $closurePath -Algorithm SHA256).Hash.ToLowerInvariant()
    sourceReproducibilityEvidenceSha256 = (Get-FileHash -LiteralPath $reproPath -Algorithm SHA256).Hash.ToLowerInvariant()
    startupEvidenceSha256 = (Get-FileHash -LiteralPath $startupPath -Algorithm SHA256).Hash.ToLowerInvariant()
    startupRuns = [int]$startup.total
    protectedRefs = [ordered]@{
        master = Get-RemoteHead 'refs/heads/master'
        dev2x = Get-RemoteHead 'refs/heads/dev/2.x'
        legacy1x = Get-RemoteHead 'refs/heads/legacy/1.x'
    }
    feedSha256 = (Get-FileHash -LiteralPath (Join-Path $repositoryRoot 'VERSION') -Algorithm SHA256).Hash.ToLowerInvariant()
    acceptanceStatus = 'pending-owner-test'
    historicalGate1 = 'open'
    exactTargetQualification = 'open'
    nativeArm64Execution = 'open'
    publicReleaseCreated = $false
    betaAuthorized = $false
    assets = $assets
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
}
[IO.File]::WriteAllText($jsonPath, (($record | ConvertTo-Json -Depth 10) + "`n"), (New-Object Text.UTF8Encoding($false)))
$assetLines = @($assets | ForEach-Object { "- ``$($_.name)`` — ``$($_.sha256)``" }) -join "`n"
$markdown = @"
# C3 1.3.0 Alpha 5 retained test-distribution record

Status: **PASS for retained, unpublished owner testing.**

Package source: ``$sourceCommit``  
External toolchain lock SHA-256: ``$lockHash``  
Startup lifecycle: ``$($startup.passed)/$($startup.total) PASS``

All three application, installer, and uninstaller lanes passed Candidate
binary/package verification. The six assets were reproduced from two clean,
path-distinct checkouts and are retained without a public release.

$assetLines

Owner native visual/keyboard/accessibility review, historical Gate 1, native
ARM64 execution, and exact target-OS qualification remain open. No Beta tag or
Beta-labelled artifact is authorized. The public feed, ``master``, ``dev/2.x``,
and ``legacy/1.x`` remain outside this Alpha transaction.
"@
[IO.File]::WriteAllText($markdownPath, $markdown.Replace("`r`n", "`n"), (New-Object Text.UTF8Encoding($false)))
Write-Host "Populated Alpha 5 qualified evidence for source C '$sourceCommit'. Commit only the qualified JSON/Markdown pair as E."
