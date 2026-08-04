[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repositoryRoot (
    'src\C3.Infrastructure\C3.Infrastructure.CSharpCandidate.csproj')
$assemblyPath = Join-Path $repositoryRoot (
    "artifacts\migration\infrastructure-csharp\$Configuration\C3.Infrastructure.CSharpCandidate.dll")
$baselinePath = Join-Path $repositoryRoot (
    'spec\infrastructure-api\v1\public-api.txt')
$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1')

& $msbuild $projectPath /nologo /m:1 /t:Rebuild `
    /p:Configuration=$Configuration /p:Platform=AnyCPU /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "The C# Infrastructure candidate build failed with exit code $LASTEXITCODE."
}

& (Join-Path $PSScriptRoot 'validate-catalogue-api.ps1') `
    -Configuration $Configuration `
    -AssemblyPath $assemblyPath `
    -BaselinePath $baselinePath `
    -ContractName 'Infrastructure candidate'

Write-Host 'The complete C# Infrastructure candidate matches the frozen VB public API.'
