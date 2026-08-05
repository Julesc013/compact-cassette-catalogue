[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$RemoteName = 'origin'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ($RemoteName -cne 'origin') { throw "Alpha 3 post-tag authority is fixed to remote 'origin'." }
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) {
    throw 'Alpha 3 post-tag record generation requires clean evidence commit E.'
}
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$recordPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.3-post-tag.json'
$template = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
if ([string]$template.status -cne 'template') { throw "Alpha 3 post-tag record is not an unpopulated template: $recordPath" }

& (Join-Path $PSScriptRoot 'verify-alpha3-tagged.ps1') -Configuration $Configuration -TagState Tagged
$tagName = 'v1.3.0a3'
$tagRef = "refs/tags/$tagName"
$tagObject = (& git -C $repositoryRoot rev-parse $tagRef).Trim()
$tagTarget = (& git -C $repositoryRoot rev-parse "$tagName^{commit}").Trim()
$headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($tagTarget -cne $headCommit) { throw "Alpha 3 tag target '$tagTarget' is not evidence commit E '$headCommit'." }
$remoteUrl = (& git -C $repositoryRoot remote get-url $RemoteName).Trim()
$remoteTagLines = @(& git -C $repositoryRoot ls-remote --tags $RemoteName $tagRef "$tagRef^{}")
if ($LASTEXITCODE -ne 0) { throw "Could not read pushed Alpha 3 tag from '$RemoteName'." }
$remoteTagObject = $null
$remoteTagTarget = $null
foreach ($line in $remoteTagLines) {
    $parts = @($line -split "`t")
    if ($parts.Count -eq 2 -and $parts[1] -ceq $tagRef) { $remoteTagObject = [string]$parts[0] }
    if ($parts.Count -eq 2 -and $parts[1] -ceq "$tagRef^{}") { $remoteTagTarget = [string]$parts[0] }
}
if ($remoteTagObject -cne $tagObject -or $remoteTagTarget -cne $tagTarget) {
    throw 'Pushed Alpha 3 tag object/target does not match the local annotated tag.'
}
$remoteLegacy = @(& git -C $repositoryRoot ls-remote --heads $RemoteName refs/heads/legacy/1.x)
if ($remoteLegacy.Count -ne 1) { throw 'Could not resolve remote legacy/1.x.' }
$legacyCommit = [string](@($remoteLegacy[0] -split "`t")[0])
if ($legacyCommit -cne 'c4115b82ea43fdd763685d862a08fe5c61db6dff') { throw "Remote legacy/1.x moved: $legacyCommit" }
$remoteBeta = @(& git -C $repositoryRoot ls-remote --tags $RemoteName refs/tags/v1.3.0b1 'refs/tags/v1.3.0b1^{}')
if ($remoteBeta.Count -ne 0) { throw 'Remote v1.3.0b1 exists without explicit human approval.' }

$qualified = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.3-qualified.json') -Raw | ConvertFrom-Json
$assets = @($qualified.assets | ForEach-Object {
    [ordered]@{
        kind = [string]$_.kind; lane = [string]$_.lane; name = [string]$_.name; sha256 = [string]$_.sha256
        entryManifestName = [string]$_.entryManifestName; entryManifestSha256 = [string]$_.entryManifestSha256
    }
})
$feedLines = @(Get-Content -LiteralPath (Join-Path $repositoryRoot 'VERSION'))
if (($feedLines -join "`n") -cne (@('1.2.0', 'Release', '14/05/2026') -join "`n")) { throw 'Public VERSION feed changed before Alpha 3 post-tag attestation.' }
$record = [ordered]@{
    schemaVersion = 1
    status = 'pass'
    releaseLabel = '1.3.0a3'
    tagName = $tagName
    tagObject = $tagObject
    tagTarget = $tagTarget
    remoteName = $RemoteName
    remoteUrl = $remoteUrl
    remoteTagObject = $remoteTagObject
    remoteTagTarget = $remoteTagTarget
    packageSource = [string]$qualified.packageSource
    toolchainLockSha256 = [string]$qualified.toolchainLockSha256
    releaseAssetRecordSha256 = [string]$qualified.releaseAssetRecordSha256
    distributionChecksumManifestSha256 = [string]$qualified.distributionChecksumManifestSha256
    assets = $assets
    publicationStatus = 'retained-unpublished'
    publicReleaseCreated = $false
    feedChanged = $false
    publicFeed = $feedLines
    legacyMoved = $false
    legacyCommit = $legacyCommit
    packagesRetained = $true
    betaAuthorized = $false
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
}
[IO.File]::WriteAllText($recordPath, (($record | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
Write-Host "Populated Alpha 3 post-tag attestation for E '$headCommit'. Commit only this file as direct child P, push dev/1.x, then run verify-alpha3-tagged.ps1 -TagState PostTag."
