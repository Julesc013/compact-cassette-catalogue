[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$ToolchainLockPath,
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release',
    [string]$CandidateRoot
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-contract.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Assert-C3Beta1ManifestPath -Path (Join-Path $PSScriptRoot 'lanes.json')
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($sourceCommit -notmatch '^[0-9a-f]{40}$' -or @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) {
    throw 'Beta 1 Candidate retention requires an exact clean source commit C-beta.'
}
if ([string]::IsNullOrWhiteSpace($CandidateRoot)) {
    $CandidateRoot = Join-Path $repositoryRoot "artifacts\candidates\1.3.0b1\$sourceCommit"
}
$CandidateRoot = [IO.Path]::GetFullPath($CandidateRoot)
if ((Split-Path -Leaf $CandidateRoot) -cne $sourceCommit) { throw 'Beta 1 Candidate root must end in the full source SHA.' }
if (Test-Path -LiteralPath $CandidateRoot) { throw "Refusing to overwrite retained Beta 1 Candidate '$CandidateRoot'." }

$ToolchainLockPath = [IO.Path]::GetFullPath($ToolchainLockPath)
$lock = Get-Content -LiteralPath $ToolchainLockPath -Raw | ConvertFrom-Json
$lockHash = (Get-FileHash -LiteralPath $ToolchainLockPath -Algorithm SHA256).Hash.ToLowerInvariant()
if ([string]$lock.status -cne 'locked' -or [string]$lock.sourceCommit -cne $sourceCommit -or
        [string]$lock.providerRefReceipt.fetchedCommit -cne $sourceCommit) {
    throw 'External lock is not immutable Candidate authority for current C-beta.'
}

$evidenceDirectory = Join-Path $CandidateRoot 'evidence'
$entriesDirectory = Join-Path $evidenceDirectory 'entries'
New-Item -ItemType Directory -Path $entriesDirectory -Force | Out-Null
& (Join-Path $PSScriptRoot 'assemble-alpha3-assets.ps1') -Configuration $Configuration `
    -OutputDirectory $CandidateRoot -EvidenceDirectory $evidenceDirectory -RequireCandidateEvidence

$portableEntries = Join-Path $repositoryRoot 'artifacts\evidence\packages\1.3.0b1'
$setupEntries = Join-Path $repositoryRoot 'artifacts\evidence\setup-packages\1.3.0b1'
$assetRecords = New-Object Collections.Generic.List[Object]
for ($index = 0; $index -lt $script:C3Beta1AssetNames.Count; $index++) {
    $name = $script:C3Beta1AssetNames[$index]
    $kind = if ($index -lt 3) { 'portable' } else { 'setup' }
    $lane = $script:C3Beta1LaneIds[$index % 3]
    $entrySource = Join-Path $(if ($name.EndsWith('-portable.zip')) { $portableEntries } else { $setupEntries }) "$name.entries.json"
    $entryTarget = Join-Path $entriesDirectory "$name.entries.json"
    [IO.File]::Copy($entrySource, $entryTarget, $false)
    $assetPath = Join-Path $CandidateRoot $name
    $assetRecords.Add([ordered]@{
            name = $name
            kind = $kind
            lane = $lane
            size = [long](Get-Item -LiteralPath $assetPath).Length
            sha256 = (Get-FileHash -LiteralPath $assetPath -Algorithm SHA256).Hash.ToLowerInvariant()
            entryManifestName = "$name.entries.json"
            entryManifestSha256 = (Get-FileHash -LiteralPath $entryTarget -Algorithm SHA256).Hash.ToLowerInvariant()
        })
}
[IO.File]::Copy($ToolchainLockPath, (Join-Path $evidenceDirectory 'toolchain-lock.json'), $false)
$utf8 = New-Object Text.UTF8Encoding($false)
[IO.File]::WriteAllText((Join-Path $evidenceDirectory 'TOOLCHAIN_LOCK_SHA256.txt'), "$lockHash  toolchain-lock.json`n", $utf8)
$receiptPath = Join-Path $evidenceDirectory 'provider-ref-receipt.json'
[IO.File]::WriteAllText($receiptPath, (($lock.providerRefReceipt | ConvertTo-Json -Depth 6) + "`n"), $utf8)
$receiptHash = (Get-FileHash -LiteralPath $receiptPath -Algorithm SHA256).Hash.ToLowerInvariant()
[IO.File]::WriteAllText((Join-Path $evidenceDirectory 'PROVIDER_REF_RECEIPT_SHA256.txt'), "$receiptHash  provider-ref-receipt.json`n", $utf8)
$releaseRecordHash = (Get-FileHash -LiteralPath (Join-Path $evidenceDirectory 'release-assets.json') -Algorithm SHA256).Hash.ToLowerInvariant()
$candidate = [ordered]@{
    schemaVersion = 1; classification = 'Candidate'; releaseLabel = '1.3.0b1'; sourceCommit = $sourceCommit
    toolchainLockSha256 = $lockHash; providerRefReceiptSha256 = $receiptHash
    releaseAssetRecordSha256 = $releaseRecordHash; assets = $assetRecords.ToArray()
    publicationStatus = 'retained-unpublished'; publicReleaseCreated = $false; tagCreated = $false; legacyMoved = $false
    retainedAtUtc = [DateTime]::UtcNow.ToString('o')
}
$candidatePath = Join-Path $evidenceDirectory 'candidate.json'
[IO.File]::WriteAllText($candidatePath, (($candidate | ConvertTo-Json -Depth 8) + "`n"), $utf8)
$candidateHash = (Get-FileHash -LiteralPath $candidatePath -Algorithm SHA256).Hash.ToLowerInvariant()
[IO.File]::WriteAllText((Join-Path $evidenceDirectory 'CANDIDATE_SHA256.txt'), "$candidateHash  candidate.json`n", $utf8)
& (Join-Path $PSScriptRoot 'verify-beta1-assets.ps1') -CandidateRoot $CandidateRoot `
    -ExpectedSourceCommit $sourceCommit -ExpectedToolchainLockSha256 $lockHash
Write-Host "Retained source-bound Beta 1 Candidate without tag, publication, feed, or legacy movement: $CandidateRoot"
