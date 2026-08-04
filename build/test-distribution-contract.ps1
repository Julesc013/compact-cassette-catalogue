[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$validator = Join-Path $PSScriptRoot 'validate-distribution-contract.ps1'
$sourceProfiles = Join-Path $repositoryRoot 'release\profiles'
$sourcePayload = Join-Path $sourceProfiles 'portable-payload.v1.json'
$sourceLanes = Join-Path $PSScriptRoot 'lanes.json'
$testRoot = Join-Path ([IO.Path]::GetTempPath()) ('c3-distribution-tests-' + [Guid]::NewGuid().ToString('N'))
$utf8 = New-Object Text.UTF8Encoding($false)
$passed = 0

function Invoke-Contract {
    param([string]$Profiles, [string]$Payload, [string]$Lanes)

    # Windows PowerShell 5.1 surfaces redirected native-process stderr as an
    # ErrorRecord. These child failures are the expected result for six negative
    # contract scenarios, so capture their exit status without allowing the
    # parent's Stop preference to terminate the harness.
    $savedErrorActionPreference = $ErrorActionPreference
    $global:LASTEXITCODE = 0
    try {
        $ErrorActionPreference = 'Continue'
        & powershell.exe -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass `
            -File $validator -ProfilesRoot $Profiles -PayloadPath $Payload -LanesPath $Lanes *> $null
        return $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
}

function Reset-Fixture {
    $profiles = Join-Path $testRoot 'profiles'
    if (Test-Path -LiteralPath $profiles) { Remove-Item -LiteralPath $profiles -Recurse -Force }
    [IO.Directory]::CreateDirectory($profiles) | Out-Null
    Copy-Item -LiteralPath (Join-Path $sourceProfiles 'win-x86-net40-portable.toml') -Destination $profiles
    Copy-Item -LiteralPath (Join-Path $sourceProfiles 'win-x64-net48-portable.toml') -Destination $profiles
    Copy-Item -LiteralPath $sourcePayload -Destination (Join-Path $profiles 'portable-payload.v1.json')
    Copy-Item -LiteralPath $sourceLanes -Destination (Join-Path $testRoot 'lanes.json')
    return $profiles
}

function Assert-Pass {
    param([int]$ExitCode, [string]$Name)
    if ($ExitCode -ne 0) { throw "$Name should pass." }
    $script:passed++
}

function Assert-Fail {
    param([int]$ExitCode, [string]$Name)
    if ($ExitCode -eq 0) { throw "$Name should fail." }
    $script:passed++
}

try {
    [IO.Directory]::CreateDirectory($testRoot) | Out-Null
    $profiles = Reset-Fixture
    Assert-Pass (Invoke-Contract $profiles (Join-Path $profiles 'portable-payload.v1.json') (Join-Path $testRoot 'lanes.json')) 'canonical contract'

    $profiles = Reset-Fixture
    $path = Join-Path $profiles 'win-x86-net40-portable.toml'
    [IO.File]::AppendAllText($path, "id = `"duplicate`"`n", $utf8)
    Assert-Fail (Invoke-Contract $profiles (Join-Path $profiles 'portable-payload.v1.json') (Join-Path $testRoot 'lanes.json')) 'duplicate TOML key'

    $profiles = Reset-Fixture
    $path = Join-Path $profiles 'win-x86-net40-portable.toml'
    [IO.File]::AppendAllText($path, "edition = `"classic`"`n", $utf8)
    Assert-Fail (Invoke-Contract $profiles (Join-Path $profiles 'portable-payload.v1.json') (Join-Path $testRoot 'lanes.json')) 'unknown profile key'

    $profiles = Reset-Fixture
    Remove-Item -LiteralPath (Join-Path $profiles 'win-x64-net48-portable.toml')
    Assert-Fail (Invoke-Contract $profiles (Join-Path $profiles 'portable-payload.v1.json') (Join-Path $testRoot 'lanes.json')) 'missing lane profile'

    $profiles = Reset-Fixture
    $path = Join-Path $profiles 'win-x86-net40-portable.toml'
    $text = [IO.File]::ReadAllText($path).Replace('status = "internal"', 'status = "preview"')
    [IO.File]::WriteAllText($path, $text, $utf8)
    Assert-Fail (Invoke-Contract $profiles (Join-Path $profiles 'portable-payload.v1.json') (Join-Path $testRoot 'lanes.json')) 'channel status conflict'

    $profiles = Reset-Fixture
    $payloadPath = Join-Path $profiles 'portable-payload.v1.json'
    $text = [IO.File]::ReadAllText($payloadPath).Replace('"target": "C3.Domain.dll"', '"target": "C3.Catalogue.dll"')
    [IO.File]::WriteAllText($payloadPath, $text, $utf8)
    Assert-Fail (Invoke-Contract $profiles $payloadPath (Join-Path $testRoot 'lanes.json')) 'duplicate payload target'

    $profiles = Reset-Fixture
    $path = Join-Path $profiles 'win-x86-net40-portable.toml'
    $text = [IO.File]::ReadAllText($path).Replace('lane = "win-x86-net40"', 'lane = "win-arm64-net481"')
    [IO.File]::WriteAllText($path, $text, $utf8)
    Assert-Fail (Invoke-Contract $profiles (Join-Path $profiles 'portable-payload.v1.json') (Join-Path $testRoot 'lanes.json')) 'unimplemented lane'
}
finally {
    if (Test-Path -LiteralPath $testRoot) { Remove-Item -LiteralPath $testRoot -Recurse -Force }
    $global:LASTEXITCODE = 0
}

Write-Host "Distribution-contract tests passed: $passed scenarios."
