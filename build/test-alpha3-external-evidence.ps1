[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha3-external-evidence.ps1')

$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ("c3-alpha3-evidence-selftest-" + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temporaryRoot | Out-Null
try {
    $evidenceName = 'retained-evidence.txt'
    $evidencePath = Join-Path $temporaryRoot $evidenceName
    [IO.File]::WriteAllText($evidencePath, "synthetic retained evidence`n", (New-Object Text.UTF8Encoding($false)))
    $evidenceHash = (Get-FileHash -LiteralPath $evidencePath -Algorithm SHA256).Hash.ToLowerInvariant()

    $historical = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'evidence-templates\alpha3-historical-gate1.json') -Raw | ConvertFrom-Json
    $historical.status = 'pass'
    $historical.privateCatalogueEvidenceFile = $evidenceName
    $historical.privateCatalogueSetSha256 = $evidenceHash
    $historical.controlResourceEvidenceFile = $evidenceName
    $historical.controlResourceEvidenceSha256 = $evidenceHash
    foreach ($workflow in @($historical.workflows)) {
        $workflow.result = 'complete'
        $workflow.unexplainedDeviationCount = 0
        foreach ($scenario in @($workflow.scenarios)) {
            $scenario.outcome = 'compatible'
            $scenario.classification = 'expected-compatible'
            $scenario.defectIds = @()
            $scenario.notes = 'Synthetic workflow observation retained for the verifier self-test.'
        }
        $workflow.evidenceFile = $evidenceName
        $workflow.evidenceSha256 = $evidenceHash
    }
    $historical.workflows[0].scenarios[4].outcome = 'known-defect-reproduced'
    $historical.workflows[0].scenarios[4].classification = 'classified-known-defect'
    $historical.workflows[0].scenarios[4].defectIds = @('APP-002')
    $historical.workflows[0].scenarios[4].notes = 'Synthetic recursive-close reproduction classified as APP-002.'
    $historical.workflows[1].scenarios[5].outcome = 'known-defect-not-reproduced'
    $historical.workflows[1].scenarios[5].classification = 'classified-known-defect'
    $historical.workflows[1].scenarios[5].defectIds = @('APP-001')
    $historical.workflows[1].scenarios[5].notes = 'Synthetic bounded run did not reproduce APP-001; the absence is explicit.'
    foreach ($cell in @($historical.catalogueExchange)) {
        $cell.result = 'pass'
        $cell.evidenceFile = $evidenceName
        $cell.evidenceSha256 = $evidenceHash
    }
    $historical.recordedAtUtc = [DateTime]::UtcNow.ToString('o')
    $historicalPath = Join-Path $temporaryRoot 'historical.json'
    [IO.File]::WriteAllText($historicalPath, (($historical | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
    Assert-C3Alpha3HistoricalGate1Evidence -Path $historicalPath | Out-Null

    $historical.workflows[0].scenarios[4].defectIds = @()
    [IO.File]::WriteAllText($historicalPath, (($historical | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
    try {
        Assert-C3Alpha3HistoricalGate1Evidence -Path $historicalPath | Out-Null
        throw 'External-evidence self-test accepted a known-defect outcome without a defect-ledger binding.'
    }
    catch {
        if ($_.Exception.Message -notmatch 'does not bind a known application defect') { throw }
    }
    $historical.workflows[0].scenarios[4].defectIds = @('APP-002')
    $historical.workflows[0].unexplainedDeviationCount = 1
    [IO.File]::WriteAllText($historicalPath, (($historical | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
    try {
        Assert-C3Alpha3HistoricalGate1Evidence -Path $historicalPath | Out-Null
        throw 'External-evidence self-test accepted an unexplained historical deviation.'
    }
    catch {
        if ($_.Exception.Message -notmatch 'unexplained deviation') { throw }
    }
    $historical.workflows[0].unexplainedDeviationCount = 0

    $target = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'evidence-templates\alpha3-target-qualification.json') -Raw | ConvertFrom-Json
    $target.status = 'pass'
    $target.packageSource = 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
    $target.toolchainLockSha256 = 'bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb'
    foreach ($record in @($target.runtime) + @($target.setup)) {
        $record.packageSha256 = 'cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc'
        $record.entryManifestSha256 = 'dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd'
        $record.result = 'pass'
        $record.evidenceFile = $evidenceName
        $record.evidenceSha256 = $evidenceHash
    }
    $target.recordedAtUtc = [DateTime]::UtcNow.ToString('o')
    $targetPath = Join-Path $temporaryRoot 'target.json'
    [IO.File]::WriteAllText($targetPath, (($target | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
    Assert-C3Alpha3TargetQualificationEvidence -Path $targetPath -PackageSource $target.packageSource `
        -ToolchainLockSha256 $target.toolchainLockSha256 -Manifest $manifest | Out-Null

    $target.setup[0].scenarios = @($target.setup[0].scenarios | Where-Object { $_ -cne 'injected-rollback' })
    [IO.File]::WriteAllText($targetPath, (($target | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
    try {
        Assert-C3Alpha3TargetQualificationEvidence -Path $targetPath -PackageSource $target.packageSource `
            -ToolchainLockSha256 $target.toolchainLockSha256 -Manifest $manifest | Out-Null
        throw 'External-evidence self-test accepted a setup matrix missing injected rollback.'
    }
    catch {
        if ($_.Exception.Message -notmatch 'missing, invalid, or incomplete') { throw }
    }
    Write-Host 'Alpha 3 external-evidence controls accepted complete historical/target indexes and rejected an incomplete setup scenario set.'
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) { Remove-Item -LiteralPath $temporaryRoot -Recurse -Force }
}
