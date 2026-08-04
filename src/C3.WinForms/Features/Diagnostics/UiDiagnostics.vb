Friend Module UiDiagnostics

    Public Sub Add(message As String)
        BufferedLogger.Information(message)
        Dim stamp As String = "[" & FormatTimestamp(DateTime.Now) & "]"
        For Each window As Form In Application.OpenForms
            Dim consoleWindow As frmConsole = TryCast(window, frmConsole)
            If consoleWindow IsNot Nothing Then
                consoleWindow.AppendEntry(stamp & " " & message)
                Exit For
            End If
        Next
    End Sub

    Public Function FormatTimestamp(value As DateTime) As String
        Return value.ToString("dd/MM/yy HH:mm:ss")
    End Function

End Module
