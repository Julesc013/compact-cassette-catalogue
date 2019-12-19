Public Class frmViewBrands

    Dim identifiers As New List(Of String)
    Dim identifierCount As Integer = 0

    Private Sub frmViewBrands_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadList()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        loadList()
    End Sub

    Public Sub loadList()
        ' Load Data from the DataSet into the ListView.

        ' Count the number of results.
        Dim resultsCount As Integer = 0

        ' Declare holder variables.
        Dim code As String
        Dim brand As String
        Dim notes As String

        Dim critNotes As String = txtNotes.Text

        ' Clear the ListView control.
        lstBrands.Items.Clear()

        ' Display items in the ListView control.
        For i As Integer = 0 To brands.Rows.Count - 1

            Dim thisRow As DataRow = brands.Rows(i)

            ' Only rows that have Not been deleted.
            If thisRow.RowState <> DataRowState.Deleted Then

                ' Get data from row.
                code = thisRow("Code").ToString.ToUpper
                brand = thisRow("Brand").ToString
                notes = thisRow("Notes").ToString

                ' Only rows that fit the filter criteria!
                If critNotes = Nothing Or notes.ToLower.Contains(critNotes.ToLower) Then

                    ' Define the list items.
                    Dim lstViewItem As ListViewItem = New ListViewItem(code)
                    lstViewItem.SubItems.Add(brand)
                    lstViewItem.SubItems.Add(notes)

                    ' Add the list items to the ListView.
                    lstBrands.Items.Add(lstViewItem)

                    resultsCount += 1

                End If

            End If

        Next

        ' Display number of results.
        txtResults.Text = CStr(resultsCount)

    End Sub

    Private Sub lstBrands_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstBrands.SelectedIndexChanged

        identifiers.Clear() ' Clear selected identifiers.
        identifierCount = lstBrands.SelectedItems.Count ' Number of indetifiers selected.

        If identifierCount >= 1 Then

            'identifierSelected = lstTapes.SelectedItems(0).SubItems(0).Text ' Get the string of the first column (subitem) in the primary selected row (i.e. the identifier).
            For i As Integer = 0 To identifierCount - 1

                ' Add all of the selected identifiers to a list.
                identifiers.Add(lstBrands.SelectedItems(i).SubItems(0).Text)

            Next

            ' Enable buttons.

            btnDelete.Enabled = True
            ' Only allow editing if only one tape is selected.
            If identifierCount = 1 Then
                btnEdit.Enabled = True
            Else
                btnEdit.Enabled = False
            End If

        Else

            ' Do not add any identifiers to the list, leave it empty.

            ' Disable buttons.
            btnEdit.Enabled = False
            btnDelete.Enabled = False

        End If

    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        ' Delete every brand in the list.

        ' Confirm with the user that they would like to delete their selection.
        Dim result As MsgBoxResult = MsgBox("Are you sure you want to delete all the selected (" & CStr(identifierCount) & ") brands?" & vbNewLine & "This action cannot be undone.", MsgBoxStyle.YesNoCancel, "Confirm Deletion")

        If result = vbYes Then

            For Each identifier In identifiers

                ' Get this brand's row in the data table.
                Dim brandRow As DataRow = brands.Rows.Find(identifier)

                ' Remove the this brand's record from the table.
                brands.Rows.Remove(brandRow)

                ' Update brand counter.
                brandCount -= 1
                counters.Rows(1)("Number") = brandCount

                ' Update change detection variable.
                changes = True
                'Update title bar
                Me.Text = fileName & "* - C3"

                'Show confirmation message
                Dim message As String = "Deleted brand " & identifier & " successfully."
                'If My.Settings.showMessages = True Then
                '    MsgBox(message, MsgBoxStyle.Information, "Successfully Deleted Brand(s)")
                'End If
                consoleAdd(message)

            Next

            loadList() ' Refresh the list data.

            lstBrands.SelectedItems.Clear() ' Clear selection of brands.
            ' Disable buttons.
            btnEdit.Enabled = False
            btnDelete.Enabled = False

        End If

    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click

        ' Get index of this brand's row in the data table.
        Dim identifier As String = identifiers(0) ' Use the first brand selected.
        frmBrandEdit.brandRow = brands.Rows.Find(identifier) ' Send this row to the next form for processing.

        ' Open the editting form.
        frmBrandEdit.Show()

    End Sub

End Class