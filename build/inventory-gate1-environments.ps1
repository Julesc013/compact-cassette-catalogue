[CmdletBinding()]
param(
    [string]$OutputPath,
    [string[]]$FileSearchRoots,
    [int]$MaximumDirectoriesPerRoot = 10000,
    [int]$MaximumMatchesPerRoot = 500,
    [switch]$SkipPlatformQueries
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $repositoryRoot 'artifacts\evidence\historical-gate1\environment-inventory.json'
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts')).TrimEnd('\') + '\'
if (-not $OutputPath.StartsWith($artifactsRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Environment inventory must remain below ignored artifact root '$artifactsRoot'."
}

function Get-C3CommandEvidence {
    param([Parameter(Mandatory = $true)][string]$Name)
    $command = Get-Command $Name -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -eq $command) {
        return [PSCustomObject]@{ name = $Name; available = $false; path = $null; version = $null; sha256 = $null }
    }
    $path = [string]$command.Source
    if ([string]::IsNullOrWhiteSpace($path)) { $path = [string]$command.Path }
    $hash = $null
    if (-not [string]::IsNullOrWhiteSpace($path) -and (Test-Path -LiteralPath $path -PathType Leaf)) {
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    return [PSCustomObject]@{
        name = $Name
        available = $true
        path = $path
        version = [string]$command.Version
        sha256 = $hash
    }
}

function Find-C3BoundedFiles {
    param(
        [Parameter(Mandatory = $true)][string]$Root,
        [Parameter(Mandatory = $true)][string[]]$Extensions,
        [Parameter(Mandatory = $true)][int]$MaximumDirectories,
        [Parameter(Mandatory = $true)][int]$MaximumMatches
    )
    $result = New-Object Collections.Generic.List[Object]
    if (-not (Test-Path -LiteralPath $Root -PathType Container)) {
        return [PSCustomObject]@{ root = $Root; status = 'absent'; directoriesVisited = 0; truncated = $false; files = @() }
    }
    $queue = New-Object Collections.Queue
    $queue.Enqueue([IO.Path]::GetFullPath($Root))
    $visited = 0
    $truncated = $false
    while ($queue.Count -gt 0) {
        if ($visited -ge $MaximumDirectories -or $result.Count -ge $MaximumMatches) { $truncated = $true; break }
        $directory = [string]$queue.Dequeue()
        $visited++
        try {
            foreach ($file in [IO.Directory]::EnumerateFiles($directory)) {
                if ($result.Count -ge $MaximumMatches) { $truncated = $true; break }
                $extension = [IO.Path]::GetExtension($file).ToLowerInvariant()
                if ($Extensions -contains $extension) {
                    $item = Get-Item -LiteralPath $file
                    $result.Add([PSCustomObject]@{
                        path = $item.FullName
                        length = [long]$item.Length
                        sha256 = (Get-FileHash -LiteralPath $item.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
                    })
                }
            }
            foreach ($child in [IO.Directory]::EnumerateDirectories($directory)) {
                try {
                    $item = Get-Item -LiteralPath $child
                    if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -eq 0) { $queue.Enqueue($item.FullName) }
                }
                catch { }
            }
        }
        catch { }
    }
    return [PSCustomObject]@{
        root = [IO.Path]::GetFullPath($Root)
        status = 'searched-once'
        directoriesVisited = $visited
        truncated = $truncated
        files = $result.ToArray()
    }
}

if ($null -eq $FileSearchRoots -or $FileSearchRoots.Count -eq 0) {
    $documents = [Environment]::GetFolderPath([Environment+SpecialFolder]::MyDocuments)
    $projectControlledRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot '..\..\..'))
    $FileSearchRoots = @(
        $projectControlledRoot,
        (Join-Path $documents 'Virtual Machines'),
        (Join-Path $env:USERPROFILE 'VirtualBox VMs'),
        (Join-Path $documents 'Hyper-V'),
        (Join-Path $env:PUBLIC 'Documents\Hyper-V')
    ) | Select-Object -Unique
}

$commands = @(
    'Get-VM', 'vmconnect.exe', 'vmrun.exe', 'VBoxManage.exe',
    'qemu-system-i386.exe', 'qemu-system-x86_64.exe', 'qemu-system-aarch64.exe',
    'WindowsSandbox.exe', 'mstsc.exe', 'Test-WSMan'
) | ForEach-Object { Get-C3CommandEvidence -Name $_ }

$hyperV = [ordered]@{ queriedOnce = $false; status = 'not-queried'; error = $null; virtualMachines = @() }
$vmware = [ordered]@{ queriedOnce = $false; status = 'tool-unavailable'; registrations = @() }
$virtualBox = [ordered]@{ queriedOnce = $false; status = 'tool-unavailable'; registrations = @() }
$sandbox = [ordered]@{
    executableAvailable = [bool](@($commands | Where-Object { $_.name -ceq 'WindowsSandbox.exe' -and $_.available }).Count)
    featureState = 'not-queried'
    featureQueryError = $null
    exactTargetQualification = $false
    historicalGuiAutomationPractical = $false
    assessment = 'Windows Sandbox cannot establish XP, Windows 7, or native ARM64 identity; the 27 interactive GUI workflows and screenshot review are not safely headless.'
}

if (-not $SkipPlatformQueries) {
    $getVm = Get-Command Get-VM -ErrorAction SilentlyContinue
    if ($null -ne $getVm) {
        $hyperV.queriedOnce = $true
        try {
            $hyperV.virtualMachines = @(Get-VM -ErrorAction Stop | ForEach-Object {
                [PSCustomObject]@{ name = $_.Name; state = [string]$_.State; generation = $_.Generation; path = $_.Path }
            })
            $hyperV.status = 'queried'
        }
        catch {
            $hyperV.status = 'inaccessible'
            $hyperV.error = $_.Exception.Message
        }
    }
    else { $hyperV.status = 'module-unavailable' }

    $vmrun = @($commands | Where-Object { $_.name -ceq 'vmrun.exe' -and $_.available })
    if ($vmrun.Count -eq 1) {
        $vmware.queriedOnce = $true
        try { $vmware.registrations = @(& $vmrun[0].path list 2>&1); $vmware.status = 'queried' }
        catch { $vmware.status = 'query-failed'; $vmware.registrations = @($_.Exception.Message) }
    }
    $vbox = @($commands | Where-Object { $_.name -ceq 'VBoxManage.exe' -and $_.available })
    if ($vbox.Count -eq 1) {
        $virtualBox.queriedOnce = $true
        try { $virtualBox.registrations = @(& $vbox[0].path list vms 2>&1); $virtualBox.status = 'queried' }
        catch { $virtualBox.status = 'query-failed'; $virtualBox.registrations = @($_.Exception.Message) }
    }
    $featureCommand = Get-Command Get-WindowsOptionalFeature -ErrorAction SilentlyContinue
    if ($null -ne $featureCommand) {
        try { $sandbox.featureState = [string](Get-WindowsOptionalFeature -Online -FeatureName Containers-DisposableClientVM -ErrorAction Stop).State }
        catch { $sandbox.featureState = 'inaccessible'; $sandbox.featureQueryError = $_.Exception.Message }
    }
}

$fileInventories = @($FileSearchRoots | ForEach-Object {
    Find-C3BoundedFiles -Root $_ -Extensions @('.vhd', '.vhdx', '.vmdk', '.vdi', '.qcow', '.qcow2', '.iso', '.rdp') `
        -MaximumDirectories $MaximumDirectoriesPerRoot -MaximumMatches $MaximumMatchesPerRoot
})
$imageFiles = @($fileInventories | ForEach-Object { $_.files } | Where-Object { [IO.Path]::GetExtension($_.path) -ne '.rdp' })
$rdpFiles = @($fileInventories | ForEach-Object { $_.files } | Where-Object { [IO.Path]::GetExtension($_.path) -eq '.rdp' })

$knownRdpServers = @()
try {
    if (Test-Path 'HKCU:\Software\Microsoft\Terminal Server Client\Servers') {
        $knownRdpServers = @(Get-ChildItem 'HKCU:\Software\Microsoft\Terminal Server Client\Servers' | Select-Object -ExpandProperty PSChildName)
    }
}
catch { }
$trustedHosts = $null
try { $trustedHosts = [string](Get-Item 'WSMan:\localhost\Client\TrustedHosts' -ErrorAction Stop).Value } catch { }

$os = Get-CimInstance Win32_OperatingSystem
$cpu = @(Get-CimInstance Win32_Processor | ForEach-Object { [PSCustomObject]@{ name = $_.Name; architecture = $_.Architecture } })
$nativeArm64 = ([string]$os.OSArchitecture -match 'ARM64' -and [string]$env:PROCESSOR_ARCHITECTURE -ceq 'ARM64')
$record = [ordered]@{
    schemaVersion = 1
    classification = 'private-local-inventory-not-runtime-qualification'
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
    repositoryCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    boundedPolicy = [ordered]@{
        eachPlatformQueriedAtMostOnce = $true
        networkRangeScanPerformed = $false
        mediaDownloaded = $false
        maximumDirectoriesPerRoot = $MaximumDirectoriesPerRoot
        maximumMatchesPerRoot = $MaximumMatchesPerRoot
    }
    host = [ordered]@{ os = $os.Caption; version = $os.Version; build = $os.BuildNumber; architecture = $os.OSArchitecture; processArchitecture = $env:PROCESSOR_ARCHITECTURE; processors = $cpu }
    commands = $commands
    hyperV = $hyperV
    vmware = $vmware
    virtualBox = $virtualBox
    qemuAvailable = [bool](@($commands | Where-Object { $_.name -like 'qemu-system-*' -and $_.available }).Count)
    windowsSandbox = $sandbox
    files = [ordered]@{ searches = $fileInventories; imageOrMediaCount = $imageFiles.Count; rdpFileCount = $rdpFiles.Count }
    knownRemoteTargets = [ordered]@{ rdpRegistryServers = $knownRdpServers; winRmTrustedHosts = $trustedHosts; probed = $false }
    nativeArm64 = [ordered]@{ localHostProvesNativeArm64 = $nativeArm64; reachableNativeArm64Proved = $nativeArm64; emulationAccepted = $false }
    conclusions = [ordered]@{
        exactXpTargetAvailable = $false
        exactWindows7TargetAvailable = $false
        exactNativeArm64TargetAvailable = $nativeArm64
        windowsSandboxMayProvideHistoricalObservationOnly = [bool]($sandbox.executableAvailable -and $sandbox.featureState -ceq 'Enabled')
        qualificationClaim = $false
    }
}
New-Item -ItemType Directory -Path (Split-Path -Parent $OutputPath) -Force | Out-Null
[IO.File]::WriteAllText($OutputPath, (($record | ConvertTo-Json -Depth 10) + "`n"), (New-Object Text.UTF8Encoding($false)))
Write-Host "Bounded Gate 1 environment inventory retained outside Git: $OutputPath"
return $record
