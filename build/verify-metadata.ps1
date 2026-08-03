[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$propsPath = Join-Path $PSScriptRoot 'Version.props'
[xml]$props = Get-Content -LiteralPath $propsPath -Raw
$values = $props.Project.PropertyGroup

$productVersion = [string]$values.C3ProductVersion
$releaseStage = [string]$values.C3ReleaseStage
$releaseDate = [DateTime]::ParseExact(
    [string]$values.C3ReleaseDate,
    'yyyy-MM-dd',
    [Globalization.CultureInfo]::InvariantCulture)
$assemblyVersion = [string]$values.C3AssemblyVersion
$catalogueFormatVersion = [string]$values.C3CatalogueFormatVersion

$failures = New-Object Collections.Generic.List[String]

function Assert-Equal {
    param(
        [string]$Name,
        [string]$Expected,
        [string]$Actual
    )

    if ($Expected -cne $Actual) {
        $failures.Add("${Name}: expected '$Expected', found '$Actual'")
    }
}

$legacyVersionPath = Join-Path $repositoryRoot 'VERSION'
$legacyVersion = @(Get-Content -LiteralPath $legacyVersionPath)
if ($legacyVersion.Count -ne 3) {
    $failures.Add("VERSION: expected exactly 3 lines, found $($legacyVersion.Count)")
}
else {
    Assert-Equal 'VERSION product version' $productVersion $legacyVersion[0]
    Assert-Equal 'VERSION release stage' $releaseStage $legacyVersion[1]
    Assert-Equal 'VERSION release date' (
        $releaseDate.ToString('dd/MM/yyyy', [Globalization.CultureInfo]::InvariantCulture)) $legacyVersion[2]
}

$buildInfoPath = Join-Path $repositoryRoot 'Compact Cassette Catalogue\Generated\BuildInfo.g.vb'
$buildInfo = Get-Content -LiteralPath $buildInfoPath -Raw
$expectedFragments = @(
    "Public Const VERSION As String = `"$productVersion`""
    "Public Const VERSIONSTAGE As String = `"$releaseStage`""
    "New DateTime($($releaseDate.Year), $($releaseDate.Month), $($releaseDate.Day), 0, 0, 0, DateTimeKind.Local)"
    "Public Const VERSIONFILE As String = `"$catalogueFormatVersion`""
    "Public Const AssemblyVersion As String = `"$assemblyVersion`""
)
foreach ($fragment in $expectedFragments) {
    if (-not $buildInfo.Contains($fragment)) {
        $failures.Add("Generated BuildInfo is missing: $fragment")
    }
}

$changelogPath = Join-Path $repositoryRoot 'CHANGELOG.md'
$changelog = Get-Content -LiteralPath $changelogPath -Raw
if (-not $changelog.Contains("Version $productVersion $releaseStage")) {
    $failures.Add("CHANGELOG does not identify Version $productVersion $releaseStage")
}

if ($failures.Count -gt 0) {
    $message = "Metadata verification failed:`n - " + ($failures -join "`n - ")
    throw $message
}

Write-Host "Metadata verified: C3 $productVersion ($releaseStage), catalogue $catalogueFormatVersion."

