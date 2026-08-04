[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [int]$StartupTimeoutSeconds = 30,
    [int]$ExitTimeoutSeconds = 10
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$lanes = @((Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json).lanes)

foreach ($lane in $lanes) {
    $executable = Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\Compact Cassette Catalogue.exe"
    if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
        throw "Missing executable for launch smoke: $($lane.id)"
    }

    # Hidden WinForms processes do not expose a discoverable MainWindowHandle,
    # so use a minimized window for an observable, noninteractive smoke test.
    $process = Start-Process -FilePath $executable -PassThru -WindowStyle Minimized
    try {
        $startupDeadline = [DateTime]::UtcNow.AddSeconds($StartupTimeoutSeconds)
        do {
            Start-Sleep -Milliseconds 200
            $process.Refresh()
            if ($process.HasExited) {
                throw "$($lane.id) exited during startup with code $($process.ExitCode)."
            }
        } while ($process.MainWindowHandle -eq [IntPtr]::Zero -and [DateTime]::UtcNow -lt $startupDeadline)

        if ($process.MainWindowHandle -eq [IntPtr]::Zero) {
            throw "$($lane.id) did not create a main window within $StartupTimeoutSeconds seconds."
        }
        if (-not $process.CloseMainWindow()) {
            throw "$($lane.id) did not accept a normal close-window request."
        }
        if (-not $process.WaitForExit($ExitTimeoutSeconds * 1000)) {
            throw "$($lane.id) did not exit within $ExitTimeoutSeconds seconds after normal close."
        }
        if ($process.ExitCode -ne 0) {
            throw "$($lane.id) exited with code $($process.ExitCode)."
        }
        Write-Host "Launch smoke passed: $($lane.id) created its main window and exited normally."
    }
    finally {
        $process.Refresh()
        if (-not $process.HasExited) {
            $process.Kill()
            [void]$process.WaitForExit(5000)
        }
        $process.Dispose()
        Start-Sleep -Milliseconds 500
    }
}
