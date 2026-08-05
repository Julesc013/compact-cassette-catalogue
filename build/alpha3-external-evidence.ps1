$script:C3Alpha3HistoricalRuntimeIds = @(
    'v1.1.2',
    'v1.2.0b1-x86-net40',
    'v1.2.0b1-x64-net40'
)
$script:C3Alpha3HistoricalScenarioIds = @(
    'launch-blocked-network-close',
    'entity-crud',
    'save-reopen-close',
    'lists-filters-settings',
    'dirty-close',
    'pending-edit-transition',
    'manual-update-blocked',
    'private-catalogues',
    'control-resource-capture'
)
$script:C3Alpha3TargetLaneIds = @(
    'win-x86-net40',
    'win-x64-net48',
    'win-arm64-net481'
)
$script:C3Alpha3SetupScenarios = @(
    'cancel-no-mutation',
    'clean-install',
    'same-byte-reinstall',
    'repair',
    'same-lane-upgrade',
    'downgrade-rejection',
    'lane-change-rejection',
    'injected-rollback',
    'running-c3-refusal',
    'altered-owned-state-refusal',
    'ownership-only-uninstall',
    'repeated-uninstall',
    'locked-self-cleanup',
    'unknown-user-data-preservation',
    'keyboard-screen-reader',
    'dpi-high-contrast'
)

function Assert-C3RetainedEvidenceFile {
    param(
        [Parameter(Mandatory = $true)][string]$IndexDirectory,
        [Parameter(Mandatory = $true)][string]$RelativePath,
        [Parameter(Mandatory = $true)][string]$ExpectedSha256,
        [Parameter(Mandatory = $true)][string]$Context
    )

    if ([IO.Path]::IsPathRooted($RelativePath) -or $ExpectedSha256 -notmatch '^[0-9a-f]{64}$') {
        throw "$Context has a rooted path or invalid SHA-256."
    }
    $root = [IO.Path]::GetFullPath($IndexDirectory).TrimEnd('\') + '\'
    $path = [IO.Path]::GetFullPath((Join-Path $IndexDirectory $RelativePath))
    if (-not $path.StartsWith($root, [StringComparison]::OrdinalIgnoreCase) -or
            -not (Test-Path -LiteralPath $path -PathType Leaf) -or
            ((Get-Item -LiteralPath $path).Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
        throw "$Context does not resolve to one ordinary retained file below its evidence index."
    }
    $actualHash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($actualHash -cne $ExpectedSha256) {
        throw "$Context retained file hash does not match: $actualHash / $ExpectedSha256"
    }
}

function Assert-C3Alpha3HistoricalGate1Evidence {
    param([Parameter(Mandatory = $true)][string]$Path)

    $Path = [IO.Path]::GetFullPath($Path)
    $record = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    if ([int]$record.schemaVersion -ne 2 -or
            [string]$record.status -cne 'pass' -or
            [string]$record.baselineTag -cne 'v1.2.0b1' -or
            [string]$record.baselineSourceCommit -cne '2413e9139a098f3321385f2f946e743012a447f5' -or
            [string]$record.runtimeKitSha256 -cne '74f059a7029ac44cc38bfae503a6ede43b2a28552e138dd8f734b9c0dde39338' -or
            [string]$record.v112Sha256 -cne '50183c989956f85364dd1cda55a1397209a646b58f7df6cda0604540e3382f9e' -or
            [string]$record.v120b1X86Sha256 -cne '205ba251175d5a6fa20a3ace6127a00e5d10d73ad30581032c8f09b20ceb7222' -or
            [string]$record.v120b1X64Sha256 -cne '257ec9d0ea86f268d8328d71041e63eb379fc1809c91593db29d883359db747c' -or
            [string]::IsNullOrWhiteSpace([string]$record.privateCatalogueEvidenceFile) -or
            [string]$record.privateCatalogueSetSha256 -notmatch '^[0-9a-f]{64}$' -or
            [string]::IsNullOrWhiteSpace([string]$record.controlResourceEvidenceFile) -or
            [string]$record.controlResourceEvidenceSha256 -notmatch '^[0-9a-f]{64}$' -or
            [string]::IsNullOrWhiteSpace([string]$record.recordedAtUtc)) {
        throw 'Historical Gate 1 completion index has invalid identity, oracle, catalogue, control/resource, or result fields.'
    }
    $indexDirectory = Split-Path -Parent $Path
    Assert-C3RetainedEvidenceFile -IndexDirectory $indexDirectory -RelativePath ([string]$record.privateCatalogueEvidenceFile) `
        -ExpectedSha256 ([string]$record.privateCatalogueSetSha256) -Context 'Historical private catalogue set'
    Assert-C3RetainedEvidenceFile -IndexDirectory $indexDirectory -RelativePath ([string]$record.controlResourceEvidenceFile) `
        -ExpectedSha256 ([string]$record.controlResourceEvidenceSha256) -Context 'Historical control/resource evidence'
    $workflows = @($record.workflows)
    if ($workflows.Count -ne $script:C3Alpha3HistoricalRuntimeIds.Count) {
        throw 'Historical Gate 1 completion index must contain exactly three oracle workflow records.'
    }
    foreach ($runtimeId in $script:C3Alpha3HistoricalRuntimeIds) {
        $workflow = @($workflows | Where-Object { [string]$_.runtimeId -ceq $runtimeId })
        if ($workflow.Count -ne 1 -or [string]$workflow[0].result -cne 'complete' -or
                [int]$workflow[0].unexplainedDeviationCount -ne 0) {
            throw "Historical Gate 1 workflow '$runtimeId' is missing, incomplete, or has an unexplained deviation."
        }
        Assert-C3RetainedEvidenceFile -IndexDirectory $indexDirectory -RelativePath ([string]$workflow[0].evidenceFile) `
            -ExpectedSha256 ([string]$workflow[0].evidenceSha256) -Context "Historical workflow $runtimeId"
        $scenarios = @($workflow[0].scenarios)
        if ($scenarios.Count -ne $script:C3Alpha3HistoricalScenarioIds.Count) {
            throw "Historical Gate 1 workflow '$runtimeId' must contain exactly nine classified scenario outcomes."
        }
        foreach ($scenarioId in $script:C3Alpha3HistoricalScenarioIds) {
            $scenario = @($scenarios | Where-Object { [string]$_.id -ceq $scenarioId })
            if ($scenario.Count -ne 1 -or [string]::IsNullOrWhiteSpace([string]$scenario[0].notes)) {
                throw "Historical Gate 1 workflow '$runtimeId' scenario '$scenarioId' is missing or lacks retained classification notes."
            }
            $outcome = [string]$scenario[0].outcome
            $classification = [string]$scenario[0].classification
            $defectIds = @($scenario[0].defectIds)
            if ($outcome -ceq 'compatible') {
                if ($classification -cne 'expected-compatible' -or $defectIds.Count -ne 0) {
                    throw "Historical Gate 1 workflow '$runtimeId' scenario '$scenarioId' has an invalid compatible classification."
                }
            }
            elseif ($outcome -ceq 'known-defect-reproduced' -or $outcome -ceq 'known-defect-not-reproduced') {
                if ($classification -cne 'classified-known-defect' -or $defectIds.Count -eq 0 -or
                        @($defectIds | Where-Object { [string]$_ -notmatch '^APP-0(0[1-9]|1[0-5])$' }).Count -ne 0) {
                    throw "Historical Gate 1 workflow '$runtimeId' scenario '$scenarioId' does not bind a known application defect."
                }
            }
            else {
                throw "Historical Gate 1 workflow '$runtimeId' scenario '$scenarioId' has an unclassified outcome '$outcome'."
            }
        }
    }
    $exchange = @($record.catalogueExchange)
    if ($exchange.Count -ne 9) { throw 'Historical Gate 1 catalogue exchange must contain exactly nine producer/reader cells.' }
    foreach ($producer in $script:C3Alpha3HistoricalRuntimeIds) {
        foreach ($reader in $script:C3Alpha3HistoricalRuntimeIds) {
            $cell = @($exchange | Where-Object { [string]$_.producer -ceq $producer -and [string]$_.reader -ceq $reader })
            if ($cell.Count -ne 1 -or [string]$cell[0].result -cne 'pass') {
                throw "Historical Gate 1 exchange '$producer -> $reader' is missing or not PASS."
            }
            Assert-C3RetainedEvidenceFile -IndexDirectory $indexDirectory -RelativePath ([string]$cell[0].evidenceFile) `
                -ExpectedSha256 ([string]$cell[0].evidenceSha256) -Context "Historical exchange $producer to $reader"
        }
    }
    return $record
}

function Assert-C3Alpha3TargetQualificationEvidence {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$PackageSource,
        [Parameter(Mandatory = $true)][string]$ToolchainLockSha256,
        [Parameter(Mandatory = $true)]$Manifest
    )

    $Path = [IO.Path]::GetFullPath($Path)
    $record = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    if ([int]$record.schemaVersion -ne 1 -or
            [string]$record.status -cne 'pass' -or
            [string]$record.releaseLabel -cne '1.3.0a3' -or
            [string]$record.packageSource -cne $PackageSource -or
            [string]$record.toolchainLockSha256 -cne $ToolchainLockSha256 -or
            [string]::IsNullOrWhiteSpace([string]$record.recordedAtUtc)) {
        throw 'Alpha 3 target qualification index has invalid result, release, source, or lock identity.'
    }
    $indexDirectory = Split-Path -Parent $Path
    $runtimeRecords = @($record.runtime)
    $setupRecords = @($record.setup)
    if ($runtimeRecords.Count -ne 3 -or $setupRecords.Count -ne 3) {
        throw 'Alpha 3 target qualification requires exactly three runtime and three setup lane records.'
    }
    foreach ($lane in @($Manifest.lanes)) {
        $runtime = @($runtimeRecords | Where-Object { [string]$_.lane -ceq [string]$lane.id })
        if ($runtime.Count -ne 1 -or [string]$runtime[0].result -cne 'pass' -or
                [string]$runtime[0].environmentId -cne [string]$lane.runtimeEnvironmentId -or
                [string]$runtime[0].packageName -cne [string]$lane.packageName -or
                [string]$runtime[0].packageSha256 -notmatch '^[0-9a-f]{64}$' -or
                [string]$runtime[0].entryManifestSha256 -notmatch '^[0-9a-f]{64}$') {
            throw "Alpha 3 target runtime qualification for '$($lane.id)' is missing or invalid."
        }
        Assert-C3RetainedEvidenceFile -IndexDirectory $indexDirectory -RelativePath ([string]$runtime[0].evidenceFile) `
            -ExpectedSha256 ([string]$runtime[0].evidenceSha256) -Context "Target runtime $($lane.id)"

        $setup = @($setupRecords | Where-Object { [string]$_.lane -ceq [string]$lane.id })
        if ($setup.Count -ne 1 -or [string]$setup[0].result -cne 'pass' -or
                [string]$setup[0].environmentId -cne [string]$lane.runtimeEnvironmentId -or
                [string]$setup[0].packageName -cne [string]$lane.setupPackageName -or
                [string]$setup[0].packageSha256 -notmatch '^[0-9a-f]{64}$' -or
                [string]$setup[0].entryManifestSha256 -notmatch '^[0-9a-f]{64}$' -or
                ((@($setup[0].scenarios | Sort-Object) -join "`n") -cne (($script:C3Alpha3SetupScenarios | Sort-Object) -join "`n"))) {
            throw "Alpha 3 target setup qualification for '$($lane.id)' is missing, invalid, or incomplete."
        }
        Assert-C3RetainedEvidenceFile -IndexDirectory $indexDirectory -RelativePath ([string]$setup[0].evidenceFile) `
            -ExpectedSha256 ([string]$setup[0].evidenceSha256) -Context "Target setup $($lane.id)"
    }
    return $record
}
