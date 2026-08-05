[CmdletBinding()]
param([Parameter(Mandatory = $true)][string]$IdentityCommit)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-identity-transition.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
function Assert-Rejected([string]$Name, [scriptblock]$Action, [string]$Pattern) {
    try { & $Action; throw "$Name unexpectedly passed." } catch { if ($_.Exception.Message -notmatch $Pattern) { throw } }
}

[void](Assert-C3Beta1IdentityTransition -RepositoryRoot $repositoryRoot -IdentityCommit $IdentityCommit -HeadCommit $IdentityCommit)
$parent = (& git -C $repositoryRoot rev-parse "$IdentityCommit^").Trim()
Assert-Rejected 'abbreviated identity SHA' {
    Assert-C3Beta1IdentityTransition -RepositoryRoot $repositoryRoot -IdentityCommit $IdentityCommit.Substring(0, 7) -HeadCommit $IdentityCommit
} 'full 40-character'
Assert-Rejected 'Alpha parent as Beta projection' {
    Assert-C3Beta1IdentityTransition -RepositoryRoot $repositoryRoot -IdentityCommit $parent -HeadCommit $IdentityCommit
} 'Beta 1 manifest property|not exact Alpha 3'
Assert-Rejected 'identity not ancestral to selected source' {
    Assert-C3Beta1IdentityTransition -RepositoryRoot $repositoryRoot -IdentityCommit $IdentityCommit -HeadCommit $parent
} 'not an ancestor'
Write-Host 'Beta identity-transition control accepted the audited Alpha3-to-Beta1 metadata commit and rejected abbreviated, non-Beta, and non-ancestral identities.'
