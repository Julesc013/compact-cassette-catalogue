[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Candidate', 'PostPromotion')]
    [string]$Mode,

    [Parameter(Mandatory = $true)]
    [string]$TrustedRepository,

    [Parameter(Mandatory = $true)]
    [string]$TargetRepository,

    [Parameter(Mandatory = $true)]
    [string]$ExpectedCommit,

    [Parameter(Mandatory = $true)]
    [string]$ExpectedTrustedMasterCommit,

    [Parameter(Mandatory = $true)]
    [string]$AttestationRef
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$releaseLabelPattern = '(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)(?:-(?:alpha|beta|rc)\.[1-9][0-9]*)?'
$fullCommitPattern = '^[0-9a-f]{40}$'

function Get-NormalizedRepositoryPath {
    param(
        [string]$Path,
        [string]$Context
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Container)) {
        throw "$Context repository does not exist: $Path"
    }

    $fullPath = [IO.Path]::GetFullPath($Path).TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar)
    $rootItem = Get-Item -LiteralPath $fullPath -Force
    if (($rootItem.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
        throw "$Context repository root cannot be a reparse point: $fullPath"
    }
    $topLevel = (Invoke-Git -Repository $fullPath -Arguments @(
            'rev-parse', '--show-toplevel')).Text.Trim()
    $normalizedTopLevel = [IO.Path]::GetFullPath($topLevel).TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar)
    if (-not $normalizedTopLevel.Equals(
            $fullPath,
            [StringComparison]::OrdinalIgnoreCase)) {
        throw "$Context path is not the repository top level: $fullPath"
    }
    return $fullPath
}

function Assert-StandardRepositoryMetadata {
    param(
        [string]$Repository,
        [string]$Context
    )

    $expectedGitDirectory = [IO.Path]::GetFullPath((Join-Path $Repository '.git'))
    if (-not (Test-Path -LiteralPath $expectedGitDirectory -PathType Container)) {
        throw "$Context must use its own standard .git directory."
    }
    $gitDirectoryItem = Get-Item -LiteralPath $expectedGitDirectory -Force
    if (($gitDirectoryItem.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
        throw "$Context .git directory cannot be a reparse point."
    }

    $actualGitDirectory = (Invoke-Git -Repository $Repository -Arguments @(
            'rev-parse', '--absolute-git-dir')).Text.Trim()
    $normalizedGitDirectory = [IO.Path]::GetFullPath($actualGitDirectory).TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar)
    if (-not $normalizedGitDirectory.Equals(
            $expectedGitDirectory,
            [StringComparison]::OrdinalIgnoreCase)) {
        throw "$Context uses linked or externally shared Git metadata."
    }
}

function Invoke-Git {
    param(
        [string]$Repository,
        [string[]]$Arguments,
        [switch]$AllowFailure
    )

    $savedErrorActionPreference = $ErrorActionPreference
    $exitCode = 0
    $output = @()
    try {
        # Windows PowerShell can promote native stderr to ErrorRecord objects.
        # The exit code remains the authoritative Git result.
        $ErrorActionPreference = 'Continue'
        $output = @(& git --no-optional-locks -C $Repository @Arguments 2>&1)
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }

    if ($exitCode -ne 0 -and -not $AllowFailure) {
        $detail = (@($output | ForEach-Object { [string]$_ }) -join "`n").Trim()
        if ([string]::IsNullOrWhiteSpace($detail)) {
            $detail = "exit code $exitCode"
        }
        throw "Git command failed ($($Arguments -join ' ')): $detail"
    }

    return [PSCustomObject]@{
        ExitCode = $exitCode
        Lines = @($output | ForEach-Object { [string]$_ })
        Text = (@($output | ForEach-Object { [string]$_ }) -join "`n")
    }
}

function Get-HeadCommit {
    param(
        [string]$Repository,
        [string]$Context
    )

    $head = (Invoke-Git -Repository $Repository -Arguments @(
            'rev-parse', '--verify', 'HEAD^{commit}')).Text.Trim()
    if ($head -cnotmatch $fullCommitPattern) {
        throw "$Context HEAD did not resolve to a full lowercase commit SHA."
    }
    return $head
}

function Assert-CleanCheckout {
    param(
        [string]$Repository,
        [string]$Context
    )

    $status = (Invoke-Git -Repository $Repository -Arguments @(
            'status', '--porcelain=v1', '--untracked-files=all')).Lines
    if ($status.Count -ne 0) {
        throw "$Context checkout is not clean; refusing to execute mutable target content."
    }
}

function Get-SingleParent {
    param(
        [string]$Repository,
        [string]$Commit,
        [string]$Context
    )

    $line = (Invoke-Git -Repository $Repository -Arguments @(
            'rev-list', '--parents', '-n', '1', $Commit)).Text.Trim()
    $parts = @($line -split ' ' | Where-Object { $_.Length -gt 0 })
    if ($parts.Count -ne 2 -or $parts[0] -cne $Commit -or
        $parts[1] -cnotmatch $fullCommitPattern) {
        throw "$Context $Commit must have exactly one parent."
    }
    return [string]$parts[1]
}

function Assert-ExactChangedPaths {
    param(
        [string]$Repository,
        [string]$ParentCommit,
        [string]$ChildCommit,
        [string[]]$AllowedPaths,
        [string]$Context
    )

    $actualPaths = @((Invoke-Git -Repository $Repository -Arguments @(
                '-c', 'core.quotepath=true',
                'diff', '--no-renames', '--name-only',
                $ParentCommit, $ChildCommit, '--')).Lines |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
            Sort-Object -Unique)
    $expectedPaths = @($AllowedPaths | Sort-Object -Unique)
    $difference = @(Compare-Object -ReferenceObject $expectedPaths `
            -DifferenceObject $actualPaths -CaseSensitive)
    if ($difference.Count -ne 0) {
        $renderedDifference = ($difference | Out-String).TrimEnd()
        throw "$Context does not contain the exact allowed path set:`n$renderedDifference"
    }
}

function Get-StrictCatalog {
    param(
        [string]$TrustedRoot,
        [string]$TargetRoot
    )

    $validatorPath = Join-Path $TrustedRoot 'build\validate-json-document.ps1'
    $schemaPath = Join-Path $TrustedRoot 'spec\release-catalog\v1\catalog.schema.json'
    $catalogPath = Join-Path $TargetRoot 'release\catalog.v1.json'
    foreach ($requiredPath in @($validatorPath, $schemaPath, $catalogPath)) {
        if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
            throw "Trusted release-target validation input is missing: $requiredPath"
        }
    }

    # The schema and validator come only from the separately checked-out,
    # trusted master commit. No target script runs before this guard succeeds.
    & $validatorPath -SchemaPath $schemaPath -DocumentPath $catalogPath `
        -MaximumBytes 4194304 | Out-Null

    $strictUtf8 = New-Object Text.UTF8Encoding($false, $true)
    $catalogText = $strictUtf8.GetString([IO.File]::ReadAllBytes($catalogPath))
    return ($catalogText | ConvertFrom-Json)
}

function Get-CurrentMilestone {
    param(
        [object]$Catalog,
        [string]$ReleaseLabel
    )

    $matches = @($Catalog.milestones | Where-Object {
            [string]$_.releaseLabel -ceq $ReleaseLabel
        })
    if ($matches.Count -ne 1) {
        throw "Release catalogue must contain exactly one '$ReleaseLabel' milestone."
    }
    return $matches[0]
}

function Get-RemoteHeads {
    param(
        [string]$TrustedRoot,
        [string[]]$References
    )

    $arguments = @('ls-remote', '--heads', 'origin') + $References
    $result = Invoke-Git -Repository $TrustedRoot -Arguments $arguments
    $heads = @{}
    foreach ($line in $result.Lines) {
        $parts = @($line.Trim() -split '\s+')
        if ($parts.Count -ne 2 -or $parts[0] -cnotmatch $fullCommitPattern -or
            $References -cnotcontains $parts[1] -or $heads.ContainsKey($parts[1])) {
            throw "origin returned an unexpected or duplicate release transport ref: $line"
        }
        $heads[$parts[1]] = $parts[0]
    }
    foreach ($reference in $References) {
        if (-not $heads.ContainsKey($reference)) {
            throw "origin does not advertise required release ref '$reference'."
        }
    }
    return $heads
}

function Get-RemoteTagRecords {
    param(
        [string]$TrustedRoot,
        [string]$TagName
    )

    $tagReference = "refs/tags/$TagName"
    $peeledReference = "$tagReference^{}"
    $allowedReferences = @($tagReference, $peeledReference)
    $result = Invoke-Git -Repository $TrustedRoot -Arguments @(
        'ls-remote', '--tags', 'origin', $tagReference, $peeledReference)
    $records = @{}
    foreach ($line in $result.Lines) {
        $parts = @($line.Trim() -split '\s+')
        if ($parts.Count -ne 2 -or $parts[0] -cnotmatch $fullCommitPattern -or
            $allowedReferences -cnotcontains $parts[1] -or
            $records.ContainsKey($parts[1])) {
            throw "origin returned an unexpected or duplicate release tag ref: $line"
        }
        $records[$parts[1]] = $parts[0]
    }
    return $records
}

if ($ExpectedCommit -cnotmatch $fullCommitPattern) {
    throw 'Expected target commit must be a full lowercase 40-character SHA.'
}
if ($ExpectedTrustedMasterCommit -cnotmatch $fullCommitPattern) {
    throw 'Expected trusted master commit must be a full lowercase 40-character SHA.'
}

$trustedRoot = Get-NormalizedRepositoryPath $TrustedRepository 'Trusted master'
$targetRoot = Get-NormalizedRepositoryPath $TargetRepository 'Target'
$branchContract = & (Join-Path $trustedRoot 'build\get-branch-contract.ps1') `
    -RepositoryRoot $trustedRoot
$qualifiedBranch = [string]$branchContract.CurrentQualified
$integrationBranch = [string]$branchContract.CurrentIntegration
$qualifiedReference = "refs/heads/$qualifiedBranch"
$integrationReference = "refs/heads/$integrationBranch"
if ($trustedRoot.Equals($targetRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Trusted master and target must be separate repository checkouts.'
}
$trustedPrefix = $trustedRoot + [IO.Path]::DirectorySeparatorChar
$targetPrefix = $targetRoot + [IO.Path]::DirectorySeparatorChar
if ($trustedRoot.StartsWith($targetPrefix, [StringComparison]::OrdinalIgnoreCase) -or
    $targetRoot.StartsWith($trustedPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Trusted master and target checkouts cannot contain one another.'
}
Assert-StandardRepositoryMetadata $trustedRoot 'Trusted master'
Assert-StandardRepositoryMetadata $targetRoot 'Target'

$expectedScriptPath = [IO.Path]::GetFullPath((Join-Path $trustedRoot `
            'build\validate-trusted-release-target.ps1'))
$runningScriptPath = [IO.Path]::GetFullPath($MyInvocation.MyCommand.Path)
if (-not $runningScriptPath.Equals(
        $expectedScriptPath,
        [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Release-target guard must execute from the trusted master checkout.'
}

Assert-CleanCheckout $trustedRoot 'Trusted master'
Assert-CleanCheckout $targetRoot 'Target'

$trustedHead = Get-HeadCommit $trustedRoot 'Trusted master'
if ($trustedHead -cne $ExpectedTrustedMasterCommit) {
    throw "Trusted checkout HEAD is $trustedHead, not expected master $ExpectedTrustedMasterCommit."
}
$targetHead = Get-HeadCommit $targetRoot 'Target'
if ($targetHead -cne $ExpectedCommit) {
    throw "Target checkout HEAD is $targetHead, not expected commit $ExpectedCommit."
}

$trustedOrigin = (Invoke-Git -Repository $trustedRoot -Arguments @(
        'remote', 'get-url', 'origin')).Text.Trim()
$targetOrigin = (Invoke-Git -Repository $targetRoot -Arguments @(
        'remote', 'get-url', 'origin')).Text.Trim()
if ([string]::IsNullOrWhiteSpace($trustedOrigin) -or
    $targetOrigin -cne $trustedOrigin) {
    throw 'Trusted master and target checkouts must use the identical origin URL.'
}

$modeToken = if ($Mode -ceq 'Candidate') { 'candidate' } else { 'post' }
$transportPattern = "^attest/v(?<label>$releaseLabelPattern)-$modeToken-(?<commit>[0-9a-f]{40})$"
if ($AttestationRef -cnotmatch $transportPattern) {
    throw "Attestation ref does not have the exact SHA-bound $modeToken transport shape."
}
$releaseLabel = [string]$Matches['label']
$transportCommit = [string]$Matches['commit']
if ($transportCommit -cne $ExpectedCommit) {
    throw 'Attestation ref SHA does not equal the expected target commit.'
}
$fullTransportRef = "refs/heads/$AttestationRef"

$parentCommit = Get-SingleParent $targetRoot $ExpectedCommit "$Mode target"
$catalog = Get-StrictCatalog $trustedRoot $targetRoot
$milestone = Get-CurrentMilestone $catalog $releaseLabel
$expectedValidationPath = "release/validation/$releaseLabel.md"
$validationPath = ([string]$milestone.validationRecord).Replace('\', '/')
if ($validationPath -cne $expectedValidationPath) {
    throw "Milestone validationRecord must be exactly '$expectedValidationPath'."
}
if ([string]$milestone.qualification.state -cne 'pass') {
    throw "$Mode milestone must record qualification state 'pass'."
}
if ([string]$milestone.promotion.tag -cne "v$releaseLabel" -or
    [string]$milestone.promotion.targetBranch -cne $qualifiedBranch) {
    throw "$Mode milestone tag or promotion target differs from its release identity."
}

$sourceCommit = [string]$milestone.qualification.sourceCommit
if ($sourceCommit -cnotmatch $fullCommitPattern) {
    throw "$Mode milestone must record a full lowercase sourceCommit."
}

$allowedPaths = @('release/catalog.v1.json', $expectedValidationPath)
if ($Mode -ceq 'Candidate') {
    if ($parentCommit -cne $sourceCommit) {
        throw "Candidate E must be the direct child of recorded source C $sourceCommit."
    }
    if ([string]$milestone.promotion.state -cne 'unpromoted') {
        throw "Candidate E must record promotion state 'unpromoted'."
    }
    if ($null -ne $milestone.promotion.tagObject) {
        throw 'Candidate E must record a null promotion tagObject before tagging.'
    }
    Assert-ExactChangedPaths $targetRoot $parentCommit $ExpectedCommit `
        $allowedPaths 'Candidate C-to-E evidence diff'

    $ancestorProbe = Invoke-Git -Repository $targetRoot -Arguments @(
        'merge-base', '--is-ancestor',
        $ExpectedTrustedMasterCommit, $parentCommit) -AllowFailure
    if ($ancestorProbe.ExitCode -ne 0) {
        throw 'Candidate C must descend from the trusted master checkpoint.'
    }
}
else {
    if ($parentCommit -cne $ExpectedTrustedMasterCommit) {
        throw 'PostPromotion P must be the direct child of trusted master E.'
    }
    if ([string]$milestone.promotion.state -cne 'tagged') {
        throw "PostPromotion P must record promotion state 'tagged'."
    }
    $recordedTagObject = [string]$milestone.promotion.tagObject
    if ($recordedTagObject -cnotmatch $fullCommitPattern) {
        throw 'PostPromotion P must record the full annotated tag-object SHA.'
    }

    $evidenceParent = Get-SingleParent $targetRoot $parentCommit 'Candidate E parent'
    if ($evidenceParent -cne $sourceCommit) {
        throw 'PostPromotion parent E is not the direct child of recorded source C.'
    }
    Assert-ExactChangedPaths $targetRoot $sourceCommit $parentCommit `
        @('release/catalog.v1.json', $expectedValidationPath) `
        'Candidate C-to-E evidence diff'

    if ([bool]$milestone.publication.feedPromoted) {
        $channel = [string]$milestone.channel
        if ($channel -notin @('beta', 'stable')) {
            throw 'Only beta or stable post-promotion outcomes may promote a feed.'
        }
        $allowedPaths += "release/feeds/$channel/release.json"
    }
    Assert-ExactChangedPaths $targetRoot $parentCommit $ExpectedCommit `
        $allowedPaths 'PostPromotion E-to-P evidence diff'

    $tagType = (Invoke-Git -Repository $targetRoot -Arguments @(
            'cat-file', '-t', "refs/tags/v$releaseLabel")).Text.Trim()
    $localTagObject = (Invoke-Git -Repository $targetRoot -Arguments @(
            'rev-parse', "refs/tags/v$releaseLabel")).Text.Trim()
    $tagCommit = (Invoke-Git -Repository $targetRoot -Arguments @(
            'rev-list', '-n', '1', "v$releaseLabel")).Text.Trim()
    if ($tagType -cne 'tag' -or $localTagObject -cne $recordedTagObject -or
        $tagCommit -cne $parentCommit) {
        throw "PostPromotion requires its recorded annotated tag object 'v$releaseLabel' at parent E."
    }
}

$remoteReferences = @(
    $qualifiedReference,
    $integrationReference,
    $fullTransportRef
)
$remoteHeads = Get-RemoteHeads $trustedRoot $remoteReferences
if ([string]$remoteHeads[$qualifiedReference] -cne $ExpectedTrustedMasterCommit) {
    throw "origin/$qualifiedBranch moved away from the trusted workflow-control commit."
}
if ([string]$remoteHeads[$fullTransportRef] -cne $ExpectedCommit) {
    throw 'The exact SHA-bound attestation transport ref does not identify the target commit.'
}
$expectedDevCommit = if ($Mode -ceq 'Candidate') {
    $parentCommit
}
else {
    $ExpectedTrustedMasterCommit
}
if ([string]$remoteHeads[$integrationReference] -cne $expectedDevCommit) {
    throw "origin/$integrationBranch moved away from required $Mode baseline $expectedDevCommit."
}

$remoteTagRecords = Get-RemoteTagRecords $trustedRoot "v$releaseLabel"
$remoteTagReference = "refs/tags/v$releaseLabel"
$remotePeeledReference = "$remoteTagReference^{}"
if ($Mode -ceq 'Candidate') {
    if ($remoteTagRecords.Count -ne 0) {
        throw "Candidate E cannot be qualified after remote tag 'v$releaseLabel' exists."
    }
}
else {
    if ($remoteTagRecords.Count -ne 2 -or
        [string]$remoteTagRecords[$remoteTagReference] -cne $recordedTagObject -or
        [string]$remoteTagRecords[$remotePeeledReference] -cne $parentCommit) {
        throw "origin tag 'v$releaseLabel' does not preserve the recorded tag object and parent E."
    }
}

Write-Host "Trusted $Mode target verified: $ExpectedCommit"
Write-Host "Transport ref: $fullTransportRef"
Write-Host "Remote $qualifiedBranch`: $($remoteHeads[$qualifiedReference])"
Write-Host "Remote $integrationBranch`: $($remoteHeads[$integrationReference])"
