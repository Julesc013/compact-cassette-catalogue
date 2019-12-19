Public Class frmModelEdit

    Public modelRow As DataRow
    Dim modelIndex As Integer
    Dim modelBrand As String
    Dim modelType As Integer
    Dim modelModel As String
    Dim modelCode As String
    Dim modelIdentifier As String
    Dim modelName As String
    Dim modelNumber As Integer
    Dim modelDateTime As DateTime
    Dim modelNotes As String

    Private Sub frmModelEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Declare variables.

        modelIndex = models.Rows.IndexOf(modelRow)
        modelBrand = CStr(models.Rows(modelIndex)("Brand"))
        modelType = CInt(models.Rows(modelIndex)("Type"))
        modelModel = CStr(models.Rows(modelIndex)("Model"))
        modelCode = CStr(models.Rows(modelIndex)("Code"))
        modelIdentifier = CStr(models.Rows(modelIndex)("Identifier"))
        modelName = CStr(models.Rows(modelIndex)("Name"))
        modelNumber = CInt(models.Rows(modelIndex)("Number"))
        modelDateTime = Convert.ToDateTime(models.Rows(modelIndex)("Date"))
        modelNotes = CStr(brands.Rows(modelIndex)("Notes"))


        ' Populate objects.

        txtBrand.Text = modelBrand
        txtType.Text = getTypeNumeral(modelType, True)
        txtModel.Text = modelModel
        txtCode.Text = modelCode
        txtName.Text = modelName
        txtNotes.Text = modelNotes


    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click

        'Get data to validate
        Dim modelModelNew As String = txtModel.Text
        Dim modelNameNew As String = txtName.Text
        Dim modelNotesNew As String = txtNotes.Text

        'Check entered data is correct
        Try

            If modelModelNew = Nothing Then ''Or Not regexAlphanumeric.IsMatch(model) Then
                'If nothing or not alphanumeric
                Throw New Exception("Model name cannot be empty or include symbols.")
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Invalid Data Entry")
            Exit Sub
        End Try


        ' Write new data to existing row.

        Dim thisModel As Object() = {modelBrand, modelType, modelModelNew, modelCode, modelIdentifier, modelNameNew, modelNumber, modelDateTime, modelNotesNew} ' Keep existing fields except name and notes.

        Dim counter As Integer = 0
        For Each thisObject As Object In thisModel

            models.Rows(modelIndex)(counter) = thisObject
            counter += 1

        Next


        changes = True
        'Update title bar
        frmMain.Text = fileName & "* - C3"

        'Show confirmation message
        Dim message As String = "Updated model " & modelBrand & " " & modelModelNew & " successfully."
        If My.Settings.showMessages = True Then
            MsgBox(message, MsgBoxStyle.Question, "Successfully Updated Model")
        End If
        consoleAdd(message)

        ' Reload data.
        frmMain.loadData() ' Reload main form.
        frmModels.loadList() ' Reload models list. (Models form will always be open while this form is.)

        Me.Close() ' Close this form.

    End Sub

End Class