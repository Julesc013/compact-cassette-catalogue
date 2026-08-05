[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ExpectedProductVersion,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedStage,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedReleaseLabel,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedTag,
    [Parameter(Mandatory = $true)]
    [datetime]$ExpectedDate,
    [ValidateSet('alpha', 'beta', 'stable')]
    [string]$ExpectedChannel = 'alpha',
    [string]$ExpectedPublicationStatus = 'retained-unpublished',
    [string]$ExpectedCatalogueFormat = '1.1.0',
    [string]$ExpectedFeedVersion = '1.2.0',
    [string]$ExpectedFeedStage = 'Release',
    [string]$ExpectedFeedDate = '14/05/2026',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$VerifyBuildOutputs
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$globalsPath = Join-Path $repositoryRoot 'Compact Cassette Catalogue\varGlobals.vb'
$assemblyInfoPath = Join-Path $repositoryRoot 'Compact Cassette Catalogue\My Project\AssemblyInfo.vb'
$projectPath = Join-Path $repositoryRoot 'Compact Cassette Catalogue\Compact Cassette Catalogue.vbproj'
$manifestPath = Join-Path $PSScriptRoot 'lanes.json'
$globals = Get-Content -LiteralPath $globalsPath -Raw
$assemblyInfo = Get-Content -LiteralPath $assemblyInfoPath -Raw
$project = Get-Content -LiteralPath $projectPath -Raw
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$failures = New-Object Collections.Generic.List[String]

function Assert-Contains {
    param([string]$Name, [string]$Text, [string]$Fragment)
    if (-not $Text.Contains($Fragment)) {
        $failures.Add("$Name is missing: $Fragment")
    }
}

Assert-Contains 'varGlobals VERSION' $globals "Public Const VERSION As String = `"$ExpectedProductVersion`""
Assert-Contains 'varGlobals VERSIONSTAGE' $globals "Public Const VERSIONSTAGE As String = `"$ExpectedStage`""
Assert-Contains 'varGlobals VERSIONDATE' $globals (
    "New DateTime({0}, {1}, {2}, 0, 0, 0, DateTimeKind.Local)" -f
    $ExpectedDate.Year, $ExpectedDate.Month, $ExpectedDate.Day)
Assert-Contains 'varGlobals VERSIONFILE' $globals "Public Const VERSIONFILE As String = `"$ExpectedCatalogueFormat`""

$numericVersion = "$ExpectedProductVersion.0"
Assert-Contains 'AssemblyVersion' $assemblyInfo "<Assembly: AssemblyVersion(`"$numericVersion`")>"
Assert-Contains 'AssemblyFileVersion' $assemblyInfo "<Assembly: AssemblyFileVersion(`"$numericVersion`")>"
Assert-Contains 'AssemblyInformationalVersion' $assemblyInfo "<Assembly: AssemblyInformationalVersion(`"$ExpectedReleaseLabel`")>"
Assert-Contains 'ApplicationVersion' $project "<ApplicationVersion>$numericVersion</ApplicationVersion>"

foreach ($manifestAssertion in @(
        @('releaseVersion', [string]$manifest.releaseVersion, $ExpectedProductVersion),
        @('releaseStage', [string]$manifest.releaseStage, $ExpectedStage),
        @('releaseLabel', [string]$manifest.releaseLabel, $ExpectedReleaseLabel),
        @('releaseTag', [string]$manifest.releaseTag, $ExpectedTag),
        @('releaseChannel', [string]$manifest.releaseChannel, $ExpectedChannel),
        @('publicationStatus', [string]$manifest.publicationStatus, $ExpectedPublicationStatus),
        @('assemblyVersion', [string]$manifest.assemblyVersion, $numericVersion),
        @('fileVersion', [string]$manifest.fileVersion, $numericVersion),
        @('assemblyProductVersion', [string]$manifest.assemblyProductVersion, $ExpectedReleaseLabel))) {
    if ([string]$manifestAssertion[1] -cne [string]$manifestAssertion[2]) {
        $failures.Add("lanes.json $($manifestAssertion[0]) is '$($manifestAssertion[1])', expected '$($manifestAssertion[2])'.")
    }
}
$expectedPackageNames = @($manifest.lanes | ForEach-Object {
    "C3-v$ExpectedReleaseLabel-$($_.id)-portable.zip"
})
$actualPackageNames = @($manifest.lanes | ForEach-Object { [string]$_.packageName })
if (($actualPackageNames -join "`n") -cne ($expectedPackageNames -join "`n")) {
    $failures.Add("lanes.json package names do not project release label '$ExpectedReleaseLabel'.")
}

$displayStage = $ExpectedStage
$releaseNotes = Get-Content -LiteralPath (Join-Path $repositoryRoot 'RELEASE_NOTES.md') -Raw
$changelog = Get-Content -LiteralPath (Join-Path $repositoryRoot 'CHANGELOG.md') -Raw
Assert-Contains 'RELEASE_NOTES.md' $releaseNotes "# Compact Cassette Catalogue $ExpectedProductVersion $displayStage"
Assert-Contains 'CHANGELOG.md' $changelog "### Version $ExpectedProductVersion $displayStage"

$feedLines = @(Get-Content -LiteralPath (Join-Path $repositoryRoot 'VERSION'))
$expectedFeedLines = @($ExpectedFeedVersion, $ExpectedFeedStage, $ExpectedFeedDate)
if (($feedLines -join "`n") -cne ($expectedFeedLines -join "`n")) {
    $failures.Add("Public VERSION feed differs from expected isolated identity: $($expectedFeedLines -join ' / ')")
}

if ($VerifyBuildOutputs) {
    $lanes = @((Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json).lanes)
    foreach ($lane in $lanes) {
        $executable = Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\Compact Cassette Catalogue.exe"
        if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
            $failures.Add("Missing build output for identity verification: $($lane.id)")
            continue
        }
        $versionInfo = (Get-Item -LiteralPath $executable).VersionInfo
        if ([string]$versionInfo.ProductVersion -cne $ExpectedReleaseLabel) {
            $failures.Add("$($lane.id) ProductVersion is '$($versionInfo.ProductVersion)', expected '$ExpectedReleaseLabel'.")
        }
        if ([string]$versionInfo.FileVersion -cne $numericVersion) {
            $failures.Add("$($lane.id) FileVersion is '$($versionInfo.FileVersion)', expected '$numericVersion'.")
        }
    }
}

if ($failures.Count -gt 0) {
    throw "Release identity verification failed:`n - $($failures -join "`n - ")"
}

$scope = if ($VerifyBuildOutputs) { 'source, documents, feed, and build outputs' } else { 'source, documents, and feed' }
Write-Host "Release identity verified for $ExpectedProductVersion $ExpectedStage ($scope); catalogue $ExpectedCatalogueFormat; public feed remains $ExpectedFeedVersion $ExpectedFeedStage."
