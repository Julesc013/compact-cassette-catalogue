[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string[]]$LaneId = @(),
    [string]$ExecutablePath,
    [ValidateSet('BuilderSmoke', 'TargetQualification')]
    [string]$ProofMode = 'BuilderSmoke',
    [string]$TargetEnvironmentId,
    [int]$StartupTimeoutSeconds = 30,
    [int]$ExitTimeoutSeconds = 30,
    [switch]$AllowKnownCloseTimeout,
    [switch]$SelfTest
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Test-StringBlank {
    param([string]$Value)
    return [string]::IsNullOrEmpty($Value) -or $Value.Trim().Length -eq 0
}

function Get-NativeArchitecture {
    $value = [string]$env:PROCESSOR_ARCHITEW6432
    if (Test-StringBlank $value) {
        $value = [string]$env:PROCESSOR_ARCHITECTURE
    }
    switch -Regex ($value) {
        '^(?i:AMD64|X64)$' { return 'x64' }
        '^(?i:X86|I386)$' { return 'x86' }
        '^(?i:ARM64|AARCH64)$' { return 'ARM64' }
        default { throw "Unsupported or unknown native host architecture '$value'." }
    }
}

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repositoryRoot = Split-Path -Parent $scriptRoot
$lanes = @(& (Join-Path $scriptRoot 'get-runtime-lanes.ps1'))
if ($SelfTest) {
    if ((Test-StringBlank $scriptRoot) -or $lanes.Count -ne 3) {
        throw 'smoke-launch.ps1 PowerShell 2 self-test could not resolve its script root or lane projection.'
    }
    Write-Host "smoke-launch.ps1 PowerShell 2 self-test passed at '$scriptRoot'."
    return
}
if ($LaneId.Count -gt 0) {
    $knownLaneIds = @($lanes | ForEach-Object { [string]$_.id })
    $unknownLaneIds = @($LaneId | Where-Object { $knownLaneIds -notcontains $_ })
    if ($unknownLaneIds.Count -gt 0) {
        throw "Unknown launch-smoke lane(s): $($unknownLaneIds -join ', ')"
    }
    $lanes = @($lanes | Where-Object { $LaneId -contains $_.id })
}
if (-not (Test-StringBlank $ExecutablePath) -and $lanes.Count -ne 1) {
    throw '-ExecutablePath requires exactly one selected -LaneId.'
}
if ($ProofMode -ceq 'TargetQualification' -and $lanes.Count -ne 1) {
    throw 'Target qualification verifies exactly one lane at a time.'
}

$nativeArchitecture = Get-NativeArchitecture
$executedCount = 0
foreach ($lane in $lanes) {
    $requiredArchitecture = [string]$lane.runtimeArchitecture
    if ($ProofMode -ceq 'TargetQualification') {
        if ((Test-StringBlank $TargetEnvironmentId) -or
                $TargetEnvironmentId -cne [string]$lane.runtimeEnvironmentId) {
            throw "$($lane.id) target proof requires environment ID '$($lane.runtimeEnvironmentId)', found '$TargetEnvironmentId'."
        }
        if ($nativeArchitecture -cne $requiredArchitecture) {
            throw "$($lane.id) target proof requires native host architecture '$requiredArchitecture', found '$nativeArchitecture'. Emulation is not qualification."
        }
    }
    else {
        $canExecute = switch ($requiredArchitecture) {
            'x86' { @('x86', 'x64', 'ARM64') -contains $nativeArchitecture }
            'x64' { @('x64', 'ARM64') -contains $nativeArchitecture }
            'ARM64' { $nativeArchitecture -ceq 'ARM64' }
            default { $false }
        }
        if (-not $canExecute) {
            Write-Warning "Builder smoke skipped $($lane.id): native host '$nativeArchitecture' cannot execute '$requiredArchitecture'. Binary inspection remains required; runtime proof belongs on '$($lane.runtimeEnvironmentId)'."
            continue
        }
    }

    $executable = if (-not (Test-StringBlank $ExecutablePath)) {
        [IO.Path]::GetFullPath($ExecutablePath)
    }
    else {
        Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\Compact Cassette Catalogue.exe"
    }
    if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
        throw "Missing executable for launch smoke: $($lane.id) at '$executable'."
    }

    $executedCount++
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
        $process.Close()
        Start-Sleep -Milliseconds 500
    }
}

Write-Host "$ProofMode completed: executed $executedCount of $($lanes.Count) selected lane(s) on native host architecture $nativeArchitecture."
