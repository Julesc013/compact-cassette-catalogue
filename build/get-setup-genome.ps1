[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$RepositoryRoot = [IO.Path]::GetFullPath($RepositoryRoot).TrimEnd('\')
$setupRoots = @(
    'Compact Cassette Catalogue Installer',
    'Compact Cassette Catalogue Uninstaller'
)

function Get-RelativePath {
    param([string]$FullPath)
    return $FullPath.Substring($RepositoryRoot.Length + 1).Replace('\', '/')
}

function Get-ProjectIdentity {
    param([string]$RelativeProjectPath)
    $fullPath = Join-Path $RepositoryRoot $RelativeProjectPath
    [xml]$project = Get-Content -LiteralPath $fullPath -Raw
    $namespace = New-Object Xml.XmlNamespaceManager($project.NameTable)
    $namespace.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
    $properties = $project.SelectSingleNode('/msb:Project/msb:PropertyGroup[not(@Condition)]', $namespace)
    return [ordered]@{
        project = $RelativeProjectPath.Replace('\', '/')
        projectGuid = [string]$properties.ProjectGuid
        outputType = [string]$properties.OutputType
        startupObject = [string]$properties.StartupObject
        rootNamespace = [string]$properties.RootNamespace
        assemblyName = [string]$properties.AssemblyName
        applicationIcon = [string]$project.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:ApplicationIcon', $namespace).'#text'
        applicationManifest = [string]$project.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:ApplicationManifest', $namespace).'#text'
    }
}

$identities = @(
    Get-ProjectIdentity 'Compact Cassette Catalogue Installer\Compact Cassette Catalogue Installer.vbproj'
    Get-ProjectIdentity 'Compact Cassette Catalogue Uninstaller\Compact Cassette Catalogue Uninstaller.vbproj'
)

$formClasses = New-Object Collections.Generic.List[Object]
$controls = New-Object Collections.Generic.List[Object]
$resourceKeys = New-Object Collections.Generic.List[Object]
$artwork = New-Object Collections.Generic.List[Object]
$applicationManifests = New-Object Collections.Generic.List[Object]

foreach ($setupRoot in $setupRoots) {
    $fullRoot = Join-Path $RepositoryRoot $setupRoot
    $manifest = Get-Item -LiteralPath (Join-Path $fullRoot 'app.manifest')
    $applicationManifests.Add([ordered]@{
            path = Get-RelativePath $manifest.FullName
            length = [long]$manifest.Length
            sha256 = (Get-FileHash -LiteralPath $manifest.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        })
    foreach ($designer in @(Get-ChildItem -LiteralPath $fullRoot -Filter 'frm*.Designer.vb' -File | Sort-Object Name)) {
        $relativePath = Get-RelativePath $designer.FullName
        $text = Get-Content -LiteralPath $designer.FullName -Raw
        $classMatch = [regex]::Match($text, '(?m)^\s*Partial\s+Class\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*$')
        if (-not $classMatch.Success) {
            throw "Could not find form class in '$relativePath'."
        }
        $formClasses.Add([ordered]@{ path = $relativePath; name = $classMatch.Groups['name'].Value })
        $controlNames = @([regex]::Matches($text, '(?m)^\s*Friend\s+WithEvents\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s+As\s+') |
            ForEach-Object { $_.Groups['name'].Value } | Sort-Object -Unique)
        $controls.Add([ordered]@{ path = $relativePath; names = $controlNames })
    }

    foreach ($resx in @(Get-ChildItem -LiteralPath $fullRoot -Filter '*.resx' -File -Recurse | Sort-Object FullName)) {
        [xml]$resource = Get-Content -LiteralPath $resx.FullName -Raw
        $keys = @($resource.root.data | ForEach-Object { [string]$_.name } | Sort-Object -Unique)
        $resourceKeys.Add([ordered]@{ path = (Get-RelativePath $resx.FullName); keys = $keys })
    }

    foreach ($asset in @(Get-ChildItem -LiteralPath $fullRoot -File -Recurse |
            Where-Object { $_.Extension -match '^\.(?:ico|jpg|jpeg|png)$' } |
            Sort-Object FullName)) {
        $artwork.Add([ordered]@{
                path = Get-RelativePath $asset.FullName
                length = [long]$asset.Length
                sha256 = (Get-FileHash -LiteralPath $asset.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
            })
    }
}

[PSCustomObject][ordered]@{
    identity = $identities
    applicationManifests = $applicationManifests.ToArray()
    formClasses = $formClasses.ToArray()
    controls = $controls.ToArray()
    resourceKeys = $resourceKeys.ToArray()
    artwork = $artwork.ToArray()
}
