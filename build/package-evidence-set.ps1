function Assert-C3PackageEvidenceSet {
    param(
        [Parameter(Mandatory = $true)][object[]]$Records,
        [switch]$RequireCandidate
    )

    $expectedLaneIds = @('win-x86-net40', 'win-x64-net48', 'win-arm64-net481')
    $actualLaneIds = @($Records | ForEach-Object { [string]$_.lane } | Sort-Object)
    if ($Records.Count -ne 3 -or
            ($actualLaneIds -join "`n") -cne (($expectedLaneIds | Sort-Object) -join "`n")) {
        throw "Package evidence must contain exactly the three release lanes; found '$($actualLaneIds -join ', ')'."
    }

    foreach ($record in $Records) {
        if ([string]::IsNullOrWhiteSpace([string]$record.releaseVersion) -or
                [string]::IsNullOrWhiteSpace([string]$record.releaseStage) -or
                [string]$record.releaseLabel -notmatch '^\d+\.\d+\.\d+(?:a\d+|b\d+)?$' -or
                [string]$record.releaseTag -cne "v$($record.releaseLabel)" -or
                [string]$record.releaseChannel -notin @('alpha', 'beta', 'stable') -or
                [string]::IsNullOrWhiteSpace([string]$record.publicationStatus) -or
                [string]$record.sourceCommit -notmatch '^[0-9a-f]{40}$' -or
                [string]$record.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$' -or
                [string]$record.toolchainMode -notin @('Preparation', 'Candidate') -or
                [string]$record.toolchainLockStatus -notin @('template', 'locked')) {
            throw "Package evidence for '$($record.lane)' has invalid release/source/mode/lock identity."
        }
    }

    $uniqueVersions = @($Records | ForEach-Object { [string]$_.releaseVersion } | Sort-Object -Unique)
    $uniqueStages = @($Records | ForEach-Object { [string]$_.releaseStage } | Sort-Object -Unique)
    $uniqueLabels = @($Records | ForEach-Object { [string]$_.releaseLabel } | Sort-Object -Unique)
    $uniqueTags = @($Records | ForEach-Object { [string]$_.releaseTag } | Sort-Object -Unique)
    $uniqueChannels = @($Records | ForEach-Object { [string]$_.releaseChannel } | Sort-Object -Unique)
    $uniquePublicationStatuses = @($Records | ForEach-Object { [string]$_.publicationStatus } | Sort-Object -Unique)
    $uniqueSources = @($Records | ForEach-Object { [string]$_.sourceCommit } | Sort-Object -Unique)
    $uniqueModes = @($Records | ForEach-Object { [string]$_.toolchainMode } | Sort-Object -Unique)
    $uniqueLockStatuses = @($Records | ForEach-Object { [string]$_.toolchainLockStatus } | Sort-Object -Unique)
    $uniqueLockHashes = @($Records | ForEach-Object { [string]$_.toolchainLockSha256 } | Sort-Object -Unique)
    if ($uniqueVersions.Count -ne 1 -or $uniqueStages.Count -ne 1 -or
            $uniqueLabels.Count -ne 1 -or $uniqueTags.Count -ne 1 -or
            $uniqueChannels.Count -ne 1 -or $uniquePublicationStatuses.Count -ne 1 -or
            $uniqueSources.Count -ne 1 -or $uniqueModes.Count -ne 1 -or
            $uniqueLockStatuses.Count -ne 1 -or $uniqueLockHashes.Count -ne 1) {
        throw 'The three-package set must use exactly one release identity, source commit, toolchain mode, lock status, and external-lock SHA-256.'
    }
    if ($uniqueLabels[0] -match 'a\d+$' -and
            ($uniqueChannels[0] -cne 'alpha' -or $uniquePublicationStatuses[0] -cne 'retained-unpublished')) {
        throw 'Alpha package evidence must be alpha-channel and retained-unpublished.'
    }

    if ($uniqueModes[0] -ceq 'Candidate' -and $uniqueLockStatuses[0] -cne 'locked') {
        throw 'Candidate package evidence requires lock status locked.'
    }
    if ($uniqueModes[0] -ceq 'Preparation' -and $uniqueLockStatuses[0] -cne 'template') {
        throw 'Preparation package evidence requires repository lock status template.'
    }
    if ($RequireCandidate -and
            ($uniqueModes[0] -cne 'Candidate' -or $uniqueLockStatuses[0] -cne 'locked')) {
        throw "Candidate packaging requires one Candidate/locked evidence set; found '$($uniqueModes[0])/$($uniqueLockStatuses[0])'."
    }

    return New-Object PSObject -Property @{
        releaseVersion = $uniqueVersions[0]
        releaseStage = $uniqueStages[0]
        releaseLabel = $uniqueLabels[0]
        releaseTag = $uniqueTags[0]
        releaseChannel = $uniqueChannels[0]
        publicationStatus = $uniquePublicationStatuses[0]
        sourceCommit = $uniqueSources[0]
        toolchainMode = $uniqueModes[0]
        toolchainLockStatus = $uniqueLockStatuses[0]
        toolchainLockSha256 = $uniqueLockHashes[0]
    }
}
