[CmdletBinding()]
param(
    [string]$Lane,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal',
    [ValidateSet('Preparation', 'Candidate')]
    [string]$ToolchainMode = 'Preparation',
    [string]$ToolchainLockPath,
    [string]$MSBuildPath,
    [switch]$AllowCompatibleFallback,
    [switch]$Rebuild,
    [switch]$PreflightOnly
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
. (Join-Path $PSScriptRoot 'servicing-version.ps1')
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$repositoryLockTemplate = Join-Path $PSScriptRoot 'toolchain-lock.json'
if ($ToolchainMode -ceq 'Candidate') {
    if ([string]::IsNullOrWhiteSpace($ToolchainLockPath)) {
        throw 'Candidate builds require -ToolchainLockPath to an external source-bound lock.'
    }
    if (-not [IO.Path]::IsPathRooted($ToolchainLockPath)) {
        throw 'Candidate -ToolchainLockPath must be absolute.'
    }
    $resolvedLockPath = [IO.Path]::GetFullPath($ToolchainLockPath)
    $repositoryPrefix = [IO.Path]::GetFullPath($repositoryRoot).TrimEnd('\') + '\'
    if ($resolvedLockPath.StartsWith($repositoryPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Candidate toolchain lock must be external to the clean frozen source checkout; a tracked self-referential lock is prohibited.'
    }
}
else {
    if (-not [string]::IsNullOrWhiteSpace($ToolchainLockPath)) {
        throw '-ToolchainLockPath is reserved for Candidate mode.'
    }
    $resolvedLockPath = $repositoryLockTemplate
}
if (-not (Test-Path -LiteralPath $resolvedLockPath -PathType Leaf)) {
    throw "Toolchain lock does not exist: $resolvedLockPath"
}
$toolchainLockSha256 = (Get-FileHash -LiteralPath $resolvedLockPath -Algorithm SHA256).Hash.ToLowerInvariant()
$lock = Get-Content -LiteralPath $resolvedLockPath -Raw | ConvertFrom-Json
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
    throw 'Candidate builds require an external toolchain lock with status "locked".'
}
if ($ToolchainMode -ceq 'Candidate') {
    if ([int]$lock.schemaVersion -ne 3) {
        throw "Candidate builds require external toolchain lock schemaVersion 3; found '$($lock.schemaVersion)'."
    }
    if ([string]::IsNullOrWhiteSpace([string]$lock.providerRefReceipt.remoteName) -or
            [string]::IsNullOrWhiteSpace([string]$lock.providerRefReceipt.remoteUrl) -or
            [string]::IsNullOrWhiteSpace([string]$lock.providerRefReceipt.providerRef) -or
            [string]$lock.providerRefReceipt.remoteTrackingRef -cne [string]$lock.expectedRemoteRef -or
            [string]$lock.providerRefReceipt.fetchedCommit -cne [string]$lock.sourceCommit -or
            [string]::IsNullOrWhiteSpace([string]$lock.providerRefReceipt.fetchedAtUtc)) {
        throw 'Candidate builds require a provider-ref receipt bound to the lock source and expected remote snapshot.'
    }
}
if ($PreflightOnly -and $ToolchainMode -cne 'Candidate') {
    throw '-PreflightOnly is available only in Candidate mode.'
}
if ($ToolchainMode -ceq 'Candidate' -and -not $PreflightOnly -and -not $Rebuild) {
    throw 'Candidate builds require -Rebuild so every byte-producing resource step is executed from clean intermediates.'
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
$sourceStatus = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
if ($LASTEXITCODE -ne 0) {
    throw 'Could not inspect candidate source worktree status.'
}
if ($ToolchainMode -ceq 'Candidate' -and [string]$lock.sourceCommit -cne $sourceCommit) {
    throw "External candidate lock source '$($lock.sourceCommit)' does not match frozen source HEAD '$sourceCommit'."
}
if ($ToolchainMode -ceq 'Candidate') {
    if ($sourceStatus.Count -ne 0) {
        throw "Candidate source must be clean before compilation; tracked, staged, or untracked changes were found:`n$($sourceStatus -join "`n")"
    }

    $submoduleStatus = @(& git -C $repositoryRoot submodule status --recursive)
    if ($LASTEXITCODE -ne 0) {
        throw 'Could not inspect candidate submodule status.'
    }
    $invalidSubmodules = @($submoduleStatus | Where-Object { $_ -match '^[\-+U]' })
    if ($invalidSubmodules.Count -ne 0) {
        throw "Candidate source contains uninitialized, mismatched, or conflicted submodules:`n$($invalidSubmodules -join "`n")"
    }

    $expectedRemoteRef = [string]$lock.expectedRemoteRef
    if ([string]::IsNullOrWhiteSpace($expectedRemoteRef) -or
            -not $expectedRemoteRef.StartsWith('refs/remotes/', [StringComparison]::Ordinal)) {
        throw 'External candidate lock must name an expected remote-tracking ref under refs/remotes/.'
    }
    & git -C $repositoryRoot show-ref --verify --quiet $expectedRemoteRef
    if ($LASTEXITCODE -ne 0) {
        throw "Expected candidate remote-tracking ref does not exist locally: $expectedRemoteRef"
    }
    $remoteCommit = (& git -C $repositoryRoot rev-parse $expectedRemoteRef).Trim()
    if ($LASTEXITCODE -ne 0 -or $remoteCommit -cne $sourceCommit) {
        throw "Frozen source '$sourceCommit' is not exactly the fetched expected remote ref '$expectedRemoteRef' ('$remoteCommit')."
    }

    & (Join-Path $PSScriptRoot 'validate-baseline-genome.ps1')
    & (Join-Path $PSScriptRoot 'validate-lanes.ps1')
}
foreach ($buildLane in $lanes) {
    $candidateLockLane = @($lock.lanes | Where-Object { $_.id -ceq $buildLane.id })
    if ($candidateLockLane.Count -ne 1) {
        throw "Toolchain lock does not contain exactly one entry for '$($buildLane.id)'."
    }
    if ($ToolchainMode -ceq 'Candidate') {
        foreach ($requiredLockProperty in @(
                'visualStudioProductVersion',
                'visualStudioInstallationVersion',
                'msbuildSha256',
                'vbcSha256',
                'referenceAssemblySetSha256',
                'resourceToolPath',
                'resourceToolSha256')) {
            if ([string]::IsNullOrWhiteSpace([string]$candidateLockLane[0].$requiredLockProperty)) {
                throw "External candidate lock lane '$($buildLane.id)' is missing '$requiredLockProperty'."
            }
        }
        foreach ($hashProperty in @('msbuildSha256', 'vbcSha256', 'referenceAssemblySetSha256', 'resourceToolSha256')) {
            if ([string]$candidateLockLane[0].$hashProperty -notmatch '^[0-9a-f]{64}$') {
                throw "External candidate lock lane '$($buildLane.id)' has invalid '$hashProperty'."
            }
        }
        if (-not [IO.Path]::IsPathRooted([string]$candidateLockLane[0].resourceToolPath)) {
            throw "External candidate lock lane '$($buildLane.id)' resourceToolPath must be absolute."
        }
        [void](Assert-C3VisualStudioServicingFloor `
                -ProductVersion ([string]$candidateLockLane[0].visualStudioProductVersion) `
                -MinimumVersion ([string]$buildLane.initialServicingPin) `
                -Context "External candidate lock lane '$($buildLane.id)'")
    }
}
if ($PreflightOnly) {
    Write-Host "Candidate preflight passed for source $sourceCommit with external lock SHA-256 $toolchainLockSha256."
    return
}

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
    $installedVersion = Get-C3VisualStudioServicingVersion `
        -ProductVersion $installedProductVersion `
        -Context "Resolved lane '$($buildLane.id)'"
    if ($ToolchainMode -ceq 'Preparation' -and
            $installedVersion -ne [version]([string]$buildLane.initialServicingPin)) {
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
    $resourceToolPath = if ($ToolchainMode -ceq 'Candidate') {
        [IO.Path]::GetFullPath([string]$lockLane[0].resourceToolPath)
    }
    else {
        if ([string]::IsNullOrWhiteSpace(${env:ProgramFiles(x86)})) {
            throw 'ProgramFiles(x86) is unavailable; the preparation resource-tool path cannot be resolved.'
        }
        [IO.Path]::GetFullPath((Join-Path ${env:ProgramFiles(x86)} (([string]$buildLane.resourceToolRelativePath).Replace('/', '\'))))
    }
    if (-not (Test-Path -LiteralPath $resourceToolPath -PathType Leaf)) {
        throw "Required ResGen resource tool was not found for '$($buildLane.id)': $resourceToolPath"
    }

    # Bind every currently known byte-producing input before the first compile.
    $referenceEvidence = Get-ReferenceAssemblyEvidence -Path $referencePath
    $msbuildHash = (Get-FileHash -LiteralPath $msbuild -Algorithm SHA256).Hash.ToLowerInvariant()
    $compilerHash = (Get-FileHash -LiteralPath $compilerPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $resourceToolHash = (Get-FileHash -LiteralPath $resourceToolPath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($ToolchainMode -ceq 'Candidate') {
        foreach ($comparison in @(
                @('MSBuild SHA-256', $msbuildHash, [string]$lockLane[0].msbuildSha256),
                @('VBC SHA-256', $compilerHash, [string]$lockLane[0].vbcSha256),
                @('reference set SHA-256', $referenceEvidence.setSha256, [string]$lockLane[0].referenceAssemblySetSha256),
                @('ResGen SHA-256', $resourceToolHash, [string]$lockLane[0].resourceToolSha256))) {
            if ([string]$comparison[1] -cne [string]$comparison[2]) {
                throw "$($buildLane.id) $($comparison[0]) '$($comparison[1])' does not match locked value '$($comparison[2])'."
            }
        }
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

    # Windows PowerShell 5.1 quotes native arguments containing spaces. A
    # single trailing backslash then escapes the generated closing quote in
    # MSBuild's command-line parser. Supply two so MSBuild receives the
    # required single directory separator without splitting Program Files.
    Write-Host "Building $($buildLane.id) from the original project with $msbuild"
    & $msbuild `
        $projectPath `
        "/t:$target" `
        "/ToolsVersion:$($buildLane.effectiveToolsVersion)" `
        "/p:Configuration=$Configuration" `
        "/p:Platform=$($buildLane.platform)" `
        "/p:PlatformTarget=$($buildLane.platformTarget)" `
        "/p:TargetFrameworkVersion=$($buildLane.targetFramework)" `
        "/p:TargetFrameworkRootPath=$referenceRoot\\" `
        "/p:FrameworkPathOverride=$referencePath" `
        "/p:AppConfig=$appConfigPath" `
        "/p:OutputPath=$outputPath\\" `
        "/p:IntermediateOutputPath=$intermediatePath\\" `
        "/p:VbcToolPath=$([IO.Path]::GetDirectoryName($compilerPath))" `
        '/p:VbcToolExe=vbc.exe' `
        "/p:ResGenToolPath=$([IO.Path]::GetDirectoryName($resourceToolPath))" `
        "/p:ResGenToolExe=$([IO.Path]::GetFileName($resourceToolPath))" `
        '/p:UseSharedCompilation=false' `
        "/p:CustomAfterMicrosoftCommonTargets=$evidenceTargets" `
        "/p:C3ExpectedMSBuildToolsVersion=$($buildLane.effectiveToolsVersion)" `
        "/p:C3ExpectedVbcPath=$compilerPath" `
        "/p:C3ExpectedFrameworkPath=$referencePath" `
        "/p:C3ExpectedResGenPath=$resourceToolPath" `
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

    $propertyEvidence = @{}
    foreach ($propertyLine in @(Get-Content -LiteralPath $propertiesPath)) {
        if ($propertyLine -notmatch '^(?<name>[^=]+)=(?<value>.*)$') {
            throw "$($buildLane.id) contains malformed MSBuild property evidence '$propertyLine'."
        }
        $propertyEvidence[$matches['name']] = $matches['value']
    }
    if ([string]$propertyEvidence['C3ExpectedResGenPath'] -cne $resourceToolPath -or
            [string]$propertyEvidence['C3ActualResGenPath'] -cne $resourceToolPath -or
            [string]$propertyEvidence['C3ResourceGenerationCompleted'] -cne 'true') {
        throw "$($buildLane.id) did not prove the forced ResGen path and CoreResGen completion in MSBuild evidence."
    }

    $finalReferenceEvidence = Get-ReferenceAssemblyEvidence -Path $referencePath
    foreach ($stabilityComparison in @(
            @('MSBuild', $msbuildHash, (Get-FileHash -LiteralPath $msbuild -Algorithm SHA256).Hash.ToLowerInvariant()),
            @('VBC', $compilerHash, (Get-FileHash -LiteralPath $compilerPath -Algorithm SHA256).Hash.ToLowerInvariant()),
            @('reference set', $referenceEvidence.setSha256, $finalReferenceEvidence.setSha256),
            @('ResGen', $resourceToolHash, (Get-FileHash -LiteralPath $resourceToolPath -Algorithm SHA256).Hash.ToLowerInvariant()))) {
        if ([string]$stabilityComparison[1] -cne [string]$stabilityComparison[2]) {
            throw "$($buildLane.id) $($stabilityComparison[0]) changed during the build: '$($stabilityComparison[1])' -> '$($stabilityComparison[2])'."
        }
    }
    $referenceEvidence = $finalReferenceEvidence

    $compilerInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($compilerPath)
    $resourceToolInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($resourceToolPath)
    $evidence = [ordered]@{
        schemaVersion = 1
        lane = [string]$buildLane.id
        configuration = $Configuration
        toolchainMode = $ToolchainMode
        initialServicingPin = [string]$buildLane.initialServicingPin
        toolchainLock = [ordered]@{
            path = $resolvedLockPath
            sha256 = $toolchainLockSha256
            status = [string]$lock.status
            sourceCommit = [string]$lock.sourceCommit
            providerRefReceipt = $lock.providerRefReceipt
        }
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
        resourceTools = @(
            [ordered]@{
                name = [IO.Path]::GetFileName($resourceToolPath)
                path = $resourceToolPath
                fileVersion = [string]$resourceToolInfo.FileVersion
                productVersion = [string]$resourceToolInfo.ProductVersion
                sha256 = $resourceToolHash
                forcedByBuild = $true
                coreResGenCompleted = $true
                propertiesEvidence = $propertiesPath
                binaryLog = $binaryLogPath
            }
        )
        buildHost = [ordered]@{
            machineName = [Environment]::MachineName
            osVersion = [Environment]::OSVersion.VersionString
            osArchitecture = [string][Runtime.InteropServices.RuntimeInformation]::OSArchitecture
            processArchitecture = [string][Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture
            powershellVersion = [string]$PSVersionTable.PSVersion
        }
        source = [ordered]@{
            commit = $sourceCommit
            expectedRemoteRef = [string]$lock.expectedRemoteRef
            worktreeStatus = @($sourceStatus)
        }
        generatedAtUtc = [DateTime]::UtcNow.ToString('o')
    }
    $evidenceJsonPath = Join-Path $evidencePath 'toolchain.json'
    $evidence | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $evidenceJsonPath -Encoding UTF8
    Write-Host "Recorded toolchain evidence: $evidenceJsonPath"
}

if ($ToolchainMode -ceq 'Candidate') {
    $finalSourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    if ($LASTEXITCODE -ne 0 -or $finalSourceCommit -cne $sourceCommit) {
        throw "Candidate source HEAD changed during the build: '$sourceCommit' -> '$finalSourceCommit'."
    }
    $finalSourceStatus = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
    if ($LASTEXITCODE -ne 0 -or $finalSourceStatus.Count -ne 0) {
        throw "Candidate source became dirty during the build:`n$($finalSourceStatus -join "`n")"
    }
    $finalSubmoduleStatus = @(& git -C $repositoryRoot submodule status --recursive)
    if ($LASTEXITCODE -ne 0) {
        throw 'Could not repeat candidate submodule closure after the build.'
    }
    $finalInvalidSubmodules = @($finalSubmoduleStatus | Where-Object { $_ -match '^[\-+U]' })
    if ($finalInvalidSubmodules.Count -ne 0) {
        throw "Candidate submodules changed during the build:`n$($finalInvalidSubmodules -join "`n")"
    }
    $finalRemoteCommit = (& git -C $repositoryRoot rev-parse ([string]$lock.expectedRemoteRef)).Trim()
    if ($LASTEXITCODE -ne 0 -or $finalRemoteCommit -cne $sourceCommit -or
            $finalRemoteCommit -cne [string]$lock.providerRefReceipt.fetchedCommit) {
        throw "Candidate remote snapshot changed during the build: expected '$sourceCommit', found '$finalRemoteCommit'."
    }
    & (Join-Path $PSScriptRoot 'validate-baseline-genome.ps1')
    & (Join-Path $PSScriptRoot 'validate-lanes.ps1')
}

$finalToolchainLockSha256 = (Get-FileHash -LiteralPath $resolvedLockPath -Algorithm SHA256).Hash.ToLowerInvariant()
if ($finalToolchainLockSha256 -cne $toolchainLockSha256) {
    throw "Toolchain lock changed during the build: $toolchainLockSha256 -> $finalToolchainLockSha256."
}

if ($ToolchainMode -ceq 'Candidate') {
    $closurePath = Join-Path $repositoryRoot 'artifacts\evidence\build\candidate-source-closure.json'
    $closure = [ordered]@{
        schemaVersion = 1
        status = 'pass'
        sourceCommit = $sourceCommit
        worktreeClean = $true
        submodulesExact = $true
        expectedRemoteRef = [string]$lock.expectedRemoteRef
        remoteSnapshotCommit = [string]$lock.providerRefReceipt.fetchedCommit
        providerRefReceipt = $lock.providerRefReceipt
        toolchainLockSha256 = $toolchainLockSha256
        genome = 'pass'
        laneProjection = 'pass'
        completedAtUtc = [DateTime]::UtcNow.ToString('o')
    }
    $closure | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $closurePath -Encoding UTF8
    Write-Host "Recorded post-build candidate source closure: $closurePath"
}

Write-Host "Built $($lanes.Count) source-identical C3 lane(s) in $ToolchainMode mode."
