Public Class frmTapes

    'Dim nullString As String = Convert.ToChar(&H0) ' An object containing this tring is considered to be "empty" or to be containing "nothing". 'This is temporary.

    Private Sub frmViewTapes_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Initialise objects.

        ' Initialise boundary values.
        datRecordedMin.MaxDate = Date.Today
        datRecordedMax.MaxDate = Date.Today
        datRecordedMax.Value = Date.Today
        numYearMin.Maximum = Date.Today.Year
        numYearMax.Maximum = Date.Today.Year
        numYearMax.Value = Date.Today.Year
        datRecordedMin.MaxDate = Date.Today
        datRecordedMax.MaxDate = Date.Today
        datRecordedMax.Value = Date.Today

        cmbBrand.SelectedIndex = 0
        cmbTypes.SelectedIndex = 0
        cmbModel.SelectedIndex = 0
        cmbCondition.SelectedIndex = 0
        cmbDeck.SelectedIndex = 0
        cmbNR.SelectedIndex = 0
        cmbContents.SelectedIndex = 0


        ' Populate objects.


        ' Load brands into combination box.
        Dim brandCount As Integer = CInt(counters.Rows(1)("Number"))
        For i As Integer = 0 To brandCount - 1
            Dim thisBrand As String = CStr(brands.Rows(i)("Brand"))
            cmbBrand.Items.Add(thisBrand)
        Next

        ' Load decks into combination boxes.
        Try ' If no decks, catch don't crash.
            For i As Integer = 0 To deckCount - 1
                Dim row As DataRow = decks.Rows(i)

                Dim thisDeck As String = CStr(row("Name"))
                cmbDeck.Items.Add(thisDeck)
            Next
        Catch
        End Try


        loadList()

    End Sub

    Private Sub loadList()
        ' Load Data from the DataSet into the ListView.

        Dim validRow As Boolean
        ' Count the number of results.
        Dim resultsCount As Integer = 0


        ' Get filter values
        Dim critBrand As String = cmbBrand.Text
        Dim critType As Integer = cmbTypes.SelectedIndex
        Dim critTypeBetter As Boolean = chkTypeBetter.Checked
        Dim critModel As String = cmbModel.Text
        Dim critLengthMin As Integer = CInt(numLengthMin.Value)
        Dim critLengthMax As Integer = CInt(numLengthMax.Value)
        Dim critYearMin As Integer = CInt(numYearMin.Value)
        Dim critYearMax As Integer = CInt(numYearMax.Value)

        Dim critName As String = txtName.Text
        Dim critCondition As Integer = getCondition(cmbCondition.SelectedIndex - 1)
        Dim critConditionBetter As Boolean = chkConditionBetter.Checked
        Dim critDeck As String = cmbDeck.Text
        Dim critRecorded As Boolean = chkRecorded.Checked

        Dim critRecordingMinBin As Long = datRecordedMin.Value.ToBinary
        Dim critRecordingMaxBin As Long = datRecordedMax.Value.ToBinary
        ' Convert the signed long to an unsigned long.
        Dim critRecordingMinBinUns As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(critRecordingMinBin), 0)
        Dim critRecordingMaxBinUns As ULong = BitConverter.ToUInt64(BitConverter.GetBytes(critRecordingMaxBin), 0)
        'Dim critRecordingMin As Date = Date.Parse(critRecordingMinTemp)
        'Dim critRecordingMax As Date = Date.Parse(critRecordingMaxTemp)

        Dim critNR As String = cmbNR.Text
        Dim critPackaged As Boolean = chkPackaged.Checked
        Dim critNotes As String = txtNotes.Text

        Dim critContents As String = cmbContents.Text
        Dim critArtist As String = txtArtist.Text
        Dim critTitle As String = txtTitle.Text


        ' Clear the ListView control.
        lstTapes.Items.Clear()

        ' Display items in the ListView control.
        For i As Integer = 0 To tapes.Rows.Count - 1

            Dim thisRow As DataRow = tapes.Rows(i)

            ' Initialise this row as a valid result given criteria.
            validRow = True

            ' Only rows that have Not been deleted.
            If thisRow.RowState <> DataRowState.Deleted Then

                ' Get data from row.
                Dim identifier As String = thisRow("Identifier").ToString
                Dim identifierShort As String = thisRow("IdentifierShort").ToString

                ' Extract brand, type and model from the model identification code.
                Dim modelCode As String = thisRow("Model").ToString
                Dim modelRow As DataRow = models.Rows.Find(modelCode)
                Dim model As String = CStr(modelRow("Model"))
                Dim brand As String = CStr(modelRow("Brand"))
                Dim type As Integer = CInt(modelCode.Substring(2, 1))

                Dim year As Integer = CInt(thisRow("Year"))
                Dim length As Integer = CInt(thisRow("Length"))
                Dim region As String = thisRow("Region").ToString
                Dim condition As Integer = CInt(thisRow("Condition"))
                Dim packaged As Boolean = CBool(thisRow("Packaged"))


                ' If the tape is not recorded onto, replace the value with a 'null' value.
                Dim recordedsBin As Long() = {Nothing, Nothing}
                Dim recordedsBinUns As ULong() = {Nothing, Nothing}
                'Dim recordeds As Date() = {Nothing, Nothing}
                Dim names As String() = {Nothing, Nothing}
                Dim NRs As String() = {Nothing, Nothing}
                Dim contentss As String() = {Nothing, Nothing}
                Dim deckss As String() = {Nothing, Nothing}
                Dim artists As String() = {vbNullChar, vbNullChar}
                Dim titles As String() = {vbNullChar, vbNullChar}

                recordedsBin(0) = CDate(thisRow("RecordedA")).ToBinary
                recordedsBin(1) = CDate(thisRow("RecordedB")).ToBinary
                ' Convert the signed long to an unsigned long.
                recordedsBinUns(0) = BitConverter.ToUInt64(BitConverter.GetBytes(recordedsBin(0)), 0)
                recordedsBinUns(1) = BitConverter.ToUInt64(BitConverter.GetBytes(recordedsBin(1)), 0)

                ' If a side is not recorded, do not try to load the values for that side.
                If Not recordedsBinUns(0) = Nothing Then 'Side A is not 0.
                    names(0) = CStr(thisRow("NameA"))
                    NRs(0) = CStr(thisRow("NRA"))
                    contentss(0) = CStr(thisRow("ContentsA"))
                    deckss(0) = CStr(thisRow("DeckA"))
                    artists(0) = CStr(thisRow("ArtistA"))
                    titles(0) = CStr(thisRow("TitleA"))
                End If
                If Not recordedsBinUns(1) = Nothing Then 'Side B is not 0.
                    names(1) = CStr(thisRow("NameB"))
                    NRs(1) = CStr(thisRow("NRB"))
                    contentss(1) = CStr(thisRow("ContentsB"))
                    deckss(1) = CStr(thisRow("DeckB"))
                    artists(1) = CStr(thisRow("ArtistB"))
                    titles(1) = CStr(thisRow("TitleB"))
                End If

                'Try
                '    recordedsBin(0) = CDate(thisRow("RecordedA")).ToBinary
                'Catch ex As InvalidCastException
                'End Try
                'Try
                '    recordedsBin(1) = CDate(thisRow("RecordedB")).ToBinary
                'Catch ex As InvalidCastException
                'End Try
                ''Try
                ''    recordeds(0) = CDate(thisRow("RecordedA"))
                ''Catch ex As InvalidCastException
                ''End Try
                ''Try
                ''    recordeds(1) = CDate(thisRow("RecordedB"))
                ''Catch ex As InvalidCastException
                ''End Try

                'Try
                '    NRs(0) = CStr(thisRow("NRA"))
                'Catch ex As InvalidCastException
                'End Try
                'Try
                '    NRs(1) = CStr(thisRow("NRB"))
                'Catch ex As InvalidCastException
                'End Try

                'Try
                '    contentss(0) = CStr(thisRow("ContentsA"))
                'Catch ex As InvalidCastException
                'End Try
                'Try
                '    contentss(1) = CStr(thisRow("ContentsB"))
                'Catch ex As InvalidCastException
                'End Try

                'Try
                '    deckss(0) = CStr(thisRow("DeckA"))
                'Catch ex As InvalidCastException
                'End Try
                'Try
                '    deckss(1) = CStr(thisRow("DeckB"))
                'Catch ex As InvalidCastException
                'End Try

                'Try
                '    artists(0) = CStr(thisRow("ArtistA"))
                'Catch ex As InvalidCastException
                'End Try
                'Try
                '    artists(1) = CStr(thisRow("ArtistB"))
                'Catch ex As InvalidCastException
                'End Try

                'Try
                '    titles(0) = CStr(thisRow("TitleA"))
                'Catch ex As InvalidCastException
                'End Try
                'Try
                '    titles(1) = CStr(thisRow("TitleB"))
                'Catch ex As InvalidCastException
                'End Try

                Dim notes As String = thisRow("Notes").ToString


                ' Filter using criteria.

                If cmbBrand.SelectedIndex <> 0 And Not brand.ToLower = critBrand.ToLower Then
                    validRow = False
                End If

                ' Filter for types.
                If critType <> 0 Then
                    If critTypeBetter = False Then

                        If Not type = critType Then
                            validRow = False
                        End If

                    Else

                        If Not type >= critType Then
                            validRow = False
                        End If

                    End If
                End If

                If cmbModel.SelectedIndex <> 0 And Not model.ToLower = critModel.ToLower Then
                    validRow = False
                End If

                If Not critLengthMin <= length Or Not critLengthMax >= length Then
                    validRow = False
                End If

                If Not critYearMin <= year Or Not critYearMax >= year Then
                    validRow = False
                End If

                If critName <> Nothing Then ' Only discard if both sides don't match.
                    If Not names(0).ToLower.Contains(critName.ToLower) And Not names(1).ToLower.Contains(critName.ToLower) Then
                        validRow = False
                    End If
                End If

                ' Filter for conditions.
                If critCondition <> -1 Then ' Condition index subtracted by 1 to give condition rating.
                    If critConditionBetter = False Then

                        If Not condition = critCondition Then
                            validRow = False
                        End If

                    Else

                        If Not condition >= critCondition Then
                            validRow = False
                        End If

                    End If
                End If

                If cmbDeck.SelectedIndex <> 0 And Not deckss.Contains(critDeck) Then
                    validRow = False
                End If


                ' Filter for recording dates only if recordings exist.

                'If critRecorded = True Then ' Only discard if both sides aren't recorded.
                '    If recordeds(0) = Nothing And recordeds(1) = Nothing Then
                '        validRow = False
                '    End If
                'End If

                If critRecorded = True Then ' Only discard if both sides aren't recorded.

                    If recordedsBinUns(0) = Nothing And recordedsBinUns(1) = Nothing Then
                        validRow = False

                    Else

                        ' If both dates are not within bounds. It IS acceptable to have only one date within bounds.
                        If Not critRecordingMinBinUns < recordedsBinUns(0) And Not critRecordingMinBinUns < recordedsBinUns(1) Then
                            validRow = False
                        End If
                        If Not critRecordingMaxBinUns > recordedsBinUns(0) And Not critRecordingMaxBinUns > recordedsBinUns(1) Then
                            validRow = False
                        End If

                        'If Not critRecordingMin < recordeds(0) And Not critRecordingMin < recordeds(1) Then
                        '    validRow = False
                        'End If
                        'If Not critRecordingMax > recordeds(0) And Not critRecordingMax > recordeds(1) Then
                        '    validRow = False
                        'End If

                    End If

                End If


                If cmbNR.SelectedIndex <> 0 And Not NRs.Contains(critNR) Then
                    validRow = False
                End If

                If critPackaged = True And Not critPackaged = packaged Then
                    validRow = False
                End If

                If critNotes <> Nothing And Not notes.ToLower.Contains(critNotes.ToLower) Then
                    validRow = False
                End If

                If cmbContents.SelectedIndex <> 0 Then ' Only discard if both sides don't match.
                    If Not contentss(0) = critContents And Not contentss(1) = critContents Then
                        validRow = False
                    End If
                End If

                If critArtist <> Nothing Then ' Only discard if both sides don't match.
                    If Not artists(0).ToLower.Contains(critArtist.ToLower) And Not artists(1).ToLower.Contains(critArtist.ToLower) Then
                        validRow = False
                    End If
                End If

                If critTitle <> Nothing Then ' Only discard if both sides don't match.
                    If Not titles(0).ToLower.Contains(critTitle.ToLower) And Not titles(1).ToLower.Contains(critTitle.ToLower) Then
                        validRow = False
                    End If
                End If



                ' Only display rows that fit the filter criteria!
                If validRow = True Then


                    ' Convert data to displayable format.
                    ' Insert an en dash "–" where data is missing.

                    ' Recording dates.
                    'lstViewItem.SubItems.Add(recordeds(0) & ", " & recordeds(1))
                    Dim dateStrA As String
                    If recordedsBin(0) = Nothing Then
                        dateStrA = "–"
                    Else
                        dateStrA = Date.FromBinary(recordedsBin(0)).ToShortDateString
                    End If

                    Dim dateStrB As String
                    If recordedsBin(1) = Nothing Then
                        dateStrB = "–"
                    Else
                        dateStrB = Date.FromBinary(recordedsBin(1)).ToShortDateString
                    End If

                    ' Noise reductions.
                    If NRs(0) = Nothing Then
                        NRs(0) = "–"
                    End If
                    If NRs(1) = Nothing Then
                        NRs(1) = "–"
                    End If

                    ' Contents.
                    If contentss(0) = Nothing Then
                        contentss(0) = "–"
                    End If
                    If contentss(1) = Nothing Then
                        contentss(1) = "–"
                    End If

                    ' Decks.
                    If deckss(0) = Nothing Then
                        deckss(0) = "–"
                    End If
                    If deckss(1) = Nothing Then
                        deckss(1) = "–"
                    End If

                    ' Artists.
                    If artists(0) = Nothing Then
                        artists(0) = "–"
                    End If
                    If artists(1) = Nothing Then
                        artists(1) = "–"
                    End If

                    ' Titles.
                    If titles(0) = Nothing Then
                        titles(0) = "–"
                    End If
                    If titles(1) = Nothing Then
                        titles(1) = "–"
                    End If


                    ' Define the displayable list items.
                    Dim lstViewItem As ListViewItem = New ListViewItem(identifierShort)
                    lstViewItem.SubItems.Add(identifier)
                    lstViewItem.SubItems.Add(names(0) & ", " & names(1))
                    lstViewItem.SubItems.Add(brand)
                    lstViewItem.SubItems.Add(model)
                    lstViewItem.SubItems.Add(getTypeNumeral(type, True))
                    lstViewItem.SubItems.Add(CStr(year))
                    lstViewItem.SubItems.Add(CStr(length))
                    lstViewItem.SubItems.Add(region)
                    lstViewItem.SubItems.Add(getConditionWorded(condition))
                    lstViewItem.SubItems.Add(CStr(packaged))

                    lstViewItem.SubItems.Add(dateStrA & ", " & dateStrB)
                    lstViewItem.SubItems.Add(NRs(0) & ", " & NRs(1))
                    lstViewItem.SubItems.Add(contentss(0) & ", " & contentss(1))
                    lstViewItem.SubItems.Add(deckss(0) & ", " & deckss(1))
                    lstViewItem.SubItems.Add(artists(0) & ", " & artists(1))
                    lstViewItem.SubItems.Add(titles(0) & ", " & titles(1))

                    lstViewItem.SubItems.Add(notes)

                    ' Add the list items to the ListView.
                    lstTapes.Items.Add(lstViewItem)

                    resultsCount += 1

                End If

            End If

        Next

        ' Display number of results.
        txtResults.Text = CStr(resultsCount)

    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        loadList()
    End Sub

    Private Sub cmbTypes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTypes.SelectedIndexChanged

        ' If the selected type is NOT "all types" then enable the ability to check the box below.

        Dim selectedIndex As Integer = cmbTypes.SelectedIndex

        If selectedIndex <> 0 Then ' If it is a specific type.

            ' Convert the Arabic to a Roman numeral.
            Dim typeNumeral As String = getTypeNumeral(selectedIndex, False)
            chkTypeBetter.Text = "Type " & typeNumeral & " or better."
            chkTypeBetter.Enabled = True

        Else

            ' When selected "all types" disable and decheck the box.
            chkTypeBetter.Enabled = False
            chkTypeBetter.Text = "Type I or better."
            chkTypeBetter.Checked = False

        End If

    End Sub

    Private Sub cmbCondition_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCondition.SelectedIndexChanged

        ' If the selected condition is NOT "all conditions" then enable the ability to check the box below.

        Dim selectedIndex As Integer = cmbCondition.SelectedIndex

        If selectedIndex <> 0 Then ' If it is a specific condition.

            ' Convert the Arabic to a Roman numeral.
            Dim condition As String = cmbCondition.Text
            chkConditionBetter.Text = condition & " or better."
            chkConditionBetter.Enabled = True

        Else

            ' When selected "all types" disable and decheck the box.
            chkConditionBetter.Enabled = False
            chkConditionBetter.Text = "Poor or better."
            chkConditionBetter.Checked = False

        End If

    End Sub

    Private Sub cmbBrand_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBrand.SelectedIndexChanged

        ' When brand changes, populate models box with all models from that brand.

        ' Reset items.
        cmbModel.Items.Clear()
        cmbModel.Items.Add("All Models")

        cmbModel.SelectedIndex = 0


        If cmbBrand.SelectedIndex <> 0 Then

            Dim modelCount As Integer = CInt(counters.Rows(2)("Number"))

            ' Load models into combination box.

            Dim selectedBrand As String = cmbBrand.Text

            Dim row As DataRow
            Dim thisBrand As String
            Dim thisModel As String

            For i As Integer = 0 To modelCount - 1

                row = models.Rows(i)
                thisBrand = CStr(row("Brand"))
                thisModel = CStr(row("Model"))

                If thisBrand = selectedBrand Then

                    cmbModel.Items.Add(thisModel)

                End If

            Next

            cmbModel.Enabled = True

        Else

            cmbModel.Enabled = False

        End If

    End Sub

    Private Sub chkRecorded_CheckedChanged(sender As Object, e As EventArgs) Handles chkRecorded.CheckedChanged

        If chkRecorded.Checked = True Then

            datRecordedMin.Enabled = True
            datRecordedMax.Enabled = True

        Else

            datRecordedMin.Enabled = False
            datRecordedMax.Enabled = False

        End If

    End Sub

End Class