[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'High')]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('CreateCandidate', 'PromoteCandidate', 'CreatePost', 'PromotePost')]
    [string]$Phase,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedCommit,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedMasterCommit,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedDevCommit
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot 'validate-release-train.ps1')
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$train = Get-Content -LiteralPath (
    Join-Path $repositoryRoot 'release\train\2.0.0.json') -Raw | ConvertFrom-Json
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
if ([string]$train.currentMilestone -cnotmatch '^alpha\.[1-6]$' -or
    [string]$identity.ReleaseChannel -cne 'alpha') {
    throw 'promote-alpha.ps1 may operate only on the active Alpha milestone.'
}
if ($PSCmdlet.ShouldProcess(
        'origin',
        "$Phase for exact $($identity.ReleaseLabel) commit $ExpectedCommit")) {
    & (Join-Path $PSScriptRoot 'invoke-release-ref-transaction.ps1') `
        -Mode $Phase `
        -ReleaseLabel ([string]$identity.ReleaseLabel) `
        -ExpectedCommit $ExpectedCommit `
        -ExpectedMasterCommit $ExpectedMasterCommit `
        -ExpectedDevCommit $ExpectedDevCommit `
        -Confirm:$false
}
