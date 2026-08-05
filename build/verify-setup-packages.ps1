[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$PortablePackageDirectory,
    [string]$PortableEvidenceDirectory,
    [string]$PackageDirectory,
    [string]$EvidenceDirectory,
    [switch]$RequireCandidateEvidence
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
. (Join-Path $PSScriptRoot 'package-evidence-set.ps1')

function Get-EntrySha256 {
    param([Parameter(Mandatory = $true)]$Entry)
    $input = $Entry.Open()
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { return ([BitConverter]::ToString($algorithm.ComputeHash($input))).Replace('-', '').ToLowerInvariant() }
    finally { $algorithm.Dispose(); $input.Dispose() }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if ([string]::IsNullOrWhiteSpace($PortablePackageDirectory)) { $PortablePackageDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)" }
if ([string]::IsNullOrWhiteSpace($PortableEvidenceDirectory)) { $PortableEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)" }
if ([string]::IsNullOrWhiteSpace($PackageDirectory)) { $PackageDirectory = Join-Path $repositoryRoot "artifacts\setup\packages\$($manifest.releaseLabel)" }
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) { $EvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\setup-packages\$($manifest.releaseLabel)" }
$PortablePackageDirectory = [IO.Path]::GetFullPath($PortablePackageDirectory)
$PortableEvidenceDirectory = [IO.Path]::GetFullPath($PortableEvidenceDirectory)
$PackageDirectory = [IO.Path]::GetFullPath($PackageDirectory)
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)

& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration `
    -PackageDirectory $PortablePackageDirectory -EvidenceDirectory $PortableEvidenceDirectory `
    -RequireCandidateEvidence:$RequireCandidateEvidence

$expectedAssets = @($lanes.setupPackageName) + @('SHA256SUMS.txt')
$actualAssets = @(Get-ChildItem -LiteralPath $PackageDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualAssets -join "`n") -cne (($expectedAssets | Sort-Object) -join "`n")) {
    throw "Setup package directory is not the exact three-ZIP/checksum set: $($actualAssets -join ', ')"
}
$expectedEvidence = @($lanes | ForEach-Object { "$($_.setupPackageName).entries.json" }) + @('ENTRY_MANIFEST_SHA256SUMS.txt')
$actualEvidence = @(Get-ChildItem -LiteralPath $EvidenceDirectory -File | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualEvidence -join "`n") -cne (($expectedEvidence | Sort-Object) -join "`n")) {
    throw "Setup evidence directory is not the exact entry-manifest/checksum set: $($actualEvidence -join ', ')"
}
$checksumLines = @(Get-Content -LiteralPath (Join-Path $PackageDirectory 'SHA256SUMS.txt'))
$entryChecksumLines = @(Get-Content -LiteralPath (Join-Path $EvidenceDirectory 'ENTRY_MANIFEST_SHA256SUMS.txt'))
if ($checksumLines.Count -ne $lanes.Count -or $entryChecksumLines.Count -ne $lanes.Count) { throw 'Setup checksum manifests must contain exactly three ordered records.' }

$portableNames = @('Compact Cassette Catalogue.exe', 'Compact Cassette Catalogue.exe.config', 'README.txt', 'RELEASE_NOTES.txt', 'BUILD.txt')
$payloadNames = @('BUILD.txt', 'Compact Cassette Catalogue.exe', 'Compact Cassette Catalogue.exe.config', 'README.txt', 'RELEASE_NOTES.txt', 'UNINSTALL.exe', 'UNINSTALL.exe.config')
$expectedEntries = @('SETUP.exe', 'SETUP.exe.config', 'payload.xml') + @($payloadNames | ForEach-Object { "payload/$_" })
$records = New-Object Collections.Generic.List[Object]

for ($index = 0; $index -lt $lanes.Count; $index++) {
    $lane = $lanes[$index]
    $packagePath = Join-Path $PackageDirectory ([string]$lane.setupPackageName)
    $packageHash = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($checksumLines[$index] -cne "$packageHash  $($lane.setupPackageName)") { throw "$($lane.id) setup package checksum line is not exact." }
    $entryManifestName = "$($lane.setupPackageName).entries.json"
    $entryManifestPath = Join-Path $EvidenceDirectory $entryManifestName
    $entryManifestHash = (Get-FileHash -LiteralPath $entryManifestPath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($entryChecksumLines[$index] -cne "$entryManifestHash  $entryManifestName") { throw "$($lane.id) setup entry-manifest checksum line is not exact." }
    $entryManifest = Get-Content -LiteralPath $entryManifestPath -Raw | ConvertFrom-Json
    $portablePath = Join-Path $PortablePackageDirectory ([string]$lane.packageName)
    $portableEntryManifestPath = Join-Path $PortableEvidenceDirectory "$($lane.packageName).entries.json"
    if ([int]$entryManifest.schemaVersion -ne 1 -or [string]$entryManifest.packageName -cne [string]$lane.setupPackageName -or
            [string]$entryManifest.packageSha256 -cne $packageHash -or [string]$entryManifest.portablePackageName -cne [string]$lane.packageName -or
            [string]$entryManifest.portablePackageSha256 -cne (Get-FileHash -LiteralPath $portablePath -Algorithm SHA256).Hash.ToLowerInvariant() -or
            [string]$entryManifest.portableEntryManifestSha256 -cne (Get-FileHash -LiteralPath $portableEntryManifestPath -Algorithm SHA256).Hash.ToLowerInvariant() -or
            [string]$entryManifest.releaseVersion -cne [string]$manifest.releaseVersion -or [string]$entryManifest.releaseStage -cne [string]$manifest.releaseStage -or
            [string]$entryManifest.releaseLabel -cne [string]$manifest.releaseLabel -or [string]$entryManifest.releaseTag -cne [string]$manifest.releaseTag -or
            [string]$entryManifest.releaseChannel -cne [string]$manifest.releaseChannel -or [string]$entryManifest.publicationStatus -cne [string]$manifest.publicationStatus -or
            [string]$entryManifest.lane -cne [string]$lane.id -or [string]$entryManifest.sourceCommit -notmatch '^[0-9a-f]{40}$' -or
            [string]$entryManifest.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$') {
        throw "$($lane.id) setup entry manifest is not bound to the exact release/portable/source/lock identity."
    }
    if ($RequireCandidateEvidence -and [string]$entryManifest.toolchainMode -cne 'Candidate') { throw "$($lane.id) setup package is not Candidate evidence." }

    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    $portableArchive = [IO.Compression.ZipFile]::OpenRead($portablePath)
    try {
        $entryNames = @($archive.Entries | ForEach-Object { $_.FullName })
        if (($entryNames -join "`n") -cne ($expectedEntries -join "`n")) { throw "$($lane.id) setup ZIP is not the exact ten-entry bundle: $($entryNames -join ', ')" }
        $retainedEntries = @($entryManifest.entries)
        if ($retainedEntries.Count -ne $expectedEntries.Count) { throw "$($lane.id) setup entry manifest must contain exactly ten records." }
        foreach ($entry in $archive.Entries) {
            if ($entry.LastWriteTime.DateTime -ne [DateTime]::new(2000, 1, 1, 0, 0, 0, [DateTimeKind]::Unspecified)) { throw "$($lane.id) '$($entry.FullName)' timestamp is not deterministic." }
            if ($entry.FullName.EndsWith('/') -or $entry.FullName.Contains('\') -or $entry.FullName.Contains('../') -or $entry.FullName -match '(?i)(\.dll$|\.msi$|\.msix$|\.application$|bootstrap|clickonce|updater?)') {
                throw "$($lane.id) contains prohibited or ambiguous setup content '$($entry.FullName)'."
            }
            $record = @($retainedEntries | Where-Object { [string]$_.name -ceq $entry.FullName })
            $hash = Get-EntrySha256 -Entry $entry
            if ($record.Count -ne 1 -or [long]$record[0].size -ne [long]$entry.Length -or [string]$record[0].sha256 -cne $hash) {
                throw "$($lane.id) retained setup entry record does not match '$($entry.FullName)'."
            }
        }
        foreach ($portableName in $portableNames) {
            $sourceEntry = $portableArchive.GetEntry($portableName)
            $setupEntry = $archive.GetEntry("payload/$portableName")
            if ($null -eq $sourceEntry -or $null -eq $setupEntry -or (Get-EntrySha256 -Entry $sourceEntry) -cne (Get-EntrySha256 -Entry $setupEntry)) {
                throw "$($lane.id) setup payload '$portableName' is not byte-identical to the qualified portable package."
            }
        }
        $setupOutput = Join-Path $repositoryRoot "artifacts\setup\bin\$($lane.id)\$Configuration"
        foreach ($mapping in @(
                @('SETUP.exe', (Join-Path $setupOutput 'installer\Compact Cassette Catalogue Installer.exe')),
                @('SETUP.exe.config', (Join-Path $setupOutput 'installer\Compact Cassette Catalogue Installer.exe.config')),
                @('payload/UNINSTALL.exe', (Join-Path $setupOutput 'uninstaller\Compact Cassette Catalogue Uninstaller.exe')),
                @('payload/UNINSTALL.exe.config', (Join-Path $setupOutput 'uninstaller\Compact Cassette Catalogue Uninstaller.exe.config')))) {
            if ((Get-EntrySha256 -Entry ($archive.GetEntry([string]$mapping[0]))) -cne (Get-FileHash -LiteralPath ([string]$mapping[1]) -Algorithm SHA256).Hash.ToLowerInvariant()) {
                throw "$($lane.id) setup entry '$($mapping[0])' does not match its verified build output."
            }
        }

        $xmlEntry = $archive.GetEntry('payload.xml')
        $reader = New-Object IO.StreamReader($xmlEntry.Open(), (New-Object Text.UTF8Encoding($false)), $true)
        try {
            $payloadXmlText = $reader.ReadToEnd()
            [xml]$payloadDocument = $payloadXmlText
        }
        finally { $reader.Dispose() }
        $root = $payloadDocument.DocumentElement
        $product = $root.Product
        if ($root.Name -cne 'C3SetupPayload' -or [string]$root.schemaVersion -cne '1' -or
                [string]$product.version -cne [string]$manifest.releaseVersion -or [string]$product.stage -cne [string]$manifest.releaseStage -or
                [string]$product.label -cne [string]$manifest.releaseLabel -or [string]$product.lane -cne [string]$lane.id -or
                [string]$product.architecture -cne [string]$lane.platformTarget -or [string]$product.framework -cne [string]$lane.targetFramework -or
                [string]$product.sourceCommit -cne [string]$entryManifest.sourceCommit) {
            throw "$($lane.id) payload.xml product identity is not exact."
        }
        $fileNodes = @($root.Files.File)
        if ($fileNodes.Count -ne $payloadNames.Count) { throw "$($lane.id) payload.xml does not contain exactly seven files." }
        for ($fileIndex = 0; $fileIndex -lt $payloadNames.Count; $fileIndex++) {
            $fileNode = $fileNodes[$fileIndex]
            $payloadEntry = $archive.GetEntry("payload/$($payloadNames[$fileIndex])")
            if ([string]$fileNode.path -cne $payloadNames[$fileIndex] -or [long]$fileNode.size -ne [long]$payloadEntry.Length -or
                    [string]$fileNode.sha256 -cne (Get-EntrySha256 -Entry $payloadEntry)) {
                throw "$($lane.id) payload.xml does not authenticate '$($payloadNames[$fileIndex])'."
            }
        }
        $escape = { param([string]$value) [Security.SecurityElement]::Escape($value) }
        $expectedXml = New-Object Collections.Generic.List[String]
        $expectedXml.Add('<?xml version="1.0" encoding="utf-8"?>')
        $expectedXml.Add('<C3SetupPayload schemaVersion="1">')
        $expectedXml.Add(('  <Product version="{0}" stage="{1}" label="{2}" lane="{3}" architecture="{4}" framework="{5}" sourceCommit="{6}" />' -f `
                    (& $escape ([string]$manifest.releaseVersion)), (& $escape ([string]$manifest.releaseStage)), (& $escape ([string]$manifest.releaseLabel)),
                    (& $escape ([string]$lane.id)), (& $escape ([string]$lane.platformTarget)), (& $escape ([string]$lane.targetFramework)), [string]$entryManifest.sourceCommit))
        $expectedXml.Add('  <Files>')
        foreach ($payloadName in $payloadNames) {
            $payloadEntry = $archive.GetEntry("payload/$payloadName")
            $expectedXml.Add(('    <File path="{0}" size="{1}" sha256="{2}" />' -f (& $escape $payloadName), $payloadEntry.Length, (Get-EntrySha256 -Entry $payloadEntry)))
        }
        $expectedXml.Add('  </Files>')
        $expectedXml.Add('</C3SetupPayload>')
        if ($payloadXmlText -cne (($expectedXml -join "`n") + "`n")) {
            throw "$($lane.id) payload.xml is not the exact canonical manifest byte projection."
        }
    }
    finally { $portableArchive.Dispose(); $archive.Dispose() }

    $setupEvidencePath = Join-Path $repositoryRoot "artifacts\evidence\setup-build\$($lane.id)\$Configuration\toolchain.json"
    $setupEvidence = Get-Content -LiteralPath $setupEvidencePath -Raw | ConvertFrom-Json
    if ([string]$setupEvidence.sourceCommit -cne [string]$entryManifest.sourceCommit -or
            [string]$setupEvidence.toolchainLockSha256 -cne [string]$entryManifest.toolchainLockSha256 -or
            [string]$setupEvidence.classification -cne [string]$entryManifest.toolchainMode) {
        throw "$($lane.id) setup package does not match setup-build evidence."
    }
    $records.Add([PSCustomObject]@{
            lane = [string]$lane.id; releaseVersion = [string]$entryManifest.releaseVersion; releaseStage = [string]$entryManifest.releaseStage
            releaseLabel = [string]$entryManifest.releaseLabel; releaseTag = [string]$entryManifest.releaseTag; releaseChannel = [string]$entryManifest.releaseChannel
            publicationStatus = [string]$entryManifest.publicationStatus; sourceCommit = [string]$entryManifest.sourceCommit
            toolchainMode = [string]$entryManifest.toolchainMode; toolchainLockStatus = $(if ([string]$entryManifest.toolchainMode -ceq 'Candidate') { 'locked' } else { 'template' })
            toolchainLockSha256 = [string]$entryManifest.toolchainLockSha256
        })
    Write-Host "Verified offline setup package: $($lane.setupPackageName) ($packageHash)"
}

$setIdentity = Assert-C3PackageEvidenceSet -Records @($records.ToArray()) -RequireCandidate:$RequireCandidateEvidence
& (Join-Path $PSScriptRoot 'verify-setup-builds.ps1') -Configuration $Configuration -ExpectedSourceCommit ([string]$setIdentity.sourceCommit)
Write-Host 'Verified exactly three deterministic setup ZIPs, exact portable-byte reuse, closed manifests, build-output identity, checksums, and one source/toolchain evidence set.'
