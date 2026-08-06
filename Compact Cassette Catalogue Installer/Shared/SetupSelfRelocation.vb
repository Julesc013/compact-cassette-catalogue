Imports System.Diagnostics
Imports System.IO
Imports System.Runtime.InteropServices

Namespace Global.C3Setup

    Public Interface ISetupProcessLauncher
        Sub Start(executablePath As String, arguments As String, workingDirectory As String)
    End Interface

    Public NotInheritable Class WindowsSetupProcessLauncher
        Implements ISetupProcessLauncher

        Public Sub Start(executablePath As String, arguments As String, workingDirectory As String) Implements ISetupProcessLauncher.Start
            Dim startInfo As New ProcessStartInfo(executablePath, arguments)
            startInfo.WorkingDirectory = workingDirectory
            startInfo.UseShellExecute = False
            Dim child As Process = Process.Start(startInfo)
            If child Is Nothing Then Throw New SetupContractException("Windows did not start the relocated uninstaller.")
            child.Dispose()
        End Sub
    End Class

    Public NotInheritable Class SetupRelocationContext

        Public Sub New(installRoot As String, statePath As String, relocationRoot As String, executablePath As String)
            Me.InstallRoot = installRoot
            Me.StatePath = statePath
            Me.RelocationRoot = relocationRoot
            Me.ExecutablePath = executablePath
        End Sub

        Public ReadOnly Property InstallRoot As String
        Public ReadOnly Property StatePath As String
        Public ReadOnly Property RelocationRoot As String
        Public ReadOnly Property ExecutablePath As String
    End Class

    Public NotInheritable Class SetupSelfRelocation

        Private Const UninstallerFileName As String = "UNINSTALL.exe"
        Private Const MoveFileDelayUntilReboot As Integer = 4

        Private Sub New()
        End Sub

        Public Shared Function PrepareAndLaunch(currentExecutablePath As String,
                                                statePath As String,
                                                temporaryBasePath As String,
                                                launcher As ISetupProcessLauncher) As SetupRelocationContext
            If launcher Is Nothing Then Throw New ArgumentNullException("launcher")
            Dim installedExecutable As String = CanonicalFile(currentExecutablePath)
            Dim installedStatePath As String = CanonicalFile(statePath)
            If Not String.Equals(Path.GetFileName(installedExecutable), UninstallerFileName, StringComparison.OrdinalIgnoreCase) OrElse
                    Not String.Equals(Path.GetFileName(installedStatePath), InstalledStateCodec.FileName, StringComparison.Ordinal) Then
                Throw New SetupContractException("Self-relocation accepts only the installed C3 uninstaller and state manifest.")
            End If
            Dim installRoot As String = SetupPathPolicy.ValidateInstallRoot(Path.GetDirectoryName(installedStatePath))
            If Not String.Equals(installedExecutable, Path.Combine(installRoot, UninstallerFileName), StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("The uninstaller is not the owned executable beside C3.installed.xml.")
            End If
            Dim installedConfig As String = installedExecutable & ".config"
            If Not File.Exists(installedExecutable) OrElse Not File.Exists(installedConfig) OrElse Not File.Exists(installedStatePath) Then
                Throw New SetupContractException("The installed uninstaller, configuration, and state manifest are required for relocation.")
            End If

            Dim temporaryBase As String = SetupPathPolicy.CanonicalDirectory(temporaryBasePath)
            If Not Directory.Exists(temporaryBase) Then Throw New SetupContractException("The relocation temporary base does not exist.")
            Dim relocationRoot As String = Path.Combine(temporaryBase, "C3-Uninstall-" & Guid.NewGuid().ToString("N"))
            RequireDirectChild(temporaryBase, relocationRoot)
            Directory.CreateDirectory(relocationRoot)
            Dim relocatedExecutable As String = Path.Combine(relocationRoot, UninstallerFileName)
            Dim relocatedConfig As String = relocatedExecutable & ".config"
            Try
                File.Copy(installedExecutable, relocatedExecutable, False)
                File.Copy(installedConfig, relocatedConfig, False)
                RequireSameFile(installedExecutable, relocatedExecutable)
                RequireSameFile(installedConfig, relocatedConfig)
                launcher.Start(relocatedExecutable,
                               "--state " & QuoteArgument(installedStatePath) & " --relocation-root " & QuoteArgument(relocationRoot),
                               relocationRoot)
                Return New SetupRelocationContext(installRoot, installedStatePath, relocationRoot, relocatedExecutable)
            Catch
                If Directory.Exists(relocationRoot) Then Directory.Delete(relocationRoot, True)
                Throw
            End Try
        End Function

        Public Shared Function ValidateRelocatedInvocation(arguments As String(),
                                                           currentExecutablePath As String) As SetupRelocationContext
            If arguments Is Nothing OrElse arguments.Length <> 4 OrElse
                    arguments(0) <> "--state" OrElse arguments(2) <> "--relocation-root" Then
                Throw New SetupContractException("The relocated uninstaller requires an exact state and relocation-root invocation.")
            End If
            Dim statePath As String = CanonicalFile(arguments(1))
            Dim relocationRoot As String = SetupPathPolicy.CanonicalDirectory(arguments(3))
            Dim executablePath As String = CanonicalFile(currentExecutablePath)
            RequireDirectChild(Path.GetDirectoryName(relocationRoot), relocationRoot)
            If Not String.Equals(executablePath, Path.Combine(relocationRoot, UninstallerFileName), StringComparison.OrdinalIgnoreCase) OrElse
                    Not String.Equals(Path.GetFileName(statePath), InstalledStateCodec.FileName, StringComparison.Ordinal) Then
                Throw New SetupContractException("The relocated process identity does not match its invocation.")
            End If
            Dim installRoot As String = SetupPathPolicy.ValidateInstallRoot(Path.GetDirectoryName(statePath))
            If String.Equals(installRoot, relocationRoot, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("The relocation root must be outside the installed product root.")
            End If
            Dim state As InstalledState = InstalledStateCodec.Read(statePath)
            If Not String.Equals(state.InstallRoot, installRoot, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("Relocation state does not own its containing install root.")
            End If
            RequireMatchesOwnedFile(state, UninstallerFileName, executablePath)
            RequireMatchesOwnedFile(state, UninstallerFileName & ".config", executablePath & ".config")
            Return New SetupRelocationContext(installRoot, statePath, relocationRoot, executablePath)
        End Function

        Public Shared Sub ScheduleCleanupAfterExit(context As SetupRelocationContext)
            If context Is Nothing Then Throw New ArgumentNullException("context")
            Dim validated As SetupRelocationContext = ValidateRelocatedInvocation(
                New String() {"--state", context.StatePath, "--relocation-root", context.RelocationRoot},
                context.ExecutablePath)
            ScheduleDelete(validated.ExecutablePath & ".config")
            ScheduleDelete(validated.ExecutablePath)
            ScheduleDelete(validated.RelocationRoot)
        End Sub

        Private Shared Sub RequireMatchesOwnedFile(state As InstalledState, name As String, actualPath As String)
            Dim owned As PayloadFile = Nothing
            For Each item As PayloadFile In state.Manifest.Files
                If String.Equals(item.Path, name, StringComparison.Ordinal) Then
                    owned = item
                    Exit For
                End If
            Next
            If owned Is Nothing OrElse Not File.Exists(actualPath) OrElse
                    New FileInfo(actualPath).Length <> owned.Length OrElse
                    FileHash.Sha256(actualPath) <> owned.Sha256 Then
                Throw New SetupContractException("The relocated byte does not match installed ownership: " & name)
            End If
        End Sub

        Private Shared Sub RequireSameFile(source As String, destination As String)
            If New FileInfo(source).Length <> New FileInfo(destination).Length OrElse
                    FileHash.Sha256(source) <> FileHash.Sha256(destination) Then
                Throw New SetupContractException("Self-relocation did not preserve exact executable bytes.")
            End If
        End Sub

        Private Shared Function CanonicalFile(path As String) As String
            If String.IsNullOrWhiteSpace(path) OrElse path.StartsWith("\\", StringComparison.Ordinal) OrElse
                    path.StartsWith("\\?\", StringComparison.Ordinal) OrElse path.StartsWith("\\.\", StringComparison.Ordinal) Then
                Throw New SetupContractException("A local absolute file path is required.")
            End If
            Dim fullPath As String = IO.Path.GetFullPath(path)
            If Not IO.Path.IsPathRooted(fullPath) Then Throw New SetupContractException("A local absolute file path is required.")
            Return fullPath
        End Function

        Private Shared Sub RequireDirectChild(parentPath As String, childPath As String)
            Dim parent As String = SetupPathPolicy.CanonicalDirectory(parentPath)
            Dim child As String = SetupPathPolicy.CanonicalDirectory(childPath)
            If Not String.Equals(Path.GetDirectoryName(child), parent, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("The relocation directory must be one direct child of its temporary base.")
            End If
        End Sub

        Private Shared Function QuoteArgument(value As String) As String
            If String.IsNullOrWhiteSpace(value) OrElse value.Contains("""") Then Throw New SetupContractException("A relocation argument cannot be quoted safely.")
            Return """" & value & """"
        End Function

        Private Shared Sub ScheduleDelete(path As String)
            If Not MoveFileEx(path, Nothing, MoveFileDelayUntilReboot) Then
                Throw New SetupContractException("Windows could not schedule relocated setup cleanup: " & path,
                                                 New ComponentModel.Win32Exception(Marshal.GetLastWin32Error()))
            End If
        End Sub

        <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Private Shared Function MoveFileEx(existingFileName As String,
                                           newFileName As String,
                                           flags As Integer) As Boolean
        End Function

    End Class

End Namespace
