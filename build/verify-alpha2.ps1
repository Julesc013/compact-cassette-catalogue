[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('Candidate', 'Tagged', 'PostTag')]
    [string]$TagState = 'Candidate',
    [string]$ToolchainLockPath,
    [switch]$SkipBuildOutputs,
    [switch]$RunLaunchSmoke,
    [switch]$AllowDirty
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot 'alpha2-tag-message.ps1')
. (Join-Path $PSScriptRoot 'alpha2-qualified-evidence.ps1')
$tagName = 'v1.3.0a2'
$identityBase = '576c6ecb0b65f97899b9abbe4cf84063151091c1'
$developmentBaseline = '58a5b7d21daf19e1b6112d44efb887c7d8ea9500'
$legacyCheckpoint = 'c4115b82ea43fdd763685d862a08fe5c61db6dff'
$alpha1TagObject = '95b530f4f726fb67b3b002b47bf1d4061e71ce3c'
$alpha1TagCommit = '8caa155103879cf41dc6ada753c0927180929059'
$alpha2Record = 'release/validation/1.3.0-alpha.2-preparation-2026-08-05.md'
$qualifiedRecord = 'release/validation/1.3.0-alpha.2-qualified.json'
$postTagRecord = 'release/validation/1.3.0-alpha.2-post-tag.json'
$headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$expectedBuildSource = $headCommit
$packageSource = $null
if ($TagState -in @('Tagged', 'PostTag') -and $SkipBuildOutputs) {
    throw "$TagState Alpha 2 verification requires the retained Candidate build and package evidence."
}
if ($TagState -in @('Tagged', 'PostTag')) {
    $manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
    $firstEntryManifest = Join-Path $repositoryRoot (
        "artifacts\evidence\packages\{0}\{1}.entries.json" -f
        $manifest.releaseLabel, $manifest.lanes[0].packageName)
    if (-not (Test-Path -LiteralPath $firstEntryManifest -PathType Leaf)) {
        throw "$TagState Alpha 2 verification requires retained package evidence: $firstEntryManifest"
    }
    $packageSource = [string](Get-Content -LiteralPath $firstEntryManifest -Raw | ConvertFrom-Json).sourceCommit
    $expectedBuildSource = $packageSource
}

& (Join-Path $PSScriptRoot 'verify-preparation.ps1') `
    -Configuration $Configuration `
    -SkipBuildOutputs:$SkipBuildOutputs `
    -ExpectedBuildSourceCommit $expectedBuildSource
& (Join-Path $PSScriptRoot 'verify-release-identity.ps1') `
    -ExpectedProductVersion '1.3.0' `
    -ExpectedStage 'Alpha 2' `
    -ExpectedReleaseLabel '1.3.0a2' `
    -ExpectedTag $tagName `
    -ExpectedDate ([datetime]'2026-08-05') `
    -Configuration $Configuration `
    -VerifyBuildOutputs:(-not $SkipBuildOutputs)
& (Join-Path $PSScriptRoot 'test-package-evidence-set.ps1')
& (Join-Path $PSScriptRoot 'test-alpha2-tag-message.ps1')
& (Join-Path $PSScriptRoot 'test-source-reproducibility.ps1') -SelfTest
& (Join-Path $PSScriptRoot 'test-target-tooling-ps2.ps1')

if ($RunLaunchSmoke -and $SkipBuildOutputs) {
    throw '-RunLaunchSmoke cannot be combined with -SkipBuildOutputs.'
}
if ($TagState -ceq 'Candidate' -and -not $SkipBuildOutputs -and
        [string]::IsNullOrWhiteSpace($ToolchainLockPath)) {
    throw 'Full Candidate Alpha 2 verification requires -ToolchainLockPath for two clean path-distinct source rebuilds.'
}
if ($TagState -cne 'Candidate' -and -not [string]::IsNullOrWhiteSpace($ToolchainLockPath)) {
    throw '-ToolchainLockPath is used only by full Candidate Alpha 2 verification; tagged modes verify retained bytes without rebuilding.'
}

& git -C $repositoryRoot merge-base --is-ancestor $developmentBaseline HEAD
if ($LASTEXITCODE -ne 0) {
    throw 'Alpha 2 candidate does not descend from the recovered development baseline.'
}
$resolvedLegacy = (& git -C $repositoryRoot rev-parse legacy/1.x).Trim()
if ($resolvedLegacy -cne $legacyCheckpoint) {
    throw "legacy/1.x moved during Alpha 2 preparation: expected '$legacyCheckpoint', found '$resolvedLegacy'."
}
$resolvedAlpha1Object = (& git -C $repositoryRoot rev-parse refs/tags/v1.3.0a1).Trim()
$resolvedAlpha1Commit = (& git -C $repositoryRoot rev-parse 'v1.3.0a1^{commit}').Trim()
if ($resolvedAlpha1Object -cne $alpha1TagObject -or $resolvedAlpha1Commit -cne $alpha1TagCommit) {
    throw "Immutable Alpha 1 identity changed: $resolvedAlpha1Object / $resolvedAlpha1Commit."
}

$allowedProductionChanges = @(
    'Compact Cassette Catalogue/My Project/AssemblyInfo.vb',
    'Compact Cassette Catalogue/varGlobals.vb'
)
$productionChanges = @(& git -C $repositoryRoot diff --name-only $identityBase -- 'Compact Cassette Catalogue')
$unexpectedProductionChanges = @($productionChanges | Where-Object { $allowedProductionChanges -notcontains $_ })
$missingIdentityChanges = @($allowedProductionChanges | Where-Object { $productionChanges -notcontains $_ })
if ($unexpectedProductionChanges.Count -gt 0) {
    throw "Alpha 2 contains application changes outside the identity projection: $($unexpectedProductionChanges -join ', ')"
}
if ($missingIdentityChanges.Count -gt 0) {
    throw "Alpha 2 is missing source identity projection(s): $($missingIdentityChanges -join ', ')"
}

$tagRef = "refs/tags/$tagName"
& git -C $repositoryRoot show-ref --verify --quiet $tagRef
$tagExists = $LASTEXITCODE -eq 0
if ($TagState -ceq 'Candidate' -and $tagExists) {
    throw "Candidate validation requires absent tag '$tagName'."
}
if ($TagState -in @('Tagged', 'PostTag')) {
    if (-not $tagExists) {
        throw "$TagState validation requires annotated tag '$tagName'."
    }
    if ((& git -C $repositoryRoot cat-file -t $tagRef).Trim() -cne 'tag') {
        throw "'$tagName' is not an annotated tag."
    }
    $tagCommit = (& git -C $repositoryRoot rev-parse "$tagName^{commit}").Trim()
    $expectedTagTarget = $headCommit
    if ($TagState -ceq 'PostTag') {
        $parents = @((& git -C $repositoryRoot rev-list --parents -n 1 HEAD).Trim().Split(' '))
        if ($parents.Count -ne 2) {
            throw 'PostTag verification requires P to be a single-parent commit directly after evidence commit E.'
        }
        $expectedTagTarget = [string]$parents[1]
    }
    if ($tagCommit -cne $expectedTagTarget) {
        throw "'$tagName' points to '$tagCommit', expected '$expectedTagTarget' for $TagState verification."
    }
    $tagObjectText = @(& git -C $repositoryRoot cat-file tag $tagRef) -join "`n"
    Assert-C3Alpha2TagMessage -Text $tagObjectText
}

if (-not $SkipBuildOutputs) {
    & (Join-Path $PSScriptRoot 'verify-packages.ps1') `
        -Configuration $Configuration `
        -RequireCandidateEvidence
    $qualifiedEvidence = $null
    if ($TagState -in @('Tagged', 'PostTag')) {
        $qualifiedEvidence = Assert-C3Alpha2QualifiedEvidence `
            -RepositoryRoot $repositoryRoot `
            -Manifest $manifest `
            -Configuration $Configuration `
            -PackageSource $packageSource
    }
    if ($TagState -ceq 'Candidate') {
        & (Join-Path $PSScriptRoot 'test-source-reproducibility.ps1') `
            -Configuration $Configuration `
            -ToolchainLockPath $ToolchainLockPath
        & (Join-Path $PSScriptRoot 'test-release-controls.ps1') `
            -Configuration $Configuration
    }
    if ([string]::IsNullOrWhiteSpace($packageSource)) {
        $manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
        $firstEntryManifest = Join-Path $repositoryRoot (
            "artifacts\evidence\packages\{0}\{1}.entries.json" -f
            $manifest.releaseLabel, $manifest.lanes[0].packageName)
        $packageSource = [string](Get-Content -LiteralPath $firstEntryManifest -Raw | ConvertFrom-Json).sourceCommit
    }
    & git -C $repositoryRoot merge-base --is-ancestor $packageSource $headCommit
    if ($LASTEXITCODE -ne 0) {
        throw "Alpha 2 package source '$packageSource' is not an ancestor of HEAD '$headCommit'."
    }
    if ($TagState -ceq 'Candidate' -and $packageSource -cne $headCommit) {
        throw "Candidate Alpha 2 packages must be built from current HEAD '$headCommit', found '$packageSource'."
    }
    if ($TagState -ceq 'Tagged') {
        $postSourceChanges = @(& git -C $repositoryRoot diff --name-only $packageSource $headCommit)
        $allowedEvidenceChanges = @($alpha2Record, $qualifiedRecord)
        $unexpectedEvidenceChanges = @($postSourceChanges | Where-Object { $allowedEvidenceChanges -notcontains $_ })
        if ($unexpectedEvidenceChanges.Count -gt 0) {
            throw "Tagged Alpha 2 has non-evidence changes after package source: $($unexpectedEvidenceChanges -join ', ')"
        }
        if ($postSourceChanges.Count -ne 2 -or @($allowedEvidenceChanges | Where-Object { $postSourceChanges -notcontains $_ }).Count -ne 0) {
            throw 'Tagged Alpha 2 evidence commit E must change exactly the human and machine-readable qualification records.'
        }
    }
    if ($TagState -ceq 'PostTag') {
        $evidenceCommit = $tagCommit
        $preTagChanges = @(& git -C $repositoryRoot diff --name-only $packageSource $evidenceCommit)
        $allowedEvidenceChanges = @($alpha2Record, $qualifiedRecord)
        $unexpectedPreTagChanges = @($preTagChanges | Where-Object { $allowedEvidenceChanges -notcontains $_ })
        if ($unexpectedPreTagChanges.Count -gt 0) {
            throw "PostTag Alpha 2 has non-evidence changes from package source C to evidence commit E: $($unexpectedPreTagChanges -join ', ')"
        }
        if ($preTagChanges.Count -ne 2 -or @($allowedEvidenceChanges | Where-Object { $preTagChanges -notcontains $_ }).Count -ne 0) {
            throw 'PostTag Alpha 2 evidence commit E must change exactly the human and machine-readable qualification records.'
        }
        $postTagChanges = @(& git -C $repositoryRoot diff --name-only $evidenceCommit $headCommit)
        if ($postTagChanges.Count -ne 1 -or $postTagChanges[0] -cne $postTagRecord) {
            throw "PostTag commit P may change only '$postTagRecord'; found: $($postTagChanges -join ', ')"
        }

        $postTagPath = Join-Path $repositoryRoot $postTagRecord
        $record = Get-Content -LiteralPath $postTagPath -Raw | ConvertFrom-Json
        $tagObject = (& git -C $repositoryRoot rev-parse $tagRef).Trim()
        $remoteName = [string]$record.remoteName
        if ($remoteName -cne 'origin') {
            throw "PostTag record must bind the authoritative remote 'origin', found '$remoteName'."
        }
        $remoteUrl = (& git -C $repositoryRoot remote get-url $remoteName).Trim()
        $remoteTagLines = @(& git -C $repositoryRoot ls-remote --tags $remoteName $tagRef "$tagRef^{}")
        if ($LASTEXITCODE -ne 0) {
            throw "Could not read remote Alpha 2 tag from '$remoteName'."
        }
        $remoteTagObject = $null
        $remoteTagTarget = $null
        foreach ($line in $remoteTagLines) {
            $parts = @($line -split "`t")
            if ($parts.Count -eq 2 -and $parts[1] -ceq $tagRef) {
                $remoteTagObject = [string]$parts[0]
            }
            if ($parts.Count -eq 2 -and $parts[1] -ceq "$tagRef^{}") {
                $remoteTagTarget = [string]$parts[0]
            }
        }
        if ($remoteTagObject -cne $tagObject -or $remoteTagTarget -cne $evidenceCommit) {
            throw "Remote Alpha 2 annotated object/target does not match local tag: $remoteTagObject / $remoteTagTarget."
        }
        $remoteLegacyLine = @(& git -C $repositoryRoot ls-remote --heads $remoteName refs/heads/legacy/1.x)
        if ($LASTEXITCODE -ne 0 -or $remoteLegacyLine.Count -ne 1) {
            throw "Could not resolve remote legacy/1.x from '$remoteName'."
        }
        $remoteLegacyCommit = [string](@($remoteLegacyLine[0] -split "`t")[0])

        $packageDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)"
        $packageEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)"
        $recordPackages = @($record.packages)
        if ($recordPackages.Count -ne $manifest.lanes.Count) {
            throw 'PostTag record does not contain exactly the three Alpha 2 package records.'
        }
        foreach ($index in 0..($manifest.lanes.Count - 1)) {
            $lane = $manifest.lanes[$index]
            $packageRecord = $recordPackages[$index]
            $qualifiedPackage = @($qualifiedEvidence.packages)[$index]
            $expectedEntryName = "$($lane.packageName).entries.json"
            $actualPackageHash = (Get-FileHash -LiteralPath (Join-Path $packageDirectory ([string]$lane.packageName)) -Algorithm SHA256).Hash.ToLowerInvariant()
            $actualEntryHash = (Get-FileHash -LiteralPath (Join-Path $packageEvidenceDirectory $expectedEntryName) -Algorithm SHA256).Hash.ToLowerInvariant()
            if ([string]$packageRecord.name -cne [string]$lane.packageName -or
                    [string]$packageRecord.sha256 -cne $actualPackageHash -or
                    [string]$packageRecord.sha256 -cne [string]$qualifiedPackage.sha256 -or
                    [string]$packageRecord.entryManifestName -cne $expectedEntryName -or
                    [string]$packageRecord.entryManifestSha256 -cne $actualEntryHash -or
                    [string]$packageRecord.entryManifestSha256 -cne [string]$qualifiedPackage.entryManifestSha256) {
                throw "PostTag record package hashes do not match retained lane '$($lane.id)'."
            }
        }
        $actualPackageChecksumHash = (Get-FileHash -LiteralPath (Join-Path $packageDirectory 'SHA256SUMS.txt') -Algorithm SHA256).Hash.ToLowerInvariant()
        $actualEntryChecksumHash = (Get-FileHash -LiteralPath (Join-Path $packageEvidenceDirectory 'ENTRY_MANIFEST_SHA256SUMS.txt') -Algorithm SHA256).Hash.ToLowerInvariant()
        $feedLines = @(Get-Content -LiteralPath (Join-Path $repositoryRoot 'VERSION'))
        if ([int]$record.schemaVersion -ne 1 -or
                [string]$record.status -cne 'pass' -or
                [string]$record.releaseLabel -cne [string]$manifest.releaseLabel -or
                [string]$record.tagName -cne $tagName -or
                [string]$record.tagObject -cne $tagObject -or
                [string]$record.tagTarget -cne $evidenceCommit -or
                [string]$record.remoteUrl -cne $remoteUrl -or
                [string]$record.remoteTagObject -cne $remoteTagObject -or
                [string]$record.remoteTagTarget -cne $remoteTagTarget -or
                [string]$record.packageSource -cne $packageSource -or
                [string]$record.toolchainLockSha256 -cne [string]$qualifiedEvidence.toolchainLockSha256 -or
                [string]$record.packageChecksumManifestSha256 -cne [string]$qualifiedEvidence.packageChecksumManifestSha256 -or
                [string]$record.entryChecksumManifestSha256 -cne [string]$qualifiedEvidence.entryChecksumManifestSha256 -or
                [string]$record.publicationStatus -cne 'retained-unpublished' -or
                [bool]$record.publicReleaseCreated -or
                [bool]$record.feedChanged -or
                (($record.publicFeed -join "`n") -cne ($feedLines -join "`n")) -or
                [bool]$record.legacyMoved -or
                [string]$record.legacyCommit -cne $legacyCheckpoint -or
                $remoteLegacyCommit -cne $legacyCheckpoint -or
                -not [bool]$record.packagesRetained -or
                [string]::IsNullOrWhiteSpace([string]$record.recordedAtUtc)) {
            throw 'PostTag record does not match the local/remote tag, retained bytes, feed, publication, or legacy boundary.'
        }
    }
    if ($RunLaunchSmoke) {
        & (Join-Path $PSScriptRoot 'smoke-launch.ps1') `
            -Configuration $Configuration `
            -AllowKnownCloseTimeout
    }
}

if (-not $AllowDirty) {
    $status = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
    if ($status.Count -ne 0) {
        throw "Alpha 2 validation requires a clean worktree:`n$($status -join "`n")"
    }
}

$scope = if ($SkipBuildOutputs) { 'source-only preparation' } elseif ($TagState -ceq 'PostTag') { 'post-tag retained-byte and remote attestation' } elseif ($TagState -ceq 'Tagged') { 'tagged retained Candidate qualification' } else { 'Candidate packages and controls' }
Write-Host "C3 1.3.0 Alpha 2 $($TagState.ToLowerInvariant()) verified for $scope; public feed and legacy ledger remain unchanged."
