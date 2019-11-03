﻿' Module: Global Variables
' Purpose: To store all the globals, constants, and data-tables for the program.
' Author: Jules Carboni
' Date Created: 5 Sep 2019

Imports System.Text.RegularExpressions

Module varGlobals

    'REMINDER: UPDATE PROG-VER, FILE-VER, AND SUPPOTED-VERS !!!
    'About program information
    Public Const VERSION As String = "0.6.0b1"
    Public Const VERSIONSTAGE As String = "Beta"
    Public ReadOnly VERSIONDATE As DateTime = New DateTime(2019, 11, 3, 11, 30, 0, DateTimeKind.Local) 'Y M D, h m s
    'About catalogue information
    Public Const VERSIONFILE As String = "1.1.0" 'Add to top of XML
    Public ReadOnly VERSIONFILESUPPORTED As String() = {"1.1.0"}

    'Current file (and directory)
    Public filePath As String = Nothing 'If nothing, cannot save (must save-as)
    Public fileName As String = "New Catalogue"

    'Has a change been made since last save?
    Public changes As Boolean = False
    Public updates As Boolean = False

    'Time the program was loaded sucessfully
    Public timeLoaded As String

    'Define regular expressions
    Public regexAlphanumeric As Regex = New Regex("/^[a-z\d\-\s]+$/i") 'Alternatively: "/^[a-z0-9]+([-\s]{1}[a-z0-9]+)*$/i"
    Public regexAlphabetic As Regex = New Regex("/^[a-z]*$/i")

    'Create data set for catalogue
    Public catalogue As DataSet = New DataSet("Catalogue")

    'Create tables for data
    Public information As DataTable = makeInformation() 'File and program versions and dates
    Public counters As DataTable = makeCounters() 'Counters for amount of decks, brands, models, and tapes
    Public decks As DataTable = makeDecks()
    Public brands As DataTable = makeBrands()
    Public models As DataTable = makeModels()
    Public tapes As DataTable = makeTapes()

    'When adding tables, update add tables section of frmMain.vb

    'Add references to counters
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
        table.Columns.Add(New DataColumn("Name", GetType(String))) 'Manufacurer & Model superstring (Full name)
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
        table.Columns.Add(New DataColumn("Model", GetType(String)))
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
        table.Columns.Add(New DataColumn("Model", GetType(String))) 'Model Identifier
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

    Function getCondition(value As Integer) As Integer
        'Convert selected index to condition score.

        Dim dictionary = New Dictionary(Of Integer, Integer) From {{0, 8}, {1, 7}, {2, 6}, {3, 5}, {4, 4}, {5, 3}, {6, 2}, {7, 1}, {8, 0}}

        Return dictionary.Item(value)

    End Function

    'Function getBrandCode(brand As String) As String

    '    Dim brandRow As DataRow = brands.Select("Brand='" & brand & "'")(0)
    '    Return CStr(brandRow("Code"))

    'End Function

    'Function getCounter(counter As String) As Integer

    '    Dim row As DataRow = counters.Select("Counter = '" & counter & "'").FirstOrDefault()

    '    If Not row Is Nothing Then
    '        Return CInt(row.Item("Value"))
    '    End If

    'End Function

    Sub consoleAdd(message As String)

        Dim now As DateTime = DateTime.Now
        Dim stamp As String = now.ToString("dd/MM/yy HH:mm:ss")

        frmConsole.lstConsole.Items.Add("[" & stamp & "] " & message)

    End Sub

End Module
