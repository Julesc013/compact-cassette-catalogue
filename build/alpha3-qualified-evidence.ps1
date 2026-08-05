. (Join-Path $PSScriptRoot 'alpha3-external-evidence.ps1')

function Assert-C3Alpha3QualifiedEvidence {
    param(
        [Parameter(Mandatory = $true)][string]$RepositoryRoot,
        [Parameter(Mandatory = $true)]$Manifest,
        [Parameter(Mandatory = $true)][string]$Configuration,
        [Parameter(Mandatory = $true)][string]$PackageSource
    )

    $recordPath = Join-Path $RepositoryRoot 'release\validation\1.3.0-alpha.3-qualified.json'
    $humanRecordPath = Join-Path $RepositoryRoot 'release\validation\1.3.0-alpha.3-qualified.md'
    $record = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
    $releaseAssetDirectory = Join-Path $RepositoryRoot "artifacts\evidence\release-assets\$($Manifest.releaseLabel)"
    $releaseAssetPath = Join-Path $releaseAssetDirectory 'release-assets.json'
    $releaseAssets = Get-Content -LiteralPath $releaseAssetPath -Raw | ConvertFrom-Json
    $distributionDirectory = Join-Path $RepositoryRoot "artifacts\distributions\$($Manifest.releaseLabel)"
    $distributionChecksumPath = Join-Path $distributionDirectory 'SHA256SUMS.txt'
    if ([int]$record.schemaVersion -ne 1 -or
            [string]$record.status -cne 'pass' -or
            [string]$record.releaseLabel -cne [string]$Manifest.releaseLabel -or
            [string]$record.packageSource -cne $PackageSource -or
            [string]$record.toolchainLockSha256 -cne [string]$releaseAssets.toolchainLockSha256 -or
            [string]$record.historicalGate1EvidenceSha256 -notmatch '^[0-9a-f]{64}$' -or
            [string]$record.targetQualificationEvidenceSha256 -notmatch '^[0-9a-f]{64}$' -or
            [string]::IsNullOrWhiteSpace([string]$record.recordedAtUtc)) {
        throw 'Alpha 3 qualified evidence record does not match its release/source/lock and completed-gate identity.'
    }

    $historicalPath = Join-Path $RepositoryRoot 'artifacts\evidence\historical-gate1\completion.json'
    $targetPath = Join-Path $RepositoryRoot "artifacts\evidence\target-qualification\$($Manifest.releaseLabel)\qualification.json"
    if ((Get-FileHash -LiteralPath $historicalPath -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$record.historicalGate1EvidenceSha256 -or
            (Get-FileHash -LiteralPath $targetPath -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$record.targetQualificationEvidenceSha256) {
        throw 'Alpha 3 qualified record does not match retained historical/target evidence-index hashes.'
    }
    Assert-C3Alpha3HistoricalGate1Evidence -Path $historicalPath | Out-Null
    $target = Assert-C3Alpha3TargetQualificationEvidence -Path $targetPath -PackageSource $PackageSource `
        -ToolchainLockSha256 ([string]$record.toolchainLockSha256) -Manifest $Manifest

    $releaseAssetHash = (Get-FileHash -LiteralPath $releaseAssetPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $distributionChecksumHash = (Get-FileHash -LiteralPath $distributionChecksumPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $closurePath = Join-Path $RepositoryRoot 'artifacts\evidence\build\candidate-source-closure.json'
    $sourceReproPath = Join-Path $RepositoryRoot "artifacts\evidence\source-reproducibility\$($Manifest.releaseLabel)\source-reproducibility.json"
    $sourceRepro = Get-Content -LiteralPath $sourceReproPath -Raw | ConvertFrom-Json
    if ([string]$record.releaseAssetRecordSha256 -cne $releaseAssetHash -or
            [string]$record.distributionChecksumManifestSha256 -cne $distributionChecksumHash -or
            [string]$record.candidateSourceClosureSha256 -cne (Get-FileHash -LiteralPath $closurePath -Algorithm SHA256).Hash.ToLowerInvariant() -or
            [string]$record.sourceReproducibilityRecordSha256 -cne (Get-FileHash -LiteralPath $sourceReproPath -Algorithm SHA256).Hash.ToLowerInvariant() -or
            [string]$sourceRepro.status -cne 'pass' -or -not [bool]$sourceRepro.includesSetup -or -not [bool]$sourceRepro.pathDistinct -or
            [string]$sourceRepro.sourceCommit -cne $PackageSource -or
            [string]$sourceRepro.toolchainLockSha256 -cne [string]$record.toolchainLockSha256 -or
            @($sourceRepro.authoritativeDistribution.PSObject.Properties).Count -ne 7) {
        throw 'Alpha 3 qualified evidence does not match final closure, complete source reproduction, or release-asset evidence.'
    }

    $assetRecords = @($record.assets)
    $releaseAssetRecords = @($releaseAssets.assets)
    if ($assetRecords.Count -ne 6 -or $releaseAssetRecords.Count -ne 6) {
        throw 'Alpha 3 qualified evidence must contain exactly six assets.'
    }
    for ($index = 0; $index -lt 6; $index++) {
        $asset = $assetRecords[$index]
        $releaseAsset = $releaseAssetRecords[$index]
        $assetPath = Join-Path $distributionDirectory ([string]$releaseAsset.name)
        $entryDirectory = if ([string]$releaseAsset.kind -ceq 'portable') {
            Join-Path $RepositoryRoot "artifacts\evidence\packages\$($Manifest.releaseLabel)"
        }
        else {
            Join-Path $RepositoryRoot "artifacts\evidence\setup-packages\$($Manifest.releaseLabel)"
        }
        $entryPath = Join-Path $entryDirectory ([string]$releaseAsset.entryManifestName)
        $assetHash = (Get-FileHash -LiteralPath $assetPath -Algorithm SHA256).Hash.ToLowerInvariant()
        $entryHash = (Get-FileHash -LiteralPath $entryPath -Algorithm SHA256).Hash.ToLowerInvariant()
        if ([string]$asset.kind -cne [string]$releaseAsset.kind -or
                [string]$asset.lane -cne [string]$releaseAsset.lane -or
                [string]$asset.name -cne [string]$releaseAsset.name -or
                [string]$asset.sha256 -cne $assetHash -or
                [string]$asset.sha256 -cne [string]$releaseAsset.sha256 -or
                [string]$asset.entryManifestName -cne [string]$releaseAsset.entryManifestName -or
                [string]$asset.entryManifestSha256 -cne $entryHash -or
                [string]$asset.entryManifestSha256 -cne [string]$releaseAsset.entryManifestSha256) {
            throw "Alpha 3 qualified asset evidence does not match '$($releaseAsset.name)'."
        }
        $targetCollection = if ([string]$asset.kind -ceq 'portable') { @($target.runtime) } else { @($target.setup) }
        $targetAsset = @($targetCollection | Where-Object { [string]$_.lane -ceq [string]$asset.lane })
        if ($targetAsset.Count -ne 1 -or [string]$targetAsset[0].packageSha256 -cne $assetHash -or
                [string]$targetAsset[0].entryManifestSha256 -cne $entryHash) {
            throw "Target qualification is not bound to the retained '$($asset.name)' bytes."
        }
    }

    $applicationRecords = @($record.applicationBuildEvidence)
    $setupRecords = @($record.setupBuildEvidence)
    if ($applicationRecords.Count -ne 3 -or $setupRecords.Count -ne 3) {
        throw 'Alpha 3 qualified evidence must contain three application and three setup build records.'
    }
    foreach ($lane in @($Manifest.lanes)) {
        $application = @($applicationRecords | Where-Object { [string]$_.lane -ceq [string]$lane.id })
        $applicationDirectory = Join-Path $RepositoryRoot "artifacts\evidence\build\$($lane.id)\$Configuration"
        if ($application.Count -ne 1 -or
                [string]$application[0].toolchainEvidenceSha256 -cne (Get-FileHash -LiteralPath (Join-Path $applicationDirectory 'toolchain.json') -Algorithm SHA256).Hash.ToLowerInvariant() -or
                [string]$application[0].binaryLogSha256 -cne (Get-FileHash -LiteralPath (Join-Path $applicationDirectory 'msbuild.binlog') -Algorithm SHA256).Hash.ToLowerInvariant()) {
            throw "Alpha 3 application build evidence does not match '$($lane.id)'."
        }
        $setup = @($setupRecords | Where-Object { [string]$_.lane -ceq [string]$lane.id })
        $setupDirectory = Join-Path $RepositoryRoot "artifacts\evidence\setup-build\$($lane.id)\$Configuration"
        if ($setup.Count -ne 1 -or
                [string]$setup[0].toolchainEvidenceSha256 -cne (Get-FileHash -LiteralPath (Join-Path $setupDirectory 'toolchain.json') -Algorithm SHA256).Hash.ToLowerInvariant() -or
                [string]$setup[0].installerBinaryLogSha256 -cne (Get-FileHash -LiteralPath (Join-Path $setupDirectory 'installer\msbuild.binlog') -Algorithm SHA256).Hash.ToLowerInvariant() -or
                [string]$setup[0].uninstallerBinaryLogSha256 -cne (Get-FileHash -LiteralPath (Join-Path $setupDirectory 'uninstaller\msbuild.binlog') -Algorithm SHA256).Hash.ToLowerInvariant()) {
            throw "Alpha 3 setup build evidence does not match '$($lane.id)'."
        }
    }

    $humanRecord = Get-Content -LiteralPath $humanRecordPath -Raw
    foreach ($requiredText in @(
            "Package source commit ``C``: ``$PackageSource``",
            "External toolchain-lock SHA-256: ``$($record.toolchainLockSha256)``",
            'Overall Alpha 3 result: PASS',
            'Publication: retained and intentionally unpublished.',
            'Beta authority: technical eligibility only')) {
        if (-not $humanRecord.Contains($requiredText)) {
            throw "Alpha 3 human qualification record is missing: $requiredText"
        }
    }
    if ($humanRecord.Contains('TEMPLATE') -or $humanRecord.Contains('FAIL / BLOCKED') -or $humanRecord.Contains('UNEXPECTED')) {
        throw 'Alpha 3 human qualification record still contains an uncompleted or failed gate marker.'
    }
    return $record
}
