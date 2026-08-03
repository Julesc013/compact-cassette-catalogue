Public Class frmBrands

    Private ReadOnly _selectedCodes As New List(Of String)()

    Private Sub frmViewBrands_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadList()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        loadList()
    End Sub

    Public Sub loadList()
        Dim values As IList(Of Brand) = brandService.GetAll(txtNotes.Text)
        lstBrands.BeginUpdate()
        Try
            lstBrands.Items.Clear()
            For Each value As Brand In values
                Dim item As New ListViewItem(value.Code)
                item.SubItems.Add(value.Name)
                item.SubItems.Add(value.Notes)
                lstBrands.Items.Add(item)
            Next
        Finally
            lstBrands.EndUpdate()
        End Try
        txtResults.Text = values.Count.ToString()
    End Sub

    Private Sub lstBrands_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstBrands.SelectedIndexChanged
        _selectedCodes.Clear()
        For Each item As ListViewItem In lstBrands.SelectedItems
            _selectedCodes.Add(item.SubItems(0).Text)
        Next

        btnDelete.Enabled = _selectedCodes.Count > 0
        btnEdit.Enabled = _selectedCodes.Count = 1
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If _selectedCodes.Count = 0 Then
            Return
        End If

        Dim prompt As String = "Delete the selected " & _selectedCodes.Count.ToString() &
            " brand(s)?" & vbNewLine & "This action cannot be undone."
        If MsgBox(prompt, MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirm Brand Deletion") <> vbYes Then
            Return
        End If

        Dim failures As New List(Of String)()
        Dim changed As Boolean = False
        For Each code As String In _selectedCodes
            Dim result As BrandOperationResult = brandService.Delete(code)
            If result.IsSuccess Then
                changed = True
                consoleAdd("Deleted brand " & code & " successfully.")
            Else
                failures.Add(code & ": " & result.Message)
            End If
        Next

        brandCount = brandService.GetAll(Nothing).Count
        If changed Then
            CompleteCatalogueMutation(Me)
        End If
        If failures.Count > 0 Then
            MsgBox(String.Join(vbNewLine, failures.ToArray()), MsgBoxStyle.Exclamation, "Some Brands Were Not Deleted")
        End If

        loadList()
        btnEdit.Enabled = False
        btnDelete.Enabled = False
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If _selectedCodes.Count <> 1 Then
            Return
        End If

        Using editor As New frmBrandEdit()
            editor.BrandCode = _selectedCodes(0)
            editor.ShowDialog(Me)
        End Using
        loadList()
    End Sub

End Class
