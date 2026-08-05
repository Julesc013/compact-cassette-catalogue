[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$OutputDirectory,
    [switch]$RequireCandidateEvidence
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Add-ZipFileEntry {
    param(
        [Parameter(Mandatory = $true)]$Archive,
        [Parameter(Mandatory = $true)][string]$EntryName,
        [Parameter(Mandatory = $true)][string]$SourcePath,
        [Parameter(Mandatory = $true)][DateTimeOffset]$Timestamp
    )

    $entry = $Archive.CreateEntry($EntryName, [IO.Compression.CompressionLevel]::Optimal)
    $entry.LastWriteTime = $Timestamp
    $input = [IO.File]::Open($SourcePath, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::Read)
    $output = $entry.Open()
    try {
        $input.CopyTo($output)
    }
    finally {
        $output.Dispose()
        $input.Dispose()
    }
}

function Add-ZipTextEntry {
    param(
        [Parameter(Mandatory = $true)]$Archive,
        [Parameter(Mandatory = $true)][string]$EntryName,
        [Parameter(Mandatory = $true)][string]$Content,
        [Parameter(Mandatory = $true)][DateTimeOffset]$Timestamp
    )

    $entry = $Archive.CreateEntry($EntryName, [IO.Compression.CompressionLevel]::Optimal)
    $entry.LastWriteTime = $Timestamp
    $output = $entry.Open()
    try {
        $encoding = New-Object Text.UTF8Encoding($false)
        $bytes = $encoding.GetBytes($Content)
        $output.Write($bytes, 0, $bytes.Length)
    }
    finally {
        $output.Dispose()
    }
}

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseVersion)"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

& (Join-Path $PSScriptRoot 'verify-builds.ps1') -Configuration $Configuration

$readmePath = Join-Path $PSScriptRoot 'package-content\README.txt'
$releaseNotesPath = Join-Path $repositoryRoot 'RELEASE_NOTES.md'
$fixedTimestamp = [DateTimeOffset]::Parse('2000-01-01T00:00:00Z')
$checksums = New-Object Collections.Generic.List[String]
foreach ($buildLane in $lanes) {
    $outputPath = Join-Path $repositoryRoot "artifacts\bin\$($buildLane.id)\$Configuration"
    $executable = Join-Path $outputPath 'Compact Cassette Catalogue.exe'
    $config = $executable + '.config'
    $evidencePath = Join-Path $repositoryRoot "artifacts\evidence\build\$($buildLane.id)\$Configuration\toolchain.json"
    $evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
    if ($RequireCandidateEvidence -and [string]$evidence.toolchainMode -cne 'Candidate') {
        throw "$($buildLane.id) package requires Candidate toolchain evidence, found '$($evidence.toolchainMode)'."
    }

    $executableHash = (Get-FileHash -LiteralPath $executable -Algorithm SHA256).Hash.ToLowerInvariant()
    $configHash = (Get-FileHash -LiteralPath $config -Algorithm SHA256).Hash.ToLowerInvariant()
    $buildText = @(
        'formatVersion=1',
        "product=$($manifest.product)",
        "releaseVersion=$($manifest.releaseVersion)",
        "lane=$($buildLane.id)",
        "configuration=$Configuration",
        "sourceCommit=$($evidence.source.commit)",
        "toolchainMode=$($evidence.toolchainMode)",
        "visualStudioProductVersion=$($evidence.visualStudio.productVersion)",
        "visualStudioInstallationVersion=$($evidence.visualStudio.installationVersion)",
        "msbuildVersion=$($evidence.msbuild.productVersion)",
        "msbuildSha256=$($evidence.msbuild.sha256)",
        "effectiveToolsVersion=$($evidence.msbuild.effectiveToolsVersion)",
        "vbcVersion=$($evidence.compiler.productVersion)",
        "vbcSha256=$($evidence.compiler.sha256)",
        "targetFramework=$($buildLane.targetFramework)",
        "referenceAssemblySetSha256=$($evidence.referenceAssemblies.setSha256)",
        "peMachine=$($buildLane.peMachine)",
        "peOptionalHeader=$($buildLane.peOptionalHeader)",
        "executableSha256=$executableHash",
        "configSha256=$configHash",
        'runtimeDllCount=0',
        'distribution=portable-classic-winforms'
    ) -join "`n"
    $buildText += "`n"

    $packagePath = Join-Path $OutputDirectory ([string]$buildLane.packageName)
    $stream = [IO.File]::Open($packagePath, [IO.FileMode]::Create, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
    $archive = New-Object IO.Compression.ZipArchive($stream, [IO.Compression.ZipArchiveMode]::Create, $false)
    try {
        Add-ZipFileEntry -Archive $archive -EntryName 'Compact Cassette Catalogue.exe' -SourcePath $executable -Timestamp $fixedTimestamp
        Add-ZipFileEntry -Archive $archive -EntryName 'Compact Cassette Catalogue.exe.config' -SourcePath $config -Timestamp $fixedTimestamp
        Add-ZipFileEntry -Archive $archive -EntryName 'README.txt' -SourcePath $readmePath -Timestamp $fixedTimestamp
        Add-ZipFileEntry -Archive $archive -EntryName 'RELEASE_NOTES.txt' -SourcePath $releaseNotesPath -Timestamp $fixedTimestamp
        Add-ZipTextEntry -Archive $archive -EntryName 'BUILD.txt' -Content $buildText -Timestamp $fixedTimestamp
    }
    finally {
        $archive.Dispose()
        $stream.Dispose()
    }

    $packageHash = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash.ToLowerInvariant()
    $checksums.Add("$packageHash  $($buildLane.packageName)")
    Write-Host "Packaged $($buildLane.id): $packagePath"
}

$checksumPath = Join-Path $OutputDirectory 'SHA256SUMS.txt'
$checksumText = (($checksums.ToArray()) -join "`n") + "`n"
[IO.File]::WriteAllText($checksumPath, $checksumText, (New-Object Text.UTF8Encoding($false)))
Write-Host "Wrote checksum manifest: $checksumPath"
