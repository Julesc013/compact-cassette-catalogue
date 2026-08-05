Public Class frmSuccess
    Private Sub frmSuccess_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AccessibleName = "Compact Cassette Catalogue uninstall complete"
        chkOpenFeedback.Text = "Open the &feedback web page after closing."
        chkOpenFeedback.AccessibleDescription = "Optionally open the C3 feedback page in the default browser."
        btnFinish.Text = "&Finish"
        btnFinish.AccessibleDescription = "Close the uninstaller and optionally open the feedback page."
        btnBack.Visible = False
        btnCancel.Visible = False
        btnFinish.Select()
    End Sub

    Private Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click

        ' If the checkbox is selected, run C3 (else dont run C3) then immediately exit this installer application.

        If chkOpenFeedback.Checked = True Then

            Try

                Process.Start(FEEDBACKLINK) ' Open feedback page.

            Catch ex As Exception

                MsgBox("Could not open the feedback page." & vbNewLine & vbNewLine & FEEDBACKLINK & vbNewLine & vbNewLine & "Error: " & ex.Message, MsgBoxStyle.Exclamation, "Could Not Open Feedback")

            End Try

        End If

        Application.Exit() ' Exit the entire installer application.

    End Sub
End Class
