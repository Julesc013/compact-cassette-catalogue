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

        deckIndex = decks.Rows.IndexOf(deckRow)

        manufacturer = CStr(decks.Rows(deckIndex)("Manufacturer"))
        model = CStr(decks.Rows(deckIndex)("Model"))
        fullName = CStr(decks.Rows(deckIndex)("Name"))
        year = CInt(decks.Rows(deckIndex)("Year"))
        condition = CInt(decks.Rows(deckIndex)("Condition"))

        type1 = CBool(decks.Rows(deckIndex)("Type1"))
        type2 = CBool(decks.Rows(deckIndex)("Type2"))
        type3 = CBool(decks.Rows(deckIndex)("Type3"))
        type4 = CBool(decks.Rows(deckIndex)("Type4"))

        HX = CBool(decks.Rows(deckIndex)("HX"))
        MPX = CBool(decks.Rows(deckIndex)("MPX"))

        DolbyB = CBool(decks.Rows(deckIndex)("DolbyB"))
        DolbyC = CBool(decks.Rows(deckIndex)("DolbyC"))
        DolbyS = CBool(decks.Rows(deckIndex)("DolbyS"))
        DBX1 = CBool(decks.Rows(deckIndex)("DBX1"))
        DBX2 = CBool(decks.Rows(deckIndex)("DBX2"))

        stereo = CBool(decks.Rows(deckIndex)("Stereo"))
        programSearch = CBool(decks.Rows(deckIndex)("ProgramSearch"))
        reverse = CBool(decks.Rows(deckIndex)("Reverse"))
        calibration = CBool(decks.Rows(deckIndex)("Calibration"))
        azimuth = CBool(decks.Rows(deckIndex)("Azimuth"))

        dubbingSlow = CBool(decks.Rows(deckIndex)("DubbingSlow"))
        dubbingFast = CBool(decks.Rows(deckIndex)("DubbingFast"))

        speedSlow = CBool(decks.Rows(deckIndex)("SpeedSlow"))
        speedNormal = CBool(decks.Rows(deckIndex)("SpeedNorm"))
        speedFast = CBool(decks.Rows(deckIndex)("SpeedFast"))

        frequencyMin = CInt(decks.Rows(deckIndex)("FrequencyLow"))
        frequencyMax = CInt(decks.Rows(deckIndex)("FrequencyHigh"))

        signalRatio = CInt(decks.Rows(deckIndex)("SignalRatio"))
        signalRatioNR = CStr(decks.Rows(deckIndex)("SignalRatioNR"))
        wowFlutter = CDec(decks.Rows(deckIndex)("WowFlutter"))
        distortion = CDec(decks.Rows(deckIndex)("Distortion"))

        heads = CInt(decks.Rows(deckIndex)("Heads"))
        wells = CInt(decks.Rows(deckIndex)("Wells"))

        dateCreated = Convert.ToDateTime(decks.Rows(deckIndex)("Date"))
        notes = CStr(decks.Rows(deckIndex)("Notes"))


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


        'Check entered data is correct
        Try

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

        Dim thisDeck As Object() = {manufacturer, model, fullName, numYear.Value, condition, type1, type2, type3, type4, HX, MPX, DolbyB, DolbyC, DolbyS, DBX1, DBX2, stereo, programSearch, reverse, calibration, azimuth, dubbingSlow, dubbingFast, frequencyMin, frequencyMax, signalRatio, signalRatioNR, wowFlutter, distortion, heads, wells, speedSlow, speedNormal, speedFast, dateCreated, notes}

        Dim counter As Integer = 0
        For Each thisObject As Object In thisDeck

            decks.Rows(deckIndex)(counter) = thisObject
            counter += 1

        Next


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