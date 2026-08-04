[CmdletBinding()]
param(
    [string]$ExpectedMilestone,
    [switch]$Rebuild,
    [switch]$Reproduce
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$branchContract = & (Join-Path $PSScriptRoot 'get-branch-contract.ps1') `
    -RepositoryRoot $repositoryRoot
$integrationBranch = [string]$branchContract.CurrentIntegration
if (-not (Test-Path -LiteralPath (Join-Path $repositoryRoot '.git'))) {
    throw 'Candidate freeze requires a full Git checkout.'
}

$branch = ([string](& git -C $repositoryRoot branch --show-current)).Trim()
if ($LASTEXITCODE -ne 0 -or $branch -cne $integrationBranch) {
    throw "Candidate freeze requires branch '$integrationBranch'; found '$branch'."
}
$worktree = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
if ($LASTEXITCODE -ne 0 -or $worktree.Count -ne 0) {
    throw 'Candidate freeze requires a clean worktree, including untracked files.'
}

& (Join-Path $PSScriptRoot 'validate-release-train.ps1')
$train = Get-Content -LiteralPath (
    Join-Path $repositoryRoot 'release\train\2.0.0.json') -Raw | ConvertFrom-Json
if (-not [string]::IsNullOrWhiteSpace($ExpectedMilestone) -and
    [string]$train.currentMilestone -cne $ExpectedMilestone) {
    throw "Expected milestone '$ExpectedMilestone', found '$($train.currentMilestone)'."
}

if ($Rebuild -or $Reproduce) {
    & (Join-Path $PSScriptRoot 'verify-milestone.ps1') `
        -ExpectedMilestone ([string]$train.currentMilestone) `
        -Rebuild:$Rebuild `
        -Reproduce:$Reproduce
}

$commit = ([string](& git -C $repositoryRoot rev-parse HEAD)).Trim()
if ($LASTEXITCODE -ne 0 -or $commit -cnotmatch '^[0-9a-f]{40}$') {
    throw 'Could not resolve the full candidate commit SHA.'
}
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$result = [PSCustomObject]@{
    Milestone = [string]$train.currentMilestone
    ReleaseLabel = [string]$identity.ReleaseLabel
    SourceCommit = $commit
    Branch = $branch
    Reproduced = [bool]$Reproduce
}

Write-Host "Frozen candidate C: $($result.ReleaseLabel) at $commit."
Write-Output $result
