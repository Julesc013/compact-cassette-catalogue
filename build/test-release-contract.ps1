[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts'))
$testRoot = [IO.Path]::GetFullPath((Join-Path $artifactsRoot "release-contract-tests-$PID"))
$canonicalCatalogPath = Join-Path $repositoryRoot 'release\catalog.v1.json'
$schemaPath = Join-Path $repositoryRoot 'spec\release-catalog\v1\catalog.schema.json'
$schemaValidator = Join-Path $PSScriptRoot 'validate-json-document.ps1'
$releaseValidator = Join-Path $PSScriptRoot 'validate-release-contract.ps1'
$labelResolver = Join-Path $PSScriptRoot 'resolve-release-label.ps1'
$utf8WithoutBom = New-Object Text.UTF8Encoding($false)
$passed = 0

function Assert-SafeArtifactsPath {
    param([string]$Path)

    $fullPath = [IO.Path]::GetFullPath($Path)
    $fullArtifactsRoot = [IO.Path]::GetFullPath($artifactsRoot)
    $fullArtifactsPrefix = $fullArtifactsRoot + [IO.Path]::DirectorySeparatorChar
    if (-not $fullPath.StartsWith($fullArtifactsPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to modify a path outside artifacts: $fullPath"
    }

    $currentPath = $fullPath
    while ($true) {
        if (Test-Path -LiteralPath $currentPath) {
            $item = Get-Item -LiteralPath $currentPath -Force
            if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw "Refusing to modify a reparse-point path: $currentPath"
            }
        }
        if ($currentPath.Equals($fullArtifactsRoot, [StringComparison]::OrdinalIgnoreCase)) {
            break
        }
        $parent = [IO.Directory]::GetParent($currentPath)
        if ($null -eq $parent) {
            throw "Could not validate artifacts ancestry for: $fullPath"
        }
        $currentPath = $parent.FullName
    }
}

Assert-SafeArtifactsPath $testRoot
if (Test-Path -LiteralPath $testRoot) {
    Remove-Item -LiteralPath $testRoot -Recurse -Force
}
[IO.Directory]::CreateDirectory($testRoot) | Out-Null

function New-CatalogCopy {
    return Get-Content -LiteralPath $canonicalCatalogPath -Raw | ConvertFrom-Json
}

function New-PlannedMilestone {
    param(
        [string]$ProductVersion,
        [string]$Stage,
        [string]$Predecessor
    )

    $milestone = ((New-CatalogCopy).milestones[0] |
        ConvertTo-Json -Depth 100 | ConvertFrom-Json)
    $resolved = & $labelResolver `
        -ProductVersion $ProductVersion `
        -ReleaseStage $Stage
    $milestone.releaseLabel = $resolved.ReleaseLabel
    $milestone.productVersion = $ProductVersion
    $milestone.stage = $Stage
    $milestone.channel = $resolved.ReleaseChannel
    $milestone.predecessor = $Predecessor
    $milestone.supersededBy = $null
    $milestone.qualification.state = 'blocked'
    $milestone.qualification.sourceCommit = $null
    $milestone.promotion.state = 'unpromoted'
    $milestone.promotion.tag = 'v' + $resolved.ReleaseLabel
    $milestone.promotion.tagObject = $null
    $milestone.publication.policy = if ($resolved.ReleaseChannel -ceq 'alpha') {
        'intentionally-unpublished'
    }
    elseif ($resolved.ReleaseChannel -ceq 'stable') {
        'public-stable'
    }
    else {
        'public-prerelease'
    }
    $milestone.publication.state = 'unpublished'
    $milestone.publication.releaseUrl = $null
    $milestone.publication.feedPromoted = $false
    $milestone.postVerification.state = 'not-applicable'
    $milestone.validationRecord =
        "release/validation/$($resolved.ReleaseLabel).md"
    $milestone.packages = @()
    $milestone.checksumManifest = $null
    return $milestone
}

function Write-CatalogFixture {
    param(
        [object]$Catalog,
        [string]$Name
    )

    $path = Join-Path $testRoot $Name
    $json = $Catalog | ConvertTo-Json -Depth 20
    [IO.File]::WriteAllText($path, $json + [Environment]::NewLine, $utf8WithoutBom)
    return $path
}

function Write-JsonDocument {
    param(
        [object]$Value,
        [string]$Path
    )

    $json = $Value | ConvertTo-Json -Depth 100
    [IO.File]::WriteAllText($Path, $json + [Environment]::NewLine, $utf8WithoutBom)
}

function Write-ValidationFixture {
    param(
        [string]$Path,
        [string]$Qualification,
        [string]$Promotion,
        [string]$SourceCommit,
        [string[]]$Hashes,
        [string]$PublicationPolicy = 'intentionally-unpublished',
        [string]$PublicationState = 'unpublished',
        [string]$PostVerification = 'not-applicable'
    )

    $lines = @(
        '# Release contract fixture'
        ''
        "Qualification: **$Qualification**"
        "Promotion: **$Promotion**"
        "Publication policy: **$PublicationPolicy**"
        "Publication state: **$PublicationState**"
        "Post-verification: **$PostVerification**"
    )
    if (-not [string]::IsNullOrWhiteSpace($SourceCommit)) {
        $lines += "Source commit: $SourceCommit"
    }
    foreach ($hash in @($Hashes)) {
        $lines += "Artifact SHA-256: $hash"
    }
    [IO.File]::WriteAllText(
        $Path,
        ($lines -join [Environment]::NewLine) + [Environment]::NewLine,
        $utf8WithoutBom)
}

function Invoke-FixtureGit {
    param(
        [string]$Root,
        [string[]]$Arguments
    )

    $savedErrorActionPreference = $ErrorActionPreference
    $exitCode = 0
    try {
        $ErrorActionPreference = 'Continue'
        & git -C $Root @Arguments 1>$null 2>$null
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
    if ($exitCode -ne 0) {
        throw "Fixture Git command failed: git $($Arguments -join ' ')"
    }
}

function Assert-SchemaPatternsCompile {
    param(
        [object]$Value,
        [string]$Path
    )

    if ($null -eq $Value) {
        return
    }
    if ($Value -is [PSCustomObject]) {
        foreach ($property in $Value.PSObject.Properties) {
            $propertyPath = "$Path/$($property.Name)"
            if ($property.Name -ceq 'pattern') {
                if ($property.Value -isnot [string]) {
                    throw "JSON Schema pattern at '$propertyPath' is not a string."
                }
                try {
                    [void]([regex]([string]$property.Value))
                }
                catch {
                    throw "JSON Schema pattern at '$propertyPath' does not compile: $($_.Exception.Message)"
                }
            }
            else {
                Assert-SchemaPatternsCompile $property.Value $propertyPath
            }
        }
        return
    }
    if ($Value -is [Array]) {
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Assert-SchemaPatternsCompile $Value[$index] "$Path/$index"
        }
    }
}

function Assert-FailsWith {
    param(
        [scriptblock]$Action,
        [string]$ExpectedFragment,
        [string]$Scenario
    )

    try {
        & $Action | Out-Null
    }
    catch {
        if ($_.Exception.Message.IndexOf($ExpectedFragment, [StringComparison]::Ordinal) -lt 0) {
            throw "$Scenario failed for an unexpected reason: $($_.Exception.Message)"
        }
        $script:passed++
        return
    }
    throw "$Scenario unexpectedly passed."
}

try {
    & $schemaValidator -SchemaPath $schemaPath -DocumentPath $canonicalCatalogPath | Out-Null
    $schemaDocument = Get-Content -LiteralPath $schemaPath -Raw | ConvertFrom-Json
    Assert-SchemaPatternsCompile $schemaDocument '$'
    & $releaseValidator | Out-Null
    $passed++

    foreach ($labelCase in @(
            [PSCustomObject]@{ Stage = 'Alpha 2'; Label = '2.0.0-alpha.2'; Channel = 'alpha' }
            [PSCustomObject]@{ Stage = 'Beta 3'; Label = '2.0.0-beta.3'; Channel = 'beta' }
            [PSCustomObject]@{ Stage = 'Release Candidate 4'; Label = '2.0.0-rc.4'; Channel = 'beta' }
            [PSCustomObject]@{ Stage = 'Release'; Label = '2.0.0'; Channel = 'stable' })) {
        $resolved = & $labelResolver -ProductVersion '2.0.0' -ReleaseStage $labelCase.Stage
        if ($resolved.ReleaseLabel -cne $labelCase.Label -or
            $resolved.ReleaseChannel -cne $labelCase.Channel) {
            throw "release-label projection failed for $($labelCase.Stage)."
        }
        $passed++
    }
    Assert-FailsWith {
        & $labelResolver -ProductVersion '02.0.0' -ReleaseStage 'Alpha 1'
    } 'canonical three-part numeric version' 'non-canonical product version'

    $extraProperty = New-CatalogCopy
    $extraProperty | Add-Member -NotePropertyName unexpected -NotePropertyValue $true
    $extraPropertyPath = Write-CatalogFixture $extraProperty 'extra-property.json'
    Assert-FailsWith {
        & $schemaValidator -SchemaPath $schemaPath -DocumentPath $extraPropertyPath
    } 'unsupported property' 'extra root property'

    $wrongBoolean = New-CatalogCopy
    $wrongBoolean.milestones[0].publication.feedPromoted = 'false'
    $wrongBooleanPath = Write-CatalogFixture $wrongBoolean 'wrong-boolean.json'
    Assert-FailsWith {
        & $schemaValidator -SchemaPath $schemaPath -DocumentPath $wrongBooleanPath
    } "expected boolean" 'wrong Boolean type'

    $missingProperty = New-CatalogCopy
    $missingProperty.PSObject.Properties.Remove('productId')
    $missingPropertyPath = Write-CatalogFixture $missingProperty 'missing-property.json'
    Assert-FailsWith {
        & $schemaValidator -SchemaPath $schemaPath -DocumentPath $missingPropertyPath
    } "missing required property 'productId'" 'missing required property'

    $invalidStage = New-CatalogCopy
    $invalidStage.milestones[0].stage = 'Preview 1'
    $invalidStagePath = Write-CatalogFixture $invalidStage 'invalid-stage.json'
    Assert-FailsWith {
        & $schemaValidator -SchemaPath $schemaPath -DocumentPath $invalidStagePath
    } 'required pattern' 'invalid release stage'

    $lowerProduct = New-CatalogCopy
    $lowerProduct.milestones = @($lowerProduct.milestones) + @(
        (New-PlannedMilestone '1.9.0' 'Alpha 1' '2.0.0-alpha.1'))
    $lowerProductPath = Write-CatalogFixture $lowerProduct 'lower-product-order.json'
    Assert-FailsWith {
        & $releaseValidator -CatalogOverridePath $lowerProductPath
    } 'must be strictly later than predecessor' 'lower product version transition'

    $stageRegression = New-CatalogCopy
    $stableMilestone = New-PlannedMilestone `
        '2.0.0' `
        'Release' `
        '2.0.0-alpha.1'
    $releaseCandidateMilestone = New-PlannedMilestone `
        '2.0.0' `
        'Release Candidate 1' `
        '2.0.0'
    $stageRegression.milestones = @($stageRegression.milestones) + @(
        $stableMilestone,
        $releaseCandidateMilestone)
    $stageRegressionPath = Write-CatalogFixture `
        $stageRegression `
        'stage-regression-order.json'
    Assert-FailsWith {
        & $releaseValidator -CatalogOverridePath $stageRegressionPath
    } 'must be strictly later than predecessor' 'stable to release-candidate regression'

    $ordinalRegression = New-CatalogCopy
    $alphaTen = New-PlannedMilestone `
        '2.0.0' `
        'Alpha 10' `
        '2.0.0-alpha.1'
    $alphaTwo = New-PlannedMilestone `
        '2.0.0' `
        'Alpha 2' `
        '2.0.0-alpha.10'
    $ordinalRegression.milestones = @($ordinalRegression.milestones) + @(
        $alphaTen,
        $alphaTwo)
    $ordinalRegressionPath = Write-CatalogFixture `
        $ordinalRegression `
        'ordinal-regression-order.json'
    Assert-FailsWith {
        & $releaseValidator -CatalogOverridePath $ordinalRegressionPath
    } 'must be strictly later than predecessor' 'alpha ordinal regression'

    $packagePathViolation = New-CatalogCopy
    $packagePathViolation.milestones[0].packages = @(
        [PSCustomObject]@{
            lane = 'win-x86-net40'
            file = '..\escape.zip'
            length = 1
            sha256 = ('1' * 64) -join ''
        })
    $packagePathViolationPath = Write-CatalogFixture `
        $packagePathViolation `
        'package-path-violation.json'
    Assert-FailsWith {
        & $schemaValidator -SchemaPath $schemaPath -DocumentPath $packagePathViolationPath
    } 'required pattern' 'package path traversal'

    $channelConflict = New-CatalogCopy
    $channelConflict.milestones[0].channel = 'beta'
    $channelConflict.milestones[0].publication.policy = 'public-prerelease'
    $channelConflict.milestones[0].postVerification.state = 'not-applicable'
    $channelConflictPath = Write-CatalogFixture $channelConflict 'channel-conflict.json'
    Assert-FailsWith {
        & $releaseValidator -CatalogOverridePath $channelConflictPath
    } "channel must be 'alpha'" 'stage/channel semantic conflict'

    $escapingRecord = New-CatalogCopy
    $escapingRecord.milestones[0].validationRecord = 'release/validation/../README.md'
    $escapingRecordPath = Write-CatalogFixture $escapingRecord 'escaping-record.json'
    Assert-FailsWith {
        & $releaseValidator -CatalogOverridePath $escapingRecordPath
    } "validationRecord must be 'release/validation/2.0.0-alpha.1.md'" 'validation path ownership'

    $historyRoot = Join-Path $testRoot 'history-repository'
    $historyRemoteRoot = Join-Path $testRoot 'history-origin.git'
    $historyBuildRoot = Join-Path $historyRoot 'build'
    $historySchemaRoot = Join-Path $historyRoot 'spec\release-catalog\v1'
    $historyBranchSchemaRoot = Join-Path $historyRoot 'spec\branch-contract\v1'
    $historyValidationRoot = Join-Path $historyRoot 'release\validation'
    foreach ($directory in @(
            $historyBuildRoot,
            $historySchemaRoot,
            $historyBranchSchemaRoot,
            $historyValidationRoot)) {
        [IO.Directory]::CreateDirectory($directory) | Out-Null
    }
    foreach ($scriptName in @(
            'get-release-identity.ps1',
            'get-release-packages.ps1',
            'get-branch-contract.ps1',
            'resolve-release-label.ps1',
            'validate-json-document.ps1',
            'validate-release-contract.ps1')) {
        Copy-Item `
            -LiteralPath (Join-Path $PSScriptRoot $scriptName) `
            -Destination (Join-Path $historyBuildRoot $scriptName)
    }
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Version.props') `
        -Destination (Join-Path $historyBuildRoot 'Version.props')
    $historyPropsPath = Join-Path $historyBuildRoot 'Version.props'
    $historyProps = [IO.File]::ReadAllText($historyPropsPath)
    $historyProps = [regex]::Replace(
        $historyProps,
        '<C3ReleaseStage>[^<]+</C3ReleaseStage>',
        '<C3ReleaseStage>Alpha 1</C3ReleaseStage>')
    $historyProps = [regex]::Replace(
        $historyProps,
        '<C3FileVersion>[^<]+</C3FileVersion>',
        '<C3FileVersion>2.0.0.1</C3FileVersion>')
    [IO.File]::WriteAllText($historyPropsPath, $historyProps, $utf8WithoutBom)
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') `
        -Destination (Join-Path $historyBuildRoot 'lanes.json')
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'branches.json') `
        -Destination (Join-Path $historyBuildRoot 'branches.json')
    Copy-Item `
        -LiteralPath (Join-Path $repositoryRoot 'spec\branch-contract\v1\branches.schema.json') `
        -Destination (Join-Path $historyBranchSchemaRoot 'branches.schema.json')
    Copy-Item -LiteralPath $schemaPath `
        -Destination (Join-Path $historySchemaRoot 'catalog.schema.json')

    $historyCatalogPath = Join-Path $historyRoot 'release\catalog.v1.json'
    $historyValidationPath = Join-Path $historyValidationRoot '2.0.0-alpha.1.md'
    $historyCatalog = New-CatalogCopy
    # The canonical repository may be at either E or P. This synthetic history
    # always starts at pre-qualification C, so project every mutable lifecycle
    # field explicitly instead of inheriting the live checkpoint phase.
    $historyCatalog.milestones = @($historyCatalog.milestones | Select-Object -First 1)
    $historyCatalog.milestones[0].qualification.state = 'blocked'
    $historyCatalog.milestones[0].qualification.sourceCommit = $null
    $historyCatalog.milestones[0].promotion.state = 'unpromoted'
    $historyCatalog.milestones[0].promotion.tagObject = $null
    $historyCatalog.milestones[0].publication.state = 'unpublished'
    $historyCatalog.milestones[0].publication.releaseUrl = $null
    $historyCatalog.milestones[0].publication.feedPromoted = $false
    $historyCatalog.milestones[0].postVerification.state = 'not-applicable'
    $historyCatalog.milestones[0].packages = @()
    $historyCatalog.milestones[0].checksumManifest = $null
    Write-JsonDocument $historyCatalog $historyCatalogPath
    Write-ValidationFixture `
        $historyValidationPath `
        'blocked' `
        'unpromoted' `
        $null `
        @()

    Invoke-FixtureGit $testRoot @('init', '--bare', $historyRemoteRoot)
    Invoke-FixtureGit $historyRoot @('init')
    Invoke-FixtureGit $historyRoot @('symbolic-ref', 'HEAD', 'refs/heads/master')
    Invoke-FixtureGit $historyRoot @('config', 'user.name', 'C3 Release Contract Tests')
    Invoke-FixtureGit $historyRoot @('config', 'user.email', 'release-contract-tests@invalid.example')
    Invoke-FixtureGit $historyRoot @('remote', 'add', 'origin', $historyRemoteRoot)
    Invoke-FixtureGit $historyRoot @('add', '--all')
    Invoke-FixtureGit $historyRoot @('commit', '-m', 'Fixture source C')
    $sourceCommit = ([string](& git -C $historyRoot rev-parse HEAD)).Trim()
    if ($LASTEXITCODE -ne 0 -or $sourceCommit -cnotmatch '^[0-9a-f]{40}$') {
        throw 'Could not resolve fixture source commit C.'
    }
    Invoke-FixtureGit $historyRoot @('branch', 'dev/2.x', $sourceCommit)
    Invoke-FixtureGit $historyRoot @('push', '--quiet', 'origin', 'master', 'dev/2.x')

    $historyIdentity = & (Join-Path $historyBuildRoot 'get-release-identity.ps1')
    $historyPackageDefinitions = @(& (Join-Path $historyBuildRoot 'get-release-packages.ps1') `
        -Identity $historyIdentity)
    $historyHashes = New-Object Collections.Generic.List[String]
    $historyPackages = New-Object Collections.Generic.List[Object]
    for ($index = 0; $index -lt $historyPackageDefinitions.Count; $index++) {
        $hash = (([string]($index + 1)) * 64) -join ''
        $historyHashes.Add($hash)
        $historyPackages.Add([PSCustomObject]@{
                lane = $historyPackageDefinitions[$index].LaneId
                file = $historyPackageDefinitions[$index].FileName
                length = $index + 1
                sha256 = $hash
            })
    }
    $manifestHash = ('9' * 64) -join ''
    $historyHashes.Add($manifestHash)
    $historyCatalog.milestones[0].qualification.state = 'pass'
    $historyCatalog.milestones[0].qualification.sourceCommit = $sourceCommit
    $historyCatalog.milestones[0].packages = $historyPackages.ToArray()
    $historyCatalog.milestones[0].checksumManifest = [PSCustomObject]@{
        file = 'SHA256SUMS.txt'
        length = 3
        sha256 = $manifestHash
    }
    Write-JsonDocument $historyCatalog $historyCatalogPath
    Write-ValidationFixture `
        $historyValidationPath `
        'pass' `
        'unpromoted' `
        $sourceCommit `
        $historyHashes.ToArray()
    Invoke-FixtureGit $historyRoot @('add', '--all')
    Invoke-FixtureGit $historyRoot @('commit', '-m', 'Fixture attestation E')
    $historyValidator = Join-Path $historyBuildRoot 'validate-release-contract.ps1'
    Assert-FailsWith {
        & $historyValidator -Mode Candidate -ExpectedCommit (
            ([string](& git -C $historyRoot rev-parse HEAD)).Trim())
    } 'requires -RequireArtifacts' 'candidate permits origin dev to remain at frozen C'

    Invoke-FixtureGit $historyRoot @('push', '--quiet', 'origin', 'master:dev/2.x')
    Assert-FailsWith {
        & $historyValidator -Mode Candidate -ExpectedCommit (
            ([string](& git -C $historyRoot rev-parse HEAD)).Trim())
    } 'origin/dev/2.x must remain at frozen source C' 'candidate rejects origin dev/2.x advancing to E'

    Invoke-FixtureGit $historyRoot @('push', '--quiet', 'origin', 'master:master')
    Assert-FailsWith {
        & $historyValidator -Mode Master
    } 'must exist as an annotated tag' 'master E without its atomic annotated tag'

    Invoke-FixtureGit $historyRoot @(
        'tag',
        '-a',
        'v2.0.0-alpha.1',
        '-m',
        'Fixture qualified checkpoint')
    $historyTagObject = ([string](& git -C $historyRoot rev-parse refs/tags/v2.0.0-alpha.1)).Trim()
    $historyTagCommit = ([string](& git -C $historyRoot rev-parse 'refs/tags/v2.0.0-alpha.1^{}')).Trim()
    if ($LASTEXITCODE -ne 0 -or
        $historyTagObject -cnotmatch '^[0-9a-f]{40}$' -or
        $historyTagCommit -cnotmatch '^[0-9a-f]{40}$') {
        throw 'Could not resolve fixture alpha annotated-tag identity.'
    }

    $historyCatalog.milestones[0].promotion.state = 'tagged'
    $historyCatalog.milestones[0].promotion.tagObject = $historyTagObject
    Write-JsonDocument $historyCatalog $historyCatalogPath
    Write-ValidationFixture `
        $historyValidationPath `
        'pass' `
        'tagged' `
        $sourceCommit `
        $historyHashes.ToArray()
    Invoke-FixtureGit $historyRoot @('add', '--all')
    Invoke-FixtureGit $historyRoot @('commit', '-m', 'Fixture post-promotion P')
    Invoke-FixtureGit $historyRoot @('branch', '-f', 'dev/2.x', 'master')
    Invoke-FixtureGit $historyRoot @('push', '--quiet', 'origin', 'master', 'dev/2.x', '--tags')

    & $historyValidator -Mode Master | Out-Null
    $passed++

    Invoke-FixtureGit $historyRoot @(
        'push',
        '--quiet',
        'origin',
        ':refs/tags/v2.0.0-alpha.1')
    Assert-FailsWith {
        & $historyValidator -Mode Master
    } 'with both tag-object and peeled-commit identities' 'deleted remote tag with stale local tag'
    Invoke-FixtureGit $historyRoot @(
        'push',
        '--quiet',
        'origin',
        'refs/tags/v2.0.0-alpha.1:refs/tags/v2.0.0-alpha.1')

    Invoke-FixtureGit $historyRoot @(
        'tag',
        '-f',
        '-a',
        'v2.0.0-alpha.1',
        '-m',
        'Adversarial replacement with unchanged target',
        $historyTagCommit)
    $replacementTagObject = ([string](& git -C $historyRoot rev-parse refs/tags/v2.0.0-alpha.1)).Trim()
    $replacementPeeledCommit = ([string](& git -C $historyRoot rev-parse 'refs/tags/v2.0.0-alpha.1^{}')).Trim()
    if ($LASTEXITCODE -ne 0 -or
        $replacementTagObject -cnotmatch '^[0-9a-f]{40}$' -or
        $replacementTagObject -ceq $historyTagObject -or
        $replacementPeeledCommit -cne $historyTagCommit) {
        throw 'Could not construct a replacement tag object with an unchanged peeled commit.'
    }
    Invoke-FixtureGit $historyRoot @(
        'push',
        '--quiet',
        '--force',
        'origin',
        'refs/tags/v2.0.0-alpha.1:refs/tags/v2.0.0-alpha.1')
    Assert-FailsWith {
        & $historyValidator -Mode Master
    } "origin tag 'v2.0.0-alpha.1' tag object differs from recorded identity" 'replaced remote tag object with unchanged peeled commit'
    Invoke-FixtureGit $historyRoot @(
        'update-ref',
        'refs/tags/v2.0.0-alpha.1',
        $historyTagObject)
    Invoke-FixtureGit $historyRoot @(
        'push',
        '--quiet',
        '--force',
        'origin',
        'refs/tags/v2.0.0-alpha.1:refs/tags/v2.0.0-alpha.1')

    $replacementHash = ('8' * 64) -join ''
    $originalHash = [string]$historyCatalog.milestones[0].packages[0].sha256
    $historyCatalog.milestones[0].packages[0].sha256 = $replacementHash
    Write-JsonDocument $historyCatalog $historyCatalogPath
    $mutatedHashes = @($historyHashes | ForEach-Object {
            if ($_ -ceq $originalHash) { $replacementHash } else { $_ }
        })
    Write-ValidationFixture `
        $historyValidationPath `
        'pass' `
        'tagged' `
        $sourceCommit `
        $mutatedHashes
    Assert-FailsWith {
        & $historyValidator -Mode Repository
    } 'immutable fields differ' 'tagged artifact evidence mutation'

    $historyCatalog.milestones[0].packages[0].sha256 = $originalHash
    Write-ValidationFixture `
        $historyValidationPath `
        'pass' `
        'tagged' `
        $sourceCommit `
        $historyHashes.ToArray()

    $historyPropsPath = Join-Path $historyBuildRoot 'Version.props'
    $historyPropsText = [IO.File]::ReadAllText($historyPropsPath)
    $historyPropsText = $historyPropsText.Replace(
        '<C3ReleaseStage>Alpha 1</C3ReleaseStage>',
        '<C3ReleaseStage>Beta 1</C3ReleaseStage>')
    $historyPropsText = $historyPropsText.Replace(
        '<C3ReleaseChannel>alpha</C3ReleaseChannel>',
        '<C3ReleaseChannel>beta</C3ReleaseChannel>')
    $historyPropsText = $historyPropsText.Replace(
        '/dev/2.x/release/feeds/alpha/release.json',
        '/master/release/feeds/beta/release.json')
    [IO.File]::WriteAllText($historyPropsPath, $historyPropsText, $utf8WithoutBom)

    $betaValidationPath = Join-Path $historyValidationRoot '2.0.0-beta.1.md'
    $betaMilestone = [PSCustomObject][ordered]@{
        releaseLabel = '2.0.0-beta.1'
        productVersion = '2.0.0'
        stage = 'Beta 1'
        channel = 'beta'
        predecessor = '2.0.0-alpha.1'
        qualification = [PSCustomObject][ordered]@{
            state = 'blocked'
            sourceCommit = $null
        }
        supersededBy = $null
        promotion = [PSCustomObject][ordered]@{
            state = 'unpromoted'
            targetBranch = 'master'
            tag = 'v2.0.0-beta.1'
            tagObject = $null
        }
        publication = [PSCustomObject][ordered]@{
            policy = 'public-prerelease'
            state = 'unpublished'
            releaseUrl = $null
            feedPromoted = $false
        }
        postVerification = [PSCustomObject][ordered]@{
            state = 'not-applicable'
        }
        validationRecord = 'release/validation/2.0.0-beta.1.md'
        packages = @()
        checksumManifest = $null
    }
    $historyCatalog.milestones = @($historyCatalog.milestones) + @($betaMilestone)
    Write-JsonDocument $historyCatalog $historyCatalogPath
    Write-ValidationFixture `
        -Path $betaValidationPath `
        -Qualification 'blocked' `
        -Promotion 'unpromoted' `
        -SourceCommit $null `
        -Hashes @() `
        -PublicationPolicy 'public-prerelease'

    $historyFeedSchemaRoot = Join-Path $historyRoot 'spec\update-feed\v1'
    [IO.Directory]::CreateDirectory($historyFeedSchemaRoot) | Out-Null
    Copy-Item `
        -LiteralPath (Join-Path $repositoryRoot 'spec\update-feed\v1\release.schema.json') `
        -Destination (Join-Path $historyFeedSchemaRoot 'release.schema.json')
    Invoke-FixtureGit $historyRoot @('add', '--all')
    Invoke-FixtureGit $historyRoot @('commit', '-m', 'Fixture beta source C')
    $betaSourceCommit = ([string](& git -C $historyRoot rev-parse HEAD)).Trim()
    if ($LASTEXITCODE -ne 0 -or $betaSourceCommit -cnotmatch '^[0-9a-f]{40}$') {
        throw 'Could not resolve fixture beta source commit C.'
    }

    $betaIdentity = & (Join-Path $historyBuildRoot 'get-release-identity.ps1')
    $betaPackageDefinitions = @(& (Join-Path $historyBuildRoot 'get-release-packages.ps1') `
        -Identity $betaIdentity)
    $betaHashes = New-Object Collections.Generic.List[String]
    $betaPackages = New-Object Collections.Generic.List[Object]
    for ($index = 0; $index -lt $betaPackageDefinitions.Count; $index++) {
        $hash = (([string]($index + 4)) * 64) -join ''
        $betaHashes.Add($hash)
        $betaPackages.Add([PSCustomObject]@{
                lane = $betaPackageDefinitions[$index].LaneId
                file = $betaPackageDefinitions[$index].FileName
                length = $index + 10
                sha256 = $hash
            })
    }
    $betaManifestHash = ('6' * 64) -join ''
    $betaHashes.Add($betaManifestHash)
    $historyCatalog.milestones[1].qualification.state = 'pass'
    $historyCatalog.milestones[1].qualification.sourceCommit = $betaSourceCommit
    $historyCatalog.milestones[1].packages = $betaPackages.ToArray()
    $historyCatalog.milestones[1].checksumManifest = [PSCustomObject][ordered]@{
        file = 'SHA256SUMS.txt'
        length = 30
        sha256 = $betaManifestHash
    }
    Write-JsonDocument $historyCatalog $historyCatalogPath
    Write-ValidationFixture `
        -Path $betaValidationPath `
        -Qualification 'pass' `
        -Promotion 'unpromoted' `
        -SourceCommit $betaSourceCommit `
        -Hashes $betaHashes.ToArray() `
        -PublicationPolicy 'public-prerelease'
    Invoke-FixtureGit $historyRoot @('add', '--all')
    Invoke-FixtureGit $historyRoot @('commit', '-m', 'Fixture beta attestation E')
    Invoke-FixtureGit $historyRoot @(
        'tag',
        '-a',
        'v2.0.0-beta.1',
        '-m',
        'Fixture qualified beta checkpoint')
    $betaTagObject = ([string](& git -C $historyRoot rev-parse refs/tags/v2.0.0-beta.1)).Trim()
    if ($LASTEXITCODE -ne 0 -or $betaTagObject -cnotmatch '^[0-9a-f]{40}$') {
        throw 'Could not resolve fixture beta annotated-tag object.'
    }

    $betaReleaseUrl = 'https://github.com/Julesc013/compact-cassette-catalogue/releases/tag/v2.0.0-beta.1'
    $assetBaseUrl = 'https://github.com/Julesc013/compact-cassette-catalogue/releases/download/v2.0.0-beta.1/'
    $historyCatalog.milestones[1].promotion.state = 'tagged'
    $historyCatalog.milestones[1].promotion.tagObject = $betaTagObject
    $historyCatalog.milestones[1].publication.state = 'published'
    $historyCatalog.milestones[1].publication.releaseUrl = $betaReleaseUrl
    $historyCatalog.milestones[1].publication.feedPromoted = $true
    $historyCatalog.milestones[1].postVerification.state = 'passed'
    Write-JsonDocument $historyCatalog $historyCatalogPath
    Write-ValidationFixture `
        -Path $betaValidationPath `
        -Qualification 'pass' `
        -Promotion 'tagged' `
        -SourceCommit $betaSourceCommit `
        -Hashes $betaHashes.ToArray() `
        -PublicationPolicy 'public-prerelease' `
        -PublicationState 'published' `
        -PostVerification 'passed'

    $betaFeedRoot = Join-Path $historyRoot 'release\feeds\beta'
    [IO.Directory]::CreateDirectory($betaFeedRoot) | Out-Null
    $betaFeedPackages = @($betaPackages | ForEach-Object {
            [PSCustomObject][ordered]@{
                lane = $_.lane
                distribution = 'portable'
                file = $_.file
                length = $_.length
                sha256 = $_.sha256
                url = $assetBaseUrl + $_.file
            }
        })
    $betaFeed = [PSCustomObject][ordered]@{
        schemaVersion = 1
        product = 'Compact Cassette Catalogue'
        productId = 'c3'
        channel = 'beta'
        version = '2.0.0'
        stage = 'Beta 1'
        informationalVersion = '2.0.0-beta.1'
        releaseDate = '2026-08-04'
        catalogueWriteFormat = '1.1.0'
        published = $true
        releaseUrl = $betaReleaseUrl
        checksumManifest = [PSCustomObject][ordered]@{
            file = 'SHA256SUMS.txt'
            length = 30
            sha256 = $betaManifestHash
            url = $assetBaseUrl + 'SHA256SUMS.txt'
        }
        packages = $betaFeedPackages
    }
    $betaFeedPath = Join-Path $betaFeedRoot 'release.json'
    Write-JsonDocument $betaFeed $betaFeedPath
    Invoke-FixtureGit $historyRoot @('add', '--all')
    Invoke-FixtureGit $historyRoot @('commit', '-m', 'Fixture beta post-promotion P')
    Invoke-FixtureGit $historyRoot @('branch', '-f', 'dev/2.x', 'master')
    Invoke-FixtureGit $historyRoot @('push', '--quiet', '--force', 'origin', 'master', 'dev/2.x', '--tags')

    & $historyValidator -Mode Master | Out-Null
    $passed++

    $betaFeed.packages[0].sha256 = ('7' * 64) -join ''
    Write-JsonDocument $betaFeed $betaFeedPath
    Assert-FailsWith {
        & $historyValidator -Mode Master
    } 'differs from the qualified portable artifact' 'promoted feed artifact mismatch'

    $tagCheckoutRoot = Join-Path $testRoot 'tag-checkout'
    Invoke-FixtureGit $historyRoot @('clone', '--quiet', '--no-local', '.', $tagCheckoutRoot)
    Invoke-FixtureGit $tagCheckoutRoot @('checkout', '--quiet', 'v2.0.0-beta.1')
    & (Join-Path $tagCheckoutRoot 'build\validate-release-contract.ps1') `
        -Mode Tag `
        -TagName 'v2.0.0-beta.1' | Out-Null
    $passed++

    $supersessionRoot = Join-Path $testRoot 'supersession-checkout'
    Invoke-FixtureGit $historyRoot @('clone', '--quiet', '--no-local', '.', $supersessionRoot)
    Invoke-FixtureGit $supersessionRoot @('checkout', '--quiet', '-b', 'supersession-fixture', 'v2.0.0-beta.1')
    Invoke-FixtureGit $supersessionRoot @('config', 'user.name', 'C3 Release Contract Tests')
    Invoke-FixtureGit $supersessionRoot @('config', 'user.email', 'release-contract-tests@invalid.example')
    $supersessionCatalogPath = Join-Path $supersessionRoot 'release\catalog.v1.json'
    $supersessionCatalog = Get-Content -LiteralPath $supersessionCatalogPath -Raw | ConvertFrom-Json
    $supersessionCatalog.milestones[1].promotion.state = 'tagged'
    $supersessionCatalog.milestones[1].promotion.tagObject = $betaTagObject
    $supersessionCatalog.milestones[1].publication.state = 'published'
    $supersessionCatalog.milestones[1].publication.releaseUrl = $betaReleaseUrl
    $supersessionCatalog.milestones[1].postVerification.state = 'failed'
    Write-JsonDocument $supersessionCatalog $supersessionCatalogPath
    $supersessionBetaValidationPath = Join-Path $supersessionRoot 'release\validation\2.0.0-beta.1.md'
    Write-ValidationFixture `
        -Path $supersessionBetaValidationPath `
        -Qualification 'pass' `
        -Promotion 'tagged' `
        -SourceCommit $betaSourceCommit `
        -Hashes $betaHashes.ToArray() `
        -PublicationPolicy 'public-prerelease' `
        -PublicationState 'published' `
        -PostVerification 'failed'
    Invoke-FixtureGit $supersessionRoot @('add', '--all')
    Invoke-FixtureGit $supersessionRoot @('commit', '-m', 'Fixture failed beta post-operation P')

    $supersessionPropsPath = Join-Path $supersessionRoot 'build\Version.props'
    $supersessionPropsText = [IO.File]::ReadAllText($supersessionPropsPath).Replace(
        '<C3ReleaseStage>Beta 1</C3ReleaseStage>',
        '<C3ReleaseStage>Beta 2</C3ReleaseStage>')
    [IO.File]::WriteAllText($supersessionPropsPath, $supersessionPropsText, $utf8WithoutBom)
    $supersessionCatalog.milestones[1].supersededBy = '2.0.0-beta.2'
    $betaSuccessor = [PSCustomObject][ordered]@{
        releaseLabel = '2.0.0-beta.2'
        productVersion = '2.0.0'
        stage = 'Beta 2'
        channel = 'beta'
        predecessor = '2.0.0-beta.1'
        qualification = [PSCustomObject][ordered]@{
            state = 'blocked'
            sourceCommit = $null
        }
        supersededBy = $null
        promotion = [PSCustomObject][ordered]@{
            state = 'unpromoted'
            targetBranch = 'master'
            tag = 'v2.0.0-beta.2'
            tagObject = $null
        }
        publication = [PSCustomObject][ordered]@{
            policy = 'public-prerelease'
            state = 'unpublished'
            releaseUrl = $null
            feedPromoted = $false
        }
        postVerification = [PSCustomObject][ordered]@{
            state = 'not-applicable'
        }
        validationRecord = 'release/validation/2.0.0-beta.2.md'
        packages = @()
        checksumManifest = $null
    }
    $supersessionCatalog.milestones = @($supersessionCatalog.milestones) + @($betaSuccessor)
    Write-JsonDocument $supersessionCatalog $supersessionCatalogPath
    Write-ValidationFixture `
        -Path (Join-Path $supersessionRoot 'release\validation\2.0.0-beta.2.md') `
        -Qualification 'blocked' `
        -Promotion 'unpromoted' `
        -SourceCommit $null `
        -Hashes @() `
        -PublicationPolicy 'public-prerelease'
    Invoke-FixtureGit $supersessionRoot @('add', '--all')
    Invoke-FixtureGit $supersessionRoot @('commit', '-m', 'Fixture immediate beta successor C')
    & (Join-Path $supersessionRoot 'build\validate-release-contract.ps1') -Mode Repository | Out-Null
    $passed++

    $betaFeed.packages[0].sha256 = [string]$betaPackages[0].sha256
    Write-JsonDocument $betaFeed $betaFeedPath

    $feedPinRoot = Join-Path $testRoot 'feed-pin-checkout'
    Invoke-FixtureGit $historyRoot @('clone', '--quiet', '--no-local', '.', $feedPinRoot)
    Invoke-FixtureGit $feedPinRoot @('config', 'user.name', 'C3 Release Contract Tests')
    Invoke-FixtureGit $feedPinRoot @('config', 'user.email', 'release-contract-tests@invalid.example')
    $feedPinPropsPath = Join-Path $feedPinRoot 'build\Version.props'
    $feedPinPropsText = [IO.File]::ReadAllText($feedPinPropsPath).Replace(
        '<C3ReleaseStage>Beta 1</C3ReleaseStage>',
        '<C3ReleaseStage>Beta 2</C3ReleaseStage>')
    [IO.File]::WriteAllText($feedPinPropsPath, $feedPinPropsText, $utf8WithoutBom)
    $feedPinCatalogPath = Join-Path $feedPinRoot 'release\catalog.v1.json'
    $feedPinCatalog = Get-Content -LiteralPath $feedPinCatalogPath -Raw | ConvertFrom-Json
    $feedPinCatalog.milestones = @($feedPinCatalog.milestones) + @($betaSuccessor)
    Write-JsonDocument $feedPinCatalog $feedPinCatalogPath
    Write-ValidationFixture `
        -Path (Join-Path $feedPinRoot 'release\validation\2.0.0-beta.2.md') `
        -Qualification 'blocked' `
        -Promotion 'unpromoted' `
        -SourceCommit $null `
        -Hashes @() `
        -PublicationPolicy 'public-prerelease'
    Invoke-FixtureGit $feedPinRoot @('add', '--all')
    Invoke-FixtureGit $feedPinRoot @('commit', '-m', 'Fixture future beta source C')
    $feedPinPath = Join-Path $feedPinRoot 'release\feeds\beta\release.json'
    $feedPinOriginalBytes = [IO.File]::ReadAllBytes($feedPinPath)
    [IO.File]::AppendAllText($feedPinPath, ' ', $utf8WithoutBom)
    Assert-FailsWith {
        & (Join-Path $feedPinRoot 'build\validate-release-contract.ps1') -Mode Repository
    } 'latest promoted beta feed differs' 'future source rewrites latest promoted feed bytes'
    [IO.File]::WriteAllBytes($feedPinPath, $feedPinOriginalBytes)

    $omissionCatalog = Get-Content -LiteralPath $feedPinCatalogPath -Raw | ConvertFrom-Json
    $onlyCurrentMilestone = ($omissionCatalog.milestones | Where-Object {
            [string]$_.releaseLabel -ceq '2.0.0-beta.2'
        })
    $onlyCurrentMilestone.predecessor = $null
    $omissionCatalog.milestones = @($onlyCurrentMilestone)
    Write-JsonDocument $omissionCatalog $feedPinCatalogPath
    Assert-FailsWith {
        & (Join-Path $feedPinRoot 'build\validate-release-contract.ps1') `
            -Mode Repository `
            -RequireRemoteBaseline
    } "milestone '2.0.0-beta.1' is missing" 'current source omits finalized master milestone'

    $rewindRoot = Join-Path $testRoot 'rewind-checkout'
    Invoke-FixtureGit $historyRoot @('clone', '--quiet', '--no-local', '.', $rewindRoot)
    $rewindCatalogPath = Join-Path $rewindRoot 'release\catalog.v1.json'
    $rewindCatalog = Get-Content -LiteralPath $rewindCatalogPath -Raw | ConvertFrom-Json
    $rewindMilestone = $rewindCatalog.milestones[1]
    $rewindMilestone.promotion.state = 'unpromoted'
    $rewindMilestone.promotion.tagObject = $null
    $rewindMilestone.publication.state = 'unpublished'
    $rewindMilestone.publication.releaseUrl = $null
    $rewindMilestone.publication.feedPromoted = $false
    $rewindMilestone.postVerification.state = 'not-applicable'
    Write-JsonDocument $rewindCatalog $rewindCatalogPath
    Write-ValidationFixture `
        -Path (Join-Path $rewindRoot 'release\validation\2.0.0-beta.1.md') `
        -Qualification 'pass' `
        -Promotion 'unpromoted' `
        -SourceCommit $betaSourceCommit `
        -Hashes $betaHashes.ToArray() `
        -PublicationPolicy 'public-prerelease'
    Assert-FailsWith {
        & (Join-Path $rewindRoot 'build\validate-release-contract.ps1') `
            -Mode Repository `
            -RequireRemoteBaseline
    } 'rewritten outside the exact E-to-P transition' 'finalized current milestone rewound to E state'

    $remoteAdvanceRoot = Join-Path $testRoot 'remote-advance'
    Invoke-FixtureGit $testRoot @('clone', '--quiet', $historyRemoteRoot, $remoteAdvanceRoot)
    Invoke-FixtureGit $remoteAdvanceRoot @('config', 'user.name', 'C3 Release Contract Tests')
    Invoke-FixtureGit $remoteAdvanceRoot @('config', 'user.email', 'release-contract-tests@invalid.example')
    Invoke-FixtureGit $remoteAdvanceRoot @('commit', '--allow-empty', '-m', 'Advance remote fixture')
    Invoke-FixtureGit $remoteAdvanceRoot @('push', '--quiet', 'origin', 'master')
    Assert-FailsWith {
        & $historyValidator -Mode Master
    } 'requires HEAD at refs/remotes/origin/master commit' 'stale master event after origin/master advances'

    Invoke-FixtureGit $historyRoot @('push', '--quiet', '--force', 'origin', 'master:master')
    Invoke-FixtureGit $remoteAdvanceRoot @('push', '--quiet', '--force', 'origin', 'HEAD:refs/heads/dev/2.x')
    Assert-FailsWith {
        & $historyValidator -Mode Master
    } 'requires HEAD at origin/dev/2.x commit' 'master ledger with origin/dev/2.x not atomically advanced'
    Invoke-FixtureGit $historyRoot @('push', '--quiet', '--force', 'origin', 'master:dev/2.x')

    [IO.File]::AppendAllText(
        $historyValidationPath,
        "`r`nUnauthorized historical evidence rewrite.`r`n",
        $utf8WithoutBom)
    Invoke-FixtureGit $historyRoot @('add', '--', 'release/validation/2.0.0-alpha.1.md')
    Invoke-FixtureGit $historyRoot @('commit', '-m', 'Rewrite historical validation evidence')
    Assert-FailsWith {
        & $historyValidator -Mode Repository
    } 'differs from finalized post-operation snapshot' 'historical validation evidence rewrite'
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        Assert-SafeArtifactsPath $testRoot
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
}

Write-Host "Release-contract tests passed: $passed scenarios."
