[CmdletBinding()]
param([string]$RemoteName = 'origin')

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-verdict.ps1')
. (Join-Path $PSScriptRoot 'beta1-publication.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'verify-beta1-tagged.ps1') -TagState Tagged
$path = Join-Path $repositoryRoot 'release\validation\1.3.0-beta.1-post-tag.json'
$template = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
if ([string]$template.status -cne 'template') { throw 'Refusing to overwrite completed Beta post-tag record.' }
$verdict = Assert-C3Beta1Verdict -Path (Join-Path $repositoryRoot 'release\validation\1.3.0-beta.1-verdict.json') -RepositoryRoot $repositoryRoot -RequireGo
$tagRef = 'refs/tags/v1.3.0b1'
$tagObject = (& git -C $repositoryRoot rev-parse $tagRef).Trim()
$tagTarget = (& git -C $repositoryRoot rev-parse 'v1.3.0b1^{commit}').Trim()
$remoteUrl = (& git -C $repositoryRoot remote get-url $RemoteName).Trim()
function Get-RemoteSha([string]$Ref) {
    $lines = @(& git -C $repositoryRoot ls-remote $RemoteName $Ref)
    if ($LASTEXITCODE -ne 0 -or $lines.Count -ne 1) { throw "Could not resolve remote '$Ref'." }
    return [string](@($lines[0] -split "`t")[0])
}
$remoteTagObject = Get-RemoteSha $tagRef
$remoteTagTarget = Get-RemoteSha "$tagRef^{}"
if ($remoteTagObject -cne $tagObject -or $remoteTagTarget -cne $tagTarget) { throw 'Remote Beta tag object/target differs from local.' }
$legacyOld = Get-RemoteSha 'refs/heads/legacy/1.x'
if ($legacyOld -cne 'c4115b82ea43fdd763685d862a08fe5c61db6dff') { throw 'legacy/1.x moved before P-beta.' }
$publicApi = Assert-C3NoPublicBetaRelease -RemoteUrl $remoteUrl
$candidateIndexPath = Join-Path (Join-Path $repositoryRoot ([string]$verdict.candidate.path).Replace('/', '\')) 'evidence\candidate.json'
$record = [ordered]@{
    schemaVersion = 1; status = 'pass'; releaseLabel = '1.3.0b1'; tagName = 'v1.3.0b1'
    tagObject = $tagObject; tagTarget = $tagTarget; packageSource = [string]$verdict.sourceCommit
    candidateIndexSha256 = (Get-FileHash -LiteralPath $candidateIndexPath -Algorithm SHA256).Hash.ToLowerInvariant()
    remoteName = $RemoteName; remoteUrl = $remoteUrl; remoteTagObject = $remoteTagObject; remoteTagTarget = $remoteTagTarget
    masterCommit = Get-RemoteSha 'refs/heads/master'; dev2Commit = Get-RemoteSha 'refs/heads/dev/2.x'; legacyOldCommit = $legacyOld
    publicReleaseApi = $publicApi; publicReleaseAbsent = $true; publicReleaseCheckedAtUtc = [DateTime]::UtcNow.ToString('o')
    publicationStatus = 'retained-unpublished'; feedChanged = $false; publicFeed = @('1.2.0', 'Release', '14/05/2026')
    masterOrDev2Changed = $false; legacyPromotionAuthorized = $true; recordedAtUtc = [DateTime]::UtcNow.ToString('o')
}
[IO.File]::WriteAllText($path, (($record | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
Write-Host 'Populated the sole P-beta record. Commit only this file, push dev/1.x, then lease-promote legacy/1.x.'
