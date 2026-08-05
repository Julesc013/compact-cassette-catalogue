[CmdletBinding()]
param(
    [ValidateSet('Repository', 'Candidate', 'Tag', 'Master', 'PostPromotion')]
    [string]$Mode = 'Repository',
    [string]$TagName,
    [string]$ExpectedCommit,
    [string]$CatalogOverridePath,
    [string]$SchemaOverridePath,
    [switch]$RequireRemoteBaseline,
    [switch]$RequireArtifacts
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$gitMetadataAvailable = Test-Path -LiteralPath (Join-Path $repositoryRoot '.git')
$catalogPath = if ([string]::IsNullOrWhiteSpace($CatalogOverridePath)) {
    Join-Path $repositoryRoot 'release\catalog.v1.json'
}
else {
    [IO.Path]::GetFullPath($CatalogOverridePath)
}
$schemaPath = if ([string]::IsNullOrWhiteSpace($SchemaOverridePath)) {
    Join-Path $repositoryRoot 'spec\release-catalog\v1\catalog.schema.json'
}
else {
    [IO.Path]::GetFullPath($SchemaOverridePath)
}
$validationRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'release\validation'))
$branchContract = & (Join-Path $PSScriptRoot 'get-branch-contract.ps1') `
    -RepositoryRoot $repositoryRoot
$qualifiedBranch = [string]$branchContract.CurrentQualified
$integrationBranch = [string]$branchContract.CurrentIntegration
$qualifiedLocalReference = "refs/heads/$qualifiedBranch"
$qualifiedRemoteReference = "refs/remotes/origin/$qualifiedBranch"
$integrationRemoteReference = "refs/remotes/origin/$integrationBranch"
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$tagResolver = Join-Path $PSScriptRoot 'resolve-release-tag.ps1'
$packageDefinitions = @(& (Join-Path $PSScriptRoot 'get-release-packages.ps1') -Identity $identity)
$failures = New-Object Collections.Generic.List[String]

function Add-Failure {
    param([string]$Message)
    $failures.Add($Message)
}

function Invoke-GitReadProbe {
    param(
        [string[]]$Arguments,
        [ref]$ExitCode
    )

    $savedErrorActionPreference = $ErrorActionPreference
    try {
        # Windows PowerShell 5.1 can promote expected native stderr into a
        # terminating NativeCommandError while the script uses Stop globally.
        # Read-only probes own their exit code and intentionally suppress stderr.
        $ErrorActionPreference = 'Continue'
        $output = @(& git -C $repositoryRoot @Arguments 2>$null)
        $ExitCode.Value = $LASTEXITCODE
        return $output
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
}

function Test-RequiredProperties {
    param(
        [object]$Value,
        [string[]]$Names,
        [string]$Context
    )

    if ($null -eq $Value) {
        Add-Failure "$Context is missing."
        return $false
    }
    $propertyNames = @($Value.PSObject.Properties | ForEach-Object { $_.Name })
    $valid = $true
    foreach ($name in $Names) {
        if ($propertyNames -cnotcontains $name) {
            Add-Failure "$Context is missing property '$name'."
            $valid = $false
        }
    }
    foreach ($name in $propertyNames) {
        if ($Names -cnotcontains $name) {
            Add-Failure "$Context has unsupported property '$name'."
            $valid = $false
        }
    }
    return $valid
}

function Test-AllowedValue {
    param(
        [string]$Value,
        [string[]]$Allowed,
        [string]$Context
    )

    if ($Allowed -cnotcontains $Value) {
        Add-Failure "$Context has unsupported value '$Value'."
        return $false
    }
    return $true
}

function Compare-CanonicalNumericText {
    param(
        [string]$Left,
        [string]$Right
    )

    $lengthComparison = $Left.Length.CompareTo($Right.Length)
    if ($lengthComparison -ne 0) {
        return $lengthComparison
    }
    return [StringComparer]::Ordinal.Compare($Left, $Right)
}

function Get-ReleaseOrder {
    param(
        [string]$ProductVersion,
        [string]$Stage
    )

    if ($ProductVersion -cnotmatch
        '^(?<major>0|[1-9][0-9]*)\.(?<minor>0|[1-9][0-9]*)\.(?<patch>0|[1-9][0-9]*)$') {
        return $null
    }
    $major = $Matches.major
    $minor = $Matches.minor
    $patch = $Matches.patch
    $stageRank = 0
    $stageSequence = '0'
    if ($Stage -ceq 'Release') {
        $stageRank = 4
    }
    elseif ($Stage -cmatch
        '^(?<family>Alpha|Beta|Release Candidate) (?<sequence>[1-9][0-9]*)$') {
        switch ($Matches.family) {
            'Alpha' { $stageRank = 1 }
            'Beta' { $stageRank = 2 }
            'Release Candidate' { $stageRank = 3 }
        }
        $stageSequence = $Matches.sequence
    }
    else {
        return $null
    }

    return [PSCustomObject]@{
        Major = $major
        Minor = $minor
        Patch = $patch
        StageRank = $stageRank
        StageSequence = $stageSequence
    }
}

function Compare-ReleaseOrder {
    param(
        [object]$Left,
        [object]$Right
    )

    foreach ($component in @('Major', 'Minor', 'Patch')) {
        $comparison = Compare-CanonicalNumericText `
            ([string]$Left.$component) `
            ([string]$Right.$component)
        if ($comparison -ne 0) {
            return $comparison
        }
    }
    $rankComparison = ([int]$Left.StageRank).CompareTo([int]$Right.StageRank)
    if ($rankComparison -ne 0) {
        return $rankComparison
    }
    return Compare-CanonicalNumericText `
        ([string]$Left.StageSequence) `
        ([string]$Right.StageSequence)
}

function Get-MasterReference {
    foreach ($candidate in @($qualifiedRemoteReference, $qualifiedLocalReference)) {
        & git -C $repositoryRoot show-ref --verify --quiet $candidate
        if ($LASTEXITCODE -eq 0) {
            return $candidate
        }
    }
    return $null
}

function Update-RemoteReleaseReferences {
    if (-not $gitMetadataAvailable) {
        Add-Failure 'release transaction validation requires full Git metadata.'
        return $false
    }

    $savedErrorActionPreference = $ErrorActionPreference
    $originExitCode = 0
    try {
        $ErrorActionPreference = 'Continue'
        & git -C $repositoryRoot remote get-url origin 1>$null 2>$null
        $originExitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
    if ($originExitCode -ne 0) {
        Add-Failure 'release transaction validation requires an origin remote.'
        return $false
    }

    $branchFetchExitCode = 0
    try {
        $ErrorActionPreference = 'Continue'
        & git -C $repositoryRoot fetch `
            --quiet `
            --no-recurse-submodules `
            --prune `
            origin `
            "+refs/heads/${qualifiedBranch}:refs/remotes/origin/${qualifiedBranch}" `
            "+refs/heads/${integrationBranch}:refs/remotes/origin/${integrationBranch}" 1>$null 2>$null
        $branchFetchExitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
    if ($branchFetchExitCode -ne 0) {
        Add-Failure "could not refresh origin/$qualifiedBranch and origin/$integrationBranch before release transaction validation."
        return $false
    }

    $tagFetchExitCode = 0
    try {
        $ErrorActionPreference = 'Continue'
        # Do not prune the caller's local-only tags. Origin existence is checked
        # independently with ls-remote; a moved immutable tag makes this
        # non-forced fetch fail closed.
        & git -C $repositoryRoot fetch `
            --quiet `
            --no-recurse-submodules `
            origin `
            'refs/tags/*:refs/tags/*' 1>$null 2>$null
        $tagFetchExitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
    if ($tagFetchExitCode -ne 0) {
        Add-Failure 'could not refresh immutable origin tags before release transaction validation.'
        return $false
    }
    return $true
}

function Get-EvidencePaths {
    param([object]$Milestone)

    return @(
        'release/catalog.v1.json'
        ([string]$Milestone.validationRecord).Replace('\', '/')
    ) | Sort-Object
}

function ConvertTo-CanonicalJsonValue {
    param([AllowNull()][object]$Value)

    if ($null -eq $Value) {
        return $null
    }
    if ($Value -is [PSCustomObject]) {
        $properties = [ordered]@{}
        foreach ($property in @($Value.PSObject.Properties | Sort-Object Name)) {
            $properties[$property.Name] = ConvertTo-CanonicalJsonValue $property.Value
        }
        return [PSCustomObject]$properties
    }
    if ($Value -is [Array]) {
        $items = New-Object Object[] $Value.Count
        for ($index = 0; $index -lt $Value.Count; $index++) {
            $items[$index] = ConvertTo-CanonicalJsonValue $Value[$index]
        }
        return ,$items
    }
    return $Value
}

function Get-CanonicalJsonText {
    param([AllowNull()][object]$Value)

    $canonicalValue = ConvertTo-CanonicalJsonValue $Value
    return ($canonicalValue | ConvertTo-Json -Compress -Depth 100)
}

function Copy-JsonValue {
    param([object]$Value)

    return (($Value | ConvertTo-Json -Depth 100) | ConvertFrom-Json)
}

function Get-CatalogAtRevision {
    param(
        [string]$Revision,
        [string]$Context,
        [switch]$AllowMissing
    )

    $probeExitCode = 0
    $catalogText = (@(Invoke-GitReadProbe `
                @('show', "${Revision}:release/catalog.v1.json") `
                ([ref]$probeExitCode)) -join "`n")
    if ($probeExitCode -ne 0 -or [string]::IsNullOrWhiteSpace($catalogText)) {
        if (-not $AllowMissing) {
            Add-Failure "$Context does not contain its release catalogue."
        }
        return $null
    }
    try {
        return ($catalogText | ConvertFrom-Json)
    }
    catch {
        Add-Failure "$Context contains an unreadable release catalogue: $($_.Exception.Message)"
        return $null
    }
}

function Test-TaggedMilestoneProjection {
    param(
        [object]$CurrentMilestone,
        [object]$TaggedMilestone,
        [string]$Tag
    )

    $projection = Copy-JsonValue $CurrentMilestone
    $projection.promotion.state = $TaggedMilestone.promotion.state
    $projection.promotion.tagObject = $TaggedMilestone.promotion.tagObject
    $projection.publication.state = $TaggedMilestone.publication.state
    $projection.publication.releaseUrl = $TaggedMilestone.publication.releaseUrl
    $projection.publication.feedPromoted = $TaggedMilestone.publication.feedPromoted
    $projection.postVerification.state = $TaggedMilestone.postVerification.state
    $projection.supersededBy = $TaggedMilestone.supersededBy
    if ($null -ne $TaggedMilestone.promotion.tagObject -or
        [string]$CurrentMilestone.promotion.tagObject -cnotmatch '^[0-9a-f]{40}$') {
        Add-Failure "tag '$Tag' milestone must project promotion.tagObject only from null at E to the exact annotated-tag object at P."
    }
    if ((Get-CanonicalJsonText $projection) -cne (Get-CanonicalJsonText $TaggedMilestone)) {
        Add-Failure "tag '$Tag' milestone identity, qualification, artifact evidence, or other immutable fields differ from the catalogue."
    }
}

function Test-MilestoneMayBeSuperseded {
    param([object]$Milestone)

    $qualificationFailure = [string]$Milestone.qualification.state -ceq 'fail' -and
        [string]$Milestone.promotion.state -ceq 'unpromoted' -and
        [string]$Milestone.publication.state -ceq 'unpublished' -and
        -not [bool]$Milestone.publication.feedPromoted
    $postVerificationFailure = [string]$Milestone.qualification.state -ceq 'pass' -and
        [string]$Milestone.promotion.state -ceq 'tagged' -and
        [string]$Milestone.publication.state -ceq 'published' -and
        [string]$Milestone.postVerification.state -ceq 'failed' -and
        -not [bool]$Milestone.publication.feedPromoted
    return $qualificationFailure -or $postVerificationFailure
}

function Test-HistoricalMilestonesAgainstBaseline {
    param(
        [object]$BaselineCatalog,
        [string]$BaselineContext,
        [string]$CurrentLabel
    )

    foreach ($baselineMilestone in @($BaselineCatalog.milestones)) {
        $baselineLabel = [string]$baselineMilestone.releaseLabel
        if (-not $labels.ContainsKey($baselineLabel)) {
            Add-Failure "$BaselineContext milestone '$baselineLabel' is missing from the current catalogue."
            continue
        }

        $currentMilestone = $labels[$baselineLabel]
        if ((Get-CanonicalJsonText $currentMilestone) -ceq
            (Get-CanonicalJsonText $baselineMilestone)) {
            continue
        }

        if ($baselineLabel -ceq $CurrentLabel) {
            $postProjection = Copy-JsonValue $currentMilestone
            $postProjection.promotion.state = $baselineMilestone.promotion.state
            $postProjection.promotion.tagObject = $baselineMilestone.promotion.tagObject
            $postProjection.publication.state = $baselineMilestone.publication.state
            $postProjection.publication.releaseUrl = $baselineMilestone.publication.releaseUrl
            $postProjection.publication.feedPromoted = $baselineMilestone.publication.feedPromoted
            $postProjection.postVerification.state = $baselineMilestone.postVerification.state
            $postProjection.supersededBy = $baselineMilestone.supersededBy
            $isExactPostTransition =
                [string]$baselineMilestone.qualification.state -ceq 'pass' -and
                [string]$baselineMilestone.promotion.state -ceq 'unpromoted' -and
                $null -eq $baselineMilestone.promotion.tagObject -and
                [string]$currentMilestone.qualification.state -ceq 'pass' -and
                [string]$currentMilestone.promotion.state -ceq 'tagged' -and
                [string]$currentMilestone.promotion.tagObject -cmatch '^[0-9a-f]{40}$' -and
                (Get-CanonicalJsonText $postProjection) -ceq
                    (Get-CanonicalJsonText $baselineMilestone)
            if (-not $isExactPostTransition) {
                Add-Failure "$BaselineContext current milestone '$baselineLabel' was rewritten outside the exact E-to-P transition."
            }
            continue
        }

        $supersessionProjection = Copy-JsonValue $currentMilestone
        $supersessionProjection.supersededBy = $baselineMilestone.supersededBy
        $isImmediateSupersession = (Test-MilestoneMayBeSuperseded $baselineMilestone) -and
            $null -eq $baselineMilestone.supersededBy -and
            [string]$currentMilestone.supersededBy -ceq $CurrentLabel -and
            $labelOrder[$CurrentLabel] -eq ($labelOrder[$baselineLabel] + 1) -and
            (Get-CanonicalJsonText $supersessionProjection) -ceq
                (Get-CanonicalJsonText $baselineMilestone)
        if (-not $isImmediateSupersession) {
            Add-Failure "$BaselineContext historical milestone '$baselineLabel' was rewritten."
        }
    }
}

function Test-EvidenceAttestation {
    param(
        [object]$Milestone,
        [string]$AttestationCommit,
        [string]$Context
    )

    $sourceCommit = [string]$Milestone.qualification.sourceCommit
    if ($sourceCommit -cnotmatch '^[0-9a-f]{40}$' -or
        $AttestationCommit -cnotmatch '^[0-9a-f]{40}$') {
        Add-Failure "$Context requires full source and attestation commit identities."
        return
    }

    & git -C $repositoryRoot merge-base --is-ancestor $sourceCommit $AttestationCommit
    if ($LASTEXITCODE -ne 0) {
        Add-Failure "source commit $sourceCommit is not an ancestor of $Context $AttestationCommit."
        return
    }

    $attestationLine = @(
        ([string](& git -C $repositoryRoot rev-list --parents -n 1 $AttestationCommit)).Trim() -split ' '
    )
    if ($attestationLine.Count -ne 2 -or $attestationLine[1] -cne $sourceCommit) {
        Add-Failure "$Context $AttestationCommit must be exactly one evidence-only commit after source $sourceCommit."
    }

    $allowedEvidence = @(Get-EvidencePaths $Milestone)
    $actualEvidence = @(& git -C $repositoryRoot diff --name-only $sourceCommit $AttestationCommit | Sort-Object)
    $evidenceDifference = @(Compare-Object $allowedEvidence $actualEvidence -CaseSensitive)
    if ($evidenceDifference.Count -gt 0) {
        Add-Failure "$Context source-to-attestation diff is not the exact evidence-only pair:`n$($evidenceDifference | Out-String)"
    }
}

function Get-FirstParentSuccessorCommit {
    param(
        [string]$Commit,
        [string]$HeadCommit,
        [string]$Context
    )

    $firstParentHistory = @(& git -C $repositoryRoot rev-list --first-parent --reverse $HeadCommit)
    if ($LASTEXITCODE -ne 0) {
        Add-Failure "$Context first-parent history could not be read."
        return $null
    }
    $commitIndex = -1
    for ($index = 0; $index -lt $firstParentHistory.Count; $index++) {
        if ([string]$firstParentHistory[$index] -ceq $Commit) {
            $commitIndex = $index
            break
        }
    }
    if ($commitIndex -lt 0) {
        Add-Failure "$Context is not on the first-parent ledger ending at HEAD."
        return $null
    }
    if ($commitIndex -eq ($firstParentHistory.Count - 1)) {
        Add-Failure "$Context records post-operation facts without a first-parent P commit."
        return $null
    }
    return [string]$firstParentHistory[$commitIndex + 1]
}

function Test-FinalizedMilestoneProjection {
    param(
        [object]$CurrentMilestone,
        [object]$FinalizedMilestone,
        [string]$FinalizedCommit
    )

    if ((Get-CanonicalJsonText $CurrentMilestone) -ceq
        (Get-CanonicalJsonText $FinalizedMilestone)) {
        return
    }

    $label = [string]$CurrentMilestone.releaseLabel
    $projection = Copy-JsonValue $CurrentMilestone
    $projection.supersededBy = $FinalizedMilestone.supersededBy
    $isImmediateSupersession = (Test-MilestoneMayBeSuperseded $FinalizedMilestone) -and
        $null -eq $FinalizedMilestone.supersededBy -and
        $null -ne $CurrentMilestone.supersededBy -and
        $labelOrder.ContainsKey($label) -and
        $labels.ContainsKey([string]$CurrentMilestone.supersededBy) -and
        $labelOrder[[string]$CurrentMilestone.supersededBy] -eq ($labelOrder[$label] + 1) -and
        (Get-CanonicalJsonText $projection) -ceq
            (Get-CanonicalJsonText $FinalizedMilestone)
    if (-not $isImmediateSupersession) {
        Add-Failure "milestone '$label' differs from finalized post-operation snapshot $FinalizedCommit."
    }
}

function Test-FinalizedMilestoneSnapshot {
    param(
        [object]$CurrentMilestone,
        [object]$TaggedMilestone,
        [string]$Tag,
        [string]$TagCommit,
        [string]$HeadCommit
    )

    $postCommit = Get-FirstParentSuccessorCommit $TagCommit $HeadCommit "tag '$Tag'"
    if ($null -eq $postCommit) {
        return
    }
    $postParents = @(
        ([string](& git -C $repositoryRoot rev-list --parents -n 1 $postCommit)).Trim() -split ' '
    )
    if ($postParents.Count -ne 2 -or $postParents[1] -cne $TagCommit) {
        Add-Failure "finalized post-operation snapshot $postCommit must be the direct single-parent child of tag '$Tag'."
    }

    $postCatalog = Get-CatalogAtRevision $postCommit "post-operation commit $postCommit"
    if ($null -eq $postCatalog) {
        return
    }
    $label = [string]$CurrentMilestone.releaseLabel
    $postMatches = @($postCatalog.milestones | Where-Object {
            [string]$_.releaseLabel -ceq $label
        })
    if ($postMatches.Count -ne 1) {
        Add-Failure "post-operation commit $postCommit does not contain exactly one '$label' milestone."
        return
    }
    $postMilestone = $postMatches[0]
    if ([string]$postMilestone.promotion.state -cne 'tagged') {
        Add-Failure "post-operation commit $postCommit does not record tagged promotion for '$label'."
    }
    Test-TaggedMilestoneProjection $postMilestone $TaggedMilestone $Tag

    $allowedPostEvidence = New-Object Collections.Generic.List[String]
    foreach ($path in @(Get-EvidencePaths $postMilestone)) {
        $allowedPostEvidence.Add($path)
    }
    if ([bool]$postMilestone.publication.feedPromoted) {
        $allowedPostEvidence.Add("release/feeds/$($postMilestone.channel)/release.json")
    }
    $actualPostEvidence = @(& git -C $repositoryRoot diff --name-only $TagCommit $postCommit | Sort-Object)
    $postDifference = @(
        Compare-Object @($allowedPostEvidence | Sort-Object) $actualPostEvidence -CaseSensitive
    )
    if ($postDifference.Count -gt 0) {
        Add-Failure "finalized post-operation commit $postCommit has an invalid evidence diff:`n$($postDifference | Out-String)"
    }

    Test-FinalizedMilestoneProjection $CurrentMilestone $postMilestone $postCommit

    $validationRelativePath = ([string]$postMilestone.validationRecord).Replace('\', '/')
    $probeExitCode = 0
    $postValidationBlob = ([string](Invoke-GitReadProbe `
            @('rev-parse', "${postCommit}:$validationRelativePath") `
            ([ref]$probeExitCode))).Trim()
    if ($probeExitCode -ne 0 -or $postValidationBlob -cnotmatch '^[0-9a-f]{40,64}$') {
        Add-Failure "post-operation commit $postCommit does not preserve '$validationRelativePath'."
        return
    }
    $currentValidationPath = Join-Path $repositoryRoot ($validationRelativePath.Replace('/', '\'))
    if (-not (Test-Path -LiteralPath $currentValidationPath -PathType Leaf)) {
        Add-Failure "finalized validation record is missing: $validationRelativePath"
        return
    }
    $currentValidationBlob = ([string](& git -C $repositoryRoot hash-object `
                "--path=$validationRelativePath" -- $currentValidationPath)).Trim()
    if ($LASTEXITCODE -ne 0 -or $currentValidationBlob -cne $postValidationBlob) {
        Add-Failure "validation record '$validationRelativePath' differs from finalized post-operation snapshot $postCommit."
    }

    Test-LatestPromotedFeedSnapshot $CurrentMilestone $postCommit
}

function Test-LatestPromotedFeedSnapshot {
    param(
        [object]$Milestone,
        [string]$PostCommit
    )

    if (-not [bool]$Milestone.publication.feedPromoted -or
        [string]$Milestone.publication.state -cne 'published' -or
        [string]$Milestone.postVerification.state -cne 'passed') {
        return
    }

    $label = [string]$Milestone.releaseLabel
    $channel = [string]$Milestone.channel
    $milestoneIndex = $labelOrder[$label]
    foreach ($candidate in $milestones) {
        $candidateLabel = [string]$candidate.releaseLabel
        if ($labelOrder[$candidateLabel] -gt $milestoneIndex -and
            [string]$candidate.channel -ceq $channel -and
            [bool]$candidate.publication.feedPromoted -and
            [string]$candidate.publication.state -ceq 'published' -and
            [string]$candidate.postVerification.state -ceq 'passed') {
            return
        }
    }

    $feedRelativePath = "release/feeds/$channel/release.json"
    $probeExitCode = 0
    $postFeedBlob = ([string](Invoke-GitReadProbe `
            @('rev-parse', "${PostCommit}:$feedRelativePath") `
            ([ref]$probeExitCode))).Trim()
    if ($probeExitCode -ne 0 -or $postFeedBlob -cnotmatch '^[0-9a-f]{40,64}$') {
        Add-Failure "successful post-operation commit $PostCommit does not preserve '$feedRelativePath'."
        return
    }

    $currentFeedPath = Join-Path $repositoryRoot ($feedRelativePath.Replace('/', '\'))
    if (-not (Test-Path -LiteralPath $currentFeedPath -PathType Leaf)) {
        Add-Failure "latest promoted $channel feed is missing: $feedRelativePath"
        return
    }
    $currentFeedBlob = ([string](& git -C $repositoryRoot hash-object `
                "--path=$feedRelativePath" -- $currentFeedPath)).Trim()
    if ($LASTEXITCODE -ne 0 -or $currentFeedBlob -cne $postFeedBlob) {
        Add-Failure "latest promoted $channel feed differs from finalized post-operation snapshot $PostCommit."
    }
}

function Get-AnnotatedTagIdentity {
    param(
        [string]$Name,
        [string]$Context
    )

    $probeExitCode = 0
    $tagType = [string](Invoke-GitReadProbe `
        @('cat-file', '-t', "refs/tags/$Name") `
        ([ref]$probeExitCode))
    if ($probeExitCode -ne 0 -or $tagType.Trim() -cne 'tag') {
        Add-Failure "$Context '$Name' must exist as an annotated tag."
        return $null
    }

    $tagObject = ([string](& git -C $repositoryRoot rev-parse "refs/tags/$Name")).Trim()
    $peeledCommit = ([string](& git -C $repositoryRoot rev-parse "refs/tags/$Name^{}")).Trim()
    if ($LASTEXITCODE -ne 0 -or
        $tagObject -cnotmatch '^[0-9a-f]{40}$' -or
        $peeledCommit -cnotmatch '^[0-9a-f]{40}$') {
        Add-Failure "$Context '$Name' does not resolve to full annotated-tag and peeled-commit identities."
        return $null
    }
    $peeledType = [string](Invoke-GitReadProbe `
        @('cat-file', '-t', $peeledCommit) `
        ([ref]$probeExitCode))
    if ($probeExitCode -ne 0 -or $peeledType.Trim() -cne 'commit') {
        Add-Failure "$Context '$Name' must peel to a Git commit."
        return $null
    }

    return [PSCustomObject]@{
        TagObject = $tagObject
        PeeledCommit = $peeledCommit
    }
}

function Test-OriginAnnotatedTagIdentity {
    param(
        [string]$Name,
        [string]$ExpectedTagObject,
        [string]$ExpectedCommit,
        [string]$Context
    )

    $probeExitCode = 0
    $remoteTags = @(Invoke-GitReadProbe `
        @('ls-remote', '--tags', 'origin', "refs/tags/$Name", "refs/tags/$Name^{}") `
        ([ref]$probeExitCode))
    if ($probeExitCode -ne 0) {
        Add-Failure "$Context could not inspect origin tag '$Name'."
        return
    }
    $tagObjects = New-Object Collections.Generic.List[String]
    $peeledCommits = New-Object Collections.Generic.List[String]
    foreach ($line in $remoteTags) {
        $fields = @(([string]$line).Trim() -split "\s+")
        if ($fields.Count -ne 2) {
            continue
        }
        if ($fields[1] -ceq "refs/tags/$Name") {
            $tagObjects.Add([string]$fields[0])
        }
        elseif ($fields[1] -ceq "refs/tags/$Name^{}") {
            $peeledCommits.Add([string]$fields[0])
        }
    }

    if ($tagObjects.Count -ne 1 -or
        $tagObjects[0] -cnotmatch '^[0-9a-f]{40}$' -or
        $peeledCommits.Count -ne 1 -or
        $peeledCommits[0] -cnotmatch '^[0-9a-f]{40}$') {
        Add-Failure "$Context requires annotated origin tag '$Name' with both tag-object and peeled-commit identities."
        return
    }
    if ($tagObjects[0] -cne $ExpectedTagObject) {
        Add-Failure "$Context origin tag '$Name' tag object differs from recorded identity $ExpectedTagObject."
    }
    if ($peeledCommits[0] -cne $ExpectedCommit) {
        Add-Failure "$Context requires annotated origin tag '$Name' peeled to $ExpectedCommit."
    }
}

function Test-PromotedUpdateFeed {
    param([object]$Milestone)

    $channel = [string]$Milestone.channel
    if ($channel -notin @('beta', 'stable')) {
        Add-Failure "promoted public feed has unsupported channel '$channel'."
        return
    }

    $feedRelativePath = "release/feeds/$channel/release.json"
    $feedPath = Join-Path $repositoryRoot ($feedRelativePath.Replace('/', '\'))
    $feedSchemaPath = Join-Path $repositoryRoot 'spec\update-feed\v1\release.schema.json'
    if (-not (Test-Path -LiteralPath $feedPath -PathType Leaf)) {
        Add-Failure "promoted update feed is missing: $feedRelativePath"
        return
    }
    if (-not (Test-Path -LiteralPath $feedSchemaPath -PathType Leaf)) {
        Add-Failure 'promoted update-feed schema is missing.'
        return
    }

    try {
        & (Join-Path $PSScriptRoot 'validate-json-document.ps1') `
            -SchemaPath $feedSchemaPath `
            -DocumentPath $feedPath `
            -MaximumBytes 32768 | Out-Null
        $feed = Get-Content -LiteralPath $feedPath -Raw | ConvertFrom-Json
    }
    catch {
        Add-Failure "promoted update feed is invalid: $($_.Exception.Message)"
        return
    }

    $expectedReleaseUrl = [string]$Milestone.publication.releaseUrl
    $expectedReleaseDate = $identity.ReleaseDate.ToString(
        'yyyy-MM-dd',
        [Globalization.CultureInfo]::InvariantCulture)
    $expectedIdentity = [ordered]@{
        product = 'Compact Cassette Catalogue'
        productId = 'c3'
        channel = $channel
        version = [string]$Milestone.productVersion
        stage = [string]$Milestone.stage
        informationalVersion = $identity.InformationalVersion
        releaseDate = $expectedReleaseDate
        catalogueWriteFormat = $identity.CatalogueFormatVersion
        releaseUrl = $expectedReleaseUrl
    }
    foreach ($propertyName in $expectedIdentity.Keys) {
        if ([string]$feed.$propertyName -cne [string]$expectedIdentity[$propertyName]) {
            Add-Failure "promoted update feed $propertyName differs from the qualified catalogue identity."
        }
    }
    if ($feed.published -isnot [bool] -or -not [bool]$feed.published) {
        Add-Failure 'promoted update feed must record published=true.'
    }

    $tag = [string]$Milestone.promotion.tag
    $assetBaseUrl = "https://github.com/Julesc013/compact-cassette-catalogue/releases/download/$tag/"
    $catalogManifest = $Milestone.checksumManifest
    $feedManifest = $feed.checksumManifest
    if ($null -eq $catalogManifest -or $null -eq $feedManifest) {
        Add-Failure 'promoted update feed and catalogue must both identify SHA256SUMS.txt.'
    }
    else {
        $expectedManifestUrl = $assetBaseUrl + [string]$catalogManifest.file
        if ([string]$feedManifest.file -cne [string]$catalogManifest.file -or
            [long]$feedManifest.length -ne [long]$catalogManifest.length -or
            [string]$feedManifest.sha256 -cne [string]$catalogManifest.sha256 -or
            [string]$feedManifest.url -cne $expectedManifestUrl) {
            Add-Failure 'promoted update-feed checksum manifest differs from the qualified catalogue artifact.'
        }
    }

    $catalogPackages = @($Milestone.packages)
    $feedPackages = @($feed.packages)
    if ($feedPackages.Count -ne $packageDefinitions.Count -or
        $catalogPackages.Count -ne $packageDefinitions.Count) {
        Add-Failure "promoted update feed must contain exactly $($packageDefinitions.Count) qualified portable packages."
    }
    foreach ($definition in $packageDefinitions) {
        $catalogMatches = @($catalogPackages | Where-Object {
                [string]$_.lane -ceq $definition.LaneId
            })
        $feedMatches = @($feedPackages | Where-Object {
                [string]$_.lane -ceq $definition.LaneId
            })
        if ($catalogMatches.Count -ne 1 -or $feedMatches.Count -ne 1) {
            Add-Failure "promoted update feed must project lane '$($definition.LaneId)' exactly once."
            continue
        }

        $catalogPackage = $catalogMatches[0]
        $feedPackage = $feedMatches[0]
        $expectedPackageUrl = $assetBaseUrl + [string]$catalogPackage.file
        if ([string]$catalogPackage.file -cne $definition.FileName -or
            [string]$feedPackage.distribution -cne 'portable' -or
            [string]$feedPackage.file -cne [string]$catalogPackage.file -or
            [long]$feedPackage.length -ne [long]$catalogPackage.length -or
            [string]$feedPackage.sha256 -cne [string]$catalogPackage.sha256 -or
            [string]$feedPackage.url -cne $expectedPackageUrl) {
            Add-Failure "promoted update-feed lane '$($definition.LaneId)' differs from the qualified portable artifact."
        }
    }
}

function Test-PostPromotionCommit {
    param(
        [object]$Milestone,
        [string]$PostCommit,
        [switch]$RequireRemoteAtTag
    )

    $recordedTag = [string]$Milestone.promotion.tag
    if ([string]$Milestone.qualification.state -cne 'pass' -or
        [string]$Milestone.promotion.state -cne 'tagged') {
        Add-Failure "post-promotion attestation requires qualification pass and tagged promotion facts."
        return
    }

    $tagIdentity = Get-AnnotatedTagIdentity $recordedTag 'post-promotion source tag'
    if ($null -eq $tagIdentity) {
        return
    }
    $tagCommit = [string]$tagIdentity.PeeledCommit
    if ([string]$Milestone.promotion.tagObject -cne [string]$tagIdentity.TagObject) {
        Add-Failure "post-promotion source tag '$recordedTag' differs from recorded promotion.tagObject."
    }
    if ($RequireRemoteAtTag) {
        Test-OriginAnnotatedTagIdentity `
            $recordedTag `
            ([string]$Milestone.promotion.tagObject) `
            $tagCommit `
            'post-promotion attestation'
    }
    Test-EvidenceAttestation $Milestone $tagCommit "tag '$recordedTag'"

    if ($RequireRemoteAtTag) {
        foreach ($remoteBranch in @($qualifiedBranch, $integrationBranch)) {
            $remoteReference = "refs/remotes/origin/$remoteBranch"
            & git -C $repositoryRoot show-ref --verify --quiet $remoteReference
            if ($LASTEXITCODE -ne 0) {
                    Add-Failure "post-promotion validation requires fetched origin/$remoteBranch."
            }
            else {
                $remoteCommit = ([string](& git -C $repositoryRoot rev-parse $remoteReference)).Trim()
                if ($remoteCommit -cne $tagCommit) {
                    Add-Failure "origin/$remoteBranch must still equal tagged attestation $tagCommit before post-promotion commit publication."
                }
            }
        }
    }

    $postLine = @(
        ([string](& git -C $repositoryRoot rev-list --parents -n 1 $PostCommit)).Trim() -split ' '
    )
    if ($postLine.Count -ne 2 -or $postLine[1] -cne $tagCommit) {
        Add-Failure "post-promotion attestation $PostCommit must be exactly one commit after tag attestation $tagCommit."
    }

    $allowedPostEvidence = New-Object Collections.Generic.List[String]
    foreach ($path in @(Get-EvidencePaths $Milestone)) {
        $allowedPostEvidence.Add($path)
    }
    if ([bool]$Milestone.publication.feedPromoted) {
        $allowedPostEvidence.Add("release/feeds/$($Milestone.channel)/release.json")
    }
    $allowedPostPaths = @($allowedPostEvidence | Sort-Object)
    $actualPostPaths = @(& git -C $repositoryRoot diff --name-only $tagCommit $PostCommit | Sort-Object)
    $postDifference = @(Compare-Object $allowedPostPaths $actualPostPaths -CaseSensitive)
    if ($postDifference.Count -gt 0) {
        Add-Failure "post-promotion diff is not the exact allowed evidence set:`n$($postDifference | Out-String)"
    }

    if ([string]$Milestone.publication.policy -ceq 'intentionally-unpublished') {
        if ([string]$Milestone.publication.state -cne 'unpublished' -or
            [bool]$Milestone.publication.feedPromoted -or
            [string]$Milestone.postVerification.state -cne 'not-applicable') {
            Add-Failure 'unpublished alpha post-promotion facts are incomplete or contradictory.'
        }
    }
    else {
        $publicSuccess = [string]$Milestone.publication.state -ceq 'published' -and
            [string]$Milestone.postVerification.state -ceq 'passed' -and
            [bool]$Milestone.publication.feedPromoted
        $publicFailure = [string]$Milestone.publication.state -ceq 'published' -and
            [string]$Milestone.postVerification.state -ceq 'failed' -and
            -not [bool]$Milestone.publication.feedPromoted
        if (-not $publicSuccess -and -not $publicFailure) {
            Add-Failure 'public post-promotion facts must record either verified feed promotion or an honest post-verification failure.'
        }
        elseif ($publicSuccess) {
            Test-PromotedUpdateFeed $Milestone
        }
    }
}

if (-not (Test-Path -LiteralPath $catalogPath -PathType Leaf)) {
    throw "Release catalogue is missing: $catalogPath"
}
if (-not (Test-Path -LiteralPath $schemaPath -PathType Leaf)) {
    throw "Release catalogue schema is missing: $schemaPath"
}

& (Join-Path $PSScriptRoot 'validate-json-document.ps1') `
    -SchemaPath $schemaPath `
    -DocumentPath $catalogPath `
    -MaximumBytes (4 * 1024 * 1024)

$catalog = Get-Content -LiteralPath $catalogPath -Raw | ConvertFrom-Json
if (-not (Test-RequiredProperties $catalog @('$schema', 'schemaVersion', 'productId', 'milestones') 'release catalogue')) {
    throw ("Release-contract validation failed:`n - " + ($failures -join "`n - "))
}
if (-not ($catalog.'$schema' -is [string]) -or -not ($catalog.productId -is [string])) {
    Add-Failure 'release catalogue schema and productId values must be strings.'
}
if (-not ($catalog.schemaVersion -is [int]) -and
    -not ($catalog.schemaVersion -is [long])) {
    Add-Failure 'release catalogue schemaVersion must be an integer.'
}
if (-not ($catalog.milestones -is [Array])) {
    Add-Failure 'release catalogue milestones must be an array.'
}
if ([string]$catalog.'$schema' -cne '../spec/release-catalog/v1/catalog.schema.json') {
    Add-Failure "release catalogue references an unexpected schema: '$($catalog.'$schema')'"
}
if ([string]$catalog.schemaVersion -cne '1') {
    Add-Failure "release catalogue schemaVersion must be 1."
}
if ([string]$catalog.productId -cne 'c3') {
    Add-Failure "release catalogue productId must be 'c3'."
}

$milestones = @($catalog.milestones)
if ($milestones.Count -eq 0) {
    Add-Failure 'release catalogue must contain at least one milestone.'
}

$labels = @{}
$labelOrder = @{}
$tags = @{}
$validationPaths = @{}
$previousLabel = $null
$previousReleaseOrder = $null
$currentMatches = New-Object Collections.Generic.List[Object]

foreach ($milestone in $milestones) {
    $required = @(
        'releaseLabel', 'productVersion', 'stage', 'channel', 'predecessor',
        'qualification', 'supersededBy', 'promotion', 'publication',
        'postVerification', 'validationRecord', 'packages', 'checksumManifest'
    )
    if (-not (Test-RequiredProperties $milestone $required 'milestone')) {
        continue
    }

    foreach ($stringProperty in @('releaseLabel', 'productVersion', 'stage', 'channel', 'validationRecord')) {
        if (-not ($milestone.$stringProperty -is [string])) {
            Add-Failure "milestone property '$stringProperty' must be a string."
        }
    }
    if ($null -ne $milestone.predecessor -and -not ($milestone.predecessor -is [string])) {
        Add-Failure 'milestone predecessor must be a string or null.'
    }
    if ($null -ne $milestone.supersededBy -and -not ($milestone.supersededBy -is [string])) {
        Add-Failure 'milestone supersededBy must be a string or null.'
    }
    if (-not ($milestone.packages -is [Array])) {
        Add-Failure 'milestone packages must be an array.'
    }
    if ($null -ne $milestone.checksumManifest -and
        $milestone.checksumManifest -isnot [PSCustomObject]) {
        Add-Failure 'milestone checksumManifest must be an object or null.'
    }

    $label = [string]$milestone.releaseLabel
    $context = "milestone '$label'"
    if ($label -cnotmatch '^[0-9]+\.[0-9]+\.[0-9]+(?:-[0-9a-z.]+)?$') {
        Add-Failure "$context has an invalid release label."
    }
    if ($labels.ContainsKey($label)) {
        Add-Failure "duplicate release label '$label'."
    }
    else {
        $labels[$label] = $milestone
        $labelOrder[$label] = $labelOrder.Count
    }

    $predecessor = $milestone.predecessor
    if ($null -eq $previousLabel) {
        if ($null -ne $predecessor) {
            Add-Failure "$context must have a null predecessor because it is first."
        }
    }
    elseif ([string]$predecessor -cne $previousLabel) {
        Add-Failure "$context predecessor must be '$previousLabel'."
    }
    $previousLabel = $label

    $milestoneProductVersion = [string]$milestone.productVersion
    $milestoneStage = [string]$milestone.stage
    $milestoneChannel = [string]$milestone.channel
    $releaseOrder = Get-ReleaseOrder $milestoneProductVersion $milestoneStage
    if ($null -ne $releaseOrder) {
        if ($null -ne $previousReleaseOrder -and
            (Compare-ReleaseOrder $releaseOrder $previousReleaseOrder) -le 0) {
            Add-Failure "$context must be strictly later than predecessor '$predecessor' in product/stage order."
        }
        $previousReleaseOrder = $releaseOrder
    }
    Test-AllowedValue $milestoneChannel @('alpha', 'beta', 'stable') "$context channel" | Out-Null

    $expectedLabel = $null
    $expectedChannel = $null
    try {
        $resolvedLabel = & (Join-Path $PSScriptRoot 'resolve-release-label.ps1') `
            -ProductVersion $milestoneProductVersion `
            -ReleaseStage $milestoneStage
        $expectedLabel = $resolvedLabel.ReleaseLabel
        $expectedChannel = $resolvedLabel.ReleaseChannel
    }
    catch {
        Add-Failure "$context has invalid product/stage identity: $($_.Exception.Message)"
    }
    if ($null -ne $expectedLabel -and $label -cne $expectedLabel) {
        Add-Failure "$context releaseLabel must be '$expectedLabel'."
    }
    if ($null -ne $expectedChannel -and $milestoneChannel -cne $expectedChannel) {
        Add-Failure "$context channel must be '$expectedChannel' for stage '$milestoneStage'."
    }

    if (-not (Test-RequiredProperties $milestone.qualification @('state', 'sourceCommit') "$context qualification")) {
        continue
    }
    if (-not (Test-RequiredProperties $milestone.promotion @('state', 'targetBranch', 'tag', 'tagObject') "$context promotion")) {
        continue
    }
    if (-not (Test-RequiredProperties $milestone.publication @('policy', 'state', 'releaseUrl', 'feedPromoted') "$context publication")) {
        continue
    }
    if (-not (Test-RequiredProperties $milestone.postVerification @('state') "$context postVerification")) {
        continue
    }

    $requiredStringValues = [ordered]@{
        'qualification state' = $milestone.qualification.state
        'promotion state' = $milestone.promotion.state
        'promotion targetBranch' = $milestone.promotion.targetBranch
        'promotion tag' = $milestone.promotion.tag
        'publication policy' = $milestone.publication.policy
        'publication state' = $milestone.publication.state
        'postVerification state' = $milestone.postVerification.state
    }
    foreach ($propertyName in $requiredStringValues.Keys) {
        if (-not ($requiredStringValues[$propertyName] -is [string])) {
            Add-Failure "$context $propertyName must be a string."
        }
    }
    if ($null -ne $milestone.qualification.sourceCommit -and
        -not ($milestone.qualification.sourceCommit -is [string])) {
        Add-Failure "$context qualification sourceCommit must be a string or null."
    }
    if ($null -ne $milestone.promotion.tagObject -and
        -not ($milestone.promotion.tagObject -is [string])) {
        Add-Failure "$context promotion tagObject must be a string or null."
    }
    if ($null -ne $milestone.publication.releaseUrl -and
        -not ($milestone.publication.releaseUrl -is [string])) {
        Add-Failure "$context publication releaseUrl must be a string or null."
    }
    if (-not ($milestone.publication.feedPromoted -is [bool])) {
        Add-Failure "$context publication feedPromoted must be Boolean."
    }

    $qualificationState = [string]$milestone.qualification.state
    $promotionState = [string]$milestone.promotion.state
    $publicationPolicy = [string]$milestone.publication.policy
    $publicationState = [string]$milestone.publication.state
    $postState = [string]$milestone.postVerification.state
    Test-AllowedValue $qualificationState @('planned', 'active', 'blocked', 'pass', 'fail') "$context qualification state" | Out-Null
    Test-AllowedValue $promotionState @('unpromoted', 'tagged') "$context promotion state" | Out-Null
    Test-AllowedValue $publicationPolicy @('intentionally-unpublished', 'public-prerelease', 'public-stable') "$context publication policy" | Out-Null
    Test-AllowedValue $publicationState @('unpublished', 'published') "$context publication state" | Out-Null
    Test-AllowedValue $postState @('not-applicable', 'pending', 'passed', 'failed') "$context post-verification state" | Out-Null

    $tag = [string]$milestone.promotion.tag
    $expectedTag = [string](& $tagResolver -ReleaseLabel $label)
    $historicalTag = if ($label -cmatch '^2\.0\.0-alpha\.[1-4]$') {
        'v' + $label
    }
    else {
        $null
    }
    if ($tag -cne $expectedTag -and $tag -cne $historicalTag) {
        Add-Failure "$context tag must be '$expectedTag'."
    }
    if ($tags.ContainsKey($tag)) {
        Add-Failure "duplicate milestone tag '$tag'."
    }
    else {
        $tags[$tag] = $milestone
    }
    if ([string]$milestone.promotion.targetBranch -cne $qualifiedBranch) {
        Add-Failure "$context promotion target must be $qualifiedBranch."
    }
    $tagObject = $milestone.promotion.tagObject
    if ($promotionState -ceq 'unpromoted' -and $null -ne $tagObject) {
        Add-Failure "$context unpromoted state requires a null promotion tagObject."
    }
    elseif ($promotionState -ceq 'tagged' -and
        ($null -eq $tagObject -or [string]$tagObject -cnotmatch '^[0-9a-f]{40}$')) {
        Add-Failure "$context tagged state requires a lowercase full annotated-tag object ID."
    }

    $sourceCommit = $milestone.qualification.sourceCommit
    $sourceCommitExists = $false
    if ($null -ne $sourceCommit -and [string]$sourceCommit -cnotmatch '^[0-9a-f]{40}$') {
        Add-Failure "$context sourceCommit must be null or a lowercase full Git SHA."
    }
    elseif ($null -ne $sourceCommit -and $gitMetadataAvailable) {
        $probeExitCode = 0
        [void](Invoke-GitReadProbe `
                @('cat-file', '-e', "$sourceCommit`^{commit}") `
                ([ref]$probeExitCode))
        if ($probeExitCode -ne 0) {
            Add-Failure "$context sourceCommit $sourceCommit is not available as a Git commit."
        }
        else {
            $sourceCommitExists = $true
        }
    }

    $packages = @($milestone.packages)
    $manifestRecord = $milestone.checksumManifest
    if ($packages.Count -eq 0) {
        if ($null -ne $manifestRecord) {
            Add-Failure "$context has a checksum manifest without package records."
        }
        if ($qualificationState -ceq 'pass') {
            Add-Failure "$context cannot pass without exact package evidence."
        }
    }
    else {
        if ($null -eq $sourceCommit) {
            Add-Failure "$context has package evidence without a sourceCommit."
        }
        $packageLanes = @{}
        $packageFiles = @{}
        foreach ($package in $packages) {
            if (-not (Test-RequiredProperties $package @('lane', 'file', 'length', 'sha256') "$context package")) {
                continue
            }
            $packageLane = [string]$package.lane
            $packageFile = [string]$package.file
            if (-not ($package.lane -is [string]) -or
                -not ($package.file -is [string]) -or
                -not ($package.sha256 -is [string])) {
                Add-Failure "$context package lane, file, and sha256 must be strings."
            }
            $packageLengthIsInteger = ($package.length -is [int]) -or
                ($package.length -is [long])
            if (-not $packageLengthIsInteger) {
                Add-Failure "$context package '$packageFile' length must be an integer."
            }
            if ($packageLane -cnotmatch '^[a-z0-9][a-z0-9.-]*$') {
                Add-Failure "$context package has invalid lane '$packageLane'."
            }
            if ($packageLanes.ContainsKey($packageLane)) {
                Add-Failure "$context records lane '$packageLane' more than once."
            }
            else {
                $packageLanes[$packageLane] = $package
            }
            if ($packageFiles.ContainsKey($packageFile)) {
                Add-Failure "$context records package file '$packageFile' more than once."
            }
            else {
                $packageFiles[$packageFile] = $package
            }
            $expectedPackageFile = "C3-v$label-$packageLane-portable.zip"
            if ($packageFile -cne $expectedPackageFile) {
                Add-Failure "$context lane '$packageLane' must use '$expectedPackageFile'."
            }
            if ($packageLengthIsInteger -and [long]$package.length -le 0) {
                Add-Failure "$context package '$packageFile' has an invalid length."
            }
            if ([string]$package.sha256 -cnotmatch '^[0-9a-f]{64}$') {
                Add-Failure "$context package '$packageFile' has an invalid SHA-256."
            }
        }
        if ($label -ceq $identity.ReleaseLabel) {
            if ($packages.Count -ne $packageDefinitions.Count) {
                Add-Failure "$context must record $($packageDefinitions.Count) current lane packages."
            }
            foreach ($definition in $packageDefinitions) {
                if (-not $packageLanes.ContainsKey($definition.LaneId)) {
                    Add-Failure "$context must record current lane '$($definition.LaneId)' exactly once."
                }
            }
        }
        if ($sourceCommitExists) {
            $probeExitCode = 0
            $historicalLanesText = (@(Invoke-GitReadProbe `
                    @('show', "${sourceCommit}:build/lanes.json") `
                    ([ref]$probeExitCode)) -join "`n")
            if ($probeExitCode -ne 0 -or [string]::IsNullOrWhiteSpace($historicalLanesText)) {
                Add-Failure "$context sourceCommit does not contain build/lanes.json."
            }
            else {
                try {
                    $historicalManifest = $historicalLanesText | ConvertFrom-Json
                    $historicalLaneIds = @($historicalManifest.lanes | Where-Object {
                            [string]$_.distribution -ceq 'portable'
                        } | ForEach-Object { [string]$_.id } | Sort-Object)
                    $recordedLaneIds = @($packages | ForEach-Object { [string]$_.lane } | Sort-Object)
                    $historicalDifference = @(Compare-Object $historicalLaneIds $recordedLaneIds -CaseSensitive)
                    if ($historicalDifference.Count -gt 0) {
                        Add-Failure "$context package lanes differ from sourceCommit build/lanes.json:`n$($historicalDifference | Out-String)"
                    }
                }
                catch {
                    Add-Failure "$context sourceCommit build/lanes.json is unreadable: $($_.Exception.Message)"
                }
            }
        }
        if (-not (Test-RequiredProperties $manifestRecord @('file', 'length', 'sha256') "$context checksum manifest")) {
            continue
        }
        if (-not ($manifestRecord.file -is [string]) -or
            -not ($manifestRecord.sha256 -is [string])) {
            Add-Failure "$context checksum manifest file and sha256 must be strings."
        }
        $manifestLengthIsInteger = ($manifestRecord.length -is [int]) -or
            ($manifestRecord.length -is [long])
        if (-not $manifestLengthIsInteger) {
            Add-Failure "$context checksum manifest length must be an integer."
        }
        if ([string]$manifestRecord.file -cne 'SHA256SUMS.txt') {
            Add-Failure "$context checksum manifest must be SHA256SUMS.txt."
        }
        if (($manifestLengthIsInteger -and [long]$manifestRecord.length -le 0) -or
            [string]$manifestRecord.sha256 -cnotmatch '^[0-9a-f]{64}$') {
            Add-Failure "$context checksum manifest identity is invalid."
        }
    }
    if ($qualificationState -ceq 'fail' -and $null -eq $sourceCommit) {
        Add-Failure "$context qualification failure must identify the exact failing sourceCommit."
    }

    if ($promotionState -cne 'unpromoted' -and $qualificationState -cne 'pass') {
        Add-Failure "$context cannot be promoted without qualification pass."
    }
    if ($publicationPolicy -ceq 'public-prerelease' -and $milestoneChannel -cne 'beta') {
        Add-Failure "$context public-prerelease policy requires the beta channel."
    }
    if ($publicationPolicy -ceq 'public-stable' -and $milestoneChannel -cne 'stable') {
        Add-Failure "$context public-stable policy requires the stable channel."
    }
    if ($milestoneChannel -ceq 'beta' -and $publicationPolicy -cne 'public-prerelease') {
        Add-Failure "$context beta channel requires public-prerelease policy."
    }
    if ($milestoneChannel -ceq 'stable' -and $publicationPolicy -cne 'public-stable') {
        Add-Failure "$context stable channel requires public-stable policy."
    }
    if ($publicationPolicy -ceq 'intentionally-unpublished') {
        if ($publicationState -cne 'unpublished' -or
            [bool]$milestone.publication.feedPromoted -or
            $null -ne $milestone.publication.releaseUrl -or
            $postState -cne 'not-applicable') {
            Add-Failure "$context intentionally-unpublished policy conflicts with publication facts."
        }
    }
    if ($publicationState -ceq 'published') {
        if ($qualificationState -cne 'pass' -or $promotionState -cne 'tagged') {
            Add-Failure "$context cannot be published before qualification and tag promotion."
        }
        $releaseUri = $null
        if (-not [Uri]::TryCreate([string]$milestone.publication.releaseUrl, [UriKind]::Absolute, [ref]$releaseUri) -or
            $releaseUri.Scheme -cne 'https') {
            Add-Failure "$context published releaseUrl must be absolute HTTPS."
        }
        else {
            $expectedReleaseUrl = "https://github.com/Julesc013/compact-cassette-catalogue/releases/tag/$tag"
            if ($releaseUri.AbsoluteUri.TrimEnd('/') -cne $expectedReleaseUrl) {
                Add-Failure "$context releaseUrl must be '$expectedReleaseUrl'."
            }
        }
        if ($postState -ceq 'not-applicable') {
            Add-Failure "$context published assets require pending/passed/failed post-verification."
        }
    }
    elseif ($null -ne $milestone.publication.releaseUrl) {
        Add-Failure "$context unpublished release must have a null releaseUrl."
    }
    if ($publicationPolicy -cne 'intentionally-unpublished' -and
        $publicationState -ceq 'unpublished' -and
        ($postState -cne 'not-applicable' -or [bool]$milestone.publication.feedPromoted)) {
        Add-Failure "$context unpublished public candidate must have not-applicable post-verification and no promoted feed."
    }
    if ([bool]$milestone.publication.feedPromoted -and
        ($publicationState -cne 'published' -or $postState -cne 'passed')) {
        Add-Failure "$context feed promotion requires published, post-verified assets."
    }
    if ([string]$milestone.channel -ceq 'alpha' -and
        $publicationPolicy -cne 'intentionally-unpublished') {
        Add-Failure "$context alpha channel must remain intentionally unpublished."
    }

    $expectedValidationRecord = "release/validation/$label.md"
    if ([string]$milestone.validationRecord -cne $expectedValidationRecord) {
        Add-Failure "$context validationRecord must be '$expectedValidationRecord'."
    }
    $validationRelative = ([string]$milestone.validationRecord).Replace('/', '\')
    $validationFull = [IO.Path]::GetFullPath((Join-Path $repositoryRoot $validationRelative))
    $validationPrefix = $validationRoot + [IO.Path]::DirectorySeparatorChar
    if (-not $validationFull.StartsWith($validationPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        Add-Failure "$context validationRecord escapes release/validation."
    }
    elseif (-not (Test-Path -LiteralPath $validationFull -PathType Leaf)) {
        Add-Failure "$context validationRecord is missing: $validationRelative"
    }
    else {
        if ($validationPaths.ContainsKey($validationFull)) {
            Add-Failure "$context reuses validationRecord '$validationRelative'."
        }
        else {
            $validationPaths[$validationFull] = $label
        }
        $validationText = Get-Content -LiteralPath $validationFull -Raw
        $validationLines = @(Get-Content -LiteralPath $validationFull)
        $headers = @(
            [PSCustomObject]@{ Name = 'Qualification'; Value = $qualificationState }
            [PSCustomObject]@{ Name = 'Promotion'; Value = $promotionState }
            [PSCustomObject]@{ Name = 'Publication policy'; Value = $publicationPolicy }
            [PSCustomObject]@{ Name = 'Publication state'; Value = $publicationState }
            [PSCustomObject]@{ Name = 'Post-verification'; Value = $postState }
        )
        foreach ($header in $headers) {
            $marker = "$($header.Name): **$($header.Value)**"
            $headerPrefix = $header.Name + ':'
            $headerLines = @($validationLines | Where-Object {
                    $_.StartsWith($headerPrefix, [StringComparison]::Ordinal)
                })
            if ($headerLines.Count -ne 1 -or $headerLines[0] -cne $marker) {
                Add-Failure "$context validation record must contain exactly one header '$marker'."
            }
        }
        if ($null -ne $sourceCommit -and -not $validationText.Contains([string]$sourceCommit)) {
            Add-Failure "$context validation record does not name its sourceCommit."
        }
        foreach ($package in $packages) {
            if (-not $validationText.Contains([string]$package.sha256)) {
                Add-Failure "$context validation record omits package hash '$($package.sha256)'."
            }
        }
        if ($null -ne $manifestRecord -and
            -not $validationText.Contains([string]$manifestRecord.sha256)) {
            Add-Failure "$context validation record omits checksum-manifest hash '$($manifestRecord.sha256)'."
        }
    }

    if ($label -ceq $identity.ReleaseLabel) {
        $currentMatches.Add($milestone)
        if ([string]$milestone.productVersion -cne $identity.ProductVersion -or
            [string]$milestone.stage -cne $identity.ReleaseStage -or
            [string]$milestone.channel -cne $identity.ReleaseChannel -or
            ($tag -cne $identity.TagName -and $tag -cne $historicalTag)) {
            Add-Failure "$context does not match build/Version.props."
        }
    }
}

if ($failures.Count -gt 0) {
    throw ("Release-contract validation failed:`n - " + ($failures -join "`n - "))
}

foreach ($milestone in $milestones) {
    if ($null -ne $milestone.supersededBy) {
        $supersededBy = [string]$milestone.supersededBy
        if (-not $labels.ContainsKey($supersededBy)) {
            Add-Failure "milestone '$($milestone.releaseLabel)' supersededBy target does not exist."
        }
        elseif ($labelOrder[$supersededBy] -le $labelOrder[[string]$milestone.releaseLabel]) {
            Add-Failure "milestone '$($milestone.releaseLabel)' supersededBy must name a later milestone."
        }
    }
}

for ($index = 0; $index -lt ($milestones.Count - 1); $index++) {
    $completed = $milestones[$index]
    $successor = $milestones[$index + 1]
    $completedContext = "milestone '$($completed.releaseLabel)'"
    $normalClosure = [string]$completed.qualification.state -ceq 'pass' -and
        [string]$completed.promotion.state -ceq 'tagged'
    if ($normalClosure -and
        [string]$completed.publication.policy -ceq 'intentionally-unpublished') {
        $normalClosure = [string]$completed.publication.state -ceq 'unpublished' -and
            [string]$completed.postVerification.state -ceq 'not-applicable'
    }
    elseif ($normalClosure) {
        $normalClosure = [string]$completed.publication.state -ceq 'published' -and
            [string]$completed.postVerification.state -ceq 'passed' -and
            [bool]$completed.publication.feedPromoted
    }

    $supersededClosure = [string]$completed.supersededBy -ceq [string]$successor.releaseLabel
    if ($supersededClosure) {
        if ([string]$completed.qualification.state -ceq 'fail') {
            $supersededClosure = [string]$completed.promotion.state -ceq 'unpromoted' -and
                [string]$completed.publication.state -ceq 'unpublished' -and
                -not [bool]$completed.publication.feedPromoted
        }
        elseif ([string]$completed.qualification.state -ceq 'pass') {
            $supersededClosure = [string]$completed.promotion.state -ceq 'tagged' -and
                [string]$completed.publication.state -ceq 'published' -and
                [string]$completed.postVerification.state -ceq 'failed' -and
                -not [bool]$completed.publication.feedPromoted
        }
        else {
            $supersededClosure = $false
        }
    }

    if ($null -ne $completed.supersededBy) {
        if (-not $supersededClosure) {
            Add-Failure "$completedContext may be superseded only after an honest qualification or post-verification failure, and only by '$($successor.releaseLabel)'."
        }
    }
    elseif (-not $normalClosure) {
        Add-Failure "$completedContext must close successfully before '$($successor.releaseLabel)' is recorded."
    }
}

if ($currentMatches.Count -ne 1) {
    Add-Failure "exactly one catalogue milestone must match current release '$($identity.ReleaseLabel)'."
}
elseif ([string]$currentMatches[0].releaseLabel -cne
    [string]$milestones[$milestones.Count - 1].releaseLabel) {
    Add-Failure "current release '$($identity.ReleaseLabel)' must be the final catalogue milestone."
}

if ($Mode -in @('Candidate', 'Tag', 'Master', 'PostPromotion') -or
    $RequireRemoteBaseline) {
    Update-RemoteReleaseReferences | Out-Null
}

if ($gitMetadataAvailable) {
    $headCommit = ([string](& git -C $repositoryRoot rev-parse HEAD)).Trim()
    $masterReference = Get-MasterReference
    if ($Mode -ceq 'Repository' -and $RequireRemoteBaseline) {
        if ($null -eq $masterReference) {
            if ($RequireRemoteBaseline) {
                Add-Failure 'repository validation requires a fetched master baseline.'
            }
        }
        else {
            $masterCommit = ([string](& git -C $repositoryRoot rev-parse $masterReference)).Trim()
            $masterCatalog = Get-CatalogAtRevision `
                $masterCommit `
                "$masterReference commit $masterCommit" `
                -AllowMissing
            if ($null -ne $masterCatalog) {
                Test-HistoricalMilestonesAgainstBaseline `
                    $masterCatalog `
                    "$masterReference commit $masterCommit" `
                    $identity.ReleaseLabel
            }
        }
    }
    foreach ($milestone in $milestones) {
        if ([string]$milestone.promotion.state -cne 'tagged') {
            continue
        }
        $recordedTag = [string]$milestone.promotion.tag
        $recordedTagObject = [string]$milestone.promotion.tagObject
        $tagIdentity = Get-AnnotatedTagIdentity `
            $recordedTag `
            'catalogue promotion tag'
        if ($null -eq $tagIdentity) {
            continue
        }
        if ([string]$tagIdentity.TagObject -cne $recordedTagObject) {
            Add-Failure "catalogue promotion tag '$recordedTag' differs from recorded promotion.tagObject $recordedTagObject."
        }
        $recordedTagCommit = [string]$tagIdentity.PeeledCommit
        if ($Mode -in @('Candidate', 'Tag', 'Master', 'PostPromotion') -or
            $RequireRemoteBaseline) {
            Test-OriginAnnotatedTagIdentity `
                $recordedTag `
                $recordedTagObject `
                $recordedTagCommit `
                'release transaction'
        }
        & git -C $repositoryRoot merge-base --is-ancestor $recordedTagCommit $headCommit
        if ($LASTEXITCODE -ne 0) {
            Add-Failure "tag '$recordedTag' is not an ancestor of current HEAD."
        }
        if ($null -ne $masterReference) {
            & git -C $repositoryRoot merge-base --is-ancestor $recordedTagCommit $masterReference
            if ($LASTEXITCODE -ne 0) {
                Add-Failure "tag '$recordedTag' is not reachable from $masterReference."
            }
        }
        $tagCatalog = Get-CatalogAtRevision $recordedTag "tag '$recordedTag'"
        if ($null -eq $tagCatalog) {
            continue
        }

        $tagRecords = @($tagCatalog.milestones | Where-Object {
                [string]$_.releaseLabel -ceq [string]$milestone.releaseLabel
            })
        if ($tagRecords.Count -ne 1 -or
            [string]$tagRecords[0].qualification.state -cne 'pass' -or
            [string]$tagRecords[0].qualification.sourceCommit -cne [string]$milestone.qualification.sourceCommit) {
            Add-Failure "tag '$recordedTag' does not preserve the matching qualification attestation."
            continue
        }
        Test-TaggedMilestoneProjection $milestone $tagRecords[0] $recordedTag
        Test-FinalizedMilestoneSnapshot `
            $milestone `
            $tagRecords[0] `
            $recordedTag `
            $recordedTagCommit `
            $headCommit

        foreach ($snapshotMilestone in @($tagCatalog.milestones)) {
            $snapshotLabel = [string]$snapshotMilestone.releaseLabel
            if ($snapshotLabel -ceq [string]$milestone.releaseLabel) {
                continue
            }
            if (-not $labels.ContainsKey($snapshotLabel)) {
                Add-Failure "tag '$recordedTag' historical milestone '$snapshotLabel' is missing from the current catalogue."
                continue
            }
            if ((Get-CanonicalJsonText $labels[$snapshotLabel]) -cne
                (Get-CanonicalJsonText $snapshotMilestone)) {
                Add-Failure "historical milestone '$snapshotLabel' differs from immutable tag snapshot '$recordedTag'."
            }
        }
    }
}

if ($Mode -ceq 'Candidate') {
    if (-not $gitMetadataAvailable) {
        Add-Failure 'candidate validation requires full Git metadata.'
    }
    elseif ($currentMatches.Count -eq 1) {
        $current = $currentMatches[0]
        $headCommit = ([string](& git -C $repositoryRoot rev-parse HEAD)).Trim()
        if ($ExpectedCommit -cnotmatch '^[0-9a-f]{40}$' -or $ExpectedCommit -cne $headCommit) {
            Add-Failure "candidate validation requires -ExpectedCommit equal to HEAD $headCommit."
        }
        if ([string]$current.qualification.state -cne 'pass' -or
            [string]$current.promotion.state -cne 'unpromoted') {
            Add-Failure 'candidate E must record qualification pass and unpromoted state.'
        }
        Test-EvidenceAttestation $current $headCommit 'candidate attestation'

        & git -C $repositoryRoot show-ref --verify --quiet $integrationRemoteReference
        if ($LASTEXITCODE -ne 0) {
            Add-Failure "candidate validation requires fetched origin/$integrationBranch."
        }
        else {
            $remoteDevCommit = ([string](& git -C $repositoryRoot rev-parse $integrationRemoteReference)).Trim()
            $sourceCommit = [string]$current.qualification.sourceCommit
            if ($remoteDevCommit -cne $sourceCommit) {
                Add-Failure "origin/$integrationBranch must remain at frozen source C $sourceCommit while candidate E $headCommit is qualified."
            }
        }

        $masterReference = Get-MasterReference
        if ($null -eq $masterReference) {
            Add-Failure 'candidate validation requires a fetched master reference.'
        }
        else {
            $masterCommit = ([string](& git -C $repositoryRoot rev-parse $masterReference)).Trim()
            if ($masterCommit -ceq $headCommit) {
                Add-Failure "candidate E $headCommit is already on $masterReference; qualification must precede promotion."
            }
            else {
                & git -C $repositoryRoot merge-base --is-ancestor $masterCommit $headCommit
                if ($LASTEXITCODE -ne 0) {
                    Add-Failure "candidate E $headCommit cannot fast-forward $masterReference."
                }
            }

            $masterCatalog = Get-CatalogAtRevision `
                $masterCommit `
                "$masterReference commit $masterCommit" `
                -AllowMissing
            if ($null -ne $masterCatalog) {
                Test-HistoricalMilestonesAgainstBaseline `
                    $masterCatalog `
                    "$masterReference commit $masterCommit" `
                    ([string]$current.releaseLabel)
            }
        }

        & git -C $repositoryRoot show-ref --verify --quiet ("refs/tags/" + [string]$current.promotion.tag)
        if ($LASTEXITCODE -eq 0) {
            Add-Failure "candidate tag '$($current.promotion.tag)' already exists; immutable tag creation is no longer a pre-operation step."
        }
        if (-not $RequireArtifacts) {
            Add-Failure 'candidate validation requires -RequireArtifacts against the recorded package bytes.'
        }
    }
}

if ($Mode -ceq 'Tag') {
    if ([string]::IsNullOrWhiteSpace($TagName)) {
        $TagName = [string]$env:GITHUB_REF_NAME
    }
    if ([string]::IsNullOrWhiteSpace($TagName) -or -not $tags.ContainsKey($TagName)) {
        Add-Failure "tag '$TagName' has no matching release-catalogue milestone."
    }
    else {
        $tagMilestone = $tags[$TagName]
        $tagIdentity = Get-AnnotatedTagIdentity $TagName 'tag'
        if ($null -ne $tagIdentity) {
            $tagCommit = [string]$tagIdentity.PeeledCommit
            $headCommit = ([string](& git -C $repositoryRoot rev-parse HEAD)).Trim()
            if ($tagCommit -cne $headCommit) {
                Add-Failure "tag validation must run with HEAD at tag commit $tagCommit."
            }
            if ($TagName -cne $identity.TagName) {
                Add-Failure "tag '$TagName' does not match Version.props tag '$($identity.TagName)'."
            }
            if ([string]$tagMilestone.qualification.state -cne 'pass') {
                Add-Failure "tag '$TagName' does not identify a qualification pass."
            }
            if ([string]$tagMilestone.promotion.state -cne 'unpromoted') {
                Add-Failure "tag '$TagName' must point to the pre-operation E attestation."
            }
            if ($null -ne $tagMilestone.promotion.tagObject) {
                Add-Failure "tag '$TagName' pre-operation E attestation must have a null promotion.tagObject."
            }
            Test-OriginAnnotatedTagIdentity `
                $TagName `
                ([string]$tagIdentity.TagObject) `
                $tagCommit `
                'tag validation'
            Test-EvidenceAttestation $tagMilestone $tagCommit "tag '$TagName'"

            $masterReference = Get-MasterReference
            if ($null -eq $masterReference) {
                Add-Failure 'tag validation requires a fetched master reference.'
            }
            else {
                & git -C $repositoryRoot merge-base --is-ancestor $tagCommit $masterReference
                if ($LASTEXITCODE -ne 0) {
                    Add-Failure "tag '$TagName' is not reachable from $masterReference."
                }
            }
        }
    }
}

if ($Mode -ceq 'PostPromotion') {
    if (-not $gitMetadataAvailable) {
        Add-Failure 'post-promotion validation requires full Git metadata.'
    }
    elseif ($currentMatches.Count -eq 1) {
        $headCommit = ([string](& git -C $repositoryRoot rev-parse HEAD)).Trim()
        if ($ExpectedCommit -cnotmatch '^[0-9a-f]{40}$' -or $ExpectedCommit -cne $headCommit) {
            Add-Failure "post-promotion validation requires -ExpectedCommit equal to HEAD $headCommit."
        }
        Test-PostPromotionCommit $currentMatches[0] $headCommit -RequireRemoteAtTag
        if (-not $RequireArtifacts) {
            Add-Failure 'post-promotion validation requires -RequireArtifacts against the recorded package bytes.'
        }
    }
}

if ($Mode -ceq 'Master') {
    if (-not $gitMetadataAvailable) {
        Add-Failure 'master-ledger validation requires full Git metadata.'
    }
    elseif ($currentMatches.Count -eq 1) {
        $current = $currentMatches[0]
        $headCommit = ([string](& git -C $repositoryRoot rev-parse HEAD)).Trim()
        $masterReference = Get-MasterReference
        if ($null -eq $masterReference) {
            Add-Failure 'master-ledger validation requires a fetched master reference.'
        }
        else {
            $masterCommit = ([string](& git -C $repositoryRoot rev-parse $masterReference)).Trim()
            if ($headCommit -cne $masterCommit) {
                Add-Failure "master-ledger validation requires HEAD at $masterReference commit $masterCommit."
            }
        }
        & git -C $repositoryRoot show-ref --verify --quiet $integrationRemoteReference
        if ($LASTEXITCODE -ne 0) {
            Add-Failure "qualified-ledger validation requires fetched origin/$integrationBranch."
        }
        else {
            $remoteDevCommit = ([string](& git -C $repositoryRoot rev-parse $integrationRemoteReference)).Trim()
            if ($headCommit -cne $remoteDevCommit) {
                Add-Failure "qualified-ledger validation requires HEAD at origin/$integrationBranch commit $remoteDevCommit."
            }
        }

        if ([string]$current.qualification.state -cne 'pass') {
            Add-Failure 'master head must identify a qualification pass.'
        }
        elseif ([string]$current.promotion.state -ceq 'tagged') {
            Test-PostPromotionCommit $current $headCommit
        }
        else {
            $masterTag = [string]$current.promotion.tag
            $masterTagIdentity = Get-AnnotatedTagIdentity `
                $masterTag `
                'master qualification tag'
            if ($null -ne $masterTagIdentity -and
                [string]$masterTagIdentity.PeeledCommit -cne $headCommit) {
                Add-Failure "master qualification tag '$masterTag' does not resolve to HEAD $headCommit."
            }
            if ($null -ne $masterTagIdentity) {
                Test-OriginAnnotatedTagIdentity `
                    $masterTag `
                    ([string]$masterTagIdentity.TagObject) `
                    $headCommit `
                    'master qualification attestation'
            }
            Test-EvidenceAttestation $current $headCommit 'master qualification attestation'
        }
    }
}

if ($RequireArtifacts) {
    if ($currentMatches.Count -eq 1) {
        $current = $currentMatches[0]
        if (@($current.packages).Count -eq 0 -or $null -eq $current.checksumManifest) {
            Add-Failure 'artifact verification requires recorded package and checksum identities.'
        }
        else {
            & (Join-Path $PSScriptRoot 'verify-packages.ps1')
            $packagesRoot = Join-Path $repositoryRoot 'artifacts\packages'
            $recordedArtifacts = @($current.packages) + @($current.checksumManifest)
            foreach ($artifact in $recordedArtifacts) {
                $artifactPath = Join-Path $packagesRoot ([string]$artifact.file)
                if (-not (Test-Path -LiteralPath $artifactPath -PathType Leaf)) {
                    Add-Failure "recorded artifact is missing: $($artifact.file)"
                    continue
                }
                $file = Get-Item -LiteralPath $artifactPath
                $hash = (Get-FileHash -LiteralPath $artifactPath -Algorithm SHA256).Hash.ToLowerInvariant()
                if ($file.Length -ne [long]$artifact.length -or
                    $hash -cne [string]$artifact.sha256) {
                    Add-Failure "recorded artifact identity differs: $($artifact.file)"
                }
            }
        }
    }
}

if ($failures.Count -gt 0) {
    throw ("Release-contract validation failed:`n - " + ($failures -join "`n - "))
}

Write-Host "Release contract verified: $($milestones.Count) milestone(s), current $($identity.ReleaseLabel), mode $Mode."
