[CmdletBinding()]
param(
    [string]$OutputPath = (Join-Path $PSScriptRoot 'baseline-genome.v1.json'),
    [string]$BaselineRef = '509c9ec29679e30dcdcb1f57d8874b850cee310c'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = & (Join-Path $PSScriptRoot 'get-baseline-genome.ps1') -RepositoryRoot $repositoryRoot -BaselineRef $BaselineRef
$json = $manifest | ConvertTo-Json -Depth 20
[IO.File]::WriteAllText(
    [IO.Path]::GetFullPath($OutputPath),
    $json + [Environment]::NewLine,
    (New-Object Text.UTF8Encoding($false)))
Write-Host "Wrote baseline genome manifest to $([IO.Path]::GetFullPath($OutputPath))"
