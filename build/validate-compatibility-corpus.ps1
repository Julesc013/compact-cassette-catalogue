[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$CorpusPath,
    [string]$SchemaPath,
    [switch]$SkipGitEvidence
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRootPath = [IO.Path]::GetFullPath($RepositoryRoot)
if ([string]::IsNullOrWhiteSpace($CorpusPath)) {
    $CorpusPath = Join-Path $repositoryRootPath 'fixtures\compatibility\1x\corpus.v1.json'
}
if ([string]::IsNullOrWhiteSpace($SchemaPath)) {
    $SchemaPath = Join-Path $repositoryRootPath `
        'spec\compatibility-corpus\v1\corpus.schema.json'
}

& (Join-Path $PSScriptRoot 'validate-json-document.ps1') `
    -SchemaPath $SchemaPath `
    -DocumentPath $CorpusPath `
    -MaximumBytes (1MB)

$corpus = Get-Content -LiteralPath $CorpusPath -Raw | ConvertFrom-Json

function Assert-UniqueValues {
    param(
        [object[]]$Values,
        [string]$Description
    )
    $duplicates = @(
        $Values |
            Group-Object |
            Where-Object { $_.Count -ne 1 } |
            ForEach-Object { $_.Name })
    if ($duplicates.Count -ne 0) {
        throw "$Description contains duplicate value(s): $($duplicates -join ', ')."
    }
}

function Get-Sha256 {
    param([string]$Path)
    return (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()
}

$settingsProfileIds = @($corpus.settingsProfiles | ForEach-Object { [string]$_.id })
$updateProfileIds = @($corpus.updateProfiles | ForEach-Object { [string]$_.id })
$releaseTags = @($corpus.releases | ForEach-Object { [string]$_.tag })
$supportedTags = @($corpus.supportPolicy.supportedProducerTags | ForEach-Object { [string]$_ })
$inventoryTags = @($corpus.supportPolicy.inventoryOnlyProducerTags | ForEach-Object { [string]$_ })

Assert-UniqueValues $settingsProfileIds 'Settings profiles'
Assert-UniqueValues $updateProfileIds 'Update profiles'
Assert-UniqueValues $releaseTags 'Release inventory'
Assert-UniqueValues ($supportedTags + $inventoryTags) 'Support policy'

$policyTags = @(($supportedTags + $inventoryTags) | Sort-Object)
$releaseTagsSorted = @($releaseTags | Sort-Object)
if (($policyTags -join "`n") -cne ($releaseTagsSorted -join "`n")) {
    throw 'Support-policy tags must partition the complete release inventory.'
}

$formatVersions = @($corpus.formatProvenance | ForEach-Object { [string]$_.format })
Assert-UniqueValues $formatVersions 'Format provenance'
$requiredFormats = @('1.0.0', '1.0.1', '1.0.2', '1.1.0')
if ((@($formatVersions | Sort-Object) -join "`n") -cne `
        (@($requiredFormats | Sort-Object) -join "`n")) {
    throw 'Format provenance must contain exactly 1.0.0, 1.0.1, 1.0.2, and 1.1.0.'
}

$gitDirectory = Join-Path $repositoryRootPath '.git'
$validateGit = -not $SkipGitEvidence -and (Test-Path -LiteralPath $gitDirectory)
$fixturePaths = New-Object 'Collections.Generic.List[string]'
$settingsFixturePaths = New-Object 'Collections.Generic.List[string]'

foreach ($profile in $corpus.settingsProfiles) {
    foreach ($fixture in $profile.fixtures) {
        $relativePath = ([string]$fixture.path).Replace('/', [IO.Path]::DirectorySeparatorChar)
        $fullPath = [IO.Path]::GetFullPath((Join-Path $repositoryRootPath $relativePath))
        $settingsRoot = [IO.Path]::GetFullPath(
            (Join-Path $repositoryRootPath 'fixtures\settings\legacy'))
        if (-not $fullPath.StartsWith(
                $settingsRoot + [IO.Path]::DirectorySeparatorChar,
                [StringComparison]::OrdinalIgnoreCase)) {
            throw "$($profile.id) fixture escapes the legacy settings corpus."
        }
        if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
            throw "$($profile.id) fixture is missing: $relativePath"
        }
        $file = Get-Item -LiteralPath $fullPath
        if ($file.Length -ne [long]$fixture.size -or
                (Get-Sha256 $fullPath) -cne [string]$fixture.sha256) {
            throw "$($profile.id) settings fixture bytes disagree with the corpus."
        }
        if ($settingsFixturePaths.Contains($fullPath)) {
            throw "Settings fixture is owned by more than one profile: $relativePath"
        }
        $settingsFixturePaths.Add($fullPath)
    }
}

foreach ($release in $corpus.releases) {
    $tag = [string]$release.tag
    $support = [string]$release.support
    $expectedSupport = if ($supportedTags -ccontains $tag) { 'supported' } else { 'inventory-only' }
    if ($support -cne $expectedSupport) {
        throw "$tag support '$support' disagrees with the support policy."
    }
    if ($settingsProfileIds -cnotcontains [string]$release.settingsProfile) {
        throw "$tag references an unknown settings profile."
    }
    if ($updateProfileIds -cnotcontains [string]$release.updateProfile) {
        throw "$tag references an unknown update profile."
    }
    if ($support -ceq 'supported' -and @($release.fixtures).Count -eq 0) {
        throw "$tag is supported but has no provenance-bearing fixture."
    }
    if ($support -ceq 'inventory-only' -and @($release.fixtures).Count -ne 0) {
        throw "$tag is inventory-only and must not imply fixture qualification."
    }

    $expectedReleaseUrl =
        "https://github.com/Julesc013/compact-cassette-catalogue/releases/tag/$tag"
    if ([string]$release.releaseUrl -cne $expectedReleaseUrl) {
        throw "$tag has a noncanonical release URL."
    }

    if ($validateGit) {
        Push-Location $repositoryRootPath
        try {
            $resolvedCommit = (& git rev-parse "$tag^{commit}" 2>$null).Trim()
            if ($LASTEXITCODE -ne 0 -or $resolvedCommit -cne [string]$release.sourceCommit) {
                throw "$tag does not resolve to its recorded source commit."
            }
        }
        finally {
            Pop-Location
            $global:LASTEXITCODE = 0
        }
    }

    $artifactNames = @($release.artifacts | ForEach-Object { [string]$_.name })
    Assert-UniqueValues $artifactNames "$tag artifacts"
    foreach ($artifact in $release.artifacts) {
        $expectedArtifactUrl =
            "https://github.com/Julesc013/compact-cassette-catalogue/releases/download/" +
            "$tag/$([string]$artifact.name)"
        if ([string]$artifact.url -cne $expectedArtifactUrl) {
            throw "$tag artifact '$($artifact.name)' has a noncanonical download URL."
        }
        if (@($release.platforms) -cnotcontains [string]$artifact.architecture) {
            throw "$tag artifact '$($artifact.name)' is outside the declared platform set."
        }
    }

    foreach ($fixture in $release.fixtures) {
        $relativePath = ([string]$fixture.path).Replace('/', [IO.Path]::DirectorySeparatorChar)
        $fullPath = [IO.Path]::GetFullPath((Join-Path $repositoryRootPath $relativePath))
        $fixtureRoot = [IO.Path]::GetFullPath(
            (Join-Path $repositoryRootPath 'fixtures\catalogues\v1.1.0\historical'))
        if (-not $fullPath.StartsWith($fixtureRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
            throw "$tag fixture escapes the historical catalogue corpus."
        }
        if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
            throw "$tag fixture is missing: $relativePath"
        }
        $file = Get-Item -LiteralPath $fullPath
        if ($file.Length -ne [long]$fixture.size) {
            throw "$tag fixture length does not match the corpus."
        }
        if ((Get-Sha256 $fullPath) -cne [string]$fixture.sha256) {
            throw "$tag fixture SHA-256 does not match the corpus."
        }
        if ($fixturePaths.Contains($fullPath)) {
            throw "Fixture is owned by more than one producer: $relativePath"
        }
        $fixturePaths.Add($fullPath)

        $settings = New-Object Xml.XmlReaderSettings
        $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
        $settings.XmlResolver = $null
        $document = New-Object Xml.XmlDocument
        $document.XmlResolver = $null
        $reader = [Xml.XmlReader]::Create($fullPath, $settings)
        try {
            $document.Load($reader)
        }
        finally {
            $reader.Dispose()
        }
        if ($document.DocumentElement.LocalName -cne 'Catalogue' -or
                -not [string]::IsNullOrEmpty($document.DocumentElement.NamespaceURI)) {
            throw "$tag fixture does not use the unqualified Catalogue root."
        }
        $fileVersion = $document.SelectSingleNode(
            "/Catalogue/Information[Information='File Version']/Value")
        if ($null -eq $fileVersion -or $fileVersion.InnerText -cne '1.1.0') {
            throw "$tag fixture does not identify catalogue format 1.1.0."
        }
        $productVersion = $document.SelectSingleNode(
            "/Catalogue/Information[Information='Program Version']/Value")
        if ($null -eq $productVersion -or
                $productVersion.InnerText -cne [string]$release.reportedProductVersion) {
            throw "$tag fixture product identity disagrees with the release inventory."
        }
    }
}

Write-Host (
    "Compatibility corpus verified: $($releaseTags.Count) public 1.x releases, " +
    "$($supportedTags.Count) supported producer(s), $($formatVersions.Count) observed format(s).")
