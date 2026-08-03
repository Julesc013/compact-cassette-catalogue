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

            BufferedLogger.RecordAction("Starting C3")
            BufferedLogger.Information("Runtime: " & RuntimeInfo.BuildLabel)
        End Sub

        Private Sub MyApplication_UnhandledException(
                sender As Object,
                e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException

            BufferedLogger.Error("Unhandled exception: " & e.Exception.Message)

            Dim context As New CrashReportContext() With {
                .CataloguePath = filePath,
                .LastAction = BufferedLogger.LastAction
            }
            Dim reportPath As String = CrashReportWriter.TryWrite(e.Exception, context)

            Dim message As String = "C3 encountered an unexpected error and must close."
            If reportPath IsNot Nothing Then
                message &= Environment.NewLine & Environment.NewLine &
                    "A diagnostic report was saved to:" & Environment.NewLine & reportPath
            Else
                message &= Environment.NewLine & Environment.NewLine &
                    "C3 could not save a diagnostic report."
            End If

            MessageBox.Show(message, "Unexpected C3 Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            e.ExitApplication = True
        End Sub

    End Class
End Namespace
