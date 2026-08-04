[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'High')]
param(
    [switch]$Rebuild,
    [switch]$SkipVerification,
    [switch]$Push
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'validate-release-train.ps1')
$train = Get-Content -LiteralPath (
    Join-Path $repositoryRoot 'release\train\2.0.0.json') -Raw | ConvertFrom-Json
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
if ([string]$train.currentMilestone -cne 'beta.1' -or
    [string]$train.status -cne 'awaiting-owner-manual-validation' -or
    [string]$identity.ReleaseLabel -cne '2.0.0-beta.1') {
    throw 'Beta candidate preparation requires the frozen Beta 1 train identity and owner-validation status.'
}

$branch = ([string](& git -C $repositoryRoot branch --show-current)).Trim()
if ($LASTEXITCODE -ne 0 -or $branch -cne 'dev') {
    throw "Beta candidate preparation requires branch 'dev'; found '$branch'."
}
$worktree = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
if ($LASTEXITCODE -ne 0 -or $worktree.Count -ne 0) {
    throw 'Beta candidate preparation requires a clean worktree.'
}

if (-not $SkipVerification) {
    & (Join-Path $PSScriptRoot 'verify-milestone.ps1') `
        -ExpectedMilestone beta.1 `
        -Rebuild:$Rebuild `
        -Reproduce
}

$commit = ([string](& git -C $repositoryRoot rev-parse HEAD)).Trim()
$candidateBranch = 'candidate/2.0.0-beta.1'
$existingLocal = [string](& git -C $repositoryRoot rev-parse --verify --quiet (
        'refs/heads/' + $candidateBranch) 2>$null)
if ($LASTEXITCODE -eq 0 -and $existingLocal.Trim() -cne $commit) {
    throw "Local $candidateBranch already identifies a different commit."
}

if ([string]::IsNullOrWhiteSpace($existingLocal) -and
    $PSCmdlet.ShouldProcess($candidateBranch, "create at exact candidate $commit")) {
    & git -C $repositoryRoot branch $candidateBranch $commit
    if ($LASTEXITCODE -ne 0) {
        throw "Could not create $candidateBranch."
    }
}
if ($Push -and $PSCmdlet.ShouldProcess('origin', "create $candidateBranch at $commit")) {
    & git -C $repositoryRoot push origin (
        "$commit`:refs/heads/$candidateBranch")
    if ($LASTEXITCODE -ne 0) {
        throw "Could not push $candidateBranch."
    }
}

Write-Host "Beta 1 candidate frozen: $commit on dev and $candidateBranch."
Write-Output ([PSCustomObject]@{
        ReleaseLabel = [string]$identity.ReleaseLabel
        CandidateCommit = $commit
        CandidateBranch = $candidateBranch
        RemotePushed = [bool]$Push
        Status = 'AUTOMATED PASS - OWNER MANUAL VALIDATION REQUIRED'
    })
