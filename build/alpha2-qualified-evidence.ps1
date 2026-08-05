function Assert-C3Alpha2QualifiedEvidence {
    param(
        [Parameter(Mandatory = $true)][string]$RepositoryRoot,
        [Parameter(Mandatory = $true)]$Manifest,
        [Parameter(Mandatory = $true)][string]$Configuration,
        [Parameter(Mandatory = $true)][string]$PackageSource
    )

    $recordPath = Join-Path $RepositoryRoot 'release\validation\1.3.0-alpha.2-qualified.json'
    $record = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
    $packageDirectory = Join-Path $RepositoryRoot "artifacts\packages\$($Manifest.releaseLabel)"
    $packageEvidenceDirectory = Join-Path $RepositoryRoot "artifacts\evidence\packages\$($Manifest.releaseLabel)"
    $firstEntryPath = Join-Path $packageEvidenceDirectory "$($Manifest.lanes[0].packageName).entries.json"
    $firstEntry = Get-Content -LiteralPath $firstEntryPath -Raw | ConvertFrom-Json
    if ([int]$record.schemaVersion -ne 1 -or
            [string]$record.status -cne 'pass' -or
            [string]$record.releaseLabel -cne [string]$Manifest.releaseLabel -or
            [string]$record.packageSource -cne $PackageSource -or
            [string]$record.toolchainLockSha256 -cne [string]$firstEntry.toolchainLockSha256 -or
            [string]::IsNullOrWhiteSpace([string]$record.recordedAtUtc)) {
        throw 'Alpha 2 qualified evidence record does not match its release/source/lock identity.'
    }

    $packageRecords = @($record.packages)
    if ($packageRecords.Count -ne @($Manifest.lanes).Count) {
        throw 'Alpha 2 qualified evidence record does not contain exactly three packages.'
    }
    foreach ($index in 0..(@($Manifest.lanes).Count - 1)) {
        $lane = $Manifest.lanes[$index]
        $packageRecord = $packageRecords[$index]
        $entryName = "$($lane.packageName).entries.json"
        $packageHash = (Get-FileHash -LiteralPath (Join-Path $packageDirectory ([string]$lane.packageName)) -Algorithm SHA256).Hash.ToLowerInvariant()
        $entryHash = (Get-FileHash -LiteralPath (Join-Path $packageEvidenceDirectory $entryName) -Algorithm SHA256).Hash.ToLowerInvariant()
        if ([string]$packageRecord.name -cne [string]$lane.packageName -or
                [string]$packageRecord.sha256 -cne $packageHash -or
                [string]$packageRecord.entryManifestName -cne $entryName -or
                [string]$packageRecord.entryManifestSha256 -cne $entryHash) {
            throw "Alpha 2 qualified evidence package hashes do not match '$($lane.id)'."
        }
    }

    $packageChecksumHash = (Get-FileHash -LiteralPath (Join-Path $packageDirectory 'SHA256SUMS.txt') -Algorithm SHA256).Hash.ToLowerInvariant()
    $entryChecksumHash = (Get-FileHash -LiteralPath (Join-Path $packageEvidenceDirectory 'ENTRY_MANIFEST_SHA256SUMS.txt') -Algorithm SHA256).Hash.ToLowerInvariant()
    $closurePath = Join-Path $RepositoryRoot 'artifacts\evidence\build\candidate-source-closure.json'
    $closureHash = (Get-FileHash -LiteralPath $closurePath -Algorithm SHA256).Hash.ToLowerInvariant()
    $sourceReproPath = Join-Path $RepositoryRoot "artifacts\evidence\source-reproducibility\$($Manifest.releaseLabel)\source-reproducibility.json"
    $sourceReproHash = (Get-FileHash -LiteralPath $sourceReproPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $sourceRepro = Get-Content -LiteralPath $sourceReproPath -Raw | ConvertFrom-Json
    if ([string]$record.packageChecksumManifestSha256 -cne $packageChecksumHash -or
            [string]$record.entryChecksumManifestSha256 -cne $entryChecksumHash -or
            [string]$record.candidateSourceClosureSha256 -cne $closureHash -or
            [string]$record.sourceReproducibilityRecordSha256 -cne $sourceReproHash -or
            [string]$sourceRepro.status -cne 'pass' -or
            [string]$sourceRepro.sourceCommit -cne $PackageSource -or
            [string]$sourceRepro.toolchainLockSha256 -cne [string]$record.toolchainLockSha256 -or
            -not [bool]$sourceRepro.pathDistinct) {
        throw 'Alpha 2 qualified evidence does not match checksum, final-closure, or source-reproducibility evidence.'
    }

    $buildRecords = @($record.buildEvidence)
    if ($buildRecords.Count -ne @($Manifest.lanes).Count) {
        throw 'Alpha 2 qualified evidence record does not contain exactly three build-evidence records.'
    }
    foreach ($index in 0..(@($Manifest.lanes).Count - 1)) {
        $lane = $Manifest.lanes[$index]
        $buildRecord = $buildRecords[$index]
        $buildEvidenceDirectory = Join-Path $RepositoryRoot "artifacts\evidence\build\$($lane.id)\$Configuration"
        $toolchainHash = (Get-FileHash -LiteralPath (Join-Path $buildEvidenceDirectory 'toolchain.json') -Algorithm SHA256).Hash.ToLowerInvariant()
        $binaryLogHash = (Get-FileHash -LiteralPath (Join-Path $buildEvidenceDirectory 'msbuild.binlog') -Algorithm SHA256).Hash.ToLowerInvariant()
        if ([string]$buildRecord.lane -cne [string]$lane.id -or
                [string]$buildRecord.toolchainEvidenceSha256 -cne $toolchainHash -or
                [string]$buildRecord.binaryLogSha256 -cne $binaryLogHash) {
            throw "Alpha 2 qualified evidence build/log hashes do not match '$($lane.id)'."
        }
    }

    return $record
}
