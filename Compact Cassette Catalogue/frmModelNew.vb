Public Class frmModelNew

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
    Public Property PreferredBrandCode As String

    Private Sub FrmAddModel_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ReloadBrandChoices(PreferredBrandCode)
    End Sub

    Public Sub ReloadBrandChoices(preferredCode As String)
        cmbBrand.Items.Clear()
        For Each row As DataRow In brands.Rows
            cmbBrand.Items.Add(New CatalogueChoice(CStr(row("Code")), CStr(row("Brand"))))
        Next
        CatalogueWorkflow.SelectChoice(cmbBrand, preferredCode)
    End Sub

    Public Sub AddBrandFromModel()
        Dim createdBrand As CatalogueCreationResult = CatalogueWorkflow.CreateBrandForDetour(Me)
        If createdBrand Is Nothing Then
            Return
        End If
        ReloadBrandChoices(createdBrand.Key)
        cmbBrand.Focus()
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        'Get data to validate
        Dim model As String = txtModel.Text
        Dim code As String = txtCode.Text.ToUpper

        Dim modelCount As Integer = models.Rows.Count

        Dim brandChoice As CatalogueChoice = TryCast(cmbBrand.SelectedItem, CatalogueChoice)
        Dim brand As String = If(brandChoice Is Nothing, Nothing, brandChoice.Text)
        Dim type As Integer = cmbType.SelectedIndex + 1
        Dim brandRow As DataRow = Nothing
        Dim identifier As String = Nothing

        Dim issues As New List(Of ValidationIssue)()
        If brandChoice Is Nothing Then
            issues.Add(New ValidationIssue("cmbBrand", "Select a brand."))
        Else
            brandRow = brands.Rows.Find(brandChoice.Key)
            If brandRow Is Nothing Then
                issues.Add(New ValidationIssue("cmbBrand", "The selected brand no longer exists."))
            End If
        End If
        If cmbType.SelectedIndex < 0 Then
            issues.Add(New ValidationIssue("cmbType", "Select a tape type."))
        End If
        If String.IsNullOrWhiteSpace(model) Then
            issues.Add(New ValidationIssue("txtModel", "Enter a model name."))
        End If
        If code.Length <> 2 Then
            issues.Add(New ValidationIssue("txtCode", "Enter a unique two-character model code."))
        End If
        If brandRow IsNot Nothing AndAlso cmbType.SelectedIndex >= 0 AndAlso code.Length = 2 Then
            identifier = CStr(brandRow("Code")) & CStr(type) & code
            For Each row As DataRow In models.Rows
                If String.Equals(CStr(row("Identifier")), identifier, StringComparison.Ordinal) Then
                    issues.Add(New ValidationIssue("txtCode", "Model identifier " & identifier & " already exists."))
                End If
                If String.Equals(CStr(row("Brand")), brand, StringComparison.OrdinalIgnoreCase) AndAlso
                        String.Equals(CStr(row("Model")), model, StringComparison.OrdinalIgnoreCase) Then
                    issues.Add(New ValidationIssue("txtModel", "That model name already exists for the selected brand."))
                End If
            Next
        End If
        If _validationErrors Is Nothing Then
            _validationErrors = New ErrorProvider(components)
            _validationErrors.ContainerControl = Me
        End If
        If Not CatalogueWorkflow.ShowValidationIssues(Me, _validationErrors, issues, "Check Model Details") Then
            Exit Sub
        End If

        ''Find next index and save data to record
        ''Dim thisIndex As Integer = CInt(counters.Rows(2)("Number")) '2 = Models row

        Dim modelRow As DataRow = models.NewRow()
        modelRow("Brand") = brand
        modelRow("Type") = type
        modelRow("Model") = model
        modelRow("Code") = code
        modelRow("Identifier") = identifier
        modelRow("Name") = txtName.Text
        modelRow("Number") = 0
        modelRow("Date") = DateTime.Now
        modelRow("Notes") = txtNotes.Text
        models.Rows.Add(modelRow)

        'Update model counter
        SynchronizeEntityCounters(counters, decks, brands, models, tapes)
        modelCount = models.Rows.Count

        changes = True

        _createdKey = identifier
        _createdDisplayName = brand & " " & model

        'Show confirmation message
        Dim message As String = "Added model " & brand & " " & model & " successfully."
        If My.Settings.showMessages AndAlso Not SuppressSuccessMessage Then
            MessageBox.Show(Me, message, "Model Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        consoleAdd(message)

        DialogResult = DialogResult.OK
        Close()

    End Sub

End Class
