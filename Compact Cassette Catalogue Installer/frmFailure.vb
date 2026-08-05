Public Class frmFailure

    Private Sub frmFailure_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not String.IsNullOrWhiteSpace(setupFailureMessage) Then lblFailure.Text = setupFailureMessage
        btnBack.Visible = False
        btnCancel.Visible = False
        btnFinish.Select()
    End Sub

    Private Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        Application.Exit()
    End Sub

End Class
