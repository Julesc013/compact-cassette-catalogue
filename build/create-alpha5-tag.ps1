[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string]$DistributionDirectory,
    [string]$RemoteName = 'origin',
    [switch]$Push
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha5-contract.ps1')
. (Join-Path $PSScriptRoot 'alpha5-tag-message.ps1')
$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($DistributionDirectory)) { $DistributionDirectory = Join-Path $repositoryRoot 'artifacts\distributions\1.3.0a5' }
$DistributionDirectory = [IO.Path]::GetFullPath($DistributionDirectory)
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw 'Alpha 5 tagging requires clean evidence commit E.' }
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0a5
if ($LASTEXITCODE -eq 0) { throw 'Refusing to create or move existing v1.3.0a5.' }
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
Assert-C3Alpha5Manifest -Manifest $manifest
$record = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.5-qualified.json') -Raw | ConvertFrom-Json
$evidenceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$sourceCommit = (& git -C $repositoryRoot rev-parse 'HEAD^').Trim()
if ([string]$record.status -cne 'pass' -or [string]$record.sourceCommit -cne $sourceCommit -or
        [string]$record.releaseLabel -cne '1.3.0a5' -or @($record.assets).Count -ne 6) {
    throw 'Alpha 5 evidence record does not bind the direct source parent and six assets.'
}
$allowed = @('release/validation/1.3.0-alpha.5-qualified.json', 'release/validation/1.3.0-alpha.5-qualified.md')
$changes = @(& git -C $repositoryRoot diff --name-only $sourceCommit $evidenceCommit | Sort-Object)
if ((Compare-Object ($allowed | Sort-Object) $changes).Count -ne 0) {
    throw "Alpha 5 evidence commit E changed unexpected paths: $($changes -join ', ')"
}
Assert-C3Alpha5Distribution -Directory $DistributionDirectory -Record $record
Assert-C3Alpha5TagMessage -Text $script:C3Alpha5TagMessage
if ($PSCmdlet.ShouldProcess("evidence commit $evidenceCommit", 'Create immutable annotated v1.3.0a5')) {
    & git -C $repositoryRoot tag -a v1.3.0a5 -m $script:C3Alpha5TagMessage $evidenceCommit
    if ($LASTEXITCODE -ne 0) { throw 'Could not create annotated v1.3.0a5.' }
    if ($Push) {
        & git -C $repositoryRoot push $RemoteName refs/tags/v1.3.0a5
        if ($LASTEXITCODE -ne 0) { throw 'Could not push v1.3.0a5.' }
    }
}
Write-Host "Created annotated v1.3.0a5 at evidence commit E '$evidenceCommit' for source C '$sourceCommit'."
