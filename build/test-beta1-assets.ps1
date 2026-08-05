[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-contract.ps1')

$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ("c3-beta1-assets-" + [Guid]::NewGuid().ToString('N'))
$sourceCommit = 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
$lockHash = $null
$candidateRoot = Join-Path $temporaryRoot $sourceCommit
$evidenceDirectory = Join-Path $candidateRoot 'evidence'
$entriesDirectory = Join-Path $evidenceDirectory 'entries'
$utf8 = New-Object Text.UTF8Encoding($false)

function Write-JsonFile {
    param([string]$Path, $Value)
    [IO.File]::WriteAllText($Path, (($Value | ConvertTo-Json -Depth 10) + "`n"), $utf8)
}
function Write-HashReceipt {
    param([string]$Directory, [string]$FileName, [string]$ReceiptName)
    $hash = (Get-FileHash -LiteralPath (Join-Path $Directory $FileName) -Algorithm SHA256).Hash.ToLowerInvariant()
    [IO.File]::WriteAllText((Join-Path $Directory $ReceiptName), "$hash  $FileName`n", $utf8)
    return $hash
}
function Assert-Rejected {
    param([string]$Name, [scriptblock]$Mutation, [string]$Pattern, [scriptblock]$Restore)
    & $Mutation
    try {
        & (Join-Path $PSScriptRoot 'verify-beta1-assets.ps1') -CandidateRoot $candidateRoot -ManifestPath $manifestPath | Out-Null
        throw "$Name unexpectedly passed."
    }
    catch {
        if ($_.Exception.Message -notmatch $Pattern) { throw }
    }
    & $Restore
}

try {
    New-Item -ItemType Directory -Path $entriesDirectory -Force | Out-Null
    $manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
    $manifest.releaseStage = 'Beta 1'; $manifest.releaseLabel = '1.3.0b1'; $manifest.releaseTag = 'v1.3.0b1'
    $manifest.releaseChannel = 'beta'; $manifest.assemblyProductVersion = '1.3.0b1'
    for ($index = 0; $index -lt 3; $index++) {
        $manifest.lanes[$index].packageName = $script:C3Beta1PortableAssetNames[$index]
        $manifest.lanes[$index].setupPackageName = $script:C3Beta1SetupAssetNames[$index]
    }
    $manifestPath = Join-Path $temporaryRoot 'lanes.json'
    Write-JsonFile -Path $manifestPath -Value $manifest

    $receipt = [ordered]@{
        remoteName = 'origin'; remoteUrl = 'https://github.com/Julesc013/compact-cassette-catalogue.git'
        providerRef = 'refs/heads/dev/1.x'; remoteTrackingRef = 'refs/remotes/origin/dev/1.x'
        fetchedCommit = $sourceCommit; fetchedAtUtc = '2026-08-06T00:00:00.0000000Z'
    }
    $lock = [ordered]@{
        schemaVersion = 3; status = 'locked'; sourceCommit = $sourceCommit
        expectedRemoteRef = 'refs/remotes/origin/dev/1.x'; providerRefReceipt = $receipt
    }
    Write-JsonFile -Path (Join-Path $evidenceDirectory 'toolchain-lock.json') -Value $lock
    $lockHash = Write-HashReceipt -Directory $evidenceDirectory -FileName 'toolchain-lock.json' -ReceiptName 'TOOLCHAIN_LOCK_SHA256.txt'
    Write-JsonFile -Path (Join-Path $evidenceDirectory 'provider-ref-receipt.json') -Value $receipt
    $receiptHash = Write-HashReceipt -Directory $evidenceDirectory -FileName 'provider-ref-receipt.json' -ReceiptName 'PROVIDER_REF_RECEIPT_SHA256.txt'

    $assets = New-Object Collections.Generic.List[Object]
    $checksumLines = New-Object Collections.Generic.List[String]
    for ($index = 0; $index -lt 6; $index++) {
        $name = $script:C3Beta1AssetNames[$index]
        $kind = if ($index -lt 3) { 'portable' } else { 'setup' }
        $lane = $script:C3Beta1LaneIds[$index % 3]
        $assetPath = Join-Path $candidateRoot $name
        [IO.File]::WriteAllText($assetPath, "synthetic $name`n", $utf8)
        $assetHash = (Get-FileHash -LiteralPath $assetPath -Algorithm SHA256).Hash.ToLowerInvariant()
        $entry = [ordered]@{
            schemaVersion = 2; packageName = $name; packageSha256 = $assetHash
            releaseLabel = '1.3.0b1'; sourceCommit = $sourceCommit; toolchainLockSha256 = $lockHash
        }
        $entryName = "$name.entries.json"
        $entryPath = Join-Path $entriesDirectory $entryName
        Write-JsonFile -Path $entryPath -Value $entry
        $assets.Add([ordered]@{
                name = $name; kind = $kind; lane = $lane; size = [long](Get-Item -LiteralPath $assetPath).Length
                sha256 = $assetHash; entryManifestName = $entryName
                entryManifestSha256 = (Get-FileHash -LiteralPath $entryPath -Algorithm SHA256).Hash.ToLowerInvariant()
            })
        $checksumLines.Add("$assetHash  $name")
    }
    [IO.File]::WriteAllText((Join-Path $candidateRoot 'SHA256SUMS.txt'), (($checksumLines.ToArray() -join "`n") + "`n"), $utf8)
    $releaseRecord = [ordered]@{
        schemaVersion = 1; classification = 'Candidate'; releaseStage = 'Beta 1'; releaseLabel = '1.3.0b1'
        releaseTag = 'v1.3.0b1'; releaseChannel = 'beta'; publicationStatus = 'retained-unpublished'
        sourceCommit = $sourceCommit; toolchainLockSha256 = $lockHash; assets = $assets.ToArray()
    }
    Write-JsonFile -Path (Join-Path $evidenceDirectory 'release-assets.json') -Value $releaseRecord
    $releaseHash = Write-HashReceipt -Directory $evidenceDirectory -FileName 'release-assets.json' -ReceiptName 'RELEASE_ASSETS_SHA256.txt'
    $candidate = [ordered]@{
        schemaVersion = 1; classification = 'Candidate'; releaseLabel = '1.3.0b1'; sourceCommit = $sourceCommit
        toolchainLockSha256 = $lockHash; providerRefReceiptSha256 = $receiptHash; releaseAssetRecordSha256 = $releaseHash
        assets = $assets.ToArray(); publicationStatus = 'retained-unpublished'; publicReleaseCreated = $false
        tagCreated = $false; legacyMoved = $false
    }
    $candidatePath = Join-Path $evidenceDirectory 'candidate.json'
    Write-JsonFile -Path $candidatePath -Value $candidate
    [void](Write-HashReceipt -Directory $evidenceDirectory -FileName 'candidate.json' -ReceiptName 'CANDIDATE_SHA256.txt')

    & (Join-Path $PSScriptRoot 'verify-beta1-assets.ps1') -CandidateRoot $candidateRoot -ManifestPath $manifestPath | Out-Null

    $firstAsset = Join-Path $candidateRoot $script:C3Beta1AssetNames[0]
    $originalAsset = [IO.File]::ReadAllBytes($firstAsset)
    Assert-Rejected -Name 'altered Candidate ZIP' -Pattern 'checksum or asset closure' `
        -Mutation { [IO.File]::AppendAllText($firstAsset, 'altered', $utf8) } `
        -Restore { [IO.File]::WriteAllBytes($firstAsset, $originalAsset) }

    $candidate.publicReleaseCreated = $true
    Assert-Rejected -Name 'public-release claim' -Pattern 'retained-unpublished, no-tag, no-ledger' `
        -Mutation { Write-JsonFile -Path $candidatePath -Value $candidate; [void](Write-HashReceipt -Directory $evidenceDirectory -FileName 'candidate.json' -ReceiptName 'CANDIDATE_SHA256.txt') } `
        -Restore { $candidate.publicReleaseCreated = $false; Write-JsonFile -Path $candidatePath -Value $candidate; [void](Write-HashReceipt -Directory $evidenceDirectory -FileName 'candidate.json' -ReceiptName 'CANDIDATE_SHA256.txt') }

    $manifest.releaseLabel = '1.3.0a3'
    Assert-Rejected -Name 'mixed Alpha manifest' -Pattern 'Beta 1 manifest property' `
        -Mutation { Write-JsonFile -Path $manifestPath -Value $manifest } `
        -Restore { $manifest.releaseLabel = '1.3.0b1'; Write-JsonFile -Path $manifestPath -Value $manifest }

    Write-Host 'Beta 1 asset controls accepted one exact source-bound six-asset Candidate and rejected byte alteration, publication, and mixed-stage identity.'
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) { Remove-Item -LiteralPath $temporaryRoot -Recurse -Force }
}

