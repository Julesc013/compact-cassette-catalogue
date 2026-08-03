Public Class frmBrandNew

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim draft As New BrandDraft(txtBrand.Text, txtCode.Text, txtNotes.Text)
        Dim result As BrandOperationResult = brandService.Create(draft, DateTime.Now)
        If Not result.IsSuccess Then
            MsgBox(result.Message, MsgBoxStyle.Exclamation, "Invalid Brand")
            Return
        End If

        brandCount = brands.Rows.Count
        changes = True
        frmMain.Text = fileName & "* - C3"

        Dim message As String = "Added brand " & result.Brand.Name & " successfully."
        If My.Settings.showMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Brand Added")
        End If
        consoleAdd(message)
        frmMain.loadData()
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
