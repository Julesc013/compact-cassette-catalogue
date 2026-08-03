Public Class frmDeckEdit

    Public Property DeckName As String

    Private Sub frmDeckEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        numYear.Maximum = Date.Today.Year
        Dim value As Deck = deckService.Find(DeckName)
        If value Is Nothing Then
            MsgBox("The selected deck no longer exists.", MsgBoxStyle.Exclamation, "Deck Not Found")
            DialogResult = DialogResult.Cancel
            Close()
            Return
        End If

        DeckName = value.Name
        WriteDetails(value.Details)
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim details As DeckDetails = ReadDetails()
        If Not details.Type1 AndAlso
                MsgBox(
                    "Deck does not support normal tapes (Type I). Is this correct?",
                    MsgBoxStyle.YesNo Or MsgBoxStyle.Question,
                    "Verify Deck") <> vbYes Then
            Return
        End If

        Dim validation As DeckOperationResult = deckService.Update(DeckName, details)
        If Not validation.IsSuccess Then
            MsgBox(validation.Message, MsgBoxStyle.Exclamation, "Invalid Deck")
            Return
        End If
        If Not details.SpeedNormal AndAlso
                MsgBox(
                    "Deck does not support normal speed (1⅞ IPS). Is this correct?",
                    MsgBoxStyle.YesNo Or MsgBoxStyle.Question,
                    "Verify Deck") <> vbYes Then
            Return
        End If

        changes = True
        frmMain.Text = fileName & "* - C3"
        Dim message As String = "Updated deck " & validation.Deck.Name & " successfully."
        If My.Settings.showMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Deck Updated")
        End If
        consoleAdd(message)
        frmMain.loadData()
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub WriteDetails(value As DeckDetails)
        txtManufacturer.Text = value.Manufacturer
        txtModel.Text = value.Model
        numYear.Value = value.Year
        cmbCondition.SelectedIndex = getCondition(value.Condition)
        chkType1.Checked = value.Type1
        chkType2.Checked = value.Type2
        chkType3.Checked = value.Type3
        chkType4.Checked = value.Type4
        chkHX.Checked = value.Hx
        chkMPX.Checked = value.Mpx
        chkDolbyB.Checked = value.DolbyB
        chkDolbyC.Checked = value.DolbyC
        chkDolbyS.Checked = value.DolbyS
        chkDBX1.Checked = value.Dbx1
        chkDBX2.Checked = value.Dbx2
        chkStereo.Checked = value.Stereo
        chkSearch.Checked = value.ProgramSearch
        chkReverse.Checked = value.Reverse
        chkCalibration.Checked = value.Calibration
        chkAzimuth.Checked = value.Azimuth
        chkDubbingHalf.Checked = value.DubbingSlow
        chkDubbingDouble.Checked = value.DubbingFast
        numFrequencyMin.Value = value.FrequencyLow
        numFrequencyMax.Value = CDec(value.FrequencyHigh) / 1000D
        numSignalRatio.Value = value.SignalRatio
        cmbSignalRatioNR.Text = value.SignalRatioNoiseReduction
        numWowFlutter.Value = value.WowFlutter
        numDistortion.Value = value.Distortion
        cmbHeads.SelectedIndex = value.Heads - 1
        cmbWells.SelectedIndex = value.Wells - 1
        chkSpeedSlow.Checked = value.SpeedSlow
        chkSpeedNormal.Checked = value.SpeedNormal
        chkSpeedFast.Checked = value.SpeedFast
        txtNotes.Text = value.Notes
    End Sub

    Private Function ReadDetails() As DeckDetails
        Return New DeckDetails(
            txtManufacturer.Text,
            txtModel.Text,
            CInt(numYear.Value),
            getCondition(cmbCondition.SelectedIndex),
            chkType1.Checked,
            chkType2.Checked,
            chkType3.Checked,
            chkType4.Checked,
            chkHX.Checked,
            chkMPX.Checked,
            chkDolbyB.Checked,
            chkDolbyC.Checked,
            chkDolbyS.Checked,
            chkDBX1.Checked,
            chkDBX2.Checked,
            chkStereo.Checked,
            chkSearch.Checked,
            chkReverse.Checked,
            chkCalibration.Checked,
            chkAzimuth.Checked,
            chkDubbingHalf.Checked,
            chkDubbingDouble.Checked,
            CInt(numFrequencyMin.Value),
            CInt(numFrequencyMax.Value * 1000D),
            CInt(numSignalRatio.Value),
            cmbSignalRatioNR.Text,
            numWowFlutter.Value,
            numDistortion.Value,
            cmbHeads.SelectedIndex + 1,
            cmbWells.SelectedIndex + 1,
            chkSpeedSlow.Checked,
            chkSpeedNormal.Checked,
            chkSpeedFast.Checked,
            txtNotes.Text)
    End Function

End Class
