[CmdletBinding()]
param(
    [string]$TrainPath,
    [string]$SchemaPath,
    [string]$CatalogPath,
    [string]$VersionPropsPath,
    [switch]$SkipGitFacts
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($TrainPath)) {
    $TrainPath = Join-Path $repositoryRoot 'release\train\2.0.0.json'
}
if ([string]::IsNullOrWhiteSpace($SchemaPath)) {
    $SchemaPath = Join-Path $repositoryRoot 'spec\release-train\v2\train.schema.json'
}
if ([string]::IsNullOrWhiteSpace($CatalogPath)) {
    $CatalogPath = Join-Path $repositoryRoot 'release\catalog.v1.json'
}
if ([string]::IsNullOrWhiteSpace($VersionPropsPath)) {
    $VersionPropsPath = Join-Path $PSScriptRoot 'Version.props'
}

$TrainPath = [IO.Path]::GetFullPath($TrainPath)
$SchemaPath = [IO.Path]::GetFullPath($SchemaPath)
$CatalogPath = [IO.Path]::GetFullPath($CatalogPath)
$VersionPropsPath = [IO.Path]::GetFullPath($VersionPropsPath)

& (Join-Path $PSScriptRoot 'validate-json-document.ps1') `
    -SchemaPath $SchemaPath `
    -DocumentPath $TrainPath `
    -MaximumBytes (256 * 1024)
& (Join-Path $PSScriptRoot 'validate-json-document.ps1') `
    -SchemaPath (Join-Path $repositoryRoot 'spec\release-catalog\v1\catalog.schema.json') `
    -DocumentPath $CatalogPath `
    -MaximumBytes (4 * 1024 * 1024)

$train = Get-Content -LiteralPath $TrainPath -Raw | ConvertFrom-Json
$catalog = Get-Content -LiteralPath $CatalogPath -Raw | ConvertFrom-Json
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1') `
    -PropsPath $VersionPropsPath
$failures = New-Object Collections.Generic.List[String]

function Add-Failure {
    param([string]$Message)

    [void]$failures.Add($Message)
}

$expectedIds = @(
    'alpha.1',
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
    'beta.1')
$milestones = @($train.milestones)
$currentIndex = -1

if ($milestones.Count -ne $expectedIds.Count) {
    Add-Failure "release train must contain exactly $($expectedIds.Count) milestones."
}
for ($index = 0; $index -lt [Math]::Min($milestones.Count, $expectedIds.Count); $index++) {
    $milestone = $milestones[$index]
    $expectedId = $expectedIds[$index]
    $expectedLabel = '2.0.0-' + $expectedId
    if ([string]$milestone.id -cne $expectedId) {
        Add-Failure "release train milestone $index must be '$expectedId'."
    }
    if ([string]$milestone.releaseLabel -cne $expectedLabel) {
        Add-Failure "release train milestone '$expectedId' must use label '$expectedLabel'."
    }
    if ([string]$milestone.id -ceq [string]$train.currentMilestone) {
        if ($currentIndex -ge 0) {
            Add-Failure 'release train currentMilestone occurs more than once.'
        }
        $currentIndex = $index
    }
}

if ($currentIndex -lt 0) {
    Add-Failure "release train currentMilestone '$($train.currentMilestone)' is absent."
}
else {
    for ($index = 0; $index -lt $milestones.Count; $index++) {
        $actualState = [string]$milestones[$index].state
        $expectedState = if ($index -lt $currentIndex) {
            'qualified'
        }
        elseif ($index -gt $currentIndex) {
            'pending'
        }
        elseif ([string]$train.status -ceq 'awaiting-owner-manual-validation') {
            'awaiting-owner-manual-validation'
        }
        else {
            'active'
        }
        if ($actualState -cne $expectedState) {
            Add-Failure "milestone '$($milestones[$index].id)' state '$actualState' must be '$expectedState'."
        }
    }

    if ([string]$train.status -ceq 'awaiting-owner-manual-validation' -and
        [string]$train.currentMilestone -cne 'beta.1') {
        Add-Failure 'only Beta 1 may await owner manual validation in the 2.0.0 train.'
    }
    $expectedLastTag = if ($currentIndex -eq 0) {
        $null
    }
    else {
        'v' + [string]$milestones[$currentIndex - 1].releaseLabel
    }
    if ($null -eq $expectedLastTag) {
        if ($null -ne $train.lastQualifiedTag) {
            Add-Failure 'lastQualifiedTag must be null before Alpha 1 qualification.'
        }
    }
    elseif ([string]$train.lastQualifiedTag -cne $expectedLastTag) {
        Add-Failure "lastQualifiedTag must be '$expectedLastTag'."
    }

    $currentReleaseLabel = [string]$milestones[$currentIndex].releaseLabel
    if ([string]$identity.ReleaseLabel -cne $currentReleaseLabel) {
        Add-Failure "build identity '$($identity.ReleaseLabel)' does not match current train label '$currentReleaseLabel'."
    }
}

$catalogByLabel = @{}
foreach ($catalogMilestone in @($catalog.milestones)) {
    $label = [string]$catalogMilestone.releaseLabel
    if ($catalogByLabel.ContainsKey($label)) {
        Add-Failure "release catalogue contains duplicate milestone '$label'."
    }
    else {
        $catalogByLabel[$label] = $catalogMilestone
    }
}

if ($currentIndex -ge 0) {
    for ($index = 0; $index -le $currentIndex; $index++) {
        $trainMilestone = $milestones[$index]
        $label = [string]$trainMilestone.releaseLabel
        if (-not $catalogByLabel.ContainsKey($label)) {
            Add-Failure "release catalogue is missing train milestone '$label'."
            continue
        }
        $catalogMilestone = $catalogByLabel[$label]
        $expectedChannel = if ([string]$trainMilestone.id -ceq 'beta.1') {
            'beta'
        }
        else {
            'alpha'
        }
        if ([string]$catalogMilestone.productVersion -cne '2.0.0' -or
            [string]$catalogMilestone.channel -cne $expectedChannel) {
            Add-Failure "release catalogue identity for '$label' does not match the train."
        }

        if ($index -lt $currentIndex) {
            if ([string]$catalogMilestone.qualification.state -cne 'pass' -or
                [string]$catalogMilestone.promotion.state -cne 'tagged' -or
                [string]$catalogMilestone.promotion.tag -cne ('v' + $label) -or
                [string]$catalogMilestone.promotion.tagObject -cnotmatch '^[0-9a-f]{40}$') {
                Add-Failure "qualified train milestone '$label' lacks completed catalogue/tag evidence."
            }
            if ([string]$trainMilestone.id -cmatch '^alpha\.' -and
                ([string]$catalogMilestone.publication.policy -cne 'intentionally-unpublished' -or
                    [string]$catalogMilestone.publication.state -cne 'unpublished' -or
                    [bool]$catalogMilestone.publication.feedPromoted -or
                    [string]$catalogMilestone.postVerification.state -cne 'not-applicable')) {
                Add-Failure "qualified Alpha milestone '$label' is not intentionally unpublished."
            }
        }
    }

    if ($null -ne $train.candidateCommit) {
        $currentLabel = [string]$milestones[$currentIndex].releaseLabel
        if (-not $catalogByLabel.ContainsKey($currentLabel) -or
            [string]$catalogByLabel[$currentLabel].qualification.sourceCommit -cne
                [string]$train.candidateCommit) {
            Add-Failure 'candidateCommit does not match current catalogue source commit C.'
        }
    }
}

$gitMetadataAvailable = Test-Path -LiteralPath (Join-Path $repositoryRoot '.git')
if (-not $SkipGitFacts -and $gitMetadataAvailable -and $currentIndex -gt 0) {
    for ($index = 0; $index -lt $currentIndex; $index++) {
        $tagName = 'v' + [string]$milestones[$index].releaseLabel
        $tagType = [string](& git -C $repositoryRoot cat-file -t "refs/tags/$tagName" 2>$null)
        if ($LASTEXITCODE -ne 0 -or $tagType.Trim() -cne 'tag') {
            Add-Failure "qualified train tag '$tagName' is missing or is not annotated."
        }
    }
}

if ($failures.Count -gt 0) {
    throw ("Release-train validation failed:`n - " + ($failures -join "`n - "))
}

Write-Host (
    "Release train verified: current $($train.currentMilestone), " +
    "status $($train.status), $($milestones.Count) milestones.")
