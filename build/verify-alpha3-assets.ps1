[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$DistributionDirectory,
    [string]$EvidenceDirectory,
    [switch]$RequireCandidateEvidence
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
$portableDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)"
$portableEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)"
$setupDirectory = Join-Path $repositoryRoot "artifacts\setup\packages\$($manifest.releaseLabel)"
$setupEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\setup-packages\$($manifest.releaseLabel)"
if ([string]::IsNullOrWhiteSpace($DistributionDirectory)) { $DistributionDirectory = Join-Path $repositoryRoot "artifacts\distributions\$($manifest.releaseLabel)" }
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) { $EvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\release-assets\$($manifest.releaseLabel)" }
$DistributionDirectory = [IO.Path]::GetFullPath($DistributionDirectory)
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)

& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration `
    -PackageDirectory $portableDirectory -EvidenceDirectory $portableEvidenceDirectory `
    -RequireCandidateEvidence:$RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration `
    -PortablePackageDirectory $portableDirectory -PortableEvidenceDirectory $portableEvidenceDirectory `
    -PackageDirectory $setupDirectory -EvidenceDirectory $setupEvidenceDirectory `
    -RequireCandidateEvidence:$RequireCandidateEvidence

$expectedAssetNames = @($lanes.packageName) + @($lanes.setupPackageName) + @('SHA256SUMS.txt')
$actualAssetNames = @(Get-ChildItem -LiteralPath $DistributionDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualAssetNames -join "`n") -cne (($expectedAssetNames | Sort-Object) -join "`n")) {
    throw "Alpha 3 distribution is not the exact six-ZIP/checksum set: $($actualAssetNames -join ', ')"
}
$expectedEvidenceNames = @('release-assets.json', 'RELEASE_ASSETS_SHA256.txt')
$actualEvidenceNames = @(Get-ChildItem -LiteralPath $EvidenceDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualEvidenceNames -join "`n") -cne (($expectedEvidenceNames | Sort-Object) -join "`n")) { throw 'Alpha 3 release-asset evidence is not the exact record/checksum pair.' }
$recordPath = Join-Path $EvidenceDirectory 'release-assets.json'
$recordHash = (Get-FileHash -LiteralPath $recordPath -Algorithm SHA256).Hash.ToLowerInvariant()
if ((Get-Content -LiteralPath (Join-Path $EvidenceDirectory 'RELEASE_ASSETS_SHA256.txt') -Raw) -cne "$recordHash  release-assets.json`n") { throw 'Release-asset evidence checksum is not exact.' }
$record = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
$expectedClassification = if ($RequireCandidateEvidence) { 'Candidate' } else { [string]$record.classification }
if ([int]$record.schemaVersion -ne 1 -or [string]$record.classification -cne $expectedClassification -or
        [string]$record.product -cne [string]$manifest.product -or [string]$record.releaseVersion -cne [string]$manifest.releaseVersion -or
        [string]$record.releaseStage -cne [string]$manifest.releaseStage -or [string]$record.releaseLabel -cne [string]$manifest.releaseLabel -or
        [string]$record.releaseTag -cne [string]$manifest.releaseTag -or [string]$record.releaseChannel -cne 'alpha' -or
        [string]$record.publicationStatus -cne 'retained-unpublished' -or [string]$record.sourceCommit -notmatch '^[0-9a-f]{40}$' -or
        [string]$record.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$' -or @($record.assets).Count -ne 6) {
    throw 'Alpha 3 release-asset record has invalid classification/release/source/lock identity.'
}
$checksumLines = @(Get-Content -LiteralPath (Join-Path $DistributionDirectory 'SHA256SUMS.txt'))
if ($checksumLines.Count -ne 6) { throw 'Alpha 3 SHA256SUMS.txt must contain exactly six lines.' }
$expectedOrder = @($lanes.packageName) + @($lanes.setupPackageName)
for ($index = 0; $index -lt $expectedOrder.Count; $index++) {
    $assetName = [string]$expectedOrder[$index]
    $assetPath = Join-Path $DistributionDirectory $assetName
    $hash = (Get-FileHash -LiteralPath $assetPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $asset = @($record.assets | Where-Object { [string]$_.name -ceq $assetName })
    if ($checksumLines[$index] -cne "$hash  $assetName" -or $asset.Count -ne 1 -or [string]$asset[0].sha256 -cne $hash -or
            [long]$asset[0].size -ne [long](Get-Item -LiteralPath $assetPath).Length) {
        throw "Alpha 3 release asset/checksum/evidence mismatch: $assetName"
    }
    $sourceDirectory = if ([string]$asset[0].kind -ceq 'portable') { $portableDirectory } elseif ([string]$asset[0].kind -ceq 'setup') { $setupDirectory } else { throw "Unknown asset kind for $assetName" }
    if ((Get-FileHash -LiteralPath (Join-Path $sourceDirectory $assetName) -Algorithm SHA256).Hash.ToLowerInvariant() -cne $hash) {
        throw "Alpha 3 assembled asset differs from its independently verified package: $assetName"
    }
    $entryDirectory = if ([string]$asset[0].kind -ceq 'portable') { $portableEvidenceDirectory } else { $setupEvidenceDirectory }
    $entryPath = Join-Path $entryDirectory ([string]$asset[0].entryManifestName)
    if ([string]$asset[0].entryManifestName -cne "$assetName.entries.json" -or
            [string]$asset[0].entryManifestSha256 -cne (Get-FileHash -LiteralPath $entryPath -Algorithm SHA256).Hash.ToLowerInvariant()) {
        throw "Alpha 3 asset is not bound to its authenticated entry manifest: $assetName"
    }
}
Write-Host 'Verified the retained Alpha 3 distribution as exactly six independently verified ZIPs, one checksum manifest, one release identity, one source, and one lock.'
