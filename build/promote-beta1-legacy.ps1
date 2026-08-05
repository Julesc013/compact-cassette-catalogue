[CmdletBinding(SupportsShouldProcess = $true)]
param([string]$RemoteName = 'origin')

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'verify-beta1-tagged.ps1') -TagState PostTag -LegacyState PrePromotion -RemoteName $RemoteName
$old = 'c4115b82ea43fdd763685d862a08fe5c61db6dff'
$head = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($PSCmdlet.ShouldProcess("$RemoteName/legacy/1.x", "Lease-protected fast-forward from $old to qualified P-beta $head")) {
    & git -C $repositoryRoot merge-base --is-ancestor $old $head
    if ($LASTEXITCODE -ne 0) { throw 'P-beta is not a fast-forward descendant of the legacy ledger.' }
    & git -C $repositoryRoot push $RemoteName "$head`:refs/heads/legacy/1.x" "--force-with-lease=refs/heads/legacy/1.x:$old"
    if ($LASTEXITCODE -ne 0) { throw 'Lease-protected Beta ledger promotion failed.' }
    & (Join-Path $PSScriptRoot 'verify-beta1-tagged.ps1') -TagState PostTag -LegacyState Promoted -RemoteName $RemoteName
}

