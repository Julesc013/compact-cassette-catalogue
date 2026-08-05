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
        if ([string]$record.sourceCommit -notmatch '^[0-9a-f]{40}$' -or
                [string]$record.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$' -or
                [string]$record.toolchainMode -notin @('Preparation', 'Candidate') -or
                [string]$record.toolchainLockStatus -notin @('template', 'locked')) {
            throw "Package evidence for '$($record.lane)' has invalid source/mode/lock identity."
        }
    }

    $uniqueSources = @($Records | ForEach-Object { [string]$_.sourceCommit } | Sort-Object -Unique)
    $uniqueModes = @($Records | ForEach-Object { [string]$_.toolchainMode } | Sort-Object -Unique)
    $uniqueLockStatuses = @($Records | ForEach-Object { [string]$_.toolchainLockStatus } | Sort-Object -Unique)
    $uniqueLockHashes = @($Records | ForEach-Object { [string]$_.toolchainLockSha256 } | Sort-Object -Unique)
    if ($uniqueSources.Count -ne 1 -or $uniqueModes.Count -ne 1 -or
            $uniqueLockStatuses.Count -ne 1 -or $uniqueLockHashes.Count -ne 1) {
        throw 'The three-package set must use exactly one source commit, toolchain mode, lock status, and external-lock SHA-256.'
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
        sourceCommit = $uniqueSources[0]
        toolchainMode = $uniqueModes[0]
        toolchainLockStatus = $uniqueLockStatuses[0]
        toolchainLockSha256 = $uniqueLockHashes[0]
    }
}
