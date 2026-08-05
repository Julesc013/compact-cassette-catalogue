[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$RemoteName = 'origin'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ($RemoteName -cne 'origin') {
    throw "Alpha 2 post-tag authority is fixed to remote 'origin'."
}
$recordPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.2-post-tag.json'
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) {
    throw 'Post-tag record generation requires clean evidence commit E.'
}
$template = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
if ([string]$template.status -cne 'template') {
    throw "Post-tag record is not an unpopulated template: $recordPath"
}

& (Join-Path $PSScriptRoot 'verify-alpha2.ps1') `
    -Configuration $Configuration `
    -TagState Tagged

$tagName = [string]$manifest.releaseTag
$tagRef = "refs/tags/$tagName"
$tagObject = (& git -C $repositoryRoot rev-parse $tagRef).Trim()
$tagTarget = (& git -C $repositoryRoot rev-parse "$tagName^{commit}").Trim()
if ($tagTarget -cne $headCommit) {
    throw "Alpha 2 tag target '$tagTarget' is not current evidence commit E '$headCommit'."
}
$remoteUrl = (& git -C $repositoryRoot remote get-url $RemoteName).Trim()
$remoteTagLines = @(& git -C $repositoryRoot ls-remote --tags $RemoteName $tagRef "$tagRef^{}")
if ($LASTEXITCODE -ne 0) {
    throw "Could not read pushed Alpha 2 tag from '$RemoteName'."
}
$remoteTagObject = $null
$remoteTagTarget = $null
foreach ($line in $remoteTagLines) {
    $parts = @($line -split "`t")
    if ($parts.Count -eq 2 -and $parts[1] -ceq $tagRef) {
        $remoteTagObject = [string]$parts[0]
    }
    if ($parts.Count -eq 2 -and $parts[1] -ceq "$tagRef^{}") {
        $remoteTagTarget = [string]$parts[0]
    }
}
if ($remoteTagObject -cne $tagObject -or $remoteTagTarget -cne $tagTarget) {
    throw "Remote Alpha 2 tag does not equal local annotated object/target: $remoteTagObject / $remoteTagTarget."
}

$remoteLegacyLine = @(& git -C $repositoryRoot ls-remote --heads $RemoteName refs/heads/legacy/1.x)
if ($LASTEXITCODE -ne 0 -or $remoteLegacyLine.Count -ne 1) {
    throw "Could not resolve remote legacy/1.x from '$RemoteName'."
}
$legacyCommit = [string](@($remoteLegacyLine[0] -split "`t")[0])
if ($legacyCommit -cne 'c4115b82ea43fdd763685d862a08fe5c61db6dff') {
    throw "Remote legacy/1.x moved before Alpha 2 post-tag attestation: $legacyCommit"
}

$qualified = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.2-qualified.json') -Raw | ConvertFrom-Json
$packageRecords = @($qualified.packages | ForEach-Object {
    [ordered]@{
        name = [string]$_.name
        sha256 = [string]$_.sha256
        entryManifestName = [string]$_.entryManifestName
        entryManifestSha256 = [string]$_.entryManifestSha256
    }
})
$feedLines = @(Get-Content -LiteralPath (Join-Path $repositoryRoot 'VERSION'))
if (($feedLines -join "`n") -cne (@('1.2.0', 'Release', '14/05/2026') -join "`n")) {
    throw 'Public VERSION feed changed before Alpha 2 post-tag attestation.'
}

$record = [ordered]@{
    schemaVersion = 1
    status = 'pass'
    releaseLabel = [string]$manifest.releaseLabel
    tagName = $tagName
    tagObject = $tagObject
    tagTarget = $tagTarget
    remoteName = $RemoteName
    remoteUrl = $remoteUrl
    remoteTagObject = $remoteTagObject
    remoteTagTarget = $remoteTagTarget
    packageSource = [string]$qualified.packageSource
    toolchainLockSha256 = [string]$qualified.toolchainLockSha256
    packageChecksumManifestSha256 = [string]$qualified.packageChecksumManifestSha256
    entryChecksumManifestSha256 = [string]$qualified.entryChecksumManifestSha256
    packages = $packageRecords
    publicationStatus = 'retained-unpublished'
    publicReleaseCreated = $false
    feedChanged = $false
    publicFeed = $feedLines
    legacyMoved = $false
    legacyCommit = $legacyCommit
    packagesRetained = $true
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
}
$recordJson = ($record | ConvertTo-Json -Depth 8) + "`n"
[IO.File]::WriteAllText($recordPath, $recordJson, (New-Object Text.UTF8Encoding($false)))
Write-Host "Populated Alpha 2 post-tag record for evidence commit E '$headCommit'. Commit only this file as direct child P, push dev/1.x, then run verify-alpha2.ps1 -TagState PostTag."
