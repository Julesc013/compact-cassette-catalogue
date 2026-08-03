Public Class frmDecks

    Private ReadOnly _selectedNames As New List(Of String)()

    Private Sub FrmViewDecks_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbNR.SelectedIndex = 0
        cmbTypes.SelectedIndex = 0
        loadList()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        loadList()
    End Sub

    Public Sub loadList()
        Dim results As New List(Of Deck)()
        Dim manufacturerFilter As String = txtManufacturer.Text
        Dim minimumFrequency As Integer = CInt(numFrequencyMax.Value * 1000D)
        Dim noiseReductionFilter As String = cmbNR.Text
        Dim typeFilter As Integer = cmbTypes.SelectedIndex

        For Each value As Deck In deckService.GetAll()
            Dim details As DeckDetails = value.Details
            If Not ContainsText(details.Manufacturer, manufacturerFilter) Then
                Continue For
            End If
            If details.FrequencyHigh < minimumFrequency Then
                Continue For
            End If
            If cmbNR.SelectedIndex > 0 AndAlso Not GetNoiseReductions(details).Contains(noiseReductionFilter) Then
                Continue For
            End If
            If chkHX.Checked AndAlso Not details.Hx Then
                Continue For
            End If
            If chkMPX.Checked AndAlso Not details.Mpx Then
                Continue For
            End If
            If typeFilter > 0 AndAlso Not SupportedTypes(details).Contains(typeFilter) Then
                Continue For
            End If
            If chkCalibration.Checked AndAlso Not details.Calibration Then
                Continue For
            End If
            results.Add(value)
        Next

        lstDecks.BeginUpdate()
        Try
            lstDecks.Items.Clear()
            For Each value As Deck In results
                Dim details As DeckDetails = value.Details
                Dim types As List(Of Integer) = SupportedTypes(details)
                Dim speeds As List(Of String) = SupportedSpeeds(details)
                Dim noiseReductions As List(Of String) = GetNoiseReductions(details)
                Dim dubbing As List(Of String) = DubbingSpeeds(details)

                Dim item As New ListViewItem(details.Manufacturer)
                item.Tag = value.Name
                item.SubItems.Add(details.Model)
                item.SubItems.Add(details.Year.ToString())
                item.SubItems.Add(String.Join(", ", types.ToArray()))
                item.SubItems.Add(details.Heads.ToString())
                item.SubItems.Add(String.Join(", ", speeds.ToArray()))
                item.SubItems.Add(FormatList(noiseReductions))
                item.SubItems.Add(details.Hx.ToString())
                item.SubItems.Add(details.Mpx.ToString())
                item.SubItems.Add(details.Stereo.ToString())
                item.SubItems.Add(details.Wells.ToString())
                item.SubItems.Add(FormatList(dubbing))
                item.SubItems.Add(details.Reverse.ToString())
                item.SubItems.Add(details.ProgramSearch.ToString())
                item.SubItems.Add(details.Calibration.ToString())
                item.SubItems.Add(details.Azimuth.ToString())
                item.SubItems.Add(
                    details.FrequencyLow.ToString() & "Hz to " &
                    (details.FrequencyHigh / 1000).ToString() & "kHz")
                item.SubItems.Add(
                    details.SignalRatio.ToString() & "dB with " &
                    details.SignalRatioNoiseReduction)
                item.SubItems.Add(details.WowFlutter.ToString() & "%")
                item.SubItems.Add(details.Distortion.ToString() & "%")
                item.SubItems.Add(getConditionWorded(details.Condition))
                lstDecks.Items.Add(item)
            Next
        Finally
            lstDecks.EndUpdate()
        End Try

        txtResults.Text = results.Count.ToString()
    End Sub

    Private Shared Function ContainsText(value As String, filter As String) As Boolean
        Return String.IsNullOrWhiteSpace(filter) OrElse
            If(value, String.Empty).IndexOf(filter.Trim(), StringComparison.CurrentCultureIgnoreCase) >= 0
    End Function

    Private Shared Function SupportedTypes(value As DeckDetails) As List(Of Integer)
        Dim result As New List(Of Integer)()
        If value.Type1 Then result.Add(1)
        If value.Type2 Then result.Add(2)
        If value.Type3 Then result.Add(3)
        If value.Type4 Then result.Add(4)
        Return result
    End Function

    Private Shared Function SupportedSpeeds(value As DeckDetails) As List(Of String)
        Dim result As New List(Of String)()
        If value.SpeedSlow Then result.Add("15/16")
        If value.SpeedNormal Then result.Add("1 7/8")
        If value.SpeedFast Then result.Add("3 3/4")
        Return result
    End Function

    Private Shared Function GetNoiseReductions(value As DeckDetails) As List(Of String)
        Dim result As New List(Of String)()
        If value.DolbyB Then result.Add("Dolby B")
        If value.DolbyC Then result.Add("Dolby C")
        If value.DolbyS Then result.Add("Dolby S")
        If value.Dbx1 Then result.Add("DBX I")
        If value.Dbx2 Then result.Add("DBX II")
        Return result
    End Function

    Private Shared Function DubbingSpeeds(value As DeckDetails) As List(Of String)
        Dim result As New List(Of String)()
        If value.DubbingSlow Then result.Add("Slow")
        If value.DubbingFast Then result.Add("Fast")
        Return result
    End Function

    Private Shared Function FormatList(values As List(Of String)) As String
        If values.Count = 0 Then
            Return "None"
        End If
        Return String.Join(", ", values.ToArray())
    End Function

    Private Sub lstDecks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstDecks.SelectedIndexChanged
        _selectedNames.Clear()
        For Each item As ListViewItem In lstDecks.SelectedItems
            _selectedNames.Add(CStr(item.Tag))
        Next
        btnDelete.Enabled = _selectedNames.Count > 0
        btnEdit.Enabled = _selectedNames.Count = 1
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If _selectedNames.Count = 0 Then
            Return
        End If

        Dim prompt As String = "Delete the selected " & _selectedNames.Count.ToString() &
            " deck(s)?" & vbNewLine & "This action cannot be undone."
        If MsgBox(prompt, MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirm Deck Deletion") <> vbYes Then
            Return
        End If

        Dim failures As New List(Of String)()
        Dim changed As Boolean = False
        For Each name As String In _selectedNames
            Dim result As DeckOperationResult = deckService.Delete(name)
            If result.IsSuccess Then
                changed = True
                consoleAdd("Deleted deck " & name & " successfully.")
            Else
                failures.Add(name & ": " & result.Message)
            End If
        Next

        If changed Then
            deckCount = deckService.GetAll().Count
            CompleteCatalogueMutation(Me)
        End If
        If failures.Count > 0 Then
            MsgBox(String.Join(vbNewLine, failures.ToArray()), MsgBoxStyle.Exclamation, "Some Decks Were Not Deleted")
        End If

        loadList()
        btnEdit.Enabled = False
        btnDelete.Enabled = False
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If _selectedNames.Count <> 1 Then
            Return
        End If

        Using editor As New frmDeckEdit()
            editor.DeckName = _selectedNames(0)
            editor.ShowDialog(Me)
        End Using
        loadList()
    End Sub

End Class
