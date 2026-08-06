[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$sharedRoot = Join-Path $repositoryRoot 'Compact Cassette Catalogue Installer\Shared'
$journalPath = Join-Path $sharedRoot 'SetupTransactionJournal.vb'
$transactionPath = Join-Path $sharedRoot 'SetupDurableTransaction.vb'
$testPath = Join-Path $repositoryRoot 'tests\C3.Setup.Characterization\Program.vb'

foreach ($requiredPath in @($journalPath, $transactionPath, $testPath)) {
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) { throw "Durable setup authority is missing: $requiredPath" }
}

$journal = [IO.File]::ReadAllText($journalPath)
$transaction = [IO.File]::ReadAllText($transactionPath)
$tests = [IO.File]::ReadAllText($testPath)
$registry = [IO.File]::ReadAllText((Join-Path $sharedRoot 'SetupRegistry.vb'))
$shortcuts = [IO.File]::ReadAllText((Join-Path $sharedRoot 'SetupShortcuts.vb'))
$expectedPhases = @(
    'prepared',
    'staged',
    'backup-complete',
    'payload-promoted',
    'shortcuts-mutated',
    'registry-mutated',
    'state-committed',
    'complete',
    'rollback-started',
    'rollback-complete'
)
foreach ($phase in $expectedPhases) {
    if (-not $journal.Contains('"' + $phase + '"')) { throw "Durable setup journal omits closed phase '$phase'." }
}
foreach ($token in @(
        'FileOptions.WriteThrough',
        'stream.Flush(True)',
        'File.Replace(temporary, path, Nothing, True)',
        'identitySha256',
        'recordSha256',
        'RetainSettledEvidence')) {
    if (-not $journal.Contains($token)) { throw "Durable setup journal omits '$token'." }
}
if (-not $transaction.Contains('stream.Flush(True)') -or -not $registry.Contains('key.Flush()') -or
        -not $registry.Contains('baseKey.Flush()') -or -not $shortcuts.Contains('stream.Flush(True)')) {
    throw 'Durable setup must flush staged/state bytes, HKLM changes, and WSH shortcut bytes before advancing its journal.'
}

$installStart = $transaction.IndexOf('Public Shared Function Install(', [StringComparison]::Ordinal)
$uninstallStart = $transaction.IndexOf('Public Shared Sub Uninstall(', [StringComparison]::Ordinal)
if ($installStart -lt 0 -or $uninstallStart -le $installStart) { throw 'Durable install/uninstall coordinators are missing or reordered.' }
$install = $transaction.Substring($installStart, $uninstallStart - $installStart)
$orderedInstallTokens = @(
    'SetupTransactionPhases.Prepared',
    'SetupTransactionPhases.Staged',
    'SetupTransactionPhases.BackupComplete',
    'SetupTransactionPhases.PayloadPromoted',
    'SetupTransactionPhases.ShortcutsMutated',
    'SetupTransactionPhases.RegistryMutated',
    'File.Move(pendingState, statePath)',
    'SetupTransactionPhases.StateCommitted',
    'SetupTransactionPhases.Complete'
)
$cursor = -1
foreach ($token in $orderedInstallTokens) {
    $next = $install.IndexOf($token, $cursor + 1, [StringComparison]::Ordinal)
    if ($next -le $cursor) { throw "Durable install does not preserve required order at '$token'." }
    $cursor = $next
}

foreach ($token in @(
        '--journal-crash-child',
        'Environment.Exit(97)',
        'InstallProcessDeathRecovers',
        'RepairProcessDeathRecovers',
        'UninstallProcessDeathRecovers',
        'RecoveryRejectsAlteredPromotedBytes',
        'InstalledStateIsCommittedLast')) {
    if (-not $tests.Contains($token)) { throw "Setup crash regression authority omits '$token'." }
}

$installerStartup = [IO.File]::ReadAllText((Join-Path $repositoryRoot 'Compact Cassette Catalogue Installer\frmMain.vb'))
$uninstallerStartup = [IO.File]::ReadAllText((Join-Path $repositoryRoot 'Compact Cassette Catalogue Uninstaller\My Project\ApplicationEvents.vb'))
if (-not $installerStartup.Contains('SetupTransactionRecovery.RecoverIncomplete(installDirectory')) {
    throw 'Installer startup does not recover the default per-machine root before showing the wizard.'
}
if (-not $uninstallerStartup.Contains('RecoverInterruptedTransaction(arguments(1))') -or
        -not $uninstallerStartup.Contains('RecoverInterruptedTransaction(statePath)')) {
    throw 'Uninstaller startup does not recover installed or relocated invocations before ownership validation.'
}

Write-Host "Durable setup journal authority verified: $($expectedPhases.Count) closed phases, write-through replacement, retained evidence, commit-last state, and child-process crash coverage."
