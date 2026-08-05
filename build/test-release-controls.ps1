[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Assert-Failure {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][scriptblock]$Action,
        [Parameter(Mandatory = $true)][string]$ExpectedPattern
    )
    try {
        & $Action
        throw "$Name unexpectedly passed."
    }
    catch {
        if ($_.Exception.Message -notmatch $ExpectedPattern) {
            throw
        }
        Write-Host "PASS $Name`: $($_.Exception.Message)"
    }
}

function Write-JsonFile {
    param(
        [Parameter(Mandatory = $true)]$Value,
        [Parameter(Mandatory = $true)][string]$Path
    )
    $json = ($Value | ConvertTo-Json -Depth 10) + "`n"
    [IO.File]::WriteAllText($Path, $json, (New-Object Text.UTF8Encoding($false)))
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot 'servicing-version.ps1')
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$remoteCommit = (& git -C $repositoryRoot rev-parse refs/remotes/origin/dev/1.x).Trim()
if ($LASTEXITCODE -ne 0 -or $sourceCommit -cne $remoteCommit) {
    throw "Release-control tests require HEAD '$sourceCommit' to equal refs/remotes/origin/dev/1.x '$remoteCommit'."
}
$sourceStatus = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
if ($sourceStatus.Count -ne 0) {
    throw "Release-control tests require a clean source tree:`n$($sourceStatus -join "`n")"
}

$testParent = Join-Path ([IO.Path]::GetTempPath()) 'c3-release-control-tests'
$testRoot = Join-Path $testParent ([Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testRoot -Force | Out-Null
try {
    $manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
    $fixtureLanes = @($manifest.lanes | ForEach-Object {
        $evidencePath = Join-Path $repositoryRoot "artifacts\evidence\build\$($_.id)\$Configuration\toolchain.json"
        $evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
        if ([string]$evidence.source.commit -cne $sourceCommit) {
            throw "$($_.id) build evidence is stale; rebuild Preparation evidence before release-control tests."
        }
        $resourceTool = @($evidence.resourceTools)[0]
        [ordered]@{
            id = [string]$_.id
            # This synthetic lock reaches source-control preflight only. An actual build
            # independently resolves and rejects any product-version mismatch.
            visualStudioProductVersion = [string]$_.initialServicingPin
            visualStudioInstallationVersion = [string]$evidence.visualStudio.installationVersion
            msbuildSha256 = [string]$evidence.msbuild.sha256
            vbcSha256 = [string]$evidence.compiler.sha256
            referenceAssemblySetSha256 = [string]$evidence.referenceAssemblies.setSha256
            resourceToolPath = [string]$resourceTool.path
            resourceToolSha256 = [string]$resourceTool.sha256
        }
    })
    $fixtureLock = [ordered]@{
        schemaVersion = 3
        purpose = 'synthetic adversarial preflight fixture; not candidate authority'
        status = 'locked'
        sourceCommit = $sourceCommit
        expectedRemoteRef = 'refs/remotes/origin/dev/1.x'
        providerRefReceipt = [ordered]@{
            remoteName = 'origin'
            remoteUrl = 'https://github.com/Julesc013/compact-cassette-catalogue.git'
            providerRef = 'refs/heads/dev/1.x'
            remoteTrackingRef = 'refs/remotes/origin/dev/1.x'
            fetchedCommit = $sourceCommit
            fetchedAtUtc = [DateTime]::UtcNow.ToString('o')
        }
        frozenAtUtc = [DateTime]::UtcNow.ToString('o')
        lanes = $fixtureLanes
    }
    $fixtureLockPath = Join-Path $testRoot 'preflight-fixture.json'
    Write-JsonFile $fixtureLock $fixtureLockPath

    Assert-Failure 'tracked self-referential lock design rejected' {
        & (Join-Path $PSScriptRoot 'build.ps1') -ToolchainMode Candidate -ToolchainLockPath (Join-Path $PSScriptRoot 'toolchain-lock.json') -PreflightOnly
    } 'external to the clean frozen source checkout'

    $wrongSourceLock = Get-Content -LiteralPath $fixtureLockPath -Raw | ConvertFrom-Json
    $wrongSourceLock.sourceCommit = '0000000000000000000000000000000000000000'
    $wrongSourceLock.providerRefReceipt.fetchedCommit = '0000000000000000000000000000000000000000'
    $wrongSourceLockPath = Join-Path $testRoot 'wrong-source.json'
    Write-JsonFile $wrongSourceLock $wrongSourceLockPath
    Assert-Failure 'external lock bound to wrong source rejected' {
        & (Join-Path $PSScriptRoot 'build.ps1') -ToolchainMode Candidate -ToolchainLockPath $wrongSourceLockPath -PreflightOnly
    } 'does not match frozen source HEAD'

    $trackedTestPath = Join-Path $repositoryRoot 'README.md'
    $trackedBytes = [IO.File]::ReadAllBytes($trackedTestPath)
    try {
        [IO.File]::WriteAllBytes($trackedTestPath, $trackedBytes + [byte[]](10))
        Assert-Failure 'dirty tracked file rejected' {
            & (Join-Path $PSScriptRoot 'build.ps1') -ToolchainMode Candidate -ToolchainLockPath $fixtureLockPath -PreflightOnly
        } 'Candidate source must be clean before compilation'
    }
    finally {
        [IO.File]::WriteAllBytes($trackedTestPath, $trackedBytes)
    }

    $untrackedTestPath = Join-Path $repositoryRoot 'C3.release-control-untracked.test'
    try {
        [IO.File]::WriteAllText($untrackedTestPath, 'adversarial test')
        Assert-Failure 'untracked production-adjacent file rejected' {
            & (Join-Path $PSScriptRoot 'build.ps1') -ToolchainMode Candidate -ToolchainLockPath $fixtureLockPath -PreflightOnly
        } 'Candidate source must be clean before compilation'
    }
    finally {
        if (Test-Path -LiteralPath $untrackedTestPath) {
            Remove-Item -LiteralPath $untrackedTestPath -Force
        }
    }

    $packageDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)"
    $packageEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)"
    $x64Lane = @($manifest.lanes | Where-Object { [string]$_.id -ceq 'win-x64-net48' })[0]
    $x86Lane = @($manifest.lanes | Where-Object { [string]$_.id -ceq 'win-x86-net40' })[0]
    $x64Package = Join-Path $packageDirectory ([string]$x64Lane.packageName)
    $x86Package = Join-Path $packageDirectory ([string]$x86Lane.packageName)
    $x64Manifest = Join-Path $packageEvidenceDirectory "$($x64Lane.packageName).entries.json"
    $x64PackageHash = (Get-FileHash -LiteralPath $x64Package -Algorithm SHA256).Hash.ToLowerInvariant()
    $x64ManifestHash = (Get-FileHash -LiteralPath $x64Manifest -Algorithm SHA256).Hash.ToLowerInvariant()

    $alteredExe = Join-Path $testRoot 'altered-exe'
    Expand-Archive -LiteralPath $x64Package -DestinationPath $alteredExe
    $alteredExePath = Join-Path $alteredExe 'Compact Cassette Catalogue.exe'
    $alteredExeBytes = [IO.File]::ReadAllBytes($alteredExePath)
    $alteredExeBytes[0] = $alteredExeBytes[0] -bxor 1
    [IO.File]::WriteAllBytes($alteredExePath, $alteredExeBytes)
    Assert-Failure 'altered extracted EXE rejected' {
        & (Join-Path $PSScriptRoot 'verify-target-runtime.ps1') -Lane win-x64-net48 -PackagePath $x64Package -ExtractedDirectory $alteredExe -ExpectedPackageSha256 $x64PackageHash -EntryManifestPath $x64Manifest -ExpectedEntryManifestSha256 $x64ManifestHash -Operator adversarial-test
    } 'Extracted file SHA-256 does not match retained entry manifest'

    $alteredBuild = Join-Path $testRoot 'altered-build'
    Expand-Archive -LiteralPath $x64Package -DestinationPath $alteredBuild
    $alteredBuildPath = Join-Path $alteredBuild 'BUILD.txt'
    $alteredBuildBytes = [IO.File]::ReadAllBytes($alteredBuildPath)
    $alteredBuildBytes[0] = $alteredBuildBytes[0] -bxor 1
    [IO.File]::WriteAllBytes($alteredBuildPath, $alteredBuildBytes)
    Assert-Failure 'altered extracted BUILD.txt rejected' {
        & (Join-Path $PSScriptRoot 'verify-target-runtime.ps1') -Lane win-x64-net48 -PackagePath $x64Package -ExtractedDirectory $alteredBuild -ExpectedPackageSha256 $x64PackageHash -EntryManifestPath $x64Manifest -ExpectedEntryManifestSha256 $x64ManifestHash -Operator adversarial-test
    } 'Extracted file SHA-256 does not match retained entry manifest'

    $unrelatedExtraction = Join-Path $testRoot 'unrelated-extraction'
    Expand-Archive -LiteralPath $x86Package -DestinationPath $unrelatedExtraction
    Assert-Failure 'correct ZIP plus unrelated extraction rejected' {
        & (Join-Path $PSScriptRoot 'verify-target-runtime.ps1') -Lane win-x64-net48 -PackagePath $x64Package -ExtractedDirectory $unrelatedExtraction -ExpectedPackageSha256 $x64PackageHash -EntryManifestPath $x64Manifest -ExpectedEntryManifestSha256 $x64ManifestHash -Operator adversarial-test
    } 'Extracted file (name/size|SHA-256) does not match retained entry manifest'

    Assert-Failure 'spoofed target-environment label rejected' {
        & (Join-Path $PSScriptRoot 'verify-target-runtime.ps1') -TargetEnvironmentId 'spoofed-target'
    } 'Caller-supplied -TargetEnvironmentId is prohibited'

    & (Join-Path $PSScriptRoot 'test-target-tooling-ps2.ps1')

    Assert-Failure 'stale servicing baseline rejected before freeze' {
        [void](Assert-C3VisualStudioServicingFloor `
                -ProductVersion '17.14.36' `
                -MinimumVersion '17.14.37' `
                -Context 'synthetic VS2022 fixture')
    } 'older than decision-date servicing floor'
    [void](Assert-C3VisualStudioServicingFloor `
            -ProductVersion '17.14.37' `
            -MinimumVersion '17.14.37' `
            -Context 'synthetic exact-floor fixture')
    [void](Assert-C3VisualStudioServicingFloor `
            -ProductVersion '17.14.38+build.1' `
            -MinimumVersion '17.14.37' `
            -Context 'synthetic later-servicing fixture')
    Assert-Failure 'malformed servicing version rejected before freeze' {
        [void](Assert-C3VisualStudioServicingFloor `
                -ProductVersion 'evergreen' `
                -MinimumVersion '17.14.37' `
                -Context 'synthetic malformed fixture')
    } 'not a parseable servicing version'

    if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) {
        throw 'Adversarial tests did not restore the clean source tree.'
    }
    Write-Host 'All release-control adversarial failures passed and the source tree was restored clean.'
}
finally {
    $resolvedTestRoot = [IO.Path]::GetFullPath($testRoot)
    $allowedPrefix = [IO.Path]::GetFullPath($testParent).TrimEnd('\') + '\'
    if ($resolvedTestRoot.StartsWith($allowedPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        if (Test-Path -LiteralPath $resolvedTestRoot) {
            Remove-Item -LiteralPath $resolvedTestRoot -Recurse -Force
        }
    }
    else {
        throw "Refusing to remove unexpected test path '$resolvedTestRoot'."
    }
}
