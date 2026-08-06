Public Class frmDeckNew
    Private _createdKey As String
    Private _createdDisplayName As String
    Private _validationErrors As ErrorProvider
    Private _cancelButton As Button

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

    Private Sub FrmAddDeck_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AutoScaleMode = AutoScaleMode.Font
        AutoScroll = True
        MinimumSize = Size
        btnAdd.Text = "Add &Deck"
        btnAdd.AccessibleName = "Add Deck"
        btnAdd.AccessibleDescription = "Add this deck to the current catalogue."
        txtManufacturer.AccessibleName = "Deck manufacturer"
        txtModel.AccessibleName = "Deck model or name"
        lblAdd.Text = "Changes are saved with the catalogue."
        _cancelButton = CatalogueUx.AddCancelButton(Me, btnAdd)
        numYear.Maximum = Date.Today.Year

        'Load defaults
        'In future, load from settings and have more defualt options.

        cmbCondition.SelectedIndex = 2
        cmbSignalRatioNR.SelectedIndex = 0
        cmbHeads.SelectedIndex = 1
        cmbWells.SelectedIndex = 0

    End Sub
    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        'Get data to validate
        Dim manufacturer As String = txtManufacturer.Text
        Dim model As String = txtModel.Text
        Dim name As String = manufacturer & " " & model

        Dim type1 As Boolean = chkType1.Checked
        Dim type2 As Boolean = chkType2.Checked
        Dim type3 As Boolean = chkType3.Checked
        Dim type4 As Boolean = chkType4.Checked

        Dim speedSlow As Boolean = chkSpeedSlow.Checked
        Dim speedNormal As Boolean = chkSpeedNormal.Checked
        Dim speedFast As Boolean = chkSpeedFast.Checked

        'Get number of decks already existing
        Dim deckCount As Integer = decks.Rows.Count

        Dim issues As New List(Of ValidationIssue)()
        If String.IsNullOrWhiteSpace(manufacturer) Then
            issues.Add(New ValidationIssue("txtManufacturer", "Enter a deck manufacturer."))
        End If
        If String.IsNullOrWhiteSpace(model) Then
            issues.Add(New ValidationIssue("txtModel", "Enter a deck model or name."))
        End If
        If Not type1 AndAlso Not type2 AndAlso Not type3 AndAlso Not type4 Then
            issues.Add(New ValidationIssue("grpTypes", "Select at least one supported tape type."))
        End If
        If Not speedSlow AndAlso Not speedNormal AndAlso Not speedFast Then
            issues.Add(New ValidationIssue("grpSpeeds", "Select at least one supported tape speed."))
        End If
        For Each row As DataRow In decks.Rows
            If String.Equals(CStr(row("Name")), name, StringComparison.OrdinalIgnoreCase) Then
                issues.Add(New ValidationIssue("txtModel", "Deck " & name & " already exists."))
            End If
        Next
        If _validationErrors Is Nothing Then
            _validationErrors = New ErrorProvider(components)
            _validationErrors.ContainerControl = Me
        End If
        If Not CatalogueWorkflow.ShowValidationIssues(Me, _validationErrors, issues, "Check Deck Details") Then
            Exit Sub
        End If

        'Verify strange data
        If type1 = False Then
            If MsgBox("Deck does not support normal tapes (Type I). Is this correct?", MsgBoxStyle.YesNo, "Verify Data Entry") = vbNo Then
                Exit Sub
            End If
        End If
        If speedNormal = False Then
            If MsgBox("Deck does not support normal speed (1⅞ IPS). Is this correct?", MsgBoxStyle.YesNo, "Verify Data Entry") = vbNo Then
                Exit Sub
            End If
        End If

        ''Find next index and save data to record
        ''Dim thisIndex As Integer = CInt(counters.Rows(0)("Number")) '0 = Decks row

        Dim condition As Integer = getCondition(cmbCondition.SelectedIndex) 'Get condition score
        Dim frequencyMax As Integer = CInt(numFrequencyMax.Value * 1000)
        Dim heads As Integer = cmbHeads.SelectedIndex + 1
        Dim wells As Integer = cmbWells.SelectedIndex + 1

        Dim deckRow As DataRow = decks.NewRow()
        deckRow("Manufacturer") = manufacturer
        deckRow("Model") = model
        deckRow("Name") = name
        deckRow("Year") = numYear.Value
        deckRow("Condition") = condition
        deckRow("Type1") = type1
        deckRow("Type2") = type2
        deckRow("Type3") = type3
        deckRow("Type4") = type4
        deckRow("HX") = chkHX.Checked
        deckRow("MPX") = chkMPX.Checked
        deckRow("DolbyB") = chkDolbyB.Checked
        deckRow("DolbyC") = chkDolbyC.Checked
        deckRow("DolbyS") = chkDolbyS.Checked
        deckRow("DBX1") = chkDBX1.Checked
        deckRow("DBX2") = chkDBX2.Checked
        deckRow("Stereo") = chkStereo.Checked
        deckRow("ProgramSearch") = chkSearch.Checked
        deckRow("Reverse") = chkReverse.Checked
        deckRow("Calibration") = chkCalibration.Checked
        deckRow("Azimuth") = chkAzimuth.Checked
        deckRow("DubbingSlow") = chkDubbingHalf.Checked
        deckRow("DubbingFast") = chkDubbingDouble.Checked
        deckRow("FrequencyLow") = numFrequencyMin.Value
        deckRow("FrequencyHigh") = frequencyMax
        deckRow("SignalRatio") = numSignalRatio.Value
        deckRow("SignalRatioNR") = cmbSignalRatioNR.Text
        deckRow("WowFlutter") = numWowFlutter.Value
        deckRow("Distortion") = numDistortion.Value
        deckRow("Heads") = heads
        deckRow("Wells") = wells
        deckRow("SpeedSlow") = speedSlow
        deckRow("SpeedNorm") = speedNormal
        deckRow("SpeedFast") = speedFast
        deckRow("Date") = DateTime.Now
        deckRow("Notes") = txtNotes.Text
        decks.Rows.Add(deckRow)

        'Update deck counter
        SynchronizeEntityCounters(counters, decks, brands, models, tapes)
        deckCount = decks.Rows.Count

        changes = True

        _createdKey = name
        _createdDisplayName = name

        'Show confirmation message
        Dim message As String = "Added deck " & name & " successfully."
        If My.Settings.showMessages AndAlso Not SuppressSuccessMessage Then
            MessageBox.Show(Me, message, "Deck Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        consoleAdd(message)

        DialogResult = DialogResult.OK
        Close()

    End Sub

End Class
