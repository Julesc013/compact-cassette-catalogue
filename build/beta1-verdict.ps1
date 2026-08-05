$script:C3Beta1PortableGateIds = @(
    'source-controls', 'application-defects', 'historical-gate1', 'candidate-build',
    'source-reproducibility', 'portable-packages', 'target-xp-runtime',
    'target-win7-runtime', 'target-arm64-runtime'
)
$script:C3Beta1SetupGateIds = @(
    'source-controls', 'application-defects', 'historical-gate1', 'candidate-build',
    'source-reproducibility', 'setup-durability', 'setup-packages',
    'target-xp-setup', 'target-win7-setup', 'target-arm64-setup'
)
$script:C3Beta1GateIds = @(($script:C3Beta1PortableGateIds + $script:C3Beta1SetupGateIds) | Sort-Object -Unique)

function Resolve-C3Beta1EvidencePath {
    param([string]$RepositoryRoot, [string]$RelativePath, [string]$Context)

    if ([string]::IsNullOrWhiteSpace($RelativePath) -or [IO.Path]::IsPathRooted($RelativePath) -or
            $RelativePath -match '(^|[\\/])\.\.([\\/]|$)') {
        throw "$Context must name one repository-relative retained evidence file."
    }
    $root = [IO.Path]::GetFullPath($RepositoryRoot).TrimEnd('\') + '\'
    $path = [IO.Path]::GetFullPath((Join-Path $RepositoryRoot $RelativePath))
    if (-not $path.StartsWith($root, [StringComparison]::OrdinalIgnoreCase) -or
            -not (Test-Path -LiteralPath $path -PathType Leaf) -or
            ((Get-Item -LiteralPath $path).Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
        throw "$Context does not resolve to one ordinary retained evidence file below the repository."
    }
    return $path
}

function Assert-C3Beta1Verdict {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$RepositoryRoot,
        [switch]$RequireGo,
        [string]$ManifestPath
    )

    . (Join-Path $PSScriptRoot 'beta1-contract.ps1')
    $record = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    foreach ($comparison in @(
            @('schemaVersion', [string]$record.schemaVersion, '1'),
            @('releaseVersion', [string]$record.releaseVersion, '1.3.0'),
            @('releaseStage', [string]$record.releaseStage, 'Beta 1'),
            @('releaseLabel', [string]$record.releaseLabel, '1.3.0b1'),
            @('releaseTag', [string]$record.releaseTag, 'v1.3.0b1'),
            @('releaseChannel', [string]$record.releaseChannel, 'beta'),
            @('publicationStatus', [string]$record.publicationStatus, 'retained-unpublished'))) {
        if ($comparison[1] -cne $comparison[2]) { throw "Beta 1 verdict $($comparison[0]) is '$($comparison[1])', expected '$($comparison[2])'." }
    }
    if ([string]$record.status -notin @('go', 'no-go') -or [string]$record.sourceCommit -notmatch '^[0-9a-f]{40}$') {
        throw 'Beta 1 verdict must be a completed GO/NO-GO bound to one full source SHA.'
    }

    $gates = @($record.gates)
    $actualGateIds = @($gates | ForEach-Object { [string]$_.id } | Sort-Object)
    if ($gates.Count -ne $script:C3Beta1GateIds.Count -or
            ($actualGateIds -join "`n") -cne (($script:C3Beta1GateIds | Sort-Object) -join "`n")) {
        throw 'Beta 1 verdict must contain each closed gate ID exactly once.'
    }
    foreach ($gate in $gates) {
        $status = [string]$gate.status
        if ($status -notin @('pass', 'fail', 'missing') -or [string]::IsNullOrWhiteSpace([string]$gate.reason)) {
            throw "Beta 1 gate '$($gate.id)' has no closed status and explanatory reason."
        }
        if ($status -ceq 'pass' -or -not [string]::IsNullOrWhiteSpace([string]$gate.evidenceFile)) {
            if ([string]$gate.evidenceSha256 -notmatch '^[0-9a-f]{64}$') { throw "Beta 1 gate '$($gate.id)' has no canonical evidence SHA-256." }
            $evidencePath = Resolve-C3Beta1EvidencePath -RepositoryRoot $RepositoryRoot -RelativePath ([string]$gate.evidenceFile) -Context "Beta 1 gate '$($gate.id)'"
            if ((Get-FileHash -LiteralPath $evidencePath -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$gate.evidenceSha256) {
                throw "Beta 1 gate '$($gate.id)' evidence hash differs from the retained file."
            }
        }
    }

    $portableGo = @($gates | Where-Object { $script:C3Beta1PortableGateIds -contains [string]$_.id -and [string]$_.status -cne 'pass' }).Count -eq 0
    $setupGo = @($gates | Where-Object { $script:C3Beta1SetupGateIds -contains [string]$_.id -and [string]$_.status -cne 'pass' }).Count -eq 0
    $overallGo = $portableGo -and $setupGo
    if ([bool]$record.portableBetaGo -ne $portableGo -or [bool]$record.classicSetupBetaGo -ne $setupGo -or
            [bool]$record.overallBetaGo -ne $overallGo -or [bool]$record.tagAuthorized -ne $overallGo -or
            [bool]$record.legacyPromotionAuthorized -ne $overallGo -or
            [string]$record.status -cne $(if ($overallGo) { 'go' } else { 'no-go' })) {
        throw 'Beta 1 component/overall verdicts and tag/ledger authority are not the mechanical result of their gates.'
    }
    if ([bool]$record.publicReleaseAuthorized -or [bool]$record.feedChangeAuthorized -or [bool]$record.masterOrDev2ChangeAuthorized) {
        throw 'Beta 1 verdict may never authorize public release, feed, master, or dev/2.x changes.'
    }

    if ([bool]$record.candidate.present) {
        if ([string]$record.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$' -or
                [string]$record.candidate.candidateIndexSha256 -notmatch '^[0-9a-f]{64}$' -or
                [string]$record.candidate.path -cne "artifacts/candidates/1.3.0b1/$($record.sourceCommit)") {
            throw 'Beta 1 Candidate verdict does not use the exact source-bound path, index hash, and lock hash.'
        }
        $candidateRoot = Join-Path $RepositoryRoot ([string]$record.candidate.path).Replace('/', '\')
        if ([string]::IsNullOrWhiteSpace($ManifestPath)) { $ManifestPath = Join-Path $RepositoryRoot 'build\lanes.json' }
        & (Join-Path $PSScriptRoot 'verify-beta1-assets.ps1') -CandidateRoot $candidateRoot `
            -ExpectedSourceCommit ([string]$record.sourceCommit) -ExpectedToolchainLockSha256 ([string]$record.toolchainLockSha256) `
            -ManifestPath $ManifestPath | Out-Null
        $candidateIndexPath = Join-Path $candidateRoot 'evidence\candidate.json'
        if ((Get-FileHash -LiteralPath $candidateIndexPath -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$record.candidate.candidateIndexSha256) {
            throw 'Beta 1 verdict Candidate-index hash differs from retained Candidate evidence.'
        }
    }
    elseif ($overallGo -or [string]$record.toolchainLockSha256 -notin @('', $null)) {
        throw 'Beta 1 GO requires the exact retained Candidate; an absent Candidate must not claim a lock.'
    }
    if ($RequireGo -and -not $overallGo) { throw 'Beta tag/ledger operation requires overallBetaGo=true with every gate PASS.' }
    if ([string]::IsNullOrWhiteSpace([string]$record.recordedAtUtc)) { throw 'Completed Beta 1 verdict has no UTC recording time.' }
    return $record
}

