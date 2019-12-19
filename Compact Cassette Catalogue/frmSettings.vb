Public Class frmSettings
    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' initialise objects.

        Select Case My.Settings.showMessages
            Case True
                cmbMessages.SelectedIndex = 0
            Case False
                cmbMessages.SelectedIndex = 1
        End Select

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        ' Save changes to the settings.

        Select Case cmbMessages.SelectedIndex
            Case 0
                My.Settings.showMessages = True
            Case 1
                My.Settings.showMessages = False
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