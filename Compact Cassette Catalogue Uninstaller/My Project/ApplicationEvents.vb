Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices

Namespace My

    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Try
                Dim arguments As String() = New String() {}
                If e.CommandLine.Count > 0 Then ReDim arguments(e.CommandLine.Count - 1)
                For index As Integer = 0 To e.CommandLine.Count - 1
                    arguments(index) = e.CommandLine(index)
                Next
                Dim executablePath As String = Global.System.Windows.Forms.Application.ExecutablePath
                If arguments.Length = 4 Then
                    RecoverInterruptedTransaction(arguments(1))
                    Dim context As C3Setup.SetupRelocationContext = C3Setup.SetupSelfRelocation.ValidateRelocatedInvocation(arguments, executablePath)
                    C3Setup.SetupSelfRelocation.ScheduleCleanupAfterExit(context)
                    uninstallStatePath = context.StatePath
                    Return
                End If

                Dim statePath As String
                If arguments.Length = 0 Then
                    statePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, C3Setup.InstalledStateCodec.FileName)
                ElseIf arguments.Length = 2 AndAlso arguments(0) = "--state" Then
                    statePath = arguments(1)
                Else
                    Throw New C3Setup.SetupContractException("The installed uninstaller accepts only its exact installed-state argument.")
                End If
                RecoverInterruptedTransaction(statePath)
                C3Setup.SetupSelfRelocation.PrepareAndLaunch(executablePath,
                                                             statePath,
                                                             Path.GetTempPath(),
                                                             New C3Setup.WindowsSetupProcessLauncher())
                e.Cancel = True
            Catch ex As Exception
                MessageBox.Show("The C3 uninstaller could not establish a safe relocated process." & Environment.NewLine & Environment.NewLine & ex.Message,
                                "Could Not Start Uninstaller",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
                e.Cancel = True
            End Try
        End Sub

        Private Shared Sub RecoverInterruptedTransaction(statePath As String)
            If String.IsNullOrWhiteSpace(statePath) OrElse
                    Not String.Equals(Path.GetFileName(statePath), C3Setup.InstalledStateCodec.FileName, StringComparison.Ordinal) Then
                Throw New C3Setup.SetupContractException("Uninstall recovery requires the exact installed-state path.")
            End If
            Dim installRoot As String = C3Setup.SetupPathPolicy.ValidateInstallRoot(Path.GetDirectoryName(Path.GetFullPath(statePath)))
            C3Setup.SetupTransactionRecovery.RecoverIncomplete(installRoot,
                                                                New C3Setup.WindowsSetupShortcutAccess(),
                                                                New C3Setup.WindowsSetupRegistryAccess())
        End Sub

    End Class

End Namespace
