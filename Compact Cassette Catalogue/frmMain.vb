' Module: Main & Edit Tape
' Purpose: To edit existing tapes and access the other functions of the program.
' Author: Jules Carboni
' Date Created: 22 Aug 2019

Imports System.Xml
Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Schema

Public Class frmMain

    Public Const MaximumCatalogueBytes As Long = 16L * 1024L * 1024L

    Public Enum EditChoice
        Apply
        Discard
        Cancel
    End Enum

    Public Enum DocumentChoice
        Save
        Discard
        Cancel
    End Enum

    'Declare variables
    Dim updatesMask As Boolean = True
    'Initialise tape index to last tape
    Dim thisTapeIndex As Integer = tapeCount - 1
    Dim thisModelType As Integer
    Dim thisTapedA As Boolean
    Dim thisTapedB As Boolean

    Private loadedFileRevision As String = Nothing
    Private closeApproved As Boolean = False

    Public Shared Function TransitionCanContinue(hasPendingEdit As Boolean,
                                                  editDecision As EditChoice,
                                                  editApplySucceeded As Boolean,
                                                  hasDocumentChanges As Boolean,
                                                  documentDecision As DocumentChoice,
                                                  documentSaveSucceeded As Boolean) As Boolean

        If hasPendingEdit Then
            If editDecision = EditChoice.Cancel Then
                Return False
            End If
            If editDecision = EditChoice.Apply AndAlso Not editApplySucceeded Then
                Return False
            End If
        End If

        If hasDocumentChanges Then
            If documentDecision = DocumentChoice.Cancel Then
                Return False
            End If
            If documentDecision = DocumentChoice.Save AndAlso Not documentSaveSucceeded Then
                Return False
            End If
        End If

        Return True

    End Function

    Public Shared Function LoadCatalogueSnapshot(cataloguePath As String,
                                                 schemaSource As DataSet,
                                                 supportedVersions As String()) As DataSet
        Dim ignoredRevision As String = Nothing
        Return LoadCatalogueSnapshot(cataloguePath, schemaSource, supportedVersions, ignoredRevision)
    End Function

    Public Shared Function LoadCatalogueSnapshot(cataloguePath As String,
                                                 schemaSource As DataSet,
                                                 supportedVersions As String(),
                                                 ByRef revision As String) As DataSet

        If schemaSource Is Nothing Then
            Throw New ArgumentNullException("schemaSource")
        End If
        If supportedVersions Is Nothing OrElse supportedVersions.Length = 0 Then
            Throw New ArgumentException("At least one supported catalogue version is required.", "supportedVersions")
        End If

        Dim bytes As Byte() = ReadCatalogueBytes(cataloguePath)
        ValidateCatalogueXml(bytes, schemaSource)

        Dim temporaryCatalogue As DataSet = schemaSource.Clone()
        temporaryCatalogue.EnforceConstraints = False
        Using stream As New MemoryStream(bytes, False)
            Using reader As XmlReader = XmlReader.Create(stream, CreateSecureCatalogueReaderSettings())
                temporaryCatalogue.ReadXml(reader, XmlReadMode.IgnoreSchema)
            End Using
        End Using
        temporaryCatalogue.EnforceConstraints = True

        Dim fileVersion As String = GetCatalogueVersion(temporaryCatalogue)
        Dim versionSupported As Boolean = False
        For Each supportedVersion As String In supportedVersions
            If String.Equals(fileVersion, supportedVersion, StringComparison.Ordinal) Then
                versionSupported = True
                Exit For
            End If
        Next
        If Not versionSupported Then
            Throw New InvalidDataException("Unsupported or missing catalogue file version: " & If(fileVersion, "(missing)"))
        End If

        revision = HashBytes(bytes)
        Return temporaryCatalogue

    End Function

    Public Shared Function SaveCatalogueTransactional(snapshot As DataSet,
                                                       destinationPath As String,
                                                       faultStage As String) As String

        If snapshot Is Nothing Then
            Throw New ArgumentNullException("snapshot")
        End If
        If String.IsNullOrWhiteSpace(destinationPath) Then
            Throw New ArgumentException("A destination path is required.", "destinationPath")
        End If

        Dim fullDestination As String = Path.GetFullPath(destinationPath)
        Dim destinationDirectory As String = Path.GetDirectoryName(fullDestination)
        If String.IsNullOrWhiteSpace(destinationDirectory) OrElse Not Directory.Exists(destinationDirectory) Then
            Throw New DirectoryNotFoundException("The catalogue destination directory does not exist.")
        End If

        Dim temporaryPath As String = Path.Combine(destinationDirectory, ".c3-save-" & Guid.NewGuid().ToString("N") & ".tmp")
        Dim backupPath As String = fullDestination & ".bak"
        Try
            InjectSaveFault(faultStage, "create")
            Using output As New FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough)
                InjectSaveFault(faultStage, "write")
                snapshot.WriteXml(output, XmlWriteMode.IgnoreSchema)
                InjectSaveFault(faultStage, "flush")
                output.Flush(True)
            End Using

            InjectSaveFault(faultStage, "reopen")
            Dim version As String = GetCatalogueVersion(snapshot)
            LoadCatalogueSnapshot(temporaryPath, snapshot, New String() {version})

            InjectSaveFault(faultStage, "cleanup")
            InjectSaveFault(faultStage, "backup")
            If File.Exists(fullDestination) Then
                If File.Exists(backupPath) Then
                    File.Delete(backupPath)
                End If
                InjectSaveFault(faultStage, "replace")
                File.Replace(temporaryPath, fullDestination, backupPath, True)
            Else
                InjectSaveFault(faultStage, "replace")
                File.Move(temporaryPath, fullDestination)
            End If

            Return CaptureFileRevision(fullDestination)
        Finally
            If File.Exists(temporaryPath) Then
                File.Delete(temporaryPath)
            End If
        End Try

    End Function

    Public Shared Function CaptureFileRevision(cataloguePath As String) As String
        Return HashBytes(ReadCatalogueBytes(cataloguePath))
    End Function

    Public Shared Function FileRevisionMatches(cataloguePath As String, expectedRevision As String) As Boolean
        If String.IsNullOrEmpty(expectedRevision) OrElse Not File.Exists(cataloguePath) Then
            Return False
        End If
        Try
            Return String.Equals(CaptureFileRevision(cataloguePath), expectedRevision, StringComparison.Ordinal)
        Catch ex As IOException
            Return False
        Catch ex As UnauthorizedAccessException
            Return False
        End Try
    End Function

    Public Shared Sub AssignTapeValues(tape As DataRow, values As IDictionary(Of String, Object))
        If tape Is Nothing Then
            Throw New ArgumentNullException("tape")
        End If
        If values Is Nothing Then
            Throw New ArgumentNullException("values")
        End If

        tape.BeginEdit()
        Try
            For Each pair As KeyValuePair(Of String, Object) In values
                If String.Equals(pair.Key, "IdentifierShort", StringComparison.Ordinal) OrElse
                        String.Equals(pair.Key, "Number", StringComparison.Ordinal) OrElse
                        String.Equals(pair.Key, "Date", StringComparison.Ordinal) Then
                    Throw New InvalidOperationException("Tape identity and creation-date fields are immutable during edit: " & pair.Key)
                End If
                If Not tape.Table.Columns.Contains(pair.Key) Then
                    Throw New InvalidOperationException("Unknown tape field: " & pair.Key)
                End If
                tape(pair.Key) = If(pair.Value, DBNull.Value)
            Next
            tape.EndEdit()
        Catch
            tape.CancelEdit()
            Throw
        End Try
    End Sub

    Private Shared Function ReadCatalogueBytes(cataloguePath As String) As Byte()
        If String.IsNullOrWhiteSpace(cataloguePath) Then
            Throw New ArgumentException("A catalogue path is required.", "cataloguePath")
        End If

        Using input As New FileStream(cataloguePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            If input.Length > MaximumCatalogueBytes Then
                Throw New InvalidDataException("Catalogue exceeds the 16 MiB safety limit.")
            End If
            If input.Length = 0 Then
                Throw New InvalidDataException("Catalogue file is empty.")
            End If
            Dim bytes(CInt(input.Length) - 1) As Byte
            Dim offset As Integer = 0
            While offset < bytes.Length
                Dim read As Integer = input.Read(bytes, offset, bytes.Length - offset)
                If read = 0 Then
                    Throw New EndOfStreamException("Catalogue ended before its declared length.")
                End If
                offset += read
            End While
            Return bytes
        End Using
    End Function

    Private Shared Function CreateSecureCatalogueReaderSettings() As XmlReaderSettings
        Dim settings As New XmlReaderSettings()
        settings.DtdProcessing = DtdProcessing.Prohibit
        settings.XmlResolver = Nothing
        settings.MaxCharactersInDocument = MaximumCatalogueBytes
        settings.MaxCharactersFromEntities = 0L
        Return settings
    End Function

    Private Shared Sub ValidateCatalogueXml(bytes As Byte(), schemaSource As DataSet)
        Dim settings As XmlReaderSettings = CreateSecureCatalogueReaderSettings()
        settings.ValidationType = ValidationType.Schema
        settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
        Using schemaText As New StringReader(schemaSource.GetXmlSchema())
            Using schemaReader As XmlReader = XmlReader.Create(schemaText, CreateSecureCatalogueReaderSettings())
                settings.Schemas.Add(Nothing, schemaReader)
            End Using
        End Using
        Using stream As New MemoryStream(bytes, False)
            Using reader As XmlReader = XmlReader.Create(stream, settings)
                While reader.Read()
                End While
            End Using
        End Using
    End Sub

    Private Shared Function GetCatalogueVersion(source As DataSet) As String
        If Not source.Tables.Contains("Information") Then
            Return Nothing
        End If
        Dim table As DataTable = source.Tables("Information")
        If Not table.Columns.Contains("Information") OrElse Not table.Columns.Contains("Value") Then
            Return Nothing
        End If
        For Each row As DataRow In table.Rows
            If String.Equals(Convert.ToString(row("Information"), Globalization.CultureInfo.InvariantCulture).Trim(), "File Version", StringComparison.Ordinal) Then
                Return normaliseCatalogueFileVersionShared(Convert.ToString(row("Value"), Globalization.CultureInfo.InvariantCulture))
            End If
        Next
        Return Nothing
    End Function

    Private Shared Function normaliseCatalogueFileVersionShared(rawVersion As String) As String
        If rawVersion Is Nothing Then
            Return Nothing
        End If
        Dim versionMatch As Match = Regex.Match(rawVersion.Trim(), "^(\d+)\.(\d+)\.(\d+)")
        If versionMatch.Success Then
            Return versionMatch.Groups(1).Value & "." & versionMatch.Groups(2).Value & "." & versionMatch.Groups(3).Value
        End If
        Return rawVersion.Trim()
    End Function

    Private Shared Function HashBytes(bytes As Byte()) As String
        Using hash As SHA256 = SHA256.Create()
            Return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", String.Empty).ToLowerInvariant()
        End Using
    End Function

    Private Shared Sub InjectSaveFault(requestedStage As String, currentStage As String)
        If String.Equals(requestedStage, currentStage, StringComparison.Ordinal) Then
            Throw New IOException("Injected catalogue save fault at stage: " & currentStage)
        End If
    End Sub

    'Dim newTape As Object() = {"", 0, 0, "", 0, "Unsaved", 0, False, False, False, "", CDate("1/1/1970"), "", "", 0, "", False, False, False, 0, 0, "", 0, 0, "", "", "", "", CDate("1/1/1970"), "", "", 0, "", False, False, False, 0, 0, "", 0, 0, "", ""} 'Default record for a new blank tape

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Indicate to subroutines that the program is in the 'initial setup' phase.
        duringSetup = True

        ' Update varaibles.
        'deckCount = CInt(counters.Rows(0)("Number"))
        'brandCount = CInt(counters.Rows(1)("Number"))
        'modelCount = CInt(counters.Rows(2)("Number"))
        'tapeCount = CInt(counters.Rows(3)("Number"))
        ' Re-assert variables that for some reasion change.
        'changes = False
        'updates = False

        ' Display about information.
        lblAbout.Text = "© " & COPYRIGHTAUTHOR & ", " & VERSIONSTAGE & " " & VERSION & " (" & COPYRIGHTYEAR & ")"

        'A dd tables to data set (a global process).
        catalogue.Tables.Add(information)
        catalogue.Tables.Add(counters)
        catalogue.Tables.Add(decks)
        catalogue.Tables.Add(brands)
        catalogue.Tables.Add(models)
        catalogue.Tables.Add(tapes)

        ' Initialise objects.
        cmbField.SelectedIndex = 0
        ' Update date boundaries.
        datRecordedA.MinDate = CDate("30/8/1963")
        datRecordedB.MinDate = CDate("30/8/1963")
        datRecordedA.MaxDate = Date.Today
        datRecordedB.MaxDate = Date.Today
        numYear.Maximum = Date.Today.Year

        ' Load console.
        frmConsole.Show()
        frmConsole.Hide()

        ' Load data (decks, brands and models).
        loadData()

        consoleAdd("Successfully loaded program.") ' Add success note to console.

        ' Check for updates if automatic update checks are enabled.
        Dim updatePolicy As String = normaliseUpdateCheckPolicy(My.Settings.checkUpdates)
        If My.Settings.checkUpdates <> updatePolicy Then
            My.Settings.checkUpdates = updatePolicy
        End If

        If shouldRunAutomaticUpdateCheck(updatePolicy) Then
            checkUpdates(False)
        End If

        ' Indicate to subroutines that the program has finished the 'initial setup' phase.
        duringSetup = False

    End Sub

    Private Function normaliseUpdateCheckPolicy(policy As String) As String

        Select Case policy
            Case "startup", "weekly", "monthly", "never"
                Return policy
            Case "manually"
                Return "never"
            Case Else
                Return "never"
        End Select

    End Function

    Private Function shouldRunAutomaticUpdateCheck(policy As String) As Boolean

        Select Case policy
            Case "startup"
                Return True
            Case "weekly", "monthly"
                Try

                    Dim daysSinceUpdate As Double = (DateTime.Now - My.Settings.lastUpdateCheck).TotalDays

                    If policy = "weekly" Then
                        Return daysSinceUpdate >= 7
                    Else
                        Return daysSinceUpdate >= 28
                    End If

                Catch ex As Exception

                    consoleAdd("Failed to read last update check date. Automatic update check will run.")
                    Return True

                End Try
            Case Else
                Return False
        End Select

    End Function

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
            Using updateReader As New StreamReader(updateClient.OpenRead(UPDATELINKCHECK))

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
        My.Settings.lastUpdateCheck = DateTime.Now


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
            cmbDeckB.Items.Clear()

            For i As Integer = 0 To deckCount - 1
                Dim row As DataRow = decks.Rows(i)

                Dim thisDeck As String = CStr(row("Name"))
                cmbDeckA.Items.Add(thisDeck)
                cmbDeckB.Items.Add(thisDeck)
            Next

        End If

        'Load latest tape if any exist
        If tapeCount > 0 Then

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
        updatesMask = False

    End Sub

    Private Function updateTape() As Boolean

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
                Dim peakA As Object = DBNull.Value
                Dim biasCalA As Object = DBNull.Value

                Dim nameA As Object = DBNull.Value
                Dim recordedA As Object = DBNull.Value
                Dim deckA As Object = DBNull.Value
                Dim inputA As Object = DBNull.Value
                Dim speedA As Object = DBNull.Value

                Dim NRA As Object = DBNull.Value
                Dim HXA As Object = DBNull.Value
                Dim MPXA As Object = DBNull.Value
                Dim dubbedA As Object = DBNull.Value

                Dim EQA As Object = DBNull.Value
                Dim levelA As Object = DBNull.Value
                Dim levelCalA As Object = DBNull.Value

                Dim contentsA As Object = DBNull.Value
                Dim artistA As Object = DBNull.Value
                Dim titleA As Object = DBNull.Value

                'B side values
                Dim peakB As Object = DBNull.Value
                Dim biasCalB As Object = DBNull.Value

                Dim nameB As Object = DBNull.Value
                Dim recordedB As Object = DBNull.Value
                Dim deckB As Object = DBNull.Value
                Dim inputB As Object = DBNull.Value
                Dim speedB As Object = DBNull.Value

                Dim NRB As Object = DBNull.Value
                Dim HXB As Object = DBNull.Value
                Dim MPXB As Object = DBNull.Value
                Dim dubbedB As Object = DBNull.Value

                Dim EQB As Object = DBNull.Value
                Dim levelB As Object = DBNull.Value
                Dim levelCalB As Object = DBNull.Value

                Dim contentsB As Object = DBNull.Value
                Dim artistB As Object = DBNull.Value
                Dim titleB As Object = DBNull.Value

                'Only save real values if that side has been marked as recorded
                If packaged = False Then

                    If tapedA = True Then

                        nameA = txtNameA.Text
                        recordedA = datRecordedA.Value
                        deckA = cmbDeckA.Text
                        inputA = cmbInputA.Text
                        peakA = CInt(numPeakA.Value)
                        speedA = cmbSpeedA.Text

                        NRA = cmbNRA.Text
                        HXA = chkHXA.Checked
                        MPXA = chkMPXA.Checked
                        dubbedA = chkDubbedA.Checked

                        EQA = cmbEQA.Text
                        biasCalA = CInt(numBiasCalA.Value)
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
                        peakB = CInt(numPeakB.Value)
                        speedB = cmbSpeedB.Text

                        NRB = cmbNRB.Text
                        HXB = chkHXB.Checked
                        MPXB = chkMPXB.Checked
                        dubbedB = chkDubbedB.Checked

                        EQB = cmbEQB.Text
                        biasCalB = CInt(numBiasCalB.Value)
                        levelB = numLevelB.Value
                        levelCalB = numLevelCalB.Value

                        contentsB = cmbContentsB.Text
                        artistB = txtArtistB.Text
                        titleB = txtTitleB.Text

                    End If

                End If


                'Write data to record

                Dim tapeValues As New Dictionary(Of String, Object) From {
                    {"Model", modelCode}, {"Year", year}, {"Length", length},
                    {"Region", cmbRegion.Text}, {"Identifier", identifier},
                    {"Condition", condition}, {"Packaged", packaged},
                    {"TapedA", tapedA}, {"TapedB", tapedB},
                    {"NameA", nameA}, {"RecordedA", recordedA}, {"DeckA", deckA},
                    {"InputA", inputA}, {"PeakA", peakA}, {"NRA", NRA},
                    {"HXA", HXA}, {"MPXA", MPXA}, {"DubbedA", dubbedA},
                    {"SpeedA", speedA}, {"BiasA", biasCodeA}, {"BiasCalA", biasCalA},
                    {"EQA", EQA}, {"LevelA", levelA}, {"LevelCalA", levelCalA},
                    {"ContentsA", contentsA}, {"ArtistA", artistA}, {"TitleA", titleA},
                    {"NameB", nameB}, {"RecordedB", recordedB}, {"DeckB", deckB},
                    {"InputB", inputB}, {"PeakB", peakB}, {"NRB", NRB},
                    {"HXB", HXB}, {"MPXB", MPXB}, {"DubbedB", dubbedB},
                    {"SpeedB", speedB}, {"BiasB", biasCodeB}, {"BiasCalB", biasCalB},
                    {"EQB", EQB}, {"LevelB", levelB}, {"LevelCalB", levelCalB},
                    {"ContentsB", contentsB}, {"ArtistB", artistB}, {"TitleB", titleB},
                    {"Notes", txtNotes.Text}}
                AssignTapeValues(tape, tapeValues)

                updates = False
                changes = True

                'Update title bar
                Me.Text = fileName & "* - C3"
                'Update buttons
                btnUpdate.Enabled = False
                UpdateTapeToolStripMenuItem.Enabled = False


                'Show confirmation message
                Dim message As String = "Updated tape " & identifierShort & " successfully."
                If My.Settings.showMessages = True Then
                    MsgBox(message, MsgBoxStyle.Question, "Successfully Updated Tape")
                End If
                consoleAdd(message)

                Return True


            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Cannot Save Incomplete Tape")
                Return False

            End Try

        Else

            'No changes to update tape with
            Dim message As String = "No changes to update tape with."
            If My.Settings.showMessages = True Then
                MsgBox(message, MsgBoxStyle.Question, "No Updates to Tape")
            End If
            'consoleAdd(message)

        End If

        Return True

    End Function

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

            'Get tape identification
            Dim tape As DataRow = tapes.Rows(subTapeIndex)
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

            'Reset change detection variables
            updates = False
            changes = True
            'Reset buttons
            btnUpdate.Enabled = False
            UpdateTapeToolStripMenuItem.Enabled = False

            'Update title bar
            Me.Text = fileName & "* - C3"

            'Reload data and display latest tape
            loadData()

            'Show confirmation message
            Dim message As String = "Deleted tape " & identifierShort & " successfully."
            'If My.Settings.showMessages = True Then
            '    MsgBox(message, MsgBoxStyle.Question, "Successfully Deleted Tape")
            'End If
            consoleAdd(message)

        End If

    End Sub

    Private Function resolvePendingTapeEdit(actionDescription As String) As Boolean
        If Not updates Then
            Return True
        End If

        Dim result As MsgBoxResult = MsgBox(
            "Changes have been made to the current tape." & vbNewLine &
            "Update current tape before " & actionDescription & "?",
            MsgBoxStyle.YesNoCancel,
            "Changes Made To Tape")

        If result = vbCancel Then
            Return False
        End If
        If result = vbYes Then
            Return updateTape()
        End If
        Return True
    End Function

    Private Function resolveCatalogueChanges(actionDescription As String) As Boolean
        If Not changes Then
            Return True
        End If

        Dim result As MsgBoxResult = MsgBox(
            "Changes have been made to the catalogue." & vbNewLine &
            "Save changes before " & actionDescription & "?",
            MsgBoxStyle.YesNoCancel,
            "Changes Made To Catalogue")

        If result = vbCancel Then
            Return False
        End If
        If result = vbYes Then
            Return saveChangesActual(False, False)
        End If
        Return True
    End Function

    Private Function saveChanges(saveAs As Boolean) As Boolean
        If Not resolvePendingTapeEdit("saving changes") Then
            Return False
        End If
        Return saveChangesActual(saveAs, False)
    End Function

    Public Function saveChangesActual(saveAs As Boolean, thenOpen As Boolean) As Boolean
        Dim targetPath As String = filePath
        Dim saveAsNewFile As Boolean = String.IsNullOrWhiteSpace(targetPath) OrElse saveAs

        If saveAsNewFile Then
            Dim dlgResult As DialogResult = dlgSaveAs.ShowDialog()
            If dlgResult = DialogResult.Cancel Then
                Return False
            End If
            If dlgResult <> DialogResult.OK OrElse String.IsNullOrWhiteSpace(dlgSaveAs.FileName) Then
                MsgBox("Bad file path selected. Catalogue not saved.", MsgBoxStyle.Critical, "File Path Error")
                Return False
            End If
            targetPath = Path.GetFullPath(dlgSaveAs.FileName)
        ElseIf Not FileRevisionMatches(targetPath, loadedFileRevision) Then
            Dim externalResult As MsgBoxResult = MsgBox(
                "The catalogue file has changed outside C3 since it was opened or saved." & vbNewLine &
                "C3 will not overwrite those external changes." & vbNewLine & vbNewLine &
                "Save your current catalogue to a different file?",
                MsgBoxStyle.YesNo Or MsgBoxStyle.Exclamation,
                "Catalogue Changed Outside C3")
            If externalResult = vbYes Then
                Return saveChangesActual(True, False)
            End If
            Return False
        End If

        Dim snapshot As DataSet = catalogue.Copy()
        Dim modifiedRow As DataRow = snapshot.Tables("Information").Rows.Find("File Modified")
        If modifiedRow Is Nothing Then
            Throw New InvalidDataException("Catalogue information is missing the File Modified row.")
        End If
        Dim modifiedValue As String = DateTime.Now.ToString()
        modifiedRow("Value") = modifiedValue

        Dim newRevision As String
        Try
            newRevision = SaveCatalogueTransactional(snapshot, targetPath, Nothing)
        Catch ex As Exception
            consoleAdd("Failed to save catalogue. Error: " & ex.Message)
            MsgBox("The catalogue was not saved. The previous destination bytes were preserved." &
                   vbNewLine & vbNewLine & "Error: " & ex.Message,
                   MsgBoxStyle.Critical,
                   "Catalogue Save Error")
            Return False
        End Try

        information.Rows.Find("File Modified")("Value") = modifiedValue
        filePath = targetPath
        fileName = Path.GetFileName(targetPath)
        fileDirectory = Path.GetDirectoryName(targetPath) & Path.DirectorySeparatorChar
        loadedFileRevision = newRevision
        updates = False
        changes = False
        btnUpdate.Enabled = False
        UpdateTapeToolStripMenuItem.Enabled = False
        Me.Text = fileName & " - C3"
        loadData()

        Dim message As String
        If saveAsNewFile Then
            message = "Saved catalogue successfully (as new file)."
        Else
            message = "Saved catalogue successfully (transactional overwrite with backup)."
        End If
        consoleAdd(message)
        Return True
    End Function

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click

        saveChanges(True)

    End Sub

    Private Sub OpenCatalogueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenCatalogueToolStripMenuItem.Click
        If resolvePendingTapeEdit("opening another catalogue") AndAlso
                resolveCatalogueChanges("opening another catalogue") Then
            openCatalogueActual()
        End If
    End Sub

    Sub openCatalogueCheckChanges()
        If resolveCatalogueChanges("opening another catalogue") Then
            openCatalogueActual()
        End If
    End Sub

    Public Function openCatalogueActual() As Boolean
        Dim dlgResult As DialogResult = dlgOpen.ShowDialog()
        If dlgResult = DialogResult.Cancel Then
            Return False
        End If
        If dlgResult <> DialogResult.OK OrElse String.IsNullOrWhiteSpace(dlgOpen.FileName) Then
            MsgBox("Bad file path selected. Catalogue not opened.", MsgBoxStyle.Critical, "File Path Error")
            Return False
        End If

        Dim selectedPath As String = Path.GetFullPath(dlgOpen.FileName)
        Dim selectedRevision As String = Nothing
        Dim temporaryCatalogue As DataSet
        Try
            temporaryCatalogue = LoadCatalogueSnapshot(selectedPath, catalogue, VERSIONFILESUPPORTED, selectedRevision)
        Catch ex As Exception
            consoleAdd("Failed to load catalogue. Active catalogue preserved. Error: " & ex.Message)
            MsgBox("The selected catalogue could not be completely validated." & vbNewLine &
                   "The active catalogue was not changed." & vbNewLine & vbNewLine &
                   "Error: " & ex.Message,
                   MsgBoxStyle.Critical,
                   "Catalogue Load Error")
            Return False
        End Try

        Dim previousCatalogue As DataSet = catalogue.Copy()
        Try
            replaceCatalogueData(catalogue, temporaryCatalogue)
        Catch ex As Exception
            replaceCatalogueData(catalogue, previousCatalogue)
            consoleAdd("Failed to activate validated catalogue. Active catalogue restored. Error: " & ex.Message)
            MsgBox("The selected catalogue was validated but could not be activated." & vbNewLine &
                   "The active catalogue was restored." & vbNewLine & vbNewLine &
                   "Error: " & ex.Message,
                   MsgBoxStyle.Critical,
                   "Catalogue Activation Error")
            Return False
        End Try

        filePath = selectedPath
        fileName = Path.GetFileName(selectedPath)
        fileDirectory = Path.GetDirectoryName(selectedPath) & Path.DirectorySeparatorChar
        loadedFileRevision = selectedRevision
        updates = False
        changes = False
        btnUpdate.Enabled = False
        UpdateTapeToolStripMenuItem.Enabled = False
        Me.Text = fileName & " - C3"
        information.Rows.Find("Program Version")("Value") = VERSION
        information.Rows.Find("Program Stage")("Value") = VERSIONSTAGE
        information.Rows.Find("Program Date")("Value") = VERSIONDATE.ToString()
        consoleAdd("Opened and validated catalogue successfully.")
        loadData()
        Return True
    End Function

    Private Shared Sub replaceCatalogueData(destination As DataSet, source As DataSet)
        destination.EnforceConstraints = False
        destination.Clear()
        destination.Merge(source, False, MissingSchemaAction.Error)
        destination.EnforceConstraints = True
        destination.AcceptChanges()
    End Sub

    Private Sub updateMade()
        ' Made an update to a field in the main form.

        If updates = False And updatesMask = False Then

            updates = True

            ' Update title bar.
            Me.Text = fileName & "* - C3"

            ' Enable buttons.
            btnUpdate.Enabled = True
            UpdateTapeToolStripMenuItem.Enabled = True

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
        'numIndex.Maximum = tapeCount ' Update the maximum index that can be scrolled to.
        'numIndex.Value = thisTapeIndex + 1
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
        'txtNumber.Text = CStr(tape("Number"))

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

        'MsgBox("Compact Cassette Catalogue (C3)" & vbNewLine & "© " & COPYRIGHTAUTHOR & ", " & COPYRIGHTYEAR & vbNewLine & vbNewLine & "Program Version: " & VERSIONSTAGE & " " & VERSION & vbNewLine & "Catalogue Version: " & VERSIONFILE & vbNewLine & VERSIONDATE.ToLongDateString & ", " & VERSIONDATE.ToLongTimeString, MsgBoxStyle.Question, "About C3")

        frmAbout.Show()

    End Sub

    Private Sub SearchTapesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchTapesToolStripMenuItem.Click
        frmTapes.Show() 'temp
    End Sub

    Private Sub SearchModelsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchModelsToolStripMenuItem.Click
        frmModels.Show() 'temp
    End Sub

    Private Sub SearchManufacturersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchManufacturersToolStripMenuItem.Click
        frmBrands.Show() 'temp
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
        If resolvePendingTapeEdit("adding a new tape") Then
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

    Public Sub closeApplication()
        Me.Close()
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        addNewTape()

    End Sub

    Private Sub NewTapeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewTapeToolStripMenuItem.Click

        addNewTape()

    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        deleteTape(thisTapeIndex, False)

    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        saveChanges(False)

    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click

        saveChanges(False)

    End Sub

    Private Sub scrollActual(change As Integer, jump As Boolean)

        If jump = True Then

            thisTapeIndex = change ' New index is the given integer.
            displayTape()

        Else

            thisTapeIndex += change ' New index is incremented/decremented by the given integer.
            displayTape()

        End If

    End Sub

    Public Sub scrollTo(change As Integer, jump As Boolean)
        If Not resolvePendingTapeEdit("scrolling") Then
            Return
        End If

        scrollActual(change, jump)

        ' Reset updates variable and buttons.
        updates = False

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
        If closeApproved Then
            e.Cancel = False
            Return
        End If

        If Not resolvePendingTapeEdit("closing") OrElse Not resolveCatalogueChanges("closing") Then
            e.Cancel = True
            Return
        End If

        closeApproved = True
        e.Cancel = False
    End Sub

    Private Sub ShowConsoleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowConsoleToolStripMenuItem.Click

        frmConsole.Show()

    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        If resolvePendingTapeEdit("creating a new catalogue") Then
            newCatalogueCheckChanges()
        End If
    End Sub

    Private Sub newCatalogueCheckChanges()
        If resolveCatalogueChanges("creating a new catalogue") Then
            closeApproved = True
            updates = False
            changes = False
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

        deleteTape(thisTapeIndex, False)

    End Sub

    Private Sub OutputConsoleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OutputConsoleToolStripMenuItem.Click

        'Output the console to a log file and save it.
        'Write the string to a new file (date and time included in file name).

        Dim outputTime As DateTime = DateTime.Now
        Dim outputName As String = "console-output_" & outputTime.ToString("yyMMdd-HHmmss") & ".txt"
        Dim outputPath As String = fileDirectory & outputName
        Using outputFile As New StreamWriter(outputPath)

            'Write header.
            outputFile.WriteLine("Compact Cassette Catalogue (v" & CStr(VERSION) & ") Console Output at " & outputTime.ToString)
            outputFile.WriteLine("--------------------------------")

            'Write each line in the current console window.
            For Each line As String In frmConsole.lstConsole.Items
                outputFile.WriteLine(line)
            Next

        End Using

        'Show confirmation message
        Dim message As String = "Successfully output console to log file."
        Dim messageDetails As String = vbNewLine & vbNewLine & "File name: " & outputName & vbNewLine & "Full directory: " & outputPath
        If My.Settings.showMessages = True Then
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

        frmSettings.Show()

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
