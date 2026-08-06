Public Class frmTapeNew

    Public ReadOnly Property CreatedKey As String
    Public ReadOnly Property CreatedDisplayName As String
    Public Property SuppressSuccessMessage As Boolean

    'Declare variables
    Dim modelIndex As Integer '0-based position in datatable
    Dim modelCode As String 'Identifier
    Dim modelType As Integer '1 to 4
    Dim number As Integer 'Number of tapes per model

    Private Sub FrmAddTape_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        'Initialise objects
        numYear.Maximum = Date.Today.Year
        datRecordedA.MinDate = CDate("30/8/1963")
        datRecordedB.MinDate = CDate("30/8/1963")
        datRecordedA.MaxDate = Date.Today
        datRecordedB.MaxDate = Date.Today

        'Load defaults
        cmbRegion.SelectedIndex = 0 'Europe
        cmbCondition.SelectedIndex = 2 'Very Good Plus
        'Side specific defaults loaded on check (below)


        'Load data (decks, brands and models)

        deckCount = decks.Rows.Count
        modelCount = models.Rows.Count

        'Load models into combination box
        'Try
        '    'If no models, catch don't crash

        cmbModel.Items.Clear()

        For i As Integer = 0 To modelCount - 1
            Dim row As DataRow = models.Rows(i)

            Dim thisModel As String = CStr(row("Brand")) & " " & CStr(row("Model"))
            cmbModel.Items.Add(thisModel)
        Next

        'Catch
        '    MsgBox("No models. Unhandled non-fatal error.", MsgBoxStyle.Critical, "No Models Error")
        'End Try


        'Load decks into combination boxes
        cmbDeckA.Items.Clear()
        cmbDeckB.Items.Clear()

        For i As Integer = 0 To deckCount - 1
            Dim row As DataRow = decks.Rows(i)

            Dim thisDeck As String = CStr(row("Name"))
            cmbDeckA.Items.Add(thisDeck)
            cmbDeckB.Items.Add(thisDeck)
        Next

    End Sub

    Private Sub CmbModel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbModel.SelectedIndexChanged
        'Get next index and generate code

        ''modelIndex = cmbModel.SelectedIndex 'Order of items in combobox will be same as in records ''NOT ANYMORE, SORTED BOXES
        ''Dim modelRow As DataRow = models.Rows(modelIndex)
        Dim modelRow As DataRow = Nothing
        For Each thisRow As DataRow In models.Rows
            Dim displayName As String = CStr(thisRow("Brand")) & " " & CStr(thisRow("Model"))
            If String.Equals(displayName, cmbModel.Text, StringComparison.Ordinal) Then
                If modelRow IsNot Nothing Then
                    MsgBox(
                        "More than one model has the display name " & cmbModel.Text & ". Use unique brand and model names before adding tapes.",
                        MsgBoxStyle.Exclamation,
                        "Ambiguous Model")
                    grpBasic.Enabled = False
                    grpTaped.Enabled = False
                    Exit Sub
                End If
                modelRow = thisRow
            End If
        Next

        If modelRow Is Nothing Then
            grpBasic.Enabled = False
            grpTaped.Enabled = False
            Exit Sub
        End If

        modelCode = CStr(modelRow("Identifier"))
        modelType = CInt(modelRow("Type"))

        'Get index for updating model-specific counter when new tape added
        Dim modelRowReal As DataRow = models.Rows.Find(modelCode)
        modelIndex = models.Rows.IndexOf(modelRowReal)

        number = NextTapeSequence(tapes, modelCode, CInt(modelRow("Number")))
        'txtNumber.Text = CStr(number)

        numYear.Enabled = True
        numLength.Enabled = True
        cmbRegion.Enabled = True
        grpBasic.Enabled = True
        grpTaped.Enabled = True

    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        modelCount = models.Rows.Count
        tapeCount = tapes.Rows.Count

        Dim bulkAddAmount As Integer = CInt(numBulkAdd.Value)
        Dim addedIdentifiers As New List(Of String)()

        Try 'Try to save tape

            Dim packaged As Boolean = chkPackaged.Checked
            Dim tapedA As Boolean = chkTapedA.Checked
            Dim tapedB As Boolean = chkTapedB.Checked


            'Validate if current tape is okay to save (all boxes filled)

            If cmbModel.Text = Nothing Then
                Throw New Exception("No model selected.")
            End If

            If packaged = False Then

                If tapedA = True And txtNameA.Text = Nothing Then
                    Throw New Exception("No name for side A.")
                End If

                If tapedB = True And txtNameB.Text = Nothing Then
                    Throw New Exception("No name for side B.")
                End If

            End If


            'Get values to be recorded


            'Get and update new Identifier

            Dim year As Integer = CInt(numYear.Value)
            Dim length As Decimal = numLength.Value

            Dim yearString As String = Str(year)
            Dim yearCode As String = yearString.Substring(yearString.Length - 2) 'Last two digits of the year

            Dim lengthCode As String = Str(CInt(length)).Replace(" ", Nothing)

            If lengthCode.Length > 2 Then
                'There is no rounding here so "129 minutes" becomes "X2"
                'Change 100 to X0, 110 to X1, and 190 to X9 (inclusive).
                lengthCode = "X" & lengthCode.Substring(1, 1)
            ElseIf lengthCode.Length < 2 Then
                lengthCode = "0" & lengthCode
            End If

            'GET TAPE MODEL INDEX NUMBER AND ASSEMBLE IDENTIFIERS IN FOR LOOP BELOW


            Dim condition As Integer = getCondition(cmbCondition.SelectedIndex)

            Dim biasCodeA As Integer = cmbBiasA.SelectedIndex + 1
            Dim biasCodeB As Integer = cmbBiasB.SelectedIndex + 1


            'Get values for recorded sides

            'A side values
            Dim peakA As Integer = Nothing
            Dim biasCalA As Integer = Nothing

            Dim nameA As String = Nothing
            Dim recordedA As Date = Nothing
            Dim deckA As String = Nothing
            Dim inputA As String = Nothing
            Dim speedA As String = Nothing

            Dim NRA As String = Nothing
            Dim HXA As Boolean = Nothing
            Dim MPXA As Boolean = Nothing
            Dim dubbedA As Boolean = Nothing

            Dim EQA As String = Nothing
            Dim levelA As Decimal = Nothing
            Dim levelCalA As Decimal = Nothing

            Dim contentsA As String = Nothing
            Dim artistA As String = Nothing
            Dim titleA As String = Nothing

            'B side values
            Dim peakB As Integer = Nothing
            Dim biasCalB As Integer = Nothing

            Dim nameB As String = Nothing
            Dim recordedB As Date = Nothing
            Dim deckB As String = Nothing
            Dim inputB As String = Nothing
            Dim speedB As String = Nothing

            Dim NRB As String = Nothing
            Dim HXB As Boolean = Nothing
            Dim MPXB As Boolean = Nothing
            Dim dubbedB As Boolean = Nothing

            Dim EQB As String = Nothing
            Dim levelB As Decimal = Nothing
            Dim levelCalB As Decimal = Nothing

            Dim contentsB As String = Nothing
            Dim artistB As String = Nothing
            Dim titleB As String = Nothing

            'Only save real values if that side has been marked as recorded
            If packaged = False Then

                If tapedA = True Then

                    nameA = txtNameA.Text
                    recordedA = datRecordedA.Value
                    deckA = cmbDeckA.Text
                    inputA = cmbInputA.Text
                    speedA = cmbSpeedA.Text
                    peakA = CInt(numPeakA.Value)
                    biasCalA = CInt(numBiasCalA.Value)

                    NRA = cmbNRA.Text
                    HXA = chkHXA.Checked
                    MPXA = chkMPXA.Checked
                    dubbedA = chkDubbedA.Checked

                    EQA = cmbEQA.Text
                    levelA = numLevelA.Value
                    levelCalA = numLevelCalA.Value

                    contentsA = cmbContentsA.Text
                    artistA = txtArtistA.Text
                    titleA = txtTitleA.Text

                End If

                If tapedB = True Then

                    nameB = txtNameB.Text
                    recordedB = datRecordedB.Value
                    deckB = cmbDeckB.Text
                    inputB = cmbInputB.Text
                    speedB = cmbSpeedB.Text
                    peakB = CInt(numPeakB.Value)
                    biasCalB = CInt(numBiasCalB.Value)

                    NRB = cmbNRB.Text
                    HXB = chkHXB.Checked
                    MPXB = chkMPXB.Checked
                    dubbedB = chkDubbedB.Checked

                    EQB = cmbEQB.Text
                    levelB = numLevelB.Value
                    levelCalB = numLevelCalB.Value

                    contentsB = cmbContentsB.Text
                    artistB = txtArtistB.Text
                    titleB = txtTitleB.Text

                End If

            End If


            Dim targetModelRow As DataRow = models.Rows.Find(modelCode)
            If targetModelRow Is Nothing Then
                Throw New InvalidOperationException("The selected model no longer exists.")
            End If
            number = NextTapeSequence(tapes, modelCode, CInt(targetModelRow("Number")))
            If number > 999 - (bulkAddAmount - 1) Then
                Throw New InvalidOperationException(
                    "This model has exhausted the three-digit tape sequence. No tapes were added.")
            End If

            'Build the whole batch as detached rows before mutating the catalogue.

            Dim pendingRows As New List(Of DataRow)()

            For bulkAddIndex As Integer = 0 To bulkAddAmount - 1


                'Finish off making the identifiers

                'Add leading zeroes to number-code (then remove extra zeroes)
                Dim sequence As Integer = number + bulkAddIndex
                Dim numberCode As String = "00" & CStr(sequence)
                numberCode = numberCode.Substring(numberCode.Length - 3, 3)

                Dim identifier As String = CStr(modelCode) & yearCode & lengthCode & numberCode 'Format: MMTmmYYLL###
                Dim identifierShort As String = CStr(modelCode) & numberCode 'Format: MMTmm###


                Dim thisTape As DataRow = tapes.NewRow()
                thisTape("Model") = modelCode
                thisTape("Year") = year
                thisTape("Length") = length
                thisTape("Region") = cmbRegion.Text
                thisTape("Number") = sequence
                thisTape("Identifier") = identifier
                thisTape("IdentifierShort") = identifierShort
                thisTape("Condition") = condition
                thisTape("Packaged") = packaged
                thisTape("TapedA") = tapedA
                thisTape("TapedB") = tapedB
                thisTape("NameA") = nameA
                thisTape("RecordedA") = recordedA
                thisTape("DeckA") = deckA
                thisTape("InputA") = inputA
                thisTape("PeakA") = peakA
                thisTape("NRA") = NRA
                thisTape("HXA") = HXA
                thisTape("MPXA") = MPXA
                thisTape("DubbedA") = dubbedA
                thisTape("SpeedA") = speedA
                thisTape("BiasA") = biasCodeA
                thisTape("BiasCalA") = biasCalA
                thisTape("EQA") = EQA
                thisTape("LevelA") = levelA
                thisTape("LevelCalA") = levelCalA
                thisTape("ContentsA") = contentsA
                thisTape("ArtistA") = artistA
                thisTape("TitleA") = titleA
                thisTape("NameB") = nameB
                thisTape("RecordedB") = recordedB
                thisTape("DeckB") = deckB
                thisTape("InputB") = inputB
                thisTape("PeakB") = peakB
                thisTape("NRB") = NRB
                thisTape("HXB") = HXB
                thisTape("MPXB") = MPXB
                thisTape("DubbedB") = dubbedB
                thisTape("SpeedB") = speedB
                thisTape("BiasB") = biasCodeB
                thisTape("BiasCalB") = biasCalB
                thisTape("EQB") = EQB
                thisTape("LevelB") = levelB
                thisTape("LevelCalB") = levelCalB
                thisTape("ContentsB") = contentsB
                thisTape("ArtistB") = artistB
                thisTape("TitleB") = titleB
                thisTape("Date") = DateTime.Now
                thisTape("Notes") = txtNotes.Text
                pendingRows.Add(thisTape)
                addedIdentifiers.Add(identifierShort)

            Next

            CommitTapeBatch(tapes, targetModelRow, counters, pendingRows, number + bulkAddAmount)
            tapeCount = tapes.Rows.Count

            changes = True
            'Update title bar
            frmMain.Text = fileName & "* - C3"


        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Cannot Save Incomplete Tape")
            Exit Sub

        End Try

        For Each identifierShort As String In addedIdentifiers
            Dim message As String = "Added tape " & identifierShort & " successfully."
            If My.Settings.showMessages = True AndAlso bulkAddAmount = 1 Then
                MsgBox(message, MsgBoxStyle.Question, "Successfully Added Tape")
            End If
            consoleAdd(message)
        Next

        'Reload data and close this form
        frmMain.loadData()
        Me.Close()

    End Sub

    Private Sub chkTapedA_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedA.CheckedChanged

        If chkTapedA.Checked = True Then

            deckCount = decks.Rows.Count

            'Check that at least 1 deck exists
            If deckCount >= 1 Then

                'Set defaults
                datRecordedA.Value = Date.Today
                cmbDeckA.SelectedIndex = 0
                'cmbDeckA.SelectedIndex = cmbDeckA.Items.Count - 1 'Latest deck

                cmbInputA.SelectedIndex = 10 'Phone input
                cmbNRA.SelectedIndex = 1 'Dolby B
                cmbSpeedA.SelectedIndex = 1 'Normal speed
                cmbContentsA.SelectedIndex = 0
                numLevelA.Value = CDec(5)

                If modelType = 1 Then 'If normal bias
                    cmbEQA.SelectedIndex = 0 '120us
                Else
                    cmbEQA.SelectedIndex = 1 '70us
                End If
                cmbBiasA.SelectedIndex = modelType - 1

                'The rest of the defaults
                txtNameA.Text = Nothing
                txtArtistA.Text = Nothing
                txtTitleA.Text = Nothing
                numPeakA.Value = 0
                numBiasCalA.Value = 0
                numLevelCalA.Value = 0
                chkHXA.Checked = False
                chkMPXA.Checked = False
                chkDubbedA.Checked = False

                'Enable data entry for side A
                grpSideA.Enabled = True

            Else

                MsgBox("Must add a deck before entering recordings.", MsgBoxStyle.Exclamation, "No decks.")
                chkTapedA.Checked = False

            End If

        Else
            grpSideA.Enabled = False

        End If

    End Sub

    Private Sub chkTapedB_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedB.CheckedChanged

        If chkTapedB.Checked = True Then

            deckCount = decks.Rows.Count

            'Check that at least 1 deck exists
            If deckCount >= 1 Then

                'Set defaults
                datRecordedB.Value = Date.Today
                cmbDeckB.SelectedIndex = 0
                'cmbDeckB.SelectedIndex = cmbDeckB.Items.Count - 1 'Latest deck

                cmbInputB.SelectedIndex = 10 'Phone input
                cmbNRB.SelectedIndex = 1 'Dolby B
                cmbSpeedB.SelectedIndex = 1 'Normal speed
                cmbContentsB.SelectedIndex = 0
                numLevelB.Value = CDec(5)

                If modelType = 1 Then 'If normal bias
                    cmbEQB.SelectedIndex = 0 '120us
                Else
                    cmbEQB.SelectedIndex = 1 '70us
                End If
                cmbBiasB.SelectedIndex = modelType - 1

                'The rest of the defaults
                txtNameB.Text = Nothing
                txtArtistB.Text = Nothing
                txtTitleB.Text = Nothing
                numPeakB.Value = 0
                numBiasCalB.Value = 0
                numLevelCalB.Value = 0
                chkHXB.Checked = False
                chkMPXB.Checked = False
                chkDubbedB.Checked = False

                'Enable data entry for side B
                grpSideB.Enabled = True

            Else

                MsgBox("Must add a deck before entering recordings.", MsgBoxStyle.Exclamation, "No decks.")
                chkTapedB.Checked = False

            End If

        Else
            grpSideB.Enabled = False

        End If

    End Sub

    Private Sub chkPackaged_CheckedChanged(sender As Object, e As EventArgs) Handles chkPackaged.CheckedChanged

        If chkPackaged.Checked = True Then
            chkTapedA.Checked = False
            chkTapedB.Checked = False
            grpTaped.Enabled = False

        Else
            grpTaped.Enabled = True

        End If

    End Sub

    'Private Sub when_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtNotes.KeyDown, MyBase.KeyDown
    '    'Escape keypress (don't trigger accept-button)

    '    If e.KeyCode = Keys.Enter And txtNotes.Focused Then

    '        'Add new line
    '        txtNotes.Text = txtNotes.Text & vbNewLine

    '    End If

    'End Sub
    'Private Sub txtNotes_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtNotes.KeyDown

    '    If e.KeyCode = Keys.Enter Then

    '        'Escape keypress (don't trigger accept-button)
    '        'Add new line
    '        txtNotes.Text = txtNotes.Text & vbNewLine

    '    End If

    'End Sub
    'Private Sub txtNotes_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtNotes.Validating

    '    'Pressing enter in notes textbox does not trigger accept-button.
    '    txtNotes.Text = txtNotes.Text & vbNewLine
    '    e.Cancel = True

    'End Sub

End Class
