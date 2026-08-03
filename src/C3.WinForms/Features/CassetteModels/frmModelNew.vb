Public Class frmModelNew

    Private Sub FrmAddModel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbBrand.DataSource = brandService.GetAll(Nothing)
        cmbBrand.DisplayMember = "Name"
        cmbBrand.ValueMember = "Code"
        cmbBrand.SelectedIndex = -1
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim selectedBrand As Brand = TryCast(cmbBrand.SelectedItem, Brand)
        Dim brandCode As String = If(selectedBrand Is Nothing, Nothing, selectedBrand.Code)
        Dim draft As New CassetteModelDraft(
            brandCode,
            cmbType.SelectedIndex + 1,
            txtModel.Text,
            txtCode.Text,
            txtName.Text,
            txtNotes.Text)
        Dim result As CassetteModelOperationResult = cassetteModelService.Create(draft, DateTime.Now)
        If Not result.IsSuccess Then
            MsgBox(result.Message, MsgBoxStyle.Exclamation, "Invalid Cassette Model")
            Return
        End If

        modelCount = models.Rows.Count
        CompleteCatalogueMutation(Me)

        Dim message As String = "Added cassette model " & result.Model.Identifier & " successfully."
        If My.Settings.showMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Cassette Model Added")
        End If
        consoleAdd(message)
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
