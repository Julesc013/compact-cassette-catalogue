[CmdletBinding()]
param(
    [string]$DistributionDirectory,
    [string]$RecordPath,
    [string]$RemoteName = 'origin',
    [switch]$VerifyRemote
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha4-contract.ps1')
. (Join-Path $PSScriptRoot 'alpha4-tag-message.ps1')
$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($DistributionDirectory)) { $DistributionDirectory = Join-Path $repositoryRoot 'artifacts\distributions\1.3.0a4' }
if ([string]::IsNullOrWhiteSpace($RecordPath)) { $RecordPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.4-qualified.json' }
$head = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ((& git -C $repositoryRoot cat-file -t refs/tags/v1.3.0a4).Trim() -cne 'tag') { throw 'v1.3.0a4 is not annotated.' }
$tagTarget = (& git -C $repositoryRoot rev-parse 'v1.3.0a4^{commit}').Trim()
$evidenceCommit = if ($tagTarget -ceq $head) {
    $head
} elseif ((& git -C $repositoryRoot rev-parse 'HEAD^').Trim() -ceq $tagTarget) {
    $tagTarget
} else {
    throw 'v1.3.0a4 points neither to the current evidence commit nor its direct post-tag parent.'
}
$tagText = (& git -C $repositoryRoot cat-file tag refs/tags/v1.3.0a4) -join "`n"
Assert-C3Alpha4TagMessage -Text $tagText
$record = Get-Content -LiteralPath $RecordPath -Raw | ConvertFrom-Json
if ([string]$record.sourceCommit -cne (& git -C $repositoryRoot rev-parse "$evidenceCommit^").Trim() -or @($record.assets).Count -ne 6) { throw 'Tagged Alpha 4 record does not bind the direct package-source parent and six assets.' }
foreach ($asset in @($record.assets)) {
    $path = Join-Path $DistributionDirectory ([string]$asset.name)
    if (-not (Test-Path -LiteralPath $path -PathType Leaf) -or
            (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$asset.sha256) {
        throw "Tagged Alpha 4 asset changed or is missing: $($asset.name)"
    }
}
if ($VerifyRemote) {
    $remote = @(& git -C $repositoryRoot ls-remote --tags $RemoteName refs/tags/v1.3.0a4 'refs/tags/v1.3.0a4^{}')
    if ($remote.Count -ne 2) { throw 'Remote Alpha 4 annotated tag object/target pair is missing.' }
    $remoteObject = (($remote | Where-Object { $_ -match 'refs/tags/v1\.3\.0a4$' }) -split '\s+')[0]
    $remoteTarget = (($remote | Where-Object { $_ -match 'refs/tags/v1\.3\.0a4\^\{\}$' }) -split '\s+')[0]
    if ($remoteObject -cne (& git -C $repositoryRoot rev-parse refs/tags/v1.3.0a4).Trim() -or $remoteTarget -cne $evidenceCommit) { throw 'Remote Alpha 4 tag differs from the local annotated object/target.' }
}
Write-Host "Verified annotated v1.3.0a4 at evidence commit $evidenceCommit and its unchanged six retained test assets."
