Public Class frmModelNew

    Private Sub FrmAddModel_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Load brands into combination box
        Dim brandCount As Integer = CInt(counters.Rows(1)("Number"))

        For i As Integer = 0 To brandCount - 1

            Dim thisBrand As String = CStr(brands.Rows(i)("Brand"))
            cmbBrand.Items.Add(thisBrand)

        Next

    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        'Get data to validate
        Dim model As String = txtModel.Text
        Dim code As String = txtCode.Text.ToUpper

        Dim modelCount As Integer = CInt(counters.Rows(2)("Number"))

        'Find the code for the selected brand
        Dim brand As String = cmbBrand.Text
        Dim brandRow As DataRow = brands.Select("Brand='" & brand & "'")(0)
        Dim brandCode As String = CStr(brandRow("Code"))

        Dim type As Integer = cmbType.SelectedIndex + 1
        Dim identifier As String = brandCode & CStr(type) & code

        'Check entered data is correct
        Try

            'Check brand and type have been selected
            If cmbBrand.Text = Nothing Then
                Throw New Exception("Must select a brand.")
            End If

            If cmbType.Text = Nothing Then
                Throw New Exception("Must select a type.")
            End If

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

            Next

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Invalid Data Entry")
            Exit Sub
        End Try

        ''Find next index and save data to record
        ''Dim thisIndex As Integer = CInt(counters.Rows(2)("Number")) '2 = Models row

        models.Rows.Add(New Object() {brand, type, model, code, identifier, txtName.Text, 0, DateTime.Now, txtNotes.Text})

        'Update model counter
        counters.Rows(2)("Number") = modelCount + 1

        changes = True
        'Update title bar
        frmMain.Text = fileName & "* - C3"

        'Show confirmation message
        Dim message As String = "Added model " & brand & " " & model & " successfully."
        If My.Settings.showMessages = True Then
            MsgBox(message, MsgBoxStyle.Information, "Successfully Added Model")
        End If
        consoleAdd(message)

        'Reload data and close this form
        frmMain.loadData()
        Me.Close()

    End Sub

End Class