Imports System.Data

Namespace CatalogueFiles.Xml.V1_1

    Public NotInheritable Class LegacyCatalogueMetadata

        Public Property FileVersion As String
        Public Property ProductVersion As String
        Public Property ProductStage As String
        Public Property ProductDate As DateTime
        Public Property CreatedAt As DateTime

    End Class

    Public NotInheritable Class LegacyCatalogueSchema

        Private Sub New()
        End Sub

        Public Shared Function Create(metadata As LegacyCatalogueMetadata) As DataSet
            If metadata Is Nothing Then
                Throw New ArgumentNullException("metadata")
            End If

            Dim document As New DataSet("Catalogue")
            document.Tables.Add(CreateInformation(metadata))
            document.Tables.Add(CreateCounters())
            document.Tables.Add(CreateDecks())
            document.Tables.Add(CreateBrands())
            document.Tables.Add(CreateModels())
            document.Tables.Add(CreateTapes())
            Return document
        End Function

        Private Shared Function CreateInformation(metadata As LegacyCatalogueMetadata) As DataTable
            Dim table As New DataTable("Information")
            table.Columns.Add("Information", GetType(String))
            table.Columns.Add("Value", GetType(String))
            table.Rows.Add("File Version", metadata.FileVersion)
            table.Rows.Add("Program Version", metadata.ProductVersion)
            table.Rows.Add("Program Stage", metadata.ProductStage)
            table.Rows.Add("Program Date", metadata.ProductDate.ToString())
            table.Rows.Add("File Created", metadata.CreatedAt.ToString())
            table.Rows.Add("File Modified", metadata.CreatedAt.ToString())
            table.Rows.Add("File Updated", metadata.CreatedAt.ToString())
            table.PrimaryKey = {table.Columns("Information")}
            Return table
        End Function

        Private Shared Function CreateCounters() As DataTable
            Dim table As New DataTable("Counters")
            table.Columns.Add("Counter", GetType(String))
            table.Columns.Add("Number", GetType(Integer))
            table.Rows.Add("Decks", 0)
            table.Rows.Add("Brands", 0)
            table.Rows.Add("Models", 0)
            table.Rows.Add("Tapes", 0)
            table.PrimaryKey = {table.Columns("Counter")}
            Return table
        End Function

        Private Shared Function CreateDecks() As DataTable
            Dim table As New DataTable("Decks")
            AddColumn(table, "Manufacturer", GetType(String))
            AddColumn(table, "Model", GetType(String))
            AddColumn(table, "Name", GetType(String))
            AddColumn(table, "Year", GetType(Integer))
            AddColumn(table, "Condition", GetType(Integer))
            AddColumn(table, "Type1", GetType(Boolean))
            AddColumn(table, "Type2", GetType(Boolean))
            AddColumn(table, "Type3", GetType(Boolean))
            AddColumn(table, "Type4", GetType(Boolean))
            AddColumn(table, "HX", GetType(Boolean))
            AddColumn(table, "MPX", GetType(Boolean))
            AddColumn(table, "DolbyB", GetType(Boolean))
            AddColumn(table, "DolbyC", GetType(Boolean))
            AddColumn(table, "DolbyS", GetType(Boolean))
            AddColumn(table, "DBX1", GetType(Boolean))
            AddColumn(table, "DBX2", GetType(Boolean))
            AddColumn(table, "Stereo", GetType(Boolean))
            AddColumn(table, "ProgramSearch", GetType(Boolean))
            AddColumn(table, "Reverse", GetType(Boolean))
            AddColumn(table, "Calibration", GetType(Boolean))
            AddColumn(table, "Azimuth", GetType(Boolean))
            AddColumn(table, "DubbingSlow", GetType(Boolean))
            AddColumn(table, "DubbingFast", GetType(Boolean))
            AddColumn(table, "FrequencyLow", GetType(Integer))
            AddColumn(table, "FrequencyHigh", GetType(Integer))
            AddColumn(table, "SignalRatio", GetType(Integer))
            AddColumn(table, "SignalRatioNR", GetType(String))
            AddColumn(table, "WowFlutter", GetType(Decimal))
            AddColumn(table, "Distortion", GetType(Decimal))
            AddColumn(table, "Heads", GetType(Integer))
            AddColumn(table, "Wells", GetType(Integer))
            AddColumn(table, "SpeedSlow", GetType(Boolean))
            AddColumn(table, "SpeedNorm", GetType(Boolean))
            AddColumn(table, "SpeedFast", GetType(Boolean))
            AddColumn(table, "Date", GetType(DateTime))
            AddColumn(table, "Notes", GetType(String))
            table.PrimaryKey = {table.Columns("Name")}
            Return table
        End Function

        Private Shared Function CreateBrands() As DataTable
            Dim table As New DataTable("Brands")
            AddColumn(table, "Brand", GetType(String))
            AddColumn(table, "Code", GetType(String))
            AddColumn(table, "Date", GetType(DateTime))
            AddColumn(table, "Notes", GetType(String))
            table.PrimaryKey = {table.Columns("Code")}
            Return table
        End Function

        Private Shared Function CreateModels() As DataTable
            Dim table As New DataTable("Models")
            AddColumn(table, "Brand", GetType(String))
            AddColumn(table, "Type", GetType(Integer))
            AddColumn(table, "Model", GetType(String))
            AddColumn(table, "Code", GetType(String))
            AddColumn(table, "Identifier", GetType(String))
            AddColumn(table, "Name", GetType(String))
            AddColumn(table, "Number", GetType(Integer))
            AddColumn(table, "Date", GetType(DateTime))
            AddColumn(table, "Notes", GetType(String))
            table.PrimaryKey = {table.Columns("Identifier")}
            Return table
        End Function

        Private Shared Function CreateTapes() As DataTable
            Dim table As New DataTable("Tapes")
            AddColumn(table, "Model", GetType(String))
            AddColumn(table, "Year", GetType(Integer))
            AddColumn(table, "Length", GetType(Decimal))
            AddColumn(table, "Region", GetType(String))
            AddColumn(table, "Number", GetType(Integer))
            AddColumn(table, "Identifier", GetType(String))
            AddColumn(table, "IdentifierShort", GetType(String))
            AddColumn(table, "Condition", GetType(Integer))
            AddColumn(table, "Packaged", GetType(Boolean))
            AddColumn(table, "TapedA", GetType(Boolean))
            AddColumn(table, "TapedB", GetType(Boolean))
            AddTapeSideColumns(table, "A")
            AddTapeSideColumns(table, "B")
            AddColumn(table, "Date", GetType(DateTime))
            AddColumn(table, "Notes", GetType(String))
            table.PrimaryKey = {table.Columns("IdentifierShort")}
            Return table
        End Function

        Private Shared Sub AddTapeSideColumns(table As DataTable, side As String)
            AddColumn(table, "Name" & side, GetType(String))
            AddColumn(table, "Recorded" & side, GetType(DateTime))
            AddColumn(table, "Deck" & side, GetType(String))
            AddColumn(table, "Input" & side, GetType(String))
            AddColumn(table, "Peak" & side, GetType(Integer))
            AddColumn(table, "NR" & side, GetType(String))
            AddColumn(table, "HX" & side, GetType(Boolean))
            AddColumn(table, "MPX" & side, GetType(Boolean))
            AddColumn(table, "Dubbed" & side, GetType(Boolean))
            AddColumn(table, "Speed" & side, GetType(String))
            AddColumn(table, "Bias" & side, GetType(Integer))
            AddColumn(table, "BiasCal" & side, GetType(Integer))
            AddColumn(table, "EQ" & side, GetType(String))
            AddColumn(table, "Level" & side, GetType(Decimal))
            AddColumn(table, "LevelCal" & side, GetType(Decimal))
            AddColumn(table, "Contents" & side, GetType(String))
            AddColumn(table, "Artist" & side, GetType(String))
            AddColumn(table, "Title" & side, GetType(String))
        End Sub

        Private Shared Sub AddColumn(table As DataTable, name As String, dataType As Type)
            table.Columns.Add(New DataColumn(name, dataType))
        End Sub

    End Class

End Namespace

