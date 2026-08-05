[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$PackageDirectory,
    [string]$EvidenceDirectory,
    [switch]$RequireCandidateEvidence
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
. (Join-Path $PSScriptRoot 'package-evidence-set.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if ([string]::IsNullOrWhiteSpace($PackageDirectory)) {
    $PackageDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)"
}
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) {
    $EvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)"
}
$PackageDirectory = [IO.Path]::GetFullPath($PackageDirectory)
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)
$expectedAssetNames = @($lanes.packageName) + @('SHA256SUMS.txt')
$actualAssetNames = @(Get-ChildItem -LiteralPath $PackageDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualAssetNames -join "`n") -cne (($expectedAssetNames | Sort-Object) -join "`n")) {
    throw "Package directory must contain exactly the three ZIPs and SHA256SUMS.txt. Found: $($actualAssetNames -join ', ')"
}

$checksumLines = @(Get-Content -LiteralPath (Join-Path $PackageDirectory 'SHA256SUMS.txt'))
if ($checksumLines.Count -ne $lanes.Count) {
    throw "SHA256SUMS.txt contains $($checksumLines.Count) lines; expected $($lanes.Count)."
}
$expectedEvidenceNames = @($lanes | ForEach-Object { "$($_.packageName).entries.json" }) + @('ENTRY_MANIFEST_SHA256SUMS.txt')
$actualEvidenceNames = @(Get-ChildItem -LiteralPath $EvidenceDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualEvidenceNames -join "`n") -cne (($expectedEvidenceNames | Sort-Object) -join "`n")) {
    throw "Package evidence directory does not contain the exact entry-manifest set: $($actualEvidenceNames -join ', ')"
}
$entryChecksumLines = @(Get-Content -LiteralPath (Join-Path $EvidenceDirectory 'ENTRY_MANIFEST_SHA256SUMS.txt'))
if ($entryChecksumLines.Count -ne $lanes.Count) {
    throw "ENTRY_MANIFEST_SHA256SUMS.txt contains $($entryChecksumLines.Count) lines; expected $($lanes.Count)."
}
$expectedEntries = @(
    'Compact Cassette Catalogue.exe',
    'Compact Cassette Catalogue.exe.config',
    'README.txt',
    'RELEASE_NOTES.txt',
    'BUILD.txt'
)
$packageEvidenceRecords = New-Object Collections.Generic.List[Object]

for ($index = 0; $index -lt $lanes.Count; $index++) {
    $lane = $lanes[$index]
    $packagePath = Join-Path $PackageDirectory ([string]$lane.packageName)
    $packageHash = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash.ToLowerInvariant()
    $expectedChecksumLine = "$packageHash  $($lane.packageName)"
    if ($checksumLines[$index] -cne $expectedChecksumLine) {
        throw "Checksum line $($index + 1) does not match '$expectedChecksumLine'."
    }

    $entryManifestName = "$($lane.packageName).entries.json"
    $entryManifestPath = Join-Path $EvidenceDirectory $entryManifestName
    $entryManifestHash = (Get-FileHash -LiteralPath $entryManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($entryChecksumLines[$index] -cne "$entryManifestHash  $entryManifestName") {
        throw "$($lane.id) retained entry-manifest checksum does not match."
    }
    $entryManifest = Get-Content -LiteralPath $entryManifestPath -Raw | ConvertFrom-Json
    if ([int]$entryManifest.schemaVersion -ne 2 -or
            [string]$entryManifest.packageName -cne [string]$lane.packageName -or
            [string]$entryManifest.packageSha256 -cne $packageHash -or
            [string]$entryManifest.releaseVersion -cne [string]$manifest.releaseVersion -or
            [string]$entryManifest.releaseStage -cne [string]$manifest.releaseStage -or
            [string]$entryManifest.releaseLabel -cne [string]$manifest.releaseLabel -or
            [string]$entryManifest.releaseTag -cne [string]$manifest.releaseTag -or
            [string]$entryManifest.releaseChannel -cne [string]$manifest.releaseChannel -or
            [string]$entryManifest.publicationStatus -cne [string]$manifest.publicationStatus -or
            [string]$entryManifest.sourceCommit -notmatch '^[0-9a-f]{40}$' -or
            [string]$entryManifest.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$') {
        throw "$($lane.id) retained entry manifest is not bound to its package/source/toolchain lock."
    }

    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entryNames = @($archive.Entries | ForEach-Object { $_.FullName })
        if (($entryNames -join "`n") -cne ($expectedEntries -join "`n")) {
            throw "$($lane.id) ZIP payload differs from the exact portable allow-list: $($entryNames -join ', ')"
        }
        $manifestEntries = @($entryManifest.entries)
        if ($manifestEntries.Count -ne $expectedEntries.Count) {
            throw "$($lane.id) retained entry manifest does not contain exactly five entries."
        }
        foreach ($entry in $archive.Entries) {
            if ($entry.LastWriteTime.DateTime -ne [DateTime]::new(2000, 1, 1, 0, 0, 0, [DateTimeKind]::Unspecified)) {
                throw "$($lane.id) entry '$($entry.FullName)' has a non-deterministic timestamp '$($entry.LastWriteTime)'."
            }
            if ($entry.FullName -match '(?i)(\.dll$|\.msi$|\.msix$|\.application$|setup\.exe$|uninstall|bootstrap|clickonce|updater?)') {
                throw "$($lane.id) contains prohibited release content '$($entry.FullName)'."
            }
            $entryRecord = @($manifestEntries | Where-Object { [string]$_.name -ceq $entry.FullName })
            if ($entryRecord.Count -ne 1 -or [int64]$entryRecord[0].size -ne [int64]$entry.Length) {
                throw "$($lane.id) retained manifest name/size mismatch for '$($entry.FullName)'."
            }
            $entryStream = $entry.Open()
            $algorithm = [Security.Cryptography.SHA256]::Create()
            try {
                $actualEntryHash = ([BitConverter]::ToString($algorithm.ComputeHash($entryStream))).Replace('-', '').ToLowerInvariant()
            }
            finally {
                $algorithm.Dispose()
                $entryStream.Dispose()
            }
            if ($actualEntryHash -cne [string]$entryRecord[0].sha256) {
                throw "$($lane.id) retained manifest SHA-256 mismatch for '$($entry.FullName)'."
            }
        }

        foreach ($mapping in @(
                @('Compact Cassette Catalogue.exe', (Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\Compact Cassette Catalogue.exe")),
                @('Compact Cassette Catalogue.exe.config', (Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\Compact Cassette Catalogue.exe.config")))) {
            $entry = $archive.GetEntry([string]$mapping[0])
            $entryStream = $entry.Open()
            $algorithm = [Security.Cryptography.SHA256]::Create()
            try {
                $entryHash = ([BitConverter]::ToString($algorithm.ComputeHash($entryStream))).Replace('-', '').ToLowerInvariant()
            }
            finally {
                $algorithm.Dispose()
                $entryStream.Dispose()
            }
            $sourceHash = (Get-FileHash -LiteralPath ([string]$mapping[1]) -Algorithm SHA256).Hash.ToLowerInvariant()
            if ($entryHash -cne $sourceHash) {
                throw "$($lane.id) ZIP entry '$($mapping[0])' does not match its verified build output."
            }
        }

        $buildEntry = $archive.GetEntry('BUILD.txt')
        $reader = New-Object IO.StreamReader($buildEntry.Open(), (New-Object Text.UTF8Encoding($false)), $true)
        try {
            $buildData = ConvertFrom-StringData ($reader.ReadToEnd())
        }
        finally {
            $reader.Dispose()
        }
        if ([string]$buildData.formatVersion -cne '2' -or
                [string]$buildData.lane -cne [string]$lane.id -or
                [string]$buildData.releaseVersion -cne [string]$manifest.releaseVersion -or
                [string]$buildData.releaseStage -cne [string]$manifest.releaseStage -or
                [string]$buildData.releaseLabel -cne [string]$manifest.releaseLabel -or
                [string]$buildData.releaseTag -cne [string]$manifest.releaseTag -or
                [string]$buildData.releaseChannel -cne [string]$manifest.releaseChannel -or
                [string]$buildData.publicationStatus -cne [string]$manifest.publicationStatus -or
                [string]$buildData.assemblyVersion -cne [string]$manifest.assemblyVersion -or
                [string]$buildData.fileVersion -cne [string]$manifest.fileVersion -or
                [string]$buildData.assemblyProductVersion -cne [string]$manifest.assemblyProductVersion -or
                [string]$buildData.sourceCommit -cne [string]$entryManifest.sourceCommit -or
                [string]$buildData.targetFramework -cne [string]$lane.targetFramework -or
                [string]$buildData.peMachine -cne [string]$lane.peMachine -or
                [string]$buildData.toolchainLockSha256 -cne [string]$entryManifest.toolchainLockSha256 -or
                [string]$buildData.resourceToolName -cne 'ResGen.exe' -or
                [string]$buildData.resourceToolSha256 -notmatch '^[0-9a-f]{64}$' -or
                [string]$buildData.runtimeDllCount -cne '0' -or
                [string]$buildData.distribution -cne 'portable-classic-winforms') {
            throw "$($lane.id) BUILD.txt does not match its lane and portable payload contract."
        }
        $packageEvidenceRecords.Add((New-Object PSObject -Property @{
            lane = [string]$lane.id
            releaseVersion = [string]$buildData.releaseVersion
            releaseStage = [string]$buildData.releaseStage
            releaseLabel = [string]$buildData.releaseLabel
            releaseTag = [string]$buildData.releaseTag
            releaseChannel = [string]$buildData.releaseChannel
            publicationStatus = [string]$buildData.publicationStatus
            sourceCommit = [string]$buildData.sourceCommit
            toolchainMode = [string]$buildData.toolchainMode
            toolchainLockStatus = [string]$buildData.toolchainLockStatus
            toolchainLockSha256 = [string]$buildData.toolchainLockSha256
        }))
    }
    finally {
        $archive.Dispose()
    }
    Write-Host "Verified portable package: $($lane.packageName) ($packageHash)"
}

$packageSetIdentity = Assert-C3PackageEvidenceSet `
    -Records @($packageEvidenceRecords.ToArray()) `
    -RequireCandidate:$RequireCandidateEvidence
if ($RequireCandidateEvidence) {
    $closurePath = Join-Path $repositoryRoot 'artifacts\evidence\build\candidate-source-closure.json'
    if (-not (Test-Path -LiteralPath $closurePath -PathType Leaf)) {
        throw "Candidate package verification requires retained post-build source closure: $closurePath"
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
        throw 'Candidate package set does not match the retained post-build source/ref/genome/lock closure.'
    }
}

Write-Host 'Verified exactly three deterministic portable ZIPs from one release/source/lock evidence set, checksums, external entry manifests, matching bytes, and prohibited-output exclusion.'
