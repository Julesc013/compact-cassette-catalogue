[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$catalogueRoot = Join-Path $repositoryRoot 'src\C3.Catalogue'
$domainRoot = Join-Path $repositoryRoot 'src\C3.Domain'
$infrastructureRoot = Join-Path $repositoryRoot 'src\C3.Infrastructure'
$presentationRoot = Join-Path $repositoryRoot 'src\C3.Presentation.WinForms'
$catalogueProject = Join-Path $catalogueRoot 'C3.Catalogue.csproj'
$infrastructureProject = Join-Path $infrastructureRoot 'C3.Infrastructure.csproj'
$presentationProject = Join-Path $presentationRoot 'C3.Presentation.WinForms.csproj'
$net40Project = Join-Path $repositoryRoot 'src\C3.WinForms\C3.WinForms.Net40.vbproj'
$net48Project = Join-Path $repositoryRoot 'src\C3.WinForms\C3.WinForms.Net48.vbproj'
$toolProject = Join-Path $repositoryRoot 'src\C3.Cli\C3.Cli.csproj'

$failures = New-Object Collections.Generic.List[String]

function Assert-FileDoesNotContain {
    param(
        [string]$Path,
        [string[]]$Patterns,
        [string]$Owner
    )

    $content = Get-Content -LiteralPath $Path -Raw
    foreach ($pattern in $Patterns) {
        if ($content -match $pattern) {
            $failures.Add("$Owner contains forbidden dependency pattern '$pattern' in '$Path'.")
        }
    }
}

foreach ($source in Get-ChildItem -LiteralPath $catalogueRoot -Recurse -File -Filter '*.vb') {
    Assert-FileDoesNotContain $source.FullName @(
        'System\.Data',
        'System\.Windows\.Forms',
        'System\.Xml',
        '\bDataSet\b',
        '\bDataRow\b',
        '\bMy\.Settings\b'
    ) 'C3.Catalogue'
}

foreach ($source in Get-ChildItem -LiteralPath $catalogueRoot -Recurse -File -Filter '*.cs') {
    Assert-FileDoesNotContain $source.FullName @(
        'System\.Data',
        'System\.Windows\.Forms',
        'System\.Xml',
        '\bDataSet\b',
        '\bDataRow\b',
        '\bMy\.Settings\b'
    ) 'C3.Catalogue C# candidate'
}

foreach ($source in Get-ChildItem -LiteralPath $domainRoot -Recurse -File -Filter '*.cs') {
    Assert-FileDoesNotContain $source.FullName @(
        'System\.Data',
        'System\.Windows\.Forms',
        'System\.Xml',
        '\bDataSet\b',
        '\bDataRow\b'
    ) 'C3.Domain'
}

$domainProject = Join-Path $domainRoot 'C3.Domain.csproj'
$domainProjectText = Get-Content -LiteralPath $domainProject -Raw
if ($domainProjectText -match '<ProjectReference') {
    $failures.Add('C3.Domain must not have project references.')
}
if ($domainProjectText -notmatch '<LangVersion>7\.3</LangVersion>') {
    $failures.Add('C3.Domain must compile with the explicit C# 7.3 language contract.')
}

foreach ($source in Get-ChildItem -LiteralPath $infrastructureRoot -Recurse -File -Filter '*.vb') {
    Assert-FileDoesNotContain $source.FullName @(
        'System\.Windows\.Forms',
        '\bMy\.Settings\b',
        '\bMessageBox\b',
        '\bForm\b'
    ) 'C3.Infrastructure'
}

foreach ($source in Get-ChildItem -LiteralPath $presentationRoot -Recurse -File -Filter '*.cs') {
    Assert-FileDoesNotContain $source.FullName @(
        'System\.Data',
        'System\.Xml',
        'System\.IO',
        '\bDataSet\b',
        '\bDataTable\b',
        '\bDataRow\b',
        '\bMy\.Settings\b',
        'C3\.Infrastructure'
    ) 'C3.Presentation.WinForms'
}

$catalogueProjectText = Get-Content -LiteralPath $catalogueProject -Raw
if ($catalogueProjectText -notmatch '<LangVersion>7\.3</LangVersion>') {
    $failures.Add('C3.Catalogue must compile with the explicit C# 7.3 language contract.')
}
if (-not $catalogueProjectText.Contains('..\C3.Domain\C3.Domain.csproj')) {
    $failures.Add('C3.Catalogue must reference the dependency-free C3.Domain substrate.')
}
$catalogueProjectReferenceCount = [regex]::Matches(
    $catalogueProjectText,
    '<ProjectReference\b').Count
if ($catalogueProjectReferenceCount -ne 1) {
    $failures.Add('C3.Catalogue may reference only C3.Domain.')
}

$infrastructureProjectText = Get-Content -LiteralPath $infrastructureProject -Raw
if ($infrastructureProjectText -notmatch '<LangVersion>7\.3</LangVersion>') {
    $failures.Add('C3.Infrastructure must compile with the explicit C# 7.3 language contract.')
}
if (-not $infrastructureProjectText.Contains('..\C3.Catalogue\C3.Catalogue.csproj')) {
    $failures.Add('C3.Infrastructure must reference C3.Catalogue.')
}
if (-not $infrastructureProjectText.Contains('..\C3.Domain\C3.Domain.csproj')) {
    $failures.Add('C3.Infrastructure must explicitly reference Domain types exposed by C3.Catalogue.')
}
$infrastructureReferenceCount = [regex]::Matches(
    $infrastructureProjectText,
    '<ProjectReference\b').Count
if ($infrastructureReferenceCount -ne 2) {
    $failures.Add('C3.Infrastructure may reference only C3.Catalogue and C3.Domain.')
}

$presentationProjectText = Get-Content -LiteralPath $presentationProject -Raw
if ($presentationProjectText -notmatch '<TargetFrameworkVersion>v4\.0</TargetFrameworkVersion>') {
    $failures.Add('C3.Presentation.WinForms must target the shared .NET Framework 4.0 boundary.')
}
if ($presentationProjectText -notmatch '<LangVersion>7\.3</LangVersion>') {
    $failures.Add('C3.Presentation.WinForms must compile with the explicit C# 7.3 language contract.')
}
if (-not $presentationProjectText.Contains('..\C3.Catalogue\C3.Catalogue.csproj')) {
    $failures.Add('C3.Presentation.WinForms must reference C3.Catalogue.')
}
if (-not $presentationProjectText.Contains('..\C3.Domain\C3.Domain.csproj')) {
    $failures.Add('C3.Presentation.WinForms must reference C3.Domain command/value contracts.')
}
if ($presentationProjectText.Contains('C3.Infrastructure')) {
    $failures.Add('C3.Presentation.WinForms must not reference C3.Infrastructure.')
}
if ([regex]::Matches($presentationProjectText, '<ProjectReference\b').Count -ne 2) {
    $failures.Add('C3.Presentation.WinForms may reference only C3.Catalogue and C3.Domain.')
}

foreach ($appProject in @($net40Project, $net48Project)) {
    $appProjectText = Get-Content -LiteralPath $appProject -Raw
    if (-not $appProjectText.Contains('..\C3.Catalogue\C3.Catalogue.csproj')) {
        $failures.Add("WinForms project '$appProject' does not reference C3.Catalogue.")
    }
    if (-not $appProjectText.Contains('..\C3.Infrastructure\C3.Infrastructure.csproj')) {
        $failures.Add("WinForms project '$appProject' does not reference C3.Infrastructure.")
    }
}

$toolProjectText = Get-Content -LiteralPath $toolProject -Raw
if ($toolProjectText -notmatch '<LangVersion>7\.3</LangVersion>') {
    $failures.Add('C3.Cli must compile with the explicit C# 7.3 language contract.')
}
if (-not $toolProjectText.Contains('..\C3.Catalogue\C3.Catalogue.csproj') -or
        -not $toolProjectText.Contains('..\C3.Infrastructure\C3.Infrastructure.csproj')) {
    $failures.Add('C3.Cli must compose the shared Catalogue and Infrastructure owners.')
}
if ([regex]::Matches($toolProjectText, '<ProjectReference\b').Count -ne 2) {
    $failures.Add('C3.Cli may reference only C3.Catalogue and C3.Infrastructure.')
}

if ($failures.Count -gt 0) {
    throw ("Dependency validation failed:`n - " + ($failures -join "`n - "))
}

Write-Host 'Dependency direction verified: WinForms -> Presentation/Catalogue/Infrastructure; Presentation -> Catalogue/Domain; Infrastructure -> Catalogue/Domain; Catalogue -> Domain.'
