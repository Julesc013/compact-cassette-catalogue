Public Class frmBrandEdit

    Public Property BrandCode As String

    Private Sub frmBrandEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim value As Brand = My.Application.Composition.BrandService.Find(BrandCode)
        If value Is Nothing Then
            MsgBox("The selected brand no longer exists.", MsgBoxStyle.Exclamation, "Brand Not Found")
            DialogResult = DialogResult.Cancel
            Close()
            Return
        End If

        BrandCode = value.Code
        txtBrand.Text = value.Name
        txtCode.Text = value.Code
        txtCode.ReadOnly = True
        txtNotes.Text = value.Notes
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim draft As New BrandDraft(txtBrand.Text, BrandCode, txtNotes.Text)
        Dim result As BrandOperationResult =
            My.Application.Composition.BrandService.Update(BrandCode, draft)
        If Not result.IsSuccess Then
            MsgBox(result.Message, MsgBoxStyle.Exclamation, "Invalid Brand")
            Return
        End If

        CompleteCatalogueMutation(Me)

        Dim message As String = "Updated brand " & result.Brand.Name & " successfully."
        If My.Application.Composition.Preferences.ShowMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Brand Updated")
        End If
        UiDiagnostics.Add(message)
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
