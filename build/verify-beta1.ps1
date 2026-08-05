[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release',
    [switch]$SkipBuildOutputs,
    [string]$CandidateRoot,
    [string]$ToolchainLockPath,
    [switch]$VerifyIdentityTransition
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-contract.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
[void](Assert-C3Beta1ManifestPath -Path (Join-Path $PSScriptRoot 'lanes.json'))
$headCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw 'Beta 1 verification requires clean frozen C-beta.' }
if ((& git -C $repositoryRoot rev-parse refs/tags/v1.3.0a2).Trim() -cne '0e8633671e55bbb7ce5e692f8e0f5c4201a62627' -or
        (& git -C $repositoryRoot rev-parse 'v1.3.0a2^{commit}').Trim() -cne '0aad46c6ad1d241caa70ceb74ccfe9bbfa12165b') {
    throw 'Immutable Alpha 2 tag object or target changed.'
}
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0a3
if ($LASTEXITCODE -eq 0) { throw 'Superseded Alpha 3 must remain untagged before Beta 1.' }
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0b1
if ($LASTEXITCODE -eq 0) { throw 'Beta 1 source/Candidate verification requires absent v1.3.0b1.' }
if ((& git -C $repositoryRoot rev-parse legacy/1.x).Trim() -cne 'c4115b82ea43fdd763685d862a08fe5c61db6dff') {
    throw 'legacy/1.x moved before complete Beta GO.'
}

if ($VerifyIdentityTransition) {
    $parents = @((& git -C $repositoryRoot rev-list --parents -n 1 HEAD).Trim().Split(' '))
    if ($parents.Count -ne 2) { throw 'C-beta identity projection must be a single-parent metadata-only commit.' }
    $allowed = @(
        'CHANGELOG.md', 'README.md', 'RELEASE_NOTES.md', 'TODO.md',
        'Compact Cassette Catalogue/My Project/AssemblyInfo.vb', 'Compact Cassette Catalogue/varGlobals.vb',
        'Compact Cassette Catalogue Installer/My Project/AssemblyInfo.vb',
        'Compact Cassette Catalogue Uninstaller/My Project/AssemblyInfo.vb',
        'SetupShared/SetupBundleRuntime.vb', 'build/get-runtime-lanes.ps1', 'build/lanes.json',
        'build/package-content/README.txt', 'tests/C3.Setup.Characterization/Program.vb'
    )
    $required = @(
        'Compact Cassette Catalogue/My Project/AssemblyInfo.vb', 'Compact Cassette Catalogue/varGlobals.vb',
        'Compact Cassette Catalogue Installer/My Project/AssemblyInfo.vb',
        'Compact Cassette Catalogue Uninstaller/My Project/AssemblyInfo.vb',
        'SetupShared/SetupBundleRuntime.vb', 'build/get-runtime-lanes.ps1', 'build/lanes.json'
    )
    $changes = @(& git -C $repositoryRoot diff --name-only $parents[1] HEAD)
    if (@($changes | Where-Object { $allowed -notcontains $_ }).Count -ne 0 -or
            @($required | Where-Object { $changes -notcontains $_ }).Count -ne 0) {
        throw "C-beta identity transition is not the closed metadata-only projection: $($changes -join ', ')"
    }
}

& (Join-Path $PSScriptRoot 'verify-preparation.ps1') -Configuration $Configuration -SkipBuildOutputs:$SkipBuildOutputs
& (Join-Path $PSScriptRoot 'verify-release-identity.ps1') -ExpectedProductVersion '1.3.0' -ExpectedStage 'Beta 1' `
    -ExpectedReleaseLabel '1.3.0b1' -ExpectedTag 'v1.3.0b1' -ExpectedChannel beta -ExpectedDate ([datetime]'2026-08-06') `
    -Configuration $Configuration -VerifyBuildOutputs:(-not $SkipBuildOutputs)
& (Join-Path $PSScriptRoot 'test-beta1-assets.ps1')
& (Join-Path $PSScriptRoot 'test-beta1-verdict.ps1')
& (Join-Path $PSScriptRoot 'test-beta1-tag-message.ps1')

if (-not $SkipBuildOutputs) {
    if ([string]::IsNullOrWhiteSpace($CandidateRoot) -or [string]::IsNullOrWhiteSpace($ToolchainLockPath)) {
        throw 'Full Beta 1 Candidate verification requires -CandidateRoot and -ToolchainLockPath.'
    }
    & (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'verify-setup-builds.ps1') -Configuration $Configuration -ExpectedSourceCommit $headCommit
    & (Join-Path $PSScriptRoot 'verify-setup-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
    & (Join-Path $PSScriptRoot 'verify-beta1-assets.ps1') -CandidateRoot $CandidateRoot -ExpectedSourceCommit $headCommit `
        -ExpectedToolchainLockSha256 (Get-FileHash -LiteralPath $ToolchainLockPath -Algorithm SHA256).Hash.ToLowerInvariant()
    & (Join-Path $PSScriptRoot 'test-source-reproducibility.ps1') -Configuration $Configuration `
        -ToolchainLockPath $ToolchainLockPath -IncludeSetup
}

Write-Host 'C3 1.3.0 Beta 1 source/Candidate controls passed; this does not itself authorize a tag, ledger movement, or publication.'

