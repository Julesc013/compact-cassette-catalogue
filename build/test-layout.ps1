[CmdletBinding()]
param(
    [ValidateSet('Discovery', 'Qualification', 'Conditional')]
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
    $EvidenceRoot = Join-Path $repositoryRoot "artifacts\tests\layout\evidence\$($Mode.ToLowerInvariant())"
}
New-Item -ItemType Directory -Force -Path $EvidenceRoot | Out-Null

$sourcePolicyPath = Join-Path $EvidenceRoot 'source-policy.json'
& (Join-Path $PSScriptRoot 'validate-layout-source-policy.ps1') -Mode $Mode -OutputPath $sourcePolicyPath

$resolveArguments = @{}
if (-not [string]::IsNullOrWhiteSpace($MSBuildPath)) { $resolveArguments.MSBuildPath = $MSBuildPath }
$msbuild = & (Join-Path $PSScriptRoot 'resolve-msbuild.ps1') @resolveArguments
$project = Join-Path $repositoryRoot 'tests\C3.Layout.Characterization\C3.Layout.Characterization.vbproj'
& $msbuild $project '/t:Build' "/p:Configuration=$Configuration" '/p:Platform=AnyCPU' '/v:minimal' '/nologo'
if ($LASTEXITCODE -ne 0) { throw "Layout characterization build failed with exit code $LASTEXITCODE." }

$executable = Join-Path $repositoryRoot "artifacts\tests\layout\$Configuration\C3.LayoutCharacterization.exe"
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$forms = if ($Mode -eq 'Discovery') {
    @('frmMain', 'frmTapeNew', 'frmTapes', 'frmModels', 'frmBrands', 'frmDecks')
} elseif ($Mode -eq 'Conditional') {
    @('frmConsole', 'frmFindResults', 'frmStatistics', 'frmSettings', 'frmAbout')
} else {
    @(
        'frmMain', 'frmTapeNew', 'frmTapes', 'frmModels', 'frmBrands', 'frmDecks',
        'frmBrandNew', 'frmBrandEdit', 'frmModelNew', 'frmModelEdit', 'frmDeckNew', 'frmDeckEdit'
    )
}
$sizes = if ($Mode -eq 'Discovery') {
    @([PSCustomObject]@{ Width = 800; Height = 552 })
} else {
    @(
        [PSCustomObject]@{ Width = 800; Height = 552 },
        [PSCustomObject]@{ Width = 1024; Height = 720 },
        [PSCustomObject]@{ Width = 1366; Height = 728 },
        [PSCustomObject]@{ Width = 1920; Height = 1040 }
    )
}
$scales = if ($Mode -eq 'Discovery') { @(1.0) } else { @(1.0, 1.25, 1.5, 2.0) }
$profiles = if ($Mode -eq 'Discovery') { @('ordinary') } else { @('ordinary', 'maximum') }
$cells = @()

foreach ($form in $forms) {
    foreach ($size in $sizes) {
        foreach ($scale in $scales) {
            foreach ($profile in $profiles) {
                $scaleText = ([double]$scale).ToString('0.00', [Globalization.CultureInfo]::InvariantCulture)
                $cellName = "$form-$($size.Width)x$($size.Height)-s$scaleText-$profile"
                $resultPath = Join-Path $EvidenceRoot "$cellName.json"
                $screenshotPath = Join-Path $EvidenceRoot "$cellName.png"
                & $executable '--form' $form '--width' ([string]$size.Width) '--height' ([string]$size.Height) `
                    '--scale' $scaleText '--profile' $profile '--source-commit' $sourceCommit `
                    '--output' $resultPath '--screenshot' $screenshotPath
                $exitCode = $LASTEXITCODE
                if (-not (Test-Path -LiteralPath $resultPath)) {
                    throw "Fresh-process layout cell did not retain a result: $cellName (exit $exitCode)."
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
    executableSha256 = (Get-FileHash -LiteralPath $executable -Algorithm SHA256).Hash.ToLowerInvariant()
    cellCount = $cells.Count
    cells = @($cells)
}
$summaryPath = Join-Path $EvidenceRoot 'summary.json'
$summary | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

if ($Mode -eq 'Discovery') {
    $unexpectedPasses = @($cells | Where-Object { $_.passed })
    $missingRootCause = @($cells | Where-Object { 'FORM_AUTOSCROLL' -notin @($_.failures) })
    if ($unexpectedPasses.Count -ne 0 -or $missingRootCause.Count -ne 0) {
        throw 'Alpha 4 discovery did not reproduce the required form-level AutoScroll failure in every target cell.'
    }
    Write-Host "Alpha 4 layout failures reproduced in $($cells.Count) fresh STA processes; evidence: $EvidenceRoot"
    return
}

$failed = @($cells | Where-Object { -not $_.passed })
if ($failed.Count -ne 0) {
    throw "Alpha 5 layout qualification failed in $($failed.Count) of $($cells.Count) fresh-process cells."
}
$scope = if ($Mode -eq 'Conditional') { 'conditional-form characterization' } else { 'layout qualification' }
Write-Host "Alpha 5 $scope passed in $($cells.Count) fresh STA processes; evidence: $EvidenceRoot"
