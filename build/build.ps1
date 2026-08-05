[CmdletBinding()]
param(
    [string]$Lane,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal',
    [ValidateSet('Preparation', 'Candidate')]
    [string]$ToolchainMode = 'Preparation',
    [string]$MSBuildPath,
    [switch]$AllowCompatibleFallback,
    [switch]$Rebuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Get-StringSha256 {
    param([Parameter(Mandatory = $true)][string]$Text)

    $algorithm = [Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [Text.Encoding]::UTF8.GetBytes($Text)
        return ([BitConverter]::ToString($algorithm.ComputeHash($bytes))).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $algorithm.Dispose()
    }
}

function Get-ReferenceAssemblyEvidence {
    param([Parameter(Mandatory = $true)][string]$Path)

    $root = [IO.Path]::GetFullPath($Path).TrimEnd('\')
    $files = @(Get-ChildItem -LiteralPath $root -File -Recurse | Sort-Object FullName | ForEach-Object {
        $relativePath = $_.FullName.Substring($root.Length + 1).Replace('\', '/')
        [PSCustomObject]@{
            path = $relativePath
            sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        }
    })
    $setText = (($files | ForEach-Object { "$($_.path)|$($_.sha256)" }) -join "`n") + "`n"
    return [PSCustomObject]@{
        path = $root
        fileCount = $files.Count
        setSha256 = Get-StringSha256 -Text $setText
        files = $files
    }
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lock = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'toolchain-lock.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if (-not [string]::IsNullOrWhiteSpace($Lane)) {
    $lanes = @($lanes | Where-Object { $_.id -ceq $Lane })
    if ($lanes.Count -eq 0) {
        $available = @($manifest.lanes | ForEach-Object { $_.id }) -join ', '
        throw "Unknown build lane '$Lane'. Available lanes: $available"
    }
}
if ($AllowCompatibleFallback) {
    throw 'Compatible MSBuild fallback is not permitted by the C3 1.3 three-lane release contract.'
}
if ($ToolchainMode -ceq 'Candidate' -and [string]$lock.status -cne 'locked') {
    throw 'Candidate builds require build/toolchain-lock.json status "locked" after candidate freeze.'
}

$projectPath = Join-Path $repositoryRoot ([string]$manifest.sourceProject)
if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
    throw "The original C3 project was not found: $projectPath"
}
$evidenceTargets = Join-Path $PSScriptRoot 'C3.BuildEvidence.targets'
$referenceRoot = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies\Microsoft\Framework\.NETFramework'
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) {
    throw 'Could not resolve the source commit for build evidence.'
}
$sourceStatus = @(& git -C $repositoryRoot status --short)

$target = if ($Rebuild) { 'Rebuild' } else { 'Build' }
foreach ($buildLane in $lanes) {
    $lockLane = @($lock.lanes | Where-Object { $_.id -ceq $buildLane.id })
    if ($lockLane.Count -ne 1) {
        throw "Toolchain lock does not contain exactly one entry for '$($buildLane.id)'."
    }

    $resolveArguments = @{
        Toolset = [string]$buildLane.toolset
        Detailed = $true
    }
    if (-not [string]::IsNullOrWhiteSpace($MSBuildPath)) {
        $resolveArguments.MSBuildPath = $MSBuildPath
    }
    if ($ToolchainMode -ceq 'Candidate') {
        $resolveArguments.ExpectedProductVersion = [string]$lockLane[0].visualStudioProductVersion
        $resolveArguments.ExpectedInstallationVersion = [string]$lockLane[0].visualStudioInstallationVersion
    }
    $resolvedToolchain = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1') @resolveArguments
    $msbuild = [string]$resolvedToolchain.msbuildPath

    $installedProductVersion = [string]$resolvedToolchain.visualStudioProductVersion
    $installedVersionMatch = [regex]::Match($installedProductVersion, '^\d+(?:\.\d+)+')
    if ($ToolchainMode -ceq 'Preparation' -and
            (-not $installedVersionMatch.Success -or $installedVersionMatch.Value -cne [string]$buildLane.initialServicingPin)) {
        Write-Warning "$($buildLane.id) resolved Visual Studio '$installedProductVersion'; the decision-date starting pin is '$($buildLane.initialServicingPin)'. This build is preparation evidence only."
    }

    $installationPath = [string]$resolvedToolchain.visualStudioInstallationPath
    if ([string]::IsNullOrWhiteSpace($installationPath)) {
        throw "MSBuild '$msbuild' could not be associated with a Visual Studio installation."
    }
    $compilerPath = Join-Path $installationPath (([string]$buildLane.compilerRelativePath).Replace('/', '\'))
    if (-not (Test-Path -LiteralPath $compilerPath -PathType Leaf)) {
        throw "Required VBC compiler was not found for '$($buildLane.id)': $compilerPath"
    }
    $referencePath = Join-Path $referenceRoot ([string]$buildLane.targetFramework)
    if (-not (Test-Path -LiteralPath $referencePath -PathType Container)) {
        throw "Required reference assemblies were not found for '$($buildLane.id)': $referencePath"
    }

    $outputPath = Join-Path $repositoryRoot "artifacts\bin\$($buildLane.id)\$Configuration"
    $intermediatePath = Join-Path $repositoryRoot "artifacts\obj\$($buildLane.id)\$Configuration"
    $evidencePath = Join-Path $repositoryRoot "artifacts\evidence\build\$($buildLane.id)\$Configuration"
    $appConfigPath = Join-Path $repositoryRoot ([string]$buildLane.appConfig)
    if (-not (Test-Path -LiteralPath $appConfigPath -PathType Leaf)) {
        throw "Build lane '$($buildLane.id)' references missing AppConfig '$appConfigPath'."
    }
    foreach ($directory in @($outputPath, $intermediatePath, $evidencePath)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }
    $binaryLogPath = Join-Path $evidencePath 'msbuild.binlog'
    $propertiesPath = Join-Path $evidencePath 'msbuild-properties.txt'

    Write-Host "Building $($buildLane.id) from the original project with $msbuild"
    & $msbuild `
        $projectPath `
        "/t:$target" `
        "/ToolsVersion:$($buildLane.effectiveToolsVersion)" `
        "/p:Configuration=$Configuration" `
        "/p:Platform=$($buildLane.platform)" `
        "/p:PlatformTarget=$($buildLane.platformTarget)" `
        "/p:TargetFrameworkVersion=$($buildLane.targetFramework)" `
        "/p:TargetFrameworkRootPath=$referenceRoot\" `
        "/p:FrameworkPathOverride=$referencePath" `
        "/p:AppConfig=$appConfigPath" `
        "/p:OutputPath=$outputPath\" `
        "/p:IntermediateOutputPath=$intermediatePath\" `
        "/p:VbcToolPath=$([IO.Path]::GetDirectoryName($compilerPath))" `
        '/p:VbcToolExe=vbc.exe' `
        '/p:UseSharedCompilation=false' `
        "/p:CustomAfterMicrosoftCommonTargets=$evidenceTargets" `
        "/p:C3ExpectedMSBuildToolsVersion=$($buildLane.effectiveToolsVersion)" `
        "/p:C3ExpectedVbcPath=$compilerPath" `
        "/p:C3ExpectedFrameworkPath=$referencePath" `
        "/p:C3BuildEvidencePropertiesPath=$propertiesPath" `
        "/binaryLogger:$binaryLogPath" `
        "/v:$Verbosity" `
        '/nologo'

    if ($LASTEXITCODE -ne 0) {
        throw "Build lane '$($buildLane.id)' failed with exit code $LASTEXITCODE."
    }
    if (-not (Test-Path -LiteralPath $binaryLogPath -PathType Leaf) -or
            -not (Test-Path -LiteralPath $propertiesPath -PathType Leaf)) {
        throw "Build lane '$($buildLane.id)' did not produce the required binary log and property evidence."
    }

    $referenceEvidence = Get-ReferenceAssemblyEvidence -Path $referencePath
    $msbuildHash = (Get-FileHash -LiteralPath $msbuild -Algorithm SHA256).Hash.ToLowerInvariant()
    $compilerHash = (Get-FileHash -LiteralPath $compilerPath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($ToolchainMode -ceq 'Candidate') {
        foreach ($comparison in @(
                @('MSBuild SHA-256', $msbuildHash, [string]$lockLane[0].msbuildSha256),
                @('VBC SHA-256', $compilerHash, [string]$lockLane[0].vbcSha256),
                @('reference set SHA-256', $referenceEvidence.setSha256, [string]$lockLane[0].referenceAssemblySetSha256))) {
            if ([string]$comparison[1] -cne [string]$comparison[2]) {
                throw "$($buildLane.id) $($comparison[0]) '$($comparison[1])' does not match locked value '$($comparison[2])'."
            }
        }
        if ([string]$lock.sourceCommit -cne $sourceCommit) {
            throw "Candidate source '$sourceCommit' does not match locked source '$($lock.sourceCommit)'."
        }
    }

    $compilerInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($compilerPath)
    $evidence = [ordered]@{
        schemaVersion = 1
        lane = [string]$buildLane.id
        configuration = $Configuration
        toolchainMode = $ToolchainMode
        initialServicingPin = [string]$buildLane.initialServicingPin
        visualStudio = [ordered]@{
            displayName = [string]$resolvedToolchain.visualStudioDisplayName
            productVersion = $installedProductVersion
            installationVersion = [string]$resolvedToolchain.visualStudioInstallationVersion
            installationPath = $installationPath
        }
        msbuild = [ordered]@{
            path = $msbuild
            fileVersion = [string]$resolvedToolchain.msbuildFileVersion
            productVersion = [string]$resolvedToolchain.msbuildProductVersion
            sha256 = $msbuildHash
            effectiveToolsVersion = [string]$buildLane.effectiveToolsVersion
            binaryLog = $binaryLogPath
            properties = $propertiesPath
        }
        compiler = [ordered]@{
            path = $compilerPath
            fileVersion = [string]$compilerInfo.FileVersion
            productVersion = [string]$compilerInfo.ProductVersion
            sha256 = $compilerHash
            sharedCompilation = $false
        }
        referenceAssemblies = $referenceEvidence
        resourceTools = [ordered]@{
            propertiesEvidence = $propertiesPath
        }
        buildHost = [ordered]@{
            machineName = [Environment]::MachineName
            osVersion = [Environment]::OSVersion.VersionString
            osArchitecture = [string][Runtime.InteropServices.RuntimeInformation]::OSArchitecture
            processArchitecture = [string][Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture
            powershellVersion = [string]$PSVersionTable.PSVersion
        }
        source = [ordered]@{
            commit = $sourceCommit
            worktreeStatus = @($sourceStatus)
        }
        generatedAtUtc = [DateTime]::UtcNow.ToString('o')
    }
    $evidenceJsonPath = Join-Path $evidencePath 'toolchain.json'
    $evidence | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $evidenceJsonPath -Encoding UTF8
    Write-Host "Recorded toolchain evidence: $evidenceJsonPath"
}

Write-Host "Built $($lanes.Count) source-identical C3 lane(s) in $ToolchainMode mode."
