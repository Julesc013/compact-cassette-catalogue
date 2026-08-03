[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$fixtureRoot = Join-Path $repositoryRoot (
    'artifacts\release-ref-transaction-tests-' + [Diagnostics.Process]::GetCurrentProcess().Id)
$workRoot = Join-Path $fixtureRoot 'work'
$remoteRoot = Join-Path $fixtureRoot 'remote.git'
$transactionScript = Join-Path $PSScriptRoot 'invoke-release-ref-transaction.ps1'
$releaseLabel = '2.0.0-alpha.1'
$tagName = 'v' + $releaseLabel
$passed = 0

function Invoke-FixtureGit {
    param(
        [string]$WorkingDirectory,
        [string[]]$Arguments
    )

    $savedErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        $output = @(& git -C $WorkingDirectory @Arguments 2>&1)
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
    if ($exitCode -ne 0) {
        throw "Fixture git $($Arguments -join ' ') failed:`n$($output -join "`n")"
    }
    return $output
}

function Get-RemoteObject {
    param([string]$Reference)

    $line = @(& git -C $workRoot ls-remote origin $Reference)
    if ($LASTEXITCODE -ne 0) {
        throw "Could not inspect fixture remote reference $Reference."
    }
    if ($line.Count -eq 0) {
        return $null
    }
    if ($line.Count -ne 1) {
        throw "Fixture remote reference $Reference was ambiguous."
    }
    return (@(([string]$line[0]).Trim() -split '\s+'))[0]
}

function Assert-Equal {
    param(
        [object]$Expected,
        [object]$Actual,
        [string]$Scenario
    )

    if ($Expected -cne $Actual) {
        throw "$Scenario`: expected '$Expected', found '$Actual'."
    }
    $script:passed++
}

function Assert-TransactionFails {
    param(
        [hashtable]$Arguments,
        [string]$Scenario
    )

    $failed = $false
    try {
        & $transactionScript @Arguments -Confirm:$false 2>$null | Out-Null
    }
    catch {
        $failed = $true
    }
    if (-not $failed) {
        throw "$Scenario`: expected the transaction to fail."
    }
    $script:passed++
}

if (Test-Path -LiteralPath $fixtureRoot) {
    $resolvedArtifacts = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts'))
    $resolvedFixture = [IO.Path]::GetFullPath($fixtureRoot)
    if (-not $resolvedFixture.StartsWith(
            $resolvedArtifacts + [IO.Path]::DirectorySeparatorChar,
            [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to remove fixture path outside artifacts: $resolvedFixture"
    }
    Remove-Item -LiteralPath $resolvedFixture -Recurse -Force
}

try {
    New-Item -ItemType Directory -Path $fixtureRoot | Out-Null
    [void](Invoke-FixtureGit $fixtureRoot @('init', '--bare', $remoteRoot))
    [void](Invoke-FixtureGit $fixtureRoot @('init', $workRoot))
    [void](Invoke-FixtureGit $workRoot @('config', 'user.name', 'C3 Contract Test'))
    [void](Invoke-FixtureGit $workRoot @('config', 'user.email', 'contract@example.invalid'))
    [void](Invoke-FixtureGit $workRoot @('remote', 'add', 'origin', $remoteRoot))

    $fixtureBuildRoot = Join-Path $workRoot 'build'
    $fixtureSchemaRoot = Join-Path $workRoot 'spec\release-catalog\v1'
    $fixtureReleaseRoot = Join-Path $workRoot 'release'
    foreach ($directory in @($fixtureBuildRoot, $fixtureSchemaRoot, $fixtureReleaseRoot)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }
    Copy-Item `
        -LiteralPath (Join-Path $repositoryRoot 'build\validate-json-document.ps1') `
        -Destination (Join-Path $fixtureBuildRoot 'validate-json-document.ps1')
    Copy-Item `
        -LiteralPath (Join-Path $repositoryRoot 'spec\release-catalog\v1\catalog.schema.json') `
        -Destination (Join-Path $fixtureSchemaRoot 'catalog.schema.json')
    Copy-Item `
        -LiteralPath (Join-Path $repositoryRoot 'release\catalog.v1.json') `
        -Destination (Join-Path $fixtureReleaseRoot 'catalog.v1.json')

    Set-Content -LiteralPath (Join-Path $workRoot 'payload.txt') -Value 'master baseline'
    [void](Invoke-FixtureGit $workRoot @('add', '--all'))
    [void](Invoke-FixtureGit $workRoot @('commit', '-m', 'Fixture master baseline'))
    [void](Invoke-FixtureGit $workRoot @('branch', '-M', 'master'))
    $masterCommit = ([string](@(Invoke-FixtureGit $workRoot @('rev-parse', 'HEAD'))[-1])).Trim()
    [void](Invoke-FixtureGit $workRoot @('push', 'origin', 'master:master'))

    [void](Invoke-FixtureGit $workRoot @('checkout', '-b', 'dev'))
    Set-Content -LiteralPath (Join-Path $workRoot 'payload.txt') -Value 'frozen C'
    [void](Invoke-FixtureGit $workRoot @('add', 'payload.txt'))
    [void](Invoke-FixtureGit $workRoot @('commit', '-m', 'Fixture payload C'))
    $sourceCommit = ([string](@(Invoke-FixtureGit $workRoot @('rev-parse', 'HEAD'))[-1])).Trim()
    [void](Invoke-FixtureGit $workRoot @('push', 'origin', 'dev:dev'))

    Set-Content -LiteralPath (Join-Path $workRoot 'evidence.txt') -Value 'qualification E'
    [void](Invoke-FixtureGit $workRoot @('add', 'evidence.txt'))
    [void](Invoke-FixtureGit $workRoot @('commit', '-m', 'Fixture qualification E'))
    $eCommit = ([string](@(Invoke-FixtureGit $workRoot @('rev-parse', 'HEAD'))[-1])).Trim()
    $candidateRef = "refs/heads/attest/$tagName-candidate-$eCommit"

    $candidateArguments = @{
        Mode = 'CreateCandidate'
        ReleaseLabel = $releaseLabel
        ExpectedCommit = $eCommit
        ExpectedMasterCommit = $masterCommit
        ExpectedDevCommit = $sourceCommit
        RepositoryRoot = $workRoot
    }
    & $transactionScript @candidateArguments -Confirm:$false | Out-Null
    Assert-Equal $eCommit (Get-RemoteObject $candidateRef) 'candidate ref is exact E'
    Assert-TransactionFails $candidateArguments 'candidate ref is create-only'

    [void](Invoke-FixtureGit $workRoot @(
            'tag', '-a', $tagName, $eCommit, '-m', 'Fixture qualified checkpoint'))
    $candidateArguments.Mode = 'PromoteCandidate'
    & $transactionScript @candidateArguments -Confirm:$false | Out-Null
    Assert-Equal $eCommit (Get-RemoteObject 'refs/heads/master') 'candidate promotion advances master'
    Assert-Equal $eCommit (Get-RemoteObject 'refs/heads/dev') 'candidate promotion advances dev'
    Assert-Equal $null (Get-RemoteObject $candidateRef) 'candidate promotion consumes transport ref'
    Assert-Equal $eCommit (Get-RemoteObject "refs/tags/$tagName^{}") 'candidate promotion creates annotated tag'

    Set-Content -LiteralPath (Join-Path $workRoot 'evidence.txt') -Value 'post-operation P'
    $fixtureCatalogPath = Join-Path $fixtureReleaseRoot 'catalog.v1.json'
    $fixtureCatalog = Get-Content -LiteralPath $fixtureCatalogPath -Raw | ConvertFrom-Json
    $fixtureCatalog.milestones[0].promotion.state = 'tagged'
    $fixtureCatalog.milestones[0].promotion.tagObject =
        ([string](@(Invoke-FixtureGit $workRoot @('rev-parse', "refs/tags/$tagName"))[-1])).Trim()
    $fixtureCatalogText = $fixtureCatalog | ConvertTo-Json -Depth 10
    [IO.File]::WriteAllText(
        $fixtureCatalogPath,
        $fixtureCatalogText + "`n",
        (New-Object Text.UTF8Encoding($false)))
    [void](Invoke-FixtureGit $workRoot @('add', 'evidence.txt', 'release/catalog.v1.json'))
    [void](Invoke-FixtureGit $workRoot @('commit', '-m', 'Fixture post-operation P'))
    $pCommit = ([string](@(Invoke-FixtureGit $workRoot @('rev-parse', 'HEAD'))[-1])).Trim()
    $postRef = "refs/heads/attest/$tagName-post-$pCommit"

    $postArguments = @{
        Mode = 'CreatePost'
        ReleaseLabel = $releaseLabel
        ExpectedCommit = $pCommit
        ExpectedMasterCommit = $eCommit
        ExpectedDevCommit = $eCommit
        RepositoryRoot = $workRoot
    }
    & $transactionScript @postArguments -Confirm:$false | Out-Null
    Assert-Equal $pCommit (Get-RemoteObject $postRef) 'post ref is exact P'
    Assert-TransactionFails $postArguments 'post ref is create-only'

    [void](Invoke-FixtureGit $workRoot @('push', 'origin', "${pCommit}:dev"))
    $postArguments.Mode = 'PromotePost'
    Assert-TransactionFails $postArguments 'stale dev lease blocks post promotion'
    [void](Invoke-FixtureGit $workRoot @('push', '--force', 'origin', "${eCommit}:dev"))

    & $transactionScript @postArguments -Confirm:$false | Out-Null
    Assert-Equal $pCommit (Get-RemoteObject 'refs/heads/master') 'post promotion advances master'
    Assert-Equal $pCommit (Get-RemoteObject 'refs/heads/dev') 'post promotion advances dev'
    Assert-Equal $null (Get-RemoteObject $postRef) 'post promotion consumes transport ref'
    Assert-Equal $eCommit (Get-RemoteObject "refs/tags/$tagName^{}") 'post promotion preserves tag target'
}
finally {
    if (Test-Path -LiteralPath $fixtureRoot) {
        Remove-Item -LiteralPath $fixtureRoot -Recurse -Force
    }
}

Write-Host "Release-reference transaction tests passed: $passed scenarios."
