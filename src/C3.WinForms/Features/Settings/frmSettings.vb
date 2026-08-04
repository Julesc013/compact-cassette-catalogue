Public Class frmSettings

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbShowMessages.SelectedIndex = If(My.Application.Composition.Preferences.ShowMessages, 0, 1)
        Select Case My.Application.Composition.Preferences.UpdatePolicy
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
        My.Application.Composition.Preferences.ShowMessages = cmbShowMessages.SelectedIndex = 0
        Select Case cmbCheckUpdates.SelectedIndex
            Case 0
                My.Application.Composition.Preferences.UpdatePolicy = UpdateCheckPolicy.Startup
            Case 1
                My.Application.Composition.Preferences.UpdatePolicy = UpdateCheckPolicy.Weekly
            Case 2
                My.Application.Composition.Preferences.UpdatePolicy = UpdateCheckPolicy.Monthly
            Case Else
                My.Application.Composition.Preferences.UpdatePolicy = UpdateCheckPolicy.Never
        End Select
        Dim saveResult As UserPreferencesSaveResult =
            My.Application.Composition.Preferences.TrySave()
        If Not saveResult.IsSuccess Then
            Dim failureMessage As String =
                "C3 could not save the settings. Your changes remain pending so you can retry." &
                Environment.NewLine & Environment.NewLine &
                saveResult.Message
            UiDiagnostics.Add("Settings save failed: " & saveResult.Message)
            MessageBox.Show(
                failureMessage,
                "Settings Not Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation)
            Return
        End If

        Dim message As String = "Successfully saved changes to settings."
        If My.Application.Composition.Preferences.ShowMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Settings Saved")
        End If
        UiDiagnostics.Add(message)
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
