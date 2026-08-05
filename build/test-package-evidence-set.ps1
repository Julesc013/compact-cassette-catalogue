[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'package-evidence-set.ps1')

function New-Records {
    param(
        [string]$Source = 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa',
        [string]$Mode = 'Candidate',
        [string]$Status = 'locked',
        [string]$Lock = 'bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb'
    )
    return @('win-x86-net40', 'win-x64-net48', 'win-arm64-net481') | ForEach-Object {
        New-Object PSObject -Property @{
            lane = $_
            releaseVersion = '1.3.0'
            releaseStage = 'Alpha 2'
            releaseLabel = '1.3.0a2'
            releaseTag = 'v1.3.0a2'
            releaseChannel = 'alpha'
            publicationStatus = 'retained-unpublished'
            sourceCommit = $Source
            toolchainMode = $Mode
            toolchainLockStatus = $Status
            toolchainLockSha256 = $Lock
        }
    }
}

function Assert-Rejected {
    param([string]$Name, [object[]]$Records, [string]$Pattern)
    try {
        [void](Assert-C3PackageEvidenceSet -Records $Records -RequireCandidate)
        throw "$Name unexpectedly passed."
    }
    catch {
        if ($_.Exception.Message -notmatch $Pattern) { throw }
        Write-Host "PASS $Name rejected: $($_.Exception.Message)"
    }
}

$valid = @(New-Records)
$result = Assert-C3PackageEvidenceSet -Records $valid -RequireCandidate
if ([string]$result.toolchainMode -cne 'Candidate' -or [string]$result.toolchainLockStatus -cne 'locked') {
    throw 'Valid one-source/one-lock Candidate set did not pass.'
}

$differentLock = @(New-Records)
$differentLock[2].toolchainLockSha256 = 'cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc'
Assert-Rejected 'cross-lane lock mismatch' $differentLock 'exactly one release identity.*external-lock SHA-256'
$differentSource = @(New-Records)
$differentSource[1].sourceCommit = 'dddddddddddddddddddddddddddddddddddddddd'
Assert-Rejected 'cross-lane source mismatch' $differentSource 'exactly one release identity.*external-lock SHA-256'
$mixedMode = @(New-Records)
$mixedMode[0].toolchainMode = 'Preparation'
$mixedMode[0].toolchainLockStatus = 'template'
Assert-Rejected 'cross-lane mode/status mismatch' $mixedMode 'exactly one release identity.*external-lock SHA-256'
$differentLabel = @(New-Records)
$differentLabel[1].releaseStage = 'Alpha 3'
$differentLabel[1].releaseLabel = '1.3.0a3'
$differentLabel[1].releaseTag = 'v1.3.0a3'
Assert-Rejected 'cross-lane release-label mismatch' $differentLabel 'exactly one release identity.*external-lock SHA-256'
$stableLookingAlpha = @(New-Records)
foreach ($record in $stableLookingAlpha) {
    $record.releaseLabel = '1.3.0'
    $record.releaseTag = 'v1.3.0'
}
Assert-Rejected 'internally consistent stable-looking Alpha identity' $stableLookingAlpha 'Alpha package evidence.*requires version\+aN label'
$mismatchedBeta = @(New-Records)
foreach ($record in $mismatchedBeta) {
    $record.releaseStage = 'Beta 2'
    $record.releaseLabel = '1.3.0b1'
    $record.releaseTag = 'v1.3.0b1'
    $record.releaseChannel = 'beta'
}
Assert-Rejected 'mismatched Beta ordinal' $mismatchedBeta 'Beta package evidence.*matching Beta N stage'
$unlockedCandidate = @(New-Records -Status template)
Assert-Rejected 'unlocked Candidate set' $unlockedCandidate 'Candidate package evidence requires lock status locked'

Write-Host 'One-release/one-source/one-lock three-package evidence assertions passed.'
