Public Class frmFailure

    Private Sub frmFailure_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AccessibleName = "Compact Cassette Catalogue uninstall failure"
        If Not String.IsNullOrWhiteSpace(uninstallFailureMessage) Then lblFailure.Text = uninstallFailureMessage
        lblFailure.AccessibleName = "Uninstall failure details"
        lblFailure.AccessibleDescription = lblFailure.Text
        btnBack.Visible = False
        btnCancel.Visible = False
        btnFinish.Text = "&Finish"
        btnFinish.AccessibleDescription = "Close the uninstaller."
        btnFinish.Select()
    End Sub

    Private Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        Application.Exit()
    End Sub

End Class
