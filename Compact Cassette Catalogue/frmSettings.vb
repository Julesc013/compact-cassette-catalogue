Public Class frmSettings
    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' initialise each object.

        Select Case My.Settings.showMessages
            Case True
                cmbShowMessages.SelectedIndex = 0
            Case False
                cmbShowMessages.SelectedIndex = 1
        End Select

        Select Case My.Settings.checkUpdates
            Case "startup"
                cmbCheckUpdates.SelectedIndex = 0
            Case "weekly"
                cmbCheckUpdates.SelectedIndex = 1
            Case "monthly"
                cmbCheckUpdates.SelectedIndex = 2
            Case "manually"
                cmbCheckUpdates.SelectedIndex = 3
        End Select

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        ' Save changes to each settings.

        Select Case cmbShowMessages.SelectedIndex
            Case 0
                My.Settings.showMessages = True
            Case 1
                My.Settings.showMessages = False
        End Select

        Select Case cmbCheckUpdates.SelectedIndex
            Case 0
                My.Settings.checkUpdates = "startup"
            Case 1
                My.Settings.checkUpdates = "weekly"
            Case 2
                My.Settings.checkUpdates = "monthly"
            Case 3
                My.Settings.checkUpdates = "manually"
        End Select


        ' Show confirmation message.
        Dim message As String = "Successfully saved changes to settings."
        If My.Settings.showMessages = True Then
            MsgBox(message, MsgBoxStyle.Question, "Successfully Saved Settings")
        End If
        consoleAdd(message)

        ' Close this form.
        Me.Close()

    End Sub

End Class