[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$OutputDirectory,
    [string]$EvidenceDirectory,
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
. (Join-Path $PSScriptRoot 'package-evidence-set.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)"
}
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) {
    $EvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
New-Item -ItemType Directory -Path $EvidenceDirectory -Force | Out-Null

& (Join-Path $PSScriptRoot 'verify-builds.ps1') -Configuration $Configuration

$evidenceByLane = @{}
$evidenceRecords = @($lanes | ForEach-Object {
    $evidencePath = Join-Path $repositoryRoot "artifacts\evidence\build\$($_.id)\$Configuration\toolchain.json"
    $laneEvidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
    $evidenceByLane[[string]$_.id] = $laneEvidence
    New-Object PSObject -Property @{
        lane = [string]$_.id
        releaseVersion = [string]$manifest.releaseVersion
        releaseStage = [string]$manifest.releaseStage
        releaseLabel = [string]$manifest.releaseLabel
        releaseTag = [string]$manifest.releaseTag
        releaseChannel = [string]$manifest.releaseChannel
        publicationStatus = [string]$manifest.publicationStatus
        sourceCommit = [string]$laneEvidence.source.commit
        toolchainMode = [string]$laneEvidence.toolchainMode
        toolchainLockStatus = [string]$laneEvidence.toolchainLock.status
        toolchainLockSha256 = [string]$laneEvidence.toolchainLock.sha256
    }
})
$packageSetIdentity = Assert-C3PackageEvidenceSet -Records $evidenceRecords -RequireCandidate:$RequireCandidateEvidence
if ($RequireCandidateEvidence) {
    $closurePath = Join-Path $repositoryRoot 'artifacts\evidence\build\candidate-source-closure.json'
    if (-not (Test-Path -LiteralPath $closurePath -PathType Leaf)) {
        throw "Candidate packaging requires retained post-build source closure: $closurePath"
    }
    $closure = Get-Content -LiteralPath $closurePath -Raw | ConvertFrom-Json
    if ([string]$closure.status -cne 'pass' -or
            [string]$closure.sourceCommit -cne [string]$packageSetIdentity.sourceCommit -or
            [string]$closure.toolchainLockSha256 -cne [string]$packageSetIdentity.toolchainLockSha256 -or
            -not [bool]$closure.worktreeClean -or
            -not [bool]$closure.submodulesExact -or
            [string]$closure.remoteSnapshotCommit -cne [string]$packageSetIdentity.sourceCommit -or
            [string]$closure.genome -cne 'pass' -or
            [string]$closure.laneProjection -cne 'pass') {
        throw 'Candidate package evidence does not match the retained post-build source/ref/genome/lock closure.'
    }
}

$readmePath = Join-Path $PSScriptRoot 'package-content\README.txt'
$releaseNotesPath = Join-Path $repositoryRoot 'RELEASE_NOTES.md'
$fixedTimestamp = [DateTimeOffset]::Parse('2000-01-01T00:00:00Z')
$checksums = New-Object Collections.Generic.List[String]
$entryManifestChecksums = New-Object Collections.Generic.List[String]
foreach ($buildLane in $lanes) {
    $outputPath = Join-Path $repositoryRoot "artifacts\bin\$($buildLane.id)\$Configuration"
    $executable = Join-Path $outputPath 'Compact Cassette Catalogue.exe'
    $config = $executable + '.config'
    $evidence = $evidenceByLane[[string]$buildLane.id]

    $executableHash = (Get-FileHash -LiteralPath $executable -Algorithm SHA256).Hash.ToLowerInvariant()
    $configHash = (Get-FileHash -LiteralPath $config -Algorithm SHA256).Hash.ToLowerInvariant()
    $buildText = @(
        'formatVersion=2',
        "product=$($manifest.product)",
        "releaseVersion=$($manifest.releaseVersion)",
        "releaseStage=$($manifest.releaseStage)",
        "releaseLabel=$($manifest.releaseLabel)",
        "releaseTag=$($manifest.releaseTag)",
        "releaseChannel=$($manifest.releaseChannel)",
        "publicationStatus=$($manifest.publicationStatus)",
        "assemblyVersion=$($manifest.assemblyVersion)",
        "fileVersion=$($manifest.fileVersion)",
        "assemblyProductVersion=$($manifest.assemblyProductVersion)",
        "lane=$($buildLane.id)",
        "configuration=$Configuration",
        "sourceCommit=$($evidence.source.commit)",
        "toolchainMode=$($evidence.toolchainMode)",
        "toolchainLockStatus=$($evidence.toolchainLock.status)",
        "toolchainLockSha256=$($evidence.toolchainLock.sha256)",
        "visualStudioProductVersion=$($evidence.visualStudio.productVersion)",
        "visualStudioInstallationVersion=$($evidence.visualStudio.installationVersion)",
        "msbuildVersion=$($evidence.msbuild.productVersion)",
        "msbuildSha256=$($evidence.msbuild.sha256)",
        "effectiveToolsVersion=$($evidence.msbuild.effectiveToolsVersion)",
        "vbcVersion=$($evidence.compiler.productVersion)",
        "vbcSha256=$($evidence.compiler.sha256)",
        "targetFramework=$($buildLane.targetFramework)",
        "referenceAssemblySetSha256=$($evidence.referenceAssemblies.setSha256)",
        "resourceToolName=$($evidence.resourceTools[0].name)",
        "resourceToolSha256=$($evidence.resourceTools[0].sha256)",
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

    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entryRecords = @($archive.Entries | ForEach-Object {
            $entryStream = $_.Open()
            $algorithm = [Security.Cryptography.SHA256]::Create()
            try {
                $entryHash = ([BitConverter]::ToString($algorithm.ComputeHash($entryStream))).Replace('-', '').ToLowerInvariant()
            }
            finally {
                $algorithm.Dispose()
                $entryStream.Dispose()
            }
            [ordered]@{
                name = $_.FullName
                size = [int64]$_.Length
                sha256 = $entryHash
            }
        })
    }
    finally {
        $archive.Dispose()
    }
    $entryManifest = [ordered]@{
        schemaVersion = 2
        packageName = [string]$buildLane.packageName
        packageSha256 = $packageHash
        releaseVersion = [string]$manifest.releaseVersion
        releaseStage = [string]$manifest.releaseStage
        releaseLabel = [string]$manifest.releaseLabel
        releaseTag = [string]$manifest.releaseTag
        releaseChannel = [string]$manifest.releaseChannel
        publicationStatus = [string]$manifest.publicationStatus
        sourceCommit = [string]$evidence.source.commit
        toolchainLockSha256 = [string]$evidence.toolchainLock.sha256
        entries = $entryRecords
    }
    $entryManifestName = "$($buildLane.packageName).entries.json"
    $entryManifestPath = Join-Path $EvidenceDirectory $entryManifestName
    $entryManifestJson = ($entryManifest | ConvertTo-Json -Depth 6) + "`n"
    [IO.File]::WriteAllText($entryManifestPath, $entryManifestJson, (New-Object Text.UTF8Encoding($false)))
    $entryManifestHash = (Get-FileHash -LiteralPath $entryManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $entryManifestChecksums.Add("$entryManifestHash  $entryManifestName")
    Write-Host "Packaged $($buildLane.id): $packagePath"
}

$checksumPath = Join-Path $OutputDirectory 'SHA256SUMS.txt'
$checksumText = (($checksums.ToArray()) -join "`n") + "`n"
[IO.File]::WriteAllText($checksumPath, $checksumText, (New-Object Text.UTF8Encoding($false)))
Write-Host "Wrote checksum manifest: $checksumPath"
$entryChecksumPath = Join-Path $EvidenceDirectory 'ENTRY_MANIFEST_SHA256SUMS.txt'
$entryChecksumText = (($entryManifestChecksums.ToArray()) -join "`n") + "`n"
[IO.File]::WriteAllText($entryChecksumPath, $entryChecksumText, (New-Object Text.UTF8Encoding($false)))
Write-Host "Wrote retained package-entry evidence: $EvidenceDirectory"
