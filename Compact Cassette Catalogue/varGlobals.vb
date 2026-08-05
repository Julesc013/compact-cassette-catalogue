' Module: Global Variables
' Purpose: To store all the globals, constants, and data-tables for the program.
' Author: Jules Carboni
' Date Created: 5 Sep 2019

Imports System.Text.RegularExpressions

Public Module varGlobals

    ' REMINDER: UPDATE PROG-VER, FILE-VER, AND SUPPOTED-VERS !!!
    ' About program information.
    Public Const VERSION As String = "1.3.0"
    Public Const VERSIONSTAGE As String = "Alpha 3"
    Public ReadOnly VERSIONDATE As DateTime = New DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Local) ' Y M D, h m s.
    ' About catalogue information.
    Public Const VERSIONFILE As String = "1.1.0" 'Add to top of XML
    Public ReadOnly VERSIONFILESUPPORTED As String() = {"1.1.0"}
    Public Const COPYRIGHTAUTHOR = "Jules Carboni"
    Public Const COPYRIGHTYEAR = "2019-2026"

    ' Hyperlinks
    Public Const CONTACTLABEL As String = "github.com/Julesc013" ' Contact label.
    Public Const CONTACTLINK As String = "https://github.com/Julesc013" ' Contact link.
    Public Const WEBSITEMAIN As String = "https://github.com/Julesc013/compact-cassette-catalogue" ' Main Website
    Public Const WEBSITEHELP As String = "https://github.com/Julesc013/compact-cassette-catalogue/wiki" ' Help/wiki Website
    Public Const UPDATELINKDOWNLOAD As String = "https://github.com/Julesc013/compact-cassette-catalogue/releases"  ' Github download page.
    ' The URL of the raw file in which the latest version information is stored.
    Public Const UPDATELINKCHECK As String = "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/VERSION" ' Raw Github file.
    Public Const FEEDBACKLINK As String = "https://github.com/Julesc013/compact-cassette-catalogue/issues/new/choose" ' Github issues page.


    ' Current file (and directory). If path is nothing, cannot save (must save-as).
    Public filePath As String = Nothing ' Includes file name.
    Public fileDirectory As String = Nothing ' Directory only.
    Public fileName As String = "New Catalogue" ' Name only.

    ' Has a change been made since last save?
    Public changes As Boolean = False
    Public updates As Boolean = False

    ' Time the program was loaded sucessfully.
    Public timeLoaded As String
    Public duringSetup As Boolean = False

    ' Define regular expressions.
    'Public regexAlphanumeric As Regex = New Regex("/^[a-z\d\-\s]+$/i")  'Alternatively: "/^[a-z0-9]+([-\s]{1}[a-z0-9]+)*$/i".
    'Public regexAlphabetic As Regex = New Regex("/^[a-z]*$/i")
    'Public regexAlphanumericBasic As Regex = New Regex("[^a-z0-9]") 'TEMP (doesnt work for hyphens).

    ' Create data set for catalogue.
    Public catalogue As DataSet = New DataSet("Catalogue")

    ' Create tables for data.
    Public information As DataTable = makeInformation() ' File and program versions and dates.
    Public counters As DataTable = makeCounters() ' Counters for amount of decks, brands, models, and tapes.
    Public decks As DataTable = makeDecks()
    Public brands As DataTable = makeBrands()
    Public models As DataTable = makeModels()
    Public tapes As DataTable = makeTapes()

    ' When adding tables, update 'add tables to dataset' section of frmMain.vb.

    ' Add references to counters.
    Public deckCount As Integer
    Public brandCount As Integer
    Public modelCount As Integer
    Public tapeCount As Integer

    Function makeInformation() As DataTable

        'Create table to store tapes
        Dim table As New DataTable

        'Create columns
        table.Columns.Add(New DataColumn("Information", GetType(String)))
        table.Columns.Add(New DataColumn("Value", GetType(String)))

        'Add information
        table.Rows.Add("File Version", VERSIONFILE)
        table.Rows.Add("Program Version", VERSION)
        table.Rows.Add("Program Stage", VERSIONSTAGE)
        table.Rows.Add("Program Date", VERSIONDATE.ToString)
        table.Rows.Add("File Created", DateTime.Now.ToString)
        table.Rows.Add("File Modified", DateTime.Now.ToString)
        table.Rows.Add("File Updated", DateTime.Now.ToString)

        'Set the Index column as the primary key column. (Necessary?)
        table.PrimaryKey = New DataColumn() {table.Columns(0)}
        'Rename datatable
        table.TableName = "Information"

        Return table

    End Function

    Function makeCounters() As DataTable

        'Note: These tables are not the primary source for counters (see above), they are only for saving to file.
        'Create table to store tapes
        Dim table As New DataTable

        'Create columns
        table.Columns.Add(New DataColumn("Counter", GetType(String)))
        table.Columns.Add(New DataColumn("Number", GetType(Integer)))

        table.Rows.Add("Decks", 0)
        table.Rows.Add("Brands", 0)
        table.Rows.Add("Models", 0)
        table.Rows.Add("Tapes", 0)

        'Set the Index column as the primary key column. (Necessary?)
        table.PrimaryKey = New DataColumn() {table.Columns(0)}
        'Rename datatable
        table.TableName = "Counters"

        Return table

    End Function

    Function makeDecks() As DataTable

        'Create table to store tapes
        Dim table As New DataTable

        'Create columns
        ''table.Columns.Add(New DataColumn("Index", GetType(Integer)))
        table.Columns.Add(New DataColumn("Manufacturer", GetType(String)))
        table.Columns.Add(New DataColumn("Model", GetType(String)))
        table.Columns.Add(New DataColumn("Name", GetType(String))) 'Manufacurer & Model superstring (Joined name)
        table.Columns.Add(New DataColumn("Year", GetType(Integer)))
        table.Columns.Add(New DataColumn("Condition", GetType(Integer)))
        table.Columns.Add(New DataColumn("Type1", GetType(Boolean)))
        table.Columns.Add(New DataColumn("Type2", GetType(Boolean)))
        table.Columns.Add(New DataColumn("Type3", GetType(Boolean)))
        table.Columns.Add(New DataColumn("Type4", GetType(Boolean)))
        table.Columns.Add(New DataColumn("HX", GetType(Boolean)))
        table.Columns.Add(New DataColumn("MPX", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DolbyB", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DolbyC", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DolbyS", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DBX1", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DBX2", GetType(Boolean)))
        table.Columns.Add(New DataColumn("Stereo", GetType(Boolean)))
        table.Columns.Add(New DataColumn("ProgramSearch", GetType(Boolean)))
        table.Columns.Add(New DataColumn("Reverse", GetType(Boolean)))
        table.Columns.Add(New DataColumn("Calibration", GetType(Boolean)))
        table.Columns.Add(New DataColumn("Azimuth", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DubbingSlow", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DubbingFast", GetType(Boolean)))
        table.Columns.Add(New DataColumn("FrequencyLow", GetType(Integer)))
        table.Columns.Add(New DataColumn("FrequencyHigh", GetType(Integer))) 'Convert from kHz to Hz
        table.Columns.Add(New DataColumn("SignalRatio", GetType(Integer)))
        table.Columns.Add(New DataColumn("SignalRatioNR", GetType(String)))
        table.Columns.Add(New DataColumn("WowFlutter", GetType(Decimal)))
        table.Columns.Add(New DataColumn("Distortion", GetType(Decimal)))
        table.Columns.Add(New DataColumn("Heads", GetType(Integer)))
        table.Columns.Add(New DataColumn("Wells", GetType(Integer)))
        table.Columns.Add(New DataColumn("SpeedSlow", GetType(Boolean)))
        table.Columns.Add(New DataColumn("SpeedNorm", GetType(Boolean)))
        table.Columns.Add(New DataColumn("SpeedFast", GetType(Boolean)))

        table.Columns.Add(New DataColumn("Date", GetType(DateTime))) 'Date and time the item was added
        table.Columns.Add(New DataColumn("Notes", GetType(String))) 'From notes multiline box

        ''table.Columns.Add(New DataColumn("Removed", GetType(Boolean))) 'Mark if this entry is not selectable

        'Set the Index column as the primary key column.
        table.PrimaryKey = New DataColumn() {table.Columns(2)}
        'Rename datatable
        table.TableName = "Decks"

        Return table

    End Function

    Function makeBrands() As DataTable

        'Create table to store tapes
        Dim table As New DataTable

        'Create columns
        ''table.Columns.Add(New DataColumn("Index", GetType(Integer)))
        table.Columns.Add(New DataColumn("Brand", GetType(String)))
        table.Columns.Add(New DataColumn("Code", GetType(String)))

        table.Columns.Add(New DataColumn("Date", GetType(DateTime))) 'Date and time the item was added
        table.Columns.Add(New DataColumn("Notes", GetType(String))) 'From notes multiline box

        ''table.Columns.Add(New DataColumn("Removed", GetType(Boolean))) 'Mark if this entry is not selectable

        'Set the Index column as the primary key column.
        table.PrimaryKey = New DataColumn() {table.Columns(1)}
        'Rename datatable
        table.TableName = "Brands"

        Return table

    End Function

    Function makeModels() As DataTable

        'Create table to store tapes
        Dim table As New DataTable

        'Create columns
        ''table.Columns.Add(New DataColumn("Index", GetType(Integer)))
        table.Columns.Add(New DataColumn("Brand", GetType(String)))
        table.Columns.Add(New DataColumn("Type", GetType(Integer))) 'Type code (1 to 4)
        table.Columns.Add(New DataColumn("Model", GetType(String))) 'Not-full name
        table.Columns.Add(New DataColumn("Code", GetType(String)))
        table.Columns.Add(New DataColumn("Identifier", GetType(String))) 'Brand & Type & Code superstring
        table.Columns.Add(New DataColumn("Name", GetType(String)))
        table.Columns.Add(New DataColumn("Number", GetType(Integer))) 'Number/counter of tapes with this model

        table.Columns.Add(New DataColumn("Date", GetType(DateTime))) 'Date and time the item was added
        table.Columns.Add(New DataColumn("Notes", GetType(String))) 'From notes multiline box

        ''table.Columns.Add(New DataColumn("Removed", GetType(Boolean))) 'Mark if this entry is not selectable

        'Set the Index column as the primary key column.
        table.PrimaryKey = New DataColumn() {table.Columns(4)}
        'Rename datatable
        table.TableName = "Models"

        Return table

    End Function

    Function makeTapes() As DataTable

        'Create table to store tapes
        Dim table As New DataTable

        'Create columns
        ''table.Columns.Add(New DataColumn("Index", GetType(Integer)))
        table.Columns.Add(New DataColumn("Model", GetType(String))) 'Model Identifier (code)
        table.Columns.Add(New DataColumn("Year", GetType(Integer)))
        table.Columns.Add(New DataColumn("Length", GetType(Decimal)))
        table.Columns.Add(New DataColumn("Region", GetType(String)))
        table.Columns.Add(New DataColumn("Number", GetType(Integer))) 'Index/counter within/per model
        table.Columns.Add(New DataColumn("Identifier", GetType(String))) 'Model-Identifier & Year & Length & Number superstring
        table.Columns.Add(New DataColumn("IdentifierShort", GetType(String)))
        table.Columns.Add(New DataColumn("Condition", GetType(Integer))) '8 point scale (8 is best, 1 is poor, 0 is broken?)
        table.Columns.Add(New DataColumn("Packaged", GetType(Boolean)))
        table.Columns.Add(New DataColumn("TapedA", GetType(Boolean)))
        table.Columns.Add(New DataColumn("TapedB", GetType(Boolean)))

        'For side A
        table.Columns.Add(New DataColumn("NameA", GetType(String)))
        table.Columns.Add(New DataColumn("RecordedA", GetType(Date)))
        table.Columns.Add(New DataColumn("DeckA", GetType(String)))
        table.Columns.Add(New DataColumn("InputA", GetType(String)))
        table.Columns.Add(New DataColumn("PeakA", GetType(Integer)))
        table.Columns.Add(New DataColumn("NRA", GetType(String)))
        table.Columns.Add(New DataColumn("HXA", GetType(Boolean)))
        table.Columns.Add(New DataColumn("MPXA", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DubbedA", GetType(Boolean)))
        table.Columns.Add(New DataColumn("SpeedA", GetType(String)))
        table.Columns.Add(New DataColumn("BiasA", GetType(Integer))) 'Index for type
        table.Columns.Add(New DataColumn("BiasCalA", GetType(Integer)))
        table.Columns.Add(New DataColumn("EQA", GetType(String))) '70 or 120
        table.Columns.Add(New DataColumn("LevelA", GetType(Decimal)))
        table.Columns.Add(New DataColumn("LevelCalA", GetType(Decimal)))
        table.Columns.Add(New DataColumn("ContentsA", GetType(String)))
        table.Columns.Add(New DataColumn("ArtistA", GetType(String)))
        table.Columns.Add(New DataColumn("TitleA", GetType(String)))

        'For side B
        table.Columns.Add(New DataColumn("NameB", GetType(String)))
        table.Columns.Add(New DataColumn("RecordedB", GetType(Date)))
        table.Columns.Add(New DataColumn("DeckB", GetType(String)))
        table.Columns.Add(New DataColumn("InputB", GetType(String)))
        table.Columns.Add(New DataColumn("PeakB", GetType(Integer)))
        table.Columns.Add(New DataColumn("NRB", GetType(String)))
        table.Columns.Add(New DataColumn("HXB", GetType(Boolean)))
        table.Columns.Add(New DataColumn("MPXB", GetType(Boolean)))
        table.Columns.Add(New DataColumn("DubbedB", GetType(Boolean)))
        table.Columns.Add(New DataColumn("SpeedB", GetType(String)))
        table.Columns.Add(New DataColumn("BiasB", GetType(Integer))) 'Index for type
        table.Columns.Add(New DataColumn("BiasCalB", GetType(Integer)))
        table.Columns.Add(New DataColumn("EQB", GetType(String))) '70 or 120
        table.Columns.Add(New DataColumn("LevelB", GetType(Decimal)))
        table.Columns.Add(New DataColumn("LevelCalB", GetType(Decimal)))
        table.Columns.Add(New DataColumn("ContentsB", GetType(String)))
        table.Columns.Add(New DataColumn("ArtistB", GetType(String)))
        table.Columns.Add(New DataColumn("TitleB", GetType(String)))

        table.Columns.Add(New DataColumn("Date", GetType(DateTime))) 'Date and time the item was added
        table.Columns.Add(New DataColumn("Notes", GetType(String))) 'From notes multiline box

        ''table.Columns.Add(New DataColumn("Removed", GetType(Boolean))) 'Mark if this entry is not selectable

        'Set the IdentifierShort column as the primary key column.
        table.PrimaryKey = New DataColumn() {table.Columns(6)}
        'Rename datatable
        table.TableName = "Tapes"

        Return table

    End Function

    Public Function IsBrandReferenced(modelRows As DataTable, brandName As String) As Boolean
        Return HasTextReference(modelRows, "Brand", brandName)
    End Function

    Public Function IsModelReferenced(tapeRows As DataTable, modelIdentifier As String) As Boolean
        Return HasTextReference(tapeRows, "Model", modelIdentifier)
    End Function

    Public Function IsDeckReferenced(tapeRows As DataTable, deckName As String) As Boolean
        Return HasTextReference(tapeRows, "DeckA", deckName) OrElse
            HasTextReference(tapeRows, "DeckB", deckName)
    End Function

    Private Function HasTextReference(table As DataTable, columnName As String, value As String) As Boolean
        If table Is Nothing Then
            Throw New ArgumentNullException("table")
        End If
        If Not table.Columns.Contains(columnName) Then
            Throw New ArgumentException("The table does not contain column '" & columnName & "'.", "columnName")
        End If

        For Each row As DataRow In table.Rows
            If row.RowState <> DataRowState.Deleted AndAlso
                    String.Equals(CStr(row(columnName)), value, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Sub RenameBrandReferences(modelRows As DataTable, oldBrandName As String, newBrandName As String)
        RenameTextReferences(modelRows, New String() {"Brand"}, oldBrandName, newBrandName)
    End Sub

    Public Sub RenameDeckReferences(tapeRows As DataTable, oldDeckName As String, newDeckName As String)
        RenameTextReferences(tapeRows, New String() {"DeckA", "DeckB"}, oldDeckName, newDeckName)
    End Sub

    Private Sub RenameTextReferences(table As DataTable, columnNames As String(), oldValue As String, newValue As String)
        If table Is Nothing Then
            Throw New ArgumentNullException("table")
        End If
        For Each columnName As String In columnNames
            If Not table.Columns.Contains(columnName) Then
                Throw New ArgumentException("The table does not contain column '" & columnName & "'.", "columnNames")
            End If
        Next

        Dim changedRows As New List(Of DataRow)()
        Dim changedColumns As New List(Of String)()
        Try
            For Each row As DataRow In table.Rows
                If row.RowState <> DataRowState.Deleted Then
                    For Each columnName As String In columnNames
                        If String.Equals(CStr(row(columnName)), oldValue, StringComparison.OrdinalIgnoreCase) Then
                            row(columnName) = newValue
                            changedRows.Add(row)
                            changedColumns.Add(columnName)
                        End If
                    Next
                End If
            Next
        Catch
            For index As Integer = changedRows.Count - 1 To 0 Step -1
                changedRows(index)(changedColumns(index)) = oldValue
            Next
            Throw
        End Try
    End Sub

    Public Sub SynchronizeEntityCounters(
            counterRows As DataTable,
            deckRows As DataTable,
            brandRows As DataTable,
            modelRows As DataTable,
            tapeRows As DataTable)

        SetCounterValue(counterRows, "Decks", ActiveRowCount(deckRows))
        SetCounterValue(counterRows, "Brands", ActiveRowCount(brandRows))
        SetCounterValue(counterRows, "Models", ActiveRowCount(modelRows))
        SetCounterValue(counterRows, "Tapes", ActiveRowCount(tapeRows))
    End Sub

    Private Function ActiveRowCount(table As DataTable) As Integer
        If table Is Nothing Then
            Throw New ArgumentNullException("table")
        End If

        Dim count As Integer = 0
        For Each row As DataRow In table.Rows
            If row.RowState <> DataRowState.Deleted Then
                count += 1
            End If
        Next
        Return count
    End Function

    Private Function FindCounterRow(counterRows As DataTable, counterName As String) As DataRow
        If counterRows Is Nothing Then
            Throw New ArgumentNullException("counterRows")
        End If
        If Not counterRows.Columns.Contains("Counter") OrElse Not counterRows.Columns.Contains("Number") Then
            Throw New ArgumentException("The counters table does not expose Counter and Number columns.", "counterRows")
        End If

        For Each row As DataRow In counterRows.Rows
            If row.RowState <> DataRowState.Deleted AndAlso
                    String.Equals(CStr(row("Counter")), counterName, StringComparison.Ordinal) Then
                Return row
            End If
        Next
        Throw New InvalidOperationException("Missing catalogue counter row '" & counterName & "'.")
    End Function

    Private Sub SetCounterValue(counterRows As DataTable, counterName As String, value As Integer)
        FindCounterRow(counterRows, counterName)("Number") = value
    End Sub

    Public Function NextTapeSequence(tapeRows As DataTable, modelIdentifier As String, storedNextSequence As Integer) As Integer
        If tapeRows Is Nothing Then
            Throw New ArgumentNullException("tapeRows")
        End If
        If storedNextSequence < 0 Then
            Throw New ArgumentOutOfRangeException("storedNextSequence")
        End If

        Dim nextSequence As Integer = storedNextSequence
        For Each row As DataRow In tapeRows.Rows
            If row.RowState <> DataRowState.Deleted AndAlso
                    String.Equals(CStr(row("Model")), modelIdentifier, StringComparison.Ordinal) Then
                nextSequence = Math.Max(nextSequence, checkedIncrement(CInt(row("Number"))))
            End If
        Next
        Return nextSequence
    End Function

    Private Function checkedIncrement(value As Integer) As Integer
        If value = Integer.MaxValue Then
            Throw New OverflowException("The tape sequence is exhausted.")
        End If
        Return value + 1
    End Function

    Public Sub CommitTapeBatch(
            tapeRows As DataTable,
            modelRow As DataRow,
            counterRows As DataTable,
            batchRows As IEnumerable(Of DataRow),
            nextSequenceExclusive As Integer)

        If tapeRows Is Nothing Then
            Throw New ArgumentNullException("tapeRows")
        End If
        If modelRow Is Nothing Then
            Throw New ArgumentNullException("modelRow")
        End If
        If batchRows Is Nothing Then
            Throw New ArgumentNullException("batchRows")
        End If

        Dim materializedRows As New List(Of DataRow)(batchRows)
        If materializedRows.Count = 0 Then
            Throw New ArgumentException("A tape batch cannot be empty.", "batchRows")
        End If

        Dim tapeCounterRow As DataRow = FindCounterRow(counterRows, "Tapes")
        Dim oldTapeCounter As Integer = CInt(tapeCounterRow("Number"))
        Dim oldModelSequence As Integer = CInt(modelRow("Number"))
        If nextSequenceExclusive < oldModelSequence Then
            Throw New InvalidOperationException("The tape sequence cannot move backwards.")
        End If

        For Each row As DataRow In materializedRows
            If row.Table IsNot tapeRows OrElse row.RowState <> DataRowState.Detached Then
                Throw New ArgumentException("Every batch row must be a detached row created by the target tape table.", "batchRows")
            End If
        Next

        Try
            For Each row As DataRow In materializedRows
                tapeRows.Rows.Add(row)
            Next
            tapeCounterRow("Number") = ActiveRowCount(tapeRows)
            modelRow("Number") = nextSequenceExclusive
        Catch
            For index As Integer = materializedRows.Count - 1 To 0 Step -1
                Dim row As DataRow = materializedRows(index)
                If row.Table Is tapeRows AndAlso row.RowState <> DataRowState.Detached Then
                    tapeRows.Rows.Remove(row)
                End If
            Next
            tapeCounterRow("Number") = oldTapeCounter
            modelRow("Number") = oldModelSequence
            Throw
        End Try
    End Sub

    Function getCondition(value As Integer) As Integer
        ' Convert selected index to condition score.

        Try

            Dim dictionary = New Dictionary(Of Integer, Integer) From {{0, 8}, {1, 7}, {2, 6}, {3, 5}, {4, 4}, {5, 3}, {6, 2}, {7, 1}, {8, 0}}

            Return dictionary.Item(value)

        Catch

            ' If not a valid index, return -1.
            Return -1

        End Try

    End Function

    Function getConditionWorded(value As Integer) As String
        ' Convert condition score to Good-Mint ranking.

        Dim dictionary = New Dictionary(Of Integer, String) From {{0, "Broken"}, {1, "Poor"}, {2, "Fair"}, {3, "Good"}, {4, "Good Plus"}, {5, "Very Good"}, {6, "Very Good Plus"}, {7, "Near Mint"}, {8, "Mint"}}

        Return dictionary.Item(value)

    End Function

    Function getTypeNumeral(value As Integer, worded As Boolean) As String
        ' Convert Arabic numeral to Roman numeral.

        Dim numerals = New Dictionary(Of Integer, String) From {{1, "I"}, {2, "II"}, {3, "III"}, {4, "IV"}}
        Dim names = New Dictionary(Of Integer, String) From {{1, "Ferric"}, {2, "Chrome"}, {3, "Ferrichrome"}, {4, "Metal"}}

        If worded = True Then
            Return numerals.Item(value) & " - " & names.Item(value)

        Else
            Return numerals.Item(value)

        End If

    End Function

    Sub consoleAdd(message As String)

        'Add line to console.
        Dim now As DateTime = DateTime.Now
        Dim stamp As String = "[" & consoleStamp(now) & "]"
        frmConsole.lstConsole.Items.Add(stamp & " " & message)

    End Sub

    Function consoleStamp(dateTime As DateTime) As String

        'Turn the provided time into a console-formatted time stamp.
        Return dateTime.ToString("dd/MM/yy HH:mm:ss")

    End Function

    Sub openWebLink(link As String)

        Try

            Process.Start(link)

        Catch ex As Exception

            Dim message As String = "Failed to open link."
            consoleAdd(message & " " & link & " Error: " & ex.Message)
            MsgBox(message & vbNewLine & vbNewLine & link & vbNewLine & vbNewLine & "Error: " & ex.Message, MsgBoxStyle.Exclamation, "Could Not Open Link")

        End Try

    End Sub

End Module
