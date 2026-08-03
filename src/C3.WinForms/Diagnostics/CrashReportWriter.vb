Imports System.IO
Imports System.Text

Public NotInheritable Class CrashReportContext

    Public Property CataloguePath As String
    Public Property LastAction As String

End Class

Public NotInheritable Class CrashReportWriter

    Private Sub New()
    End Sub

    Public Shared Function TryWrite(exception As Exception, context As CrashReportContext) As String
        If exception Is Nothing Then
            Return Nothing
        End If

        Try
            Dim localData As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Dim reportDirectory As String = Path.Combine(localData, "C3", "CrashReports")
            Directory.CreateDirectory(reportDirectory)

            Dim fileName As String = "C3-error-" & DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") & ".log"
            Dim reportPath As String = Path.Combine(reportDirectory, fileName)
            File.WriteAllText(reportPath, BuildReport(exception, context), New UTF8Encoding(False))
            Return reportPath
        Catch
            Return Nothing
        End Try
    End Function

    Private Shared Function BuildReport(exception As Exception, context As CrashReportContext) As String
        Dim report As New StringBuilder()
        report.AppendLine("C3 crash report")
        report.AppendLine("===============")
        report.AppendLine("Created (UTC): " & DateTime.UtcNow.ToString("O"))
        report.AppendLine("Product version: " & VERSION & " " & VERSIONSTAGE)
        report.AppendLine("Build lane: " & RuntimeInfo.BuildLabel)
        report.AppendLine("Operating system: " & Environment.OSVersion.ToString())
        report.AppendLine("CLR version: " & Environment.Version.ToString())
        report.AppendLine("Process bitness: " & (IntPtr.Size * 8).ToString() & "-bit")

        If context IsNot Nothing Then
            report.AppendLine("Catalogue path: " & If(context.CataloguePath, "(new catalogue)"))
            report.AppendLine("Last action: " & If(context.LastAction, "(unknown)"))
        Else
            report.AppendLine("Catalogue path: (unknown)")
            report.AppendLine("Last action: " & BufferedLogger.LastAction)
        End If

        report.AppendLine()
        report.AppendLine("Exception")
        report.AppendLine("---------")
        report.AppendLine(exception.ToString())
        report.AppendLine()
        report.AppendLine("Recent log")
        report.AppendLine("----------")
        For Each entry As String In BufferedLogger.Tail()
            report.AppendLine(entry)
        Next

        Return report.ToString()
    End Function

End Class

