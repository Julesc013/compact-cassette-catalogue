[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$sourceRoot = Join-Path $repositoryRoot 'src\C3.WinForms'
$net40Path = Join-Path $sourceRoot 'C3.WinForms.Net40.vbproj'
$net48Path = Join-Path $sourceRoot 'C3.WinForms.Net48.vbproj'
[xml]$net40 = Get-Content -LiteralPath $net40Path -Raw
[xml]$net48 = Get-Content -LiteralPath $net48Path -Raw

function Get-NamespaceManager {
    param([xml]$Document)
    $manager = New-Object Xml.XmlNamespaceManager($Document.NameTable)
    $manager.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
    Write-Output -NoEnumerate $manager
}

function Get-PropertyValue {
    param(
        [xml]$Document,
        [string]$PropertyName
    )
    $manager = Get-NamespaceManager $Document
    $node = $Document.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:$PropertyName", $manager)
    if ($null -eq $node) {
        return $null
    }
    return [string]$node.InnerText
}

function Get-Includes {
    param(
        [xml]$Document,
        [string]$ItemName
    )
    $manager = Get-NamespaceManager $Document
    return @($Document.SelectNodes("/msb:Project/msb:ItemGroup/msb:$ItemName", $manager) |
        ForEach-Object { [string]$_.Include })
}

function Normalize-RelativePath {
    param([string]$Path)
    return $Path.Replace('/', '\').TrimStart([char[]]'.\')
}

$failures = New-Object Collections.Generic.List[String]

foreach ($propertyName in @('RootNamespace', 'AssemblyName', 'StartupObject', 'OutputType', 'MyType')) {
    $net40Value = Get-PropertyValue $net40 $propertyName
    $net48Value = Get-PropertyValue $net48 $propertyName
    if ($net40Value -cne $net48Value) {
        $failures.Add("$propertyName differs: net40='$net40Value', net48='$net48Value'")
    }
}

if ((Get-PropertyValue $net40 'TargetFrameworkVersion') -cne 'v4.0') {
    $failures.Add('Net40 project must target v4.0.')
}
if ((Get-PropertyValue $net48 'TargetFrameworkVersion') -cne 'v4.8') {
    $failures.Add('Net48 project must target v4.8.')
}

$physicalSources = @(Get-ChildItem -LiteralPath $sourceRoot -Recurse -File -Filter '*.vb' |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
    ForEach-Object { Normalize-RelativePath $_.FullName.Substring($sourceRoot.Length + 1) } |
    Sort-Object -Unique)
$net40Sources = @(Get-Includes $net40 'Compile' |
    Where-Object { -not $_.StartsWith('..', [StringComparison]::Ordinal) } |
    ForEach-Object { Normalize-RelativePath $_ } |
    Sort-Object -Unique)
$sourceDifference = @(Compare-Object $physicalSources $net40Sources)
if ($sourceDifference.Count -gt 0) {
    $failures.Add("Net40 compile list differs from physical sources:`n" + ($sourceDifference | Out-String))
}

$net48Compile = @(Get-Includes $net48 'Compile')
if ($net48Compile -notcontains '**\*.vb') {
    $failures.Add('Net48 project must compile the shared **\*.vb source tree.')
}

$net40LinkedSources = @(Get-Includes $net40 'Compile' |
    Where-Object { $_.StartsWith('..', [StringComparison]::Ordinal) } |
    Sort-Object -Unique)
$net48LinkedSources = @(Get-Includes $net48 'Compile' |
    Where-Object { $_.StartsWith('..', [StringComparison]::Ordinal) } |
    Sort-Object -Unique)
$linkedSourceDifference = @(Compare-Object $net40LinkedSources $net48LinkedSources)
if ($linkedSourceDifference.Count -gt 0) {
    $failures.Add("Linked WinForms sources differ between lanes:`n" + ($linkedSourceDifference | Out-String))
}

$physicalResources = @(Get-ChildItem -LiteralPath $sourceRoot -Recurse -File -Filter '*.resx' |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
    ForEach-Object { Normalize-RelativePath $_.FullName.Substring($sourceRoot.Length + 1) } |
    Sort-Object -Unique)
$net40Resources = @(Get-Includes $net40 'EmbeddedResource' |
    ForEach-Object { Normalize-RelativePath $_ } |
    Sort-Object -Unique)
$resourceDifference = @(Compare-Object $physicalResources $net40Resources)
if ($resourceDifference.Count -gt 0) {
    $failures.Add("Net40 resource list differs from physical resources:`n" + ($resourceDifference | Out-String))
}

$net48Resources = @(Get-Includes $net48 'EmbeddedResource')
if ($net48Resources -notcontains '**\*.resx') {
    $failures.Add('Net48 project must include the shared **\*.resx resource tree.')
}

if ($failures.Count -gt 0) {
    throw ("WinForms project parity failed:`n - " + ($failures -join "`n - "))
}

Write-Host "WinForms project parity verified for $($physicalSources.Count) source and $($physicalResources.Count) resource files."
