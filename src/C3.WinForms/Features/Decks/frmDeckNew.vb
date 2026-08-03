Public Class frmDeckNew

    Private Sub FrmAddDeck_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        numYear.Maximum = Date.Today.Year
        cmbCondition.SelectedIndex = 2
        cmbSignalRatioNR.SelectedIndex = 0
        cmbHeads.SelectedIndex = 1
        cmbWells.SelectedIndex = 0
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim details As DeckDetails = ReadDetails()
        If Not details.Type1 AndAlso
                MsgBox(
                    "Deck does not support normal tapes (Type I). Is this correct?",
                    MsgBoxStyle.YesNo Or MsgBoxStyle.Question,
                    "Verify Deck") <> vbYes Then
            Return
        End If
        If Not details.SpeedNormal AndAlso
                MsgBox(
                    "Deck does not support normal speed (1⅞ IPS). Is this correct?",
                    MsgBoxStyle.YesNo Or MsgBoxStyle.Question,
                    "Verify Deck") <> vbYes Then
            Return
        End If

        Dim result As DeckOperationResult = deckService.Create(details, DateTime.Now)
        If Not result.IsSuccess Then
            MsgBox(result.Message, MsgBoxStyle.Exclamation, "Invalid Deck")
            Return
        End If

        deckCount = deckService.GetAll().Count
        CompleteCatalogueMutation(Me)

        Dim message As String = "Added deck " & result.Deck.Name & " successfully."
        If preferences.ShowMessages Then
            MsgBox(message, MsgBoxStyle.Information, "Deck Added")
        End If
        consoleAdd(message)
        DialogResult = DialogResult.OK
        Close()
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
