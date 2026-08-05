[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$recordPath = Join-Path $repositoryRoot 'release\validation\1.3.0-alpha.2-qualified.json'
$template = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
if ([string]$template.status -cne 'template') {
    throw "Alpha 2 qualified evidence record is not an unpopulated template: $recordPath"
}
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) {
    throw 'Qualified evidence generation requires clean frozen package source C.'
}
& git -C $repositoryRoot show-ref --verify --quiet refs/tags/v1.3.0a2
if ($LASTEXITCODE -eq 0) {
    throw 'Qualified evidence E must be prepared before creating v1.3.0a2.'
}

& (Join-Path $PSScriptRoot 'verify-packages.ps1') -Configuration $Configuration -RequireCandidateEvidence
$packageDirectory = Join-Path $repositoryRoot "artifacts\packages\$($manifest.releaseLabel)"
$packageEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\packages\$($manifest.releaseLabel)"
$firstEntry = Get-Content -LiteralPath (Join-Path $packageEvidenceDirectory "$($manifest.lanes[0].packageName).entries.json") -Raw | ConvertFrom-Json
$packageSource = [string]$firstEntry.sourceCommit
if ($packageSource -cne (& git -C $repositoryRoot rev-parse HEAD).Trim()) {
    throw "Qualified evidence must be generated at package source C '$packageSource'."
}
$packageRecords = @($manifest.lanes | ForEach-Object {
    $entryName = "$($_.packageName).entries.json"
    [ordered]@{
        name = [string]$_.packageName
        sha256 = (Get-FileHash -LiteralPath (Join-Path $packageDirectory ([string]$_.packageName)) -Algorithm SHA256).Hash.ToLowerInvariant()
        entryManifestName = $entryName
        entryManifestSha256 = (Get-FileHash -LiteralPath (Join-Path $packageEvidenceDirectory $entryName) -Algorithm SHA256).Hash.ToLowerInvariant()
    }
})
$buildRecords = @($manifest.lanes | ForEach-Object {
    $buildEvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\build\$($_.id)\$Configuration"
    [ordered]@{
        lane = [string]$_.id
        toolchainEvidenceSha256 = (Get-FileHash -LiteralPath (Join-Path $buildEvidenceDirectory 'toolchain.json') -Algorithm SHA256).Hash.ToLowerInvariant()
        binaryLogSha256 = (Get-FileHash -LiteralPath (Join-Path $buildEvidenceDirectory 'msbuild.binlog') -Algorithm SHA256).Hash.ToLowerInvariant()
    }
})
$record = [ordered]@{
    schemaVersion = 1
    status = 'pass'
    releaseLabel = [string]$manifest.releaseLabel
    packageSource = $packageSource
    toolchainLockSha256 = [string]$firstEntry.toolchainLockSha256
    candidateSourceClosureSha256 = (Get-FileHash -LiteralPath (Join-Path $repositoryRoot 'artifacts\evidence\build\candidate-source-closure.json') -Algorithm SHA256).Hash.ToLowerInvariant()
    sourceReproducibilityRecordSha256 = (Get-FileHash -LiteralPath (Join-Path $repositoryRoot "artifacts\evidence\source-reproducibility\$($manifest.releaseLabel)\source-reproducibility.json") -Algorithm SHA256).Hash.ToLowerInvariant()
    packageChecksumManifestSha256 = (Get-FileHash -LiteralPath (Join-Path $packageDirectory 'SHA256SUMS.txt') -Algorithm SHA256).Hash.ToLowerInvariant()
    entryChecksumManifestSha256 = (Get-FileHash -LiteralPath (Join-Path $packageEvidenceDirectory 'ENTRY_MANIFEST_SHA256SUMS.txt') -Algorithm SHA256).Hash.ToLowerInvariant()
    packages = $packageRecords
    buildEvidence = $buildRecords
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
}
$json = ($record | ConvertTo-Json -Depth 8) + "`n"
[IO.File]::WriteAllText($recordPath, $json, (New-Object Text.UTF8Encoding($false)))
Write-Host "Populated Alpha 2 qualified evidence for package source C '$packageSource'. Update the human validation record and commit only those evidence records as E."
