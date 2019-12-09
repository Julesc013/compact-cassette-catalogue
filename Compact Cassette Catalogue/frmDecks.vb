Public Class frmDecks
    Private Sub FrmViewDecks_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Intialise objects.
        cmbNR.SelectedIndex = 0
        cmbTypes.SelectedIndex = 0

        ' Load list.
        loadList()

    End Sub
    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        loadList()
    End Sub

    Private Sub loadList()
        ' Load Data from the DataSet into the ListView.

        Dim validRow As Boolean
        ' Count the number of results.
        Dim resultsCount As Integer = 0


        ' Get filter values
        Dim critManufacturer As String = txtManufacturer.Text
        Dim critFrequencyMax As Integer = Cint(numFrequencyMax.Value * 1000)
        Dim critNR As String = CStr(cmbNR.Text)
        Dim critHX As Boolean = chkHX.Checked
        Dim critMPX As Boolean = chkMPX.Checked
        Dim critType As Integer = cmbTypes.SelectedIndex
        Dim critCalibration As Boolean = chkCalibration.Checked

        ' Clear the ListView control.
        lstDecks.Items.Clear()

        ' Display items in the ListView control.
        For i As Integer = 0 To decks.Rows.Count - 1

            ' Declare holder variables.
            Dim manufacturer As String
            Dim model As String
            Dim year As Integer
            Dim types As New List(Of Integer)
            Dim heads As Integer
            Dim speeds As New List(Of String)
            Dim NRs As New List(Of String)
            Dim HX As Boolean
            Dim MPX As Boolean
            Dim stereo As Boolean
            Dim wells As Integer
            Dim dubbing As New List(Of String)
            Dim reverse As Boolean
            Dim search As Boolean
            Dim calibration As Boolean
            Dim azimuth As Boolean
            Dim frequencyLow As Integer
            Dim frequencyHigh As Integer
            Dim signalRatio As Integer
            Dim signalRatioNR As String
            Dim wowFlutter As Integer
            Dim distortion As Integer
            Dim condition As Integer


            Dim thisRow As DataRow = decks.Rows(i)

            ' Initialise this row as a valid result given criteria.
            validRow = True

            ' Only rows that have Not been deleted.
            If thisRow.RowState <> DataRowState.Deleted Then

                ' Get data from row.
                manufacturer = thisRow("Manufacturer").ToString
                model = thisRow("Model").ToString
                year = CInt(thisRow("Year"))

                ' Types:
                If CBool(thisRow("Type1")) = True Then
                    types.Add(1)
                End If
                If CBool(thisRow("Type2")) = True Then
                    types.Add(2)
                End If
                If CBool(thisRow("Type3")) = True Then
                    types.Add(3)
                End If
                If CBool(thisRow("Type4")) = True Then
                    types.Add(4)
                End If

                heads = CInt(thisRow("Heads"))

                ' Speeds:
                If CBool(thisRow("SpeedSlow")) = True Then
                    speeds.Add("15/16")
                End If
                If CBool(thisRow("SpeedNorm")) = True Then
                    speeds.Add("1 7/8")
                End If
                If CBool(thisRow("SpeedFast")) = True Then
                    speeds.Add("3 3/4")
                End If

                ' NRs:
                If CBool(thisRow("DolbyB")) = True Then
                    NRs.Add("Dolby B")
                End If
                If CBool(thisRow("DolbyC")) = True Then
                    NRs.Add("Dolby C")
                End If
                If CBool(thisRow("DolbyS")) = True Then
                    NRs.Add("Dolby S")
                End If
                If CBool(thisRow("DBX1")) = True Then
                    NRs.Add("DBX I")
                End If
                If CBool(thisRow("DBX2")) = True Then
                    NRs.Add("DBX II")
                End If

                HX = CBool(thisRow("HX"))
                MPX = CBool(thisRow("MPX"))
                stereo = CBool(thisRow("Stereo"))
                wells = CInt(thisRow("Wells"))

                ' Dubbing:
                If CBool(thisRow("DubbingSlow")) = True Then
                    dubbing.Add("Slow")
                End If
                If CBool(thisRow("DubbingFast")) = True Then
                    dubbing.Add("Fast")
                End If

                reverse = CBool(thisRow("Reverse"))
                search = CBool(thisRow("ProgramSearch"))
                calibration = CBool(thisRow("Calibration"))
                azimuth = CBool(thisRow("Azimuth"))

                frequencyLow = CInt(thisRow("FrequencyLow"))
                frequencyHigh = CInt(thisRow("FrequencyHigh"))
                signalRatio = CInt(thisRow("SignalRatio"))
                signalRatioNR = CStr(thisRow("SignalRatioNR"))
                wowFlutter = CInt(thisRow("WowFlutter"))
                distortion = CInt(thisRow("Distortion"))

                condition = CInt(thisRow("Condition"))


                ' Filter using criteria.

                If critManufacturer <> Nothing And Not manufacturer.ToLower.Contains(critManufacturer.ToLower) Then
                    validRow = False
                End If

                If Not critFrequencyMax <= frequencyHigh Then
                    validRow = False
                End If

                If cmbNR.SelectedIndex <> 0 And Not NRs.Contains(critNR) Then
                    validRow = False
                End If
                If critHX = True And Not HX = critHX Then
                    validRow = False
                End If
                If critMPX = True And Not MPX = critMPX Then
                    validRow = False
                End If

                If cmbTypes.SelectedIndex <> 0 And Not types.Contains(critType) Then
                    validRow = False
                End If

                If critCalibration = True And Not calibration = critCalibration Then
                    validRow = False
                End If

                ' Only rows that fit the filter criteria!
                If validRow = True Then

                    ' Format list items for displaying.
                    Dim dubbingList As String = String.Join(", ", dubbing.ToArray())
                    If dubbingList = Nothing Then
                        dubbingList = "None"
                    End If

                    Dim NRsList As String = String.Join(", ", NRs.ToArray())
                    If NRsList = Nothing Then
                        NRsList = "None"
                    End If

                    ' Define the list items.
                    Dim lstViewItem As ListViewItem = New ListViewItem(manufacturer)
                    lstViewItem.SubItems.Add(model)
                    lstViewItem.SubItems.Add(year.ToString)
                    lstViewItem.SubItems.Add(String.Join(", ", types.ToArray()))
                    lstViewItem.SubItems.Add(heads.ToString)
                    lstViewItem.SubItems.Add(String.Join(", ", speeds.ToArray()))
                    lstViewItem.SubItems.Add(NRsList)
                    lstViewItem.SubItems.Add(HX.ToString)
                    lstViewItem.SubItems.Add(MPX.ToString)
                    lstViewItem.SubItems.Add(stereo.ToString)
                    lstViewItem.SubItems.Add(wells.ToString)
                    lstViewItem.SubItems.Add(dubbingList)
                    lstViewItem.SubItems.Add(reverse.ToString)
                    lstViewItem.SubItems.Add(search.ToString)
                    lstViewItem.SubItems.Add(calibration.ToString)
                    lstViewItem.SubItems.Add(azimuth.ToString)
                    lstViewItem.SubItems.Add(frequencyLow.ToString & "Hz to " & (frequencyHigh / 1000).ToString & "kHz")
                    lstViewItem.SubItems.Add(signalRatio.ToString & "dB with " & signalRatioNR)
                    lstViewItem.SubItems.Add(wowFlutter.ToString & "%")
                    lstViewItem.SubItems.Add(distortion.ToString & "%")
                    lstViewItem.SubItems.Add(getConditionWorded(condition))

                    ' Add the list items to the ListView.
                    lstDecks.Items.Add(lstViewItem)

                    resultsCount += 1

                End If

            End If

        Next

        ' Display number of results.
        txtResults.Text = CStr(resultsCount)

    End Sub

End Class