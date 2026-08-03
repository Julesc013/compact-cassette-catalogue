Public Class frmSettings

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbShowMessages.SelectedIndex = If(preferences.ShowMessages, 0, 1)
        Select Case preferences.UpdatePolicy
            Case UpdateCheckPolicy.Startup
                cmbCheckUpdates.SelectedIndex = 0
            Case UpdateCheckPolicy.Weekly
                cmbCheckUpdates.SelectedIndex = 1
            Case UpdateCheckPolicy.Monthly
                cmbCheckUpdates.SelectedIndex = 2
            Case Else
                cmbCheckUpdates.SelectedIndex = 3
        End Select
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        preferences.ShowMessages = cmbShowMessages.SelectedIndex = 0
        Select Case cmbCheckUpdates.SelectedIndex
            Case 0
                preferences.UpdatePolicy = UpdateCheckPolicy.Startup
            Case 1
                preferences.UpdatePolicy = UpdateCheckPolicy.Weekly
            Case 2
                preferences.UpdatePolicy = UpdateCheckPolicy.Monthly
            Case Else
                preferences.UpdatePolicy = UpdateCheckPolicy.Never
        End Select
        preferences.Save()

        Dim message As String = "Successfully saved changes to settings."
        If preferences.ShowMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Settings Saved")
        End If
        consoleAdd(message)
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
