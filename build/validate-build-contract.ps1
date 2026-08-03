[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$commonPropsPath = Join-Path $PSScriptRoot 'C3.Common.props'
$expectedPropsPath = [IO.Path]::GetFullPath($commonPropsPath)
$failures = New-Object Collections.Generic.List[String]

if (-not (Test-Path -LiteralPath $commonPropsPath -PathType Leaf)) {
    throw "Shared compiler contract is missing: $commonPropsPath"
}

[xml]$commonProps = Get-Content -LiteralPath $commonPropsPath -Raw
$commonNamespace = New-Object Xml.XmlNamespaceManager($commonProps.NameTable)
$commonNamespace.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
$deterministic = $commonProps.SelectSingleNode(
    '/msb:Project/msb:PropertyGroup/msb:Deterministic',
    $commonNamespace)
$pathMap = $commonProps.SelectSingleNode(
    '/msb:Project/msb:PropertyGroup/msb:PathMap',
    $commonNamespace)
if ($null -eq $deterministic -or $deterministic.InnerText -cne 'true') {
    $failures.Add('C3.Common.props must enable deterministic compilation.')
}
if ($null -eq $pathMap -or
    -not $pathMap.InnerText.Contains('$(C3RepositoryRoot)=/_/')) {
    $failures.Add('C3.Common.props must map the repository root out of compiler output.')
}

$projectRoots = @(
    (Join-Path $repositoryRoot 'src'),
    (Join-Path $repositoryRoot 'tests')
)
$projects = @(Get-ChildItem -LiteralPath $projectRoots -Recurse -File |
    Where-Object { $_.Extension -in @('.vbproj', '.csproj') } |
    Sort-Object FullName)
if ($projects.Count -eq 0) {
    $failures.Add('No managed projects were found under src or tests.')
}

foreach ($project in $projects) {
    [xml]$document = Get-Content -LiteralPath $project.FullName -Raw
    $namespace = New-Object Xml.XmlNamespaceManager($document.NameTable)
    $namespace.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
    $imports = @($document.SelectNodes('/msb:Project/msb:Import', $namespace))
    $matchingImports = New-Object Collections.Generic.List[Xml.XmlNode]

    foreach ($import in $imports) {
        $importPath = [string]$import.Project
        if ([string]::IsNullOrWhiteSpace($importPath) -or $importPath.Contains('$(')) {
            continue
        }
        $resolvedPath = [IO.Path]::GetFullPath((Join-Path $project.DirectoryName $importPath))
        if ($resolvedPath.Equals($expectedPropsPath, [StringComparison]::OrdinalIgnoreCase)) {
            $matchingImports.Add($import)
        }
    }

    $relativeProject = $project.FullName.Substring($repositoryRoot.Length + 1)
    if ($matchingImports.Count -ne 1) {
        $failures.Add("$relativeProject must import build/C3.Common.props exactly once.")
    }
    else {
        $conditionAttribute = $matchingImports[0].Attributes['Condition']
        if ($null -ne $conditionAttribute -and
            -not [string]::IsNullOrWhiteSpace($conditionAttribute.Value)) {
            $failures.Add("$relativeProject must not conditionally import build/C3.Common.props.")
        }
    }

    foreach ($propertyName in @('Deterministic', 'PathMap')) {
        $localProperty = $document.SelectSingleNode(
            "/msb:Project/msb:PropertyGroup/msb:$propertyName",
            $namespace)
        if ($null -ne $localProperty) {
            $failures.Add("$relativeProject duplicates shared property $propertyName.")
        }
    }
}

if ($failures.Count -gt 0) {
    throw ("Build-contract validation failed:`n - " + ($failures -join "`n - "))
}

Write-Host "Shared deterministic compiler contract verified for $($projects.Count) managed projects."
