[CmdletBinding(SupportsShouldProcess = $true)]
param([switch]$Push, [string]$RemoteName = 'origin')

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-verdict.ps1')
. (Join-Path $PSScriptRoot 'beta1-tag-message.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$verdict = Assert-C3Beta1Verdict -Path (Join-Path $repositoryRoot 'release\validation\1.3.0-beta.1-verdict.json') `
    -RepositoryRoot $repositoryRoot -RequireGo
$head = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$parents = @((& git -C $repositoryRoot rev-list --parents -n 1 HEAD).Trim().Split(' '))
if ($parents.Count -ne 2 -or $parents[1] -cne [string]$verdict.sourceCommit) { throw 'Tag creation requires exact C-beta -> E-beta topology.' }
$changes = @(& git -C $repositoryRoot diff --name-only $verdict.sourceCommit HEAD | Sort-Object)
$expected = @('release/validation/1.3.0-beta.1-verdict.json', 'release/validation/1.3.0-beta.1-verdict.md') | Sort-Object
if (($changes -join "`n") -cne ($expected -join "`n")) { throw 'E-beta contains changes outside the two qualification records.' }
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0b1
if ($LASTEXITCODE -eq 0) { throw 'Refusing to create or move existing v1.3.0b1.' }
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw 'Beta tag creation requires clean E-beta.' }
if ($PSCmdlet.ShouldProcess("E-beta $head", 'Create immutable annotated v1.3.0b1 after complete GO')) {
    & git -C $repositoryRoot tag -a v1.3.0b1 -m (Get-C3Beta1TagMessage) $head
    if ($LASTEXITCODE -ne 0) { throw 'Could not create annotated v1.3.0b1.' }
    & (Join-Path $PSScriptRoot 'verify-beta1-tagged.ps1') -TagState Tagged
    if ($Push) {
        & git -C $repositoryRoot push $RemoteName refs/tags/v1.3.0b1
        if ($LASTEXITCODE -ne 0) { throw 'Could not push v1.3.0b1.' }
    }
}

