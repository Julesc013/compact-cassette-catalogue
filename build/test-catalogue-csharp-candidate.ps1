[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repositoryRoot 'src\C3.Catalogue\C3.Catalogue.CSharpCandidate.csproj'
$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1')

& $msbuild `
    $project `
    '/t:Build' `
    "/p:Configuration=$Configuration" `
    '/p:Platform=AnyCPU' `
    '/m' `
    '/v:minimal' `
    '/nologo'

if ($LASTEXITCODE -ne 0) {
    throw "C# catalogue candidate build failed with exit code $LASTEXITCODE."
}

$assemblyPath = Join-Path $repositoryRoot (
    "artifacts\bin\candidates\net40\$Configuration\C3.Catalogue.CSharpCandidate.dll")
$translatedNamespaces = @(
    'C3.Catalogue.Brands.'
    'C3.Catalogue.CassetteModels.'
    'C3.Catalogue.Decks.'
)
foreach ($namespacePrefix in $translatedNamespaces) {
    & (Join-Path $PSScriptRoot 'validate-catalogue-api.ps1') `
        -Configuration $Configuration `
        -AssemblyPath $assemblyPath `
        -NamespacePrefix $namespacePrefix
}

Write-Host "C# catalogue candidate verified for $($translatedNamespaces.Count) translated feature(s)."
