[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$tracePath = Join-Path $repositoryRoot 'Compact Cassette Catalogue\StartupTrace.vb'
$applicationPath = Join-Path $repositoryRoot 'Compact Cassette Catalogue\ApplicationEvents.vb'
$mainPath = Join-Path $repositoryRoot 'Compact Cassette Catalogue\frmMain.vb'
$projectPath = Join-Path $repositoryRoot 'Compact Cassette Catalogue\Compact Cassette Catalogue.vbproj'

$trace = Get-Content -LiteralPath $tracePath -Raw
$application = Get-Content -LiteralPath $applicationPath -Raw
$main = Get-Content -LiteralPath $mainPath -Raw
$project = Get-Content -LiteralPath $projectPath -Raw

$requiredTraceFragments = @(
    'Environment.GetEnvironmentVariable("C3_STARTUP_TRACE")',
    'If String.IsNullOrWhiteSpace(tracePath) Then',
    'Stopwatch.GetTimestamp()',
    'DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)',
    'Catch',
    'Diagnostic evidence must never delay or prevent normal startup.'
)
foreach ($fragment in $requiredTraceFragments) {
    if (-not $trace.Contains($fragment)) {
        throw "Startup tracing is missing required fail-safe fragment: $fragment"
    }
}

$milestoneSources = [ordered]@{
    'application.startup.enter' = $application
    'application.startup.complete' = $application
    'main.constructor.enter' = $main
    'main.initialize-component.complete' = $main
    'main.load.enter' = $main
    'main.load.complete' = $main
    'main.handle-created' = $main
    'main.shown' = $main
    'main.first-idle' = $main
}
foreach ($entry in $milestoneSources.GetEnumerator()) {
    if (-not $entry.Value.Contains('StartupTrace.Record("' + $entry.Key + '")')) {
        throw "Startup instrumentation is missing milestone '$($entry.Key)'."
    }
}

if (-not $main.Contains('RemoveHandler Application.Idle, AddressOf RecordFirstIdle')) {
    throw 'The first-idle probe must unsubscribe itself before recording the one-shot milestone.'
}
if (-not $project.Contains('<Compile Include="StartupTrace.vb" />')) {
    throw 'The original application project does not compile StartupTrace.vb.'
}
if ($trace -match '(?im)^\s*(Public|Friend)\s+(Class|Structure)\s+') {
    throw 'Startup instrumentation must remain an internal module and may not add a public object model.'
}

Write-Host 'Startup instrumentation source contract passed: opt-in, fail-safe, high-resolution, one-shot idle, and all nine application milestones present.'
