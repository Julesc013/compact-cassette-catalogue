[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Get-PeMetadata {
    param([Parameter(Mandatory = $true)][string]$Path)

    $stream = [IO.File]::Open($Path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::Read)
    $reader = New-Object IO.BinaryReader($stream)
    try {
        $stream.Position = 0x3c
        $peOffset = $reader.ReadInt32()
        $stream.Position = $peOffset
        $signature = $reader.ReadUInt32()
        $machine = $reader.ReadUInt16()
        $sectionCount = $reader.ReadUInt16()
        $stream.Position = $peOffset + 20
        $optionalHeaderSize = $reader.ReadUInt16()
        $optionalHeaderOffset = $peOffset + 24
        $stream.Position = $optionalHeaderOffset
        $optionalHeader = $reader.ReadUInt16()

        $dataDirectoryOffset = switch ($optionalHeader) {
            0x010b { $optionalHeaderOffset + 96 }
            0x020b { $optionalHeaderOffset + 112 }
            default { throw ("Unsupported PE optional header 0x{0:x4}." -f $optionalHeader) }
        }
        $stream.Position = $dataDirectoryOffset + (14 * 8)
        $cliHeaderRva = $reader.ReadUInt32()
        $cliHeaderSize = $reader.ReadUInt32()
        if ($cliHeaderRva -eq 0 -or $cliHeaderSize -lt 20) {
            throw 'Executable does not contain a valid CLR header directory.'
        }

        $sectionTableOffset = $optionalHeaderOffset + $optionalHeaderSize
        $cliHeaderOffset = $null
        for ($index = 0; $index -lt $sectionCount; $index++) {
            $stream.Position = $sectionTableOffset + ($index * 40) + 8
            $virtualSize = $reader.ReadUInt32()
            $virtualAddress = $reader.ReadUInt32()
            $rawSize = $reader.ReadUInt32()
            $rawOffset = $reader.ReadUInt32()
            $mappedSize = [Math]::Max([uint64]$virtualSize, [uint64]$rawSize)
            if ([uint64]$cliHeaderRva -ge [uint64]$virtualAddress -and
                    [uint64]$cliHeaderRva -lt ([uint64]$virtualAddress + $mappedSize)) {
                $cliHeaderOffset = [uint64]$rawOffset + ([uint64]$cliHeaderRva - [uint64]$virtualAddress)
                break
            }
        }
        if ($null -eq $cliHeaderOffset) {
            throw ("Could not map CLR header RVA 0x{0:x8} to a PE section." -f $cliHeaderRva)
        }
        $stream.Position = [int64]$cliHeaderOffset + 16
        $corFlags = $reader.ReadUInt32()

        return [PSCustomObject]@{
            signature = $signature
            machine = $machine
            optionalHeader = $optionalHeader
            corFlags = $corFlags
            cliHeaderRva = $cliHeaderRva
            cliHeaderSize = $cliHeaderSize
        }
    }
    finally {
        $reader.Dispose()
        $stream.Dispose()
    }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
$expectedLaneIds = @('win-x86-net40', 'win-x64-net48', 'win-arm64-net481')
if (($lanes.id -join "`n") -cne ($expectedLaneIds -join "`n")) {
    throw "Release manifest must contain exactly these ordered lanes: $($expectedLaneIds -join ', ')."
}
if (@($lanes | Where-Object { [string]$_.status -cne 'required' }).Count -ne 0) {
    throw 'Every C3 1.3 release lane must be required.'
}

$genome = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'baseline-genome.v1.json') -Raw | ConvertFrom-Json
$expectedSettings = @($genome.settings | ForEach-Object { [string]$_.name })
$expectedVersion = ([string]$manifest.releaseVersion) + '.0'
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()

foreach ($lane in $lanes) {
    $outputDirectory = Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration"
    $executable = Join-Path $outputDirectory 'Compact Cassette Catalogue.exe'
    $config = $executable + '.config'
    foreach ($path in @($executable, $config)) {
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Missing $($lane.id) output: $path"
        }
    }

    $pe = Get-PeMetadata -Path $executable
    $expectedMachine = [Convert]::ToUInt16(([string]$lane.peMachine).Substring(2), 16)
    $expectedHeader = [Convert]::ToUInt16(([string]$lane.peOptionalHeader).Substring(2), 16)
    if ($pe.signature -ne 0x00004550 -or $pe.machine -ne $expectedMachine -or
            $pe.optionalHeader -ne $expectedHeader) {
        throw ("{0} PE mismatch: signature=0x{1:x8}, machine=0x{2:x4}, optional=0x{3:x4}." -f
            $lane.id, $pe.signature, $pe.machine, $pe.optionalHeader)
    }

    $corFlagIlOnly = 0x00000001
    $corFlag32BitRequired = 0x00000002
    $corFlag32BitPreferred = 0x00020000
    if (($pe.corFlags -band $corFlagIlOnly) -eq 0) {
        throw ("{0} CorFlags 0x{1:x8} does not include ILONLY." -f $lane.id, $pe.corFlags)
    }
    $has32BitRequired = ($pe.corFlags -band $corFlag32BitRequired) -ne 0
    if ($has32BitRequired -ne [bool]$lane.requires32Bit) {
        throw ("{0} CorFlags 0x{1:x8} has unexpected 32BITREQ state." -f $lane.id, $pe.corFlags)
    }
    if (($pe.corFlags -band $corFlag32BitPreferred) -ne 0) {
        throw ("{0} CorFlags 0x{1:x8} unexpectedly includes 32BITPREFERRED." -f $lane.id, $pe.corFlags)
    }

    $binaryText = [Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($executable))
    $frameworkMoniker = ".NETFramework,Version=$($lane.targetFramework)"
    if (-not $binaryText.Contains($frameworkMoniker)) {
        throw "$($lane.id) executable does not contain TargetFrameworkAttribute '$frameworkMoniker'."
    }

    $assemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($executable).Version.ToString()
    $versionInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($executable)
    if ($assemblyVersion -cne $expectedVersion -or
            [string]$versionInfo.FileVersion -cne $expectedVersion -or
            [string]$versionInfo.ProductVersion -cne $expectedVersion) {
        throw "$($lane.id) version mismatch: assembly=$assemblyVersion, file=$($versionInfo.FileVersion), product=$($versionInfo.ProductVersion), expected=$expectedVersion."
    }

    [xml]$configDocument = Get-Content -LiteralPath $config -Raw
    $runtime = $configDocument.configuration.startup.supportedRuntime
    if ([string]$runtime.version -cne 'v4.0' -or [string]$runtime.sku -cne $frameworkMoniker) {
        throw "$($lane.id) config does not declare supported runtime '$frameworkMoniker'."
    }
    $actualSettings = @($configDocument.configuration.userSettings.'Compact_Cassette_Catalogue.My.MySettings'.setting |
        ForEach-Object { [string]$_.name })
    if (($actualSettings -join "`n") -cne ($expectedSettings -join "`n")) {
        throw "$($lane.id) config settings do not match the baseline genome."
    }

    $runtimeDlls = @(Get-ChildItem -LiteralPath $outputDirectory -Filter '*.dll' -File)
    if ($runtimeDlls.Count -ne 0) {
        throw "$($lane.id) unexpectedly produced runtime DLLs: $($runtimeDlls.Name -join ', ')"
    }

    $toolchainEvidencePath = Join-Path $repositoryRoot "artifacts\evidence\build\$($lane.id)\$Configuration\toolchain.json"
    if (-not (Test-Path -LiteralPath $toolchainEvidencePath -PathType Leaf)) {
        throw "$($lane.id) is missing toolchain evidence: $toolchainEvidencePath"
    }
    $toolchainEvidence = Get-Content -LiteralPath $toolchainEvidencePath -Raw | ConvertFrom-Json
    if ([string]$toolchainEvidence.lane -cne [string]$lane.id -or
            [string]$toolchainEvidence.source.commit -cne $sourceCommit -or
            [string]$toolchainEvidence.msbuild.effectiveToolsVersion -cne [string]$lane.effectiveToolsVersion) {
        throw "$($lane.id) toolchain evidence does not match the lane, source commit, or effective tools version."
    }

    Write-Host ("Verified {0}: machine=0x{1:x4}, header=0x{2:x4}, CorFlags=0x{3:x8}, framework={4}, version={5}, settings={6}, runtime DLLs=0" -f
        $lane.id, $pe.machine, $pe.optionalHeader, $pe.corFlags, $lane.targetFramework, $expectedVersion, $actualSettings.Count)
}
