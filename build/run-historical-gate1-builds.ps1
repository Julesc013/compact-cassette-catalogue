[CmdletBinding()]
param(
    [string]$MSBuildPath = 'C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Get-StringSha256 {
    param([string]$Text)
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try {
        return ([BitConverter]::ToString($algorithm.ComputeHash([Text.Encoding]::UTF8.GetBytes($Text)))).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $algorithm.Dispose()
    }
}

function Get-ReferenceSet {
    param([string]$Root)
    $fullRoot = [IO.Path]::GetFullPath($Root).TrimEnd('\')
    $files = @(Get-ChildItem -LiteralPath $fullRoot -File -Recurse | Sort-Object FullName | ForEach-Object {
            $relative = $_.FullName.Substring($fullRoot.Length + 1).Replace('\', '/')
            [PSCustomObject]@{
                path = $relative
                length = [long]$_.Length
                sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
            }
        })
    $setText = (($files | ForEach-Object { "$($_.path)|$($_.length)|$($_.sha256)" }) -join "`n") + "`n"
    return [PSCustomObject]@{
        path = $fullRoot
        fileCount = $files.Count
        sha256 = Get-StringSha256 -Text $setText
    }
}

function Get-PeSummary {
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
        $dataDirectoryOffset = if ($optionalHeader -eq 0x010b) { $optionalHeaderOffset + 96 } elseif ($optionalHeader -eq 0x020b) { $optionalHeaderOffset + 112 } else { throw 'Unsupported PE optional header.' }
        $stream.Position = $dataDirectoryOffset + (14 * 8)
        $cliRva = $reader.ReadUInt32()
        [void]$reader.ReadUInt32()
        $sectionTableOffset = $optionalHeaderOffset + $optionalHeaderSize
        $cliOffset = $null
        for ($index = 0; $index -lt $sectionCount; $index++) {
            $stream.Position = $sectionTableOffset + ($index * 40) + 8
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
        if ($null -eq $cliOffset) { throw 'Could not map CLR header.' }
        $stream.Position = [int64]$cliOffset + 16
        $corFlags = $reader.ReadUInt32()
        return [PSCustomObject]@{
            signature = ('0x{0:x8}' -f $signature)
            machine = ('0x{0:x4}' -f $machine)
            optionalHeader = ('0x{0:x4}' -f $optionalHeader)
            corFlags = ('0x{0:x8}' -f $corFlags)
        }
    }
    finally {
        $reader.Dispose()
        $stream.Dispose()
    }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$sourceRef = 'v1.2.0b1'
$sourceCommit = '2413e9139a098f3321385f2f946e743012a447f5'
$resolvedSource = (& git -C $repositoryRoot rev-parse "$sourceRef^{commit}").Trim()
if ($LASTEXITCODE -ne 0 -or $resolvedSource -cne $sourceCommit) {
    throw "$sourceRef does not resolve to immutable Gate 1 source '$sourceCommit'."
}
if (-not (Test-Path -LiteralPath $MSBuildPath -PathType Leaf)) {
    throw "Historical MSBuild is not installed: $MSBuildPath"
}
$MSBuildPath = [IO.Path]::GetFullPath($MSBuildPath)
$msbuildInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($MSBuildPath)
if ([string]$msbuildInfo.FileVersion -cne '14.0.25420.1' -or [string]$msbuildInfo.ProductVersion -cne '14.0.25420.1') {
    throw "Historical MSBuild must be exactly 14.0.25420.1; found '$($msbuildInfo.FileVersion)' / '$($msbuildInfo.ProductVersion)'."
}
$vbcPath = Join-Path (Split-Path -Parent $MSBuildPath) 'Roslyn\vbc.exe'
if (-not (Test-Path -LiteralPath $vbcPath -PathType Leaf)) {
    throw "Historical Roslyn VB compiler is missing: $vbcPath"
}
$referenceRoot = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0'
if (-not (Test-Path -LiteralPath $referenceRoot -PathType Container)) {
    throw "Historical .NET Framework 4.0 reference assemblies are missing: $referenceRoot"
}

$gateRoot = Join-Path $repositoryRoot 'artifacts\historical-gate1'
$sourceRoot = Join-Path $gateRoot 'source-v1.2.0b1'
if (-not (Test-Path -LiteralPath $sourceRoot -PathType Container)) {
    New-Item -ItemType Directory -Path $gateRoot -Force | Out-Null
    & git -C $repositoryRoot worktree add --detach $sourceRoot $sourceCommit
    if ($LASTEXITCODE -ne 0) { throw 'Could not create the isolated historical source worktree.' }
}
$worktreeCommit = (& git -C $sourceRoot rev-parse HEAD).Trim()
$worktreeStatus = @(& git -C $sourceRoot status --short)
if ($worktreeCommit -cne $sourceCommit -or $worktreeStatus.Count -ne 0) {
    throw "Historical source worktree is not clean exact source '$sourceCommit'."
}

$projectPath = Join-Path $sourceRoot 'Compact Cassette Catalogue\Compact Cassette Catalogue.vbproj'
$officialRoot = Join-Path $repositoryRoot 'artifacts\baseline\official-v1.2.0b1'
$laneContracts = @(
    [PSCustomObject]@{ platform = 'x86'; machine = '0x014c'; optionalHeader = '0x010b'; requires32Bit = $true; official = 'C3-v1.2.0b1-win-x86.exe' },
    [PSCustomObject]@{ platform = 'x64'; machine = '0x8664'; optionalHeader = '0x020b'; requires32Bit = $false; official = 'C3-v1.2.0b1-win-x64.exe' }
)
$results = New-Object Collections.Generic.List[Object]
foreach ($lane in $laneContracts) {
    $outputDirectory = Join-Path $gateRoot "builds\$($lane.platform)\$Configuration"
    $intermediateDirectory = Join-Path $gateRoot "obj\$($lane.platform)\$Configuration"
    New-Item -ItemType Directory -Path $outputDirectory,$intermediateDirectory -Force | Out-Null
    $outputArgument = $outputDirectory.TrimEnd('\') + '\\'
    $intermediateArgument = $intermediateDirectory.TrimEnd('\') + '\\'
    & $MSBuildPath $projectPath '/nologo' '/m:1' '/t:Rebuild' '/v:minimal' `
        "/p:Configuration=$Configuration" "/p:Platform=$($lane.platform)" `
        '/p:TargetFrameworkVersion=v4.0' "/p:OutputPath=$outputArgument" `
        "/p:IntermediateOutputPath=$intermediateArgument" '/p:UseSharedCompilation=false'
    if ($LASTEXITCODE -ne 0) {
        throw "Historical $($lane.platform) build failed with exit code $LASTEXITCODE."
    }
    $executable = Join-Path $outputDirectory 'Compact Cassette Catalogue.exe'
    if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
        throw "Historical $($lane.platform) build did not produce its executable."
    }
    $pe = Get-PeSummary -Path $executable
    $has32BitRequired = (([Convert]::ToUInt32($pe.corFlags.Substring(2), 16) -band 0x2) -ne 0)
    if ($pe.signature -cne '0x00004550' -or $pe.machine -cne $lane.machine -or
            $pe.optionalHeader -cne $lane.optionalHeader -or $has32BitRequired -ne [bool]$lane.requires32Bit) {
        throw "Historical $($lane.platform) PE result does not match its release architecture contract."
    }
    $binaryText = [Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($executable))
    if (-not $binaryText.Contains('.NETFramework,Version=v4.0')) {
        throw "Historical $($lane.platform) build is not marked for .NET Framework 4.0."
    }
    $officialPath = Join-Path $officialRoot $lane.official
    $rebuiltHash = (Get-FileHash -LiteralPath $executable -Algorithm SHA256).Hash.ToLowerInvariant()
    $officialHash = (Get-FileHash -LiteralPath $officialPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $results.Add([ordered]@{
            platform = $lane.platform
            outputPath = $executable
            length = [long](Get-Item -LiteralPath $executable).Length
            sha256 = $rebuiltHash
            pe = $pe
            targetFramework = '.NETFramework,Version=v4.0'
            officialPath = $officialPath
            officialLength = [long](Get-Item -LiteralPath $officialPath).Length
            officialSha256 = $officialHash
            byteIdentical = ($rebuiltHash -ceq $officialHash)
        })
}

$evidence = [ordered]@{
    schemaVersion = 1
    classification = 'historical-compatibility-laboratory-only'
    capturedAtUtc = [DateTime]::UtcNow.ToString('o')
    sourceRef = $sourceRef
    sourceCommit = $sourceCommit
    configuration = $Configuration
    msbuild = [ordered]@{
        path = $MSBuildPath
        fileVersion = [string]$msbuildInfo.FileVersion
        productVersion = [string]$msbuildInfo.ProductVersion
        sha256 = (Get-FileHash -LiteralPath $MSBuildPath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    vbc = [ordered]@{
        path = $vbcPath
        fileVersion = [string](Get-Item -LiteralPath $vbcPath).VersionInfo.FileVersion
        sha256 = (Get-FileHash -LiteralPath $vbcPath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    references = Get-ReferenceSet -Root $referenceRoot
    builds = $results.ToArray()
    releaseAuthority = $false
}
$evidenceDirectory = Join-Path $gateRoot 'evidence'
New-Item -ItemType Directory -Path $evidenceDirectory -Force | Out-Null
$evidencePath = Join-Path $evidenceDirectory 'historical-builds.json'
$evidence | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $evidencePath -Encoding UTF8

Write-Host "Historical Gate 1 builds passed structural verification: $evidencePath"
foreach ($result in $results) {
    Write-Host "$($result.platform): $($result.sha256); byte-identical to official=$($result.byteIdentical)"
}
