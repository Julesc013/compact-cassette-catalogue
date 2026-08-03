[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$productVersion = $identity.ProductVersion
$releaseStage = $identity.ReleaseStage
$releaseChannel = $identity.ReleaseChannel
$updateFeedUrl = $identity.UpdateFeedUrl
$releaseDate = $identity.ReleaseDate
$assemblyVersion = $identity.AssemblyVersion
$fileVersion = $identity.FileVersion
$catalogueFormatVersion = $identity.CatalogueFormatVersion
$informationalVersion = $identity.InformationalVersion

$failures = New-Object Collections.Generic.List[String]

$updateFeedUri = $null
if (-not [Uri]::TryCreate($updateFeedUrl, [UriKind]::Absolute, [ref]$updateFeedUri)) {
    $failures.Add("update feed URL is not absolute: '$updateFeedUrl'")
}
else {
    if ($updateFeedUri.Scheme -cne 'https') {
        $failures.Add('update feed URL must use HTTPS.')
    }
    if ($updateFeedUri.Host -cne 'raw.githubusercontent.com') {
        $failures.Add('update feed URL must use the approved raw.githubusercontent.com host.')
    }
    $expectedFeedSuffix = "/release/feeds/$releaseChannel/VERSION"
    if (-not $updateFeedUri.AbsolutePath.EndsWith(
            $expectedFeedSuffix,
            [StringComparison]::Ordinal)) {
        $failures.Add("update feed URL must end with '$expectedFeedSuffix'.")
    }
    if ($releaseChannel -ceq 'alpha' -and
        $updateFeedUri.AbsolutePath.IndexOf(
            '/Julesc013/compact-cassette-catalogue/dev/',
            [StringComparison]::Ordinal) -lt 0) {
        $failures.Add('the alpha update feed must remain isolated on the dev branch.')
    }
    if (-not [string]::IsNullOrEmpty($updateFeedUri.Query) -or
        -not [string]::IsNullOrEmpty($updateFeedUri.Fragment)) {
        $failures.Add('update feed URL must not contain a query or fragment.')
    }
}

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

$channelDirectory = Join-Path $repositoryRoot ("release\feeds\" + $releaseChannel)
$channelVersionPath = Join-Path $channelDirectory 'VERSION'
$channelVersion = @(Get-Content -LiteralPath $channelVersionPath)
if ($channelVersion.Count -ne 3) {
    $failures.Add("$releaseChannel/VERSION: expected exactly 3 lines, found $($channelVersion.Count)")
}
else {
    Assert-Equal "$releaseChannel/VERSION product version" $productVersion $channelVersion[0]
    Assert-Equal "$releaseChannel/VERSION release stage" $releaseStage $channelVersion[1]
    Assert-Equal "$releaseChannel/VERSION release date" (
        $releaseDate.ToString('dd/MM/yyyy', [Globalization.CultureInfo]::InvariantCulture)) $channelVersion[2]
}

$channelManifestPath = Join-Path $channelDirectory 'release.json'
$channelManifest = Get-Content -LiteralPath $channelManifestPath -Raw | ConvertFrom-Json
Assert-Equal 'release manifest schema' '1' ([string]$channelManifest.schemaVersion)
Assert-Equal 'release manifest product ID' 'c3' ([string]$channelManifest.productId)
Assert-Equal 'release manifest channel' $releaseChannel ([string]$channelManifest.channel)
Assert-Equal 'release manifest version' $productVersion ([string]$channelManifest.version)
Assert-Equal 'release manifest stage' $releaseStage ([string]$channelManifest.stage)
Assert-Equal 'release manifest informational version' $informationalVersion ([string]$channelManifest.informationalVersion)
Assert-Equal 'release manifest release date' (
    $releaseDate.ToString('yyyy-MM-dd', [Globalization.CultureInfo]::InvariantCulture)) (
    [string]$channelManifest.releaseDate)
Assert-Equal 'release manifest catalogue writer' $catalogueFormatVersion (
    [string]$channelManifest.catalogueWriteFormat)
Assert-Equal 'development feed must not claim publication' 'False' ([string]$channelManifest.published)

$legacyRootPath = Join-Path $repositoryRoot 'VERSION'
$legacyChannelPath = Join-Path $repositoryRoot 'release\feeds\legacy-1x\VERSION'
$legacyRoot = @(Get-Content -LiteralPath $legacyRootPath)
$legacyChannel = @(Get-Content -LiteralPath $legacyChannelPath)
if ($legacyRoot.Count -ne 3) {
    $failures.Add("root VERSION: expected exactly 3 lines, found $($legacyRoot.Count)")
}
if ($legacyChannel.Count -ne 3) {
    $failures.Add("legacy-1x/VERSION: expected exactly 3 lines, found $($legacyChannel.Count)")
}
if ($legacyRoot.Count -eq 3 -and $legacyChannel.Count -eq 3) {
    Assert-Equal 'root and legacy-1x VERSION content' ($legacyChannel -join "`n") ($legacyRoot -join "`n")
    if ([Version]$legacyRoot[0] -ge [Version]'2.0.0') {
        $failures.Add('root VERSION must remain on the legacy 1.x update line')
    }
}

$buildInfoPath = Join-Path $repositoryRoot 'src\C3.WinForms\Generated\BuildInfo.g.vb'
$buildInfo = Get-Content -LiteralPath $buildInfoPath -Raw
$expectedFragments = @(
    "Public Const VERSION As String = `"$productVersion`""
    "Public Const VERSIONSTAGE As String = `"$releaseStage`""
    "New DateTime($($releaseDate.Year), $($releaseDate.Month), $($releaseDate.Day), 0, 0, 0, DateTimeKind.Local)"
    "Public Const VERSIONFILE As String = `"$catalogueFormatVersion`""
    "Public Const ReleaseChannel As String = `"$releaseChannel`""
    "Public Const UpdateFeedUrl As String = `"$updateFeedUrl`""
    "Public Const AssemblyVersion As String = `"$assemblyVersion`""
    "Public Const FileVersion As String = `"$fileVersion`""
    "Public Const InformationalVersion As String = `"$informationalVersion`""
)
foreach ($fragment in $expectedFragments) {
    if (-not $buildInfo.Contains($fragment)) {
        $failures.Add("Generated BuildInfo is missing: $fragment")
    }
}

$versionAssemblyInfoPath = Join-Path $repositoryRoot 'src\Shared\Generated\VersionAssemblyInfo.g.vb'
$versionAssemblyInfo = Get-Content -LiteralPath $versionAssemblyInfoPath -Raw
$expectedAssemblyFragments = @(
    "<Assembly: AssemblyVersion(`"$assemblyVersion`")>"
    "<Assembly: AssemblyFileVersion(`"$fileVersion`")>"
    "<Assembly: AssemblyInformationalVersion(`"$informationalVersion`")>"
)
foreach ($fragment in $expectedAssemblyFragments) {
    if (-not $versionAssemblyInfo.Contains($fragment)) {
        $failures.Add("Generated VersionAssemblyInfo is missing: $fragment")
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

Write-Host "Metadata verified: C3 $informationalVersion, channel $releaseChannel, legacy writer $catalogueFormatVersion."
