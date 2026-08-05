[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$OutputDirectory,
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
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) { $OutputDirectory = Join-Path $repositoryRoot "artifacts\distributions\$($manifest.releaseLabel)" }
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) { $EvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\release-assets\$($manifest.releaseLabel)" }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)
New-Item -ItemType Directory -Path $OutputDirectory,$EvidenceDirectory -Force | Out-Null

& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration `
    -PackageDirectory $portableDirectory -EvidenceDirectory $portableEvidenceDirectory `
    -RequireCandidateEvidence:$RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration `
    -PortablePackageDirectory $portableDirectory -PortableEvidenceDirectory $portableEvidenceDirectory `
    -PackageDirectory $setupDirectory -EvidenceDirectory $setupEvidenceDirectory `
    -RequireCandidateEvidence:$RequireCandidateEvidence

$assetRecords = New-Object Collections.Generic.List[Object]
$checksumLines = New-Object Collections.Generic.List[String]
$sourceCommit = $null
$lockSha256 = $null
foreach ($kind in @('portable', 'setup')) {
    foreach ($lane in $lanes) {
        $packageName = if ($kind -ceq 'portable') { [string]$lane.packageName } else { [string]$lane.setupPackageName }
        $sourceDirectory = if ($kind -ceq 'portable') { $portableDirectory } else { $setupDirectory }
        $entryEvidenceDirectory = if ($kind -ceq 'portable') { $portableEvidenceDirectory } else { $setupEvidenceDirectory }
        $sourcePath = Join-Path $sourceDirectory $packageName
        $destinationPath = Join-Path $OutputDirectory $packageName
        [IO.File]::Copy($sourcePath, $destinationPath, $true)
        $packageHash = (Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256).Hash.ToLowerInvariant()
        if ((Get-FileHash -LiteralPath $destinationPath -Algorithm SHA256).Hash.ToLowerInvariant() -cne $packageHash) { throw "Copied asset changed bytes: $packageName" }
        $entryManifestPath = Join-Path $entryEvidenceDirectory "$packageName.entries.json"
        $entryManifest = Get-Content -LiteralPath $entryManifestPath -Raw | ConvertFrom-Json
        if ($null -eq $sourceCommit) { $sourceCommit = [string]$entryManifest.sourceCommit; $lockSha256 = [string]$entryManifest.toolchainLockSha256 }
        if ([string]$entryManifest.sourceCommit -cne $sourceCommit -or [string]$entryManifest.toolchainLockSha256 -cne $lockSha256) {
            throw 'The six release assets do not share one source commit and external toolchain lock.'
        }
        $checksumLines.Add("$packageHash  $packageName")
        $assetRecords.Add([ordered]@{
                name = $packageName
                kind = $kind
                lane = [string]$lane.id
                size = [long](Get-Item -LiteralPath $sourcePath).Length
                sha256 = $packageHash
                entryManifestName = "$packageName.entries.json"
                entryManifestSha256 = (Get-FileHash -LiteralPath $entryManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()
            })
    }
}
$utf8 = New-Object Text.UTF8Encoding($false)
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'SHA256SUMS.txt'), (($checksumLines.ToArray() -join "`n") + "`n"), $utf8)
$releaseRecord = [ordered]@{
    schemaVersion = 1
    classification = $(if ($RequireCandidateEvidence) { 'Candidate' } else { 'Preparation' })
    product = [string]$manifest.product
    releaseVersion = [string]$manifest.releaseVersion
    releaseStage = [string]$manifest.releaseStage
    releaseLabel = [string]$manifest.releaseLabel
    releaseTag = [string]$manifest.releaseTag
    releaseChannel = [string]$manifest.releaseChannel
    publicationStatus = [string]$manifest.publicationStatus
    sourceCommit = $sourceCommit
    toolchainLockSha256 = $lockSha256
    assets = $assetRecords.ToArray()
}
$recordPath = Join-Path $EvidenceDirectory 'release-assets.json'
[IO.File]::WriteAllText($recordPath, (($releaseRecord | ConvertTo-Json -Depth 8) + "`n"), $utf8)
$recordHash = (Get-FileHash -LiteralPath $recordPath -Algorithm SHA256).Hash.ToLowerInvariant()
[IO.File]::WriteAllText((Join-Path $EvidenceDirectory 'RELEASE_ASSETS_SHA256.txt'), "$recordHash  release-assets.json`n", $utf8)
Write-Host "Assembled the exact six-archive $($manifest.releaseLabel) asset set and checksum manifest: $OutputDirectory"
