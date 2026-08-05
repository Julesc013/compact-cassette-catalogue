[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$AssessmentPath,
    [string]$CandidateRoot,
    [string]$OutputPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-contract.ps1')
. (Join-Path $PSScriptRoot 'beta1-verdict.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
[void](Assert-C3Beta1ManifestPath -Path (Join-Path $PSScriptRoot 'lanes.json'))
$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if (@(& git -C $repositoryRoot status --porcelain=v1 --untracked-files=all).Count -ne 0) { throw 'Beta 1 verdict generation requires clean C-beta.' }
if ([string]::IsNullOrWhiteSpace($OutputPath)) { $OutputPath = Join-Path $repositoryRoot 'release\validation\1.3.0-beta.1-verdict.json' }
$existing = Get-Content -LiteralPath $OutputPath -Raw | ConvertFrom-Json
if ([string]$existing.status -cne 'template') { throw 'Refusing to overwrite a completed Beta 1 verdict.' }
$assessment = Get-Content -LiteralPath $AssessmentPath -Raw | ConvertFrom-Json
$gates = @($assessment.gates)
$actualIds = @($gates | ForEach-Object { [string]$_.id } | Sort-Object)
if ([int]$assessment.schemaVersion -ne 1 -or [string]$assessment.releaseLabel -cne '1.3.0b1' -or
        ($actualIds -join "`n") -cne (($script:C3Beta1GateIds | Sort-Object) -join "`n")) {
    throw 'Beta 1 assessment does not contain the exact closed gate set.'
}
$gateRecords = @($gates | ForEach-Object {
        $gate = $_; $evidenceHash = $null
        if (-not [string]::IsNullOrWhiteSpace([string]$gate.evidenceFile)) {
            $evidencePath = Resolve-C3Beta1EvidencePath -RepositoryRoot $repositoryRoot -RelativePath ([string]$gate.evidenceFile) -Context "Beta 1 gate '$($gate.id)'"
            $evidenceHash = (Get-FileHash -LiteralPath $evidencePath -Algorithm SHA256).Hash.ToLowerInvariant()
        }
        [ordered]@{ id = [string]$gate.id; status = [string]$gate.status; evidenceFile = $gate.evidenceFile; evidenceSha256 = $evidenceHash; reason = [string]$gate.reason }
    })
$portableGo = @($gateRecords | Where-Object { $script:C3Beta1PortableGateIds -contains [string]$_.id -and [string]$_.status -cne 'pass' }).Count -eq 0
$setupGo = @($gateRecords | Where-Object { $script:C3Beta1SetupGateIds -contains [string]$_.id -and [string]$_.status -cne 'pass' }).Count -eq 0
$candidate = [ordered]@{ present = $false; path = $null; candidateIndexSha256 = $null }
$lockHash = $null
if (-not [string]::IsNullOrWhiteSpace($CandidateRoot)) {
    $CandidateRoot = [IO.Path]::GetFullPath($CandidateRoot)
    & (Join-Path $PSScriptRoot 'verify-beta1-assets.ps1') -CandidateRoot $CandidateRoot -ExpectedSourceCommit $sourceCommit | Out-Null
    $candidateRecord = Get-Content -LiteralPath (Join-Path $CandidateRoot 'evidence\candidate.json') -Raw | ConvertFrom-Json
    $lockHash = [string]$candidateRecord.toolchainLockSha256
    $candidate.present = $true
    $candidate.path = "artifacts/candidates/1.3.0b1/$sourceCommit"
    $candidate.candidateIndexSha256 = (Get-FileHash -LiteralPath (Join-Path $CandidateRoot 'evidence\candidate.json') -Algorithm SHA256).Hash.ToLowerInvariant()
}
if (($portableGo -and $setupGo) -and -not $candidate.present) { throw 'A GO assessment cannot be recorded without the exact retained six-asset Candidate.' }
$record = [ordered]@{
    schemaVersion = 1; status = $(if ($portableGo -and $setupGo) { 'go' } else { 'no-go' })
    releaseVersion = '1.3.0'; releaseStage = 'Beta 1'; releaseLabel = '1.3.0b1'; releaseTag = 'v1.3.0b1'
    releaseChannel = 'beta'; publicationStatus = 'retained-unpublished'; sourceCommit = $sourceCommit
    toolchainLockSha256 = $lockHash; candidate = $candidate; gates = $gateRecords
    portableBetaGo = $portableGo; classicSetupBetaGo = $setupGo; overallBetaGo = ($portableGo -and $setupGo)
    tagAuthorized = ($portableGo -and $setupGo); legacyPromotionAuthorized = ($portableGo -and $setupGo)
    publicReleaseAuthorized = $false; feedChangeAuthorized = $false; masterOrDev2ChangeAuthorized = $false
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
}
$utf8 = New-Object Text.UTF8Encoding($false)
[IO.File]::WriteAllText($OutputPath, (($record | ConvertTo-Json -Depth 10) + "`n"), $utf8)
[void](Assert-C3Beta1Verdict -Path $OutputPath -RepositoryRoot $repositoryRoot)
Write-Host "Recorded mechanical Beta 1 $($record.status.ToUpperInvariant()) with portable=$portableGo setup=$setupGo overall=$($portableGo -and $setupGo)."

