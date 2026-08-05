Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(
                sender As Object,
                e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup

            migrateSettingsIfRequired()

        End Sub

        Private Sub migrateSettingsIfRequired()

            Try

                If Not My.Settings.settingsUpgradeRequired Then
                    Return
                End If

                ' Upgrade copies values into this version's profile. It does not
                ' rewrite or delete the previous version's user.config.
                My.Settings.Upgrade()
                My.Settings.checkUpdates = normaliseMigratedUpdatePolicy(My.Settings.checkUpdates)
                My.Settings.defaultDirectory = normaliseMigratedDirectory(My.Settings.defaultDirectory)
                My.Settings.settingsUpgradeRequired = False
                My.Settings.Save()

            Catch ex As Exception

                ' Keep the in-memory marker armed. A fresh process will also
                ' reload the default True value when the failed save wrote no
                ' durable state, so the next startup can retry safely.
                Try
                    My.Settings.settingsUpgradeRequired = True
                Catch
                    ' A corrupt current profile must not turn migration into a
                    ' startup failure; the default remains armed for a clean retry.
                End Try

                Try
                    Global.System.Windows.Forms.MessageBox.Show(
                        "Compact Cassette Catalogue could not migrate your existing settings." &
                        vbNewLine & vbNewLine &
                        "The application will continue with safe settings and retry next time." &
                        vbNewLine & vbNewLine & "Error: " & ex.Message,
                        "Settings Migration Failed",
                        Global.System.Windows.Forms.MessageBoxButtons.OK,
                        Global.System.Windows.Forms.MessageBoxIcon.Exclamation)
                Catch
                    ' Settings migration and its notice must never block startup.
                End Try

            End Try

        End Sub

        Private Function normaliseMigratedUpdatePolicy(policy As String) As String

            If policy Is Nothing Then
                Return "never"
            End If

            Select Case policy.Trim().ToLowerInvariant()
                Case "startup", "true"
                    Return "startup"
                Case "weekly"
                    Return "weekly"
                Case "monthly"
                    Return "monthly"
                Case "never", "manually", "false"
                    Return "never"
                Case Else
                    Return "never"
            End Select

        End Function

        Private Function normaliseMigratedDirectory(directory As String) As String

            If String.IsNullOrWhiteSpace(directory) OrElse
                    String.Equals(
                        directory,
                        "My.Computer.FileSystem.SpecialDirectories.MyDocuments",
                        StringComparison.Ordinal) Then
                Return My.Computer.FileSystem.SpecialDirectories.MyDocuments
            End If

            Return directory

        End Function

    End Class
End Namespace
