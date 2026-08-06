Public Class frmModelNew

    Public ReadOnly Property CreatedKey As String
    Public ReadOnly Property CreatedDisplayName As String
    Public Property SuppressSuccessMessage As Boolean

    Private Sub FrmAddModel_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Load brands into combination box
        Dim brandCount As Integer = brands.Rows.Count
        cmbBrand.Items.Clear()

        For i As Integer = 0 To brandCount - 1

            Dim thisBrand As String = CStr(brands.Rows(i)("Brand"))
            cmbBrand.Items.Add(thisBrand)

        Next

    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        'Get data to validate
        Dim model As String = txtModel.Text
        Dim code As String = txtCode.Text.ToUpper

        Dim modelCount As Integer = models.Rows.Count

        Dim brand As String = cmbBrand.Text
        Dim type As Integer = cmbType.SelectedIndex + 1
        Dim brandRow As DataRow = Nothing
        Dim identifier As String = Nothing

        'Check entered data is correct
        Try

            'Check brand and type have been selected
            If cmbBrand.Text = Nothing Then
                Throw New Exception("Must select a brand.")
            End If

            If cmbType.Text = Nothing Then
                Throw New Exception("Must select a type.")
            End If

            For Each candidate As DataRow In brands.Rows
                If String.Equals(CStr(candidate("Brand")), brand, StringComparison.Ordinal) Then
                    brandRow = candidate
                    Exit For
                End If
            Next
            If brandRow Is Nothing Then
                Throw New Exception("The selected brand no longer exists.")
            End If

            Dim brandCode As String = CStr(brandRow("Code"))
            identifier = brandCode & CStr(type) & code

            If model = Nothing Then ''Or Not regexAlphanumeric.IsMatch(model) Then
                'If nothing or not alphanumeric
                Throw New Exception("Model name cannot be empty or include symbols.")
            End If

            If code = Nothing Or Not code.Length = 2 Then ''Or Not regexAlphabetic.IsMatch(code) Then
                'If nothing or not alphabetic or not length of two chars
                Throw New Exception("Code must be 2 characters and cannot include numbers or symbols.")
            End If

            'Check if this code is already used
            For i As Integer = 0 To modelCount - 1

                Dim row As DataRow = models.Rows(i)
                Dim thisIdentifier As String = CStr(row("Identifier"))

                If thisIdentifier = identifier Then
                    'If has same code
                    Throw New Exception("Code must be unique." & vbNewLine & thisIdentifier & " already exists.")
                End If

                If String.Equals(CStr(row("Brand")), brand, StringComparison.OrdinalIgnoreCase) AndAlso
                        String.Equals(CStr(row("Model")), model, StringComparison.OrdinalIgnoreCase) Then
                    Throw New Exception("Model display name must be unique within its brand.")
                End If

            Next

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Invalid Data Entry")
            Exit Sub
        End Try

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
        'Update title bar
        frmMain.Text = fileName & "* - C3"

        'Show confirmation message
        Dim message As String = "Added model " & brand & " " & model & " successfully."
        If My.Settings.showMessages = True Then
            MsgBox(message, MsgBoxStyle.Question, "Successfully Added Model")
        End If
        consoleAdd(message)

        'Reload data and close this form
        frmMain.loadData()
        Me.Close()

    End Sub

End Class
