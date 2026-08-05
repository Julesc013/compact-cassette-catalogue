[CmdletBinding()]
param(
    [string]$Destination,
    [switch]$Force,
    [switch]$VerifyOnly
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$contractPath = Join-Path $PSScriptRoot 'historical-toolchain.json'
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json
if ([int]$contract.schemaVersion -ne 1 -or
        [string]$contract.classification -cne 'historical-compatibility-laboratory-only') {
    throw 'Unsupported historical toolchain contract.'
}

if ([string]::IsNullOrWhiteSpace($Destination)) {
    $Destination = Join-Path $repositoryRoot 'artifacts\toolchains\vs2015-buildtools'
}
$Destination = [IO.Path]::GetFullPath($Destination)
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts')).TrimEnd('\') + '\'
if (-not $Destination.StartsWith($artifactsRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to prepare historical toolchain material outside '$artifactsRoot'."
}

$installer = $contract.installer
$installerPath = Join-Path $Destination ([string]$installer.name)
$downloadPath = $installerPath + '.download'
New-Item -ItemType Directory -Path $Destination -Force | Out-Null

if ($VerifyOnly -and -not (Test-Path -LiteralPath $installerPath -PathType Leaf)) {
    throw "Historical toolchain installer is missing: $installerPath"
}
if (-not $VerifyOnly -and ($Force -or -not (Test-Path -LiteralPath $installerPath -PathType Leaf))) {
    if (Test-Path -LiteralPath $downloadPath -PathType Leaf) {
        Remove-Item -LiteralPath $downloadPath -Force
    }
    Invoke-WebRequest -Uri ([string]$installer.sourceUrl) -OutFile $downloadPath -UseBasicParsing
    Move-Item -LiteralPath $downloadPath -Destination $installerPath -Force
}

$file = Get-Item -LiteralPath $installerPath
$sha256 = (Get-FileHash -LiteralPath $installerPath -Algorithm SHA256).Hash.ToLowerInvariant()
$sha1 = (Get-FileHash -LiteralPath $installerPath -Algorithm SHA1).Hash.ToLowerInvariant()
$signature = Get-AuthenticodeSignature -LiteralPath $installerPath
$signerSubject = ''
$signerThumbprint = ''
if ($null -ne $signature.SignerCertificate) {
    $signerSubject = [string]$signature.SignerCertificate.Subject
    $signerThumbprint = [string]$signature.SignerCertificate.Thumbprint
}
$checks = @(
    @('length', [string]$file.Length, [string]$installer.length),
    @('SHA-256', $sha256, [string]$installer.sha256),
    @('SHA-1', $sha1, [string]$installer.sha1),
    @('file version', [string]$file.VersionInfo.FileVersion, [string]$installer.fileVersion),
    @('product version', [string]$file.VersionInfo.ProductVersion, [string]$installer.productVersion),
    @('signature status', [string]$signature.Status, 'Valid'),
    @('signer subject', $signerSubject, [string]$installer.signerSubject),
    @('signer thumbprint', $signerThumbprint, [string]$installer.signerThumbprint)
)
foreach ($check in $checks) {
    if ([string]$check[1] -cne [string]$check[2]) {
        throw "Historical toolchain installer $($check[0]) '$($check[1])' does not match '$($check[2])'."
    }
}

$evidenceDirectory = Join-Path $repositoryRoot 'artifacts\evidence\historical-toolchain'
New-Item -ItemType Directory -Path $evidenceDirectory -Force | Out-Null
$evidencePath = Join-Path $evidenceDirectory 'build-tools-2015-update-3-bootstrapper.json'
$evidence = [ordered]@{
    schemaVersion = 1
    classification = [string]$contract.classification
    verifiedAtUtc = [DateTime]::UtcNow.ToString('o')
    sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    product = [string]$contract.product
    expectedMsbuildVersion = [string]$contract.expectedMsbuildVersion
    installer = [ordered]@{
        path = $file.FullName
        sourceUrl = [string]$installer.sourceUrl
        length = [long]$file.Length
        sha256 = $sha256
        sha1 = $sha1
        fileVersion = [string]$file.VersionInfo.FileVersion
        productVersion = [string]$file.VersionInfo.ProductVersion
        signatureStatus = [string]$signature.Status
        signerSubject = $signerSubject
        signerThumbprint = $signerThumbprint
    }
    installationStatus = 'not-installed'
    releaseAuthority = $false
}
$evidence | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $evidencePath -Encoding UTF8

Write-Host "Verified historical compatibility-lab bootstrapper: $installerPath"
Write-Host "SHA-256: $sha256"
Write-Host "Evidence: $evidencePath"
Write-Host 'Installation and reconstructed-build qualification remain separate open gates.'
