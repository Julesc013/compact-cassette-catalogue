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
    'C3.Infrastructure.dll'
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

Write-Host "All packaged binary identities match $expectedProductVersion."
