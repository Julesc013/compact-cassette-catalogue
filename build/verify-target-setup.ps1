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
    [switch]$LaunchSetup,
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
        throw "Retained setup entry manifest must contain exactly one unescaped '$Name' string property."
    }
    return [string]$matches[0].Groups['value'].Value
}

function ConvertFrom-SetupEntryManifestJson {
    param([Parameter(Mandatory = $true)][string]$Text)

    $schemaMatches = [Regex]::Matches($Text, '"schemaVersion"\s*:\s*(?<value>[0-9]+)', [Text.RegularExpressions.RegexOptions]::Singleline)
    if ($schemaMatches.Count -ne 1 -or [int]$schemaMatches[0].Groups['value'].Value -ne 1) {
        throw 'Retained setup entry manifest must contain exactly one supported schemaVersion.'
    }

    # The builder writes this closed, canonical entry object shape. Restricting the
    # parser to that shape keeps the target verifier independent of modern JSON APIs.
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
        schemaVersion = 1
        packageName = Get-ManifestStringProperty -Text $Text -Name 'packageName'
        packageSha256 = Get-ManifestStringProperty -Text $Text -Name 'packageSha256'
        portablePackageName = Get-ManifestStringProperty -Text $Text -Name 'portablePackageName'
        portablePackageSha256 = Get-ManifestStringProperty -Text $Text -Name 'portablePackageSha256'
        portableEntryManifestSha256 = Get-ManifestStringProperty -Text $Text -Name 'portableEntryManifestSha256'
        releaseVersion = Get-ManifestStringProperty -Text $Text -Name 'releaseVersion'
        releaseStage = Get-ManifestStringProperty -Text $Text -Name 'releaseStage'
        releaseLabel = Get-ManifestStringProperty -Text $Text -Name 'releaseLabel'
        releaseTag = Get-ManifestStringProperty -Text $Text -Name 'releaseTag'
        releaseChannel = Get-ManifestStringProperty -Text $Text -Name 'releaseChannel'
        publicationStatus = Get-ManifestStringProperty -Text $Text -Name 'publicationStatus'
        lane = Get-ManifestStringProperty -Text $Text -Name 'lane'
        toolchainMode = Get-ManifestStringProperty -Text $Text -Name 'toolchainMode'
        sourceCommit = Get-ManifestStringProperty -Text $Text -Name 'sourceCommit'
        toolchainLockSha256 = Get-ManifestStringProperty -Text $Text -Name 'toolchainLockSha256'
        entries = $entries
    }
}

function Get-IsAdministrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
. (Join-Path $scriptRoot 'target-environment.ps1')
$runtimeLanes = @(& (Join-Path $scriptRoot 'get-runtime-lanes.ps1'))

if ($SelfTest) {
    if ((Test-StringBlank $scriptRoot) -or $runtimeLanes.Count -ne 3) {
        throw 'verify-target-setup.ps1 PowerShell 2 self-test could not resolve its script root or lane projection.'
    }
    foreach ($selfLane in $runtimeLanes) {
        if (Test-StringBlank ([string]$selfLane.setupPackageName)) {
            throw 'verify-target-setup.ps1 PowerShell 2 self-test found an incomplete setup-package projection.'
        }
    }
    $selfHash = Get-Sha256 -Path (Join-Path $scriptRoot 'get-runtime-lanes.ps1')
    if ($selfHash -notmatch '^[0-9a-f]{64}$') {
        throw 'verify-target-setup.ps1 PowerShell 2 self-test could not compute SHA-256.'
    }
    $selfManifest = ConvertFrom-SetupEntryManifestJson '{"schemaVersion":1,"packageName":"setup.zip","packageSha256":"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa","portablePackageName":"portable.zip","portablePackageSha256":"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb","portableEntryManifestSha256":"cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc","releaseVersion":"1.3.0","releaseStage":"Alpha 3","releaseLabel":"1.3.0a3","releaseTag":"v1.3.0a3","releaseChannel":"alpha","publicationStatus":"retained-unpublished","lane":"win-x86-net40","sourceCommit":"dddddddddddddddddddddddddddddddddddddddd","toolchainMode":"Candidate","toolchainLockSha256":"eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee","entries":[{"name":"SETUP.exe","size":1,"sha256":"ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"}]}'
    if ([string]$selfManifest.packageName -cne 'setup.zip' -or @($selfManifest.entries).Count -ne 1) {
        throw 'verify-target-setup.ps1 PowerShell 2 self-test could not parse retained setup entry-manifest evidence.'
    }
    Write-Host "verify-target-setup.ps1 PowerShell 2 self-test passed at '$scriptRoot'."
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
        throw "-$($requiredValue[0]) is required for target setup verification."
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
if (-not (Test-Path -LiteralPath $PackagePath -PathType Leaf)) { throw "Transferred setup package is missing: $PackagePath" }
if ([IO.Path]::GetFileName($PackagePath) -cne [string]$laneContract.setupPackageName) {
    throw "Transferred setup package name '$([IO.Path]::GetFileName($PackagePath))' does not match '$($laneContract.setupPackageName)'."
}
if (-not (Test-Path -LiteralPath $ExtractedDirectory -PathType Container)) { throw "Extracted setup directory is missing: $ExtractedDirectory" }
if (-not (Test-Path -LiteralPath $EntryManifestPath -PathType Leaf)) { throw "Retained setup entry manifest is missing: $EntryManifestPath" }

$expectedRootFiles = @('SETUP.exe', 'SETUP.exe.config', 'payload.xml')
$expectedPayloadFiles = @(
    'BUILD.txt',
    'Compact Cassette Catalogue.exe',
    'Compact Cassette Catalogue.exe.config',
    'README.txt',
    'RELEASE_NOTES.txt',
    'UNINSTALL.exe',
    'UNINSTALL.exe.config'
)
$expectedManifestEntries = @($expectedRootFiles)
foreach ($payloadName in $expectedPayloadFiles) { $expectedManifestEntries += "payload/$payloadName" }

$rootItem = Get-Item -LiteralPath $ExtractedDirectory
if (($rootItem.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
    throw 'Extracted setup root must not be a reparse point.'
}
$rootItems = @(Get-ChildItem -LiteralPath $ExtractedDirectory)
$rootDirectories = @($rootItems | Where-Object { $_.PSIsContainer })
$rootFiles = @($rootItems | Where-Object { -not $_.PSIsContainer })
if ($rootDirectories.Count -ne 1 -or $rootDirectories[0].Name -cne 'payload' -or
        (($rootDirectories[0].Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0)) {
    throw 'Extracted setup must contain exactly one ordinary payload directory.'
}
$invalidRootFiles = @($rootFiles | Where-Object { ($_.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0 })
if ($invalidRootFiles.Count -ne 0) {
    throw "Extracted setup root contains a reparse point: $(@($invalidRootFiles | ForEach-Object { $_.Name }) -join ', ')"
}
if ((@($rootFiles | ForEach-Object { $_.Name } | Sort-Object) -join "`n") -cne (($expectedRootFiles | Sort-Object) -join "`n")) {
    throw "Extracted setup root does not match its exact file allow-list: $(@($rootFiles | ForEach-Object { $_.Name }) -join ', ')"
}
$payloadDirectory = $rootDirectories[0].FullName
$payloadItems = @(Get-ChildItem -LiteralPath $payloadDirectory)
$invalidPayloadItems = @($payloadItems | Where-Object {
    $_.PSIsContainer -or (($_.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0)
})
if ($invalidPayloadItems.Count -ne 0) {
    throw "Extracted setup payload contains a directory or reparse point: $(@($invalidPayloadItems | ForEach-Object { $_.Name }) -join ', ')"
}
if ((@($payloadItems | ForEach-Object { $_.Name } | Sort-Object) -join "`n") -cne (($expectedPayloadFiles | Sort-Object) -join "`n")) {
    throw "Extracted setup payload does not match its exact file allow-list: $(@($payloadItems | ForEach-Object { $_.Name }) -join ', ')"
}

$packageHash = Get-Sha256 -Path $PackagePath
if ($packageHash -cne $ExpectedPackageSha256.ToLowerInvariant()) {
    throw "Transferred setup package SHA-256 '$packageHash' does not match expected '$ExpectedPackageSha256'."
}
$entryManifestHash = Get-Sha256 -Path $EntryManifestPath
if ($entryManifestHash -cne $ExpectedEntryManifestSha256.ToLowerInvariant()) {
    throw "Retained setup entry-manifest SHA-256 '$entryManifestHash' does not match expected '$ExpectedEntryManifestSha256'."
}
$entryManifest = ConvertFrom-SetupEntryManifestJson ([IO.File]::ReadAllText($EntryManifestPath))
if ([string]$entryManifest.packageName -cne [string]$laneContract.setupPackageName -or
        [string]$entryManifest.packageSha256 -cne $packageHash -or
        [string]$entryManifest.portablePackageName -cne [string]$laneContract.packageName -or
        [string]$entryManifest.portablePackageSha256 -notmatch '^[0-9a-f]{64}$' -or
        [string]$entryManifest.portableEntryManifestSha256 -notmatch '^[0-9a-f]{64}$' -or
        [string]$entryManifest.releaseVersion -cne [string]$laneContract.releaseVersion -or
        [string]$entryManifest.releaseStage -cne [string]$laneContract.releaseStage -or
        [string]$entryManifest.releaseLabel -cne [string]$laneContract.releaseLabel -or
        [string]$entryManifest.releaseTag -cne [string]$laneContract.releaseTag -or
        [string]$entryManifest.releaseChannel -cne [string]$laneContract.releaseChannel -or
        [string]$entryManifest.publicationStatus -cne [string]$laneContract.publicationStatus -or
        [string]$entryManifest.lane -cne $Lane -or
        [string]$entryManifest.sourceCommit -notmatch '^[0-9a-f]{40}$' -or
        [string]$entryManifest.toolchainMode -notmatch '^(Preparation|Candidate)$' -or
        [string]$entryManifest.toolchainLockSha256 -notmatch '^[0-9a-f]{64}$') {
    throw 'Retained setup entry manifest is not bound to the selected bundle, portable package, source, and toolchain lock.'
}
$manifestEntries = @($entryManifest.entries)
if ($manifestEntries.Count -ne $expectedManifestEntries.Count) {
    throw "Retained setup entry manifest contains $($manifestEntries.Count) entries; expected exactly ten."
}
$manifestNames = @($manifestEntries | ForEach-Object { [string]$_.name } | Sort-Object)
if (($manifestNames -join "`n") -cne (($expectedManifestEntries | Sort-Object) -join "`n")) {
    throw "Retained setup entry manifest does not contain the exact bundle allow-list: $($manifestNames -join ', ')"
}

$actualFilesByManifestName = @{}
foreach ($rootFile in $rootFiles) { $actualFilesByManifestName[$rootFile.Name] = $rootFile }
foreach ($payloadFile in $payloadItems) { $actualFilesByManifestName["payload/$($payloadFile.Name)"] = $payloadFile }
foreach ($manifestEntry in $manifestEntries) {
    $manifestName = [string]$manifestEntry.name
    $actualFile = $actualFilesByManifestName[$manifestName]
    if ($null -eq $actualFile -or [int64]$manifestEntry.size -ne [int64]$actualFile.Length) {
        throw "Extracted setup file name/size does not match retained entry manifest: $manifestName"
    }
    if ((Get-Sha256 -Path $actualFile.FullName) -cne [string]$manifestEntry.sha256) {
        throw "Extracted setup file SHA-256 does not match retained entry manifest: $manifestName"
    }
}

[xml]$payloadManifest = [IO.File]::ReadAllText((Join-Path $ExtractedDirectory 'payload.xml'))
$product = $payloadManifest.C3SetupPayload.Product
if ([string]$payloadManifest.C3SetupPayload.schemaVersion -cne '1' -or
        [string]$product.version -cne [string]$laneContract.releaseVersion -or
        [string]$product.stage -cne [string]$laneContract.releaseStage -or
        [string]$product.label -cne [string]$laneContract.releaseLabel -or
        [string]$product.lane -cne $Lane -or
        [string]$product.framework -cne [string]$laneContract.targetFramework -or
        [string]$product.sourceCommit -cne [string]$entryManifest.sourceCommit) {
    throw 'Authenticated payload.xml identity does not match the selected setup lane and retained source.'
}

$targetFacts = Get-C3TargetEnvironmentFacts
$derivedTargetEnvironmentId = Assert-C3TargetEnvironment -LaneContract $laneContract -Facts $targetFacts
$callerIsAdministrator = Get-IsAdministrator
$launchState = 'not-requested'
$setupExitCode = ''
if ($LaunchSetup) {
    $setupExecutable = Join-Path $ExtractedDirectory 'SETUP.exe'
    $process = Start-Process -FilePath $setupExecutable -WorkingDirectory $ExtractedDirectory -PassThru
    $process.WaitForExit()
    $setupExitCode = [string]$process.ExitCode
    $launchState = 'completed-manual-result-required'
}

if (Test-StringBlank $EvidenceOutput) {
    $EvidenceOutput = Join-Path (Get-Location) "$Lane-setup-evidence.txt"
}
$evidenceLines = @(
    'formatVersion=1',
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
    "callerIsAdministrator=$callerIsAdministrator",
    "packageName=$($laneContract.setupPackageName)",
    "packageSha256=$packageHash",
    "entryManifestSha256=$entryManifestHash",
    "portablePackageName=$($entryManifest.portablePackageName)",
    "portablePackageSha256=$($entryManifest.portablePackageSha256)",
    "portableEntryManifestSha256=$($entryManifest.portableEntryManifestSha256)",
    "setupExecutableSha256=$(Get-Sha256 -Path (Join-Path $ExtractedDirectory 'SETUP.exe'))",
    "payloadManifestSha256=$(Get-Sha256 -Path (Join-Path $ExtractedDirectory 'payload.xml'))",
    "uninstallerSha256=$(Get-Sha256 -Path (Join-Path $payloadDirectory 'UNINSTALL.exe'))",
    "sourceCommit=$($entryManifest.sourceCommit)",
    "toolchainMode=$($entryManifest.toolchainMode)",
    "toolchainLockSha256=$($entryManifest.toolchainLockSha256)",
    'bundlePreflight=pass',
    "setupLaunch=$launchState",
    "setupExitCode=$setupExitCode",
    "recordedAtUtc=$([DateTime]::UtcNow.ToString('o'))"
)
[IO.File]::WriteAllLines([IO.Path]::GetFullPath($EvidenceOutput), [string[]]$evidenceLines, (New-Object Text.UTF8Encoding($false)))
if ($LaunchSetup) {
    Write-Host "Target setup bundle preflight passed and the wizard returned. Evidence was recorded at '$EvidenceOutput'; the manual mutation/UI record determines qualification."
}
else {
    Write-Host "Target setup bundle preflight passed without launching or mutating the machine. Evidence was recorded at '$EvidenceOutput'. Rerun with -LaunchSetup only for an authorized manual setup case."
}
