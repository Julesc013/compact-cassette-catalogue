[CmdletBinding()]
param(
    [string]$Destination,
    [switch]$Force
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($Destination)) {
    $Destination = Join-Path $repositoryRoot 'artifacts\baseline\official-v1.2.0b1'
}
$Destination = [IO.Path]::GetFullPath($Destination)
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts')).TrimEnd('\') + '\'
if (-not $Destination.StartsWith($artifactsRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to download baseline assets outside '$artifactsRoot'."
}

$assets = @(
    [pscustomobject]@{
        name = 'C3-v1.2.0b1-win-x86.exe'
        size = 1326592
        sha256 = '205ba251175d5a6fa20a3ace6127a00e5d10d73ad30581032c8f09b20ceb7222'
    },
    [pscustomobject]@{
        name = 'C3-v1.2.0b1-win-x64.exe'
        size = 1326080
        sha256 = '257ec9d0ea86f268d8328d71041e63eb379fc1809c91593db29d883359db747c'
    }
)

New-Item -ItemType Directory -Path $Destination -Force | Out-Null
foreach ($asset in $assets) {
    $path = Join-Path $Destination $asset.name
    $downloadPath = $path + '.download'
    if ($Force -or -not (Test-Path -LiteralPath $path -PathType Leaf)) {
        if (Test-Path -LiteralPath $downloadPath -PathType Leaf) {
            Remove-Item -LiteralPath $downloadPath -Force
        }
        $url = "https://github.com/Julesc013/compact-cassette-catalogue/releases/download/v1.2.0b1/$($asset.name)"
        Invoke-WebRequest -Uri $url -OutFile $downloadPath -UseBasicParsing
        Move-Item -LiteralPath $downloadPath -Destination $path -Force
    }

    $file = Get-Item -LiteralPath $path
    $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($file.Length -ne [long]$asset.size) {
        throw "$($asset.name) has size $($file.Length), expected $($asset.size)."
    }
    if ($hash -cne [string]$asset.sha256) {
        throw "$($asset.name) has SHA-256 $hash, expected $($asset.sha256)."
    }
    Write-Host "Verified official baseline asset: $($asset.name) ($hash)"
}
