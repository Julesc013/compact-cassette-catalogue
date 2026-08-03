[CmdletBinding()]
param(
    [string]$PropsPath = (Join-Path $PSScriptRoot 'Version.props')
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

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
$parsedProductVersion = $null
if (-not [Version]::TryParse($productVersion, [ref]$parsedProductVersion)) {
    throw "C3ProductVersion is not numeric: $productVersion"
}

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

$stageSlug = ($releaseStage.ToLowerInvariant() -replace '[^a-z0-9]+', '.').Trim('.')
$releaseLabel = $productVersion
if (-not [string]::Equals($releaseStage, 'Release', [StringComparison]::OrdinalIgnoreCase)) {
    if ([string]::IsNullOrWhiteSpace($stageSlug)) {
        throw "C3ReleaseStage cannot form a release label: $releaseStage"
    }
    $releaseLabel += '-' + $stageSlug
}

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
    TagName = 'v' + $releaseLabel
    PropsPath = [IO.Path]::GetFullPath($PropsPath)
}
