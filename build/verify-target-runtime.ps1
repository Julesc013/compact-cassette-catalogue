[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('win-x86-net40', 'win-x64-net48', 'win-arm64-net481')]
    [string]$Lane,
    [Parameter(Mandatory = $true)][string]$PackagePath,
    [Parameter(Mandatory = $true)][string]$ExtractedDirectory,
    [Parameter(Mandatory = $true)][string]$ExpectedPackageSha256,
    [Parameter(Mandatory = $true)][string]$TargetEnvironmentId,
    [Parameter(Mandatory = $true)][string]$Operator,
    [string]$EvidenceOutput
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Get-Sha256 {
    param([Parameter(Mandatory = $true)][string]$Path)

    $algorithm = New-Object Security.Cryptography.SHA256CryptoServiceProvider
    $stream = [IO.File]::Open($Path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::Read)
    try {
        return ([BitConverter]::ToString($algorithm.ComputeHash($stream))).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $stream.Dispose()
        $algorithm.Dispose()
    }
}

function Get-NativeArchitecture {
    $value = [string]$env:PROCESSOR_ARCHITEW6432
    if ([string]::IsNullOrWhiteSpace($value)) {
        $value = [string]$env:PROCESSOR_ARCHITECTURE
    }
    switch -Regex ($value) {
        '^(?i:AMD64|X64)$' { return 'x64' }
        '^(?i:X86|I386)$' { return 'x86' }
        '^(?i:ARM64|AARCH64)$' { return 'ARM64' }
        default { throw "Unsupported or unknown native host architecture '$value'." }
    }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$runtimeLanes = @(& (Join-Path $PSScriptRoot 'get-runtime-lanes.ps1'))
$laneContract = @($runtimeLanes | Where-Object { $_.id -ceq $Lane })
if ($laneContract.Count -ne 1) {
    throw "Release manifest does not contain exactly one lane '$Lane'."
}
$laneContract = $laneContract[0]
$PackagePath = [IO.Path]::GetFullPath($PackagePath)
$ExtractedDirectory = [IO.Path]::GetFullPath($ExtractedDirectory)
if (-not (Test-Path -LiteralPath $PackagePath -PathType Leaf)) {
    throw "Transferred package is missing: $PackagePath"
}
if ([IO.Path]::GetFileName($PackagePath) -cne [string]$laneContract.packageName) {
    throw "Transferred package name '$([IO.Path]::GetFileName($PackagePath))' does not match '$($laneContract.packageName)'."
}
if (-not (Test-Path -LiteralPath $ExtractedDirectory -PathType Container)) {
    throw "Extracted package directory is missing: $ExtractedDirectory"
}

$expectedEntries = @(
    'BUILD.txt',
    'Compact Cassette Catalogue.exe',
    'Compact Cassette Catalogue.exe.config',
    'README.txt',
    'RELEASE_NOTES.txt'
)
$actualEntries = @(Get-ChildItem -LiteralPath $ExtractedDirectory | Where-Object { -not $_.PSIsContainer } | Sort-Object Name | ForEach-Object { $_.Name })
if (($actualEntries -join "`n") -cne ($expectedEntries -join "`n")) {
    throw "Extracted payload does not match the exact portable allow-list: $($actualEntries -join ', ')"
}

$packageHash = Get-Sha256 -Path $PackagePath
if ($packageHash -cne $ExpectedPackageSha256.ToLowerInvariant()) {
    throw "Transferred package SHA-256 '$packageHash' does not match expected '$ExpectedPackageSha256'."
}
$buildPath = Join-Path $ExtractedDirectory 'BUILD.txt'
$buildData = ConvertFrom-StringData ([IO.File]::ReadAllText($buildPath))
$executable = Join-Path $ExtractedDirectory 'Compact Cassette Catalogue.exe'
$config = $executable + '.config'
$executableHash = Get-Sha256 -Path $executable
$configHash = Get-Sha256 -Path $config
if ([string]$buildData.lane -cne $Lane -or
        [string]$buildData.targetFramework -cne [string]$laneContract.targetFramework -or
        [string]$buildData.peMachine -cne [string]$laneContract.peMachine -or
        [string]$buildData.executableSha256 -cne $executableHash -or
        [string]$buildData.configSha256 -cne $configHash) {
    throw 'Extracted EXE/config bytes or BUILD.txt do not match the selected release-lane contract.'
}

$nativeArchitecture = Get-NativeArchitecture
if ($nativeArchitecture -cne [string]$laneContract.runtimeArchitecture) {
    throw "$Lane requires native '$($laneContract.runtimeArchitecture)' target architecture, found '$nativeArchitecture'. Emulation is not qualification."
}
if ($TargetEnvironmentId -cne [string]$laneContract.runtimeEnvironmentId) {
    throw "$Lane requires target environment ID '$($laneContract.runtimeEnvironmentId)', found '$TargetEnvironmentId'."
}

& (Join-Path $PSScriptRoot 'smoke-launch.ps1') `
    -LaneId $Lane `
    -ExecutablePath $executable `
    -ProofMode TargetQualification `
    -TargetEnvironmentId $TargetEnvironmentId

if ([string]::IsNullOrWhiteSpace($EvidenceOutput)) {
    $EvidenceOutput = Join-Path (Get-Location) "$Lane-runtime-evidence.txt"
}
$evidenceLines = @(
    'formatVersion=1',
    "lane=$Lane",
    "targetEnvironmentId=$TargetEnvironmentId",
    "runtimeClaim=$($laneContract.runtimeClaim)",
    "operator=$Operator",
    "machineName=$([Environment]::MachineName)",
    "osVersion=$([Environment]::OSVersion.VersionString)",
    "nativeArchitecture=$nativeArchitecture",
    "packageName=$($laneContract.packageName)",
    "packageSha256=$packageHash",
    "executableSha256=$executableHash",
    "configSha256=$configHash",
    "sourceCommit=$($buildData.sourceCommit)",
    "toolchainMode=$($buildData.toolchainMode)",
    'launchSmoke=pass',
    "recordedAtUtc=$([DateTime]::UtcNow.ToString('o'))"
)
[IO.File]::WriteAllLines([IO.Path]::GetFullPath($EvidenceOutput), [string[]]$evidenceLines, (New-Object Text.UTF8Encoding($false)))
Write-Host "Target runtime launch proof passed and was recorded at '$EvidenceOutput'. Complete the manual workflow record before qualifying the lane."
