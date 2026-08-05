[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lock = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'toolchain-lock.json') -Raw | ConvertFrom-Json
$expected = @(
    [PSCustomObject]@{ id = 'win-x86-net40'; platform = 'x86'; framework = 'v4.0'; toolset = '15'; toolsVersion = '15.0'; machine = '0x014c'; header = '0x010b'; package = 'C3-v1.3.0-win-x86-net40-portable.zip'; resourceTool = 'Microsoft SDKs/Windows/v10.0A/bin/NETFX 4.6.1 Tools/ResGen.exe' },
    [PSCustomObject]@{ id = 'win-x64-net48'; platform = 'x64'; framework = 'v4.8'; toolset = '17'; toolsVersion = 'Current'; machine = '0x8664'; header = '0x020b'; package = 'C3-v1.3.0-win-x64-net48-portable.zip'; resourceTool = 'Microsoft SDKs/Windows/v10.0A/bin/NETFX 4.8.1 Tools/ResGen.exe' },
    [PSCustomObject]@{ id = 'win-arm64-net481'; platform = 'ARM64'; framework = 'v4.8.1'; toolset = '18'; toolsVersion = 'Current'; machine = '0xaa64'; header = '0x020b'; package = 'C3-v1.3.0-win-arm64-net481-portable.zip'; resourceTool = 'Microsoft SDKs/Windows/v10.0A/bin/NETFX 4.8.1 Tools/ResGen.exe' }
)
$lanes = @($manifest.lanes)
if ($lanes.Count -ne $expected.Count) {
    throw "Release manifest contains $($lanes.Count) lanes; exactly three are required."
}
for ($index = 0; $index -lt $expected.Count; $index++) {
    $actual = $lanes[$index]
    $contract = $expected[$index]
    foreach ($comparison in @(
            @('id', [string]$actual.id, $contract.id),
            @('platform', [string]$actual.platform, $contract.platform),
            @('platformTarget', [string]$actual.platformTarget, $contract.platform),
            @('targetFramework', [string]$actual.targetFramework, $contract.framework),
            @('toolset', [string]$actual.toolset, $contract.toolset),
            @('effectiveToolsVersion', [string]$actual.effectiveToolsVersion, $contract.toolsVersion),
            @('resourceToolRelativePath', [string]$actual.resourceToolRelativePath, $contract.resourceTool),
            @('peMachine', [string]$actual.peMachine, $contract.machine),
            @('peOptionalHeader', [string]$actual.peOptionalHeader, $contract.header),
            @('packageName', [string]$actual.packageName, $contract.package),
            @('status', [string]$actual.status, 'required'))) {
        if ([string]$comparison[1] -cne [string]$comparison[2]) {
            throw "Lane $index $($comparison[0]) '$($comparison[1])' does not match '$($comparison[2])'."
        }
    }
    $appConfig = Join-Path $repositoryRoot ([string]$actual.appConfig)
    if (-not (Test-Path -LiteralPath $appConfig -PathType Leaf)) {
        throw "$($actual.id) AppConfig is missing: $appConfig"
    }
}

$runtimeLanes = @(& (Join-Path $PSScriptRoot 'get-runtime-lanes.ps1'))
if ($runtimeLanes.Count -ne $lanes.Count) {
    throw 'PowerShell 2 target projection does not contain exactly the manifest lanes.'
}
for ($index = 0; $index -lt $lanes.Count; $index++) {
    foreach ($propertyName in @('id', 'packageName', 'targetFramework', 'peMachine', 'runtimeEnvironmentId', 'runtimeArchitecture', 'runtimeClaim')) {
        if ([string]$runtimeLanes[$index].$propertyName -cne [string]$lanes[$index].$propertyName) {
            throw "Target projection $index property '$propertyName' does not match lanes.json."
        }
    }
}

$lockIds = @($lock.lanes | ForEach-Object { [string]$_.id })
if (($lockIds -join "`n") -cne (($expected.id) -join "`n")) {
    throw 'Toolchain lock lane IDs do not exactly match the release manifest.'
}
if ([string]$lock.status -notin @('template', 'locked')) {
    throw "Unknown toolchain lock status '$($lock.status)'."
}
if ([int]$lock.schemaVersion -ne 2) {
    throw "Toolchain lock schemaVersion '$($lock.schemaVersion)' is unsupported; expected 2."
}
if ([string]$lock.status -ceq 'locked') {
    if ([string]::IsNullOrWhiteSpace([string]$lock.sourceCommit) -or
            [string]::IsNullOrWhiteSpace([string]$lock.frozenAtUtc) -or
            [string]::IsNullOrWhiteSpace([string]$lock.expectedRemoteRef)) {
        throw 'Locked toolchain policy requires sourceCommit, expectedRemoteRef, and frozenAtUtc.'
    }
    foreach ($lockedLane in @($lock.lanes)) {
        foreach ($propertyName in @('visualStudioProductVersion', 'visualStudioInstallationVersion', 'msbuildSha256', 'vbcSha256', 'referenceAssemblySetSha256', 'resourceToolPath', 'resourceToolSha256')) {
            if ([string]::IsNullOrWhiteSpace([string]$lockedLane.$propertyName)) {
                throw "Locked toolchain lane '$($lockedLane.id)' is missing $propertyName."
            }
        }
        if (-not [IO.Path]::IsPathRooted([string]$lockedLane.resourceToolPath) -or
                [string]$lockedLane.resourceToolSha256 -notmatch '^[0-9a-f]{64}$') {
            throw "Locked toolchain lane '$($lockedLane.id)' has an invalid resource-tool path or SHA-256."
        }
    }
}

$projectPath = Join-Path $repositoryRoot ([string]$manifest.sourceProject)
[xml]$project = Get-Content -LiteralPath $projectPath -Raw
$namespace = New-Object Xml.XmlNamespaceManager($project.NameTable)
$namespace.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
foreach ($configuration in @('Debug', 'Release')) {
    $condition = " '`$(Configuration)|`$(Platform)' == '$configuration|ARM64' "
    $group = @($project.SelectNodes('/msb:Project/msb:PropertyGroup', $namespace) |
        Where-Object { $_.GetAttribute('Condition') -ceq $condition })
    if ($group.Count -ne 1) {
        throw "Project must contain exactly one $configuration|ARM64 property group."
    }
    if ([string]$group[0].PlatformTarget -cne 'ARM64' -or
            [string]$group[0].Prefer32Bit -cne 'false' -or
            [string]$group[0].TreatWarningsAsErrors -cne 'true') {
        throw "$configuration|ARM64 does not enforce ARM64, Prefer32Bit=false, and TreatWarningsAsErrors=true."
    }
    if ($configuration -ceq 'Release' -and [string]$group[0].Optimize -cne 'true') {
        throw 'Release|ARM64 must enable optimization.'
    }
}

$solution = Get-Content -LiteralPath (Join-Path $repositoryRoot 'Compact Cassette Catalogue.sln') -Raw
foreach ($requiredLine in @(
        'Debug|ARM64 = Debug|ARM64',
        'Release|ARM64 = Release|ARM64',
        '{DD0ADBE8-82D1-4620-8073-FF8EC6392135}.Debug|ARM64.Build.0 = Debug|ARM64',
        '{DD0ADBE8-82D1-4620-8073-FF8EC6392135}.Release|ARM64.Build.0 = Release|ARM64')) {
    if (-not $solution.Contains($requiredLine)) {
        throw "Solution is missing ARM64 mapping: $requiredLine"
    }
}
foreach ($historicalProjectGuid in @(
        '{D48984D9-8E3B-413D-A8DA-7DF5B4B3C09B}',
        '{12C86E82-C311-466C-97B1-C1E1ACC75A9F}')) {
    if ($solution -match [regex]::Escape($historicalProjectGuid) + '\.(Debug|Release)\|ARM64\.Build\.0') {
        throw "Historical installer project $historicalProjectGuid must not enter the ARM64 build graph."
    }
}

$conditionalSource = @(Get-ChildItem -LiteralPath (Join-Path $repositoryRoot 'Compact Cassette Catalogue') -Filter '*.vb' -File -Recurse |
    Select-String -Pattern '^\s*#If\s+.*(?:NET(?:40|48|481)|ARM64|X64|X86)' -CaseSensitive:$false)
if ($conditionalSource.Count -gt 0) {
    throw "Architecture/framework-conditional application source is prohibited: $($conditionalSource.Path -join ', ')"
}

foreach ($targetScript in @('smoke-launch.ps1', 'verify-target-runtime.ps1', 'target-environment.ps1', 'test-target-environment.ps1')) {
    $targetScriptContent = Get-Content -LiteralPath (Join-Path $PSScriptRoot $targetScript) -Raw
    if ($targetScriptContent.Contains('$PSScriptRoot')) {
        throw "$targetScript is target-side PowerShell 2 tooling and must not use `$PSScriptRoot."
    }
}
$targetVerifier = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'verify-target-runtime.ps1') -Raw
foreach ($requiredTargetControl in @(
        'Caller-supplied -TargetEnvironmentId is prohibited',
        'Get-C3TargetEnvironmentFacts',
        'Assert-C3TargetEnvironment')) {
    if (-not $targetVerifier.Contains($requiredTargetControl)) {
        throw "Target verifier is missing mechanical environment control '$requiredTargetControl'."
    }
}

Write-Host 'Three-lane contract verified: exact manifest, lock shape, ARM64 configurations, installer exclusion, and source identity policy pass.'
