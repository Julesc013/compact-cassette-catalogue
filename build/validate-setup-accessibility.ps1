[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$contracts = [ordered]@{
    'Compact Cassette Catalogue Installer/frmMain.vb' = @(
        'Private Sub ConfigureAccessibility()',
        'Me.AccessibleName = "Compact Cassette Catalogue Setup Wizard"',
        'txtDirectory.AccessibleName = "Installation directory"',
        'barInstallProgress.AccessibleName = "Installation progress"',
        'Me.AcceptButton = btnInstall',
        'Me.AcceptButton = btnNext',
        'Me.AcceptButton = Nothing'
    )
    'Compact Cassette Catalogue Installer/frmFailure.vb' = @('lblFailure.AccessibleName = "Setup failure details"', 'btnFinish.Text = "&Finish"')
    'Compact Cassette Catalogue Installer/frmSuccess.vb' = @('Me.AccessibleName = "Compact Cassette Catalogue setup complete"', 'chkStartProgram.AccessibleDescription', 'btnFinish.Text = "&Finish"')
    'Compact Cassette Catalogue Uninstaller/frmMain.vb' = @(
        'Private Sub ConfigureAccessibility()',
        'Me.AccessibleName = "Compact Cassette Catalogue Uninstaller"',
        'barInstallProgress.AccessibleName = "Uninstallation progress"',
        'Me.AcceptButton = btnUninstall',
        'Me.AcceptButton = Nothing'
    )
    'Compact Cassette Catalogue Uninstaller/frmFailure.vb' = @('lblFailure.AccessibleName = "Uninstall failure details"', 'btnFinish.Text = "&Finish"')
    'Compact Cassette Catalogue Uninstaller/frmSuccess.vb' = @('Me.AccessibleName = "Compact Cassette Catalogue uninstall complete"', 'chkOpenFeedback.AccessibleDescription', 'btnFinish.Text = "&Finish"')
}
foreach ($relativePath in $contracts.Keys) {
    $text = [IO.File]::ReadAllText((Join-Path $repositoryRoot $relativePath))
    foreach ($fragment in $contracts[$relativePath]) {
        if (-not $text.Contains([string]$fragment)) { throw "$relativePath is missing accessibility contract fragment: $fragment" }
    }
}
Write-Host 'Classic setup accessibility contract verified: keyboard defaults/mnemonics, named paths/status/progress, and completion/failure semantics are explicit.'
