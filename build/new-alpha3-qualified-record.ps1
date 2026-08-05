[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha3-external-evidence.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$recordPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.3-qualified.json'
$template = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
if ([string]$template.status -cne 'template') { throw "Alpha 3 qualified record is not an unpopulated template: $recordPath" }
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) {
    throw 'Alpha 3 qualification-record generation requires clean frozen package source C.'
}
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0a3
if ($LASTEXITCODE -eq 0) { throw 'Alpha 3 evidence commit E must be prepared before creating v1.3.0a3.' }

$packageSource = (& git -C $repositoryRoot rev-parse HEAD).Trim()
& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-setup-builds.ps1') -Configuration $Configuration -ExpectedSourceCommit $packageSource
& (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-alpha3-assets.ps1') -Configuration $Configuration -RequireCandidateEvidence

$releaseAssetDirectory = Join-Path $repositoryRoot "artifacts\evidence\release-assets\$($manifest.releaseLabel)"
$releaseAssetPath = Join-Path $releaseAssetDirectory 'release-assets.json'
$releaseAssets = Get-Content -LiteralPath $releaseAssetPath -Raw | ConvertFrom-Json
if ([string]$releaseAssets.sourceCommit -cne $packageSource -or [string]$releaseAssets.classification -cne 'Candidate') {
    throw 'Alpha 3 release assets are not Candidate bytes from current package source C.'
}
$lockSha256 = [string]$releaseAssets.toolchainLockSha256

$historicalPath = Join-Path $repositoryRoot 'artifacts\evidence\historical-gate1\completion.json'
$targetPath = Join-Path $repositoryRoot "artifacts\evidence\target-qualification\$($manifest.releaseLabel)\qualification.json"
Assert-C3Alpha3HistoricalGate1Evidence -Path $historicalPath | Out-Null
$target = Assert-C3Alpha3TargetQualificationEvidence -Path $targetPath -PackageSource $packageSource `
    -ToolchainLockSha256 $lockSha256 -Manifest $manifest

$sourceReproPath = Join-Path $repositoryRoot "artifacts\evidence\source-reproducibility\$($manifest.releaseLabel)\source-reproducibility.json"
$sourceRepro = Get-Content -LiteralPath $sourceReproPath -Raw | ConvertFrom-Json
if ([string]$sourceRepro.status -cne 'pass' -or -not [bool]$sourceRepro.includesSetup -or
        -not [bool]$sourceRepro.pathDistinct -or [string]$sourceRepro.sourceCommit -cne $packageSource -or
        [string]$sourceRepro.toolchainLockSha256 -cne $lockSha256 -or
        @($sourceRepro.authoritativeDistribution.PSObject.Properties).Count -ne 7) {
    throw 'Alpha 3 source-reproducibility evidence is incomplete or not bound to current source/lock.'
}

$distributionDirectory = Join-Path $repositoryRoot "artifacts\distributions\$($manifest.releaseLabel)"
$assetRecords = @($releaseAssets.assets | ForEach-Object {
    $asset = $_
    $targetCollection = if ([string]$asset.kind -ceq 'portable') { @($target.runtime) } else { @($target.setup) }
    $targetAsset = @($targetCollection | Where-Object { [string]$_.lane -ceq [string]$asset.lane })
    if ($targetAsset.Count -ne 1 -or [string]$targetAsset[0].packageSha256 -cne [string]$asset.sha256 -or
            [string]$targetAsset[0].entryManifestSha256 -cne [string]$asset.entryManifestSha256) {
        throw "Target evidence does not qualify exact retained asset '$($asset.name)'."
    }
    $actualHash = (Get-FileHash -LiteralPath (Join-Path $distributionDirectory ([string]$asset.name)) -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($actualHash -cne [string]$asset.sha256) { throw "Retained distribution asset changed: $($asset.name)" }
    [ordered]@{
        kind = [string]$asset.kind
        lane = [string]$asset.lane
        name = [string]$asset.name
        sha256 = [string]$asset.sha256
        entryManifestName = [string]$asset.entryManifestName
        entryManifestSha256 = [string]$asset.entryManifestSha256
    }
})
$applicationBuildRecords = @($manifest.lanes | ForEach-Object {
    $directory = Join-Path $repositoryRoot "artifacts\evidence\build\$($_.id)\$Configuration"
    [ordered]@{
        lane = [string]$_.id
        toolchainEvidenceSha256 = (Get-FileHash -LiteralPath (Join-Path $directory 'toolchain.json') -Algorithm SHA256).Hash.ToLowerInvariant()
        binaryLogSha256 = (Get-FileHash -LiteralPath (Join-Path $directory 'msbuild.binlog') -Algorithm SHA256).Hash.ToLowerInvariant()
    }
})
$setupBuildRecords = @($manifest.lanes | ForEach-Object {
    $directory = Join-Path $repositoryRoot "artifacts\evidence\setup-build\$($_.id)\$Configuration"
    [ordered]@{
        lane = [string]$_.id
        toolchainEvidenceSha256 = (Get-FileHash -LiteralPath (Join-Path $directory 'toolchain.json') -Algorithm SHA256).Hash.ToLowerInvariant()
        installerBinaryLogSha256 = (Get-FileHash -LiteralPath (Join-Path $directory 'installer\msbuild.binlog') -Algorithm SHA256).Hash.ToLowerInvariant()
        uninstallerBinaryLogSha256 = (Get-FileHash -LiteralPath (Join-Path $directory 'uninstaller\msbuild.binlog') -Algorithm SHA256).Hash.ToLowerInvariant()
    }
})

$record = [ordered]@{
    schemaVersion = 1
    status = 'pass'
    releaseLabel = [string]$manifest.releaseLabel
    packageSource = $packageSource
    toolchainLockSha256 = $lockSha256
    historicalGate1EvidenceSha256 = (Get-FileHash -LiteralPath $historicalPath -Algorithm SHA256).Hash.ToLowerInvariant()
    targetQualificationEvidenceSha256 = (Get-FileHash -LiteralPath $targetPath -Algorithm SHA256).Hash.ToLowerInvariant()
    candidateSourceClosureSha256 = (Get-FileHash -LiteralPath (Join-Path $repositoryRoot 'artifacts\evidence\build\candidate-source-closure.json') -Algorithm SHA256).Hash.ToLowerInvariant()
    sourceReproducibilityRecordSha256 = (Get-FileHash -LiteralPath $sourceReproPath -Algorithm SHA256).Hash.ToLowerInvariant()
    releaseAssetRecordSha256 = (Get-FileHash -LiteralPath $releaseAssetPath -Algorithm SHA256).Hash.ToLowerInvariant()
    distributionChecksumManifestSha256 = (Get-FileHash -LiteralPath (Join-Path $distributionDirectory 'SHA256SUMS.txt') -Algorithm SHA256).Hash.ToLowerInvariant()
    assets = $assetRecords
    applicationBuildEvidence = $applicationBuildRecords
    setupBuildEvidence = $setupBuildRecords
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
}
[IO.File]::WriteAllText($recordPath, (($record | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
Write-Host "Populated Alpha 3 qualification evidence for package source C '$packageSource'. Complete the human qualification record, then commit exactly those two files as E."
