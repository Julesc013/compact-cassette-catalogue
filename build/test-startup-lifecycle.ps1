[CmdletBinding()]
param(
    [string[]]$LaneId = @('win-x86-net40', 'win-x64-net48'),
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$ExecutablePath,
    [int]$Cycles = 5,
    [int]$WarmRunsPerCycle = 3,
    [int]$StartupTimeoutSeconds = 30,
    [int]$ExitTimeoutSeconds = 30,
    [int]$QuiescentDelayMilliseconds = 1500,
    [string]$EvidenceDirectory
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if ($Cycles -lt 1 -or $WarmRunsPerCycle -lt 0) {
    throw 'Startup lifecycle testing requires at least one cycle and a nonnegative warm-run count.'
}
if (-not [string]::IsNullOrWhiteSpace($ExecutablePath) -and $LaneId.Count -ne 1) {
    throw '-ExecutablePath requires exactly one selected lane.'
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'lanes.json') -Raw | ConvertFrom-Json
$knownLanes = @($manifest.lanes | Where-Object { $_.id -in @('win-x86-net40', 'win-x64-net48') })
$lanes = @($knownLanes | Where-Object { $LaneId -contains [string]$_.id })
if ($lanes.Count -ne $LaneId.Count) {
    $unknown = @($LaneId | Where-Object { $knownLanes.id -notcontains $_ })
    throw "Startup lifecycle testing supports the executable builder lanes only; unknown selection: $($unknown -join ', ')."
}

$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) {
    throw 'Could not resolve startup-test source commit.'
}
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) {
    $EvidenceDirectory = Join-Path $repositoryRoot "artifacts\evidence\startup\$sourceCommit"
}
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)
[void](New-Item -ItemType Directory -Path $EvidenceDirectory -Force)

$requiredMilestones = @(
    'application.startup.enter',
    'application.startup.complete',
    'main.constructor.enter',
    'main.initialize-component.complete',
    'main.handle-created',
    'main.load.enter',
    'main.load.complete',
    'main.shown',
    'main.first-idle'
)
$orderedPairs = @(
    @('application.startup.enter', 'application.startup.complete'),
    @('application.startup.complete', 'main.constructor.enter'),
    @('main.constructor.enter', 'main.initialize-component.complete'),
    @('main.handle-created', 'main.load.enter'),
    @('main.load.enter', 'main.load.complete'),
    @('main.load.complete', 'main.shown'),
    @('main.shown', 'main.first-idle')
)

function Read-StartupTrace {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return @()
    }
    return @(Get-Content -LiteralPath $Path | ForEach-Object {
        $fields = @($_ -split "`t", 6)
        if ($fields.Count -ne 6) {
            throw "Malformed startup trace line in '$Path': $_"
        }
        [PSCustomObject][ordered]@{
            sequence = [int]$fields[0]
            utc = [string]$fields[1]
            stopwatchTicks = [long]$fields[2]
            processId = [int]$fields[3]
            threadId = [int]$fields[4]
            milestone = [string]$fields[5]
        }
    })
}

$results = New-Object Collections.Generic.List[Object]
$failureCount = 0
foreach ($lane in $lanes) {
    $executable = if (-not [string]::IsNullOrWhiteSpace($ExecutablePath)) {
        [IO.Path]::GetFullPath($ExecutablePath)
    }
    else {
        Join-Path $repositoryRoot "artifacts\bin\$($lane.id)\$Configuration\Compact Cassette Catalogue.exe"
    }
    if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
        throw "Missing startup lifecycle executable for $($lane.id): $executable"
    }
    $configurationPath = $executable + '.config'
    if (-not (Test-Path -LiteralPath $configurationPath -PathType Leaf)) {
        throw "Missing startup lifecycle configuration for $($lane.id): $configurationPath"
    }
    $executableHash = (Get-FileHash -LiteralPath $executable -Algorithm SHA256).Hash.ToLowerInvariant()

    $runNumber = 0
    for ($cycle = 1; $cycle -le $Cycles; $cycle++) {
        for ($withinCycle = 0; $withinCycle -le $WarmRunsPerCycle; $withinCycle++) {
            $runNumber++
            $profile = if ($withinCycle -eq 0) { 'quiescent-cycle-first' } else { 'immediate-repeat' }
            if ($withinCycle -eq 0 -and $QuiescentDelayMilliseconds -gt 0) {
                Start-Sleep -Milliseconds $QuiescentDelayMilliseconds
            }

            $caseName = '{0}-{1:d3}-{2}' -f $lane.id, $runNumber, $profile
            $tracePath = Join-Path $EvidenceDirectory ($caseName + '.tsv')
            if (Test-Path -LiteralPath $tracePath) {
                Remove-Item -LiteralPath $tracePath -Force
            }
            $process = $null
            $processStartUtc = [DateTime]::UtcNow
            $timer = [Diagnostics.Stopwatch]::StartNew()
            $startupElapsed = $null
            $firstIdleElapsed = $null
            $closeElapsed = $null
            $mainWindowHandle = 0L
            $cleanupKilled = $false
            $failure = $null
            $trace = @()
            try {
                $previousTracePath = [Environment]::GetEnvironmentVariable('C3_STARTUP_TRACE', 'Process')
                [Environment]::SetEnvironmentVariable('C3_STARTUP_TRACE', $tracePath, 'Process')
                try {
                    $process = Start-Process -FilePath $executable -WorkingDirectory (Split-Path -Parent $executable) -WindowStyle Normal -PassThru
                }
                finally {
                    [Environment]::SetEnvironmentVariable('C3_STARTUP_TRACE', $previousTracePath, 'Process')
                }

                $deadline = [DateTime]::UtcNow.AddSeconds($StartupTimeoutSeconds)
                do {
                    Start-Sleep -Milliseconds 100
                    $process.Refresh()
                    if ($process.HasExited) {
                        throw "Process exited during startup with code $($process.ExitCode)."
                    }
                    $trace = @(Read-StartupTrace -Path $tracePath)
                    if ($startupElapsed -eq $null -and $process.MainWindowHandle -ne [IntPtr]::Zero) {
                        $startupElapsed = $timer.Elapsed.TotalMilliseconds
                        $mainWindowHandle = $process.MainWindowHandle.ToInt64()
                    }
                    if ($firstIdleElapsed -eq $null -and $trace.milestone -contains 'main.first-idle') {
                        $firstIdleElapsed = $timer.Elapsed.TotalMilliseconds
                    }
                } while (($startupElapsed -eq $null -or $firstIdleElapsed -eq $null) -and [DateTime]::UtcNow -lt $deadline)

                if ($startupElapsed -eq $null) {
                    throw "No visible main-window handle was created within $StartupTimeoutSeconds seconds."
                }
                if ($firstIdleElapsed -eq $null) {
                    throw "The first-idle milestone was not reached within $StartupTimeoutSeconds seconds."
                }

                $trace = @(Read-StartupTrace -Path $tracePath)
                foreach ($milestone in $requiredMilestones) {
                    $count = @($trace | Where-Object { $_.milestone -ceq $milestone }).Count
                    if ($count -ne 1) {
                        throw "Milestone '$milestone' occurred $count times; exactly one occurrence is required."
                    }
                }
                foreach ($pair in $orderedPairs) {
                    $before = @($trace | Where-Object { $_.milestone -ceq $pair[0] })[0].sequence
                    $after = @($trace | Where-Object { $_.milestone -ceq $pair[1] })[0].sequence
                    if ($before -ge $after) {
                        throw "Milestone '$($pair[0])' did not precede '$($pair[1])'."
                    }
                }

                $closeTimer = [Diagnostics.Stopwatch]::StartNew()
                if (-not $process.CloseMainWindow()) {
                    throw 'The visible main window rejected a normal close request.'
                }
                if (-not $process.WaitForExit($ExitTimeoutSeconds * 1000)) {
                    throw "The process did not exit within $ExitTimeoutSeconds seconds after a normal close request."
                }
                $closeElapsed = $closeTimer.Elapsed.TotalMilliseconds
                if ($process.ExitCode -ne 0) {
                    throw "The process exited with code $($process.ExitCode)."
                }
            }
            catch {
                $failure = $_.Exception.Message
                $failureCount++
            }
            finally {
                $timer.Stop()
                if ($process -ne $null) {
                    $process.Refresh()
                    if (-not $process.HasExited) {
                        $cleanupKilled = $true
                        $process.Kill()
                        [void]$process.WaitForExit(5000)
                    }
                    $process.Close()
                }
                $trace = @(Read-StartupTrace -Path $tracePath)
            }

            $result = [PSCustomObject][ordered]@{
                laneId = [string]$lane.id
                run = $runNumber
                cycle = $cycle
                profile = $profile
                passed = ($failure -eq $null -and -not $cleanupKilled)
                failure = $failure
                cleanupKilled = $cleanupKilled
                sourceCommit = $sourceCommit
                executablePath = $executable
                executableSha256 = $executableHash
                processStartUtc = $processStartUtc.ToString('o')
                mainWindowHandle = $mainWindowHandle
                mainWindowElapsedMs = if ($startupElapsed -eq $null) { $null } else { [Math]::Round([double]$startupElapsed, 3) }
                firstIdleElapsedMs = if ($firstIdleElapsed -eq $null) { $null } else { [Math]::Round([double]$firstIdleElapsed, 3) }
                closeElapsedMs = if ($closeElapsed -eq $null) { $null } else { [Math]::Round([double]$closeElapsed, 3) }
                totalElapsedMs = [Math]::Round($timer.Elapsed.TotalMilliseconds, 3)
                tracePath = $tracePath
                traceSha256 = if (Test-Path -LiteralPath $tracePath) { (Get-FileHash -LiteralPath $tracePath -Algorithm SHA256).Hash.ToLowerInvariant() } else { $null }
                milestones = @($trace)
            }
            $results.Add($result)
            Write-Host ("{0}: {1} window={2}ms idle={3}ms close={4}ms" -f $caseName, $(if ($result.passed) { 'PASS' } else { 'FAIL' }), $result.mainWindowElapsedMs, $result.firstIdleElapsedMs, $result.closeElapsedMs)
        }
    }
}

$summary = [PSCustomObject][ordered]@{
    schemaVersion = 1
    sourceCommit = $sourceCommit
    generatedAtUtc = [DateTime]::UtcNow.ToString('o')
    host = [Environment]::OSVersion.VersionString
    cycles = $Cycles
    warmRunsPerCycle = $WarmRunsPerCycle
    total = $results.Count
    passed = @($results | Where-Object { $_.passed }).Count
    failed = $failureCount
    results = $results.ToArray()
}
$summaryPath = Join-Path $EvidenceDirectory 'startup-lifecycle-summary.json'
$summary | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $summaryPath -Encoding UTF8
Write-Host "Startup lifecycle matrix: $($summary.passed)/$($summary.total) passed; evidence: $summaryPath"

if ($failureCount -ne 0) {
    throw "Startup lifecycle matrix failed $failureCount of $($results.Count) runs."
}
