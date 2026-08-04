Friend Module ExternalLinkLauncher

    Public Sub Open(link As String)
        Try
            Process.Start(link)
        Catch ex As Exception
            Dim message As String = "Failed to open link."
            UiDiagnostics.Add(message & " " & link & " Error: " & ex.Message)
            MsgBox(
                message & vbNewLine & vbNewLine & link & vbNewLine & vbNewLine &
                    "Error: " & ex.Message,
                MsgBoxStyle.Exclamation,
                "Could Not Open Link")
        End Try
    End Sub

End Module
