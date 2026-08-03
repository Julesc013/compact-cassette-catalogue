[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ProductVersion,
    [Parameter(Mandatory = $true)]
    [string]$ReleaseStage
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$numericIdentifier = '(?:0|[1-9][0-9]*)'
$productVersionPattern = "^$numericIdentifier\.$numericIdentifier\.$numericIdentifier`$"
$parsedProductVersion = $null
if ($ProductVersion -cnotmatch $productVersionPattern -or
    -not [Version]::TryParse($ProductVersion, [ref]$parsedProductVersion)) {
    throw "ProductVersion must be a canonical three-part numeric version: $ProductVersion"
}

$stageSlug = $null
$releaseChannel = $null
if ($ReleaseStage -ceq 'Release') {
    $stageSlug = 'release'
    $releaseChannel = 'stable'
}
elseif ($ReleaseStage -cmatch '^Alpha ([1-9][0-9]*)$') {
    $stageSlug = 'alpha.' + $Matches[1]
    $releaseChannel = 'alpha'
}
elseif ($ReleaseStage -cmatch '^Beta ([1-9][0-9]*)$') {
    $stageSlug = 'beta.' + $Matches[1]
    $releaseChannel = 'beta'
}
elseif ($ReleaseStage -cmatch '^Release Candidate ([1-9][0-9]*)$') {
    $stageSlug = 'rc.' + $Matches[1]
    $releaseChannel = 'beta'
}
else {
    throw "ReleaseStage must be 'Alpha N', 'Beta N', 'Release Candidate N', or 'Release': $ReleaseStage"
}

$releaseLabel = $ProductVersion
if ($ReleaseStage -cne 'Release') {
    $releaseLabel += '-' + $stageSlug
}

[PSCustomObject]@{
    ProductVersion = $ProductVersion
    ReleaseStage = $ReleaseStage
    StageSlug = $stageSlug
    ReleaseChannel = $releaseChannel
    ReleaseLabel = $releaseLabel
}
