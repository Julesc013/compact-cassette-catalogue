[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'High')]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet(
        'alpha.2',
        'alpha.3',
        'alpha.4',
        'alpha.5',
        'alpha.6',
        'alpha.7',
        'alpha.8',
        'alpha.9',
        'alpha.10',
        'alpha.11',
        'alpha.12',
        'beta.1')]
    [string]$Milestone,
    [string]$ReleaseDate = (Get-Date -Format 'yyyy-MM-dd')
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$branchContract = & (Join-Path $PSScriptRoot 'get-branch-contract.ps1') `
    -RepositoryRoot $repositoryRoot
$qualifiedBranch = [string]$branchContract.CurrentQualified
$integrationBranch = [string]$branchContract.CurrentIntegration
$trainPath = Join-Path $repositoryRoot 'release\train\2.0.0.json'
$catalogPath = Join-Path $repositoryRoot 'release\catalog.v1.json'
$propsPath = Join-Path $PSScriptRoot 'Version.props'
$utf8WithoutBom = New-Object Text.UTF8Encoding($false)

try {
    [void][DateTime]::ParseExact(
        $ReleaseDate,
        'yyyy-MM-dd',
        [Globalization.CultureInfo]::InvariantCulture)
}
catch {
    throw "ReleaseDate '$ReleaseDate' must use yyyy-MM-dd."
}

& (Join-Path $PSScriptRoot 'validate-release-train.ps1')
& (Join-Path $PSScriptRoot 'validate-release-contract.ps1') -Mode Repository

$branch = ([string](& git -C $repositoryRoot branch --show-current)).Trim()
if ($LASTEXITCODE -ne 0 -or $branch -cne $integrationBranch) {
    throw "Milestone transition requires branch '$integrationBranch'; found '$branch'."
}
$worktree = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
if ($LASTEXITCODE -ne 0 -or $worktree.Count -ne 0) {
    throw 'Milestone transition requires a clean worktree.'
}
$head = ([string](& git -C $repositoryRoot rev-parse HEAD)).Trim()
$integration = ([string](& git -C $repositoryRoot rev-parse (
            "refs/heads/$integrationBranch"))).Trim()
$qualified = ([string](& git -C $repositoryRoot rev-parse (
            "refs/heads/$qualifiedBranch"))).Trim()
$remoteIntegration = ([string](& git -C $repositoryRoot rev-parse (
            "refs/remotes/origin/$integrationBranch"))).Trim()
$remoteQualified = ([string](& git -C $repositoryRoot rev-parse (
            "refs/remotes/origin/$qualifiedBranch"))).Trim()
if ($head -cne $integration) {
    throw "Milestone transition requires HEAD at local $integrationBranch."
}
if ($head -cne $remoteIntegration) {
    throw "Milestone transition requires origin/$integrationBranch at current HEAD."
}
if ($qualified -cne $remoteQualified) {
    throw "Local and origin/$qualifiedBranch must identify the same qualified checkpoint."
}
& git -C $repositoryRoot merge-base --is-ancestor $qualified $head
if ($LASTEXITCODE -ne 0) {
    throw "$integrationBranch must descend from qualified ledger $qualifiedBranch."
}

$train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
$catalog = Get-Content -LiteralPath $catalogPath -Raw | ConvertFrom-Json
$milestones = @($train.milestones)
$currentIndex = -1
for ($index = 0; $index -lt $milestones.Count; $index++) {
    if ([string]$milestones[$index].id -ceq [string]$train.currentMilestone) {
        $currentIndex = $index
        break
    }
}
if ($currentIndex -lt 0 -or $currentIndex + 1 -ge $milestones.Count -or
    [string]$milestones[$currentIndex + 1].id -cne $Milestone) {
    throw "Milestone '$Milestone' is not the immediate successor to '$($train.currentMilestone)'."
}

$previousLabel = [string]$milestones[$currentIndex].releaseLabel
$previousTag = 'v' + $previousLabel
$previousCatalog = @($catalog.milestones | Where-Object {
        [string]$_.releaseLabel -ceq $previousLabel
    })
if ($previousCatalog.Count -ne 1 -or
    [string]$previousCatalog[0].qualification.state -cne 'pass' -or
    [string]$previousCatalog[0].promotion.state -cne 'tagged') {
    throw "Previous milestone '$previousLabel' is not qualified and tagged in the release catalogue."
}
$tagType = ([string](& git -C $repositoryRoot cat-file -t (
            'refs/tags/' + $previousTag))).Trim()
if ($LASTEXITCODE -ne 0 -or $tagType -cne 'tag') {
    throw "Previous qualified tag '$previousTag' is missing or not annotated."
}
$tagCommit = ([string](& git -C $repositoryRoot rev-list -n 1 (
            'refs/tags/' + $previousTag))).Trim()
$qualifiedLine = @((([string](& git -C $repositoryRoot rev-list `
                    --parents -n 1 $qualified)).Trim()) -split ' ')
if ($qualifiedLine.Count -ne 2 -or $qualifiedLine[1] -cne $tagCommit) {
    throw "$qualifiedBranch must identify direct post-operation child P of $previousTag."
}
$previousValidationPath = "release/validation/$previousLabel.md"
& git -C $repositoryRoot diff --quiet $qualified HEAD -- `
    'release/catalog.v1.json' $previousValidationPath
if ($LASTEXITCODE -ne 0) {
    throw 'Integration lifecycle evidence differs from the qualified post-operation checkpoint.'
}

$next = $milestones[$currentIndex + 1]
$nextLabel = [string]$next.releaseLabel
$stage = if ($Milestone -cmatch '^alpha\.(?<sequence>[1-9]|1[0-2])$') {
    'Alpha ' + $Matches.sequence
}
else {
    'Beta 1'
}
$channel = if ($Milestone -ceq 'beta.1') { 'beta' } else { 'alpha' }
$publicationPolicy = if ($channel -ceq 'beta') {
    'public-prerelease'
}
else {
    'intentionally-unpublished'
}
$feedBranch = if ($channel -ceq 'alpha') {
    $integrationBranch
}
else {
    $qualifiedBranch
}
$feedUrl =
    'https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/' +
    "$feedBranch/release/feeds/$channel/release.json"
$fileVersion = '2.0.0.' + ($currentIndex + 2).ToString(
    [Globalization.CultureInfo]::InvariantCulture)

$nextCatalog = [PSCustomObject][ordered]@{
    releaseLabel = $nextLabel
    productVersion = '2.0.0'
    stage = $stage
    channel = $channel
    predecessor = $previousLabel
    qualification = [PSCustomObject][ordered]@{
        state = 'active'
        sourceCommit = $null
    }
    supersededBy = $null
    promotion = [PSCustomObject][ordered]@{
        state = 'unpromoted'
        targetBranch = $qualifiedBranch
        tag = 'v' + $nextLabel
        tagObject = $null
    }
    publication = [PSCustomObject][ordered]@{
        policy = $publicationPolicy
        state = 'unpublished'
        releaseUrl = $null
        feedPromoted = $false
    }
    postVerification = [PSCustomObject][ordered]@{
        state = 'not-applicable'
    }
    validationRecord = "release/validation/$nextLabel.md"
    packages = @()
    checksumManifest = $null
}

if (@($catalog.milestones | Where-Object {
            [string]$_.releaseLabel -ceq $nextLabel
        }).Count -ne 0) {
    throw "Release catalogue already contains '$nextLabel'."
}

if ($PSCmdlet.ShouldProcess($nextLabel, "begin release-train milestone $Milestone")) {
    $milestones[$currentIndex].state = 'qualified'
    $milestones[$currentIndex + 1].state = 'active'
    $train.currentMilestone = $Milestone
    $train.status = 'active'
    $train.lastQualifiedTag = $previousTag
    $train.candidateCommit = $null
    [IO.File]::WriteAllText(
        $trainPath,
        (($train | ConvertTo-Json -Depth 100) + "`n"),
        $utf8WithoutBom)

    $catalog.milestones = @($catalog.milestones) + @($nextCatalog)
    [IO.File]::WriteAllText(
        $catalogPath,
        (($catalog | ConvertTo-Json -Depth 100) + "`n"),
        $utf8WithoutBom)

    $propsText = [IO.File]::ReadAllText($propsPath)
    $replacements = [ordered]@{
        C3ReleaseStage = $stage
        C3ReleaseChannel = $channel
        C3UpdateFeedUrl = $feedUrl
        C3ReleaseDate = $ReleaseDate
        C3FileVersion = $fileVersion
    }
    foreach ($property in $replacements.Keys) {
        $pattern = "<$property>[^<]+</$property>"
        if (-not [regex]::IsMatch($propsText, $pattern)) {
            throw "Could not find build identity property '$property'."
        }
        $replacement = "<$property>$($replacements[$property])</$property>"
        $propsText = [regex]::Replace($propsText, $pattern, $replacement)
    }
    [IO.File]::WriteAllText($propsPath, $propsText, $utf8WithoutBom)

    $validationPath = Join-Path $repositoryRoot (
        "release\validation\$nextLabel.md")
    $validationText = @"
# C3 $stage release validation

Qualification: **active**

Promotion: **unpromoted**

Publication policy: **$publicationPolicy**

Publication state: **unpublished**

Post-verification: **not-applicable**

The milestone is active. Candidate source, package, compatibility, workflow, and
deferred evidence are recorded only after the implementation is frozen and
qualified through the repository-owned C/E/P transaction.
"@
    [IO.File]::WriteAllText(
        $validationPath,
        $validationText.TrimEnd() + "`n",
        $utf8WithoutBom)

    $changelogPath = Join-Path $repositoryRoot 'CHANGELOG.md'
    $changelog = [IO.File]::ReadAllText($changelogPath)
    $heading = "### Version 2.0.0 $stage - In development"
    if ($changelog.Contains($heading)) {
        throw "CHANGELOG already contains '$heading'."
    }
    $releaseMarker = '## Releases'
    $releaseIndex = $changelog.IndexOf(
        $releaseMarker,
        [StringComparison]::Ordinal)
    if ($releaseIndex -lt 0) {
        throw 'CHANGELOG is missing its Releases section.'
    }
    $insertIndex = $releaseIndex + $releaseMarker.Length
    $changelog = $changelog.Insert(
        $insertIndex,
        "`r`n`r`n$heading`r`n`r`n- Milestone implementation in progress.`r`n")
    [IO.File]::WriteAllText($changelogPath, $changelog, $utf8WithoutBom)

    $releaseNotesPath = Join-Path $repositoryRoot 'RELEASE_NOTES.md'
    $releaseNotes = @"
# Compact Cassette Catalogue 2.0.0 $stage

C3 2.0.0 $stage is an in-development checkpoint in the repository-owned 2.0
release train. Its scope, evidence, limitations, and package identities will be
recorded here before candidate freeze.

This checkpoint is not published. See the
[execution plan](docs/planning/2.0-execution-plan.md) and
[validation record](release/validation/$nextLabel.md).
"@
    [IO.File]::WriteAllText(
        $releaseNotesPath,
        $releaseNotes.TrimEnd() + "`n",
        $utf8WithoutBom)

    & (Join-Path $PSScriptRoot 'sync-version.ps1')
    & (Join-Path $PSScriptRoot 'validate-release-train.ps1')
    & (Join-Path $PSScriptRoot 'verify-metadata.ps1')
    & (Join-Path $PSScriptRoot 'validate-release-contract.ps1') -Mode Repository
}

Write-Host (
    "Prepared $nextLabel identity. Review and commit this transition separately " +
    'before milestone implementation.')
