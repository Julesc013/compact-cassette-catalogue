[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$forbiddenRoots = @('src', 'SetupShared')
foreach ($relativeRoot in $forbiddenRoots) {
    $path = Join-Path $repositoryRoot $relativeRoot
    if (Test-Path -LiteralPath $path) {
        throw "C3 1.x prohibits the hybrid production root '$relativeRoot/'."
    }
}

$contractPath = Join-Path $PSScriptRoot 'setup-shared-source-contract.json'
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json
if ([int]$contract.schemaVersion -ne 1) { throw 'Unsupported shared setup source contract schema.' }
if ([string]$contract.canonicalRoot -cne 'Compact Cassette Catalogue Installer/Shared') {
    throw 'Shared setup source contract does not use the ratified Installer-owned root.'
}

$sharedRoot = Join-Path $repositoryRoot ([string]$contract.canonicalRoot)
if (-not (Test-Path -LiteralPath $sharedRoot -PathType Container)) {
    throw "Canonical shared setup source root is missing: $sharedRoot"
}

$expectedFiles = @($contract.files | Sort-Object name)
$actualFiles = @(Get-ChildItem -LiteralPath $sharedRoot -Filter '*.vb' -File | Sort-Object Name)
if ($expectedFiles.Count -ne 13 -or $actualFiles.Count -ne $expectedFiles.Count) {
    throw "Shared setup source closure requires exactly 13 files; expected $($expectedFiles.Count), found $($actualFiles.Count)."
}

for ($index = 0; $index -lt $expectedFiles.Count; $index++) {
    $expected = $expectedFiles[$index]
    $actual = $actualFiles[$index]
    if ([string]$expected.name -cne $actual.Name) {
        throw "Shared setup source file mismatch at index ${index}: '$($actual.Name)'."
    }
    $actualHash = (Get-FileHash -LiteralPath $actual.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($actualHash -cne [string]$expected.sha256) {
        throw "Shared setup source changed during topology normalization: '$($actual.Name)'."
    }
}

$projects = @(
    [PSCustomObject]@{
        Name = 'Installer'
        Path = 'Compact Cassette Catalogue Installer/Compact Cassette Catalogue Installer.vbproj'
        IncludePrefix = 'Shared\'
    },
    [PSCustomObject]@{
        Name = 'Uninstaller'
        Path = 'Compact Cassette Catalogue Uninstaller/Compact Cassette Catalogue Uninstaller.vbproj'
        IncludePrefix = '..\Compact Cassette Catalogue Installer\Shared\'
    },
    [PSCustomObject]@{
        Name = 'Setup characterization'
        Path = 'tests/C3.Setup.Characterization/C3.Setup.Characterization.vbproj'
        IncludePrefix = '..\..\Compact Cassette Catalogue Installer\Shared\'
    }
)

foreach ($project in $projects) {
    $projectPath = Join-Path $repositoryRoot $project.Path
    [xml]$xml = Get-Content -LiteralPath $projectPath -Raw
    $namespace = New-Object Xml.XmlNamespaceManager($xml.NameTable)
    $namespace.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
    $linked = @($xml.SelectNodes('//msb:Compile', $namespace) |
        Where-Object {
            $linkNode = $_.SelectSingleNode('msb:Link', $namespace)
            $null -ne $linkNode -and [string]$linkNode.InnerText -like 'Shared\*'
        } |
        Sort-Object { [string]$_.SelectSingleNode('msb:Link', $namespace).InnerText })
    if ($linked.Count -ne $expectedFiles.Count) {
        throw "$($project.Name) must compile each of the 13 shared setup files exactly once."
    }
    for ($index = 0; $index -lt $expectedFiles.Count; $index++) {
        $name = [string]$expectedFiles[$index].name
        $entry = $linked[$index]
        $link = [string]$entry.SelectSingleNode('msb:Link', $namespace).InnerText
        if ($link -cne "Shared\$name" -or
                [string]$entry.Include -cne "$($project.IncludePrefix)$name") {
            throw "$($project.Name) has a noncanonical shared Compile item for '$name'."
        }
        $resolvedInclude = [IO.Path]::GetFullPath((Join-Path (Split-Path -Parent $projectPath) ([string]$entry.Include)))
        $expectedPath = [IO.Path]::GetFullPath((Join-Path $sharedRoot $name))
        if ($resolvedInclude -cne $expectedPath) {
            throw "$($project.Name) does not resolve '$name' to the canonical physical source."
        }
    }
}

$trackedVb = @(& git -C $repositoryRoot ls-files '*.vb')
if ($LASTEXITCODE -ne 0) { throw 'Could not inventory tracked VB.NET source.' }
$allowedPrefixes = @(
    'Compact Cassette Catalogue/',
    'Compact Cassette Catalogue Installer/',
    'Compact Cassette Catalogue Uninstaller/',
    'tests/'
)
$unexpected = @($trackedVb | Where-Object {
        $path = $_
        -not ($allowedPrefixes | Where-Object { $path.StartsWith($_, [StringComparison]::Ordinal) })
    })
if ($unexpected.Count -ne 0) {
    throw "Tracked VB.NET source exists outside the three product roots or tests: $($unexpected -join ', ')"
}

Write-Host "C3 1.x source topology verified: three product roots, one Installer-owned 13-file shared setup engine, and no root src/ or SetupShared/."
