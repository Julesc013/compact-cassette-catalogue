Public Class frmTapeNew

    Private NotInheritable Class ModelChoice
        Public Sub New(label As String, value As CassetteModel)
            Me.Label = label
            Me.Value = value
        End Sub

        Public ReadOnly Property Label As String
        Public ReadOnly Property Value As CassetteModel
    End Class

    Private _selectedModel As CassetteModel

    Private Sub FrmAddTape_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        numYear.Maximum = Date.Today.Year
        datRecordedA.MinDate = New DateTime(1963, 8, 30)
        datRecordedB.MinDate = New DateTime(1963, 8, 30)
        datRecordedA.MaxDate = Date.Today
        datRecordedB.MaxDate = Date.Today
        cmbRegion.SelectedIndex = 0
        cmbCondition.SelectedIndex = 2

        Dim choices As New List(Of ModelChoice)()
        For Each value As CassetteModel In cassetteModelService.GetAll()
            Dim brand As Brand = brandService.Find(value.BrandCode)
            Dim brandName As String = If(brand Is Nothing, value.BrandCode, brand.Name)
            choices.Add(New ModelChoice(brandName & " " & value.ModelName, value))
        Next
        cmbModel.DataSource = choices
        cmbModel.DisplayMember = "Label"
        cmbModel.SelectedIndex = -1

        cmbDeckA.Items.Clear()
        cmbDeckB.Items.Clear()
        For Each value As Deck In deckService.GetAll()
            cmbDeckA.Items.Add(value.Name)
            cmbDeckB.Items.Add(value.Name)
        Next
    End Sub

    Private Sub CmbModel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbModel.SelectedIndexChanged
        Dim choice As ModelChoice = TryCast(cmbModel.SelectedItem, ModelChoice)
        _selectedModel = If(choice Is Nothing, Nothing, choice.Value)
        Dim hasModel As Boolean = _selectedModel IsNot Nothing
        numYear.Enabled = hasModel
        numLength.Enabled = hasModel
        cmbRegion.Enabled = hasModel
        grpBasic.Enabled = hasModel
        grpTaped.Enabled = hasModel
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim draft As New TapeDraft(
            If(_selectedModel Is Nothing, Nothing, _selectedModel.Identifier),
            CInt(numYear.Value),
            numLength.Value,
            cmbRegion.Text,
            getCondition(cmbCondition.SelectedIndex),
            chkPackaged.Checked,
            ReadSideA(),
            ReadSideB(),
            txtNotes.Text)
        Dim result As TapeOperationResult = tapeService.CreateMany(
            draft,
            CInt(numBulkAdd.Value),
            DateTime.Now)
        If Not result.IsSuccess Then
            MsgBox(result.Message, MsgBoxStyle.Exclamation, "Cannot Add Tape")
            Return
        End If

        For Each value As Tape In result.Tapes
            consoleAdd("Added tape " & value.ShortIdentifier & " successfully.")
        Next
        If My.Settings.showMessages Then
            Dim message As String = If(
                result.Tapes.Count = 1,
                "Added tape " & result.Tapes(0).ShortIdentifier & " successfully.",
                "Added " & result.Tapes.Count.ToString() & " tapes successfully.")
            MsgBox(message, MsgBoxStyle.Information, "Tape Added")
        End If

        tapeCount = tapes.Rows.Count
        modelCount = models.Rows.Count
        changes = True
        frmMain.Text = fileName & "* - C3"
        frmMain.loadData()
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Function ReadSideA() As TapeSide
        If Not chkTapedA.Checked Then
            Return TapeSide.Empty()
        End If
        Return New TapeSide(
            True,
            txtNameA.Text,
            datRecordedA.Value,
            cmbDeckA.Text,
            cmbInputA.Text,
            CInt(numPeakA.Value),
            cmbNRA.Text,
            chkHXA.Checked,
            chkMPXA.Checked,
            chkDubbedA.Checked,
            cmbSpeedA.Text,
            cmbBiasA.SelectedIndex + 1,
            CInt(numBiasCalA.Value),
            cmbEQA.Text,
            numLevelA.Value,
            numLevelCalA.Value,
            cmbContentsA.Text,
            txtArtistA.Text,
            txtTitleA.Text)
    End Function

    Private Function ReadSideB() As TapeSide
        If Not chkTapedB.Checked Then
            Return TapeSide.Empty()
        End If
        Return New TapeSide(
            True,
            txtNameB.Text,
            datRecordedB.Value,
            cmbDeckB.Text,
            cmbInputB.Text,
            CInt(numPeakB.Value),
            cmbNRB.Text,
            chkHXB.Checked,
            chkMPXB.Checked,
            chkDubbedB.Checked,
            cmbSpeedB.Text,
            cmbBiasB.SelectedIndex + 1,
            CInt(numBiasCalB.Value),
            cmbEQB.Text,
            numLevelB.Value,
            numLevelCalB.Value,
            cmbContentsB.Text,
            txtArtistB.Text,
            txtTitleB.Text)
    End Function

    Private Sub chkTapedA_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedA.CheckedChanged
        If Not chkTapedA.Checked Then
            grpSideA.Enabled = False
            Return
        End If
        If deckService.GetAll().Count = 0 Then
            MsgBox("Add a deck before entering recordings.", MsgBoxStyle.Exclamation, "No Decks")
            chkTapedA.Checked = False
            Return
        End If
        ApplySideADefaults()
        grpSideA.Enabled = True
    End Sub

    Private Sub chkTapedB_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedB.CheckedChanged
        If Not chkTapedB.Checked Then
            grpSideB.Enabled = False
            Return
        End If
        If deckService.GetAll().Count = 0 Then
            MsgBox("Add a deck before entering recordings.", MsgBoxStyle.Exclamation, "No Decks")
            chkTapedB.Checked = False
            Return
        End If
        ApplySideBDefaults()
        grpSideB.Enabled = True
    End Sub

    Private Sub ApplySideADefaults()
        datRecordedA.Value = Date.Today
        cmbDeckA.SelectedIndex = 0
        cmbInputA.SelectedIndex = 10
        cmbNRA.SelectedIndex = 1
        cmbSpeedA.SelectedIndex = 1
        cmbContentsA.SelectedIndex = 0
        numLevelA.Value = 5D
        cmbEQA.SelectedIndex = If(_selectedModel IsNot Nothing AndAlso _selectedModel.TypeNumber = 1, 0, 1)
        cmbBiasA.SelectedIndex = If(_selectedModel Is Nothing, 0, _selectedModel.TypeNumber - 1)
        txtNameA.Clear()
        txtArtistA.Clear()
        txtTitleA.Clear()
        numPeakA.Value = 0D
        numBiasCalA.Value = 0D
        numLevelCalA.Value = 0D
        chkHXA.Checked = False
        chkMPXA.Checked = False
        chkDubbedA.Checked = False
    End Sub

    Private Sub ApplySideBDefaults()
        datRecordedB.Value = Date.Today
        cmbDeckB.SelectedIndex = 0
        cmbInputB.SelectedIndex = 10
        cmbNRB.SelectedIndex = 1
        cmbSpeedB.SelectedIndex = 1
        cmbContentsB.SelectedIndex = 0
        numLevelB.Value = 5D
        cmbEQB.SelectedIndex = If(_selectedModel IsNot Nothing AndAlso _selectedModel.TypeNumber = 1, 0, 1)
        cmbBiasB.SelectedIndex = If(_selectedModel Is Nothing, 0, _selectedModel.TypeNumber - 1)
        txtNameB.Clear()
        txtArtistB.Clear()
        txtTitleB.Clear()
        numPeakB.Value = 0D
        numBiasCalB.Value = 0D
        numLevelCalB.Value = 0D
        chkHXB.Checked = False
        chkMPXB.Checked = False
        chkDubbedB.Checked = False
    End Sub

    Private Sub chkPackaged_CheckedChanged(sender As Object, e As EventArgs) Handles chkPackaged.CheckedChanged
        If chkPackaged.Checked Then
            chkTapedA.Checked = False
            chkTapedB.Checked = False
            grpTaped.Enabled = False
        Else
            grpTaped.Enabled = _selectedModel IsNot Nothing
        End If
    End Sub

End Class
