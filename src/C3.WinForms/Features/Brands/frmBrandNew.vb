Public Class frmBrandNew

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim draft As New BrandDraft(txtBrand.Text, txtCode.Text, txtNotes.Text)
        Dim result As BrandOperationResult =
            My.Application.Composition.BrandService.Create(draft, DateTime.Now)
        If Not result.IsSuccess Then
            MsgBox(result.Message, MsgBoxStyle.Exclamation, "Invalid Brand")
            Return
        End If

        CompleteCatalogueMutation(Me)

        Dim message As String = "Added brand " & result.Brand.Name & " successfully."
        If My.Application.Composition.Preferences.ShowMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Brand Added")
        End If
        UiDiagnostics.Add(message)
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
