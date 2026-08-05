[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$OutputPath,
    [string]$ExpectedRemoteRef = 'refs/remotes/origin/dev/1.x',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
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
if (-not $ExpectedRemoteRef.StartsWith('refs/remotes/', [StringComparison]::Ordinal)) {
    throw '-ExpectedRemoteRef must name a remote-tracking ref under refs/remotes/.'
}

$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $sourceCommit -notmatch '^[0-9a-f]{40}$') {
    throw 'Could not resolve the source commit for the candidate lock.'
}
$sourceStatus = @(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all)
if ($LASTEXITCODE -ne 0 -or $sourceStatus.Count -ne 0) {
    throw "Candidate-lock capture requires a clean source tree:`n$($sourceStatus -join "`n")"
}
& git -C $repositoryRoot show-ref --verify --quiet $ExpectedRemoteRef
if ($LASTEXITCODE -ne 0) {
    throw "Expected remote-tracking ref does not exist locally: $ExpectedRemoteRef"
}
$remoteCommit = (& git -C $repositoryRoot rev-parse $ExpectedRemoteRef).Trim()
if ($LASTEXITCODE -ne 0 -or $remoteCommit -cne $sourceCommit) {
    throw "Candidate-lock source '$sourceCommit' is not exactly '$ExpectedRemoteRef' ('$remoteCommit')."
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

$lock = [ordered]@{
    schemaVersion = 2
    purpose = 'external immutable source-bound candidate toolchain lock'
    status = 'locked'
    sourceCommit = $sourceCommit
    expectedRemoteRef = $ExpectedRemoteRef
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
