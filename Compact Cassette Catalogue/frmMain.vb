' Module: Main & Edit Tape
' Purpose: To edit existing tapes and access the other functions of the program.
' Author: Jules Carboni
' Date Created: 22 Aug 2019

Imports System.Xml
Imports System.IO
Imports System.Text.RegularExpressions

Public Class frmMain

    'Declare variables
    Dim updatesMask As Boolean = True
    'Initialise tape index to last tape
    Dim thisTapeIndex As Integer = tapeCount - 1
    Dim thisModelType As Integer
    Dim thisTapedA As Boolean
    Dim thisTapedB As Boolean

    'Dim newTape As Object() = {"", 0, 0, "", 0, "Unsaved", 0, False, False, False, "", CDate("1/1/1970"), "", "", 0, "", False, False, False, 0, 0, "", 0, 0, "", "", "", "", CDate("1/1/1970"), "", "", 0, "", False, False, False, 0, 0, "", 0, 0, "", ""} 'Default record for a new blank tape

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ''Update varaibles
        'deckCount = CInt(counters.Rows(0)("Number"))
        'brandCount = CInt(counters.Rows(1)("Number"))
        'modelCount = CInt(counters.Rows(2)("Number"))
        'tapeCount = CInt(counters.Rows(3)("Number"))
        ''Re-assert variables that for some reasion change
        'changes = False
        'updates = False

        'Display about information
        lblAbout.Text = "© Jules Carboni, " & VERSIONSTAGE & " " & VERSION & " (" & VERSIONDATE.ToString("d/M/yy") & ")"

        'Add tables to data set (a global process)
        catalogue.Tables.Add(information)
        catalogue.Tables.Add(counters)
        catalogue.Tables.Add(decks)
        catalogue.Tables.Add(brands)
        catalogue.Tables.Add(models)
        catalogue.Tables.Add(tapes)

        'Initialise objects
        cmbField.SelectedIndex = 0 'move to search-load subroutine when it gets implemented
        'Update date boundaries
        datRecordedA.MinDate = CDate("30/8/1963")
        datRecordedB.MinDate = CDate("30/8/1963")
        datRecordedA.MaxDate = Date.Today
        datRecordedB.MaxDate = Date.Today
        numYear.Maximum = Date.Today.Year

        'Load console
        frmConsole.Show()
        frmConsole.Hide()

        'Load data (decks, brands and models)
        loadData()

        consoleAdd("Successfully loaded program.") 'Add success note to console

    End Sub

    Public Sub loadData()

        'Call this when adding/deleting tapes, models, brands or decks.


        'Mask update routines
        updatesMask = True

        'Load data (decks, brands and models)

        deckCount = CInt(counters.Rows(0)("Number"))
        tapeCount = CInt(counters.Rows(3)("Number"))
        thisTapeIndex = tapeCount - 1 'Select latest tape

        'Load decks into combination boxes
        If deckCount > 0 Then
            'If no decks, catch don't crash

            cmbDeckA.Items.Clear()

            For i As Integer = 0 To deckCount - 1
                Dim row As DataRow = decks.Rows(i)

                Dim thisDeck As String = CStr(row("Name"))
                cmbDeckA.Items.Add(thisDeck)
                cmbDeckB.Items.Add(thisDeck)
            Next

        End If

        'Load latest tape if any exist
        If tapeCount > 0 Then

            'Enable scrolling and searching only if there is more than one record
            grpFind.Enabled = True
            grpScroll.Enabled = True

            'Enable groups and elements
            btnDelete.Enabled = True
            DeleteTapeToolStripMenuItem.Enabled = True
            btnUpdate.Enabled = True
            UpdateTapeToolStripMenuItem.Enabled = True
            grpIdentification.Enabled = True
            grpData.Enabled = True

            displayTape()

        Else

            'Disnable scrolling and searching
            grpFind.Enabled = False
            grpScroll.Enabled = False

            'Disnable groups and elements
            btnDelete.Enabled = False
            DeleteTapeToolStripMenuItem.Enabled = False
            btnUpdate.Enabled = False
            UpdateTapeToolStripMenuItem.Enabled = False
            grpIdentification.Enabled = False
            grpData.Enabled = False

        End If

        'Unmask update routines
        updatesMask = False

    End Sub

    Private Sub updateTape()

        If updates = True Then

            Try

                'Open existing data row
                Dim tape As DataRow = tapes.Rows(thisTapeIndex)
                Dim identifierShort As String = CStr(tape("IdentifierShort"))
                Dim number As Integer = CInt(tape("Number"))
                Dim modelCode As String = CStr(tape("Model"))

                ''model Name from identification/code
                'Dim modelRow As DataRow = models.Rows.Find(modelCode)
                'Dim modelName As String = CStr(modelRow("Brand")) & " " & CStr(modelRow("Model"))
                ''Find model type
                'thisModelType = CInt(modelCode.Substring(2, 1))


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


                'Write data to record

                Dim thisTape As Object() = {modelCode, year, length, cmbRegion.Text, Number, identifier, identifierShort, condition, packaged, tapedA, tapedB, nameA, recordedA, deckA, inputA, peakA, NRA, HXA, MPXA, dubbedA, speedA, biasCodeA, biasCalA, EQA, levelA, levelCalA, contentsA, artistA, titleA, nameB, recordedB, deckB, inputB, peakB, NRB, HXB, MPXB, dubbedB, speedB, biasCodeB, biasCalB, EQB, levelB, levelCalB, contentsB, artistB, titleB, DateTime.Now, txtNotes.Text} 'The data to be written for this tape entry

                'Write new data to existing row
                Dim counter As Integer = 0

                For Each thisObject As Object In thisTape

                    tapes.Rows(thisTapeIndex)(counter) = thisObject
                    counter += 1

                Next

                updates = False
                changes = True
                'Update title bar
                Me.Text = fileName & "* - C3"

                'Show confirmation message
                Dim message As String = "Updated tape " & identifierShort & " successfully."
                If My.Settings.showMessages = True Then
                    MsgBox(message, MsgBoxStyle.Information, "Successfully Updated Tape")
                End If
                consoleAdd(message)


            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Cannot Save Incomplete Tape")
                Exit Sub

            End Try

        Else

            'No changes to update tape with
            Dim message As String = "No changes to update tape with."
            If My.Settings.showMessages = True Then
                MsgBox(message, MsgBoxStyle.Information, "No Updates to Tape")
            End If
            'consoleAdd(message)

        End If

    End Sub

    Private Sub deleteTape()

        Dim result As MsgBoxResult = MsgBox("Are you sure you want to delete the current tape?" & vbNewLine & "This action cannot be undone.", MsgBoxStyle.YesNoCancel, "Confirm Deletion")

        If result = vbYes Then

            'Get tape identification
            Dim tape As DataRow = tapes.Rows(thisTapeIndex)
            Dim identifierShort As String = CStr(tape("IdentifierShort"))


            'Get index for updating model-specific counter when tape deleted
            Dim modelCode As String = CStr(tape("Model"))
            Dim modelRowReal As DataRow = models.Rows.Find(modelCode)
            Dim modelIndex As Integer = models.Rows.IndexOf(modelRowReal)

            'Remove the record for this tape
            tapes.Rows.Remove(tape)

            'Update tape and model counters
            tapeCount -= 1
            counters.Rows(3)("Number") = tapeCount
            Dim number As Integer = CInt(models.Rows(modelIndex)("Number"))
            models.Rows(modelIndex)("Number") = number - 1

            'Reload data and display latest tape
            loadData()

            changes = True
            'Update title bar
            Me.Text = fileName & "* - C3"

            'Show confirmation message
            Dim message As String = "Deleted tape " & identifierShort & " successfully."
            'If My.Settings.showMessages = True Then
            '    MsgBox(message, MsgBoxStyle.Information, "Successfully Deleted Tape")
            'End If
            consoleAdd(message)

        End If

    End Sub

    Private Sub saveChanges(saveAs As Boolean)

        If updates = True Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before saving changes?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()

                    'SAVE CHANGES
                    saveChangesActual(saveAs, False)

            ElseIf result = vbNo Then

                'SAVE CHANGES
                saveChangesActual(saveAs, False)

            End If

        Else

            'SAVE CHANGES
            saveChangesActual(saveAs, False)

        End If

    End Sub

    Public Sub saveChangesActual(saveAs As Boolean, thenOpen As Boolean)

        'If there is no filepath, it is not saved
        Dim saved As Boolean = filePath IsNot Nothing

        If saved = False Or saveAs = True Then
            'SAVE AS NEW FILE

            Dim dlgResult As DialogResult = dlgSaveAs.ShowDialog()
            Dim selectedPath As String = dlgSaveAs.FileName

            If dlgResult = DialogResult.OK And selectedPath IsNot Nothing Then
                'If user has given a valid file path.

                'Lock in selected file path
                filePath = selectedPath
                Dim fileTree As String() = selectedPath.Split("\"c)
                fileName = fileTree(fileTree.Length - 1)

                'Update file information
                information.Rows(5)("Value") = DateTime.Now.ToString

                catalogue.WriteXml(filePath)

                'Reset changes variable
                changes = False
                'Update title bar
                Me.Text = fileName & " - C3"

                'Show confirmation message
                Dim message As String = "Saved catalogue successfully (as new file)."
                'If My.Settings.showMessages = True Then
                '    MsgBox(message, MsgBoxStyle.Information, "Successfully Saved Catalogue")
                'End If
                consoleAdd(message)

                If thenOpen = True Then
                    openCatalogueActual()
                End If

            ElseIf dlgResult <> DialogResult.Cancel Then
                'If user did NOT deliberately cancel save procedure.

                'Show error message
                MsgBox("Bad file path selected. Catalogue not saved.", MsgBoxStyle.Critical, "File Path Error")

            End If

        Else
            'SAVE OVERWRITE FILE

            'Update file information
            information.Rows(5)("Value") = DateTime.Now.ToString

            catalogue.WriteXml(filePath)

            'Reset changes variable
            changes = False
            'Update title bar
            Me.Text = fileName & " - C3"

            'Show confirmation message
            Dim message As String = "Saved catalogue successfully (overwrote file)."
            'If My.Settings.showMessages = True Then
            '    MsgBox(message, MsgBoxStyle.Information, "Successfully Saved Catalogue")
            'End If
            consoleAdd(message)

            If thenOpen = True Then
                openCatalogueActual()
            End If

        End If

    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click

        saveChanges(True)

    End Sub

    Private Sub OpenCatalogueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenCatalogueToolStripMenuItem.Click
        'Load/open catalogue from XML file

        If updates = True Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before opening catalogue?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()

                    'CHECK CHANGES
                    openCatalogueCheckChanges()

            ElseIf result = vbNo Then

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

        If changes = True Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the catalogue." & vbNewLine & "Save changes before opening new catalogue?", MsgBoxStyle.YesNoCancel, "Changes Made To Catalogue")

            If result = vbYes Then

                saveChangesActual(False, True)

                'OPEN CAT
                openCatalogueActual()

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

        'Get directories
        Dim dlgResult As DialogResult = dlgOpen.ShowDialog()
        Dim selectedPath As String = dlgOpen.FileName

        If dlgResult = DialogResult.OK And selectedPath IsNot Nothing Then
            'If user has given a valid file path.

            Dim fileVersion As String = Nothing

            'Check file format version
            Dim xmlFileText As StreamReader
            xmlFileText = File.OpenText(selectedPath)
            While xmlFileText.Peek <> -1 And fileVersion = Nothing

                If xmlFileText.ReadLine().Contains("File Version") Then
                    'Disect line into version numbers (and strip out tag headers)
                    'Only works for 3 digit with snapshot numbers (x.x.xbx) version numbers

                    Dim numbers As String() = xmlFileText.ReadLine().Split("."c) 'Split <1.2.3b4> into {<1,2,3b4>}
                    numbers(0) = numbers(0).Split(">"c)(1) 'Split <x1 into 1
                    numbers(2) = numbers(2).Split("<"c)(0) 'Split 3b4> into 3b4
                    numbers(2) = Regex.Split(numbers(2), "[a-zA-z]")(0) 'Split 3b4 into 3
                    fileVersion = numbers(0) & "." & numbers(1) & "." & numbers(2)
                End If

            End While

            'Only load if the file version is supported.
            If VERSIONFILESUPPORTED.Contains(fileVersion) Then

                'Lock in selected file path
                filePath = selectedPath
                Dim fileTree As String() = selectedPath.Split("\"c)
                fileName = fileTree(fileTree.Length - 1)

                'Temp Disable strict loading rules
                catalogue.EnforceConstraints = False
                'Clear the existing dataset
                catalogue.Clear()
                'Read the XML file
                catalogue.ReadXml(selectedPath)

                'Reset changes variable
                updates = False
                changes = False
                'Update title bar
                Me.Text = fileName & " - C3"

                'Update file information
                information.Rows(1)("Value") = VERSION
                information.Rows(2)("Value") = VERSIONSTAGE
                information.Rows(3)("Value") = VERSIONDATE.ToString

                'Show confirmation message
                Dim message As String = "Opened catalogue successfully."
                'If My.Settings.showMessages = True Then
                '    MsgBox(message, MsgBoxStyle.Information, "Successfully Saved Catalogue")
                'End If
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
        'Made an update to a field in the main form

        If updates = False And updatesMask = False Then

            updates = True
            'Update title bar
            Me.Text = fileName & "* - C3"
        End If

    End Sub

    Private Sub updateScrollers()

        'Ensure users can't scroll out-of-bounds

        If thisTapeIndex = 0 Then
            btnPrevious.Enabled = False
        Else
            btnPrevious.Enabled = True
        End If

        If thisTapeIndex = tapeCount - 1 Then
            btnNext.Enabled = False
        Else
            btnNext.Enabled = True
        End If

    End Sub

    Private Sub displayTape()

        'Mask update routines
        updatesMask = True

        'Ensure users can't scroll out-of-bounds
        updateScrollers()


        Dim tape As DataRow = tapes.Rows(thisTapeIndex)

        'Display identifiers
        txtLong.Text = CStr(tape("Identifier"))
        txtShort.Text = CStr(tape("IdentifierShort"))
        txtIndex.Text = CStr(thisTapeIndex + 1)
        txtTotal.Text = CStr(tapeCount)

        'Find model name from identification/code
        Dim modelCode As String = CStr(tape("Model"))
        Dim modelRow As DataRow = models.Rows.Find(modelCode)
        Dim modelName As String = CStr(modelRow("Brand")) & " " & CStr(modelRow("Model"))
        'Find model type
        thisModelType = CInt(modelCode.Substring(2, 1))

        'Populate groups and elements
        txtModel.Text = modelName
        numYear.Value = CInt(tape("Year"))
        numLength.Value = CInt(tape("Length"))
        cmbRegion.Text = CStr(tape("Region"))
        txtNumber.Text = CStr(tape("Number"))

        Dim condition As Integer = getCondition(CInt(tape("Condition")))
        cmbCondition.SelectedIndex = condition
        chkPackaged.Checked = CBool(tape("Packaged"))

        'Enable "taped sides" groups and load data

        thisTapedA = CBool(tape("TapedA"))
        If thisTapedA = True Then
            'If side A recorded, load data

            chkTapedA.Checked = True

            txtNameA.Text = CStr(tape("NameA"))
            datRecordedA.Value = CDate(tape("RecordedA"))

            cmbDeckA.Text = CStr(tape("DeckA"))
            cmbInputA.Text = CStr(tape("InputA"))

            numPeakA.Value = CInt(tape("PeakA"))
            numLevelA.Value = CDec(tape("LevelA"))
            numLevelCalA.Value = CDec(tape("LevelCalA"))

            cmbEQA.Text = CStr(tape("EQA"))
            cmbBiasA.SelectedIndex = CInt(tape("BiasA")) - 1
            numBiasCalA.Value = CInt(tape("BiasCalA"))

            cmbNRA.Text = CStr(tape("NRA"))
            chkHXA.Checked = CBool(tape("HXA"))
            chkMPXA.Checked = CBool(tape("MPXA"))

            cmbSpeedA.Text = CStr(tape("SpeedA"))
            chkDubbedA.Checked = CBool(tape("DubbedA"))

            'Contents for recording
            cmbContentsA.Text = CStr(tape("ContentsA"))
            txtArtistA.Text = CStr(tape("ArtistA"))
            txtTitleA.Text = CStr(tape("TitleA"))

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

        thisTapedB = CBool(tape("TapedB"))
        If thisTapedB = True Then

            chkTapedB.Checked = True

            txtNameB.Text = CStr(tape("NameB"))
            datRecordedB.Value = CDate(tape("RecordedB"))

            cmbDeckB.Text = CStr(tape("DeckB"))
            cmbInputB.Text = CStr(tape("InputB"))

            numPeakB.Value = CInt(tape("PeakB"))
            numLevelB.Value = CDec(tape("LevelB"))
            numLevelCalB.Value = CDec(tape("LevelCalB"))

            cmbEQB.Text = CStr(tape("EQB"))
            cmbBiasB.SelectedIndex = CInt(tape("BiasB")) - 1
            numBiasCalB.Value = CInt(tape("BiasCalB"))

            cmbNRB.Text = CStr(tape("NRB"))
            chkHXB.Checked = CBool(tape("HXB"))
            chkMPXB.Checked = CBool(tape("MPXB"))

            cmbSpeedB.Text = CStr(tape("SpeedB"))
            chkDubbedB.Checked = CBool(tape("DubbedB"))

            'Contents for recording
            cmbContentsB.Text = CStr(tape("ContentsB"))
            txtArtistB.Text = CStr(tape("ArtistB"))
            txtTitleB.Text = CStr(tape("TitleB"))

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
        txtNotes.Text = CStr(tape("Notes"))

        'Unmask update routines
        updatesMask = False

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        MsgBox("Compact Cassette Catalogue (C3)" & vbNewLine & "© Jules Carboni, 2019" & vbNewLine & vbNewLine & "Program Version: " & VERSIONSTAGE & " " & VERSION & vbNewLine & "Catalogue Version: " & VERSIONFILE & vbNewLine & VERSIONDATE.ToLongDateString & ", " & VERSIONDATE.ToLongTimeString, MsgBoxStyle.Information, "About C3")
    End Sub

    Private Sub SearchTapesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchTapesToolStripMenuItem.Click
        frmTapes.Show() 'temp
    End Sub

    Private Sub SearchModelsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchModelsToolStripMenuItem.Click
        frmModels.Show() 'temp
    End Sub

    Private Sub SearchManufacturersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchManufacturersToolStripMenuItem.Click
        frmViewBrands.Show() 'temp
    End Sub

    Private Sub ViewDecksToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewDecksToolStripMenuItem.Click
        frmDecks.Show() 'temp
    End Sub

    Private Sub ViewStatisticsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewStatisticsToolStripMenuItem.Click
        frmStatistics.Show() 'temp
    End Sub


    Private Sub ChkTapedA_CheckedChanged(sender As Object, e As EventArgs) Handles chkTapedA.CheckedChanged

        updateMade()

        If chkTapedA.Checked = True Then

            deckCount = CInt(counters.Rows(0)("Number"))

            'Check that at least 1 deck exists
            If deckCount >= 1 Then

                If thisTapedA = False Then

                    'Set defaults
                    datRecordedA.Value = Date.Today
                    cmbDeckA.SelectedIndex = 0
                    'cmbDeckA.SelectedIndex = cmbDeckA.Items.Count - 1 'Latest deck

                    cmbInputA.SelectedIndex = 10 'Phone input
                    cmbNRA.SelectedIndex = 1 'Dolby B
                    cmbSpeedA.SelectedIndex = 1 'Normal speed
                    cmbContentsA.SelectedIndex = 0
                    numLevelA.Value = CDec(5)

                    If thisModelType = 1 Then 'If normal bias
                        cmbEQA.SelectedIndex = 0 '120us
                    Else
                        cmbEQA.SelectedIndex = 1 '70us
                    End If
                    cmbBiasA.SelectedIndex = thisModelType - 1

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

            deckCount = CInt(counters.Rows(0)("Number"))

            'Check that at least 1 deck exists
            If deckCount >= 1 Then

                If thisTapedB = False Then

                    'Set defaults
                    datRecordedB.Value = Date.Today
                    cmbDeckB.SelectedIndex = 0
                    'cmbDeckB.SelectedIndex = cmbDeckB.Items.Count - 1 'Latest deck

                    cmbInputB.SelectedIndex = 10 'Phone input
                    cmbNRB.SelectedIndex = 1 'Dolby B
                    cmbSpeedB.SelectedIndex = 1 'Normal speed
                    cmbContentsB.SelectedIndex = 0
                    numLevelB.Value = CDec(5)

                    If thisModelType = 1 Then 'If normal bias
                        cmbEQB.SelectedIndex = 0 '120us
                    Else
                        cmbEQB.SelectedIndex = 1 '70us
                    End If
                    cmbBiasB.SelectedIndex = thisModelType - 1

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
        frmFindResults.Show() ''temp
    End Sub

    Private Sub NewDeckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewDeckToolStripMenuItem.Click
        frmDeckNew.Show() 'temp
    End Sub

    Private Sub NewManufactererToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewManufactererToolStripMenuItem.Click
        frmBrandNew.Show() 'temp
    End Sub

    Private Sub NewModelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewModelToolStripMenuItem.Click

        If CInt(counters.Rows(1)("Number")) > 0 Then
            frmModelNew.Show()
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

        If updates = True Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before adding new tape?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()

                    'ADD A NEW TAPE
                    addNewTapeActual()

            ElseIf result = vbNo Then

                'ADD A NEW TAPE
                addNewTapeActual()

            End If

        Else

            'ADD A NEW TAPE
            addNewTapeActual()

        End If

    End Sub

    Private Sub addNewTapeActual()

        modelCount = CInt(counters.Rows(2)("Number"))

        'Check that there is at least 1 model (and 1 deck for recording)

        If modelCount >= 1 Then
            frmTapeNew.Show() 'temp

        Else
            MsgBox("Add at least one model first.", MsgBoxStyle.Exclamation, "No Models")

        End If

    End Sub

    Private Sub closeApplication()
        'Check for unsaved changes to the whole catalogue, offer to save, then close

        If changes = True Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the catalogue." & vbNewLine & "Save changes before closing?", MsgBoxStyle.YesNoCancel, "Changes Made To Catalogue")

            If result = vbYes Then

                saveChanges(False)

                'CLOSE
                Application.Exit()

            ElseIf result = vbNo Then

                'CLOSE
                Application.Exit()

            End If

        Else

            'CLOSE
            Application.Exit()

        End If

    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        addNewTape()

    End Sub

    Private Sub NewTapeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewTapeToolStripMenuItem.Click

        addNewTape()

    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        deleteTape()

    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        saveChanges(False)

    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click

        saveChanges(False)

    End Sub

    Private Sub BtnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click

        If updates = True Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before scrolling?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()

                    'SCROLL PREVIOUS

                    thisTapeIndex -= 1
                    displayTape()

            ElseIf result = vbNo Then
                'SCROLL PREVIOUS

                thisTapeIndex -= 1
                displayTape()

            End If

        Else

            'SCROLL PREVIOUS

            thisTapeIndex -= 1
            displayTape()

        End If

    End Sub

    Private Sub BtnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click

        If updates = True Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before scrolling?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()

                    'SCROLL NEXT

                    thisTapeIndex += 1
                    displayTape()

            ElseIf result = vbNo Then
                'SCROLL NEXT

                thisTapeIndex += 1
                displayTape()

            End If

        Else

            'SCROLL NEXT

            thisTapeIndex += 1
            displayTape()

        End If

    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click

        closeApplication()

    End Sub

    Private Sub frmMain_Close(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        'Handle the X button

        closeApplication()

        e.Cancel = True

    End Sub

    Private Sub ShowConsoleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowConsoleToolStripMenuItem.Click

        frmConsole.Show()

    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        'Make a new catalogue (check before saving first)

        'Check if tape updated
        If updates = True Then

            Dim result As MsgBoxResult = MsgBox("Changes have been made to the current tape." & vbNewLine & "Update current tape before creating new catalogue?", MsgBoxStyle.YesNoCancel, "Changes Made To Tape")

            If result = vbYes Then

                updateTape()

                    'NEW CAT
                    newCatalogueCheckChanges()

            ElseIf result = vbNo Then

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

        If changes = True Then

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

        ''temp
        'open feedback webpage (email?)

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

    Private Sub chkHXB_CheckedChanged(sender As Object, e As EventArgs) Handles chkHXB.CheckedChanged

        updateMade()

    End Sub

    Private Sub cmbSpeedB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSpeedB.SelectedIndexChanged

        updateMade()

    End Sub

    Private Sub chkDubbedB_CheckedChanged(sender As Object, e As EventArgs) Handles chkDubbedB.CheckedChanged

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

    Private Sub UpdateTapeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateTapeToolStripMenuItem.Click

        updateTape()

    End Sub

    Private Sub DeleteTapeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteTapeToolStripMenuItem.Click

        deleteTape()

    End Sub
End Class