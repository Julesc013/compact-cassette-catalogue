Public Class frmDeckEdit

    Public deckRow As DataRow
    Dim deckIndex As Integer

    Dim manufacturer As String
    Dim model As String
    Dim fullName As String
    Dim year As Integer
    Dim condition As Integer

    Dim type1 As Boolean
    Dim type2 As Boolean
    Dim type3 As Boolean
    Dim type4 As Boolean

    Dim HX As Boolean
    Dim MPX As Boolean

    Dim DolbyB As Boolean
    Dim DolbyC As Boolean
    Dim DolbyS As Boolean
    Dim DBX1 As Boolean
    Dim DBX2 As Boolean

    Dim stereo As Boolean
    Dim programSearch As Boolean
    Dim reverse As Boolean
    Dim calibration As Boolean
    Dim azimuth As Boolean

    Dim dubbingSlow As Boolean
    Dim dubbingFast As Boolean

    Dim speedSlow As Boolean
    Dim speedNormal As Boolean
    Dim speedFast As Boolean

    Dim frequencyMin As Integer
    Dim frequencyMax As Integer
    Dim signalRatio As Integer
    Dim signalRatioNR As String
    Dim wowFlutter As Decimal
    Dim distortion As Decimal

    Dim heads As Integer
    Dim wells As Integer

    Dim dateCreated As DateTime
    Dim notes As String

    Private Sub frmDeckEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Declare variables.

        If deckRow Is Nothing Then
            Throw New InvalidOperationException("Deck Edit requires a catalogue row.")
        End If

        deckIndex = decks.Rows.IndexOf(deckRow)

        manufacturer = CStr(deckRow("Manufacturer"))
        model = CStr(deckRow("Model"))
        fullName = CStr(deckRow("Name"))
        year = CInt(deckRow("Year"))
        condition = CInt(deckRow("Condition"))

        type1 = CBool(deckRow("Type1"))
        type2 = CBool(deckRow("Type2"))
        type3 = CBool(deckRow("Type3"))
        type4 = CBool(deckRow("Type4"))

        HX = CBool(deckRow("HX"))
        MPX = CBool(deckRow("MPX"))

        DolbyB = CBool(deckRow("DolbyB"))
        DolbyC = CBool(deckRow("DolbyC"))
        DolbyS = CBool(deckRow("DolbyS"))
        DBX1 = CBool(deckRow("DBX1"))
        DBX2 = CBool(deckRow("DBX2"))

        stereo = CBool(deckRow("Stereo"))
        programSearch = CBool(deckRow("ProgramSearch"))
        reverse = CBool(deckRow("Reverse"))
        calibration = CBool(deckRow("Calibration"))
        azimuth = CBool(deckRow("Azimuth"))

        dubbingSlow = CBool(deckRow("DubbingSlow"))
        dubbingFast = CBool(deckRow("DubbingFast"))

        speedSlow = CBool(deckRow("SpeedSlow"))
        speedNormal = CBool(deckRow("SpeedNorm"))
        speedFast = CBool(deckRow("SpeedFast"))

        frequencyMin = CInt(deckRow("FrequencyLow"))
        frequencyMax = CInt(deckRow("FrequencyHigh"))

        signalRatio = CInt(deckRow("SignalRatio"))
        signalRatioNR = CStr(deckRow("SignalRatioNR"))
        wowFlutter = CDec(deckRow("WowFlutter"))
        distortion = CDec(deckRow("Distortion"))

        heads = CInt(deckRow("Heads"))
        wells = CInt(deckRow("Wells"))

        dateCreated = Convert.ToDateTime(deckRow("Date"))
        notes = CStr(deckRow("Notes"))


        ' Populate objects.

        txtManufacturer.Text = manufacturer
        txtModel.Text = model
        numYear.Value = year
        cmbCondition.SelectedIndex = getCondition(condition) 'Use condition index

        chkType1.Checked = type1
        chkType2.Checked = type2
        chkType3.Checked = type3
        chkType4.Checked = type4

        chkHX.Checked = HX
        chkMPX.Checked = MPX
        chkDolbyB.Checked = DolbyB
        chkDolbyC.Checked = DolbyC
        chkDolbyS.Checked = DolbyS
        chkDBX1.Checked = DBX1
        chkDBX2.Checked = DBX2

        chkSpeedSlow.Checked = speedSlow
        chkSpeedNormal.Checked = speedNormal
        chkSpeedFast.Checked = speedFast

        chkStereo.Checked = stereo
        chkSearch.Checked = programSearch
        chkReverse.Checked = reverse
        chkCalibration.Checked = calibration
        chkAzimuth.Checked = azimuth
        chkDubbingHalf.Checked = dubbingSlow
        chkDubbingDouble.Checked = dubbingFast

        numFrequencyMin.Value = frequencyMin
        numFrequencyMax.Value = CInt(frequencyMax / 1000)
        numSignalRatio.Value = signalRatio
        cmbSignalRatioNR.Text = signalRatioNR
        numWowFlutter.Value = wowFlutter
        numDistortion.Value = distortion

        cmbHeads.SelectedIndex = heads - 1
        cmbWells.SelectedIndex = wells - 1

        txtNotes.Text = notes


    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click

        'Get data to validate


        manufacturer = txtManufacturer.Text
        model = txtModel.Text
        year = CInt(numYear.Value)
        condition = getCondition(cmbCondition.SelectedIndex) 'Get condition score

        HX = chkHX.Checked
        MPX = chkMPX.Checked
        DolbyB = chkDolbyB.Checked
        DolbyC = chkDolbyC.Checked
        DolbyS = chkDolbyS.Checked
        DBX1 = chkDBX1.Checked
        DBX2 = chkDBX2.Checked

        stereo = chkStereo.Checked
        programSearch = chkSearch.Checked
        reverse = chkReverse.Checked
        calibration = chkCalibration.Checked
        azimuth = chkAzimuth.Checked
        dubbingSlow = chkDubbingHalf.Checked
        dubbingFast = chkDubbingDouble.Checked

        frequencyMin = CInt(numFrequencyMin.Value)
        frequencyMax = CInt(numFrequencyMax.Value * 1000)
        signalRatio = CInt(numSignalRatio.Value)
        signalRatioNR = cmbSignalRatioNR.Text
        wowFlutter = numWowFlutter.Value
        distortion = numDistortion.Value

        type1 = chkType1.Checked
        type2 = chkType2.Checked
        type3 = chkType3.Checked
        type4 = chkType4.Checked

        speedSlow = chkSpeedSlow.Checked
        speedNormal = chkSpeedNormal.Checked
        speedFast = chkSpeedFast.Checked

        heads = cmbHeads.SelectedIndex + 1
        wells = cmbWells.SelectedIndex + 1

        notes = txtNotes.Text

        Dim fullNameNew As String = manufacturer & " " & model


        'Check entered data is correct
        Try

            If String.IsNullOrWhiteSpace(manufacturer) OrElse String.IsNullOrWhiteSpace(model) Then
                Throw New Exception("Manufacturer and model names cannot be empty.")
            End If

            Dim duplicate As DataRow = decks.Rows.Find(fullNameNew)
            If duplicate IsNot Nothing AndAlso duplicate IsNot deckRow Then
                Throw New Exception("A deck named " & fullNameNew & " already exists.")
            End If

            If type1 = False And type2 = False And type3 = False And type4 = False Then
                'If no types selected
                Throw New Exception("No types selected.")
            End If

            If speedSlow = False And speedNormal = False And speedFast = False Then
                'If no speeds selected
                Throw New Exception("No speeds selected.")
            End If

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


        ' Write new data to existing row.

        If Not String.Equals(fullName, fullNameNew, StringComparison.Ordinal) Then
            RenameDeckReferences(tapes, fullName, fullNameNew)
        End If

        deckRow("Manufacturer") = manufacturer
        deckRow("Model") = model
        deckRow("Name") = fullNameNew
        deckRow("Year") = year
        deckRow("Condition") = condition
        deckRow("Type1") = type1
        deckRow("Type2") = type2
        deckRow("Type3") = type3
        deckRow("Type4") = type4
        deckRow("HX") = HX
        deckRow("MPX") = MPX
        deckRow("DolbyB") = DolbyB
        deckRow("DolbyC") = DolbyC
        deckRow("DolbyS") = DolbyS
        deckRow("DBX1") = DBX1
        deckRow("DBX2") = DBX2
        deckRow("Stereo") = stereo
        deckRow("ProgramSearch") = programSearch
        deckRow("Reverse") = reverse
        deckRow("Calibration") = calibration
        deckRow("Azimuth") = azimuth
        deckRow("DubbingSlow") = dubbingSlow
        deckRow("DubbingFast") = dubbingFast
        deckRow("FrequencyLow") = frequencyMin
        deckRow("FrequencyHigh") = frequencyMax
        deckRow("SignalRatio") = signalRatio
        deckRow("SignalRatioNR") = signalRatioNR
        deckRow("WowFlutter") = wowFlutter
        deckRow("Distortion") = distortion
        deckRow("Heads") = heads
        deckRow("Wells") = wells
        deckRow("SpeedSlow") = speedSlow
        deckRow("SpeedNorm") = speedNormal
        deckRow("SpeedFast") = speedFast
        deckRow("Notes") = notes
        fullName = fullNameNew


        changes = True
        'Update title bar
        frmMain.Text = fileName & "* - C3"

        'Show confirmation message
        Dim message As String = "Updated deck " & fullName & " successfully."
        If My.Settings.showMessages = True Then
            MsgBox(message, MsgBoxStyle.Question, "Successfully Updated Deck")
        End If
        consoleAdd(message)

        ' Reload data.
        frmMain.loadData() ' Reload main form.
        frmDecks.loadList() ' Reload decks list. (Decks form will always be open while this form is.)

        Me.Close() ' Close this form.

    End Sub

End Class
