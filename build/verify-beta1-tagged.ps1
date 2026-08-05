[CmdletBinding()]
param(
    [ValidateSet('Tagged', 'PostTag')][string]$TagState = 'Tagged',
    [ValidateSet('PrePromotion', 'Promoted')][string]$LegacyState = 'Promoted',
    [string]$RemoteName = 'origin'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-tag-message.ps1')
. (Join-Path $PSScriptRoot 'beta1-verdict.ps1')
. (Join-Path $PSScriptRoot 'beta1-contract.ps1')
. (Join-Path $PSScriptRoot 'beta1-publication.ps1')
. (Join-Path $PSScriptRoot 'beta1-topology.ps1')

function Get-C3RemoteSha {
    param([string]$Remote, [string]$Ref)
    $lines = @(& git -C $repositoryRoot ls-remote $Remote $Ref)
    if ($LASTEXITCODE -ne 0 -or $lines.Count -ne 1) { throw "Could not resolve exact remote ref '$Ref'." }
    return [string](@($lines[0] -split "`t")[0])
}
$repositoryRoot = Split-Path -Parent $PSScriptRoot
[void](Assert-C3Beta1ManifestPath -Path (Join-Path $PSScriptRoot 'lanes.json'))
$verdictPath = Join-Path $repositoryRoot 'release\validation\1.3.0-beta.1-verdict.json'
$verdict = Assert-C3Beta1Verdict -Path $verdictPath -RepositoryRoot $repositoryRoot -RequireGo
$headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$evidenceCommit = $headCommit
if ($TagState -ceq 'PostTag') {
    $parents = @((& git -C $repositoryRoot rev-list --parents -n 1 HEAD).Trim().Split(' '))
    if ($parents.Count -ne 2) { throw 'P-beta must be the single-parent direct child of E-beta.' }
    $evidenceCommit = $parents[1]
}
$tagRef = 'refs/tags/v1.3.0b1'
if ((& git -C $repositoryRoot cat-file -t $tagRef).Trim() -cne 'tag') { throw 'v1.3.0b1 is not an annotated tag.' }
$tagText = @(& git -C $repositoryRoot cat-file tag $tagRef) -join "`n"
Assert-C3Beta1TagMessage -Text $tagText
$tagObject = (& git -C $repositoryRoot rev-parse $tagRef).Trim()
$tagTarget = (& git -C $repositoryRoot rev-parse 'v1.3.0b1^{commit}').Trim()
if ($tagTarget -cne $evidenceCommit) { throw 'v1.3.0b1 does not point to exact E-beta.' }
$evidenceParents = @((& git -C $repositoryRoot rev-list --parents -n 1 $evidenceCommit).Trim().Split(' '))
if ($evidenceParents.Count -ne 2 -or $evidenceParents[1] -cne [string]$verdict.sourceCommit) { throw 'E-beta is not the direct child of package source C-beta.' }
$evidenceChanges = @(& git -C $repositoryRoot diff --name-only $verdict.sourceCommit $evidenceCommit)
$expectedEvidenceChanges = @('release/validation/1.3.0-beta.1-verdict.json', 'release/validation/1.3.0-beta.1-verdict.md')
if ((($evidenceChanges | Sort-Object) -join "`n") -cne (($expectedEvidenceChanges | Sort-Object) -join "`n")) { throw 'E-beta must change exactly the machine and human Beta verdict records.' }
$humanVerdict = Get-Content -LiteralPath (Join-Path $repositoryRoot 'release\validation\1.3.0-beta.1-verdict.md') -Raw
foreach ($fragment in @(
        'Status: GO',
        "Package source C-beta: ``$($verdict.sourceCommit)``",
        'Portable Beta GO: true',
        'Classic setup Beta GO: true',
        'Overall Beta GO: true',
        'Tag authorized: true',
        'Legacy promotion authorized: true',
        'Public GitHub release: not authorized',
        'VERSION feed: unchanged',
        '`master` and `dev/2.x`: unchanged')) {
    if (-not $humanVerdict.Contains($fragment)) { throw "E-beta human verdict is missing: $fragment" }
}
if ($humanVerdict.Contains('TEMPLATE')) { throw 'E-beta human verdict still contains a template marker.' }
if ((@(Get-Content -LiteralPath (Join-Path $repositoryRoot 'VERSION')) -join "`n") -cne (@('1.2.0', 'Release', '14/05/2026') -join "`n")) { throw 'Public VERSION feed changed during Beta.' }
Assert-C3Beta1CommitTopology -RepositoryRoot $repositoryRoot -SourceCommit ([string]$verdict.sourceCommit) `
    -EvidenceCommit $evidenceCommit -PostTagCommit $(if ($TagState -ceq 'PostTag') { $headCommit } else { $null })
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw "$TagState Beta verification requires a clean tree." }

$legacyCheckpoint = 'c4115b82ea43fdd763685d862a08fe5c61db6dff'
if ($TagState -ceq 'Tagged') {
    if ((& git -C $repositoryRoot rev-parse legacy/1.x).Trim() -cne $legacyCheckpoint) { throw 'legacy/1.x moved before P-beta.' }
    Write-Host 'Verified exact C-beta -> E-beta -> annotated v1.3.0b1 topology with complete GO; P-beta and legacy promotion remain pending.'
    return
}

$postPath = Join-Path $repositoryRoot 'release\validation\1.3.0-beta.1-post-tag.json'
$post = Get-Content -LiteralPath $postPath -Raw | ConvertFrom-Json
$postChanges = @(& git -C $repositoryRoot diff --name-only $evidenceCommit $headCommit)
if ($postChanges.Count -ne 1 -or $postChanges[0] -cne 'release/validation/1.3.0-beta.1-post-tag.json') { throw 'P-beta may change only the post-tag attestation.' }
$remoteUrl = (& git -C $repositoryRoot remote get-url $RemoteName).Trim()
$remoteTagObject = Get-C3RemoteSha -Remote $RemoteName -Ref $tagRef
$remoteTagTarget = Get-C3RemoteSha -Remote $RemoteName -Ref "$tagRef^{}"
$remoteDev = Get-C3RemoteSha -Remote $RemoteName -Ref 'refs/heads/dev/1.x'
$remoteLegacy = Get-C3RemoteSha -Remote $RemoteName -Ref 'refs/heads/legacy/1.x'
$remoteMaster = Get-C3RemoteSha -Remote $RemoteName -Ref 'refs/heads/master'
$remoteDev2 = Get-C3RemoteSha -Remote $RemoteName -Ref 'refs/heads/dev/2.x'
$publicApi = Assert-C3NoPublicBetaRelease -RemoteUrl $remoteUrl
$expectedLegacy = if ($LegacyState -ceq 'Promoted') { $headCommit } else { $legacyCheckpoint }
if ($remoteTagObject -cne $tagObject -or $remoteTagTarget -cne $evidenceCommit -or $remoteDev -cne $headCommit -or
        $remoteLegacy -cne $expectedLegacy -or $remoteMaster -cne [string]$post.masterCommit -or $remoteDev2 -cne [string]$post.dev2Commit) {
    throw 'Remote tag/development/ledger/protected refs do not match the exact Beta topology and boundary snapshot.'
}
$candidateIndexPath = Join-Path (Join-Path $repositoryRoot ([string]$verdict.candidate.path).Replace('/', '\')) 'evidence\candidate.json'
if ([int]$post.schemaVersion -ne 1 -or [string]$post.status -cne 'pass' -or [string]$post.releaseLabel -cne '1.3.0b1' -or
        [string]$post.tagName -cne 'v1.3.0b1' -or [string]$post.tagObject -cne $tagObject -or [string]$post.tagTarget -cne $evidenceCommit -or
        [string]$post.packageSource -cne [string]$verdict.sourceCommit -or
        [string]$post.candidateIndexSha256 -cne (Get-FileHash -LiteralPath $candidateIndexPath -Algorithm SHA256).Hash.ToLowerInvariant() -or
        [string]$post.remoteName -cne $RemoteName -or [string]$post.remoteUrl -cne $remoteUrl -or
        [string]$post.remoteTagObject -cne $tagObject -or [string]$post.remoteTagTarget -cne $evidenceCommit -or
        [string]$post.legacyOldCommit -cne $legacyCheckpoint -or [string]$post.publicReleaseApi -cne $publicApi -or
        -not [bool]$post.publicReleaseAbsent -or [string]$post.publicationStatus -cne 'retained-unpublished' -or
        [bool]$post.feedChanged -or [bool]$post.masterOrDev2Changed -or -not [bool]$post.legacyPromotionAuthorized) {
    throw 'P-beta record does not bind GO bytes, tag, protected refs, absent public release, feed, and promotion authority.'
}
Write-Host "Verified exact C-beta -> E-beta -> tag -> P-beta with legacy state '$LegacyState'; publication/feed/master/dev2 unchanged."
