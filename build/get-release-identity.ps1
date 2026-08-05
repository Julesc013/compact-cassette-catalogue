[CmdletBinding()]
param(
    [string]$PropsPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($PropsPath)) {
    $PropsPath = Join-Path $PSScriptRoot 'Version.props'
}

if (-not (Test-Path -LiteralPath $PropsPath -PathType Leaf)) {
    throw "Release identity file is missing: $PropsPath"
}

[xml]$props = Get-Content -LiteralPath $PropsPath -Raw
$values = $props.Project.PropertyGroup

function Get-RequiredProperty {
    param([string]$Name)

    $nodes = @($values.ChildNodes | Where-Object { $_.LocalName -ceq $Name })
    if ($nodes.Count -ne 1 -or [string]::IsNullOrWhiteSpace($nodes[0].InnerText)) {
        throw "Release identity property '$Name' is missing or empty."
    }
    return $nodes[0].InnerText.Trim()
}

$productVersion = Get-RequiredProperty 'C3ProductVersion'
$releaseStage = Get-RequiredProperty 'C3ReleaseStage'
$releaseChannel = Get-RequiredProperty 'C3ReleaseChannel'
$updateFeedUrl = Get-RequiredProperty 'C3UpdateFeedUrl'
$releaseDateText = Get-RequiredProperty 'C3ReleaseDate'
$assemblyVersion = Get-RequiredProperty 'C3AssemblyVersion'
$fileVersion = Get-RequiredProperty 'C3FileVersion'
$catalogueFormatVersion = Get-RequiredProperty 'C3LegacyCatalogueFormatVersion'

$releaseDate = [DateTime]::ParseExact(
    $releaseDateText,
    'yyyy-MM-dd',
    [Globalization.CultureInfo]::InvariantCulture)
[void][Version]$assemblyVersion
[void][Version]$fileVersion
[void][Version]$catalogueFormatVersion

$releaseLabelIdentity = & (Join-Path $PSScriptRoot 'resolve-release-label.ps1') `
    -ProductVersion $productVersion `
    -ReleaseStage $releaseStage
$stageSlug = $releaseLabelIdentity.StageSlug
$expectedChannel = $releaseLabelIdentity.ReleaseChannel

if ($releaseChannel -cne $expectedChannel) {
    throw "C3ReleaseChannel '$releaseChannel' conflicts with stage '$releaseStage'; expected '$expectedChannel'."
}

$releaseLabel = $releaseLabelIdentity.ReleaseLabel
$tagName = & (Join-Path $PSScriptRoot 'resolve-release-tag.ps1') `
    -ReleaseLabel $releaseLabel

[PSCustomObject]@{
    ProductVersion = $productVersion
    ReleaseStage = $releaseStage
    StageSlug = $stageSlug
    ReleaseLabel = $releaseLabel
    ReleaseChannel = $releaseChannel
    UpdateFeedUrl = $updateFeedUrl
    ReleaseDate = $releaseDate
    AssemblyVersion = $assemblyVersion
    FileVersion = $fileVersion
    CatalogueFormatVersion = $catalogueFormatVersion
    InformationalVersion = $releaseLabel
    TagName = $tagName
    PropsPath = [IO.Path]::GetFullPath($PropsPath)
}
