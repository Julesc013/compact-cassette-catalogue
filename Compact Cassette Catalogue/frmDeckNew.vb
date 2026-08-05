Public Class frmDeckNew
    Private Sub FrmAddDeck_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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

        'Check entered data is correct
        Try

            If manufacturer = Nothing Then ''Or Not regexAlphanumeric.IsMatch(manufacturer) Then
                'If nothing or not alphanumeric
                Throw New Exception("Manufacturer name cannot be empty or include symbols.")
            End If

            If model = Nothing Then ''Or Not regexAlphanumeric.IsMatch(model) Then
                'If nothing or not alphanumeric
                Throw New Exception("Model name cannot be empty or include symbols.")
            End If

            If type1 = False And type2 = False And type3 = False And type4 = False Then
                'If no types selected
                Throw New Exception("No types selected.")
            End If

            If speedSlow = False And speedNormal = False And speedFast = False Then
                'If no speeds selected
                Throw New Exception("No speeds selected.")
            End If

            'Check if this deck is already added
            For i As Integer = 0 To deckCount - 1
                Dim row As DataRow = decks.Rows(i)

                Dim thisName As String = CStr(row("Name"))

                If thisName = name Then
                    'If has same name
                    Throw New Exception("Name must be unique." & vbNewLine & thisName & " already exists.")
                End If

            Next

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Invalid Data Entry")
            Exit Sub
        End Try

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
        'Update title bar
        frmMain.Text = fileName & "* - C3"

        'Show confirmation message
        Dim message As String = "Added deck " & name & " successfully."
        If My.Settings.showMessages = True Then
            MsgBox(message, MsgBoxStyle.Question, "Successfully Added Deck")
        End If
        consoleAdd(message)

        'Reload data and close this form
        frmMain.loadData()
        Me.Close()

    End Sub

End Class
