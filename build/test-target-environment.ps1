[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
. (Join-Path $scriptRoot 'target-environment.ps1')
$lanes = @(& (Join-Path $scriptRoot 'get-runtime-lanes.ps1'))

function New-TestFacts {
    param(
        [string]$OsVersion,
        [int]$OsBuild,
        [int]$ServicePackMajor,
        [string]$NativeArchitecture,
        [int]$FrameworkFullInstalled,
        [string]$FrameworkVersion,
        [int64]$FrameworkRelease
    )
    return New-Object PSObject -Property @{
        osVersion = $OsVersion
        osBuild = $OsBuild
        servicePackMajor = $ServicePackMajor
        servicePackMinor = 0
        nativeArchitecture = $NativeArchitecture
        frameworkFullInstalled = $FrameworkFullInstalled
        frameworkVersion = $FrameworkVersion
        frameworkRelease = $FrameworkRelease
    }
}

function Assert-TestFailure {
    param(
        [Parameter(Mandatory = $true)]$Lane,
        [Parameter(Mandatory = $true)]$Facts,
        [Parameter(Mandatory = $true)][string]$ExpectedPattern
    )
    try {
        [void](Assert-C3TargetEnvironment -LaneContract $Lane -Facts $Facts)
        throw "Target identity negative test unexpectedly passed for '$($Lane.id)'."
    }
    catch {
        if ($_.Exception.Message -notmatch $ExpectedPattern) {
            throw
        }
    }
}

$xpLane = @($lanes | Where-Object { $_.id -ceq 'win-x86-net40' })[0]
$x64Lane = @($lanes | Where-Object { $_.id -ceq 'win-x64-net48' })[0]
$arm64Lane = @($lanes | Where-Object { $_.id -ceq 'win-arm64-net481' })[0]
$xpFacts = New-TestFacts '5.1.2600' 2600 3 'x86' 1 '4.0.30319' 0
$x64Facts = New-TestFacts '6.1.7601' 7601 1 'x64' 1 '4.8.09037' 528049
$arm64Facts = New-TestFacts '10.0.22000' 22000 0 'ARM64' 1 '4.8.09037' 533325

if ((Assert-C3TargetEnvironment $xpLane $xpFacts) -cne 'xp-sp3-x86-net40' -or
        (Assert-C3TargetEnvironment $x64Lane $x64Facts) -cne 'windows-7-sp1-x64-net48' -or
        (Assert-C3TargetEnvironment $arm64Lane $arm64Facts) -cne 'windows-11-21h2-arm64-net481') {
    throw 'A valid target identity did not derive its closed environment ID.'
}

$wrongServicePack = New-TestFacts '5.1.2600' 2600 2 'x86' 1 '4.0.30319' 0
Assert-TestFailure $xpLane $wrongServicePack 'requires Windows XP.*SP3'
$wrongFramework = New-TestFacts '6.1.7601' 7601 1 'x64' 1 '4.7.03062' 461814
Assert-TestFailure $x64Lane $wrongFramework 'requires \.NET Framework 4\.8 Full'
$wrongArmHost = New-TestFacts '10.0.22000' 22000 0 'x64' 1 '4.8.09037' 533325
Assert-TestFailure $arm64Lane $wrongArmHost "requires native 'ARM64' architecture"
$wrongArmBuild = New-TestFacts '10.0.22621' 22621 0 'ARM64' 1 '4.8.09037' 533325
Assert-TestFailure $arm64Lane $wrongArmBuild 'requires Windows 11 21H2/RTM'

try {
    & (Join-Path $scriptRoot 'verify-target-runtime.ps1') -TargetEnvironmentId 'spoofed-environment-label'
    throw 'Caller-supplied target environment label was not rejected.'
}
catch {
    if ($_.Exception.Message -notmatch 'Caller-supplied -TargetEnvironmentId is prohibited') {
        throw
    }
}

Write-Host 'Target environment assertions passed valid identities and rejected spoofed label, wrong service pack, framework, architecture, and OS build.'
