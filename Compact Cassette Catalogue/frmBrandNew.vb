Public Class frmBrandNew
    Private _createdKey As String
    Private _createdDisplayName As String
    Private _validationErrors As ErrorProvider

    Public ReadOnly Property CreatedKey As String
        Get
            Return _createdKey
        End Get
    End Property

    Public ReadOnly Property CreatedDisplayName As String
        Get
            Return _createdDisplayName
        End Get
    End Property

    Public Property SuppressSuccessMessage As Boolean

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        'Get data to validate
        Dim brand As String = txtBrand.Text
        Dim code As String = txtCode.Text.ToUpper

        Dim brandCount As Integer = brands.Rows.Count
        Dim issues As New List(Of ValidationIssue)()
        If String.IsNullOrWhiteSpace(brand) Then
            issues.Add(New ValidationIssue("txtBrand", "Enter a brand name."))
        End If
        If code.Length <> 2 Then
            issues.Add(New ValidationIssue("txtCode", "Enter a unique two-character brand code."))
        End If
        For Each row As DataRow In brands.Rows
            If String.Equals(CStr(row("Code")), code, StringComparison.OrdinalIgnoreCase) Then
                issues.Add(New ValidationIssue("txtCode", "Brand code " & code & " already exists."))
            End If
            If String.Equals(CStr(row("Brand")), brand, StringComparison.OrdinalIgnoreCase) Then
                issues.Add(New ValidationIssue("txtBrand", "Brand " & brand & " already exists."))
            End If
        Next
        If _validationErrors Is Nothing Then
            _validationErrors = New ErrorProvider(components)
            _validationErrors.ContainerControl = Me
        End If
        If Not CatalogueWorkflow.ShowValidationIssues(Me, _validationErrors, issues, "Check Brand Details") Then
            Exit Sub
        End If

        ''Find next index and save data to record
        ''Dim thisIndex As Integer = CInt(counters.Rows(1)("Number")) '1 = Brands row

        Dim brandRow As DataRow = brands.NewRow()
        brandRow("Brand") = brand
        brandRow("Code") = code
        brandRow("Date") = DateTime.Now
        brandRow("Notes") = txtNotes.Text
        brands.Rows.Add(brandRow)

        'Update brand counter
        SynchronizeEntityCounters(counters, decks, brands, models, tapes)
        brandCount = brands.Rows.Count

        changes = True

        _createdKey = code
        _createdDisplayName = brand

        'Show confirmation message
        Dim message As String = "Added brand " & brand & " successfully."
        If My.Settings.showMessages AndAlso Not SuppressSuccessMessage Then
            MessageBox.Show(Me, message, "Brand Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        consoleAdd(message)

        DialogResult = DialogResult.OK
        Close()

    End Sub
End Class
