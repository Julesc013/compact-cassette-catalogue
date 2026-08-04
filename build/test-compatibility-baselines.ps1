[CmdletBinding()]
param(
    [string]$BaselineRoot,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$SkipBuild
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($BaselineRoot)) {
    $BaselineRoot = Join-Path $repositoryRoot 'artifacts\compatibility\official'
}
$baselineRootPath = [IO.Path]::GetFullPath($BaselineRoot)

& (Join-Path $PSScriptRoot 'validate-compatibility-corpus.ps1')
if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'test.ps1') -Configuration $Configuration
}

$testExecutable = Join-Path $repositoryRoot `
    "artifacts\tests\characterization\$Configuration\C3.CharacterizationTests.exe"
if (-not (Test-Path -LiteralPath $testExecutable -PathType Leaf)) {
    throw "Characterization driver is missing: $testExecutable"
}

$laneAssemblies = @(
    (Join-Path $repositoryRoot `
        "artifacts\bin\win-x86-net40\$Configuration\C3.Infrastructure.dll"),
    (Join-Path $repositoryRoot `
        "artifacts\bin\win-x64-net48\$Configuration\C3.Infrastructure.dll"))
foreach ($assembly in $laneAssemblies) {
    if (-not (Test-Path -LiteralPath $assembly -PathType Leaf)) {
        throw "Build both product lanes before the exact-binary compatibility matrix: $assembly"
    }
}
$laneHashes = @($laneAssemblies | ForEach-Object {
        (Get-FileHash -LiteralPath $_ -Algorithm SHA256).Hash.ToLowerInvariant()
    } | Select-Object -Unique)
if ($laneHashes.Count -ne 1) {
    throw 'The two product lanes do not carry the same legacy persistence implementation.'
}

$corpusPath = Join-Path $repositoryRoot 'fixtures\compatibility\1x\corpus.v1.json'
$corpus = Get-Content -LiteralPath $corpusPath -Raw | ConvertFrom-Json
$workRoot = Join-Path $repositoryRoot 'artifacts\compatibility\matrix'
if (Test-Path -LiteralPath $workRoot) {
    Remove-Item -LiteralPath $workRoot -Recurse -Force
}
[IO.Directory]::CreateDirectory($workRoot) | Out-Null

$currentWriterOutput = Join-Path $workRoot 'current-v1.1-writer.xml'
& $testExecutable '--write-current-v1.1' $currentWriterOutput
if ($LASTEXITCODE -ne 0) {
    throw 'The current v1.1 writer probe failed.'
}

$probe = Join-Path $PSScriptRoot 'invoke-legacy-baseline-probe.ps1'
$system32PowerShell = Join-Path $env:SystemRoot 'System32\WindowsPowerShell\v1.0\powershell.exe'
$x86PowerShell = Join-Path $env:SystemRoot 'SysWOW64\WindowsPowerShell\v1.0\powershell.exe'
$results = New-Object 'Collections.Generic.List[object]'

foreach ($release in @($corpus.releases | Where-Object { $_.support -ceq 'supported' })) {
    foreach ($artifact in $release.artifacts) {
        $binaryPath = Join-Path `
            (Join-Path $baselineRootPath ([string]$release.tag)) `
            ([string]$artifact.name)
        if (-not (Test-Path -LiteralPath $binaryPath -PathType Leaf)) {
            throw "Official baseline is missing. Run build/fetch-compatibility-baselines.ps1: $binaryPath"
        }
        $binary = Get-Item -LiteralPath $binaryPath
        $binaryHash = (Get-FileHash -LiteralPath $binaryPath -Algorithm SHA256).Hash.ToLowerInvariant()
        if ($binary.Length -ne [long]$artifact.size -or
                $binaryHash -cne [string]$artifact.sha256) {
            throw "Official baseline differs from the corpus: $binaryPath"
        }

        $hostExecutable = if ([string]$artifact.architecture -ceq 'x86') {
            $x86PowerShell
        }
        else {
            $system32PowerShell
        }
        $roundTrip = Join-Path $workRoot (
            ([IO.Path]::GetFileNameWithoutExtension([string]$artifact.name)) + '-round-trip.xml')
        $probeArguments = @(
            '-NoLogo', '-NoProfile', '-NonInteractive', '-ExecutionPolicy', 'Bypass',
            '-File', $probe,
            '-BinaryPath', $binaryPath,
            '-InputPath', $currentWriterOutput,
            '-OutputPath', $roundTrip,
            '-ExpectedProductVersion', [string]$release.reportedProductVersion,
            '-ExpectedStage', [string]$release.reportedStage,
            '-ExpectedCatalogueFormat', [string]$release.catalogueFormat)
        & $hostExecutable @probeArguments
        if ($LASTEXITCODE -ne 0) {
            throw "Historical reader/writer probe failed: $($artifact.name)"
        }

        & $testExecutable '--validate-v1.1' $roundTrip
        if ($LASTEXITCODE -ne 0) {
            throw "Current reader rejected historical writer output: $($artifact.name)"
        }
        $results.Add([PSCustomObject]@{
                tag = [string]$release.tag
                artifact = [string]$artifact.name
                artifactSha256 = $binaryHash
                architecture = [string]$artifact.architecture
                result = 'pass'
            })
    }
}

$summary = [PSCustomObject]@{
    schemaVersion = 1
    currentWriter = $currentWriterOutput
    sharedInfrastructureSha256 = $laneHashes[0]
    results = @($results | ForEach-Object { $_ })
}
$summaryPath = Join-Path $workRoot 'summary.json'
[IO.File]::WriteAllText(
    $summaryPath,
    (($summary | ConvertTo-Json -Depth 10) + "`n"),
    (New-Object Text.UTF8Encoding($false)))
Write-Host "$($results.Count) exact historical artifact matrix row(s) passed in both C3 build lanes."
Write-Host "Matrix summary: $summaryPath"
