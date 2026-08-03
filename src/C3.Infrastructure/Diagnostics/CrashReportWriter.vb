Imports System.IO
Imports System.Text

Namespace Diagnostics

Public NotInheritable Class CrashReportContext

    Public Property ProductVersion As String
    Public Property ReleaseStage As String
    Public Property BuildLane As String
    Public Property OperatingSystem As String
    Public Property ClrVersion As String
    Public Property ProcessBitness As String
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
        If context Is Nothing Then
            context = New CrashReportContext() With {.LastAction = BufferedLogger.LastAction}
        End If

        Dim report As New StringBuilder()
        report.AppendLine("C3 crash report")
        report.AppendLine("===============")
        report.AppendLine("Created (UTC): " & DateTime.UtcNow.ToString("O"))
        report.AppendLine("Product version: " & If(context.ProductVersion, "(unknown)") & " " & If(context.ReleaseStage, String.Empty))
        report.AppendLine("Build lane: " & If(context.BuildLane, "(unknown)"))
        report.AppendLine("Operating system: " & If(context.OperatingSystem, "(unknown)"))
        report.AppendLine("CLR version: " & If(context.ClrVersion, "(unknown)"))
        report.AppendLine("Process bitness: " & If(context.ProcessBitness, "(unknown)"))

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

End Namespace
