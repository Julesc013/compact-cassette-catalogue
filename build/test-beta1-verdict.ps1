[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-verdict.ps1')

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ("c3-beta1-verdict-" + [Guid]::NewGuid().ToString('N'))
$path = Join-Path $temporaryRoot 'verdict.json'
$utf8 = New-Object Text.UTF8Encoding($false)
function Write-Record { param($Value) [IO.File]::WriteAllText($path, (($Value | ConvertTo-Json -Depth 10) + "`n"), $utf8) }
function Assert-Rejected {
    param([string]$Name, [scriptblock]$Mutation, [string]$Pattern, [scriptblock]$Restore, [switch]$RequireGo)
    & $Mutation; Write-Record $record
    try {
        [void](Assert-C3Beta1Verdict -Path $path -RepositoryRoot $repositoryRoot -RequireGo:$RequireGo)
        throw "$Name unexpectedly passed."
    }
    catch { if ($_.Exception.Message -notmatch $Pattern) { throw } }
    & $Restore
}

try {
    New-Item -ItemType Directory -Path $temporaryRoot | Out-Null
    $record = [ordered]@{
        schemaVersion = 1; status = 'no-go'; releaseVersion = '1.3.0'; releaseStage = 'Beta 1'
        releaseLabel = '1.3.0b1'; releaseTag = 'v1.3.0b1'; releaseChannel = 'beta'
        publicationStatus = 'retained-unpublished'; sourceCommit = 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
        toolchainLockSha256 = $null; candidate = [ordered]@{ present = $false; path = $null; candidateIndexSha256 = $null }
        gates = @($script:C3Beta1GateIds | ForEach-Object { [ordered]@{ id = $_; status = 'missing'; evidenceFile = $null; evidenceSha256 = $null; reason = 'Exact evidence is not yet available.' } })
        portableBetaGo = $false; classicSetupBetaGo = $false; overallBetaGo = $false
        tagAuthorized = $false; legacyPromotionAuthorized = $false; publicReleaseAuthorized = $false
        feedChangeAuthorized = $false; masterOrDev2ChangeAuthorized = $false; recordedAtUtc = '2026-08-06T00:00:00Z'
    }
    Write-Record $record
    [void](Assert-C3Beta1Verdict -Path $path -RepositoryRoot $repositoryRoot)

    Assert-Rejected -Name 'operator-forced GO' -Pattern 'mechanical result' `
        -Mutation { $record.overallBetaGo = $true; $record.tagAuthorized = $true } `
        -Restore { $record.overallBetaGo = $false; $record.tagAuthorized = $false }
    Assert-Rejected -Name 'public release authority' -Pattern 'may never authorize' `
        -Mutation { $record.publicReleaseAuthorized = $true } `
        -Restore { $record.publicReleaseAuthorized = $false }
    Assert-Rejected -Name 'PASS without evidence' -Pattern 'evidence SHA-256' `
        -Mutation { $record.gates[0].status = 'pass' } `
        -Restore { $record.gates[0].status = 'missing' }
    Assert-Rejected -Name 'NO-GO tag attempt' -Pattern 'requires overallBetaGo=true' -RequireGo `
        -Mutation { } -Restore { }
    Assert-Rejected -Name 'missing gate row' -Pattern 'each closed gate ID exactly once' `
        -Mutation { $script:savedGates = $record.gates; $record.gates = @($record.gates | Select-Object -Skip 1) } `
        -Restore { $record.gates = $script:savedGates }

    Write-Host 'Beta 1 verdict controls accepted a truthful NO-GO and rejected forced GO, public authority, evidence-free PASS, tag attempt, and incomplete gate set.'
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) { Remove-Item -LiteralPath $temporaryRoot -Recurse -Force }
}

