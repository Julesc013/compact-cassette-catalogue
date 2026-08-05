[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$OutputPath,
    [string]$RemoteName = 'origin',
    [string]$ProviderRef = 'refs/heads/dev/1.x',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot 'servicing-version.ps1')
if (-not [IO.Path]::IsPathRooted($OutputPath)) {
    throw '-OutputPath must be an absolute path outside the source repository.'
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
$repositoryPrefix = [IO.Path]::GetFullPath($repositoryRoot).TrimEnd('\') + '\'
if ($OutputPath.StartsWith($repositoryPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Candidate locks must be external to the source repository.'
}
if (Test-Path -LiteralPath $OutputPath) {
    throw "Refusing to overwrite immutable candidate lock '$OutputPath'."
}
if ($RemoteName -notmatch '^[A-Za-z0-9._-]+$') {
    throw "Remote name '$RemoteName' contains unsupported characters."
}
if ($ProviderRef -notmatch '^refs/heads/[A-Za-z0-9._/-]+$' -or
        $ProviderRef -match '(?:\.\.|[~^:?*\[\\])') {
    throw "Provider ref '$ProviderRef' must be a closed refs/heads/... name."
}
$providerBranch = $ProviderRef.Substring('refs/heads/'.Length)
$ExpectedRemoteRef = "refs/remotes/$RemoteName/$providerBranch"

$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $sourceCommit -notmatch '^[0-9a-f]{40}$') {
    throw 'Could not resolve the source commit for the candidate lock.'
}
$sourceStatus = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
if ($LASTEXITCODE -ne 0 -or $sourceStatus.Count -ne 0) {
    throw "Candidate-lock capture requires a clean source tree:`n$($sourceStatus -join "`n")"
}
& (Join-Path $PSScriptRoot 'validate-baseline-genome.ps1')
& (Join-Path $PSScriptRoot 'validate-lanes.ps1')
& (Join-Path $PSScriptRoot 'verify-builds.ps1') -Configuration $Configuration

$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$lockedLanes = @($manifest.lanes | ForEach-Object {
    $evidencePath = Join-Path $repositoryRoot "artifacts\evidence\build\$($_.id)\$Configuration\toolchain.json"
    $evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
    $resourceTools = @($evidence.resourceTools)
    if ([string]$evidence.source.commit -cne $sourceCommit -or
            @($evidence.source.worktreeStatus).Count -ne 0 -or
            [string]$evidence.toolchainMode -cne 'Preparation' -or
            $resourceTools.Count -ne 1 -or
            [string]$resourceTools[0].sha256 -notmatch '^[0-9a-f]{64}$') {
        throw "$($_.id) preparation evidence is not clean, source-current, or resource-tool closed."
    }
    try {
        [void](Assert-C3VisualStudioServicingFloor `
                -ProductVersion ([string]$evidence.visualStudio.productVersion) `
                -MinimumVersion ([string]$_.initialServicingPin) `
                -Context ([string]$_.id))
    }
    catch {
        throw "$($_.Exception.Message) Update, rebuild Preparation evidence, and retry candidate freeze."
    }
    [ordered]@{
        id = [string]$_.id
        visualStudioProductVersion = [string]$evidence.visualStudio.productVersion
        visualStudioInstallationVersion = [string]$evidence.visualStudio.installationVersion
        msbuildSha256 = [string]$evidence.msbuild.sha256
        vbcSha256 = [string]$evidence.compiler.sha256
        referenceAssemblySetSha256 = [string]$evidence.referenceAssemblies.setSha256
        resourceToolPath = [string]$resourceTools[0].path
        resourceToolSha256 = [string]$resourceTools[0].sha256
    }
})

# Fetch only during the freeze transaction. Candidate builds consume this
# retained snapshot offline and never refresh provider state themselves.
$remoteUrl = (& git -C $repositoryRoot remote get-url $RemoteName).Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($remoteUrl)) {
    throw "Could not resolve URL for candidate source remote '$RemoteName'."
}
$refspec = "+${ProviderRef}:${ExpectedRemoteRef}"
& git -C $repositoryRoot fetch --no-tags $RemoteName $refspec
if ($LASTEXITCODE -ne 0) {
    throw "Could not fetch provider ref '$ProviderRef' from '$RemoteName'."
}
$fetchedAtUtc = [DateTime]::UtcNow.ToString('o')
$remoteCommit = (& git -C $repositoryRoot rev-parse $ExpectedRemoteRef).Trim()
if ($LASTEXITCODE -ne 0 -or $remoteCommit -cne $sourceCommit) {
    throw "Candidate-lock source '$sourceCommit' is not exactly fetched '$RemoteName/$ProviderRef' ('$remoteCommit')."
}

$lock = [ordered]@{
    schemaVersion = 3
    purpose = 'external immutable source-bound candidate toolchain lock'
    status = 'locked'
    sourceCommit = $sourceCommit
    expectedRemoteRef = $ExpectedRemoteRef
    providerRefReceipt = [ordered]@{
        remoteName = $RemoteName
        remoteUrl = $remoteUrl
        providerRef = $ProviderRef
        remoteTrackingRef = $ExpectedRemoteRef
        fetchedCommit = $remoteCommit
        fetchedAtUtc = $fetchedAtUtc
    }
    frozenAtUtc = [DateTime]::UtcNow.ToString('o')
    lanes = $lockedLanes
}
$outputDirectory = [IO.Path]::GetDirectoryName($OutputPath)
if (-not (Test-Path -LiteralPath $outputDirectory -PathType Container)) {
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
}
$json = ($lock | ConvertTo-Json -Depth 8) + "`n"
[IO.File]::WriteAllText($OutputPath, $json, (New-Object Text.UTF8Encoding($false)))
$lockHash = (Get-FileHash -LiteralPath $OutputPath -Algorithm SHA256).Hash.ToLowerInvariant()
Write-Host "Created immutable candidate lock for source $sourceCommit at '$OutputPath'."
Write-Host "toolchainLockSha256=$lockHash"
