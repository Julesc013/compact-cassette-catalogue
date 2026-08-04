[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$branchContract = & (Join-Path $PSScriptRoot 'get-branch-contract.ps1') `
    -RepositoryRoot $repositoryRoot
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
    if (@('alpha', 'beta', 'stable') -cnotcontains $releaseChannel) {
        $failures.Add("update feed channel is not supported: '$releaseChannel'.")
    }
    $expectedFeedBranch = if ($releaseChannel -ceq 'alpha') {
        [string]$branchContract.CurrentIntegration
    }
    else {
        [string]$branchContract.CurrentQualified
    }
    $expectedFeedUrl =
        "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/" +
        "$expectedFeedBranch/release/feeds/$releaseChannel/release.json"

    if ($updateFeedUri.Scheme -cne 'https') {
        $failures.Add('update feed URL must use HTTPS.')
    }
    if ($updateFeedUri.Host -cne 'raw.githubusercontent.com') {
        $failures.Add('update feed URL must use the approved raw.githubusercontent.com host.')
    }
    if ($updateFeedUri.Port -ne 443 -or -not $updateFeedUri.IsDefaultPort) {
        $failures.Add('update feed URL must use the default HTTPS port.')
    }
    if (-not [string]::IsNullOrEmpty($updateFeedUri.UserInfo) -or
        -not [string]::IsNullOrEmpty($updateFeedUri.Query) -or
        -not [string]::IsNullOrEmpty($updateFeedUri.Fragment)) {
        $failures.Add(
            'update feed URL must not contain credentials, a query, or a fragment.')
    }
    if ($updateFeedUri.OriginalString -cne $expectedFeedUrl) {
        $failures.Add("update feed URL must be exactly '$expectedFeedUrl'.")
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

$channelManifestPath = & (Join-Path $PSScriptRoot 'get-update-manifest-path.ps1') `
    -Identity $identity `
    -RepositoryRoot $repositoryRoot
$updateFeedSchemaPath = Join-Path $repositoryRoot 'spec\update-feed\v1\release.schema.json'
& (Join-Path $PSScriptRoot 'validate-json-document.ps1') `
    -SchemaPath $updateFeedSchemaPath `
    -DocumentPath $channelManifestPath `
    -MaximumBytes (32 * 1024) | Out-Null
$channelManifest = Get-Content -LiteralPath $channelManifestPath -Raw | ConvertFrom-Json
$expectedManifestProperties = @(
    'schemaVersion',
    'product',
    'productId',
    'channel',
    'version',
    'stage',
    'informationalVersion',
    'releaseDate',
    'catalogueWriteFormat',
    'published',
    'releaseUrl',
    'checksumManifest',
    'packages'
)
$actualManifestProperties = @(
    $channelManifest.PSObject.Properties | ForEach-Object { $_.Name }
)
foreach ($propertyName in $expectedManifestProperties) {
    if ($actualManifestProperties -cnotcontains $propertyName) {
        $failures.Add("release manifest is missing property '$propertyName'.")
    }
}
foreach ($propertyName in $actualManifestProperties) {
    if ($expectedManifestProperties -cnotcontains $propertyName) {
        $failures.Add("release manifest contains unsupported property '$propertyName'.")
    }
}
Assert-Equal 'release manifest schema' '1' ([string]$channelManifest.schemaVersion)
Assert-Equal 'release manifest product' 'Compact Cassette Catalogue' ([string]$channelManifest.product)
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
if ($null -ne $channelManifest.releaseUrl) {
    $failures.Add('an unpublished development feed must have a null releaseUrl.')
}
if ($null -ne $channelManifest.checksumManifest) {
    $failures.Add('an unpublished development feed must have a null checksumManifest.')
}
if ($channelManifest.packages -isnot [Array] -or @($channelManifest.packages).Count -ne 0) {
    $failures.Add('an unpublished development feed must have an empty packages array.')
}

$legacyRootPath = Join-Path $repositoryRoot 'VERSION'
$legacyChannelPath = Join-Path $repositoryRoot 'release\feeds\legacy-1x\VERSION'
$feedRoot = Join-Path $repositoryRoot 'release\feeds'
$legacyChannelFullPath = [IO.Path]::GetFullPath($legacyChannelPath)
foreach ($feedVersion in Get-ChildItem -LiteralPath $feedRoot -Recurse -File -Filter 'VERSION') {
    if (-not [string]::Equals(
            $feedVersion.FullName,
            $legacyChannelFullPath,
            [StringComparison]::OrdinalIgnoreCase)) {
        $failures.Add(
            "2.x channel feeds must use release.json; remove '$($feedVersion.FullName)'.")
    }
}
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

$updateBranchesPath = Join-Path $repositoryRoot `
    'src\Shared\Generated\UpdateBranches.g.vb'
$updateBranches = Get-Content -LiteralPath $updateBranchesPath -Raw
$expectedBranchFragments = @(
    "Public Const AlphaFeedBranch As String = `"$($branchContract.CurrentIntegration)`""
    "Public Const PublishedFeedBranch As String = `"$($branchContract.CurrentQualified)`""
)
foreach ($fragment in $expectedBranchFragments) {
    if (-not $updateBranches.Contains($fragment)) {
        $failures.Add("Generated UpdateBranches is missing: $fragment")
    }
}

$csharpUpdateBranchesPath = Join-Path $repositoryRoot `
    'src\Shared\Generated\UpdateBranches.g.cs'
$csharpUpdateBranches = Get-Content -LiteralPath $csharpUpdateBranchesPath -Raw
$expectedCSharpBranchFragments = @(
    "public const string AlphaFeedBranch = `"$($branchContract.CurrentIntegration)`";"
    "public const string PublishedFeedBranch = `"$($branchContract.CurrentQualified)`";"
)
foreach ($fragment in $expectedCSharpBranchFragments) {
    if (-not $csharpUpdateBranches.Contains($fragment)) {
        $failures.Add("Generated C# UpdateBranches is missing: $fragment")
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
