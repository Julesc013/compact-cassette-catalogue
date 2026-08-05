[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$validator = Join-Path $PSScriptRoot 'validate-release-train.ps1'
$schemaPath = Join-Path $repositoryRoot 'spec\release-train\v2\train.schema.json'
$canonicalTrain = Join-Path $repositoryRoot 'release\train\2.0.0.json'
$canonicalCatalog = Join-Path $repositoryRoot 'release\catalog.v1.json'
$canonicalProps = Join-Path $PSScriptRoot 'Version.props'
$testRoot = Join-Path ([IO.Path]::GetTempPath()) (
    'c3-release-train-' + [Guid]::NewGuid().ToString('N'))
$trainPath = Join-Path $testRoot 'train.json'
$catalogPath = Join-Path $testRoot 'catalog.json'
$propsPath = Join-Path $testRoot 'Version.props'
$utf8WithoutBom = New-Object Text.UTF8Encoding($false)
$passed = 0

function Reset-Fixtures {
    Copy-Item -LiteralPath $canonicalTrain -Destination $trainPath -Force
    Copy-Item -LiteralPath $canonicalCatalog -Destination $catalogPath -Force
    Copy-Item -LiteralPath $canonicalProps -Destination $propsPath -Force

    # Keep failure-path scenarios independent of the live train's current
    # milestone. The canonical validator is exercised separately before this
    # harness; these fixtures always begin from the Alpha 1 controller shape so
    # the same negative mutations remain meaningful through Beta 1.
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $train.currentMilestone = 'alpha.1'
    $train.status = 'active'
    $train.lastQualifiedTag = $null
    $train.candidateCommit = $null
    foreach ($milestone in @($train.milestones)) {
        $milestone.state = if ([string]$milestone.id -ceq 'alpha.1') {
            'active'
        }
        else {
            'pending'
        }
    }
    Write-JsonFixture $trainPath $train

    $catalog = Get-Content -LiteralPath $catalogPath -Raw | ConvertFrom-Json
    $catalog.milestones = @($catalog.milestones | Select-Object -First 1)
    Write-JsonFixture $catalogPath $catalog

    $props = [IO.File]::ReadAllText($propsPath)
    $props = [regex]::Replace(
        $props,
        '<C3ReleaseStage>[^<]+</C3ReleaseStage>',
        '<C3ReleaseStage>Alpha 1</C3ReleaseStage>')
    $props = [regex]::Replace(
        $props,
        '<C3FileVersion>[^<]+</C3FileVersion>',
        '<C3FileVersion>2.0.0.1</C3FileVersion>')
    [IO.File]::WriteAllText($propsPath, $props, $utf8WithoutBom)
}

function Write-JsonFixture {
    param(
        [string]$Path,
        [object]$Value
    )

    [IO.File]::WriteAllText(
        $Path,
        (($Value | ConvertTo-Json -Depth 100) + "`n"),
        $utf8WithoutBom)
}

function Invoke-TrainValidator {
    $arguments = @(
        '-NoLogo',
        '-NoProfile',
        '-NonInteractive',
        '-ExecutionPolicy', 'Bypass',
        '-File', $validator,
        '-TrainPath', $trainPath,
        '-SchemaPath', $schemaPath,
        '-CatalogPath', $catalogPath,
        '-VersionPropsPath', $propsPath,
        '-SkipGitFacts')
    $savedErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        [void](& powershell.exe @arguments 2>&1)
        return $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
}

function Assert-Passes {
    param([string]$Scenario)

    if ((Invoke-TrainValidator) -ne 0) {
        throw "$Scenario`: expected release-train validation to pass."
    }
    $script:passed++
}

function Assert-Fails {
    param([string]$Scenario)

    if ((Invoke-TrainValidator) -eq 0) {
        throw "$Scenario`: expected release-train validation to fail."
    }
    $script:passed++
}

try {
    [IO.Directory]::CreateDirectory($testRoot) | Out-Null

    Reset-Fixtures
    Assert-Passes 'canonical Alpha 1 controller'

    Reset-Fixtures
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $train.publicationPolicy.stable = 'unchanged-rc-promotion'
    Write-JsonFixture $trainPath $train
    Assert-Fails 'obsolete unchanged-RC stable policy'

    Reset-Fixtures
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $first = $train.milestones[0]
    $train.milestones[0] = $train.milestones[1]
    $train.milestones[1] = $first
    Write-JsonFixture $trainPath $train
    Assert-Fails 'milestone reordering'

    Reset-Fixtures
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $train.milestones = @($train.milestones | Select-Object -First 12)
    Write-JsonFixture $trainPath $train
    Assert-Fails 'missing Beta milestone'

    Reset-Fixtures
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $train.milestones[11].id = 'alpha.13'
    $train.milestones[11].releaseLabel = '2.0.0-alpha.13'
    Write-JsonFixture $trainPath $train
    Assert-Fails 'unsupported Alpha 13 milestone'

    Reset-Fixtures
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $train.lastQualifiedTag = 'v2.0.0-alpha.1'
    Write-JsonFixture $trainPath $train
    Assert-Fails 'premature last-qualified tag'

    Reset-Fixtures
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $train.candidateCommit = '0000000000000000000000000000000000000000'
    Write-JsonFixture $trainPath $train
    Assert-Fails 'candidate SHA disagreement'

    Reset-Fixtures
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $train.status = 'awaiting-owner-manual-validation'
    $train.milestones[0].state = 'awaiting-owner-manual-validation'
    Write-JsonFixture $trainPath $train
    Assert-Fails 'owner acceptance on an Alpha'

    Reset-Fixtures
    $train = Get-Content -LiteralPath $trainPath -Raw | ConvertFrom-Json
    $train.currentMilestone = 'alpha.2'
    $train.lastQualifiedTag = 'v2.0.0-alpha.1'
    $train.milestones[0].state = 'qualified'
    $train.milestones[1].state = 'active'
    $props = [IO.File]::ReadAllText($propsPath)
    $props = $props.Replace('<C3ReleaseStage>Alpha 1</C3ReleaseStage>',
        '<C3ReleaseStage>Alpha 2</C3ReleaseStage>')
    $props = $props.Replace('<C3FileVersion>2.0.0.1</C3FileVersion>',
        '<C3FileVersion>2.0.0.2</C3FileVersion>')
    [IO.File]::WriteAllText($propsPath, $props, $utf8WithoutBom)
    Write-JsonFixture $trainPath $train
    Assert-Fails 'advanced pointer without qualified catalogue evidence'

    Reset-Fixtures
    $props = [IO.File]::ReadAllText($propsPath)
    $props = $props.Replace('<C3ReleaseStage>Alpha 1</C3ReleaseStage>',
        '<C3ReleaseStage>Alpha 2</C3ReleaseStage>')
    $props = $props.Replace('<C3FileVersion>2.0.0.1</C3FileVersion>',
        '<C3FileVersion>2.0.0.2</C3FileVersion>')
    [IO.File]::WriteAllText($propsPath, $props, $utf8WithoutBom)
    Assert-Fails 'build identity disagreement'
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        $resolvedTestRoot = [IO.Path]::GetFullPath($testRoot)
        $resolvedTempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
        if (-not $resolvedTestRoot.StartsWith(
                $resolvedTempRoot,
                [StringComparison]::OrdinalIgnoreCase) -or
            [IO.Path]::GetFileName($resolvedTestRoot) -cnotmatch
                '^c3-release-train-[0-9a-f]{32}$') {
            throw "Refusing to remove unsafe release-train test path: $resolvedTestRoot"
        }
        Remove-Item -LiteralPath $resolvedTestRoot -Recurse -Force
    }
}

# GitHub Actions appends `exit $LASTEXITCODE` to PowerShell steps. Expected
# negative child-process cases must not leak their exit code past a successful
# test harness.
$global:LASTEXITCODE = 0
Write-Host "Release-train tests passed: $passed scenarios."
