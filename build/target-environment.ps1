# PowerShell 2-compatible target identity collection and closed lane assertions.

function Get-C3NativeArchitecture {
    $value = [string]$env:PROCESSOR_ARCHITEW6432
    if ([string]::IsNullOrEmpty($value) -or $value.Trim().Length -eq 0) {
        $value = [string]$env:PROCESSOR_ARCHITECTURE
    }
    switch -Regex ($value) {
        '^(?i:AMD64|X64)$' { return 'x64' }
        '^(?i:X86|I386)$' { return 'x86' }
        '^(?i:ARM64|AARCH64)$' { return 'ARM64' }
        default { throw "Unsupported or unknown native host architecture '$value'." }
    }
}

function Get-C3TargetEnvironmentFacts {
    $operatingSystems = @(Get-WmiObject -Class Win32_OperatingSystem)
    if ($operatingSystems.Count -ne 1) {
        throw "Expected one Win32_OperatingSystem result, found $($operatingSystems.Count)."
    }
    $operatingSystem = $operatingSystems[0]

    $frameworkKeyPath = 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full'
    $frameworkKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey($frameworkKeyPath)
    if ($null -eq $frameworkKey) {
        return New-Object PSObject -Property @{
            osVersion = [string]$operatingSystem.Version
            osBuild = [int]$operatingSystem.BuildNumber
            servicePackMajor = [int]$operatingSystem.ServicePackMajorVersion
            servicePackMinor = [int]$operatingSystem.ServicePackMinorVersion
            nativeArchitecture = Get-C3NativeArchitecture
            frameworkFullInstalled = 0
            frameworkVersion = ''
            frameworkRelease = 0
        }
    }
    try {
        $installValue = $frameworkKey.GetValue('Install', 0)
        $versionValue = $frameworkKey.GetValue('Version', '')
        $releaseValue = $frameworkKey.GetValue('Release', 0)
        return New-Object PSObject -Property @{
            osVersion = [string]$operatingSystem.Version
            osBuild = [int]$operatingSystem.BuildNumber
            servicePackMajor = [int]$operatingSystem.ServicePackMajorVersion
            servicePackMinor = [int]$operatingSystem.ServicePackMinorVersion
            nativeArchitecture = Get-C3NativeArchitecture
            frameworkFullInstalled = [int]$installValue
            frameworkVersion = [string]$versionValue
            frameworkRelease = [int64]$releaseValue
        }
    }
    finally {
        $frameworkKey.Close()
    }
}

function Assert-C3TargetEnvironment {
    param(
        [Parameter(Mandatory = $true)]$LaneContract,
        [Parameter(Mandatory = $true)]$Facts
    )

    if ([string]$Facts.nativeArchitecture -cne [string]$LaneContract.runtimeArchitecture) {
        throw "$($LaneContract.id) requires native '$($LaneContract.runtimeArchitecture)' architecture, found '$($Facts.nativeArchitecture)'. Emulation is not qualification."
    }

    switch ([string]$LaneContract.id) {
        'win-x86-net40' {
            if ([string]$Facts.osVersion -cne '5.1.2600' -or
                    [int]$Facts.osBuild -ne 2600 -or
                    [int]$Facts.servicePackMajor -ne 3) {
                throw "win-x86-net40 requires Windows XP 5.1.2600 SP3; found version '$($Facts.osVersion)', build '$($Facts.osBuild)', SP '$($Facts.servicePackMajor)'."
            }
            if ([int]$Facts.frameworkFullInstalled -ne 1 -or [string]$Facts.frameworkVersion -notmatch '^4\.0(?:\.|$)') {
                throw "win-x86-net40 requires .NET Framework 4.0 Full; found Install='$($Facts.frameworkFullInstalled)', Version='$($Facts.frameworkVersion)'."
            }
        }
        'win-x64-net48' {
            if ([string]$Facts.osVersion -cne '6.1.7601' -or
                    [int]$Facts.osBuild -ne 7601 -or
                    [int]$Facts.servicePackMajor -ne 1) {
                throw "win-x64-net48 requires Windows 7 6.1.7601 SP1; found version '$($Facts.osVersion)', build '$($Facts.osBuild)', SP '$($Facts.servicePackMajor)'."
            }
            if ([int]$Facts.frameworkFullInstalled -ne 1 -or [int64]$Facts.frameworkRelease -lt 528049) {
                throw "win-x64-net48 requires .NET Framework 4.8 Full (Release >= 528049); found Install='$($Facts.frameworkFullInstalled)', Version='$($Facts.frameworkVersion)', Release='$($Facts.frameworkRelease)'."
            }
        }
        'win-arm64-net481' {
            if ([string]$Facts.osVersion -cne '10.0.22000' -or
                    [int]$Facts.osBuild -ne 22000 -or
                    [int]$Facts.servicePackMajor -ne 0) {
                throw "win-arm64-net481 requires Windows 11 21H2/RTM 10.0.22000; found version '$($Facts.osVersion)', build '$($Facts.osBuild)', SP '$($Facts.servicePackMajor)'."
            }
            if ([int]$Facts.frameworkFullInstalled -ne 1 -or [int64]$Facts.frameworkRelease -lt 533320) {
                throw "win-arm64-net481 requires .NET Framework 4.8.1 Full (Release >= 533320); found Install='$($Facts.frameworkFullInstalled)', Version='$($Facts.frameworkVersion)', Release='$($Facts.frameworkRelease)'."
            }
        }
        default {
            throw "No target identity contract exists for lane '$($LaneContract.id)'."
        }
    }
    return [string]$LaneContract.runtimeEnvironmentId
}
