Public Class frmViewBrands
    Private Sub frmViewBrands_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadList()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        loadList()
    End Sub

    Private Sub loadList()
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

End Class