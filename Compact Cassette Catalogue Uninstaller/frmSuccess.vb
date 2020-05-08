Public Class frmSuccess
    Private Sub frmSuccess_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click

        ' If the checkbox is selected, run C3 (else dont run C3) then immediately exit this installer application.

        If chkOpenFeedback.Checked = True Then

            Process.Start(FEEDBACKLINK) ' Start C3.

        End If

        Application.Exit() ' Exit the entire installer application.

    End Sub
End Class