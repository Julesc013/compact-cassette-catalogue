[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$windowsPowerShell = Join-Path $env:SystemRoot 'System32\WindowsPowerShell\v1.0\powershell.exe'
if (-not (Test-Path -LiteralPath $windowsPowerShell -PathType Leaf)) {
    throw "Windows PowerShell executable was not found: $windowsPowerShell"
}

foreach ($scriptName in @('smoke-launch.ps1', 'verify-target-runtime.ps1', 'verify-target-setup.ps1')) {
    $scriptPath = Join-Path $scriptRoot $scriptName
    $output = @(& $windowsPowerShell -Version 2 -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -File $scriptPath -SelfTest 2>&1)
    if ($LASTEXITCODE -ne 0) {
        throw "$scriptName failed under Windows PowerShell 2.0:`n$($output -join "`n")"
    }
    if (($output -join "`n") -notmatch 'PowerShell 2 self-test passed') {
        throw "$scriptName did not report a successful PowerShell 2.0 self-test:`n$($output -join "`n")"
    }
    Write-Host ($output -join "`n")
}

$environmentOutput = @(& $windowsPowerShell -Version 2 -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass `
    -File (Join-Path $scriptRoot 'test-target-environment.ps1') 2>&1)
if ($LASTEXITCODE -ne 0) {
    throw "test-target-environment.ps1 failed under Windows PowerShell 2.0:`n$($environmentOutput -join "`n")"
}
Write-Host ($environmentOutput -join "`n")

Write-Host 'Actual target-side scripts parsed and executed successfully under Windows PowerShell 2.0.'
