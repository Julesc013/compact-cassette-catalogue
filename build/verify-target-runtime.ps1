[CmdletBinding()]
param(
    [ValidateSet('win-x86-net40', 'win-x64-net48', 'win-arm64-net481')]
    [string]$Lane,
    [string]$PackagePath,
    [string]$ExtractedDirectory,
    [string]$ExpectedPackageSha256,
    [string]$EntryManifestPath,
    [string]$ExpectedEntryManifestSha256,
    [Alias('TargetEnvironmentId')]
    [string]$AssertedTargetEnvironmentId,
    [string]$Operator,
    [string]$EvidenceOutput,
    [switch]$SelfTest
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Test-StringBlank {
    param([string]$Value)
    return [string]::IsNullOrEmpty($Value) -or $Value.Trim().Length -eq 0
}

function Get-Sha256 {
    param([Parameter(Mandatory = $true)][string]$Path)

    $algorithm = New-Object Security.Cryptography.SHA256CryptoServiceProvider
    $stream = [IO.File]::Open($Path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::Read)
    try {
        return ([BitConverter]::ToString($algorithm.ComputeHash($stream))).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $stream.Close()
        $algorithm.Clear()
    }
}

function Get-ManifestStringProperty {
    param(
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$Name
    )

    $pattern = '"' + [Regex]::Escape($Name) + '"\s*:\s*"(?<value>[^"\\]*)"'
    $matches = [Regex]::Matches($Text, $pattern, [Text.RegularExpressions.RegexOptions]::Singleline)
    if ($matches.Count -ne 1) {
        throw "Retained entry manifest must contain exactly one unescaped '$Name' string property."
    }
    return [string]$matches[0].Groups['value'].Value
}

function ConvertFrom-EntryManifestJson {
    param([Parameter(Mandatory = $true)][string]$Text)

    $schemaMatches = [Regex]::Matches($Text, '"schemaVersion"\s*:\s*(?<value>[0-9]+)', [Text.RegularExpressions.RegexOptions]::Singleline)
    if ($schemaMatches.Count -ne 1 -or [int]$schemaMatches[0].Groups['value'].Value -ne 2) {
        throw 'Retained entry manifest must contain exactly one supported schemaVersion.'
    }

    # The builder writes this closed, canonical entry object shape. The target parses only
    # that shape so Windows PowerShell 2 does not depend on a JSON assembly or modern CLR.
    $entryPattern = '\{\s*"name"\s*:\s*"(?<name>[^"\\]+)"\s*,\s*"size"\s*:\s*(?<size>[0-9]+)\s*,\s*"sha256"\s*:\s*"(?<sha256>[0-9a-f]{64})"\s*\}'
    $entryMatches = [Regex]::Matches($Text, $entryPattern, [Text.RegularExpressions.RegexOptions]::Singleline)
    $entries = @($entryMatches | ForEach-Object {
        New-Object PSObject -Property @{
            name = [string]$_.Groups['name'].Value
            size = [int64]$_.Groups['size'].Value
            sha256 = [string]$_.Groups['sha256'].Value
        }
    })
    return New-Object PSObject -Property @{
        schemaVersion = 2
        packageName = Get-ManifestStringProperty -Text $Text -Name 'packageName'
        packageSha256 = Get-ManifestStringProperty -Text $Text -Name 'packageSha256'
        releaseVersion = Get-ManifestStringProperty -Text $Text -Name 'releaseVersion'
        releaseStage = Get-ManifestStringProperty -Text $Text -Name 'releaseStage'
        releaseLabel = Get-ManifestStringProperty -Text $Text -Name 'releaseLabel'
        releaseTag = Get-ManifestStringProperty -Text $Text -Name 'releaseTag'
        releaseChannel = Get-ManifestStringProperty -Text $Text -Name 'releaseChannel'
        publicationStatus = Get-ManifestStringProperty -Text $Text -Name 'publicationStatus'
        sourceCommit = Get-ManifestStringProperty -Text $Text -Name 'sourceCommit'
        toolchainLockSha256 = Get-ManifestStringProperty -Text $Text -Name 'toolchainLockSha256'
        entries = $entries
    }
}

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repositoryRoot = Split-Path -Parent $scriptRoot
. (Join-Path $scriptRoot 'target-environment.ps1')
$runtimeLanes = @(& (Join-Path $scriptRoot 'get-runtime-lanes.ps1'))
if ($SelfTest) {
    if ((Test-StringBlank $scriptRoot) -or $runtimeLanes.Count -ne 3) {
        throw 'verify-target-runtime.ps1 PowerShell 2 self-test could not resolve its script root or lane projection.'
    }
    $selfHash = Get-Sha256 -Path (Join-Path $scriptRoot 'get-runtime-lanes.ps1')
    if ($selfHash -notmatch '^[0-9a-f]{64}$') {
        throw 'verify-target-runtime.ps1 PowerShell 2 self-test could not compute SHA-256.'
    }
    $selfManifest = ConvertFrom-EntryManifestJson '{"schemaVersion":2,"packageName":"test.zip","packageSha256":"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa","releaseVersion":"1.3.0","releaseStage":"Alpha 2","releaseLabel":"1.3.0a2","releaseTag":"v1.3.0a2","releaseChannel":"alpha","publicationStatus":"retained-unpublished","sourceCommit":"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb","toolchainLockSha256":"cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc","entries":[{"name":"BUILD.txt","size":1,"sha256":"dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd"}]}'
    if ([string]$selfManifest.packageName -cne 'test.zip' -or @($selfManifest.entries).Count -ne 1) {
        throw 'verify-target-runtime.ps1 PowerShell 2 self-test could not parse retained entry-manifest evidence.'
    }
    & (Join-Path $scriptRoot 'test-target-environment.ps1')
    Write-Host "verify-target-runtime.ps1 PowerShell 2 self-test passed at '$scriptRoot'."
    return
}
if (-not (Test-StringBlank $AssertedTargetEnvironmentId)) {
    throw 'Caller-supplied -TargetEnvironmentId is prohibited; target identity is derived mechanically from OS, architecture, and framework facts.'
}
foreach ($requiredValue in @(
        @('Lane', $Lane),
        @('PackagePath', $PackagePath),
        @('ExtractedDirectory', $ExtractedDirectory),
        @('ExpectedPackageSha256', $ExpectedPackageSha256),
        @('EntryManifestPath', $EntryManifestPath),
        @('ExpectedEntryManifestSha256', $ExpectedEntryManifestSha256),
        @('Operator', $Operator))) {
    if (Test-StringBlank ([string]$requiredValue[1])) {
        throw "-$($requiredValue[0]) is required for target runtime verification."
    }
}
$laneContract = @($runtimeLanes | Where-Object { $_.id -ceq $Lane })
if ($laneContract.Count -ne 1) {
    throw "Release manifest does not contain exactly one lane '$Lane'."
}
$laneContract = $laneContract[0]
$PackagePath = [IO.Path]::GetFullPath($PackagePath)
$ExtractedDirectory = [IO.Path]::GetFullPath($ExtractedDirectory)
$EntryManifestPath = [IO.Path]::GetFullPath($EntryManifestPath)
if (-not (Test-Path -LiteralPath $PackagePath -PathType Leaf)) {
    throw "Transferred package is missing: $PackagePath"
}
if ([IO.Path]::GetFileName($PackagePath) -cne [string]$laneContract.packageName) {
    throw "Transferred package name '$([IO.Path]::GetFileName($PackagePath))' does not match '$($laneContract.packageName)'."
}
if (-not (Test-Path -LiteralPath $ExtractedDirectory -PathType Container)) {
    throw "Extracted package directory is missing: $ExtractedDirectory"
}
if (-not (Test-Path -LiteralPath $EntryManifestPath -PathType Leaf)) {
    throw "Retained package-entry manifest is missing: $EntryManifestPath"
}

$expectedEntries = @(
    'Compact Cassette Catalogue.exe',
    'Compact Cassette Catalogue.exe.config',
    'README.txt',
    'RELEASE_NOTES.txt',
    'BUILD.txt'
)
$allExtractedItems = @(Get-ChildItem -LiteralPath $ExtractedDirectory)
$invalidExtractedItems = @($allExtractedItems | Where-Object {
    $_.PSIsContainer -or (($_.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0)
})
if ($invalidExtractedItems.Count -ne 0) {
    $invalidNames = @($invalidExtractedItems | ForEach-Object { $_.Name })
    throw "Extracted payload contains a directory or reparse point: $($invalidNames -join ', ')"
}
$actualFiles = @($allExtractedItems | Sort-Object Name)
$actualEntries = @($actualFiles | ForEach-Object { $_.Name })
if (($actualEntries -join "`n") -cne (($expectedEntries | Sort-Object) -join "`n")) {
    throw "Extracted payload does not match the exact portable allow-list: $($actualEntries -join ', ')"
}

$packageHash = Get-Sha256 -Path $PackagePath
if ($packageHash -cne $ExpectedPackageSha256.ToLowerInvariant()) {
    throw "Transferred package SHA-256 '$packageHash' does not match expected '$ExpectedPackageSha256'."
}
$entryManifestHash = Get-Sha256 -Path $EntryManifestPath
if ($entryManifestHash -cne $ExpectedEntryManifestSha256.ToLowerInvariant()) {
    throw "Retained entry-manifest SHA-256 '$entryManifestHash' does not match expected '$ExpectedEntryManifestSha256'."
}
$entryManifest = ConvertFrom-EntryManifestJson ([IO.File]::ReadAllText($EntryManifestPath))
if ([string]$entryManifest.packageName -cne [string]$laneContract.packageName -or
        [string]$entryManifest.packageSha256 -cne $packageHash -or
        [string]$entryManifest.releaseVersion -cne [string]$laneContract.releaseVersion -or
        [string]$entryManifest.releaseStage -cne [string]$laneContract.releaseStage -or
        [string]$entryManifest.releaseLabel -cne [string]$laneContract.releaseLabel -or
        [string]$entryManifest.releaseTag -cne [string]$laneContract.releaseTag -or
        [string]$entryManifest.releaseChannel -cne [string]$laneContract.releaseChannel -or
        [string]$entryManifest.publicationStatus -cne [string]$laneContract.publicationStatus -or
        [string]$entryManifest.sourceCommit -notmatch '^[0-9a-f]{40}$' -or
        [string]$entryManifest.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$') {
    throw 'Retained entry manifest is not bound to the selected package, source, and toolchain lock.'
}
$manifestEntries = @($entryManifest.entries)
if ($manifestEntries.Count -ne $expectedEntries.Count) {
    throw "Retained entry manifest contains $($manifestEntries.Count) entries; expected exactly five."
}
$manifestNames = @($manifestEntries | ForEach-Object { [string]$_.name } | Sort-Object)
if (($manifestNames -join "`n") -cne (($expectedEntries | Sort-Object) -join "`n")) {
    throw "Retained entry manifest does not contain the exact portable allow-list: $($manifestNames -join ', ')"
}
foreach ($actualFile in $actualFiles) {
    $entryRecord = @($manifestEntries | Where-Object { [string]$_.name -ceq $actualFile.Name })
    if ($entryRecord.Count -ne 1 -or [int64]$entryRecord[0].size -ne [int64]$actualFile.Length) {
        throw "Extracted file name/size does not match retained entry manifest: $($actualFile.Name)"
    }
    $actualFileHash = Get-Sha256 -Path $actualFile.FullName
    if ($actualFileHash -cne [string]$entryRecord[0].sha256) {
        throw "Extracted file SHA-256 does not match retained entry manifest: $($actualFile.Name)"
    }
}

$buildPath = Join-Path $ExtractedDirectory 'BUILD.txt'
$buildData = ConvertFrom-StringData ([IO.File]::ReadAllText($buildPath))
$executable = Join-Path $ExtractedDirectory 'Compact Cassette Catalogue.exe'
$config = $executable + '.config'
$executableHash = Get-Sha256 -Path $executable
$configHash = Get-Sha256 -Path $config
if ([string]$buildData.formatVersion -cne '2' -or
        [string]$buildData.lane -cne $Lane -or
        [string]$buildData.releaseVersion -cne [string]$laneContract.releaseVersion -or
        [string]$buildData.releaseStage -cne [string]$laneContract.releaseStage -or
        [string]$buildData.releaseLabel -cne [string]$laneContract.releaseLabel -or
        [string]$buildData.releaseTag -cne [string]$laneContract.releaseTag -or
        [string]$buildData.releaseChannel -cne [string]$laneContract.releaseChannel -or
        [string]$buildData.publicationStatus -cne [string]$laneContract.publicationStatus -or
        [string]$buildData.targetFramework -cne [string]$laneContract.targetFramework -or
        [string]$buildData.peMachine -cne [string]$laneContract.peMachine -or
        [string]$buildData.sourceCommit -cne [string]$entryManifest.sourceCommit -or
        [string]$buildData.toolchainLockSha256 -cne [string]$entryManifest.toolchainLockSha256 -or
        ([string]$buildData.toolchainMode -ceq 'Candidate' -and [string]$buildData.toolchainLockStatus -cne 'locked') -or
        ([string]$buildData.toolchainMode -ceq 'Preparation' -and [string]$buildData.toolchainLockStatus -cne 'template') -or
        [string]$buildData.executableSha256 -cne $executableHash -or
        [string]$buildData.configSha256 -cne $configHash) {
    throw 'Extracted EXE/config bytes or BUILD.txt do not match the selected release-lane contract.'
}

$targetFacts = Get-C3TargetEnvironmentFacts
$derivedTargetEnvironmentId = Assert-C3TargetEnvironment -LaneContract $laneContract -Facts $targetFacts

& (Join-Path $scriptRoot 'smoke-launch.ps1') `
    -LaneId $Lane `
    -ExecutablePath $executable `
    -ProofMode TargetQualification `
    -TargetEnvironmentId $derivedTargetEnvironmentId

if (Test-StringBlank $EvidenceOutput) {
    $EvidenceOutput = Join-Path (Get-Location) "$Lane-runtime-evidence.txt"
}
$evidenceLines = @(
    'formatVersion=2',
    "lane=$Lane",
    "releaseVersion=$($laneContract.releaseVersion)",
    "releaseStage=$($laneContract.releaseStage)",
    "releaseLabel=$($laneContract.releaseLabel)",
    "releaseTag=$($laneContract.releaseTag)",
    "releaseChannel=$($laneContract.releaseChannel)",
    "publicationStatus=$($laneContract.publicationStatus)",
    "targetEnvironmentId=$derivedTargetEnvironmentId",
    "runtimeClaim=$($laneContract.runtimeClaim)",
    "operator=$Operator",
    "machineName=$([Environment]::MachineName)",
    "osVersion=$($targetFacts.osVersion)",
    "osBuild=$($targetFacts.osBuild)",
    "servicePackMajor=$($targetFacts.servicePackMajor)",
    "servicePackMinor=$($targetFacts.servicePackMinor)",
    "nativeArchitecture=$($targetFacts.nativeArchitecture)",
    "frameworkFullInstalled=$($targetFacts.frameworkFullInstalled)",
    "frameworkVersion=$($targetFacts.frameworkVersion)",
    "frameworkRelease=$($targetFacts.frameworkRelease)",
    "packageName=$($laneContract.packageName)",
    "packageSha256=$packageHash",
    "entryManifestSha256=$entryManifestHash",
    "executableSha256=$executableHash",
    "configSha256=$configHash",
    "sourceCommit=$($buildData.sourceCommit)",
    "toolchainMode=$($buildData.toolchainMode)",
    'launchSmoke=pass',
    "recordedAtUtc=$([DateTime]::UtcNow.ToString('o'))"
)
[IO.File]::WriteAllLines([IO.Path]::GetFullPath($EvidenceOutput), [string[]]$evidenceLines, (New-Object Text.UTF8Encoding($false)))
Write-Host "Target runtime launch proof passed and was recorded at '$EvidenceOutput'. Complete the manual workflow record before qualifying the lane."
