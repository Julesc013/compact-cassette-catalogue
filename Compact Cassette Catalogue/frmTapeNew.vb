Public Class frmTapeNew

    Private _createdKey As String
    Private _createdDisplayName As String
    Private _validationErrors As ErrorProvider

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
    Public Property PreferredModelIdentifier As String
    Public Property PreferredDeckName As String

    'Declare variables
    Dim modelIndex As Integer '0-based position in datatable
    Dim modelCode As String 'Identifier
    Dim modelType As Integer '1 to 4
    Dim number As Integer 'Number of tapes per model

    Private Sub FrmAddTape_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigureCreationActions()

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

        ReloadModelChoices(PreferredModelIdentifier)
        ReloadDeckChoices(PreferredDeckName)
    End Sub

    Private Sub ConfigureCreationActions()
        btnAdd.Text = "&Add Tape"
        btnAdd.AccessibleName = "Add Tape"
        btnAdd.AccessibleDescription = "Add this tape to the current catalogue."
        cmbModel.AccessibleName = "Tape model"
        cmbDeckA.AccessibleName = "Side A recording deck"
        cmbDeckB.AccessibleName = "Side B recording deck"
        chkTapedA.AccessibleName = "Recorded Side A"
        chkTapedB.AccessibleName = "Recorded Side B"
        lblAdd.Text = "Changes are saved with the catalogue."

    End Sub

    Private Sub btnAddModel_Click(sender As Object, e As EventArgs) Handles btnAddModel.Click
        AddModelFromTape()
    End Sub

    Private Sub btnAddDeck_Click(sender As Object, e As EventArgs) Handles btnAddDeck.Click
        AddDeckFromTape()
    End Sub

    Public Sub ReloadModelChoices(preferredIdentifier As String)
        modelCount = models.Rows.Count
        cmbModel.Items.Clear()
        For Each row As DataRow In models.Rows
            cmbModel.Items.Add(New CatalogueChoice(
                CStr(row("Identifier")),
                CStr(row("Brand")) & " " & CStr(row("Model"))))
        Next
        CatalogueWorkflow.SelectChoice(cmbModel, preferredIdentifier)
    End Sub

    Public Sub ReloadDeckChoices(preferredName As String)
        ReloadDeckChoices(preferredName, preferredName)
    End Sub

    Private Sub ReloadDeckChoices(preferredNameA As String, preferredNameB As String)
        deckCount = decks.Rows.Count
        cmbDeckA.Items.Clear()
        cmbDeckB.Items.Clear()
        For Each row As DataRow In decks.Rows
            Dim name As String = CStr(row("Name"))
            cmbDeckA.Items.Add(New CatalogueChoice(name, name))
            cmbDeckB.Items.Add(New CatalogueChoice(name, name))
        Next
        CatalogueWorkflow.SelectChoice(cmbDeckA, preferredNameA)
        CatalogueWorkflow.SelectChoice(cmbDeckB, preferredNameB)
    End Sub

    Public Sub ReloadDeckChoicesForDetour(createdName As String)
        Dim preferredNameA As String = CatalogueWorkflow.SelectedChoiceKey(cmbDeckA)
        Dim preferredNameB As String = CatalogueWorkflow.SelectedChoiceKey(cmbDeckB)
        If chkTapedB.Checked AndAlso Not chkTapedA.Checked Then
            preferredNameB = createdName
        ElseIf chkTapedA.Checked AndAlso chkTapedB.Checked Then
            preferredNameA = createdName
            preferredNameB = createdName
        Else
            preferredNameA = createdName
        End If
        ReloadDeckChoices(preferredNameA, preferredNameB)
    End Sub

    Public Sub AddModelFromTape()
        Dim createdModel As CatalogueCreationResult = CatalogueWorkflow.CreateModelForDetour(Me)
        If createdModel Is Nothing Then
            Return
        End If
        ReloadModelChoices(createdModel.Key)
        cmbModel.Focus()
    End Sub

    Public Sub AddDeckFromTape()
        Dim createdDeck As CatalogueCreationResult = CatalogueWorkflow.CreateDeckForDetour(Me)
        If createdDeck Is Nothing Then
            Return
        End If
        ReloadDeckChoicesForDetour(createdDeck.Key)
        cmbDeckA.Focus()
    End Sub

    Private Sub CmbModel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbModel.SelectedIndexChanged
        'Get next index and generate code

        Dim selectedIdentifier As String = CatalogueWorkflow.SelectedChoiceKey(cmbModel)
        Dim modelRow As DataRow = If(
            String.IsNullOrWhiteSpace(selectedIdentifier),
            Nothing,
            models.Rows.Find(selectedIdentifier))

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

        Dim issues As New List(Of ValidationIssue)()
        If String.IsNullOrWhiteSpace(CatalogueWorkflow.SelectedChoiceKey(cmbModel)) Then
            issues.Add(New ValidationIssue("cmbModel", "Select a tape model."))
        End If
        If Not chkPackaged.Checked Then
            If chkTapedA.Checked AndAlso String.IsNullOrWhiteSpace(txtNameA.Text) Then
                issues.Add(New ValidationIssue("txtNameA", "Enter a label for recorded side A."))
            End If
            If chkTapedB.Checked AndAlso String.IsNullOrWhiteSpace(txtNameB.Text) Then
                issues.Add(New ValidationIssue("txtNameB", "Enter a label for recorded side B."))
            End If
            If chkTapedA.Checked AndAlso String.IsNullOrWhiteSpace(CatalogueWorkflow.SelectedChoiceKey(cmbDeckA)) Then
                issues.Add(New ValidationIssue("cmbDeckA", "Select the deck used for side A."))
            End If
            If chkTapedB.Checked AndAlso String.IsNullOrWhiteSpace(CatalogueWorkflow.SelectedChoiceKey(cmbDeckB)) Then
                issues.Add(New ValidationIssue("cmbDeckB", "Select the deck used for side B."))
            End If
        End If
        If _validationErrors Is Nothing Then
            _validationErrors = New ErrorProvider(components)
            _validationErrors.ContainerControl = Me
        End If
        If Not CatalogueWorkflow.ShowValidationIssues(Me, _validationErrors, issues, "Check Tape Details") Then
            Exit Sub
        End If

        Try 'Try to save tape

            Dim packaged As Boolean = chkPackaged.Checked
            Dim tapedA As Boolean = chkTapedA.Checked
            Dim tapedB As Boolean = chkTapedB.Checked


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
                    deckA = CatalogueWorkflow.SelectedChoiceKey(cmbDeckA)
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
                    deckB = CatalogueWorkflow.SelectedChoiceKey(cmbDeckB)
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

            _createdKey = addedIdentifiers(0)
            _createdDisplayName = If(
                addedIdentifiers.Count = 1,
                addedIdentifiers(0),
                addedIdentifiers.Count.ToString(Globalization.CultureInfo.InvariantCulture) & " tapes")


        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Unable to Add Tape", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub

        End Try

        For Each identifierShort As String In addedIdentifiers
            Dim message As String = "Added tape " & identifierShort & " successfully."
            If My.Settings.showMessages AndAlso bulkAddAmount = 1 AndAlso Not SuppressSuccessMessage Then
                MessageBox.Show(Me, message, "Tape Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            consoleAdd(message)
        Next

        DialogResult = DialogResult.OK
        Close()

    End Sub

    Private Sub chkTapedA_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedA.CheckedChanged

        If chkTapedA.Checked = True Then

            deckCount = decks.Rows.Count

            If deckCount = 0 Then
                Dim createdDeck As CatalogueCreationResult = CatalogueWorkflow.CreateDeckForDetour(Me)
                If createdDeck Is Nothing Then
                    chkTapedA.Checked = False
                    Return
                End If
                ReloadDeckChoices(createdDeck.Key)
                deckCount = decks.Rows.Count
            End If

            'Check that at least 1 deck exists
            If deckCount >= 1 Then

                'Set defaults
                datRecordedA.Value = Date.Today
                If cmbDeckA.SelectedIndex < 0 Then
                    cmbDeckA.SelectedIndex = 0
                End If
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

                chkTapedA.Checked = False

            End If

        Else
            grpSideA.Enabled = False

        End If

    End Sub

    Private Sub chkTapedB_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedB.CheckedChanged

        If chkTapedB.Checked = True Then

            deckCount = decks.Rows.Count

            If deckCount = 0 Then
                Dim createdDeck As CatalogueCreationResult = CatalogueWorkflow.CreateDeckForDetour(Me)
                If createdDeck Is Nothing Then
                    chkTapedB.Checked = False
                    Return
                End If
                ReloadDeckChoices(createdDeck.Key)
                deckCount = decks.Rows.Count
            End If

            'Check that at least 1 deck exists
            If deckCount >= 1 Then

                'Set defaults
                datRecordedB.Value = Date.Today
                If cmbDeckB.SelectedIndex < 0 Then
                    cmbDeckB.SelectedIndex = 0
                End If
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
