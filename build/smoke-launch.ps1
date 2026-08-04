[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string[]]$LaneId = @(),
    [int]$StartupTimeoutSeconds = 30,
    [int]$ExitTimeoutSeconds = 30,
    [switch]$AllowKnownCloseTimeout
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$lanes = @((Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json).lanes)
if ($LaneId.Count -gt 0) {
    $unknownLaneIds = @($LaneId | Where-Object { $_ -notin @($lanes.id) })
    if ($unknownLaneIds.Count -gt 0) {
        throw "Unknown launch-smoke lane(s): $($unknownLaneIds -join ', ')"
    }
    $lanes = @($lanes | Where-Object { $_.id -in $LaneId })
}

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
        $exitedNormally = $process.WaitForExit($ExitTimeoutSeconds * 1000)
        if (-not $exitedNormally) {
            $message = "$($lane.id) did not exit within $ExitTimeoutSeconds seconds after normal close; this reproduces the frozen recursive Closing/Application.Exit baseline defect."
            if (-not $AllowKnownCloseTimeout) {
                throw $message
            }
            Write-Warning $message
            Write-Host "Launch smoke passed with known close defect: $($lane.id) created its main window; cleanup will terminate the process."
        }
        else {
            if ($process.ExitCode -ne 0) {
                throw "$($lane.id) exited with code $($process.ExitCode)."
            }
            Write-Host "Launch smoke passed: $($lane.id) created its main window and exited normally."
        }
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
