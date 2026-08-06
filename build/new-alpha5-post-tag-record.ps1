[CmdletBinding()]
param([string]$RemoteName = 'origin')

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha5-contract.ps1')
. (Join-Path $PSScriptRoot 'alpha5-tag-message.ps1')
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$recordPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.5-post-tag.json'
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw 'Post-tag generation requires clean evidence commit E.' }
$template = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
if ([string]$template.status -cne 'template') { throw 'Alpha 5 post-tag record is not an unpopulated template.' }
$evidenceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$tagObject = (& git -C $repositoryRoot rev-parse refs/tags/v1.3.0a5).Trim()
$tagTarget = (& git -C $repositoryRoot rev-parse 'v1.3.0a5^{commit}').Trim()
if ((& git -C $repositoryRoot cat-file -t refs/tags/v1.3.0a5).Trim() -cne 'tag' -or $tagTarget -cne $evidenceCommit) {
    throw 'v1.3.0a5 must be an annotated tag targeting current evidence commit E.'
}
$tagText = (& git -C $repositoryRoot cat-file tag refs/tags/v1.3.0a5) -join "`n"
Assert-C3Alpha5TagMessage -Text $tagText
$remoteLines = @(& git -C $repositoryRoot ls-remote --tags $RemoteName refs/tags/v1.3.0a5 'refs/tags/v1.3.0a5^{}')
$remoteObject = @($remoteLines | Where-Object { $_ -match 'refs/tags/v1\.3\.0a5$' } | ForEach-Object { ($_ -split "`t")[0] })
$remoteTarget = @($remoteLines | Where-Object { $_ -match 'refs/tags/v1\.3\.0a5\^\{\}$' } | ForEach-Object { ($_ -split "`t")[0] })
if ($remoteObject.Count -ne 1 -or $remoteTarget.Count -ne 1 -or $remoteObject[0] -cne $tagObject -or $remoteTarget[0] -cne $tagTarget) {
    throw 'Remote Alpha 5 annotated tag object/target differs from local authority.'
}
$qualified = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.5-qualified.json') -Raw | ConvertFrom-Json
function Get-RemoteHead([string]$Ref) {
    $lines = @(& git -C $repositoryRoot ls-remote --heads $RemoteName $Ref)
    if ($LASTEXITCODE -ne 0 -or $lines.Count -ne 1) { throw "Could not verify protected ref '$Ref'." }
    return [string](@($lines[0] -split "`t")[0])
}
$actualProtected = [ordered]@{
    master = Get-RemoteHead 'refs/heads/master'
    dev2x = Get-RemoteHead 'refs/heads/dev/2.x'
    legacy1x = Get-RemoteHead 'refs/heads/legacy/1.x'
}
foreach ($name in $actualProtected.Keys) {
    if ([string]$actualProtected[$name] -cne [string]$qualified.protectedRefs.$name) { throw "Protected ref '$name' moved during Alpha 5 transaction." }
}
if ((Get-FileHash -LiteralPath (Join-Path $repositoryRoot 'VERSION') -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$qualified.feedSha256) {
    throw 'Public VERSION feed changed during Alpha 5 transaction.'
}
$record = [ordered]@{
    schemaVersion = 1
    status = 'pass'
    releaseLabel = '1.3.0a5'
    tagName = 'v1.3.0a5'
    tagObject = $tagObject
    tagTarget = $tagTarget
    packageSourceCommit = [string]$qualified.sourceCommit
    toolchainLockSha256 = [string]$qualified.toolchainLockSha256
    assetsRetained = 6
    remoteTagVerified = $true
    publicationStatus = 'retained-unpublished'
    publicReleaseCreated = $false
    betaTagCreated = $false
    betaArtifactsCreated = $false
    feedChanged = $false
    protectedRefs = $actualProtected
    ownerAcceptanceStatus = 'pending'
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
}
[IO.File]::WriteAllText($recordPath, (($record | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
Write-Host "Populated Alpha 5 post-tag attestation for E '$evidenceCommit'. Commit only this file as direct child P."
