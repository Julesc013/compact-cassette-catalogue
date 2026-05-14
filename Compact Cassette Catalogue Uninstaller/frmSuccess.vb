Public Class frmSuccess
    Private Sub frmSuccess_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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
