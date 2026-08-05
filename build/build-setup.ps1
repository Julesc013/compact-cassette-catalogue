[CmdletBinding()]
param(
    [string]$Lane,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('Preparation', 'Candidate')]
    [string]$ToolchainMode = 'Preparation',
    [string]$ToolchainLockPath,
    [ValidateSet('minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal'
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

function Get-ReferenceSetSha256 {
    param([string]$Path)
    $root = [IO.Path]::GetFullPath($Path).TrimEnd('\')
    $lines = @(Get-ChildItem -LiteralPath $root -File -Recurse | Sort-Object FullName | ForEach-Object {
            $relative = $_.FullName.Substring($root.Length + 1).Replace('\', '/')
            "$relative|$((Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant())"
        })
    return Get-StringSha256 -Text (($lines -join "`n") + "`n")
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lanes = @($manifest.lanes)
if (-not [string]::IsNullOrWhiteSpace($Lane)) {
    $lanes = @($lanes | Where-Object { $_.id -ceq $Lane })
    if ($lanes.Count -ne 1) { throw "Unknown setup lane '$Lane'." }
}

if ($ToolchainMode -ceq 'Candidate') {
    if ([string]::IsNullOrWhiteSpace($ToolchainLockPath) -or -not [IO.Path]::IsPathRooted($ToolchainLockPath)) {
        throw 'Candidate setup builds require an absolute external -ToolchainLockPath.'
    }
    $resolvedLockPath = [IO.Path]::GetFullPath($ToolchainLockPath)
    & (Join-Path $PSScriptRoot 'build.ps1') -ToolchainMode Candidate -ToolchainLockPath $resolvedLockPath -PreflightOnly
}
else {
    if (-not [string]::IsNullOrWhiteSpace($ToolchainLockPath)) { throw '-ToolchainLockPath is reserved for Candidate setup builds.' }
    $resolvedLockPath = Join-Path $PSScriptRoot 'toolchain-lock.json'
}
$initialLockHash = (Get-FileHash -LiteralPath $resolvedLockPath -Algorithm SHA256).Hash.ToLowerInvariant()
$lock = Get-Content -LiteralPath $resolvedLockPath -Raw | ConvertFrom-Json
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Could not resolve setup source HEAD.' }
$referenceRoot = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies\Microsoft\Framework\.NETFramework'
$evidenceTargets = Join-Path $PSScriptRoot 'C3.BuildEvidence.targets'
$projects = @(
    [PSCustomObject]@{ id = 'installer'; path = 'Compact Cassette Catalogue Installer\Compact Cassette Catalogue Installer.vbproj'; executable = 'Compact Cassette Catalogue Installer.exe' },
    [PSCustomObject]@{ id = 'uninstaller'; path = 'Compact Cassette Catalogue Uninstaller\Compact Cassette Catalogue Uninstaller.vbproj'; executable = 'Compact Cassette Catalogue Uninstaller.exe' }
)

foreach ($buildLane in $lanes) {
    $lockLane = @($lock.lanes | Where-Object { $_.id -ceq $buildLane.id })
    if ($lockLane.Count -ne 1) { throw "Toolchain lock does not contain exactly one '$($buildLane.id)' entry." }
    $resolve = @{ Toolset = [string]$buildLane.toolset; Detailed = $true }
    if ($ToolchainMode -ceq 'Candidate') {
        $resolve.ExpectedProductVersion = [string]$lockLane[0].visualStudioProductVersion
        $resolve.ExpectedInstallationVersion = [string]$lockLane[0].visualStudioInstallationVersion
    }
    $toolchain = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1') @resolve
    $msbuild = [string]$toolchain.msbuildPath
    $compiler = Join-Path ([string]$toolchain.visualStudioInstallationPath) (([string]$buildLane.compilerRelativePath).Replace('/', '\'))
    $referencePath = Join-Path $referenceRoot ([string]$buildLane.targetFramework)
    $resourceTool = if ($ToolchainMode -ceq 'Candidate') {
        [IO.Path]::GetFullPath([string]$lockLane[0].resourceToolPath)
    }
    else {
        [IO.Path]::GetFullPath((Join-Path ${env:ProgramFiles(x86)} (([string]$buildLane.resourceToolRelativePath).Replace('/', '\'))))
    }
    foreach ($requiredPath in @($msbuild, $compiler, $referencePath, $resourceTool)) {
        if (-not (Test-Path -LiteralPath $requiredPath)) { throw "Required setup build input is missing: $requiredPath" }
    }
    $msbuildHash = (Get-FileHash -LiteralPath $msbuild -Algorithm SHA256).Hash.ToLowerInvariant()
    $compilerHash = (Get-FileHash -LiteralPath $compiler -Algorithm SHA256).Hash.ToLowerInvariant()
    $resourceHash = (Get-FileHash -LiteralPath $resourceTool -Algorithm SHA256).Hash.ToLowerInvariant()
    $referenceHash = Get-ReferenceSetSha256 -Path $referencePath
    if ($ToolchainMode -ceq 'Candidate') {
        foreach ($comparison in @(
                @('MSBuild', $msbuildHash, [string]$lockLane[0].msbuildSha256),
                @('VBC', $compilerHash, [string]$lockLane[0].vbcSha256),
                @('ResGen', $resourceHash, [string]$lockLane[0].resourceToolSha256),
                @('reference set', $referenceHash, [string]$lockLane[0].referenceAssemblySetSha256))) {
            if ([string]$comparison[1] -cne [string]$comparison[2]) {
                throw "$($buildLane.id) setup $($comparison[0]) does not match the external lock."
            }
        }
    }

    $laneEvidenceRoot = Join-Path $repositoryRoot "artifacts\evidence\setup-build\$($buildLane.id)\$Configuration"
    New-Item -ItemType Directory -Path $laneEvidenceRoot -Force | Out-Null
    $outputs = New-Object Collections.Generic.List[Object]
    foreach ($project in $projects) {
        $projectPath = Join-Path $repositoryRoot ([string]$project.path)
        $outputPath = Join-Path $repositoryRoot "artifacts\setup\bin\$($buildLane.id)\$Configuration\$($project.id)"
        $intermediatePath = Join-Path $repositoryRoot "artifacts\setup\obj\$($buildLane.id)\$Configuration\$($project.id)"
        $projectEvidence = Join-Path $laneEvidenceRoot ([string]$project.id)
        New-Item -ItemType Directory -Path $outputPath,$intermediatePath,$projectEvidence -Force | Out-Null
        $binaryLog = Join-Path $projectEvidence 'msbuild.binlog'
        $properties = Join-Path $projectEvidence 'msbuild-properties.txt'
        $appConfig = Join-Path $repositoryRoot ([string]$buildLane.setupAppConfig)
        $pathMap = "$repositoryRoot=C:\c3\src"
        & $msbuild $projectPath '/t:Rebuild' "/ToolsVersion:$($buildLane.effectiveToolsVersion)" `
            "/p:Configuration=$Configuration" "/p:Platform=$($buildLane.platform)" `
            "/p:PlatformTarget=$($buildLane.platformTarget)" "/p:TargetFrameworkVersion=$($buildLane.targetFramework)" `
            "/p:TargetFrameworkRootPath=$referenceRoot\\" "/p:FrameworkPathOverride=$referencePath" `
            "/p:AppConfig=$appConfig" "/p:OutputPath=$outputPath\\" "/p:IntermediateOutputPath=$intermediatePath\\" `
            "/p:VbcToolPath=$([IO.Path]::GetDirectoryName($compiler))" '/p:VbcToolExe=vbc.exe' `
            "/p:ResGenToolPath=$([IO.Path]::GetDirectoryName($resourceTool))" "/p:ResGenToolExe=$([IO.Path]::GetFileName($resourceTool))" `
            '/p:Deterministic=true' '/p:DebugSymbols=false' '/p:DebugType=None' "/p:PathMap=$pathMap" `
            '/p:UseSharedCompilation=false' "/p:CustomAfterMicrosoftCommonTargets=$evidenceTargets" `
            "/p:C3ExpectedMSBuildToolsVersion=$($buildLane.effectiveToolsVersion)" "/p:C3ExpectedVbcPath=$compiler" `
            "/p:C3ExpectedFrameworkPath=$referencePath" "/p:C3ExpectedResGenPath=$resourceTool" `
            "/p:C3ExpectedPathMap=$pathMap" "/p:C3BuildEvidencePropertiesPath=$properties" `
            "/binaryLogger:$binaryLog" "/v:$Verbosity" '/nologo'
        if ($LASTEXITCODE -ne 0) { throw "$($buildLane.id) $($project.id) setup build failed." }
        $executable = Join-Path $outputPath ([string]$project.executable)
        $config = $executable + '.config'
        if (-not (Test-Path -LiteralPath $executable) -or -not (Test-Path -LiteralPath $config) -or
                -not (Test-Path -LiteralPath $binaryLog) -or -not (Test-Path -LiteralPath $properties)) {
            throw "$($buildLane.id) $($project.id) omitted a required output or evidence file."
        }
        $unexpected = @(Get-ChildItem -LiteralPath $outputPath -File | Where-Object { $_.FullName -notin @($executable, $config) })
        if ($unexpected.Count -ne 0) { throw "$($buildLane.id) $($project.id) emitted an unexpected runtime file: $($unexpected.Name -join ', ')" }
        $version = (Get-Item -LiteralPath $executable).VersionInfo
        if ([string]$version.FileVersion -cne [string]$manifest.fileVersion -or
                [string]$version.ProductVersion -cne [string]$manifest.assemblyProductVersion) {
            throw "$($buildLane.id) $($project.id) has incorrect $($manifest.releaseLabel) binary version metadata."
        }
        $outputs.Add([ordered]@{
                id = [string]$project.id
                executablePath = $executable
                executableLength = [long](Get-Item -LiteralPath $executable).Length
                executableSha256 = (Get-FileHash -LiteralPath $executable -Algorithm SHA256).Hash.ToLowerInvariant()
                configPath = $config
                configSha256 = (Get-FileHash -LiteralPath $config -Algorithm SHA256).Hash.ToLowerInvariant()
                binaryLogSha256 = (Get-FileHash -LiteralPath $binaryLog -Algorithm SHA256).Hash.ToLowerInvariant()
                propertiesSha256 = (Get-FileHash -LiteralPath $properties -Algorithm SHA256).Hash.ToLowerInvariant()
            })
    }
    $evidence = [ordered]@{
        schemaVersion = 1
        classification = $ToolchainMode
        lane = [string]$buildLane.id
        releaseLabel = [string]$manifest.releaseLabel
        sourceCommit = $sourceCommit
        toolchainLockSha256 = $initialLockHash
        visualStudioProductVersion = [string]$toolchain.visualStudioProductVersion
        visualStudioInstallationVersion = [string]$toolchain.visualStudioInstallationVersion
        msbuildSha256 = $msbuildHash
        vbcSha256 = $compilerHash
        resourceToolSha256 = $resourceHash
        referenceAssemblySetSha256 = $referenceHash
        outputs = $outputs.ToArray()
        generatedAtUtc = [DateTime]::UtcNow.ToString('o')
    }
    $evidence | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $laneEvidenceRoot 'toolchain.json') -Encoding UTF8
}

if ($ToolchainMode -ceq 'Candidate') {
    & (Join-Path $PSScriptRoot 'build.ps1') -ToolchainMode Candidate -ToolchainLockPath $resolvedLockPath -PreflightOnly
}
$finalLockHash = (Get-FileHash -LiteralPath $resolvedLockPath -Algorithm SHA256).Hash.ToLowerInvariant()
if ($finalLockHash -cne $initialLockHash) { throw 'The setup toolchain lock changed during the build.' }
Write-Host "Built installer and uninstaller for $($lanes.Count) setup lane(s) in $ToolchainMode mode."
