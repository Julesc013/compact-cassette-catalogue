[CmdletBinding()]
param([string]$RemoteName = 'origin')

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha5-tag-message.ps1')
$repositoryRoot = Split-Path -Parent $PSScriptRoot
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw 'Alpha 5 post-tag verification requires clean P.' }
$postCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$evidenceCommit = (& git -C $repositoryRoot rev-parse 'HEAD^').Trim()
$changes = @(& git -C $repositoryRoot diff --name-only $evidenceCommit $postCommit)
if ($changes.Count -ne 1 -or $changes[0] -cne 'release/validation/1.3.0-alpha.5-post-tag.json') {
    throw "Alpha 5 P must change only its post-tag record: $($changes -join ', ')"
}
$record = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.5-post-tag.json') -Raw | ConvertFrom-Json
$tagObject = (& git -C $repositoryRoot rev-parse refs/tags/v1.3.0a5).Trim()
$tagTarget = (& git -C $repositoryRoot rev-parse 'v1.3.0a5^{commit}').Trim()
$tagText = (& git -C $repositoryRoot cat-file tag refs/tags/v1.3.0a5) -join "`n"
Assert-C3Alpha5TagMessage -Text $tagText
if ([string]$record.status -cne 'pass' -or [string]$record.tagObject -cne $tagObject -or
        [string]$record.tagTarget -cne $evidenceCommit -or $tagTarget -cne $evidenceCommit -or
        [bool]$record.publicReleaseCreated -or [bool]$record.betaTagCreated -or [bool]$record.betaArtifactsCreated -or [bool]$record.feedChanged) {
    throw 'Alpha 5 post-tag record does not bind immutable E/tag or unchanged publication/Beta/feed state.'
}
$remote = @(& git -C $repositoryRoot ls-remote --tags $RemoteName refs/tags/v1.3.0a5 'refs/tags/v1.3.0a5^{}')
if ($remote.Count -ne 2) { throw 'Remote Alpha 5 annotated object/target pair is missing.' }
Write-Host "Verified Alpha 5 topology C -> E/tag -> P at '$postCommit'; retained unpublished and Beta-authority boundaries remain closed."
