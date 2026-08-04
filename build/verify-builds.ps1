[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$lanes = @((Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json).lanes)
$genome = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'baseline-genome.v1.json') -Raw | ConvertFrom-Json
$expectedSettings = @($genome.settings | ForEach-Object { [string]$_.name })

foreach ($lane in $lanes) {
    $outputDirectory = Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration"
    $executable = Join-Path $outputDirectory 'Compact Cassette Catalogue.exe'
    $config = $executable + '.config'
    foreach ($path in @($executable, $config)) {
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Missing $($lane.id) output: $path"
        }
    }

    $stream = [IO.File]::Open($executable, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::Read)
    $reader = New-Object IO.BinaryReader($stream)
    try {
        $stream.Position = 0x3c
        $peOffset = $reader.ReadInt32()
        $stream.Position = $peOffset
        $signature = $reader.ReadUInt32()
        $machine = $reader.ReadUInt16()
        $stream.Position = $peOffset + 24
        $optionalHeader = $reader.ReadUInt16()
    }
    finally {
        $reader.Dispose()
        $stream.Dispose()
    }

    $isX86 = [string]$lane.platformTarget -ceq 'x86'
    $expectedMachine = if ($isX86) { 0x014c } else { 0x8664 }
    $expectedHeader = if ($isX86) { 0x010b } else { 0x020b }
    if ($signature -ne 0x00004550 -or $machine -ne $expectedMachine -or $optionalHeader -ne $expectedHeader) {
        throw ("{0} PE mismatch: signature=0x{1:x8}, machine=0x{2:x4}, optional=0x{3:x4}." -f
            $lane.id, $signature, $machine, $optionalHeader)
    }

    $binaryText = [Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($executable))
    $frameworkMoniker = ".NETFramework,Version=$($lane.targetFramework)"
    if (-not $binaryText.Contains($frameworkMoniker)) {
        throw "$($lane.id) executable does not contain TargetFrameworkAttribute '$frameworkMoniker'."
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

    Write-Host ("Verified {0}: machine=0x{1:x4}, framework={2}, settings={3}, runtime DLLs=0" -f
        $lane.id, $machine, $lane.targetFramework, $actualSettings.Count)
}
