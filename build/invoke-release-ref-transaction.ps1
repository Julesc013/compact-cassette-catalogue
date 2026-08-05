[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'High')]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('CreateCandidate', 'PromoteCandidate', 'CreatePost', 'PromotePost')]
    [string]$Mode,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)(?:-(?:alpha|beta|rc)\.[1-9][0-9]*)?$')]
    [string]$ReleaseLabel,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-f]{40}$')]
    [string]$ExpectedCommit,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-f]{40}$')]
    [string]$ExpectedMasterCommit,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-f]{40}$')]
    [string]$ExpectedDevCommit,

    [ValidatePattern('^[A-Za-z0-9][A-Za-z0-9._-]*$')]
    [string]$RemoteName = 'origin',
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$RepositoryRoot = [IO.Path]::GetFullPath($RepositoryRoot)
$branchContract = & (Join-Path $PSScriptRoot 'get-branch-contract.ps1') `
    -RepositoryRoot $RepositoryRoot
$qualifiedBranch = [string]$branchContract.CurrentQualified
$integrationBranch = [string]$branchContract.CurrentIntegration
$qualifiedReference = "refs/heads/$qualifiedBranch"
$integrationReference = "refs/heads/$integrationBranch"
$tagName = & (Join-Path $PSScriptRoot 'resolve-release-tag.ps1') `
    -ReleaseLabel $ReleaseLabel
$phaseName = if ($Mode -like '*Candidate') { 'candidate' } else { 'post' }
$attestationBranch = "attest/$tagName-$phaseName-$ExpectedCommit"
$attestationReference = "refs/heads/$attestationBranch"
$tagReference = "refs/tags/$tagName"

function Invoke-Git {
    param([string[]]$Arguments)

    $savedErrorActionPreference = $ErrorActionPreference
    try {
        # Git writes ordinary progress to stderr. Windows PowerShell 5.1 can
        # otherwise turn a successful native command into NativeCommandError.
        $ErrorActionPreference = 'Continue'
        $output = @(& git -C $RepositoryRoot @Arguments 2>&1)
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
    if ($exitCode -ne 0) {
        throw "git $($Arguments -join ' ') failed:`n$($output -join "`n")"
    }
    return $output
}

function Invoke-GitProbe {
    param(
        [string[]]$Arguments,
        [ref]$ExitCode
    )

    $savedErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        $output = @(& git -C $RepositoryRoot @Arguments 2>$null)
        $ExitCode.Value = $LASTEXITCODE
        return $output
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
}

function Get-RemoteReferenceObject {
    param(
        [string]$Reference,
        [switch]$AllowMissing
    )

    $exitCode = 0
    $lines = @(Invoke-GitProbe `
        @('ls-remote', $RemoteName, $Reference) `
        ([ref]$exitCode))
    if ($exitCode -ne 0) {
        throw "Could not inspect $RemoteName $Reference."
    }
    $matches = @($lines | Where-Object {
            $parts = @(([string]$_).Trim() -split '\s+')
            $parts.Count -eq 2 -and $parts[1] -ceq $Reference
        })
    if ($matches.Count -eq 0 -and $AllowMissing) {
        return $null
    }
    if ($matches.Count -ne 1) {
        throw "$RemoteName $Reference must resolve exactly once."
    }
    return (@(([string]$matches[0]).Trim() -split '\s+'))[0]
}

function Assert-RemoteReference {
    param(
        [string]$Reference,
        [string]$ExpectedObject
    )

    $actualObject = Get-RemoteReferenceObject $Reference
    if ($actualObject -cne $ExpectedObject) {
        throw "$RemoteName $Reference is $actualObject; expected $ExpectedObject."
    }
}

function Assert-CommitObject {
    param([string]$Commit)

    $exitCode = 0
    [void](Invoke-GitProbe @('cat-file', '-e', "$Commit`^{commit}") ([ref]$exitCode))
    if ($exitCode -ne 0) {
        throw "Expected commit object is unavailable: $Commit"
    }
}

function Assert-Ancestor {
    param(
        [string]$Ancestor,
        [string]$Descendant,
        [string]$Context
    )

    $exitCode = 0
    [void](Invoke-GitProbe `
        @('merge-base', '--is-ancestor', $Ancestor, $Descendant) `
        ([ref]$exitCode))
    if ($exitCode -ne 0) {
        throw "$Context requires $Descendant to fast-forward $Ancestor."
    }
}

function Get-RemoteAnnotatedTag {
    $exitCode = 0
    $lines = @(Invoke-GitProbe `
        @('ls-remote', '--tags', $RemoteName, $tagReference, "$tagReference^{}") `
        ([ref]$exitCode))
    if ($exitCode -ne 0) {
        throw "Could not inspect $RemoteName tag $tagName."
    }

    $tagObject = $null
    $peeledCommit = $null
    foreach ($line in $lines) {
        $parts = @(([string]$line).Trim() -split '\s+')
        if ($parts.Count -ne 2) {
            continue
        }
        if ($parts[1] -ceq $tagReference) {
            $tagObject = $parts[0]
        }
        elseif ($parts[1] -ceq "$tagReference^{}") {
            $peeledCommit = $parts[0]
        }
    }
    if ($tagObject -cnotmatch '^[0-9a-f]{40}$' -or
        $peeledCommit -cnotmatch '^[0-9a-f]{40}$') {
        throw "$RemoteName tag $tagName must exist exactly once as an annotated tag."
    }
    return [PSCustomObject]@{
        TagObject = $tagObject
        PeeledCommit = $peeledCommit
    }
}

function Get-TransactionMilestone {
    $catalogPath = Join-Path $RepositoryRoot 'release\catalog.v1.json'
    $schemaPath = Join-Path $RepositoryRoot 'spec\release-catalog\v1\catalog.schema.json'
    $validatorPath = Join-Path $RepositoryRoot 'build\validate-json-document.ps1'
    foreach ($requiredPath in @($catalogPath, $schemaPath, $validatorPath)) {
        if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
            throw "Release transaction input is missing: $requiredPath"
        }
    }
    & $validatorPath `
        -SchemaPath $schemaPath `
        -DocumentPath $catalogPath `
        -MaximumBytes (4 * 1024 * 1024) | Out-Null

    $catalog = Get-Content -LiteralPath $catalogPath -Raw | ConvertFrom-Json
    $matches = @($catalog.milestones | Where-Object {
            [string]$_.releaseLabel -ceq $ReleaseLabel
        })
    if ($matches.Count -ne 1) {
        throw "Release catalogue must contain exactly one '$ReleaseLabel' milestone."
    }
    $milestone = $matches[0]
    if ([string]$milestone.promotion.tag -cne $tagName) {
        throw "Release catalogue milestone '$ReleaseLabel' must own tag $tagName."
    }
    return $milestone
}

if (-not (Test-Path -LiteralPath (Join-Path $RepositoryRoot '.git'))) {
    throw "RepositoryRoot is not a Git worktree: $RepositoryRoot"
}

$status = @(Invoke-Git @('status', '--porcelain=v1', '--untracked-files=all'))
if ($status.Count -ne 0) {
    throw 'Release reference transactions require a completely clean worktree.'
}

foreach ($commit in @($ExpectedCommit, $ExpectedMasterCommit, $ExpectedDevCommit)) {
    Assert-CommitObject $commit
}

$headCommit = ([string](@(Invoke-Git @('rev-parse', 'HEAD'))[-1])).Trim()
if ($headCommit -cne $ExpectedCommit) {
    throw "HEAD is $headCommit; expected exact transaction commit $ExpectedCommit."
}

$parentFields = @((([string](@(Invoke-Git `
                    @('rev-list', '--parents', '-n', '1', $ExpectedCommit))[-1])).Trim()) -split ' ')
if ($parentFields.Count -ne 2) {
    throw "$ExpectedCommit must have exactly one parent."
}
$parentCommit = $parentFields[1]

Assert-RemoteReference $qualifiedReference $ExpectedMasterCommit
Assert-RemoteReference $integrationReference $ExpectedDevCommit

$remoteAttestation = Get-RemoteReferenceObject $attestationReference -AllowMissing
$remoteTagObject = Get-RemoteReferenceObject $tagReference -AllowMissing
$transactionMilestone = Get-TransactionMilestone

if ($Mode -like '*Candidate') {
    if ($parentCommit -cne $ExpectedDevCommit) {
        throw "Candidate E must be the direct child of origin/$integrationBranch C $ExpectedDevCommit."
    }
    Assert-Ancestor $ExpectedMasterCommit $ExpectedCommit 'Candidate promotion'
    if ([string]$transactionMilestone.promotion.state -cne 'unpromoted' -or
        $null -ne $transactionMilestone.promotion.tagObject) {
        throw 'Candidate E must record unpromoted state and a null annotated-tag object.'
    }
}
else {
    if ($ExpectedMasterCommit -cne $ExpectedDevCommit) {
        throw "Post-operation creation/promotion requires $qualifiedBranch and $integrationBranch to identify the same E."
    }
    if ($parentCommit -cne $ExpectedMasterCommit) {
        throw "Post-operation P must be the direct child of E $ExpectedMasterCommit."
    }
    if ([string]$transactionMilestone.promotion.state -cne 'tagged' -or
        [string]$transactionMilestone.promotion.tagObject -cnotmatch '^[0-9a-f]{40}$') {
        throw 'Post-operation P must record tagged state and the full annotated-tag object.'
    }
}

if ($Mode -ceq 'CreateCandidate') {
    if ($null -ne $remoteAttestation) {
        throw "$RemoteName $attestationReference already exists; transport refs are create-only."
    }
    if ($null -ne $remoteTagObject) {
        throw "$RemoteName $tagReference already exists before candidate qualification."
    }
    $arguments = @(
        'push',
        "--force-with-lease=${attestationReference}:",
        $RemoteName,
        "${ExpectedCommit}:$attestationReference"
    )
    if ($PSCmdlet.ShouldProcess(
            "$RemoteName/$attestationBranch",
            "create exact candidate transport ref at $ExpectedCommit")) {
        [void](Invoke-Git $arguments)
    }
}
elseif ($Mode -ceq 'PromoteCandidate') {
    if ($remoteAttestation -cne $ExpectedCommit) {
        throw "$RemoteName $attestationReference is not exact E $ExpectedCommit."
    }
    if ($null -ne $remoteTagObject) {
        throw "$RemoteName $tagReference already exists; tag creation is create-only."
    }

    $tagType = ([string](@(Invoke-Git @('cat-file', '-t', $tagReference))[-1])).Trim()
    $tagCommit = ([string](@(Invoke-Git @('rev-list', '-n', '1', $tagReference))[-1])).Trim()
    if ($tagType -cne 'tag' -or $tagCommit -cne $ExpectedCommit) {
        throw "Local $tagReference must be an annotated tag resolving exactly to E $ExpectedCommit."
    }

    $arguments = @(
        'push', '--atomic',
        "--force-with-lease=${qualifiedReference}:$ExpectedMasterCommit",
        "--force-with-lease=${integrationReference}:$ExpectedDevCommit",
        "--force-with-lease=${attestationReference}:$ExpectedCommit",
        "--force-with-lease=${tagReference}:",
        $RemoteName,
        "${ExpectedCommit}:$qualifiedReference",
        "${ExpectedCommit}:$integrationReference",
        "${tagReference}:${tagReference}",
        ":$attestationReference"
    )
    if ($PSCmdlet.ShouldProcess(
            $RemoteName,
            "atomically promote/tag E $ExpectedCommit and consume $attestationBranch")) {
        [void](Invoke-Git $arguments)
    }
}
elseif ($Mode -ceq 'CreatePost') {
    if ($null -ne $remoteAttestation) {
        throw "$RemoteName $attestationReference already exists; transport refs are create-only."
    }
    $remoteTag = Get-RemoteAnnotatedTag
    if ($remoteTag.PeeledCommit -cne $ExpectedMasterCommit) {
        throw "$RemoteName $tagReference does not resolve to E $ExpectedMasterCommit."
    }
    if ($remoteTag.TagObject -cne [string]$transactionMilestone.promotion.tagObject) {
        throw "$RemoteName $tagReference differs from the annotated-tag object recorded by P."
    }
    $arguments = @(
        'push',
        "--force-with-lease=${attestationReference}:",
        $RemoteName,
        "${ExpectedCommit}:$attestationReference"
    )
    if ($PSCmdlet.ShouldProcess(
            "$RemoteName/$attestationBranch",
            "create exact post-operation transport ref at $ExpectedCommit")) {
        [void](Invoke-Git $arguments)
    }
}
else {
    if ($remoteAttestation -cne $ExpectedCommit) {
        throw "$RemoteName $attestationReference is not exact P $ExpectedCommit."
    }
    $remoteTag = Get-RemoteAnnotatedTag
    if ($remoteTag.PeeledCommit -cne $ExpectedMasterCommit) {
        throw "$RemoteName $tagReference does not resolve to E $ExpectedMasterCommit."
    }
    if ($remoteTag.TagObject -cne [string]$transactionMilestone.promotion.tagObject) {
        throw "$RemoteName $tagReference differs from the annotated-tag object recorded by P."
    }
    Assert-Ancestor $ExpectedMasterCommit $ExpectedCommit 'Post-operation promotion'

    $arguments = @(
        'push', '--atomic',
        "--force-with-lease=${qualifiedReference}:$ExpectedMasterCommit",
        "--force-with-lease=${integrationReference}:$ExpectedDevCommit",
        "--force-with-lease=${attestationReference}:$ExpectedCommit",
        $RemoteName,
        "${ExpectedCommit}:$qualifiedReference",
        "${ExpectedCommit}:$integrationReference",
        ":$attestationReference"
    )
    if ($PSCmdlet.ShouldProcess(
            $RemoteName,
            "atomically promote P $ExpectedCommit and consume $attestationBranch")) {
        [void](Invoke-Git $arguments)
    }
}

Write-Host "Release reference transaction '$Mode' verified for $ExpectedCommit."
Write-Output $attestationBranch
