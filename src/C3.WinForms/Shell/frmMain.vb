' Module: Main & Edit Tape
' Purpose: To edit existing tapes and access the other functions of the program.
' Author: Jules Carboni
' Date Created: 22 Aug 2019

Imports System.IO
Imports System.Net

Public Class frmMain

    'Declare variables
    Private _allowClose As Boolean
    Private _consoleWindow As frmConsole
    Private _hasPendingTapeEdits As Boolean
    Private _suppressTapeEditTracking As Boolean = True
    Private _currentTapeIndex As Integer = -1
    Private _currentModelType As Integer
    Private _wasSideARecorded As Boolean
    Private _wasSideBRecorded As Boolean
    Private _tapeCount As Integer

    'Dim newTape As Object() = {"", 0, 0, "", 0, "Unsaved", 0, False, False, False, "", CDate("1/1/1970"), "", "", 0, "", False, False, False, 0, 0, "", 0, 0, "", "", "", "", CDate("1/1/1970"), "", "", 0, "", False, False, False, 0, 0, "", 0, 0, "", ""} 'Default record for a new blank tape

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Display about information.
        lblAbout.Text = "© " & COPYRIGHTAUTHOR & ", " & VERSIONSTAGE & " " & VERSION & " (" & COPYRIGHTYEAR & ")"

        ' Initialise objects.
        cmbField.SelectedIndex = 0
        ' Update date boundaries.
        datRecordedA.MinDate = CDate("30/8/1963")
        datRecordedB.MinDate = CDate("30/8/1963")
        datRecordedA.MaxDate = Date.Today
        datRecordedB.MaxDate = Date.Today
        numYear.Maximum = Date.Today.Year

        _consoleWindow = New frmConsole()
        _consoleWindow.Show(Me)
        _consoleWindow.Hide()

        ' Load data (decks, brands and models).
        loadData()

        consoleAdd("Successfully loaded program.") ' Add success note to console.

        If UpdateCheckSchedule.ShouldCheck(
                preferences.UpdatePolicy,
                preferences.LastUpdateCheck,
                DateTime.Now) Then
            checkUpdates(False)
        End If

    End Sub

    Private Sub enableBestEffortTls()

        Try

            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol Or CType(768, SecurityProtocolType) Or CType(3072, SecurityProtocolType)

        Catch ex As Exception

            consoleAdd("Failed to enable best-effort TLS 1.1/1.2 support. Error: " & ex.Message)

        End Try

    End Sub

    Private Function isNewerVersion(latestVersion As String) As Boolean

        Try

            Dim latest As New Version(latestVersion)
            Dim current As New Version(VERSION)
            Return latest.CompareTo(current) > 0

        Catch ex As Exception

            Return latestVersion <> VERSION

        End Try

    End Function

    Sub checkUpdates(Optional manualCheck As Boolean = False)

        ' Check for updates to the program.

        ' Declare variables.
        Dim latestVersion As String
        Dim latestVersionStage As String
        Dim latestVersionDate As Date = DateTime.MinValue

        Dim updateAvailable As Boolean

        Dim message As String
        Dim messageDetails As String


        ' Get variables from URL.
        Try

            enableBestEffortTls()

            Dim updateClient As WebClient = New WebClient()
            Using updateReader As New StreamReader(updateClient.OpenRead(UpdateFeedUrl))

                ' Assume there are only 3 lines (and in data is in this order).
                latestVersion = updateReader.ReadLine()
                latestVersionStage = updateReader.ReadLine()
                DateTime.TryParse(updateReader.ReadLine(), latestVersionDate)

                ' Set success message for log.
                message = "Successfully checked for updates."

            End Using


        Catch ex As Exception


            ' Add confirmation message to console.
            message = "Failed to check for updates."
            consoleAdd(message & " Error: " & ex.Message)

            If manualCheck = True Then

                Dim boxTitle As String = "Update Check Failed"
                Dim boxMessage As String = "Compact Cassette Catalogue could not check for updates." & vbNewLine & vbNewLine & "This can happen on old Windows systems when GitHub HTTPS/TLS support is unavailable." & vbNewLine & vbNewLine & "Error: " & ex.Message & vbNewLine & vbNewLine & "Would you like to open the releases page in your browser?"
                Dim boxResult As DialogResult = MessageBox.Show(boxMessage, boxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)

                If boxResult = DialogResult.Yes Then
                    openWebLink(UPDATELINKDOWNLOAD)
                End If

            End If

            ' Exit this sub. Do not attempt to check versions further.
            Exit Sub


        End Try


        ' Check if a new version exists. (Ignore stage and date.)
        If isNewerVersion(latestVersion) Then

            updateAvailable = True
            messageDetails = "Found v" & latestVersion & "."

        Else

            updateAvailable = False
            messageDetails = "None found."

        End If


        ' Add confirmation message to console.
        consoleAdd(message & " " & messageDetails)


        ' If an update exists, show a message with a link.
        If updateAvailable = True Then

            ' Set up message box.
            Dim boxVersionCurrent As String = "Current version: " & VERSION '& " (" & VERSIONDATE.ToShortDateString & ")"
            Dim boxVersionLatest As String = "Latest version: " & latestVersion '& " (" & latestVersionDate.ToShortDateString & ")"

            Dim boxTitle As String = "Update Available"
            Dim boxReleaseDate As String = Nothing
            If latestVersionDate <> DateTime.MinValue Then
                boxReleaseDate = vbNewLine & "(Released " & latestVersionDate.ToString("dd MMMM yyyy") & ")"
            End If
            Dim boxMessage As String = "A Compact Cassette Catalogue update is available for download." & vbNewLine & vbNewLine & boxVersionCurrent & vbNewLine & boxVersionLatest & boxReleaseDate & vbNewLine & vbNewLine & "Would you like to be taken to the download page?"

            Dim boxResult As DialogResult
            boxResult = MessageBox.Show(boxMessage, boxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information)

            If boxResult = vbYes Then

                openWebLink(UPDATELINKDOWNLOAD) ' Open the downloads page.

            End If

        Else

            ' Don't show this message if the program is just starting up.
            If manualCheck = True Then

                MessageBox.Show("Compact Cassette Catalogue is up to date." & vbNewLine & VERSION & " is the latest version.", "No Updates Available", MessageBoxButtons.OK, MessageBoxIcon.Information)

            End If

        End If


        ' Set this date and time as the most recent update check.
        preferences.LastUpdateCheck = DateTime.Now
        preferences.Save()


    End Sub

    Public Sub loadData()

        'Call this when adding/deleting tapes, models, brands or decks.


        'Mask update routines
        _suppressTapeEditTracking = True

        'Load data (decks, brands and models)

        _tapeCount = tapeService.GetAll().Count
        _currentTapeIndex = _tapeCount - 1 'Select latest tape

        cmbDeckA.Items.Clear()
        cmbDeckB.Items.Clear()
        For Each value As Deck In deckService.GetAll()
            cmbDeckA.Items.Add(value.Name)
            cmbDeckB.Items.Add(value.Name)
        Next

        'Load latest tape if any exist
        If _tapeCount > 0 Then

            ' Enable scrolling and searching only if there is more than one record
            grpFind.Enabled = True
            grpScroll.Enabled = True

            ' Enable groups and elements
            btnDelete.Enabled = True
            DeleteTapeToolStripMenuItem.Enabled = True
            'btnUpdate.Enabled = True
            'UpdateTapeToolStripMenuItem.Enabled = True
            grpIdentification.Enabled = True
            grpData.Enabled = True

            displayTape()

        Else

            ' Disnable scrolling and searching
            grpFind.Enabled = False
            grpScroll.Enabled = False

            ' Disnable groups and elements
            btnDelete.Enabled = False
            DeleteTapeToolStripMenuItem.Enabled = False
            'btnUpdate.Enabled = False
            'UpdateTapeToolStripMenuItem.Enabled = False
            grpIdentification.Enabled = False
            grpData.Enabled = False

        End If

        'Unmask update routines
        _suppressTapeEditTracking = False

    End Sub

    Private Function CurrentTape() As Tape
        Dim values As IList(Of Tape) = tapeService.GetAll()
        If _currentTapeIndex < 0 OrElse _currentTapeIndex >= values.Count Then
            Return Nothing
        End If
        Return values(_currentTapeIndex)
    End Function

    Private Sub DiscardPendingTapeEdits()
        _hasPendingTapeEdits = False
        btnUpdate.Enabled = False
        UpdateTapeToolStripMenuItem.Enabled = False
        If _tapeCount > 0 Then
            displayTape()
        End If
    End Sub

    Private Sub updateTape()

        If _hasPendingTapeEdits Then

            Try

                Dim tape As Tape = CurrentTape()
                If tape Is Nothing Then
                    Throw New InvalidOperationException("The selected tape no longer exists.")
                End If
                Dim identifierShort As String = tape.ShortIdentifier
                Dim number As Integer = tape.Number
                Dim modelCode As String = tape.ModelIdentifier

                'Validate all new data entered

                Dim packaged As Boolean = chkPackaged.Checked
                Dim tapedA As Boolean = chkTapedA.Checked
                Dim tapedB As Boolean = chkTapedB.Checked

                If packaged = False Then

                    If tapedA = True And txtNameA.Text = Nothing Then
                        Throw New Exception("No name for side A.")
                    End If

                    If tapedB = True And txtNameB.Text = Nothing Then
                        Throw New Exception("No name for side B.")
                    End If

                End If


                'Get and update new long Identifier

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

                'Add leading zeroes to number-code (then remove extra zeroes)
                Dim numberCode As String = "00" & CStr(Number)
                numberCode = numberCode.Substring(numberCode.Length - 3, 3)

                Dim identifier As String = CStr(modelCode) & yearCode & lengthCode & numberCode 'Format: MMTmmYYLL###

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


                Dim sideA As New TapeSide(
                    tapedA,
                    nameA,
                    recordedA,
                    deckA,
                    inputA,
                    peakA,
                    NRA,
                    HXA,
                    MPXA,
                    dubbedA,
                    speedA,
                    biasCodeA,
                    biasCalA,
                    EQA,
                    levelA,
                    levelCalA,
                    contentsA,
                    artistA,
                    titleA)
                Dim sideB As New TapeSide(
                    tapedB,
                    nameB,
                    recordedB,
                    deckB,
                    inputB,
                    peakB,
                    NRB,
                    HXB,
                    MPXB,
                    dubbedB,
                    speedB,
                    biasCodeB,
                    biasCalB,
                    EQB,
                    levelB,
                    levelCalB,
                    contentsB,
                    artistB,
                    titleB)
                Dim draft As New TapeDraft(
                    modelCode,
                    year,
                    length,
                    cmbRegion.Text,
                    condition,
                    packaged,
                    sideA,
                    sideB,
                    txtNotes.Text)
                Dim updateResult As TapeOperationResult = tapeService.Update(identifierShort, draft)
                If Not updateResult.IsSuccess Then
                    Throw New InvalidOperationException(updateResult.Message)
                End If
                identifier = updateResult.Tapes(0).Identifier
                txtLong.Text = identifier

                _hasPendingTapeEdits = False
                catalogueSession.MarkChanged()

                'Update title bar
                Me.Text = catalogueSession.DisplayName & "* - C3"
                'Update buttons
                btnUpdate.Enabled = False
                UpdateTapeToolStripMenuItem.Enabled = False


                'Show confirmation message
                Dim message As String = "Updated tape " & identifierShort & " successfully."
                If preferences.ShowMessages Then
                    MsgBox(message, MsgBoxStyle.Question, "Successfully Updated Tape")
                End If
                consoleAdd(message)


            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Cannot Save Incomplete Tape")
                Exit Sub

            End Try

        Else

            'No changes to update tape with
            Dim message As String = "No changes to update tape with."
            If preferences.ShowMessages Then
                MsgBox(message, MsgBoxStyle.Question, "No Updates to Tape")
            End If
            'consoleAdd(message)

        End If

    End Sub

    Public Sub deleteTape(subTapeIndex As Integer, ignoreWarning As Boolean)

        Dim result As MsgBoxResult

        If ignoreWarning = False Then

            ' Present this warning to the user and use it to determine whether or not the tape should be deleted.
            result = MsgBox("Are you sure you want to delete the current tape?" & vbNewLine & "This action cannot be undone.", MsgBoxStyle.YesNoCancel, "Confirm Deletion")

        Else

            ' If warnings have been presented outside the function then checking again is not necessary, proceed directly to deletion.
            result = vbYes

        End If

        If result = vbYes Then

            Dim values As IList(Of Tape) = tapeService.GetAll()
            If subTapeIndex < 0 OrElse subTapeIndex >= values.Count Then
                MsgBox("The selected tape no longer exists.", MsgBoxStyle.Exclamation, "Tape Not Found")
                Return
            End If
            Dim identifierShort As String = values(subTapeIndex).ShortIdentifier


            Dim deletion As TapeOperationResult = tapeService.Delete(identifierShort)
            If Not deletion.IsSuccess Then
                MsgBox(deletion.Message, MsgBoxStyle.Exclamation, "Tape Not Deleted")
                Return
            End If
            _tapeCount = tapeService.GetAll().Count

            'Reset change detection variables
            _hasPendingTapeEdits = False
            catalogueSession.MarkChanged()
            'Reset buttons
            btnUpdate.Enabled = False
            UpdateTapeToolStripMenuItem.Enabled = False

            'Update title bar
            Me.Text = catalogueSession.DisplayName & "* - C3"

            'Reload data and display latest tape
            loadData()

            'Show confirmation message
            Dim message As String = "Deleted tape " & identifierShort & " successfully."
            consoleAdd(message)

        End If

    End Sub

    Private Sub saveChanges(saveAs As Boolean)

        If _hasPendingTapeEdits Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before saving changes?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()
                If _hasPendingTapeEdits Then
                    Return
                End If

                'SAVE CHANGES
                saveChangesActual(saveAs, False)

            ElseIf result = vbNo Then

                DiscardPendingTapeEdits()
                'SAVE CHANGES
                saveChangesActual(saveAs, False)

            End If

        Else

            'SAVE CHANGES
            saveChangesActual(saveAs, False)

        End If

    End Sub

    Public Sub saveChangesActual(saveAs As Boolean, thenOpen As Boolean)

        BufferedLogger.RecordAction(If(saveAs, "Save catalogue as", "Save catalogue"))

        'If there is no filepath, it is not saved
        Dim saved As Boolean = catalogueSession.FilePath IsNot Nothing
        Dim destinationPath As String = catalogueSession.FilePath

        Dim message As String = Nothing

        If saved = False Or saveAs = True Then
            'SAVE AS NEW FILE

            If Directory.Exists(preferences.DefaultDirectory) Then
                dlgSaveAs.InitialDirectory = preferences.DefaultDirectory
            End If
            Dim dlgResult As DialogResult = dlgSaveAs.ShowDialog()
            Dim selectedPath As String = dlgSaveAs.FileName

            If dlgResult = DialogResult.OK And Not String.IsNullOrWhiteSpace(selectedPath) Then
                'If user has given a valid file path.

                destinationPath = selectedPath

                'Make confirmation message
                message = "Saved catalogue successfully (as new file)."

            ElseIf dlgResult = DialogResult.Cancel Then
                'If user DID deliberately cancel save procedure.

                Exit Sub 'Exit and don't try to save.

            Else
                'If user did NOT deliberately cancel save procedure.

                'Show error message
                MsgBox("Bad file path selected. Catalogue not saved.", MsgBoxStyle.Critical, "File Path Error")
                Exit Sub

            End If

        Else

            'SAVE OVERWRITE FILE

            'Make confirmation message
            message = "Saved catalogue successfully (overwrote file)."

        End If

        catalogueMetadata.MarkModified(DateTime.Now)

        Dim expectedRevision As CatalogueRevision = Nothing
        If Not saveAs AndAlso String.Equals(
                destinationPath,
                catalogueSession.FilePath,
                StringComparison.OrdinalIgnoreCase) Then
            expectedRevision = catalogueSession.Revision
        End If

        Dim saveResult As LegacyCatalogueSaveResult = catalogueStore.Save(
            destinationPath,
            catalogue,
            expectedRevision,
            VERSIONFILESUPPORTED)
        If Not saveResult.IsSuccess Then
            BufferedLogger.Error("Catalogue save failed: " & saveResult.Message)
            MsgBox(
                "Catalogue was not saved." & vbNewLine & vbNewLine & saveResult.Message,
                MsgBoxStyle.Critical,
                "Catalogue Save Error")
            Exit Sub
        End If

        Dim savedFileName As String = Path.GetFileName(destinationPath)
        catalogueSession.MarkSaved(destinationPath, savedFileName, saveResult.Revision)
        'Discard updates made to current tape and reload from saved data.

        'Reset changes variable
        _hasPendingTapeEdits = False

        'Reset buttons
        btnUpdate.Enabled = False
        UpdateTapeToolStripMenuItem.Enabled = False

        'Update title bar
        Me.Text = catalogueSession.DisplayName & " - C3"

        'Reload from saved data
        loadData()


        'Show confirmation message
        consoleAdd(message)

        If thenOpen = True Then
            openCatalogueActual()
        End If

    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click

        saveChanges(True)

    End Sub

    Private Sub OpenCatalogueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenCatalogueToolStripMenuItem.Click
        'Load/open catalogue from XML file

        If _hasPendingTapeEdits Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before opening catalogue?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()
                If _hasPendingTapeEdits Then
                    Return
                End If

                'CHECK CHANGES
                openCatalogueCheckChanges()

            ElseIf result = vbNo Then

                DiscardPendingTapeEdits()
                'CHECK CHANGES
                openCatalogueCheckChanges()

            End If

        Else

            'CHECK CHANGES
            openCatalogueCheckChanges()

        End If

    End Sub

    Sub openCatalogueCheckChanges()
        'Check for unsaved changes to the whole catalogue

        If catalogueSession.IsDirty Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the catalogue." & vbNewLine & "Save changes before opening new catalogue?", MsgBoxStyle.YesNoCancel, "Changes Made To Catalogue")

            If result = vbYes Then

                saveChangesActual(False, True)

            ElseIf result = vbNo Then

                'OPEN CAT
                openCatalogueActual()

            End If

        Else

            'OPEN CAT
            openCatalogueActual()

        End If

    End Sub

    Public Sub openCatalogueActual()

        BufferedLogger.RecordAction("Open catalogue")

        'Get directories
        If Directory.Exists(preferences.DefaultDirectory) Then
            dlgOpen.InitialDirectory = preferences.DefaultDirectory
        End If
        Dim dlgResult As DialogResult = dlgOpen.ShowDialog()
        Dim selectedPath As String = dlgOpen.FileName

        If dlgResult = DialogResult.OK And selectedPath IsNot Nothing Then
            'If user has given a valid file path.

            Dim loadResult As LegacyCatalogueLoadResult = catalogueStore.Load(
                selectedPath,
                catalogue,
                VERSIONFILESUPPORTED)
            If Not loadResult.IsSuccess Then
                BufferedLogger.Error("Catalogue load failed: " & loadResult.Message)
                MsgBox(
                    "Catalogue was not opened. The current catalogue is unchanged." &
                        vbNewLine & vbNewLine & loadResult.Message,
                    MsgBoxStyle.Critical,
                    "Catalogue Load Error")
                Exit Sub
            End If

            Dim fileVersion As String = loadResult.FileVersion

            'Only load if the file version is supported.
            If VERSIONFILESUPPORTED.Contains(fileVersion) Then

                Dim selectedFileName As String = Path.GetFileName(selectedPath)
                replaceCatalogue(loadResult.Document)
                catalogueSession.MarkLoaded(selectedPath, selectedFileName, loadResult.Revision)


                'Reset changes variable
                _hasPendingTapeEdits = False

                'Reset buttons
                btnUpdate.Enabled = False
                UpdateTapeToolStripMenuItem.Enabled = False

                'Update title bar
                Me.Text = catalogueSession.DisplayName & " - C3"


                catalogueMetadata.RefreshProductMetadata(VERSION, VERSIONSTAGE, VERSIONDATE)

                'Show confirmation message
                Dim message As String = "Opened catalogue successfully."
                consoleAdd(message)

                'Load data into forms
                loadData()

            Else
                'If file is not the right version.

                'Make string of list of supported versions
                Dim versionsSupported As String = VERSIONFILESUPPORTED(0)
                For i As Integer = 1 To VERSIONFILESUPPORTED.Length - 1
                    versionsSupported = versionsSupported & ", " & VERSIONFILESUPPORTED(i)
                Next

                'Show error message
                MsgBox("Format version of this file is not supported." & vbNewLine & "Selected file version: " & fileVersion & vbNewLine & "Supported file version(s): " & versionsSupported, MsgBoxStyle.Critical, "Unsupported File Version")

            End If

        ElseIf dlgResult <> DialogResult.Cancel Then
            'If user did NOT deliberately cancel save procedure.

            'Show error message
            MsgBox("Bad file path selected. Catalogue not opened.", MsgBoxStyle.Critical, "File Path Error")

        End If

    End Sub

    Private Sub updateMade()
        ' Made an update to a field in the main form.

        If Not _hasPendingTapeEdits AndAlso Not _suppressTapeEditTracking Then

            _hasPendingTapeEdits = True

            ' Update title bar.
            Me.Text = catalogueSession.DisplayName & "* - C3"

            ' Enable buttons.
            btnUpdate.Enabled = True
            UpdateTapeToolStripMenuItem.Enabled = True

        End If

    End Sub

    Private Sub updateScrollers()

        'Ensure users can't scroll out-of-bounds

        If _currentTapeIndex = 0 Then
            btnPrevious.Enabled = False
        Else
            btnPrevious.Enabled = True
        End If

        If _currentTapeIndex = _tapeCount - 1 Then
            btnNext.Enabled = False
        Else
            btnNext.Enabled = True
        End If

    End Sub

    Private Sub displayTape()

        'Mask update routines
        _suppressTapeEditTracking = True

        'Ensure users can't scroll out-of-bounds
        updateScrollers()


        Dim tape As Tape = CurrentTape()
        If tape Is Nothing Then
            _suppressTapeEditTracking = False
            Return
        End If

        'Display identifiers
        txtLong.Text = tape.Identifier
        txtShort.Text = tape.ShortIdentifier
        txtIndex.Text = CStr(_currentTapeIndex + 1)
        txtTotal.Text = CStr(_tapeCount)

        'Find model name from identification/code
        Dim model As CassetteModel = cassetteModelService.Find(tape.ModelIdentifier)
        Dim modelName As String = tape.ModelIdentifier
        If model IsNot Nothing Then
            Dim brand As Brand = brandService.Find(model.BrandCode)
            modelName = If(brand Is Nothing, model.BrandCode, brand.Name) & " " & model.ModelName
            _currentModelType = model.TypeNumber
        Else
            _currentModelType = 1
        End If

        'Populate groups and elements
        txtModel.Text = modelName
        numYear.Value = tape.Year
        numLength.Value = tape.LengthMinutes
        cmbRegion.Text = tape.Region
        'txtNumber.Text = CStr(tape("Number"))

        Dim condition As Integer = getCondition(tape.Condition)
        cmbCondition.SelectedIndex = condition
        chkPackaged.Checked = tape.Packaged

        'Enable "taped sides" groups and load data

        _wasSideARecorded = tape.SideA.IsRecorded
        If _wasSideARecorded Then
            'If side A recorded, load data

            chkTapedA.Checked = True

            txtNameA.Text = tape.SideA.Name
            datRecordedA.Value = tape.SideA.RecordedAt

            cmbDeckA.Text = tape.SideA.DeckName
            cmbInputA.Text = tape.SideA.InputName

            numPeakA.Value = tape.SideA.PeakLevel
            numLevelA.Value = tape.SideA.Level
            numLevelCalA.Value = tape.SideA.LevelCalibration

            cmbEQA.Text = tape.SideA.Equalization
            cmbBiasA.SelectedIndex = tape.SideA.Bias - 1
            numBiasCalA.Value = tape.SideA.BiasCalibration

            cmbNRA.Text = tape.SideA.NoiseReduction
            chkHXA.Checked = tape.SideA.Hx
            chkMPXA.Checked = tape.SideA.Mpx

            cmbSpeedA.Text = tape.SideA.Speed
            chkDubbedA.Checked = tape.SideA.Dubbed

            'Contents for recording
            cmbContentsA.Text = tape.SideA.Contents
            txtArtistA.Text = tape.SideA.Artist
            txtTitleA.Text = tape.SideA.Title

        Else
            'Else, return all objects to their default values

            chkTapedA.Checked = False

            txtNameA.Text = Nothing
            datRecordedA.Value = Date.Today

            cmbDeckA.Text = Nothing
            cmbInputA.Text = Nothing

            numPeakA.Value = 0
            numLevelA.Value = 5
            numLevelCalA.Value = 0

            cmbEQA.Text = Nothing
            cmbBiasA.SelectedIndex = 0 'required?
            cmbBiasA.Text = Nothing
            numBiasCalA.Value = 0

            cmbNRA.Text = Nothing
            chkHXA.Checked = False
            chkMPXA.Checked = False

            cmbSpeedA.Text = Nothing
            chkDubbedA.Checked = False

            'Contents for recording
            cmbContentsA.Text = Nothing
            txtArtistA.Text = Nothing
            txtTitleA.Text = Nothing

        End If

        _wasSideBRecorded = tape.SideB.IsRecorded
        If _wasSideBRecorded Then

            chkTapedB.Checked = True

            txtNameB.Text = tape.SideB.Name
            datRecordedB.Value = tape.SideB.RecordedAt

            cmbDeckB.Text = tape.SideB.DeckName
            cmbInputB.Text = tape.SideB.InputName

            numPeakB.Value = tape.SideB.PeakLevel
            numLevelB.Value = tape.SideB.Level
            numLevelCalB.Value = tape.SideB.LevelCalibration

            cmbEQB.Text = tape.SideB.Equalization
            cmbBiasB.SelectedIndex = tape.SideB.Bias - 1
            numBiasCalB.Value = tape.SideB.BiasCalibration

            cmbNRB.Text = tape.SideB.NoiseReduction
            chkHXB.Checked = tape.SideB.Hx
            chkMPXB.Checked = tape.SideB.Mpx

            cmbSpeedB.Text = tape.SideB.Speed
            chkDubbedB.Checked = tape.SideB.Dubbed

            'Contents for recording
            cmbContentsB.Text = tape.SideB.Contents
            txtArtistB.Text = tape.SideB.Artist
            txtTitleB.Text = tape.SideB.Title

        Else
            'Else, return all objects to their default values

            chkTapedB.Checked = False

            txtNameB.Text = Nothing
            datRecordedB.Value = Date.Today

            cmbDeckB.Text = Nothing
            cmbInputB.Text = Nothing

            numPeakB.Value = 0
            numLevelB.Value = 5
            numLevelCalB.Value = 0

            cmbEQB.Text = Nothing
            cmbBiasB.SelectedIndex = 0 'required?
            cmbBiasB.Text = Nothing
            numBiasCalB.Value = 0

            cmbNRB.Text = Nothing
            chkHXB.Checked = False
            chkMPXB.Checked = False

            cmbSpeedB.Text = Nothing
            chkDubbedB.Checked = False

            'Contents for recording
            cmbContentsB.Text = Nothing
            txtArtistB.Text = Nothing
            txtTitleB.Text = Nothing

        End If

        'Load notes
        txtNotes.Text = tape.Notes

        'Unmask update routines
        _suppressTapeEditTracking = False

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click

        'MsgBox("Compact Cassette Catalogue (C3)" & vbNewLine & "© " & COPYRIGHTAUTHOR & ", " & COPYRIGHTYEAR & vbNewLine & vbNewLine & "Program Version: " & VERSIONSTAGE & " " & VERSION & vbNewLine & "Catalogue Version: " & VERSIONFILE & vbNewLine & VERSIONDATE.ToLongDateString & ", " & VERSIONDATE.ToLongTimeString, MsgBoxStyle.Question, "About C3")

        Using window As New frmAbout()
            window.ShowDialog(Me)
        End Using

    End Sub

    Private Sub SearchTapesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchTapesToolStripMenuItem.Click
        Dim window As New frmTapes()
        window.Show(Me)
    End Sub

    Private Sub SearchModelsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchModelsToolStripMenuItem.Click
        Dim window As New frmModels()
        window.Show(Me)
    End Sub

    Private Sub SearchManufacturersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchManufacturersToolStripMenuItem.Click
        Dim window As New frmBrands()
        window.Show(Me)
    End Sub

    Private Sub ViewDecksToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewDecksToolStripMenuItem.Click
        Dim window As New frmDecks()
        window.Show(Me)
    End Sub

    Private Sub ViewStatisticsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewStatisticsToolStripMenuItem.Click
        Dim window As New frmStatistics()
        window.Show(Me)
    End Sub


    Private Sub ChkTapedA_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedA.CheckedChanged

        updateMade()

        If chkTapedA.Checked = True Then

            Dim deckCount As Integer = deckService.GetAll().Count

            'Check that at least 1 deck exists
            If deckCount >= 1 Then

                If Not _wasSideARecorded Then

                    'Set defaults
                    datRecordedA.Value = Date.Today
                    cmbDeckA.SelectedIndex = 0
                    'cmbDeckA.SelectedIndex = cmbDeckA.Items.Count - 1 'Latest deck

                    cmbInputA.SelectedIndex = 10 'Phone input
                    cmbNRA.SelectedIndex = 1 'Dolby B
                    cmbSpeedA.SelectedIndex = 1 'Normal speed
                    cmbContentsA.SelectedIndex = 0
                    numLevelA.Value = CDec(5)

                    If _currentModelType = 1 Then 'If normal bias
                        cmbEQA.SelectedIndex = 0 '120us
                    Else
                        cmbEQA.SelectedIndex = 1 '70us
                    End If
                    cmbBiasA.SelectedIndex = _currentModelType - 1

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

                End If

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

    Private Sub ChkTapedB_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedB.CheckedChanged

        updateMade()

        If chkTapedB.Checked = True Then

            Dim deckCount As Integer = deckService.GetAll().Count

            'Check that at least 1 deck exists
            If deckCount >= 1 Then

                If Not _wasSideBRecorded Then

                    'Set defaults
                    datRecordedB.Value = Date.Today
                    cmbDeckB.SelectedIndex = 0
                    'cmbDeckB.SelectedIndex = cmbDeckB.Items.Count - 1 'Latest deck

                    cmbInputB.SelectedIndex = 10 'Phone input
                    cmbNRB.SelectedIndex = 1 'Dolby B
                    cmbSpeedB.SelectedIndex = 1 'Normal speed
                    cmbContentsB.SelectedIndex = 0
                    numLevelB.Value = CDec(5)

                    If _currentModelType = 1 Then 'If normal bias
                        cmbEQB.SelectedIndex = 0 '120us
                    Else
                        cmbEQB.SelectedIndex = 1 '70us
                    End If
                    cmbBiasB.SelectedIndex = _currentModelType - 1

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

                End If

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

    Private Sub BtnFind_Click(sender As Object, e As EventArgs) Handles btnFind.Click
        Dim window As New frmFindResults()
        window.Show(Me)
    End Sub

    Private Sub NewDeckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewDeckToolStripMenuItem.Click
        Using editor As New frmDeckNew()
            editor.ShowDialog(Me)
        End Using
    End Sub

    Private Sub NewManufactererToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewManufactererToolStripMenuItem.Click
        Using editor As New frmBrandNew()
            editor.ShowDialog(Me)
        End Using
    End Sub

    Private Sub NewModelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewModelToolStripMenuItem.Click

        If brandService.GetAll(Nothing).Count > 0 Then
            Using editor As New frmModelNew()
                editor.ShowDialog(Me)
            End Using
        Else
            MsgBox("Add at least one brand first.", MsgBoxStyle.Exclamation, "No Brands")
        End If

    End Sub

    Private Sub ChkPackaged_CheckedChanged(sender As Object, e As EventArgs) Handles chkPackaged.CheckedChanged

        updateMade()

        If chkPackaged.Checked = True Then
            chkTapedA.Checked = False
            chkTapedB.Checked = False
            grpTaped.Enabled = False

        Else
            grpTaped.Enabled = True

        End If

    End Sub

    Private Sub addNewTape()

        If _hasPendingTapeEdits Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before adding new tape?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()
                If _hasPendingTapeEdits Then
                    Return
                End If

                'ADD A NEW TAPE
                addNewTapeActual()

            ElseIf result = vbNo Then

                DiscardPendingTapeEdits()
                'ADD A NEW TAPE
                addNewTapeActual()

            End If

        Else

            'ADD A NEW TAPE
            addNewTapeActual()

        End If

    End Sub

    Private Sub addNewTapeActual()

        Dim modelCount As Integer = cassetteModelService.GetAll().Count

        'Check that there is at least 1 model (and 1 deck for recording)

        If modelCount >= 1 Then
            Using editor As New frmTapeNew()
                editor.ShowDialog(Me)
            End Using

        Else
            MsgBox("Add at least one model first.", MsgBoxStyle.Exclamation, "No Models")

        End If

    End Sub

    Public Sub closeApplication()
        If CanCloseApplication() Then
            _allowClose = True
            Close()
        End If
    End Sub

    Public Sub RefreshAfterCatalogueMutation()
        Me.Text = catalogueSession.DisplayName & "* - C3"
        loadData()
    End Sub

    Private Function CanCloseApplication() As Boolean
        If _hasPendingTapeEdits Then
            Dim pendingResult As MsgBoxResult = MsgBox(
                "Changes have been made to the current tape." & vbNewLine &
                    "Update current tape before closing?",
                MsgBoxStyle.YesNoCancel Or MsgBoxStyle.Question,
                "Changes Made To Tape")
            If pendingResult = vbCancel Then
                Return False
            End If
            If pendingResult = vbYes Then
                updateTape()
                If _hasPendingTapeEdits Then
                    Return False
                End If
            ElseIf pendingResult = vbNo Then
                DiscardPendingTapeEdits()
            End If
        End If

        If Not catalogueSession.IsDirty Then
            Return True
        End If

        Dim result As MsgBoxResult = MsgBox(
            "Changes have been made to the catalogue." & vbNewLine & "Save changes before closing?",
            MsgBoxStyle.YesNoCancel Or MsgBoxStyle.Question,
            "Changes Made To Catalogue")
        If result = vbCancel Then
            Return False
        End If
        If result = vbNo Then
            Return True
        End If

        saveChanges(False)
        Return Not catalogueSession.IsDirty
    End Function

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        addNewTape()

    End Sub

    Private Sub NewTapeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewTapeToolStripMenuItem.Click

        addNewTape()

    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        deleteTape(_currentTapeIndex, False)

    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        saveChanges(False)

    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click

        saveChanges(False)

    End Sub

    Private Sub scrollActual(change As Integer, jump As Boolean)

        If jump = True Then

            _currentTapeIndex = change ' New index is the given integer.
            displayTape()

        Else

            _currentTapeIndex += change ' New index is incremented/decremented by the given integer.
            displayTape()

        End If

    End Sub

    Public Sub ScrollToTape(shortIdentifier As String)
        Dim values As IList(Of Tape) = tapeService.GetAll()
        For index As Integer = 0 To values.Count - 1
            If String.Equals(
                    values(index).ShortIdentifier,
                    shortIdentifier,
                    StringComparison.OrdinalIgnoreCase) Then
                scrollTo(index, True)
                Return
            End If
        Next
        MsgBox("The selected tape no longer exists.", MsgBoxStyle.Exclamation, "Tape Not Found")
    End Sub

    Public Sub scrollTo(change As Integer, jump As Boolean)

        If _hasPendingTapeEdits Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before scrolling?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()
                If _hasPendingTapeEdits Then
                    Return
                End If

                scrollActual(change, jump) ' Scroll!

            ElseIf result = vbNo Then

                DiscardPendingTapeEdits()
                scrollActual(change, jump) ' Scroll!

            End If

        Else

            scrollActual(change, jump) ' Scroll!

        End If

        ' Reset updates variable and buttons.
        _hasPendingTapeEdits = False

        btnUpdate.Enabled = False
        UpdateTapeToolStripMenuItem.Enabled = False

    End Sub

    Private Sub BtnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click

        scrollTo(-1, False)

    End Sub

    Private Sub BtnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click

        scrollTo(1, False)

    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click

        closeApplication()

    End Sub

    Private Sub frmMain_Close(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If _allowClose Then
            Return
        End If

        e.Cancel = Not CanCloseApplication()
        If Not e.Cancel Then
            _allowClose = True
        End If
    End Sub

    Private Sub ShowConsoleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowConsoleToolStripMenuItem.Click
        If _consoleWindow Is Nothing OrElse _consoleWindow.IsDisposed Then
            _consoleWindow = New frmConsole()
        End If
        _consoleWindow.Show(Me)
        _consoleWindow.BringToFront()

    End Sub

    Private Sub frmMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        If _consoleWindow IsNot Nothing AndAlso Not _consoleWindow.IsDisposed Then
            _consoleWindow.ClosePermanently()
        End If
    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        'Make a new catalogue (check before saving first)

        'Check if tape updated
        If _hasPendingTapeEdits Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before creating new catalogue?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()
                If _hasPendingTapeEdits Then
                    Return
                End If

                'NEW CAT
                newCatalogueCheckChanges()

            ElseIf result = vbNo Then

                DiscardPendingTapeEdits()
                'NEW CAT
                newCatalogueCheckChanges()

            End If

        Else

            'NEW CAT
            newCatalogueCheckChanges()

        End If

    End Sub

    Private Sub newCatalogueCheckChanges()
        'Check for unsaved changes to the whole catalogue

        If catalogueSession.IsDirty Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the catalogue." & vbNewLine & "Save changes before creating new catalogue?", MsgBoxStyle.YesNoCancel, "Changes Made To Catalogue")

            If result = vbYes Then

                saveChangesActual(False, False)

                'NEW CAT
                Application.Restart()

            ElseIf result = vbNo Then

                'NEW CAT
                Application.Restart()

            End If

        Else

            'NEW CAT
            Application.Restart()

        End If

    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click

        updateTape()

    End Sub

    Private Sub FeedbackToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FeedbackToolStripMenuItem.Click

        openWebLink(FEEDBACKLINK)

    End Sub

    Private Sub numYear_ValueChanged(sender As Object, e As EventArgs) Handles numYear.ValueChanged

        updateMade()

    End Sub

    Private Sub numLength_ValueChanged(sender As Object, e As EventArgs) Handles numLength.ValueChanged

        updateMade()

    End Sub

    Private Sub cmbRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbRegion.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub cmbCondition_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCondition.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub txtNotes_TextChanged(sender As Object, e As EventArgs) Handles txtNotes.TextChanged

        updateMade()

    End Sub

    Private Sub txtNameA_TextChanged(sender As Object, e As EventArgs) Handles txtNameA.TextChanged

        updateMade()

    End Sub

    Private Sub datRecordedA_ValueChanged(sender As Object, e As EventArgs) Handles datRecordedA.ValueChanged

        updateMade()

    End Sub

    Private Sub cmbDeckA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbDeckA.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub cmbInputA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbInputA.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub numPeakA_ValueChanged(sender As Object, e As EventArgs) Handles numPeakA.ValueChanged

        updateMade()

    End Sub

    Private Sub cmbNRA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbNRA.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub cmbEQA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbEQA.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub cmbBiasA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBiasA.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub numBiasCalA_ValueChanged(sender As Object, e As EventArgs) Handles numBiasCalA.ValueChanged

        updateMade()

    End Sub

    Private Sub numLevelA_ValueChanged(sender As Object, e As EventArgs) Handles numLevelA.ValueChanged

        updateMade()

    End Sub

    Private Sub numLevelCalA_ValueChanged(sender As Object, e As EventArgs) Handles numLevelCalA.ValueChanged

        updateMade()

    End Sub

    Private Sub chkHXA_CheckedChanged(sender As Object, e As EventArgs) Handles chkHXA.CheckedChanged

        updateMade()

    End Sub

    Private Sub cmbSpeedA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSpeedA.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub chkDubbedA_CheckedChanged(sender As Object, e As EventArgs) Handles chkDubbedA.CheckedChanged

        updateMade()

    End Sub

    Private Sub chkMPXA_CheckedChanged(sender As Object, e As EventArgs) Handles chkMPXA.CheckedChanged

        updateMade()

    End Sub

    Private Sub cmbContentsA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbContentsA.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub txtArtistA_TextChanged(sender As Object, e As EventArgs) Handles txtArtistA.TextChanged

        updateMade()

    End Sub

    Private Sub txtTitleA_TextChanged(sender As Object, e As EventArgs) Handles txtTitleA.TextChanged

        updateMade()

    End Sub

    Private Sub UpdateTapeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateTapeToolStripMenuItem.Click

        updateTape()

    End Sub

    Private Sub DeleteTapeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteTapeToolStripMenuItem.Click

        deleteTape(_currentTapeIndex, False)

    End Sub

    Private Function ResolveOutputDirectory() As String
        If Not String.IsNullOrWhiteSpace(catalogueSession.FilePath) Then
            Dim catalogueDirectory As String = Path.GetDirectoryName(catalogueSession.FilePath)
            If Directory.Exists(catalogueDirectory) Then
                Return catalogueDirectory
            End If
        End If

        If Directory.Exists(preferences.DefaultDirectory) Then
            Return preferences.DefaultDirectory
        End If

        Return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    End Function

    Private Sub OutputConsoleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OutputConsoleToolStripMenuItem.Click

        'Output the console to a log file and save it.
        'Write the string to a new file (date and time included in file name).

        Dim outputTime As DateTime = DateTime.Now
        Dim outputName As String = "console-output_" & outputTime.ToString("yyMMdd-HHmmss") & ".txt"
        Dim outputPath As String = Path.Combine(ResolveOutputDirectory(), outputName)
        Try
            Using outputFile As New StreamWriter(outputPath)

                'Write header.
                outputFile.WriteLine("Compact Cassette Catalogue (v" & CStr(VERSION) & ") Console Output at " & outputTime.ToString)
                outputFile.WriteLine("--------------------------------")

                'Write the bounded diagnostic log, independently of console visibility.
                For Each line As String In BufferedLogger.Tail()
                    outputFile.WriteLine(line)
                Next

            End Using
        Catch ex As Exception
            consoleAdd("Failed to output console log. Error: " & ex.Message)
            MsgBox(
                "C3 could not write the console log." & vbNewLine & vbNewLine & ex.Message,
                MsgBoxStyle.Exclamation,
                "Console Log Not Saved")
            Return
        End Try

        'Show confirmation message
        Dim message As String = "Successfully output console to log file."
        Dim messageDetails As String = vbNewLine & vbNewLine & "File name: " & outputName & vbNewLine & "Full directory: " & outputPath
        If preferences.ShowMessages Then
            MsgBox(message & messageDetails, MsgBoxStyle.Question, "Successfully Output Console Log")
        End If
        consoleAdd(message)

    End Sub

    Private Sub txtNameB_TextChanged(sender As Object, e As EventArgs) Handles txtNameB.TextChanged

        updateMade()

    End Sub

    Private Sub datRecordedB_ValueChanged(sender As Object, e As EventArgs) Handles datRecordedB.ValueChanged

        updateMade()

    End Sub

    Private Sub cmbDeckB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbDeckB.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub cmbInputB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbInputB.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub numPeakB_ValueChanged(sender As Object, e As EventArgs) Handles numPeakB.ValueChanged

        updateMade()

    End Sub

    Private Sub cmbNRB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbNRB.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub cmbEQB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbEQB.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub cmbBiasB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBiasB.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub numBiasCalB_ValueChanged(sender As Object, e As EventArgs) Handles numBiasCalB.ValueChanged

        updateMade()

    End Sub

    Private Sub numLevelB_ValueChanged(sender As Object, e As EventArgs) Handles numLevelB.ValueChanged

        updateMade()

    End Sub

    Private Sub numLevelCalB_ValueChanged(sender As Object, e As EventArgs) Handles numLevelCalB.ValueChanged

        updateMade()

    End Sub

    Private Sub cmbSpeedB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSpeedB.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub chkDubbedB_CheckedChanged(sender As Object, e As EventArgs) Handles chkDubbedB.CheckedChanged

        updateMade()

    End Sub

    Private Sub chkHXB_CheckedChanged(sender As Object, e As EventArgs) Handles chkHXB.CheckedChanged

        updateMade()

    End Sub

    Private Sub chkMPXB_CheckedChanged(sender As Object, e As EventArgs) Handles chkMPXB.CheckedChanged

        updateMade()

    End Sub

    Private Sub cmbContentsB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbContentsB.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub txtArtistB_TextChanged(sender As Object, e As EventArgs) Handles txtArtistB.TextChanged

        updateMade()

    End Sub

    Private Sub txtTitleB_TextChanged(sender As Object, e As EventArgs) Handles txtTitleB.TextChanged

        updateMade()

    End Sub

    Private Sub PreferencesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PreferencesToolStripMenuItem.Click

        Using window As New frmSettings()
            window.ShowDialog(Me)
        End Using

    End Sub

    Private Sub CheckForUpdatesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckForUpdatesToolStripMenuItem.Click

        checkUpdates(True)

    End Sub

    Private Sub OpenDownloadsPageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenDownloadsPageToolStripMenuItem.Click

        openWebLink(UPDATELINKDOWNLOAD) ' Open the downloads page.

    End Sub

    Private Sub HelpGuideToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpGuideToolStripMenuItem.Click

        ' Open help/wiki website.
        openWebLink(WEBSITEHELP)

    End Sub

    'Private Sub numIndex_ValueChanged(sender As Object, e As EventArgs)

    '    ' When the index is changed, jump to it!
    '    Dim newIndex As Integer = CInt(numIndex.Value)
    '    scrollIndex(newIndex, True)

    'End Sub

End Class
