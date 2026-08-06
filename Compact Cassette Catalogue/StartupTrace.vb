Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Threading

Friend Module StartupTrace

    Private ReadOnly traceLock As New Object()
    Private traceSequence As Integer

    Friend Sub Record(milestone As String)

        Try
            Dim tracePath As String = Environment.GetEnvironmentVariable("C3_STARTUP_TRACE")
            If String.IsNullOrWhiteSpace(tracePath) Then
                Return
            End If

            Dim fullPath As String = Path.GetFullPath(tracePath)
            Dim parentDirectory As String = Path.GetDirectoryName(fullPath)
            If Not String.IsNullOrWhiteSpace(parentDirectory) Then
                Directory.CreateDirectory(parentDirectory)
            End If

            SyncLock traceLock
                traceSequence += 1
                Dim fields As String() = {
                    traceSequence.ToString(CultureInfo.InvariantCulture),
                    DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                    Stopwatch.GetTimestamp().ToString(CultureInfo.InvariantCulture),
                    Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture),
                    Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture),
                    NormaliseField(milestone)
                }
                Using writer As New StreamWriter(fullPath, True, New UTF8Encoding(False))
                    writer.WriteLine(String.Join(vbTab, fields))
                End Using
            End SyncLock
        Catch
            ' Diagnostic evidence must never delay or prevent normal startup.
        End Try

    End Sub

    Private Function NormaliseField(value As String) As String
        If value Is Nothing Then
            Return String.Empty
        End If
        Return value.Replace(vbTab, " ").Replace(vbCr, " ").Replace(vbLf, " ")
    End Function

End Module
