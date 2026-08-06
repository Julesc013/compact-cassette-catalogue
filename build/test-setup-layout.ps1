[CmdletBinding()]
param(
    [ValidateSet('Discovery', 'Qualification')]
    [string]$Mode = 'Qualification',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$MSBuildPath,
    [string]$EvidenceRoot
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($EvidenceRoot)) {
    $EvidenceRoot = Join-Path $repositoryRoot "artifacts\tests\setup-layout\evidence\$($Mode.ToLowerInvariant())"
}
New-Item -ItemType Directory -Force -Path $EvidenceRoot | Out-Null

$sourcePolicyPath = Join-Path $EvidenceRoot 'source-policy.json'
& (Join-Path $PSScriptRoot 'validate-setup-layout-source-policy.ps1') -OutputPath $sourcePolicyPath

$resolveArguments = @{ Toolset = '15' }
if (-not [string]::IsNullOrWhiteSpace($MSBuildPath)) { $resolveArguments.MSBuildPath = $MSBuildPath }
$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1') @resolveArguments

$buildRoot = Join-Path $repositoryRoot "artifacts\tests\setup-layout\$Configuration"
$installerOutput = Join-Path $buildRoot 'installer'
$uninstallerOutput = Join-Path $buildRoot 'uninstaller'
$installerObject = Join-Path $buildRoot 'obj-installer'
$uninstallerObject = Join-Path $buildRoot 'obj-uninstaller'
New-Item -ItemType Directory -Force -Path $installerOutput, $uninstallerOutput, $installerObject, $uninstallerObject | Out-Null

$setupBuilds = @(
    [PSCustomObject]@{
        Project = Join-Path $repositoryRoot 'Compact Cassette Catalogue Installer\Compact Cassette Catalogue Installer.vbproj'
        Output = $installerOutput
        Object = $installerObject
    },
    [PSCustomObject]@{
        Project = Join-Path $repositoryRoot 'Compact Cassette Catalogue Uninstaller\Compact Cassette Catalogue Uninstaller.vbproj'
        Output = $uninstallerOutput
        Object = $uninstallerObject
    }
)
foreach ($setupBuild in $setupBuilds) {
    & $msbuild $setupBuild.Project '/t:Rebuild' "/p:Configuration=$Configuration" '/p:Platform=x86' `
        "/p:OutputPath=$($setupBuild.Output)\" "/p:IntermediateOutputPath=$($setupBuild.Object)\" `
        '/p:TargetFrameworkVersion=v4.0' '/p:UseSharedCompilation=false' '/v:minimal' '/nologo'
    if ($LASTEXITCODE -ne 0) { throw "Setup layout input build failed with exit code $LASTEXITCODE." }
}

$layoutProject = Join-Path $repositoryRoot 'tests\C3.Layout.Characterization\C3.Layout.Characterization.vbproj'
& $msbuild $layoutProject '/t:Build' "/p:Configuration=$Configuration" '/p:Platform=AnyCPU' `
    '/p:UseSharedCompilation=false' '/v:minimal' '/nologo'
if ($LASTEXITCODE -ne 0) { throw "Layout characterization build failed with exit code $LASTEXITCODE." }

$harness = Join-Path $repositoryRoot "artifacts\tests\layout\$Configuration\C3.LayoutCharacterization.exe"
$installerAssembly = Join-Path $installerOutput 'Compact Cassette Catalogue Installer.exe'
$uninstallerAssembly = Join-Path $uninstallerOutput 'Compact Cassette Catalogue Uninstaller.exe'
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()

$cases = @(
    [PSCustomObject]@{ Name = 'installer-main-introduction'; Assembly = $installerAssembly; Type = 'Compact_Cassette_Catalogue_Installer.frmMain'; Page = 'pnlIntroduction' },
    [PSCustomObject]@{ Name = 'installer-main-options'; Assembly = $installerAssembly; Type = 'Compact_Cassette_Catalogue_Installer.frmMain'; Page = 'pnlOptions' },
    [PSCustomObject]@{ Name = 'installer-main-ready'; Assembly = $installerAssembly; Type = 'Compact_Cassette_Catalogue_Installer.frmMain'; Page = 'pnlReady' },
    [PSCustomObject]@{ Name = 'installer-main-install'; Assembly = $installerAssembly; Type = 'Compact_Cassette_Catalogue_Installer.frmMain'; Page = 'pnlInstall' },
    [PSCustomObject]@{ Name = 'installer-success'; Assembly = $installerAssembly; Type = 'Compact_Cassette_Catalogue_Installer.frmSuccess'; Page = 'pnlSuccess' },
    [PSCustomObject]@{ Name = 'installer-failure'; Assembly = $installerAssembly; Type = 'Compact_Cassette_Catalogue_Installer.frmFailure'; Page = 'pnlFailure' },
    [PSCustomObject]@{ Name = 'uninstaller-main-ready'; Assembly = $uninstallerAssembly; Type = 'Compact_Cassette_Catalogue_Uninstaller.frmMain'; Page = 'pnlReady' },
    [PSCustomObject]@{ Name = 'uninstaller-main-uninstall'; Assembly = $uninstallerAssembly; Type = 'Compact_Cassette_Catalogue_Uninstaller.frmMain'; Page = 'pnlUninstall' },
    [PSCustomObject]@{ Name = 'uninstaller-success'; Assembly = $uninstallerAssembly; Type = 'Compact_Cassette_Catalogue_Uninstaller.frmSuccess'; Page = 'pnlSuccess' },
    [PSCustomObject]@{ Name = 'uninstaller-failure'; Assembly = $uninstallerAssembly; Type = 'Compact_Cassette_Catalogue_Uninstaller.frmFailure'; Page = 'pnlFailure' }
)
$sizes = @(
    [PSCustomObject]@{ Width = 800; Height = 552 },
    [PSCustomObject]@{ Width = 1024; Height = 720 },
    [PSCustomObject]@{ Width = 1366; Height = 728 },
    [PSCustomObject]@{ Width = 1920; Height = 1040 }
)
$scales = @(1.0, 1.25, 1.5, 2.0)
$profiles = @('ordinary', 'maximum')
$cells = @()

foreach ($case in $cases) {
    foreach ($size in $sizes) {
        foreach ($scale in $scales) {
            foreach ($profile in $profiles) {
                $scaleText = ([double]$scale).ToString('0.00', [Globalization.CultureInfo]::InvariantCulture)
                $cellName = "$($case.Name)-$($size.Width)x$($size.Height)-s$scaleText-$profile"
                $resultPath = Join-Path $EvidenceRoot "$cellName.json"
                $screenshotPath = Join-Path $EvidenceRoot "$cellName.png"
                & $harness '--form' $case.Name '--assembly-path' $case.Assembly '--type-name' $case.Type `
                    '--page-state' $case.Page '--width' ([string]$size.Width) '--height' ([string]$size.Height) `
                    '--scale' $scaleText '--profile' $profile '--source-commit' $sourceCommit `
                    '--output' $resultPath '--screenshot' $screenshotPath
                $exitCode = $LASTEXITCODE
                if (-not (Test-Path -LiteralPath $resultPath)) {
                    throw "Fresh-process setup layout cell did not retain a result: $cellName (exit $exitCode)."
                }
                $result = Get-Content -LiteralPath $resultPath -Raw | ConvertFrom-Json
                $cells += [PSCustomObject][ordered]@{
                    name = $cellName
                    passed = [bool]$result.Passed
                    failureCount = @($result.Failures).Count
                    failures = @($result.Failures)
                    resultSha256 = (Get-FileHash -LiteralPath $resultPath -Algorithm SHA256).Hash.ToLowerInvariant()
                    screenshotSha256 = (Get-FileHash -LiteralPath $screenshotPath -Algorithm SHA256).Hash.ToLowerInvariant()
                }
            }
        }
    }
}

$summary = [PSCustomObject][ordered]@{
    schemaVersion = 1
    mode = $Mode
    sourceCommit = $sourceCommit
    harnessSha256 = (Get-FileHash -LiteralPath $harness -Algorithm SHA256).Hash.ToLowerInvariant()
    installerSha256 = (Get-FileHash -LiteralPath $installerAssembly -Algorithm SHA256).Hash.ToLowerInvariant()
    uninstallerSha256 = (Get-FileHash -LiteralPath $uninstallerAssembly -Algorithm SHA256).Hash.ToLowerInvariant()
    sourcePolicySha256 = (Get-FileHash -LiteralPath $sourcePolicyPath -Algorithm SHA256).Hash.ToLowerInvariant()
    caseCount = $cases.Count
    cellCount = $cells.Count
    passedCount = @($cells | Where-Object { $_.passed }).Count
    failedCount = @($cells | Where-Object { -not $_.passed }).Count
    cells = @($cells)
}
$summaryPath = Join-Path $EvidenceRoot 'summary.json'
$summary | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

$failed = @($cells | Where-Object { -not $_.passed })
if ($Mode -eq 'Qualification' -and $failed.Count -ne 0) {
    throw "Alpha 5 setup layout qualification failed in $($failed.Count) of $($cells.Count) fresh-process cells."
}
if ($Mode -eq 'Discovery') {
    Write-Host "Setup layout discovery retained $($cells.Count) cells: $($cells.Count - $failed.Count) passed, $($failed.Count) failed; evidence: $EvidenceRoot"
    return
}
Write-Host "Alpha 5 setup layout qualification passed in $($cells.Count) fresh STA processes; evidence: $EvidenceRoot"
