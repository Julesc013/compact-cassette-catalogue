Public Class frmModelEdit

    Public Property ModelIdentifier As String

    Private Sub frmModelEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim value As CassetteModel = cassetteModelService.Find(ModelIdentifier)
        If value Is Nothing Then
            MsgBox(
                "The selected cassette model no longer exists.",
                MsgBoxStyle.Exclamation,
                "Cassette Model Not Found")
            DialogResult = DialogResult.Cancel
            Close()
            Return
        End If

        ModelIdentifier = value.Identifier
        Dim brand As Brand = brandService.Find(value.BrandCode)
        txtBrand.Text = If(brand Is Nothing, value.BrandCode, brand.Name)
        txtType.Text = getTypeNumeral(value.TypeNumber, True)
        txtModel.Text = value.ModelName
        txtCode.Text = value.Code
        txtName.Text = value.DisplayName
        txtNotes.Text = value.Notes
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim existing As CassetteModel = cassetteModelService.Find(ModelIdentifier)
        If existing Is Nothing Then
            MsgBox(
                "The selected cassette model no longer exists.",
                MsgBoxStyle.Exclamation,
                "Cassette Model Not Found")
            Close()
            Return
        End If

        Dim draft As New CassetteModelDraft(
            existing.BrandCode,
            existing.TypeNumber,
            txtModel.Text,
            existing.Code,
            txtName.Text,
            txtNotes.Text)
        Dim result As CassetteModelOperationResult = cassetteModelService.Update(ModelIdentifier, draft)
        If Not result.IsSuccess Then
            MsgBox(result.Message, MsgBoxStyle.Exclamation, "Invalid Cassette Model")
            Return
        End If

        CompleteCatalogueMutation(Me)

        Dim message As String = "Updated cassette model " & result.Model.Identifier & " successfully."
        If preferences.ShowMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Cassette Model Updated")
        End If
        consoleAdd(message)
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
