Public Class frmModels

    Private ReadOnly _selectedIdentifiers As New List(Of String)()
    Private _brands As IList(Of Brand) = New List(Of Brand)()

    Private Sub frmModels_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadBrandChoices()
        cmbBrand.SelectedIndex = 0
        cmbTypes.SelectedIndex = 0
        loadList()
    End Sub

    Private Sub LoadBrandChoices()
        _brands = brandService.GetAll(Nothing)
        cmbBrand.Items.Clear()
        cmbBrand.Items.Add("All Brands")
        For Each value As Brand In _brands
            cmbBrand.Items.Add(value.Name)
        Next
    End Sub

    Public Sub loadList()
        Dim selectedBrandCode As String = Nothing
        If cmbBrand.SelectedIndex > 0 AndAlso cmbBrand.SelectedIndex <= _brands.Count Then
            selectedBrandCode = _brands(cmbBrand.SelectedIndex - 1).Code
        End If

        Dim selectedType As Integer = cmbTypes.SelectedIndex
        Dim minimumType As Boolean = chkTypeBetter.Checked
        Dim nameFilter As String = txtName.Text
        Dim notesFilter As String = txtNotes.Text

        Dim results As New List(Of CassetteModel)()
        For Each value As CassetteModel In cassetteModelService.GetAll()
            If selectedBrandCode IsNot Nothing AndAlso
                    Not String.Equals(value.BrandCode, selectedBrandCode, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If
            If selectedType > 0 Then
                If minimumType AndAlso value.TypeNumber < selectedType Then
                    Continue For
                End If
                If Not minimumType AndAlso value.TypeNumber <> selectedType Then
                    Continue For
                End If
            End If
            If Not ContainsText(value.DisplayName, nameFilter) OrElse
                    Not ContainsText(value.Notes, notesFilter) Then
                Continue For
            End If
            results.Add(value)
        Next

        lstModels.BeginUpdate()
        Try
            lstModels.Items.Clear()
            For Each value As CassetteModel In results
                Dim item As New ListViewItem(value.Identifier)
                item.SubItems.Add(ResolveBrandName(value.BrandCode))
                item.SubItems.Add(getTypeNumeral(value.TypeNumber, True))
                item.SubItems.Add(value.ModelName)
                item.SubItems.Add(value.Code)
                item.SubItems.Add(value.DisplayName)
                item.SubItems.Add(value.TapeCount.ToString())
                item.SubItems.Add(value.Notes)
                lstModels.Items.Add(item)
            Next
        Finally
            lstModels.EndUpdate()
        End Try

        txtResults.Text = results.Count.ToString()
    End Sub

    Private Shared Function ContainsText(value As String, filter As String) As Boolean
        Return String.IsNullOrWhiteSpace(filter) OrElse
            If(value, String.Empty).IndexOf(filter.Trim(), StringComparison.CurrentCultureIgnoreCase) >= 0
    End Function

    Private Function ResolveBrandName(code As String) As String
        For Each value As Brand In _brands
            If String.Equals(value.Code, code, StringComparison.OrdinalIgnoreCase) Then
                Return value.Name
            End If
        Next
        Return code
    End Function

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadBrandChoices()
        cmbBrand.SelectedIndex = 0
        loadList()
    End Sub

    Private Sub cmbTypes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTypes.SelectedIndexChanged
        Dim selectedType As Integer = cmbTypes.SelectedIndex
        If selectedType > 0 Then
            chkTypeBetter.Text = "Type " & getTypeNumeral(selectedType, False) & " or better."
            chkTypeBetter.Enabled = True
        Else
            chkTypeBetter.Enabled = False
            chkTypeBetter.Text = "Type I or better."
            chkTypeBetter.Checked = False
        End If
    End Sub

    Private Sub lstModels_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstModels.SelectedIndexChanged
        _selectedIdentifiers.Clear()
        For Each item As ListViewItem In lstModels.SelectedItems
            _selectedIdentifiers.Add(item.SubItems(0).Text)
        Next
        btnDelete.Enabled = _selectedIdentifiers.Count > 0
        btnEdit.Enabled = _selectedIdentifiers.Count = 1
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If _selectedIdentifiers.Count = 0 Then
            Return
        End If

        Dim prompt As String = "Delete the selected " & _selectedIdentifiers.Count.ToString() &
            " cassette model(s)?" & vbNewLine & "This action cannot be undone."
        If MsgBox(prompt, MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirm Model Deletion") <> vbYes Then
            Return
        End If

        Dim failures As New List(Of String)()
        Dim changed As Boolean = False
        For Each identifier As String In _selectedIdentifiers
            Dim result As CassetteModelOperationResult = cassetteModelService.Delete(identifier)
            If result.IsSuccess Then
                changed = True
                consoleAdd("Deleted cassette model " & identifier & " successfully.")
            Else
                failures.Add(identifier & ": " & result.Message)
            End If
        Next

        If changed Then
            modelCount = models.Rows.Count
            CompleteCatalogueMutation(Me)
        End If
        If failures.Count > 0 Then
            MsgBox(
                String.Join(vbNewLine, failures.ToArray()),
                MsgBoxStyle.Exclamation,
                "Some Cassette Models Were Not Deleted")
        End If

        loadList()
        btnEdit.Enabled = False
        btnDelete.Enabled = False
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If _selectedIdentifiers.Count <> 1 Then
            Return
        End If

        Using editor As New frmModelEdit()
            editor.ModelIdentifier = _selectedIdentifiers(0)
            editor.ShowDialog(Me)
        End Using
        loadList()
    End Sub

End Class
