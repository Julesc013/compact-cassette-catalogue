[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$ExpectedSourceCommit
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Get-PeMetadata {
    param([string]$Path)
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
        $directoryOffset = if ($optionalHeader -eq 0x010b) { $optionalHeaderOffset + 96 } elseif ($optionalHeader -eq 0x020b) { $optionalHeaderOffset + 112 } else { throw 'Unsupported setup PE header.' }
        $stream.Position = $directoryOffset + (14 * 8)
        $cliRva = $reader.ReadUInt32()
        $cliSize = $reader.ReadUInt32()
        If ($cliRva -eq 0 -or $cliSize -lt 20) { throw 'Setup executable has no valid CLR header.' }
        $sectionTable = $optionalHeaderOffset + $optionalHeaderSize
        $cliOffset = $null
        for ($index = 0; $index -lt $sectionCount; $index++) {
            $stream.Position = $sectionTable + ($index * 40) + 8
            $virtualSize = $reader.ReadUInt32()
            $virtualAddress = $reader.ReadUInt32()
            $rawSize = $reader.ReadUInt32()
            $rawOffset = $reader.ReadUInt32()
            $mapped = [Math]::Max([uint64]$virtualSize, [uint64]$rawSize)
            if ([uint64]$cliRva -ge [uint64]$virtualAddress -and [uint64]$cliRva -lt ([uint64]$virtualAddress + $mapped)) {
                $cliOffset = [uint64]$rawOffset + ([uint64]$cliRva - [uint64]$virtualAddress)
                break
            }
        }
        if ($null -eq $cliOffset) { throw 'Could not map setup CLR header.' }
        $stream.Position = [int64]$cliOffset + 16
        return [PSCustomObject]@{ signature = $signature; machine = $machine; optionalHeader = $optionalHeader; corFlags = $reader.ReadUInt32() }
    }
    finally {
        $reader.Dispose()
        $stream.Dispose()
    }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
if ([string]::IsNullOrWhiteSpace($ExpectedSourceCommit)) { $ExpectedSourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim() }
if ($ExpectedSourceCommit -notmatch '^[0-9a-f]{40}$') { throw 'Expected setup source commit is not a full Git SHA.' }
$projects = @(
    [PSCustomObject]@{ id = 'installer'; executable = 'Compact Cassette Catalogue Installer.exe' },
    [PSCustomObject]@{ id = 'uninstaller'; executable = 'Compact Cassette Catalogue Uninstaller.exe' }
)

foreach ($lane in @($manifest.lanes)) {
    $evidencePath = Join-Path $repositoryRoot "artifacts\evidence\setup-build\$($lane.id)\$Configuration\toolchain.json"
    if (-not (Test-Path -LiteralPath $evidencePath)) { throw "$($lane.id) setup build evidence is missing." }
    $evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
    if ([string]$evidence.lane -cne [string]$lane.id -or [string]$evidence.sourceCommit -cne $ExpectedSourceCommit -or
            [string]$evidence.releaseLabel -cne [string]$manifest.releaseLabel) {
        throw "$($lane.id) setup evidence identity does not match the selected source and release."
    }
    foreach ($project in $projects) {
        $projectId = [string]$project.id
        $projectExecutable = [string]$project.executable
        $outputDirectory = Join-Path $repositoryRoot "artifacts\setup\bin\$($lane.id)\$Configuration\$projectId"
        $executable = Join-Path $outputDirectory $projectExecutable
        $config = $executable + '.config'
        $record = @($evidence.outputs | Where-Object { [string]$_.id -ceq $projectId })
        if ($record.Count -ne 1 -or -not (Test-Path -LiteralPath $executable) -or -not (Test-Path -LiteralPath $config)) {
            throw "$($lane.id) $projectId output/evidence closure is incomplete: records=$($record.Count), exe=$([bool](Test-Path -LiteralPath $executable)), config=$([bool](Test-Path -LiteralPath $config)), path='$executable'."
        }
        if ((Get-FileHash -LiteralPath $executable -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$record[0].executableSha256 -or
                (Get-FileHash -LiteralPath $config -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$record[0].configSha256) {
            throw "$($lane.id) $projectId bytes no longer match their build evidence."
        }
        $pe = Get-PeMetadata -Path $executable
        $expectedMachine = [Convert]::ToUInt16(([string]$lane.peMachine).Substring(2), 16)
        $expectedHeader = [Convert]::ToUInt16(([string]$lane.peOptionalHeader).Substring(2), 16)
        $requires32 = ($pe.corFlags -band 2) -ne 0
        if ($pe.signature -ne 0x00004550 -or $pe.machine -ne $expectedMachine -or $pe.optionalHeader -ne $expectedHeader -or
                ($pe.corFlags -band 1) -eq 0 -or $requires32 -ne [bool]$lane.requires32Bit -or ($pe.corFlags -band 0x00020000) -ne 0) {
            throw "$($lane.id) $projectId PE/CLR architecture does not match the closed lane mapping."
        }
        $binaryText = [Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($executable))
        $framework = ".NETFramework,Version=$($lane.targetFramework)"
        if (-not $binaryText.Contains($framework)) { throw "$($lane.id) $projectId lacks TargetFrameworkAttribute '$framework'." }
        $version = [Diagnostics.FileVersionInfo]::GetVersionInfo($executable)
        if ([Reflection.AssemblyName]::GetAssemblyName($executable).Version.ToString() -cne [string]$manifest.assemblyVersion -or
                [string]$version.FileVersion -cne [string]$manifest.fileVersion -or
                [string]$version.ProductVersion -cne [string]$manifest.assemblyProductVersion) {
            throw "$($lane.id) $projectId $($manifest.releaseLabel) version metadata is incorrect."
        }
        [xml]$configDocument = Get-Content -LiteralPath $config -Raw
        if ([string]$configDocument.configuration.startup.supportedRuntime.version -cne 'v4.0' -or
                [string]$configDocument.configuration.startup.supportedRuntime.sku -cne $framework) {
            throw "$($lane.id) $projectId runtime config is not '$framework'."
        }
        $unexpected = @(Get-ChildItem -LiteralPath $outputDirectory -File | Where-Object { $_.FullName -notin @($executable, $config) })
        if ($unexpected.Count -ne 0) { throw "$($lane.id) $projectId contains unexpected outputs: $($unexpected.Name -join ', ')" }
        Write-Host ("Verified setup {0}/{1}: machine=0x{2:x4}, header=0x{3:x4}, CorFlags=0x{4:x8}, framework={5}." -f $lane.id,$projectId,$pe.machine,$pe.optionalHeader,$pe.corFlags,$lane.targetFramework)
    }
}

Write-Host "All six $($manifest.releaseLabel) setup executables passed closed PE/framework/config/evidence verification."
