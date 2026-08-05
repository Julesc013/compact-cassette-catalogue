[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('Tagged', 'PostTag')]
    [string]$TagState = 'Tagged',
    [string]$RemoteName = 'origin',
    [switch]$AllowDirty
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha3-tag-message.ps1')
. (Join-Path $PSScriptRoot 'alpha3-qualified-evidence.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$qualifiedPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.3-qualified.json'
$qualified = Get-Content -LiteralPath $qualifiedPath -Raw | ConvertFrom-Json
$packageSource = [string]$qualified.packageSource
$headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$tagName = 'v1.3.0a3'
$tagRef = "refs/tags/$tagName"
$qualifiedHumanRecord = 'release/validation/1.3.0-alpha.3-qualified.md'
$qualifiedMachineRecord = 'release/validation/1.3.0-alpha.3-qualified.json'
$postTagRecord = 'release/validation/1.3.0-alpha.3-post-tag.json'
$legacyCheckpoint = 'c4115b82ea43fdd763685d862a08fe5c61db6dff'
if ($RemoteName -cne 'origin') { throw "Alpha 3 tag authority is fixed to remote 'origin'." }
if ($packageSource -notmatch '^[0-9a-f]{40}$') { throw 'Alpha 3 qualified record has no valid package source C.' }

& git -C $repositoryRoot show-ref --verify --quiet $tagRef
if ($LASTEXITCODE -ne 0) { throw "$TagState verification requires annotated tag '$tagName'." }
if ((& git -C $repositoryRoot cat-file -t $tagRef).Trim() -cne 'tag') { throw "'$tagName' is not an annotated tag." }
$tagObjectText = @(& git -C $repositoryRoot cat-file tag $tagRef) -join "`n"
Assert-C3Alpha3TagMessage -Text $tagObjectText
$tagObject = (& git -C $repositoryRoot rev-parse $tagRef).Trim()
$tagCommit = (& git -C $repositoryRoot rev-parse "$tagName^{commit}").Trim()
$evidenceCommit = $headCommit
if ($TagState -ceq 'PostTag') {
    $parents = @((& git -C $repositoryRoot rev-list --parents -n 1 HEAD).Trim().Split(' '))
    if ($parents.Count -ne 2) { throw 'Alpha 3 post-tag P must be a single-parent commit directly after E.' }
    $evidenceCommit = [string]$parents[1]
}
if ($tagCommit -cne $evidenceCommit) {
    throw "'$tagName' points to '$tagCommit', expected evidence commit E '$evidenceCommit' for $TagState verification."
}
& git -C $repositoryRoot merge-base --is-ancestor $packageSource $evidenceCommit
if ($LASTEXITCODE -ne 0) { throw "Alpha 3 package source C '$packageSource' is not an ancestor of E '$evidenceCommit'." }
$evidenceChanges = @(& git -C $repositoryRoot diff --name-only $packageSource $evidenceCommit)
$allowedEvidenceChanges = @($qualifiedHumanRecord, $qualifiedMachineRecord)
if ($evidenceChanges.Count -ne 2 -or
        @($evidenceChanges | Where-Object { $allowedEvidenceChanges -notcontains $_ }).Count -ne 0 -or
        @($allowedEvidenceChanges | Where-Object { $evidenceChanges -notcontains $_ }).Count -ne 0) {
    throw "Alpha 3 evidence commit E must change exactly the human and machine qualification records: $($evidenceChanges -join ', ')"
}
if ($TagState -ceq 'PostTag') {
    $postTagChanges = @(& git -C $repositoryRoot diff --name-only $evidenceCommit $headCommit)
    if ($postTagChanges.Count -ne 1 -or $postTagChanges[0] -cne $postTagRecord) {
        throw "Alpha 3 post-tag commit P may change only '$postTagRecord': $($postTagChanges -join ', ')"
    }
}

$resolvedLegacy = (& git -C $repositoryRoot rev-parse legacy/1.x).Trim()
if ($resolvedLegacy -cne $legacyCheckpoint) { throw "legacy/1.x moved during Alpha 3: $resolvedLegacy" }
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0b1
if ($LASTEXITCODE -eq 0) { throw 'A Beta tag exists without the explicit human approval required beyond Alpha 3.' }

& (Join-Path $PSScriptRoot 'verify-preparation.ps1') -Configuration $Configuration -ExpectedBuildSourceCommit $packageSource
& (Join-Path $PSScriptRoot 'verify-release-identity.ps1') -ExpectedProductVersion '1.3.0' -ExpectedStage 'Alpha 3' `
    -ExpectedReleaseLabel '1.3.0a3' -ExpectedTag $tagName -ExpectedDate ([datetime]'2026-08-05') `
    -Configuration $Configuration -VerifyBuildOutputs
& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-setup-builds.ps1') -Configuration $Configuration -ExpectedSourceCommit $packageSource
& (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-alpha3-assets.ps1') -Configuration $Configuration -RequireCandidateEvidence
$qualifiedEvidence = Assert-C3Alpha3QualifiedEvidence -RepositoryRoot $repositoryRoot -Manifest $manifest `
    -Configuration $Configuration -PackageSource $packageSource

if ($TagState -ceq 'PostTag') {
    $record = Get-Content -LiteralPath (Join-Path $repositoryRoot $postTagRecord) -Raw | ConvertFrom-Json
    $remoteUrl = (& git -C $repositoryRoot remote get-url $RemoteName).Trim()
    $remoteTagLines = @(& git -C $repositoryRoot ls-remote --tags $RemoteName $tagRef "$tagRef^{}")
    if ($LASTEXITCODE -ne 0) { throw "Could not read remote Alpha 3 tag from '$RemoteName'." }
    $remoteTagObject = $null
    $remoteTagTarget = $null
    foreach ($line in $remoteTagLines) {
        $parts = @($line -split "`t")
        if ($parts.Count -eq 2 -and $parts[1] -ceq $tagRef) { $remoteTagObject = [string]$parts[0] }
        if ($parts.Count -eq 2 -and $parts[1] -ceq "$tagRef^{}") { $remoteTagTarget = [string]$parts[0] }
    }
    $remoteDevelopment = @(& git -C $repositoryRoot ls-remote --heads $RemoteName refs/heads/dev/1.x)
    $remoteLegacy = @(& git -C $repositoryRoot ls-remote --heads $RemoteName refs/heads/legacy/1.x)
    $remoteBeta = @(& git -C $repositoryRoot ls-remote --tags $RemoteName refs/tags/v1.3.0b1 'refs/tags/v1.3.0b1^{}')
    if ($remoteDevelopment.Count -ne 1 -or [string](@($remoteDevelopment[0] -split "`t")[0]) -cne $headCommit) {
        throw 'Remote dev/1.x does not equal post-tag attestation P.'
    }
    if ($remoteLegacy.Count -ne 1 -or [string](@($remoteLegacy[0] -split "`t")[0]) -cne $legacyCheckpoint) {
        throw 'Remote legacy/1.x moved during Alpha 3.'
    }
    if ($remoteBeta.Count -ne 0) { throw 'Remote v1.3.0b1 exists without explicit human approval.' }
    $feedLines = @(Get-Content -LiteralPath (Join-Path $repositoryRoot 'VERSION'))
    $recordAssets = @($record.assets)
    $qualifiedAssets = @($qualifiedEvidence.assets)
    if ($recordAssets.Count -ne 6) { throw 'Alpha 3 post-tag record must contain exactly six retained assets.' }
    for ($index = 0; $index -lt 6; $index++) {
        foreach ($property in @('kind', 'lane', 'name', 'sha256', 'entryManifestName', 'entryManifestSha256')) {
            if ([string]$recordAssets[$index].$property -cne [string]$qualifiedAssets[$index].$property) {
                throw "Alpha 3 post-tag asset '$index' property '$property' differs from E."
            }
        }
    }
    if ([int]$record.schemaVersion -ne 1 -or [string]$record.status -cne 'pass' -or
            [string]$record.releaseLabel -cne '1.3.0a3' -or [string]$record.tagName -cne $tagName -or
            [string]$record.tagObject -cne $tagObject -or [string]$record.tagTarget -cne $evidenceCommit -or
            [string]$record.remoteName -cne $RemoteName -or [string]$record.remoteUrl -cne $remoteUrl -or
            [string]$record.remoteTagObject -cne $remoteTagObject -or [string]$record.remoteTagTarget -cne $remoteTagTarget -or
            [string]$record.packageSource -cne $packageSource -or
            [string]$record.toolchainLockSha256 -cne [string]$qualifiedEvidence.toolchainLockSha256 -or
            [string]$record.releaseAssetRecordSha256 -cne [string]$qualifiedEvidence.releaseAssetRecordSha256 -or
            [string]$record.distributionChecksumManifestSha256 -cne [string]$qualifiedEvidence.distributionChecksumManifestSha256 -or
            [string]$record.publicationStatus -cne 'retained-unpublished' -or [bool]$record.publicReleaseCreated -or
            [bool]$record.feedChanged -or (($record.publicFeed -join "`n") -cne ($feedLines -join "`n")) -or
            [bool]$record.legacyMoved -or [string]$record.legacyCommit -cne $legacyCheckpoint -or
            -not [bool]$record.packagesRetained -or [bool]$record.betaAuthorized -or
            [string]::IsNullOrWhiteSpace([string]$record.recordedAtUtc) -or
            $remoteTagObject -cne $tagObject -or $remoteTagTarget -cne $evidenceCommit) {
        throw 'Alpha 3 post-tag record does not match tag, retained bytes, remote refs, publication/feed/legacy, or Beta authority.'
    }
}

if (-not $AllowDirty) {
    $status = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
    if ($status.Count -ne 0) { throw "Alpha 3 $TagState verification requires a clean worktree:`n$($status -join "`n")" }
}
Write-Host "C3 1.3.0 Alpha 3 $TagState topology, retained Candidate bytes, external qualification, and authority boundaries passed."
