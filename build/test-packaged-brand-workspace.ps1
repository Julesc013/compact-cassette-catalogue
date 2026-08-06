[CmdletBinding()]
param(
    [string[]]$LaneId = @(),
    [switch]$SkipPackage,
    [switch]$KeepWork
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$packagesRoot = Join-Path $repositoryRoot 'artifacts\packages'
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$packageDefinitions = @(& (Join-Path $PSScriptRoot 'get-release-packages.ps1') `
    -Identity $identity)

if ($LaneId.Count -gt 0) {
    $requested = @($LaneId | Sort-Object -Unique)
    $unknown = @($requested | Where-Object {
            $candidate = $_
            @($packageDefinitions | Where-Object {
                    $_.LaneId -ceq $candidate
                }).Count -eq 0
        })
    if ($unknown.Count -gt 0) {
        throw "Unknown package lane(s): $($unknown -join ', ')"
    }

    $packageDefinitions = @($packageDefinitions | Where-Object {
            $candidate = $_.LaneId
            @($requested | Where-Object { $_ -ceq $candidate }).Count -gt 0
        })
}

if ($packageDefinitions.Count -eq 0) {
    throw 'No portable package lanes were selected.'
}

if (-not $SkipPackage) {
    & (Join-Path $PSScriptRoot 'package.ps1') -SkipBuild
}

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName Microsoft.VisualBasic
Add-Type -AssemblyName System.Drawing
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;

public static class C3PackagedBrandNativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(
        IntPtr window,
        uint message,
        IntPtr parameter,
        string text);

    [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
    public static extern IntPtr SendMessagePointer(
        IntPtr window,
        uint message,
        IntPtr parameter,
        IntPtr messageData);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PostMessage(
        IntPtr window,
        uint message,
        UIntPtr parameter,
        IntPtr messageData);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr window);

    [DllImport("user32.dll")]
    public static extern void keybd_event(
        byte virtualKey,
        byte scanCode,
        uint flags,
        UIntPtr extraInfo);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr window, out RECT rectangle);

    [DllImport("user32.dll")]
    public static extern bool PrintWindow(IntPtr window, IntPtr deviceContext, uint flags);
}
'@

$script:automationRoot = [System.Windows.Automation.AutomationElement]::RootElement
$script:processCondition = $null
$script:activeProcess = $null
$script:success = $false
$script:catalogues = @{}
$script:payloads = @{}
$script:performance = @{}
$temporaryBase = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$workRoot = Join-Path $temporaryBase ("C3-packaged-brand-workflow-{0}" -f [Guid]::NewGuid().ToString('N'))
[void](New-Item -ItemType Directory -Path $workRoot)

function New-PropertyCondition {
    param(
        [System.Windows.Automation.AutomationProperty]$Property,
        [object]$Value
    )

    return New-Object System.Windows.Automation.PropertyCondition($Property, $Value)
}

function Wait-C3Element {
    param(
        [string]$AutomationId,
        [string]$Name,
        [System.Windows.Automation.AutomationElement]$Within,
        [int]$TimeoutSeconds = 15,
        [System.Windows.Automation.ControlType]$ControlType
    )

    $conditions = New-Object Collections.Generic.List[System.Windows.Automation.Condition]
    if ($null -eq $Within) {
        $conditions.Add($script:processCondition)
    }
    if (-not [string]::IsNullOrWhiteSpace($AutomationId)) {
        $conditions.Add((New-PropertyCondition `
                    -Property ([System.Windows.Automation.AutomationElement]::AutomationIdProperty) `
                    -Value $AutomationId))
    }
    if (-not [string]::IsNullOrWhiteSpace($Name)) {
        $conditions.Add((New-PropertyCondition `
                    -Property ([System.Windows.Automation.AutomationElement]::NameProperty) `
                    -Value $Name))
    }
    if ($null -ne $ControlType) {
        $conditions.Add((New-PropertyCondition `
                    -Property ([System.Windows.Automation.AutomationElement]::ControlTypeProperty) `
                    -Value $ControlType))
    }
    if ($conditions.Count -eq 0) {
        throw 'Wait-C3Element requires an automation ID or name.'
    }

    $condition = if ($conditions.Count -eq 1) {
        $conditions[0]
    }
    else {
        New-Object System.Windows.Automation.AndCondition($conditions.ToArray())
    }
    $scope = if ($null -eq $Within) {
        [System.Windows.Automation.TreeScope]::Descendants
    }
    else {
        [System.Windows.Automation.TreeScope]::Descendants
    }
    $root = if ($null -eq $Within) { $script:automationRoot } else { $Within }
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)

    do {
        $element = $root.FindFirst($scope, $condition)
        if ($null -ne $element) {
            return $element
        }
        if ($null -ne $script:activeProcess) {
            $script:activeProcess.Refresh()
            if ($script:activeProcess.HasExited) {
                throw "C3 exited before '$AutomationId$Name' appeared (exit $($script:activeProcess.ExitCode))."
            }
        }
        Start-Sleep -Milliseconds 100
    }
    while ([DateTime]::UtcNow -lt $deadline)

    throw "Timed out waiting for C3 element: automationId='$AutomationId', name='$Name'."
}

function Wait-C3ElementAbsent {
    param(
        [string]$AutomationId,
        [string]$Name,
        [int]$TimeoutSeconds = 15
    )

    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $conditions = @($script:processCondition)
        if (-not [string]::IsNullOrWhiteSpace($AutomationId)) {
            $conditions += New-PropertyCondition `
                -Property ([System.Windows.Automation.AutomationElement]::AutomationIdProperty) `
                -Value $AutomationId
        }
        if (-not [string]::IsNullOrWhiteSpace($Name)) {
            $conditions += New-PropertyCondition `
                -Property ([System.Windows.Automation.AutomationElement]::NameProperty) `
                -Value $Name
        }
        $condition = New-Object System.Windows.Automation.AndCondition($conditions)
        $element = $script:automationRoot.FindFirst(
            [System.Windows.Automation.TreeScope]::Descendants,
            $condition)
        if ($null -eq $element) {
            return
        }
        Start-Sleep -Milliseconds 100
    }
    while ([DateTime]::UtcNow -lt $deadline)

    throw "C3 element remained present: automationId='$AutomationId', name='$Name'."
}

function Wait-C3ValueElementByName {
    param(
        [System.Windows.Automation.AutomationElement]$Within,
        [string]$Name,
        [int]$TimeoutSeconds = 15
    )

    $condition = New-PropertyCondition `
        -Property ([System.Windows.Automation.AutomationElement]::NameProperty) `
        -Value $Name
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $candidates = $Within.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $condition)
        for ($index = 0; $index -lt $candidates.Count; $index++) {
            $candidate = $candidates.Item($index)
            $supported = @($candidate.GetSupportedPatterns() | Where-Object {
                    $_ -eq [System.Windows.Automation.ValuePattern]::Pattern
                })
            if ($supported.Count -eq 1) {
                $valuePattern = $candidate.GetCurrentPattern(
                    [System.Windows.Automation.ValuePattern]::Pattern)
                if (-not $valuePattern.Current.IsReadOnly) {
                    return $candidate
                }
            }
        }
        Start-Sleep -Milliseconds 100
    }
    while ([DateTime]::UtcNow -lt $deadline)

    throw "Timed out waiting for a writable Value element named '$Name'."
}

function Invoke-C3Element {
    param([System.Windows.Automation.AutomationElement]$Element)

    $automationId = $Element.Current.AutomationId
    $name = $Element.Current.Name
    $controlType = $Element.Current.ControlType.ProgrammaticName
    $lastFailure = $null

    for ($attempt = 1; $attempt -le 3; $attempt++) {
        try {
            if (-not $Element.Current.IsEnabled) {
                throw 'element is disabled'
            }
            $pattern = $Element.GetCurrentPattern(
                [System.Windows.Automation.InvokePattern]::Pattern)
            $pattern.Invoke()
            Start-Sleep -Milliseconds 200
            return
        }
        catch {
            $lastFailure = $_
            Start-Sleep -Milliseconds 150
        }
    }

    $window = [IntPtr]$Element.Current.NativeWindowHandle
    if ($window -ne [IntPtr]::Zero) {
        [void][C3PackagedBrandNativeMethods]::SendMessagePointer(
            $window,
            0x00F5,
            [IntPtr]::Zero,
            [IntPtr]::Zero)
        Start-Sleep -Milliseconds 200
        return
    }

    throw "C3 element '$automationId' ('$name', $controlType) could not be invoked after three attempts and has no native button handle: $lastFailure"
}

function Set-C3Text {
    param(
        [System.Windows.Automation.AutomationElement]$Element,
        [string]$Value
    )

    $window = [IntPtr]$Element.Current.NativeWindowHandle
    if ($window -eq [IntPtr]::Zero) {
        throw "C3 text element '$($Element.Current.AutomationId)' has no native window handle."
    }
    [void][C3PackagedBrandNativeMethods]::SendMessage(
        $window,
        0x000C,
        [IntPtr]::Zero,
        $Value)
    Start-Sleep -Milliseconds 100
}

function Set-C3Value {
    param(
        [System.Windows.Automation.AutomationElement]$Element,
        [string]$Value
    )

    try {
        $pattern = $Element.GetCurrentPattern(
            [System.Windows.Automation.ValuePattern]::Pattern)
    }
    catch {
        throw "C3 element '$($Element.Current.AutomationId)' ('$($Element.Current.Name)', $($Element.Current.ControlType.ProgrammaticName)) does not support Value: $($_.Exception.Message)"
    }
    $pattern.SetValue($Value)
    Start-Sleep -Milliseconds 100
}

function Select-C3BrandRow {
    param(
        [System.Windows.Automation.AutomationElement]$BrandWindow,
        [string]$Code
    )

    $nameCondition = New-PropertyCondition `
        -Property ([System.Windows.Automation.AutomationElement]::NameProperty) `
        -Value $Code
    $typeCondition = New-PropertyCondition `
        -Property ([System.Windows.Automation.AutomationElement]::ControlTypeProperty) `
        -Value ([System.Windows.Automation.ControlType]::DataItem)
    $condition = New-Object System.Windows.Automation.AndCondition(
        $nameCondition,
        $typeCondition)
    $row = $BrandWindow.FindFirst(
        [System.Windows.Automation.TreeScope]::Descendants,
        $condition)
    if ($null -eq $row) {
        throw "Brand row '$Code' was not found."
    }

    try {
        $selection = $row.GetCurrentPattern(
            [System.Windows.Automation.SelectionItemPattern]::Pattern)
    }
    catch {
        throw "Brand row '$Code' does not support SelectionItem: $($_.Exception.Message)"
    }
    $selection.Select()
    Start-Sleep -Milliseconds 250
}

function Assert-C3BrandRow {
    param(
        [System.Windows.Automation.AutomationElement]$BrandWindow,
        [string]$Code,
        [string]$ExpectedName,
        [int]$TimeoutSeconds = 10
    )

    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        try {
            Select-C3BrandRow -BrandWindow $BrandWindow -Code $Code
            $texts = $BrandWindow.FindAll(
                [System.Windows.Automation.TreeScope]::Descendants,
                (New-PropertyCondition `
                        -Property ([System.Windows.Automation.AutomationElement]::NameProperty) `
                        -Value $ExpectedName))
            if ($texts.Count -gt 0) {
                return
            }
        }
        catch {
            # The form rebuilds ListView rows after mutations; retry until stable.
        }
        Start-Sleep -Milliseconds 100
    }
    while ([DateTime]::UtcNow -lt $deadline)

    throw "Brand '$Code' did not present the expected name '$ExpectedName'."
}

function Assert-C3BrandCount {
    param(
        [System.Windows.Automation.AutomationElement]$BrandWindow,
        [int]$Count,
        [int]$TimeoutSeconds = 10
    )

    $list = Wait-C3Element `
        -AutomationId 'brandListView' `
        -Name 'Brands' `
        -Within $BrandWindow `
        -TimeoutSeconds 10
    $rowCondition = New-PropertyCondition `
        -Property ([System.Windows.Automation.AutomationElement]::ControlTypeProperty) `
        -Value ([System.Windows.Automation.ControlType]::DataItem)
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $rows = $list.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $rowCondition)
        if ($rows.Count -eq $Count) {
            return
        }
        Start-Sleep -Milliseconds 100
    }
    while ([DateTime]::UtcNow -lt $deadline)

    throw "Brand list did not contain the expected $Count row(s)."
}

function Measure-C3WindowPaint {
    param([System.Windows.Automation.AutomationElement]$Window)

    $handle = [IntPtr]$Window.Current.NativeWindowHandle
    if ($handle -eq [IntPtr]::Zero) {
        throw "C3 window '$($Window.Current.Name)' has no native handle for paint measurement."
    }
    $rectangle = New-Object C3PackagedBrandNativeMethods+RECT
    if (-not [C3PackagedBrandNativeMethods]::GetWindowRect($handle, [ref]$rectangle)) {
        throw "Could not read bounds for C3 window '$($Window.Current.Name)'."
    }
    $width = $rectangle.Right - $rectangle.Left
    $height = $rectangle.Bottom - $rectangle.Top
    if ($width -le 0 -or $height -le 0) {
        throw "C3 window '$($Window.Current.Name)' has invalid paint bounds $width by $height."
    }

    $bitmap = New-Object Drawing.Bitmap($width, $height)
    $graphics = [Drawing.Graphics]::FromImage($bitmap)
    $deviceContext = [IntPtr]::Zero
    try {
        $deviceContext = $graphics.GetHdc()
        $stopwatch = [Diagnostics.Stopwatch]::StartNew()
        $painted = [C3PackagedBrandNativeMethods]::PrintWindow(
            $handle,
            $deviceContext,
            0)
        $stopwatch.Stop()
        if (-not $painted) {
            throw "PrintWindow failed for '$($Window.Current.Name)'."
        }
        return [Math]::Round($stopwatch.Elapsed.TotalMilliseconds, 3)
    }
    finally {
        if ($deviceContext -ne [IntPtr]::Zero) {
            $graphics.ReleaseHdc($deviceContext)
        }
        $graphics.Dispose()
        $bitmap.Dispose()
    }
}

function Start-C3Package {
    param([string]$Executable)

    $startup = [Diagnostics.Stopwatch]::StartNew()
    $process = Start-Process `
        -FilePath $Executable `
        -WorkingDirectory (Split-Path -Parent $Executable) `
        -PassThru
    $deadline = [DateTime]::UtcNow.AddSeconds(30)
    while ($process.MainWindowHandle -eq [IntPtr]::Zero -and
            [DateTime]::UtcNow -lt $deadline) {
        Start-Sleep -Milliseconds 200
        $process.Refresh()
        if ($process.HasExited) {
            throw "Packaged C3 exited during startup with code $($process.ExitCode)."
        }
    }
    if ($process.MainWindowHandle -eq [IntPtr]::Zero) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        throw 'Packaged C3 did not create its main window.'
    }

    $script:activeProcess = $process
    $script:processCondition = New-PropertyCondition `
        -Property ([System.Windows.Automation.AutomationElement]::ProcessIdProperty) `
        -Value $process.Id
    Start-Sleep -Seconds 2
    $startup.Stop()
    $process | Add-Member `
        -MemberType NoteProperty `
        -Name C3StartupMilliseconds `
        -Value ([Math]::Round($startup.Elapsed.TotalMilliseconds, 3))
    return $process
}

function Stop-C3Package {
    param([Diagnostics.Process]$Process)

    if ($null -eq $Process) {
        return
    }
    $Process.Refresh()
    if (-not $Process.HasExited) {
        [void]$Process.CloseMainWindow()
        [void]$Process.WaitForExit(5000)
    }
    $Process.Refresh()
    if (-not $Process.HasExited) {
        Stop-Process -Id $Process.Id -Force
        [void]$Process.WaitForExit(5000)
    }
    $Process.Dispose()
    $script:activeProcess = $null
    $script:processCondition = $null
}

function Send-C3Shortcut {
    param(
        [Diagnostics.Process]$Process,
        [uint32]$VirtualKey
    )

    $Process.Refresh()
    $mainWindow = [System.Windows.Automation.AutomationElement]::FromHandle(
        $Process.MainWindowHandle)
    if ($null -eq $mainWindow) {
        throw 'The packaged C3 main window did not expose an automation element.'
    }
    $windowPattern = $mainWindow.GetCurrentPattern(
        [System.Windows.Automation.WindowPattern]::Pattern)
    $windowPattern.SetWindowVisualState(
        [System.Windows.Automation.WindowVisualState]::Normal)
    $focusAnchor = Wait-C3Element `
        -AutomationId 'btnAdd' `
        -Name '' `
        -Within $mainWindow `
        -TimeoutSeconds 30
    $focusAnchor.SetFocus()
    Start-Sleep -Milliseconds 100

    $window = $Process.MainWindowHandle
    $keyParameter = [UIntPtr]::new([uint64]$VirtualKey)
    if (-not [C3PackagedBrandNativeMethods]::PostMessage(
            $window,
            0x0100,
            $keyParameter,
            [IntPtr]::Zero)) {
        throw "Unable to post key-down 0x$($VirtualKey.ToString('X2')) to packaged C3 (Win32 $([Runtime.InteropServices.Marshal]::GetLastWin32Error()))."
    }
    if (-not [C3PackagedBrandNativeMethods]::PostMessage(
            $window,
            0x0101,
            $keyParameter,
            [IntPtr]::Zero)) {
        throw "Unable to post key-up 0x$($VirtualKey.ToString('X2')) to packaged C3 (Win32 $([Runtime.InteropServices.Marshal]::GetLastWin32Error()))."
    }
    Start-Sleep -Milliseconds 300
}

function Send-C3ModifiedShortcut {
    param(
        [Diagnostics.Process]$Process,
        [byte]$ModifierVirtualKey,
        [byte]$VirtualKey
    )

    $Process.Refresh()
    $window = $Process.MainWindowHandle
    if ($window -eq [IntPtr]::Zero) {
        throw 'The packaged C3 process has no main window for a modified shortcut.'
    }
    if (-not [C3PackagedBrandNativeMethods]::SetForegroundWindow($window)) {
        throw "Unable to foreground packaged C3 for shortcut input (Win32 $([Runtime.InteropServices.Marshal]::GetLastWin32Error()))."
    }

    Start-Sleep -Milliseconds 200
    [C3PackagedBrandNativeMethods]::keybd_event($ModifierVirtualKey, 0, 0, [UIntPtr]::Zero)
    [C3PackagedBrandNativeMethods]::keybd_event($VirtualKey, 0, 0, [UIntPtr]::Zero)
    [C3PackagedBrandNativeMethods]::keybd_event($VirtualKey, 0, 2, [UIntPtr]::Zero)
    [C3PackagedBrandNativeMethods]::keybd_event($ModifierVirtualKey, 0, 2, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 300
}

function Close-C3Window {
    param([System.Windows.Automation.AutomationElement]$Window)

    try {
        $pattern = $Window.GetCurrentPattern(
            [System.Windows.Automation.WindowPattern]::Pattern)
    }
    catch {
        throw "C3 window '$($Window.Current.AutomationId)' ('$($Window.Current.Name)', $($Window.Current.ControlType.ProgrammaticName)) does not support Window: $($_.Exception.Message)"
    }
    $pattern.Close()
    Start-Sleep -Milliseconds 300
}

function Open-C3BrandWindow {
    param([Diagnostics.Process]$Process)

    $lastFailure = $null
    for ($attempt = 1; $attempt -le 5; $attempt++) {
        try {
            Send-C3Shortcut -Process $Process -VirtualKey 0x75
            return Wait-C3Element `
                -AutomationId 'BrandWorkspaceForm' `
                -Name 'Brands - C3' `
                -Within $null `
                -TimeoutSeconds 3
        }
        catch {
            $lastFailure = $_
            Start-Sleep -Milliseconds 500
        }
    }

    throw "Packaged C3 did not open Brands after five F6 attempts: $lastFailure"
}

function Set-FileDialogPathAndAccept {
    param(
        [string]$DialogName,
        [string]$Path,
        [string]$AcceptName
    )

    $dialog = Wait-C3Element `
        -AutomationId '' `
        -Name $DialogName `
        -Within $null `
        -TimeoutSeconds 15
    $fileName = Wait-C3ValueElementByName `
        -Within $dialog `
        -Name 'File name:' `
        -TimeoutSeconds 10
    Set-C3Value -Element $fileName -Value $Path
    $accept = Wait-C3Element `
        -AutomationId '1' `
        -Name $AcceptName `
        -Within $dialog `
        -TimeoutSeconds 10
    Invoke-C3Element $accept
    Wait-C3ElementAbsent -AutomationId '' -Name $DialogName -TimeoutSeconds 20
}

function Invoke-BrandMutationAndSave {
    param(
        [string]$Lane,
        [string]$Executable,
        [string]$CataloguePath,
        [string]$Code,
        [string]$OriginalName,
        [string]$FinalName
    )

    $process = $null
    try {
        $process = Start-C3Package -Executable $Executable
        $brandOpen = [Diagnostics.Stopwatch]::StartNew()
        $brandWindow = Open-C3BrandWindow -Process $process
        $brandOpen.Stop()
        $interactionDurations = New-Object Collections.Generic.List[Double]

        Invoke-C3Element (Wait-C3Element 'newButton' '' $brandWindow)
        Set-C3Text (Wait-C3Element 'brandNameTextBox' '' $brandWindow) $OriginalName
        Set-C3Text (Wait-C3Element 'brandCodeTextBox' '' $brandWindow) $Code
        Set-C3Text `
            (Wait-C3Element 'brandNotesTextBox' '' $brandWindow) `
            "Created through the exact $Lane portable package."
        $interaction = [Diagnostics.Stopwatch]::StartNew()
        Invoke-C3Element (Wait-C3Element 'applyButton' '' $brandWindow)
        Assert-C3BrandRow $brandWindow $Code $OriginalName
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=create|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))

        Invoke-C3Element (Wait-C3Element 'editButton' '' $brandWindow)
        Set-C3Text (Wait-C3Element 'brandNameTextBox' '' $brandWindow) $FinalName
        Set-C3Text `
            (Wait-C3Element 'brandNotesTextBox' '' $brandWindow) `
            'Edited through the packaged OEM+ workspace.'
        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element 'applyButton' '' $brandWindow)
        Assert-C3BrandRow $brandWindow $Code $FinalName
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=edit|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))

        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element 'undoButton' '' $brandWindow)
        Assert-C3BrandRow $brandWindow $Code $OriginalName
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=undo-edit|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))
        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element 'redoButton' '' $brandWindow)
        Assert-C3BrandRow $brandWindow $Code $FinalName
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=redo-edit|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))

        Set-C3Text `
            (Wait-C3Element 'filterTextBox' '' $brandWindow) `
            'no packaged brand matches this filter'
        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element 'applyFilterButton' '' $brandWindow)
        Assert-C3BrandCount $brandWindow 0
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=apply-filter|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))
        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element 'clearFilterButton' '' $brandWindow)
        Assert-C3BrandCount $brandWindow 1
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=clear-filter|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))

        Select-C3BrandRow $brandWindow $Code
        Invoke-C3Element (Wait-C3Element 'deleteButton' '' $brandWindow)
        $deleteDialog = Wait-C3Element '' 'Delete brand' $null 10
        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element '6' 'Yes' $deleteDialog 10)
        Assert-C3BrandCount $brandWindow 0
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=confirm-delete|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))
        Wait-C3ElementAbsent '' 'Delete brand' 10
        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element 'undoButton' '' $brandWindow)
        Assert-C3BrandRow $brandWindow $Code $FinalName
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=undo-delete|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))
        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element 'redoButton' '' $brandWindow)
        Assert-C3BrandCount $brandWindow 0
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=redo-delete|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))
        $interaction.Restart()
        Invoke-C3Element (Wait-C3Element 'undoButton' '' $brandWindow)
        Assert-C3BrandRow $brandWindow $Code $FinalName
        $interaction.Stop()
        $interactionDurations.Add($interaction.Elapsed.TotalMilliseconds)
        Write-Host ('PACKAGED_BRAND_COMMAND|lane={0}|operation=final-undo-delete|elapsed-ms={1}' -f $Lane, [Math]::Round($interaction.Elapsed.TotalMilliseconds, 3))

        $paintMilliseconds = Measure-C3WindowPaint -Window $brandWindow
        $process.Refresh()
        $peakWorkingSet = [Int64]$process.PeakWorkingSet64
        $maximumInteraction = [Math]::Round(
            ($interactionDurations | Measure-Object -Maximum).Maximum,
            3)
        $script:performance[$Lane] = [PSCustomObject]@{
            StartupMilliseconds = $process.C3StartupMilliseconds
            BrandOpenMilliseconds = [Math]::Round(
                $brandOpen.Elapsed.TotalMilliseconds,
                3)
            MaximumInteractionMilliseconds = $maximumInteraction
            PaintMilliseconds = $paintMilliseconds
            PeakWorkingSetBytes = $peakWorkingSet
        }
        if ($process.C3StartupMilliseconds -gt 30000 -or
                $brandOpen.Elapsed.TotalMilliseconds -gt 15000 -or
                $maximumInteraction -gt 5000 -or
                $paintMilliseconds -gt 5000 -or
                $peakWorkingSet -gt 536870912) {
            throw (
                "Packaged Brand performance exceeded a conservative safety ceiling in '{0}': startup-ms={1}, brand-open-ms={2}, interaction-max-ms={3}, paint-ms={4}, peak-working-set-bytes={5}." -f `
                    $Lane,
                    $process.C3StartupMilliseconds,
                    [Math]::Round($brandOpen.Elapsed.TotalMilliseconds, 3),
                    $maximumInteraction,
                    $paintMilliseconds,
                    $peakWorkingSet)
        }

        Close-C3Window $brandWindow
        Wait-C3ElementAbsent 'BrandWorkspaceForm' 'Brands - C3' 10
        Invoke-C3Element (Wait-C3Element 'btnSave' '' $null 10)
        Set-FileDialogPathAndAccept `
            -DialogName 'Save Catalogue As' `
            -Path $CataloguePath `
            -AcceptName 'Save'

        $deadline = [DateTime]::UtcNow.AddSeconds(15)
        while (-not (Test-Path -LiteralPath $CataloguePath -PathType Leaf) -and
                [DateTime]::UtcNow -lt $deadline) {
            Start-Sleep -Milliseconds 100
        }
        if (-not (Test-Path -LiteralPath $CataloguePath -PathType Leaf)) {
            throw "Packaged C3 did not create '$CataloguePath'."
        }
        $catalogueText = Get-Content -LiteralPath $CataloguePath -Raw
        if ($catalogueText -notmatch [Regex]::Escape($FinalName)) {
            throw "Saved catalogue does not contain '$FinalName'."
        }
    }
    finally {
        Stop-C3Package -Process $process
    }
}

function Assert-BrandCatalogueReopens {
    param(
        [string]$TargetLane,
        [string]$SourceLane,
        [string]$Executable,
        [string]$CataloguePath,
        [string]$Code,
        [string]$ExpectedName
    )

    $process = $null
    try {
        $process = Start-C3Package -Executable $Executable
        Send-C3ModifiedShortcut `
            -Process $process `
            -ModifierVirtualKey 0x11 `
            -VirtualKey 0x4F
        Set-FileDialogPathAndAccept `
            -DialogName 'Open Catalogue' `
            -Path $CataloguePath `
            -AcceptName 'Open'
        $brandWindow = Open-C3BrandWindow -Process $process
        Assert-C3BrandRow $brandWindow $Code $ExpectedName
        Write-Host (
            'PACKAGED_BRAND_REOPEN|source={0}|target={1}|result=pass' -f `
                $SourceLane,
                $TargetLane)
        Close-C3Window $brandWindow
    }
    finally {
        Stop-C3Package -Process $process
    }
}

try {
    Write-Host "Packaged Brand workflow temporary root: $workRoot"
    foreach ($definition in $packageDefinitions) {
        $packagePath = Join-Path $packagesRoot $definition.FileName
        if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
            throw "Release package is missing: $packagePath"
        }

        $extractRoot = Join-Path $workRoot ("payload-" + $definition.LaneId)
        [void](New-Item -ItemType Directory -Path $extractRoot)
        Expand-Archive -LiteralPath $packagePath -DestinationPath $extractRoot
        $payloadRoots = @(Get-ChildItem -LiteralPath $extractRoot -Directory)
        if ($payloadRoots.Count -ne 1) {
            throw "Package '$($definition.FileName)' did not contain one rooted payload."
        }
        $executable = Join-Path $payloadRoots[0].FullName 'Compact Cassette Catalogue.exe'
        if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
            throw "Package '$($definition.FileName)' is missing the Desktop executable."
        }
        $script:payloads[$definition.LaneId] = [PSCustomObject]@{
            Executable = $executable
            PackagePath = $packagePath
            PackageHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $packagePath).Hash.ToLowerInvariant()
            ExecutableHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $executable).Hash.ToLowerInvariant()
        }
    }

    $index = 0
    foreach ($definition in $packageDefinitions) {
        $index++
        $code = if ($index -eq 1) { 'PF' } else { 'PS' }
        $originalName = "Package $($definition.LaneId) Brand"
        $finalName = "$originalName Edited"
        $cataloguePath = Join-Path $workRoot ("$($definition.LaneId)-brands.xml")
        Invoke-BrandMutationAndSave `
            -Lane $definition.LaneId `
            -Executable $script:payloads[$definition.LaneId].Executable `
            -CataloguePath $cataloguePath `
            -Code $code `
            -OriginalName $originalName `
            -FinalName $finalName
        $script:catalogues[$definition.LaneId] = [PSCustomObject]@{
            Path = $cataloguePath
            Code = $code
            Name = $finalName
            Hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $cataloguePath).Hash.ToLowerInvariant()
        }
        Write-Host (
            'PACKAGED_BRAND_MUTATION|lane={0}|package-sha256={1}|exe-sha256={2}|catalogue-sha256={3}|result=pass' -f `
                $definition.LaneId,
                $script:payloads[$definition.LaneId].PackageHash,
                $script:payloads[$definition.LaneId].ExecutableHash,
                $script:catalogues[$definition.LaneId].Hash)
        $measurement = $script:performance[$definition.LaneId]
        Write-Host (
            'PACKAGED_BRAND_PERFORMANCE|lane={0}|startup-ms={1}|brand-open-ms={2}|interaction-max-ms={3}|paint-ms={4}|peak-working-set-bytes={5}' -f `
                $definition.LaneId,
                $measurement.StartupMilliseconds,
                $measurement.BrandOpenMilliseconds,
                $measurement.MaximumInteractionMilliseconds,
                $measurement.PaintMilliseconds,
                $measurement.PeakWorkingSetBytes)
    }

    foreach ($source in $packageDefinitions) {
        foreach ($target in $packageDefinitions) {
            $catalogue = $script:catalogues[$source.LaneId]
            Assert-BrandCatalogueReopens `
                -TargetLane $target.LaneId `
                -SourceLane $source.LaneId `
                -Executable $script:payloads[$target.LaneId].Executable `
                -CataloguePath $catalogue.Path `
                -Code $catalogue.Code `
                -ExpectedName $catalogue.Name
        }
    }

    $script:success = $true
    Write-Host (
        'Packaged Brand workflow passed: {0} mutation lane(s), {1} reopen matrix row(s).' -f `
            $packageDefinitions.Count,
            ($packageDefinitions.Count * $packageDefinitions.Count))
}
finally {
    if ($script:success -and -not $KeepWork) {
        $resolvedWorkRoot = [IO.Path]::GetFullPath($workRoot)
        $expectedPrefix = Join-Path $temporaryBase 'C3-packaged-brand-workflow-'
        if (-not $resolvedWorkRoot.StartsWith(
                $expectedPrefix,
                [StringComparison]::OrdinalIgnoreCase)) {
            throw "Refusing to remove unexpected workflow path: $resolvedWorkRoot"
        }
        Remove-Item -LiteralPath $resolvedWorkRoot -Recurse -Force
    }
    elseif (Test-Path -LiteralPath $workRoot -PathType Container) {
        Write-Host "Packaged Brand workflow evidence retained at: $workRoot"
    }
}
