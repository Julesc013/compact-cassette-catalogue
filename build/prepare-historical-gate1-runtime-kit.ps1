[CmdletBinding()]
param(
    [string]$OutputDirectory,
    [switch]$ForceDownload
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Add-ZipFile {
    param($Archive, [string]$Name, [string]$Path, [DateTimeOffset]$Timestamp)
    $entry = $Archive.CreateEntry($Name, [IO.Compression.CompressionLevel]::Optimal)
    $entry.LastWriteTime = $Timestamp
    $input = [IO.File]::OpenRead($Path)
    $output = $entry.Open()
    try { $input.CopyTo($output) }
    finally { $output.Dispose(); $input.Dispose() }
}

function Add-ZipText {
    param($Archive, [string]$Name, [string]$Content, [DateTimeOffset]$Timestamp)
    $entry = $Archive.CreateEntry($Name, [IO.Compression.CompressionLevel]::Optimal)
    $entry.LastWriteTime = $Timestamp
    $output = $entry.Open()
    try {
        $bytes = (New-Object Text.UTF8Encoding($false)).GetBytes($Content)
        $output.Write($bytes, 0, $bytes.Length)
    }
    finally { $output.Dispose() }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts')).TrimEnd('\') + '\'
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) { $OutputDirectory = Join-Path $repositoryRoot 'artifacts\historical-gate1\runtime-kit' }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not ($OutputDirectory.TrimEnd('\') + '\').StartsWith($artifactsRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to prepare historical runtime material outside '$artifactsRoot'."
}

& (Join-Path $PSScriptRoot 'download-baseline-assets.ps1') -Force:$ForceDownload
$v112Directory = Join-Path $repositoryRoot 'artifacts\baseline\official-v1.1.2'
$v112Path = Join-Path $v112Directory 'C3-v1.1.2.exe'
if ($ForceDownload -or -not (Test-Path -LiteralPath $v112Path -PathType Leaf)) {
    New-Item -ItemType Directory -Path $v112Directory -Force | Out-Null
    $downloadPath = $v112Path + '.download'
    if (Test-Path -LiteralPath $downloadPath) { Remove-Item -LiteralPath $downloadPath -Force }
    Invoke-WebRequest -Uri 'https://github.com/Julesc013/compact-cassette-catalogue/releases/download/v1.1.2/C3-v1.1.2.exe' -OutFile $downloadPath -UseBasicParsing
    Move-Item -LiteralPath $downloadPath -Destination $v112Path -Force
}

$inputs = @(
    [PSCustomObject]@{ name = 'C3-v1.1.2.exe'; path = $v112Path; size = 1356800; sha256 = '50183c989956f85364dd1cda55a1397209a646b58f7df6cda0604540e3382f9e' },
    [PSCustomObject]@{ name = 'C3-v1.2.0b1-win-x86.exe'; path = (Join-Path $repositoryRoot 'artifacts\baseline\official-v1.2.0b1\C3-v1.2.0b1-win-x86.exe'); size = 1326592; sha256 = '205ba251175d5a6fa20a3ace6127a00e5d10d73ad30581032c8f09b20ceb7222' },
    [PSCustomObject]@{ name = 'C3-v1.2.0b1-win-x64.exe'; path = (Join-Path $repositoryRoot 'artifacts\baseline\official-v1.2.0b1\C3-v1.2.0b1-win-x64.exe'); size = 1326080; sha256 = '257ec9d0ea86f268d8328d71041e63eb379fc1809c91593db29d883359db747c' },
    [PSCustomObject]@{ name = 'blank.xml'; path = (Join-Path $repositoryRoot 'fixtures\catalogues\v1.1.0\historical\v1.1.2\blank.xml'); size = 1183; sha256 = '038a8a79dfa87eead5476ee0e143df7e09b8dd94da26e183388213fe06d4c48f' }
)
foreach ($input in $inputs) {
    if (-not (Test-Path -LiteralPath $input.path -PathType Leaf) -or [long](Get-Item -LiteralPath $input.path).Length -ne [long]$input.size -or
            (Get-FileHash -LiteralPath $input.path -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$input.sha256) {
        throw "Historical runtime input failed exact length/SHA-256 verification: $($input.name)"
    }
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$readme = [IO.File]::ReadAllText((Join-Path $PSScriptRoot 'package-content\HISTORICAL_GATE1_README.txt')).Replace("`r`n", "`n").Replace("`r", "`n")
$checksumText = (($inputs | ForEach-Object { "$($_.sha256)  $($_.name)" }) -join "`n") + "`n"
$kitPath = Join-Path $OutputDirectory 'C3-1.3.0-historical-gate1-runtime-kit.zip'
$fixedTimestamp = [DateTimeOffset]::Parse('2000-01-01T00:00:00Z')
$stream = [IO.File]::Open($kitPath, [IO.FileMode]::Create, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
$archive = New-Object IO.Compression.ZipArchive($stream, [IO.Compression.ZipArchiveMode]::Create, $false)
try {
    foreach ($input in $inputs) { Add-ZipFile -Archive $archive -Name $input.name -Path $input.path -Timestamp $fixedTimestamp }
    Add-ZipText -Archive $archive -Name 'SHA256SUMS.txt' -Content $checksumText -Timestamp $fixedTimestamp
    Add-ZipText -Archive $archive -Name 'README.txt' -Content $readme -Timestamp $fixedTimestamp
}
finally { $archive.Dispose(); $stream.Dispose() }

$kitHash = (Get-FileHash -LiteralPath $kitPath -Algorithm SHA256).Hash.ToLowerInvariant()
$evidence = [ordered]@{
    schemaVersion = 1
    classification = 'historical-compatibility-laboratory-only'
    packageName = [IO.Path]::GetFileName($kitPath)
    packageSha256 = $kitHash
    sourceCommitV112 = '817fbae296acb6cd6a5a56ed299a517bf6e62036'
    sourceCommitV120b1 = '2413e9139a098f3321385f2f946e743012a447f5'
    files = @($inputs | ForEach-Object { [ordered]@{ name = $_.name; size = $_.size; sha256 = $_.sha256 } })
}
$evidencePath = Join-Path $OutputDirectory 'runtime-kit.json'
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 6) + "`n"), (New-Object Text.UTF8Encoding($false)))
Write-Host "Prepared historical Gate 1 runtime kit: $kitPath ($kitHash)"
