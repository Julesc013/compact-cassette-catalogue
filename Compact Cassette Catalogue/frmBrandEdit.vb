Public Class frmBrandEdit

    Public brandRow As DataRow
    Dim brandIndex As Integer
    Dim brandName As String
    Dim brandCode As String
    Dim brandDateTime As DateTime
    Dim brandNotes As String

    Private Sub frmBrandEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Declare variables.

        brandIndex = brands.Rows.IndexOf(brandRow)
        brandCode = CStr(brands.Rows(brandIndex)("Code"))
        brandDateTime = Convert.ToDateTime(brands.Rows(brandIndex)("Date"))
        brandName = CStr(brands.Rows(brandIndex)("Brand"))
        brandNotes = CStr(brands.Rows(brandIndex)("Notes"))


        ' Populate objects.

        txtBrand.Text = brandName
        txtCode.Text = brandCode
        txtNotes.Text = brandNotes


    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click

        ' Get data to validate.
        Dim brandNameNew As String = txtBrand.Text
        Dim brandNotesNew As String = txtNotes.Text

        If String.IsNullOrWhiteSpace(brandNameNew) Then
            MsgBox("Brand name cannot be empty.", MsgBoxStyle.Exclamation, "Invalid Data Entry")
            Exit Sub
        End If

        For Each candidate As DataRow In brands.Rows
            If candidate IsNot brandRow AndAlso
                    String.Equals(CStr(candidate("Brand")), brandNameNew, StringComparison.OrdinalIgnoreCase) Then
                MsgBox("Brand name must be unique.", MsgBoxStyle.Exclamation, "Invalid Data Entry")
                Exit Sub
            End If
        Next


        ' Write new data to existing row.

        RenameBrandReferences(models, brandName, brandNameNew)
        brandRow("Brand") = brandNameNew
        brandRow("Notes") = brandNotesNew
        brandName = brandNameNew


        changes = True
        ' Update title bar.
        frmMain.Text = fileName & "* - C3"

        ' Show confirmation message.
        Dim message As String = "Updated brand " & brandName & " successfully."
        If My.Settings.showMessages = True Then
            MsgBox(message, MsgBoxStyle.Question, "Successfully Updated Brand")
        End If
        consoleAdd(message)

        ' Reload data.
        frmMain.loadData() ' Reload main form.
        frmBrands.loadList() ' Reload brands list. (Brands form will always be open while this form is.)

        Me.Close() ' Close this form.

    End Sub

End Class
