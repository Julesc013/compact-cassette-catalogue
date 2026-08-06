[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string]$DistributionDirectory,
    [string]$RecordPath,
    [string]$RemoteName = 'origin',
    [switch]$Push
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha4-contract.ps1')
. (Join-Path $PSScriptRoot 'alpha4-tag-message.ps1')
$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($DistributionDirectory)) { $DistributionDirectory = Join-Path $repositoryRoot 'artifacts\distributions\1.3.0a4' }
if ([string]::IsNullOrWhiteSpace($RecordPath)) { $RecordPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.4-qualified.json' }
$DistributionDirectory = [IO.Path]::GetFullPath($DistributionDirectory)
$RecordPath = [IO.Path]::GetFullPath($RecordPath)

if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw 'Alpha 4 tagging requires a clean evidence commit.' }
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0a4
if ($LASTEXITCODE -eq 0) { throw 'Refusing to create or move existing v1.3.0a4.' }
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
Assert-C3Alpha4Manifest -Manifest $manifest
if (-not (Test-Path -LiteralPath $RecordPath -PathType Leaf)) { throw "Alpha 4 qualification record is missing: $RecordPath" }
$record = Get-Content -LiteralPath $RecordPath -Raw | ConvertFrom-Json
$head = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$parent = (& git -C $repositoryRoot rev-parse 'HEAD^').Trim()
if ([int]$record.schemaVersion -ne 1 -or [string]$record.status -cne 'pass' -or
        [string]$record.releaseLabel -cne '1.3.0a4' -or [string]$record.releaseTag -cne 'v1.3.0a4' -or
        [string]$record.publicationStatus -cne 'retained-unpublished' -or
        [string]$record.sourceCommit -cne $parent -or [string]$record.sourceCommit -notmatch '^[0-9a-f]{40}$' -or
        [string]$record.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$' -or @($record.assets).Count -ne 6) {
    throw 'Alpha 4 qualification record does not bind a passing source/lock/six-asset test checkpoint.'
}
$allowedEvidenceChanges = @('release/validation/1.3.0-alpha.4-qualified.json', 'release/validation/1.3.0-alpha.4-qualified.md')
$evidenceChanges = @(& git -C $repositoryRoot diff --name-only $parent $head | Sort-Object)
$unexpectedEvidenceChanges = @(Compare-Object -ReferenceObject ($allowedEvidenceChanges | Sort-Object) -DifferenceObject $evidenceChanges)
if ($unexpectedEvidenceChanges.Count -ne 0) {
    throw "Alpha 4 evidence commit must change only the qualified JSON/Markdown pair: $($evidenceChanges -join ', ')"
}
$expectedTopLevel = @($script:C3Alpha4AssetNames) + @('SHA256SUMS.txt')
$actualTopLevel = @(Get-ChildItem -LiteralPath $DistributionDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualTopLevel -join "`n") -cne (($expectedTopLevel | Sort-Object) -join "`n")) { throw 'Alpha 4 distribution is not the exact six-ZIP/checksum set.' }
$checksumLines = @(Get-Content -LiteralPath (Join-Path $DistributionDirectory 'SHA256SUMS.txt'))
if ($checksumLines.Count -ne 6) { throw 'Alpha 4 checksum file must contain exactly six records.' }
foreach ($name in $script:C3Alpha4AssetNames) {
    $asset = @($record.assets | Where-Object { [string]$_.name -ceq $name })
    $path = Join-Path $DistributionDirectory $name
    $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($asset.Count -ne 1 -or [string]$asset[0].sha256 -cne $hash -or [long]$asset[0].size -ne [long](Get-Item -LiteralPath $path).Length -or
            @($checksumLines | Where-Object { $_ -ceq "$hash  $name" }).Count -ne 1) {
        throw "Alpha 4 qualification record/checksum/asset mismatch: $name"
    }
}
Assert-C3Alpha4TagMessage -Text $script:C3Alpha4TagMessage
if ($PSCmdlet.ShouldProcess("evidence commit $head", 'Create immutable annotated v1.3.0a4')) {
    & git -C $repositoryRoot tag -a v1.3.0a4 -m $script:C3Alpha4TagMessage $head
    if ($LASTEXITCODE -ne 0) { throw 'Could not create annotated v1.3.0a4.' }
    if ($Push) {
        & git -C $repositoryRoot push $RemoteName refs/tags/v1.3.0a4
        if ($LASTEXITCODE -ne 0) { throw 'Could not push v1.3.0a4.' }
    }
}
Write-Host "Created annotated v1.3.0a4 at evidence commit $head for package source $parent."
