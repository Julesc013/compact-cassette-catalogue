Imports C3.Catalogue.Decks
Imports System.Data

Namespace CatalogueFiles.Xml.V1_1

    Public NotInheritable Class LegacyDeckRepository
        Implements IDeckRepository

        Private ReadOnly _documentProvider As Func(Of DataSet)

        Public Sub New(documentProvider As Func(Of DataSet))
            If documentProvider Is Nothing Then
                Throw New ArgumentNullException("documentProvider")
            End If
            _documentProvider = documentProvider
        End Sub

        Public Function GetAll() As IList(Of Deck) Implements IDeckRepository.GetAll
            Dim values As New List(Of Deck)()
            For Each row As DataRow In DecksTable().Rows
                If row.RowState <> DataRowState.Deleted Then
                    values.Add(Map(row))
                End If
            Next
            Return values
        End Function

        Public Function FindByName(name As String) As Deck Implements IDeckRepository.FindByName
            Dim row As DataRow = FindRow(name)
            If row Is Nothing Then
                Return Nothing
            End If
            Return Map(row)
        End Function

        Public Function NameExists(name As String) As Boolean Implements IDeckRepository.NameExists
            Return FindRow(name) IsNot Nothing
        End Function

        Public Function IsReferencedByTape(name As String) As Boolean Implements IDeckRepository.IsReferencedByTape
            For Each row As DataRow In RequireTable("Tapes").Rows
                If row.RowState = DataRowState.Deleted Then
                    Continue For
                End If
                If String.Equals(ReadString(row, "DeckA"), name, StringComparison.OrdinalIgnoreCase) OrElse
                        String.Equals(ReadString(row, "DeckB"), name, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Sub Add(value As Deck) Implements IDeckRepository.Add
            Dim row As DataRow = DecksTable().NewRow()
            row("Name") = value.Name
            row("Date") = value.AddedAt
            WriteDetails(row, value.Details)
            DecksTable().Rows.Add(row)
            SynchronizeDeckCounter()
        End Sub

        Public Sub Update(value As Deck) Implements IDeckRepository.Update
            Dim row As DataRow = FindRow(value.Name)
            If row Is Nothing Then
                Throw New InvalidOperationException("The selected deck no longer exists.")
            End If
            WriteDetails(row, value.Details)
        End Sub

        Public Sub Delete(name As String) Implements IDeckRepository.Delete
            Dim row As DataRow = FindRow(name)
            If row Is Nothing Then
                Throw New InvalidOperationException("The selected deck no longer exists.")
            End If
            DecksTable().Rows.Remove(row)
            SynchronizeDeckCounter()
        End Sub

        Private Function Document() As DataSet
            Dim value As DataSet = _documentProvider()
            If value Is Nothing Then
                Throw New InvalidOperationException("No active catalogue document is available.")
            End If
            Return value
        End Function

        Private Function DecksTable() As DataTable
            Return RequireTable("Decks")
        End Function

        Private Function RequireTable(name As String) As DataTable
            Dim table As DataTable = Document().Tables(name)
            If table Is Nothing Then
                Throw New InvalidOperationException("Catalogue table '" & name & "' is missing.")
            End If
            Return table
        End Function

        Private Function FindRow(name As String) As DataRow
            If String.IsNullOrWhiteSpace(name) Then
                Return Nothing
            End If
            For Each row As DataRow In DecksTable().Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(ReadString(row, "Name"), name, StringComparison.OrdinalIgnoreCase) Then
                    Return row
                End If
            Next
            Return Nothing
        End Function

        Private Shared Function Map(row As DataRow) As Deck
            Dim details As New DeckDetails(
                ReadString(row, "Manufacturer"),
                ReadString(row, "Model"),
                ReadInteger(row, "Year"),
                ReadInteger(row, "Condition"),
                ReadBoolean(row, "Type1"),
                ReadBoolean(row, "Type2"),
                ReadBoolean(row, "Type3"),
                ReadBoolean(row, "Type4"),
                ReadBoolean(row, "HX"),
                ReadBoolean(row, "MPX"),
                ReadBoolean(row, "DolbyB"),
                ReadBoolean(row, "DolbyC"),
                ReadBoolean(row, "DolbyS"),
                ReadBoolean(row, "DBX1"),
                ReadBoolean(row, "DBX2"),
                ReadBoolean(row, "Stereo"),
                ReadBoolean(row, "ProgramSearch"),
                ReadBoolean(row, "Reverse"),
                ReadBoolean(row, "Calibration"),
                ReadBoolean(row, "Azimuth"),
                ReadBoolean(row, "DubbingSlow"),
                ReadBoolean(row, "DubbingFast"),
                ReadInteger(row, "FrequencyLow"),
                ReadInteger(row, "FrequencyHigh"),
                ReadInteger(row, "SignalRatio"),
                ReadString(row, "SignalRatioNR"),
                ReadDecimal(row, "WowFlutter"),
                ReadDecimal(row, "Distortion"),
                ReadInteger(row, "Heads"),
                ReadInteger(row, "Wells"),
                ReadBoolean(row, "SpeedSlow"),
                ReadBoolean(row, "SpeedNorm"),
                ReadBoolean(row, "SpeedFast"),
                ReadString(row, "Notes"))
            Return New Deck(ReadString(row, "Name"), ReadDate(row, "Date"), details)
        End Function

        Private Shared Sub WriteDetails(row As DataRow, value As DeckDetails)
            row("Manufacturer") = value.Manufacturer
            row("Model") = value.Model
            row("Year") = value.Year
            row("Condition") = value.Condition
            row("Type1") = value.Type1
            row("Type2") = value.Type2
            row("Type3") = value.Type3
            row("Type4") = value.Type4
            row("HX") = value.Hx
            row("MPX") = value.Mpx
            row("DolbyB") = value.DolbyB
            row("DolbyC") = value.DolbyC
            row("DolbyS") = value.DolbyS
            row("DBX1") = value.Dbx1
            row("DBX2") = value.Dbx2
            row("Stereo") = value.Stereo
            row("ProgramSearch") = value.ProgramSearch
            row("Reverse") = value.Reverse
            row("Calibration") = value.Calibration
            row("Azimuth") = value.Azimuth
            row("DubbingSlow") = value.DubbingSlow
            row("DubbingFast") = value.DubbingFast
            row("FrequencyLow") = value.FrequencyLow
            row("FrequencyHigh") = value.FrequencyHigh
            row("SignalRatio") = value.SignalRatio
            row("SignalRatioNR") = value.SignalRatioNoiseReduction
            row("WowFlutter") = value.WowFlutter
            row("Distortion") = value.Distortion
            row("Heads") = value.Heads
            row("Wells") = value.Wells
            row("SpeedSlow") = value.SpeedSlow
            row("SpeedNorm") = value.SpeedNormal
            row("SpeedFast") = value.SpeedFast
            row("Notes") = value.Notes
        End Sub

        Private Shared Function ReadString(row As DataRow, name As String) As String
            Return If(row.IsNull(name), String.Empty, Convert.ToString(row(name)))
        End Function

        Private Shared Function ReadInteger(row As DataRow, name As String) As Integer
            Return If(row.IsNull(name), 0, Convert.ToInt32(row(name)))
        End Function

        Private Shared Function ReadBoolean(row As DataRow, name As String) As Boolean
            Return Not row.IsNull(name) AndAlso Convert.ToBoolean(row(name))
        End Function

        Private Shared Function ReadDecimal(row As DataRow, name As String) As Decimal
            Return If(row.IsNull(name), Decimal.Zero, Convert.ToDecimal(row(name)))
        End Function

        Private Shared Function ReadDate(row As DataRow, name As String) As DateTime
            Return If(row.IsNull(name), DateTime.MinValue, Convert.ToDateTime(row(name)))
        End Function

        Private Sub SynchronizeDeckCounter()
            Dim counter As DataRow = RequireTable("Counters").Rows.Find("Decks")
            If counter IsNot Nothing Then
                counter("Number") = DecksTable().Rows.Count
            End If
        End Sub

    End Class

End Namespace
