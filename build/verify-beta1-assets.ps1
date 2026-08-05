[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$CandidateRoot,
    [string]$ExpectedSourceCommit,
    [string]$ExpectedToolchainLockSha256,
    [string]$ManifestPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-contract.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($ManifestPath)) { $ManifestPath = Join-Path $PSScriptRoot 'lanes.json' }
$manifest = Assert-C3Beta1ManifestPath -Path $ManifestPath
$CandidateRoot = [IO.Path]::GetFullPath($CandidateRoot)
$sourceCommit = Split-Path -Leaf $CandidateRoot
if ($sourceCommit -notmatch '^[0-9a-f]{40}$' -or
        (-not [string]::IsNullOrWhiteSpace($ExpectedSourceCommit) -and $sourceCommit -cne $ExpectedSourceCommit)) {
    throw "Beta 1 Candidate directory must end in its exact 40-character source commit: $CandidateRoot"
}

$expectedTopLevel = @($script:C3Beta1AssetNames + 'SHA256SUMS.txt') | Sort-Object
$actualTopLevel = @(Get-ChildItem -LiteralPath $CandidateRoot -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualTopLevel -join "`n") -cne ($expectedTopLevel -join "`n")) {
    throw "Beta 1 Candidate root is not the exact six-ZIP/checksum set: $($actualTopLevel -join ', ')"
}
$unexpectedDirectories = @(Get-ChildItem -LiteralPath $CandidateRoot -Directory | Where-Object { $_.Name -cne 'evidence' })
if ($unexpectedDirectories.Count -ne 0) { throw 'Beta 1 Candidate root contains an unexpected directory.' }

$evidenceDirectory = Join-Path $CandidateRoot 'evidence'
$expectedEvidence = @(
    'CANDIDATE_SHA256.txt', 'PROVIDER_REF_RECEIPT_SHA256.txt', 'RELEASE_ASSETS_SHA256.txt',
    'TOOLCHAIN_LOCK_SHA256.txt', 'candidate.json', 'provider-ref-receipt.json',
    'release-assets.json', 'toolchain-lock.json'
) | Sort-Object
$actualEvidence = @(Get-ChildItem -LiteralPath $evidenceDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualEvidence -join "`n") -cne ($expectedEvidence -join "`n")) {
    throw "Beta 1 Candidate evidence has an unexpected file set: $($actualEvidence -join ', ')"
}
$unexpectedEvidenceDirectories = @(Get-ChildItem -LiteralPath $evidenceDirectory -Directory | Where-Object { $_.Name -cne 'entries' })
if ($unexpectedEvidenceDirectories.Count -ne 0) { throw 'Beta 1 Candidate evidence contains an unexpected directory.' }

function Assert-HashReceipt {
    param([string]$Directory, [string]$FileName, [string]$ReceiptName)
    $hash = (Get-FileHash -LiteralPath (Join-Path $Directory $FileName) -Algorithm SHA256).Hash.ToLowerInvariant()
    if ((Get-Content -LiteralPath (Join-Path $Directory $ReceiptName) -Raw) -cne "$hash  $FileName`n") {
        throw "Beta 1 Candidate hash receipt is invalid for '$FileName'."
    }
    return $hash
}

$lockHash = Assert-HashReceipt -Directory $evidenceDirectory -FileName 'toolchain-lock.json' -ReceiptName 'TOOLCHAIN_LOCK_SHA256.txt'
if (-not [string]::IsNullOrWhiteSpace($ExpectedToolchainLockSha256) -and $lockHash -cne $ExpectedToolchainLockSha256) {
    throw "Beta 1 Candidate lock hash '$lockHash' differs from the expected hash."
}
$lock = Get-Content -LiteralPath (Join-Path $evidenceDirectory 'toolchain-lock.json') -Raw | ConvertFrom-Json
if ([int]$lock.schemaVersion -ne 3 -or [string]$lock.status -cne 'locked' -or
        [string]$lock.sourceCommit -cne $sourceCommit -or
        [string]$lock.providerRefReceipt.fetchedCommit -cne $sourceCommit) {
    throw 'Beta 1 Candidate lock is not locked to the Candidate source and fetched provider ref.'
}
$receiptHash = Assert-HashReceipt -Directory $evidenceDirectory -FileName 'provider-ref-receipt.json' -ReceiptName 'PROVIDER_REF_RECEIPT_SHA256.txt'
$providerReceipt = Get-Content -LiteralPath (Join-Path $evidenceDirectory 'provider-ref-receipt.json') -Raw | ConvertFrom-Json
foreach ($propertyName in @('remoteName', 'remoteUrl', 'providerRef', 'remoteTrackingRef', 'fetchedCommit', 'fetchedAtUtc')) {
    if ([string]$providerReceipt.$propertyName -cne [string]$lock.providerRefReceipt.$propertyName) {
        throw "Beta 1 Candidate provider receipt property '$propertyName' differs from the immutable lock."
    }
}

$releaseRecordHash = Assert-HashReceipt -Directory $evidenceDirectory -FileName 'release-assets.json' -ReceiptName 'RELEASE_ASSETS_SHA256.txt'
$candidateHash = Assert-HashReceipt -Directory $evidenceDirectory -FileName 'candidate.json' -ReceiptName 'CANDIDATE_SHA256.txt'
$releaseRecord = Get-Content -LiteralPath (Join-Path $evidenceDirectory 'release-assets.json') -Raw | ConvertFrom-Json
$candidate = Get-Content -LiteralPath (Join-Path $evidenceDirectory 'candidate.json') -Raw | ConvertFrom-Json
if ([int]$releaseRecord.schemaVersion -ne 1 -or [string]$releaseRecord.classification -cne 'Candidate' -or
        [string]$releaseRecord.releaseLabel -cne '1.3.0b1' -or [string]$releaseRecord.releaseStage -cne 'Beta 1' -or
        [string]$releaseRecord.releaseTag -cne 'v1.3.0b1' -or [string]$releaseRecord.releaseChannel -cne 'beta' -or
        [string]$releaseRecord.publicationStatus -cne 'retained-unpublished' -or
        [string]$releaseRecord.sourceCommit -cne $sourceCommit -or [string]$releaseRecord.toolchainLockSha256 -cne $lockHash) {
    throw 'Beta 1 release-asset record is not Candidate evidence with the exact release/source/lock identity.'
}
if ([int]$candidate.schemaVersion -ne 1 -or [string]$candidate.classification -cne 'Candidate' -or
        [string]$candidate.releaseLabel -cne '1.3.0b1' -or [string]$candidate.sourceCommit -cne $sourceCommit -or
        [string]$candidate.toolchainLockSha256 -cne $lockHash -or
        [string]$candidate.providerRefReceiptSha256 -cne $receiptHash -or
        [string]$candidate.releaseAssetRecordSha256 -cne $releaseRecordHash -or
        [string]$candidate.publicationStatus -cne 'retained-unpublished' -or [bool]$candidate.publicReleaseCreated -or
        [bool]$candidate.tagCreated -or [bool]$candidate.legacyMoved) {
    throw 'Beta 1 Candidate index violates retained-unpublished, no-tag, no-ledger, or source/lock evidence closure.'
}

$checksumLines = @(Get-Content -LiteralPath (Join-Path $CandidateRoot 'SHA256SUMS.txt'))
$releaseAssets = @($releaseRecord.assets)
$candidateAssets = @($candidate.assets)
if ($checksumLines.Count -ne 6 -or $releaseAssets.Count -ne 6 -or $candidateAssets.Count -ne 6) {
    throw 'Beta 1 Candidate must bind exactly six assets in checksums and both evidence records.'
}
$entryNames = @($script:C3Beta1AssetNames | ForEach-Object { "$_.entries.json" }) | Sort-Object
$actualEntryNames = @(Get-ChildItem -LiteralPath (Join-Path $evidenceDirectory 'entries') -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualEntryNames -join "`n") -cne ($entryNames -join "`n")) { throw 'Beta 1 Candidate does not retain exactly six entry manifests.' }
for ($index = 0; $index -lt 6; $index++) {
    $name = $script:C3Beta1AssetNames[$index]
    $expectedKind = if ($index -lt 3) { 'portable' } else { 'setup' }
    $expectedLane = $script:C3Beta1LaneIds[$index % 3]
    $assetPath = Join-Path $CandidateRoot $name
    $assetHash = (Get-FileHash -LiteralPath $assetPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $entryName = "$name.entries.json"
    $entryPath = Join-Path (Join-Path $evidenceDirectory 'entries') $entryName
    $entryHash = (Get-FileHash -LiteralPath $entryPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $releaseAsset = @($releaseAssets | Where-Object { [string]$_.name -ceq $name })
    $candidateAsset = @($candidateAssets | Where-Object { [string]$_.name -ceq $name })
    if ($checksumLines[$index] -cne "$assetHash  $name" -or $releaseAsset.Count -ne 1 -or $candidateAsset.Count -ne 1) {
        throw "Beta 1 Candidate checksum or asset closure failed for '$name'."
    }
    foreach ($asset in @($releaseAsset[0], $candidateAsset[0])) {
        if ([string]$asset.kind -cne $expectedKind -or [string]$asset.lane -cne $expectedLane -or
                [string]$asset.sha256 -cne $assetHash -or [long]$asset.size -ne [long](Get-Item -LiteralPath $assetPath).Length -or
                [string]$asset.entryManifestName -cne $entryName -or [string]$asset.entryManifestSha256 -cne $entryHash) {
            throw "Beta 1 Candidate evidence differs from retained asset or entry manifest '$name'."
        }
    }
    $entry = Get-Content -LiteralPath $entryPath -Raw | ConvertFrom-Json
    if ([string]$entry.packageName -cne $name -or [string]$entry.packageSha256 -cne $assetHash -or
            [string]$entry.releaseLabel -cne '1.3.0b1' -or [string]$entry.sourceCommit -cne $sourceCommit -or
            [string]$entry.toolchainLockSha256 -cne $lockHash) {
        throw "Beta 1 retained entry manifest is not bound to '$name', source, and lock."
    }
}

Write-Host "Verified exact retained Beta 1 Candidate '$CandidateRoot' (candidate-index SHA-256 $candidateHash)."
