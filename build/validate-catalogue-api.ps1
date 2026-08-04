[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$AssemblyPath,
    [string]$BaselinePath,
    [string]$NamespacePrefix,
    [switch]$WriteBaseline
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($AssemblyPath)) {
    $AssemblyPath = Join-Path $repositoryRoot (
        "artifacts\bin\libraries\net40\$Configuration\C3.Catalogue.dll")
}
if ([string]::IsNullOrWhiteSpace($BaselinePath)) {
    $BaselinePath = Join-Path $repositoryRoot 'spec\catalogue-api\v1\public-api.txt'
}
$AssemblyPath = [IO.Path]::GetFullPath($AssemblyPath)
$BaselinePath = [IO.Path]::GetFullPath($BaselinePath)

function Get-TypeName {
    param([Type]$Type)

    if ($Type.IsByRef) {
        return (Get-TypeName $Type.GetElementType()) + '&'
    }
    if ($Type.IsArray) {
        return (Get-TypeName $Type.GetElementType()) + '[]'
    }
    if ($Type.IsGenericType) {
        $definitionName = $Type.GetGenericTypeDefinition().FullName
        $tick = $definitionName.IndexOf('`')
        if ($tick -ge 0) {
            $definitionName = $definitionName.Substring(0, $tick)
        }
        $arguments = @($Type.GetGenericArguments() | ForEach-Object { Get-TypeName $_ })
        return $definitionName + '<' + ($arguments -join ',') + '>'
    }
    if ($Type.IsGenericParameter) {
        return $Type.Name
    }
    return $Type.FullName
}

function Get-Parameters {
    param([Reflection.ParameterInfo[]]$Parameters)

    return (@($Parameters | ForEach-Object {
                (Get-TypeName $_.ParameterType) + ' ' + $_.Name
            }) -join ',')
}

if (-not (Test-Path -LiteralPath $AssemblyPath -PathType Leaf)) {
    throw "Catalogue assembly is missing: $AssemblyPath"
}

# Load a byte copy so reflection does not lock the build output for the
# remainder of a long-running parent verification process.
$assemblyBytes = [IO.File]::ReadAllBytes($AssemblyPath)
$assembly = [Reflection.Assembly]::Load($assemblyBytes)
$lines = New-Object Collections.Generic.List[String]
foreach ($type in @($assembly.GetExportedTypes() | Sort-Object FullName)) {
    $isInterface = (($type.Attributes -band
            [Reflection.TypeAttributes]::Interface) -ne 0)
    $kind = if ($type.IsEnum) {
        'enum'
    }
    elseif ($isInterface) {
        'interface'
    }
    elseif ($type.IsValueType) {
        'struct'
    }
    else {
        'class'
    }
    $modifiers = @()
    if ($type.IsAbstract -and $type.IsSealed) { $modifiers += 'static' }
    elseif ($type.IsAbstract) { $modifiers += 'abstract' }
    elseif ($type.IsSealed) { $modifiers += 'sealed' }
    $lines.Add("type|$($type.FullName)|$kind|$($modifiers -join ',')")

    if ($type.IsEnum) {
        foreach ($name in [Enum]::GetNames($type)) {
            $numericValue = [Convert]::ToInt64([Enum]::Parse($type, $name))
            $lines.Add("enum|$($type.FullName)|$name|$numericValue")
        }
        continue
    }

    $declared = [Reflection.BindingFlags]::Public -bor
        [Reflection.BindingFlags]::Instance -bor
        [Reflection.BindingFlags]::Static -bor
        [Reflection.BindingFlags]::DeclaredOnly
    foreach ($constructor in @($type.GetConstructors($declared))) {
        $lines.Add(
            "ctor|$($type.FullName)|$(Get-Parameters $constructor.GetParameters())")
    }
    foreach ($property in @($type.GetProperties($declared))) {
        $access = ''
        if ($null -ne $property.GetGetMethod()) { $access += 'get' }
        if ($null -ne $property.GetSetMethod()) { $access += 'set' }
        $lines.Add(
            "property|$($type.FullName)|$($property.Name)|$(Get-TypeName $property.PropertyType)|$access")
    }
    foreach ($event in @($type.GetEvents($declared))) {
        $lines.Add(
            "event|$($type.FullName)|$($event.Name)|$(Get-TypeName $event.EventHandlerType)")
    }
    foreach ($method in @($type.GetMethods($declared) | Where-Object { -not $_.IsSpecialName })) {
        $scope = if ($method.IsStatic) { 'static' } else { 'instance' }
        $lines.Add(
            "method|$($type.FullName)|$($method.Name)|$scope|$(Get-TypeName $method.ReturnType)|$(Get-Parameters $method.GetParameters())")
    }
}

$actual = @($lines | Sort-Object -CaseSensitive -Unique)
if (-not [string]::IsNullOrWhiteSpace($NamespacePrefix)) {
    $escapedPrefix = [regex]::Escape($NamespacePrefix)
    $actual = @($actual | Where-Object { $_ -match $escapedPrefix })
}
if ($WriteBaseline) {
    if (-not [string]::IsNullOrWhiteSpace($NamespacePrefix)) {
        throw 'A namespace-filtered projection cannot replace the complete baseline.'
    }
    $parent = Split-Path -Parent $BaselinePath
    if (-not (Test-Path -LiteralPath $parent -PathType Container)) {
        [IO.Directory]::CreateDirectory($parent) | Out-Null
    }
    $utf8WithoutBom = New-Object Text.UTF8Encoding($false)
    [IO.File]::WriteAllText($BaselinePath, (($actual -join "`n") + "`n"), $utf8WithoutBom)
    Write-Host "Wrote catalogue public API baseline: $BaselinePath"
    return
}

if (-not (Test-Path -LiteralPath $BaselinePath -PathType Leaf)) {
    throw "Catalogue public API baseline is missing: $BaselinePath"
}
$expected = @(Get-Content -LiteralPath $BaselinePath | Where-Object {
        -not [string]::IsNullOrWhiteSpace($_) -and -not $_.StartsWith('#')
    })
if (-not [string]::IsNullOrWhiteSpace($NamespacePrefix)) {
    $expected = @($expected | Where-Object { $_ -match $escapedPrefix })
}
$difference = @(Compare-Object -ReferenceObject $expected -DifferenceObject $actual -CaseSensitive)
if ($difference.Count -gt 0) {
    throw ("Catalogue public API differs from the frozen VB oracle:`n" +
        (($difference | Format-Table -AutoSize | Out-String).TrimEnd()))
}

$scope = if ([string]::IsNullOrWhiteSpace($NamespacePrefix)) {
    'complete assembly'
}
else {
    $NamespacePrefix
}
Write-Host "Catalogue public API matches the frozen VB oracle for ${scope}: $($actual.Count) signatures."
