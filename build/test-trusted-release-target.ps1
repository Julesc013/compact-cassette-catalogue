param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts'))
$testRoot = [IO.Path]::GetFullPath((Join-Path $artifactsRoot `
            "trusted-release-target-tests-$PID"))
$utf8WithoutBom = New-Object Text.UTF8Encoding($false)
$releaseLabel = '2.0.0-alpha.1'
$tagName = "v$releaseLabel"
$passed = 0

function Assert-SafeTestPath {
    param([string]$Path)

    $artifactsPrefix = $artifactsRoot + [IO.Path]::DirectorySeparatorChar
    if (-not $Path.StartsWith(
            $artifactsPrefix,
            [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to modify a path outside artifacts: $Path"
    }
}

function Invoke-FixtureGit {
    param(
        [string]$Repository,
        [string[]]$Arguments
    )

    $savedErrorActionPreference = $ErrorActionPreference
    $output = @()
    $exitCode = 0
    try {
        $ErrorActionPreference = 'Continue'
        if ([string]::IsNullOrWhiteSpace($Repository)) {
            $output = @(& git @Arguments 2>&1)
        }
        else {
            $output = @(& git -C $Repository @Arguments 2>&1)
        }
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
    if ($exitCode -ne 0) {
        throw "Fixture Git failed ($($Arguments -join ' ')):`n$($output -join "`n")"
    }
    return (@($output | ForEach-Object { [string]$_ }) -join "`n").Trim()
}

function Write-Utf8Text {
    param(
        [string]$Path,
        [string]$Text
    )

    $parent = Split-Path -Parent $Path
    [IO.Directory]::CreateDirectory($parent) | Out-Null
    [IO.File]::WriteAllText($Path, $Text, $utf8WithoutBom)
}

function Write-Catalog {
    param(
        [string]$Repository,
        [object]$Catalog
    )

    $json = $Catalog | ConvertTo-Json -Depth 30
    Write-Utf8Text (Join-Path $Repository 'release\catalog.v1.json') `
        ($json + [Environment]::NewLine)
}

function New-Catalog {
    return [PSCustomObject][ordered]@{
        '$schema' = '../spec/release-catalog/v1/catalog.schema.json'
        schemaVersion = 1
        productId = 'c3'
        milestones = @(
            [PSCustomObject][ordered]@{
                releaseLabel = $releaseLabel
                productVersion = '2.0.0'
                stage = 'Alpha 1'
                channel = 'alpha'
                predecessor = $null
                qualification = [PSCustomObject][ordered]@{
                    state = 'blocked'
                    sourceCommit = $null
                }
                supersededBy = $null
                promotion = [PSCustomObject][ordered]@{
                    state = 'unpromoted'
                    targetBranch = 'master'
                    tag = $tagName
                    tagObject = $null
                }
                publication = [PSCustomObject][ordered]@{
                    policy = 'intentionally-unpublished'
                    state = 'unpublished'
                    releaseUrl = $null
                    feedPromoted = $false
                }
                postVerification = [PSCustomObject][ordered]@{
                    state = 'not-applicable'
                }
                validationRecord = "release/validation/$releaseLabel.md"
                packages = @()
                checksumManifest = $null
            }
        )
    }
}

function Assert-GuardPasses {
    param(
        [string]$TrustedRepository,
        [string]$TargetRepository,
        [string]$Mode,
        [string]$ExpectedCommit,
        [string]$ExpectedMaster,
        [string]$AttestationRef,
        [string]$Name
    )

    & (Join-Path $TrustedRepository 'build\validate-trusted-release-target.ps1') `
        -Mode $Mode `
        -TrustedRepository $TrustedRepository `
        -TargetRepository $TargetRepository `
        -ExpectedCommit $ExpectedCommit `
        -ExpectedTrustedMasterCommit $ExpectedMaster `
        -AttestationRef $AttestationRef *> $null
    $script:passed++
    Write-Host "PASS: $Name"
}

function Assert-GuardFails {
    param(
        [scriptblock]$Action,
        [string]$Name
    )

    $failed = $false
    try {
        & $Action *> $null
    }
    catch {
        $failed = $true
    }
    if (-not $failed) {
        throw "Expected trusted release-target guard failure: $Name"
    }
    $script:passed++
    Write-Host "PASS: $Name"
}

Assert-SafeTestPath $testRoot
if (Test-Path -LiteralPath $testRoot) {
    Remove-Item -LiteralPath $testRoot -Recurse -Force
}
[IO.Directory]::CreateDirectory($testRoot) | Out-Null

try {
    $origin = Join-Path $testRoot 'origin.git'
    $seed = Join-Path $testRoot 'seed'
    $trusted = Join-Path $testRoot 'trusted'
    $target = Join-Path $testRoot 'target'

    Invoke-FixtureGit '' @('init', '--bare', $origin) | Out-Null
    Invoke-FixtureGit '' @('init', $seed) | Out-Null
    Invoke-FixtureGit $seed @('config', 'user.name', 'C3 Contract Tests') | Out-Null
    Invoke-FixtureGit $seed @('config', 'user.email', 'c3-tests@example.invalid') | Out-Null
    Invoke-FixtureGit $seed @('config', 'commit.gpgsign', 'false') | Out-Null
    Invoke-FixtureGit $seed @('config', 'tag.gpgsign', 'false') | Out-Null

    foreach ($relativePath in @(
            'build\validate-trusted-release-target.ps1',
            'build\validate-json-document.ps1',
            'spec\release-catalog\v1\catalog.schema.json')) {
        $sourcePath = Join-Path $repositoryRoot $relativePath
        $destinationPath = Join-Path $seed $relativePath
        [IO.Directory]::CreateDirectory((Split-Path -Parent $destinationPath)) |
            Out-Null
        [IO.File]::Copy($sourcePath, $destinationPath, $true)
    }

    $catalog = New-Catalog
    Write-Catalog $seed $catalog
    Write-Utf8Text (Join-Path $seed "release\validation\$releaseLabel.md") `
        "Planned fixture.`n"
    Write-Utf8Text (Join-Path $seed 'source.txt') "Candidate source.`n"
    Invoke-FixtureGit $seed @('add', '--all') | Out-Null
    Invoke-FixtureGit $seed @('commit', '-m', 'fixture: source C') | Out-Null
    Invoke-FixtureGit $seed @('branch', '-M', 'master') | Out-Null
    $sourceCommit = Invoke-FixtureGit $seed @('rev-parse', 'HEAD')
    Invoke-FixtureGit $seed @('remote', 'add', 'origin', $origin) | Out-Null
    Invoke-FixtureGit $seed @('push', 'origin', 'master') | Out-Null
    Invoke-FixtureGit $seed @('push', 'origin', 'HEAD:refs/heads/dev') | Out-Null

    Invoke-FixtureGit '' @('clone', '--branch', 'master', $origin, $trusted) |
        Out-Null
    Invoke-FixtureGit '' @('clone', '--branch', 'master', $origin, $target) |
        Out-Null

    $catalog.milestones[0].qualification.state = 'pass'
    $catalog.milestones[0].qualification.sourceCommit = $sourceCommit
    Write-Catalog $seed $catalog
    Write-Utf8Text (Join-Path $seed "release\validation\$releaseLabel.md") `
        "Qualification: pass`nSource commit: $sourceCommit`n"
    Invoke-FixtureGit $seed @('add', '--all') | Out-Null
    Invoke-FixtureGit $seed @('commit', '-m', 'test(release): attest E') | Out-Null
    $candidateCommit = Invoke-FixtureGit $seed @('rev-parse', 'HEAD')
    $candidateRef = "attest/v$releaseLabel-candidate-$candidateCommit"
    Invoke-FixtureGit $seed @('push', 'origin',
        "HEAD:refs/heads/$candidateRef") | Out-Null
    Invoke-FixtureGit $target @('fetch', '--tags', 'origin',
        "refs/heads/$candidateRef") | Out-Null
    Invoke-FixtureGit $target @('checkout', '--detach', $candidateCommit) | Out-Null

    Assert-GuardPasses $trusted $target 'Candidate' $candidateCommit `
        $sourceCommit $candidateRef `
        'candidate accepts exact C-to-E evidence and SHA-bound transport'

    Invoke-FixtureGit $seed @('checkout', '--detach', $sourceCommit) | Out-Null
    $invalidCandidateCatalog = $catalog | ConvertTo-Json -Depth 30 |
        ConvertFrom-Json
    $invalidCandidateCatalog.milestones[0].promotion.tagObject = $sourceCommit
    Write-Catalog $seed $invalidCandidateCatalog
    Write-Utf8Text (Join-Path $seed "release\validation\$releaseLabel.md") `
        "Qualification: pass with fabricated tag object`nSource commit: $sourceCommit`n"
    Invoke-FixtureGit $seed @('add', '--all') | Out-Null
    Invoke-FixtureGit $seed @('commit', '-m',
        'test(release): invalid E tag object') | Out-Null
    $invalidCandidateCommit = Invoke-FixtureGit $seed @('rev-parse', 'HEAD')
    $invalidCandidateRef =
        "attest/v$releaseLabel-candidate-$invalidCandidateCommit"
    Invoke-FixtureGit $seed @('push', 'origin',
        "HEAD:refs/heads/$invalidCandidateRef") | Out-Null
    Invoke-FixtureGit $target @('fetch', 'origin',
        "refs/heads/$invalidCandidateRef") | Out-Null
    Invoke-FixtureGit $target @('checkout', '--detach', $invalidCandidateCommit) |
        Out-Null
    Assert-GuardFails {
        & (Join-Path $trusted 'build\validate-trusted-release-target.ps1') `
            -Mode Candidate `
            -TrustedRepository $trusted `
            -TargetRepository $target `
            -ExpectedCommit $invalidCandidateCommit `
            -ExpectedTrustedMasterCommit $sourceCommit `
            -AttestationRef $invalidCandidateRef
    } 'candidate rejects a fabricated pre-promotion tag object'

    Invoke-FixtureGit $seed @('checkout', '--detach', $candidateCommit) | Out-Null
    Invoke-FixtureGit $target @('checkout', '--detach', $candidateCommit) | Out-Null

    Invoke-FixtureGit $seed @('push', 'origin',
        "$candidateCommit`:refs/heads/dev") | Out-Null
    Assert-GuardFails {
        & (Join-Path $trusted 'build\validate-trusted-release-target.ps1') `
            -Mode Candidate `
            -TrustedRepository $trusted `
            -TargetRepository $target `
            -ExpectedCommit $candidateCommit `
            -ExpectedTrustedMasterCommit $sourceCommit `
            -AttestationRef $candidateRef
    } 'candidate rejects dev moving from C to E'
    Invoke-FixtureGit $seed @('push', '--force', 'origin',
        "$sourceCommit`:refs/heads/dev") | Out-Null

    Invoke-FixtureGit $seed @('push', 'origin',
        "$candidateCommit`:refs/heads/master") | Out-Null
    Invoke-FixtureGit $seed @('push', 'origin',
        "$candidateCommit`:refs/heads/dev") | Out-Null
    Invoke-FixtureGit $seed @('tag', '-a', $tagName, '-m',
        "Qualified $tagName", $candidateCommit) | Out-Null
    $tagObject = Invoke-FixtureGit $seed @('rev-parse', $tagName)
    Invoke-FixtureGit $seed @('push', 'origin', "refs/tags/$tagName") | Out-Null

    Invoke-FixtureGit $trusted @('fetch', '--tags', 'origin') | Out-Null
    Invoke-FixtureGit $trusted @('checkout', '--detach', $candidateCommit) | Out-Null

    $catalog.milestones[0].promotion.state = 'tagged'
    $catalog.milestones[0].promotion.tagObject = $tagObject
    Write-Catalog $seed $catalog
    Write-Utf8Text (Join-Path $seed "release\validation\$releaseLabel.md") `
        "Qualification: pass`nPromotion: tagged`nSource commit: $sourceCommit`n"
    Invoke-FixtureGit $seed @('add', '--all') | Out-Null
    Invoke-FixtureGit $seed @('commit', '-m', 'test(release): attest P') | Out-Null
    $postCommit = Invoke-FixtureGit $seed @('rev-parse', 'HEAD')
    $postRef = "attest/v$releaseLabel-post-$postCommit"
    Invoke-FixtureGit $seed @('push', 'origin',
        "HEAD:refs/heads/$postRef") | Out-Null
    Invoke-FixtureGit $target @('fetch', '--tags', 'origin',
        "refs/heads/$postRef") | Out-Null
    Invoke-FixtureGit $target @('checkout', '--detach', $postCommit) | Out-Null

    Assert-GuardPasses $trusted $target 'PostPromotion' $postCommit `
        $candidateCommit $postRef `
        'post-promotion accepts exact E-to-P evidence and SHA-bound transport'

    Invoke-FixtureGit $seed @('tag', '--force', '-a', $tagName, '-m',
        "Replacement $tagName", $candidateCommit) | Out-Null
    Invoke-FixtureGit $seed @('push', '--force', 'origin',
        "refs/tags/$tagName") | Out-Null
    Assert-GuardFails {
        & (Join-Path $trusted 'build\validate-trusted-release-target.ps1') `
            -Mode PostPromotion `
            -TrustedRepository $trusted `
            -TargetRepository $target `
            -ExpectedCommit $postCommit `
            -ExpectedTrustedMasterCommit $candidateCommit `
            -AttestationRef $postRef
    } 'post-promotion rejects a replaced remote tag object at the same E'
    Invoke-FixtureGit $seed @('update-ref', "refs/tags/$tagName", $tagObject) |
        Out-Null
    Invoke-FixtureGit $seed @('push', '--force', 'origin',
        "refs/tags/$tagName") | Out-Null

    Invoke-FixtureGit $seed @('checkout', '--detach', $candidateCommit) | Out-Null
    Write-Catalog $seed $catalog
    Write-Utf8Text (Join-Path $seed "release\validation\$releaseLabel.md") `
        "Qualification: pass`nPromotion: tagged with unexpected path`nSource commit: $sourceCommit`n"
    Write-Utf8Text (Join-Path $seed 'unexpected.txt') "Not evidence.`n"
    Invoke-FixtureGit $seed @('add', '--all') | Out-Null
    Invoke-FixtureGit $seed @('commit', '-m', 'test(release): invalid P paths') |
        Out-Null
    $invalidPostCommit = Invoke-FixtureGit $seed @('rev-parse', 'HEAD')
    $invalidPostRef = "attest/v$releaseLabel-post-$invalidPostCommit"
    Invoke-FixtureGit $seed @('push', 'origin',
        "HEAD:refs/heads/$invalidPostRef") | Out-Null
    Invoke-FixtureGit $target @('fetch', '--tags', 'origin',
        "refs/heads/$invalidPostRef") | Out-Null
    Invoke-FixtureGit $target @('checkout', '--detach', $invalidPostCommit) |
        Out-Null
    Assert-GuardFails {
        & (Join-Path $trusted 'build\validate-trusted-release-target.ps1') `
            -Mode PostPromotion `
            -TrustedRepository $trusted `
            -TargetRepository $target `
            -ExpectedCommit $invalidPostCommit `
            -ExpectedTrustedMasterCommit $candidateCommit `
            -AttestationRef $invalidPostRef
    } 'post-promotion rejects any path outside its exact evidence set'

    Write-Host "Trusted release-target guard tests passed: $passed"
}
finally {
    Assert-SafeTestPath $testRoot
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
}
