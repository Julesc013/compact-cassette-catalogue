[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$expectedAssemblyVersion = $identity.AssemblyVersion
$expectedFileVersion = $identity.FileVersion
$expectedProductVersion = $identity.InformationalVersion

$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$binaryNames = @(
    'Compact Cassette Catalogue.exe'
    'C3.Catalogue.dll'
    'C3.Domain.dll'
    'C3.Infrastructure.dll'
    'C3.Presentation.WinForms.dll'
)

foreach ($lane in @($manifest.lanes)) {
    $outputDirectory = Join-Path $repositoryRoot ([string]$lane.outputDirectory)
    $configuredOutput = Split-Path -Parent $outputDirectory
    $outputDirectory = Join-Path $configuredOutput $Configuration

    foreach ($binaryName in $binaryNames) {
        $binaryPath = Join-Path $outputDirectory $binaryName
        if (-not (Test-Path -LiteralPath $binaryPath -PathType Leaf)) {
            throw "Missing binary metadata input for $($lane.id): $binaryPath"
        }

        $assemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($binaryPath).Version.ToString()
        $fileInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($binaryPath)
        if ($assemblyVersion -cne $expectedAssemblyVersion) {
            throw "$($lane.id)/$binaryName assembly version: expected '$expectedAssemblyVersion', found '$assemblyVersion'."
        }
        if ($fileInfo.FileVersion -cne $expectedFileVersion) {
            throw "$($lane.id)/$binaryName file version: expected '$expectedFileVersion', found '$($fileInfo.FileVersion)'."
        }
        if ($fileInfo.ProductVersion -cne $expectedProductVersion) {
            throw "$($lane.id)/$binaryName product version: expected '$expectedProductVersion', found '$($fileInfo.ProductVersion)'."
        }

        Write-Host "Binary metadata verified: $($lane.id)/$binaryName"
    }
}

$toolPath = Join-Path $repositoryRoot "artifacts\bin\cli\$Configuration\c3.exe"
if (-not (Test-Path -LiteralPath $toolPath -PathType Leaf)) {
    throw "Missing catalogue CLI metadata input: $toolPath"
}
$toolAssemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($toolPath).Version.ToString()
$toolInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($toolPath)
if ($toolAssemblyVersion -cne $expectedAssemblyVersion -or
        $toolInfo.FileVersion -cne $expectedFileVersion -or
        $toolInfo.ProductVersion -cne $expectedProductVersion) {
    throw 'c3.exe identity does not match the canonical release identity.'
}
Write-Host 'Binary metadata verified: cli/c3.exe'

Write-Host "All packaged binary identities match $expectedProductVersion."
