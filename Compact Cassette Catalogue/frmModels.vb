Public Class frmModels

    Dim identifiers As New List(Of String)
    Dim identifierCount As Integer = 0

    Private Sub frmModels_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Populate objects.

        Dim brandCount As Integer = CInt(counters.Rows(1)("Number"))

        ' Load brands into combination box.
        For i As Integer = 0 To brandCount - 1
            Dim thisBrand As String = CStr(brands.Rows(i)("Brand"))
            cmbBrand.Items.Add(thisBrand)
        Next


        ' Initialise objects.
        cmbBrand.SelectedIndex = 0
        cmbTypes.SelectedIndex = 0

        loadList()

    End Sub

    Private Sub loadList()
        ' Load Data from the DataSet into the ListView.

        Dim validRow As Boolean
        ' Count the number of results.
        Dim resultsCount As Integer = 0


        ' Get filter values
        Dim critBrand As String = cmbBrand.Text
        Dim critType As Integer = cmbTypes.SelectedIndex
        Dim critTypeBetter As Boolean = chkTypeBetter.Checked
        Dim critName As String = txtName.Text
        Dim critNotes As String = txtNotes.Text

        ' Clear the ListView control.
        lstModels.Items.Clear()

        ' Display items in the ListView control.
        For i As Integer = 0 To models.Rows.Count - 1

            Dim thisRow As DataRow = models.Rows(i)

            ' Initialise this row as a valid result given criteria.
            validRow = True

            ' Only rows that have Not been deleted.
            If thisRow.RowState <> DataRowState.Deleted Then

                ' Get data from row.
                Dim identifier As String = thisRow("Identifier").ToString
                Dim brand As String = thisRow("Brand").ToString
                Dim type As Integer = CInt(thisRow("Type"))
                Dim model As String = thisRow("Model").ToString
                Dim code As String = thisRow("Code").ToString
                Dim fullName As String = thisRow("Name").ToString
                Dim number As Integer = CInt(thisRow("Number"))
                Dim notes As String = thisRow("Notes").ToString


                ' Filter using criteria.

                If cmbBrand.SelectedIndex <> 0 And Not brand.ToLower = critBrand.ToLower Then
                    validRow = False
                End If

                ' Filter for types.
                If critType <> 0 Then
                    If critTypeBetter = False Then

                        If Not type = critType Then
                            validRow = False
                        End If

                    Else

                        If Not type >= critType Then
                            validRow = False
                        End If

                    End If
                End If

                If critName <> Nothing And Not fullName.ToLower.Contains(critName.ToLower) Then
                    validRow = False
                End If

                If critNotes <> Nothing And Not notes.ToLower.Contains(critNotes.ToLower) Then
                    validRow = False
                End If


                ' Only display rows that fit the filter criteria!
                If validRow = True Then

                    ' Define the list items.
                    Dim lstViewItem As ListViewItem = New ListViewItem(identifier)
                    lstViewItem.SubItems.Add(brand)
                    lstViewItem.SubItems.Add(getTypeNumeral(type, True))
                    lstViewItem.SubItems.Add(model)
                    lstViewItem.SubItems.Add(code)
                    lstViewItem.SubItems.Add(fullName)
                    lstViewItem.SubItems.Add(number.ToString)
                    lstViewItem.SubItems.Add(notes)

                    ' Add the list items to the ListView.
                    lstModels.Items.Add(lstViewItem)

                    resultsCount += 1

                End If

            End If

        Next

        ' Display number of results.
        txtResults.Text = CStr(resultsCount)

    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        loadList()
    End Sub

    Private Sub cmbTypes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTypes.SelectedIndexChanged

        ' If the selected type is not "all types" then enable the ability to check the box below.

        Dim selectedIndex As Integer = cmbTypes.SelectedIndex

        If selectedIndex <> 0 Then ' If it is a specific type.

            ' Convert the Arabic to a Roman numeral.
            Dim typeNumeral As String = getTypeNumeral(selectedIndex, False)
            chkTypeBetter.Text = "Type " & typeNumeral & " or better."
            chkTypeBetter.Enabled = True

        Else

            ' When selected "all types" disable and decheck the box.
            chkTypeBetter.Enabled = False
            chkTypeBetter.Text = "Type I or better."
            chkTypeBetter.Checked = False

        End If

    End Sub

    Private Sub lstModels_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstModels.SelectedIndexChanged

        identifiers.Clear() ' Clear selected identifiers.
        identifierCount = lstModels.SelectedItems.Count ' Number of indetifiers selected.

        If identifierCount >= 1 Then

            'identifierSelected = lstTapes.SelectedItems(0).SubItems(0).Text ' Get the string of the first column (subitem) in the primary selected row (i.e. the identifier).
            For i As Integer = 0 To identifierCount - 1

                ' Add all of the selected identifiers to a list.
                identifiers.Add(lstModels.SelectedItems(i).SubItems(0).Text)

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

End Class