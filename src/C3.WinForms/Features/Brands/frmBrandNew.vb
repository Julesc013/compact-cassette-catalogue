Public Class frmBrandNew
    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        'Get data to validate
        Dim brand As String = txtBrand.Text
        Dim code As String = txtCode.Text.ToUpper

        Dim brandCount As Integer = CInt(counters.Rows(1)("Number"))

        'Check entered data is correct
        Try

            If brand = Nothing Then ''Or Not regexAlphanumeric.IsMatch(brand) Then
                'If nothing or not alphanumeric
                Throw New Exception("Brand name cannot be empty or include symbols.")
            End If

            If code = Nothing Or Not code.Length = 2 Then ''Or Not regexAlphabetic.IsMatch(code) Then
                'If nothing or not alphabetic or not length of two chars
                Throw New Exception("Code must be 2 characters and cannot include numbers or symbols.")
            End If

            'Check if this code is already used
            For i As Integer = 0 To brandCount - 1
                Dim row As DataRow = brands.Rows(i)

                If CStr(row("Code")) = code Then
                    'If not removed and has save code
                    Throw New Exception("Code must be unique." & vbNewLine & code & " already exists.")
                End If

            Next

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Invalid Data Entry")
            Exit Sub
        End Try

        ''Find next index and save data to record
        ''Dim thisIndex As Integer = CInt(counters.Rows(1)("Number")) '1 = Brands row

        brands.Rows.Add(New Object() {brand, code, DateTime.Now, txtNotes.Text})

        'Update brand counter
        counters.Rows(1)("Number") = brandCount + 1

        changes = True
        'Update title bar
        frmMain.Text = fileName & "* - C3"

        'Show confirmation message
        Dim message As String = "Added brand " & brand & " successfully."
        If My.Settings.showMessages = True Then
            MsgBox(message, MsgBoxStyle.Question, "Successfully Added Brand")
        End If
        consoleAdd(message)

        'Reload data and close this form
        frmMain.loadData()
        Me.Close()

    End Sub
End Class