Imports C3.Catalogue.Tapes
Imports System.Data

Namespace CatalogueFiles.Xml.V1_1

    Public NotInheritable Class LegacyTapeRepository
        Implements ITapeRepository

        Private ReadOnly _documentProvider As Func(Of DataSet)

        Public Sub New(documentProvider As Func(Of DataSet))
            If documentProvider Is Nothing Then
                Throw New ArgumentNullException("documentProvider")
            End If
            _documentProvider = documentProvider
        End Sub

        Public Function GetAll() As IList(Of Tape) Implements ITapeRepository.GetAll
            Dim values As New List(Of Tape)()
            For Each row As DataRow In TapesTable().Rows
                If row.RowState <> DataRowState.Deleted Then
                    values.Add(Map(row))
                End If
            Next
            Return values
        End Function

        Public Function FindByShortIdentifier(identifier As String) As Tape _
                Implements ITapeRepository.FindByShortIdentifier

            Dim row As DataRow = FindRow(identifier)
            Return If(row Is Nothing, Nothing, Map(row))
        End Function

        Public Function ModelExists(identifier As String) As Boolean Implements ITapeRepository.ModelExists
            Return FindModelRow(identifier) IsNot Nothing
        End Function

        Public Function NextNumberForModel(identifier As String) As Integer _
                Implements ITapeRepository.NextNumberForModel

            Dim maximum As Integer = -1
            For Each row As DataRow In TapesTable().Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(ReadString(row, "Model"), identifier, StringComparison.OrdinalIgnoreCase) Then
                    maximum = Math.Max(maximum, ReadInteger(row, "Number"))
                End If
            Next
            Return maximum + 1
        End Function

        Public Function IdentifierExists(identifier As String, shortIdentifier As String) As Boolean _
                Implements ITapeRepository.IdentifierExists

            For Each row As DataRow In TapesTable().Rows
                If row.RowState = DataRowState.Deleted Then
                    Continue For
                End If
                If String.Equals(ReadString(row, "Identifier"), identifier, StringComparison.OrdinalIgnoreCase) OrElse
                        String.Equals(
                            ReadString(row, "IdentifierShort"),
                            shortIdentifier,
                            StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Sub AddRange(values As IList(Of Tape)) Implements ITapeRepository.AddRange
            If values Is Nothing Then
                Throw New ArgumentNullException("values")
            End If

            Dim addedRows As New List(Of DataRow)()
            Dim affectedModels As New List(Of String)()
            Try
                For Each value As Tape In values
                    Dim row As DataRow = TapesTable().NewRow()
                    Write(row, value, True)
                    TapesTable().Rows.Add(row)
                    addedRows.Add(row)
                    If Not affectedModels.Contains(value.ModelIdentifier) Then
                        affectedModels.Add(value.ModelIdentifier)
                    End If
                Next
                SynchronizeCounts(affectedModels)
            Catch
                For index As Integer = addedRows.Count - 1 To 0 Step -1
                    If addedRows(index).Table IsNot Nothing Then
                        TapesTable().Rows.Remove(addedRows(index))
                    End If
                Next
                SynchronizeCounts(affectedModels)
                Throw
            End Try
        End Sub

        Public Sub Update(value As Tape) Implements ITapeRepository.Update
            Dim row As DataRow = FindRow(value.ShortIdentifier)
            If row Is Nothing Then
                Throw New InvalidOperationException("The selected tape no longer exists.")
            End If
            Write(row, value, False)
        End Sub

        Public Sub Delete(shortIdentifier As String) Implements ITapeRepository.Delete
            Dim row As DataRow = FindRow(shortIdentifier)
            If row Is Nothing Then
                Throw New InvalidOperationException("The selected tape no longer exists.")
            End If
            Dim modelIdentifier As String = ReadString(row, "Model")
            TapesTable().Rows.Remove(row)
            SynchronizeCounts(New List(Of String) From {modelIdentifier})
        End Sub

        Private Function Document() As DataSet
            Dim value As DataSet = _documentProvider()
            If value Is Nothing Then
                Throw New InvalidOperationException("No active catalogue document is available.")
            End If
            Return value
        End Function

        Private Function TapesTable() As DataTable
            Return RequireTable("Tapes")
        End Function

        Private Function RequireTable(name As String) As DataTable
            Dim table As DataTable = Document().Tables(name)
            If table Is Nothing Then
                Throw New InvalidOperationException("Catalogue table '" & name & "' is missing.")
            End If
            Return table
        End Function

        Private Function FindRow(identifier As String) As DataRow
            If String.IsNullOrWhiteSpace(identifier) Then
                Return Nothing
            End If
            For Each row As DataRow In TapesTable().Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(
                            ReadString(row, "IdentifierShort"),
                            identifier,
                            StringComparison.OrdinalIgnoreCase) Then
                    Return row
                End If
            Next
            Return Nothing
        End Function

        Private Function FindModelRow(identifier As String) As DataRow
            For Each row As DataRow In RequireTable("Models").Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(
                            ReadString(row, "Identifier"),
                            identifier,
                            StringComparison.OrdinalIgnoreCase) Then
                    Return row
                End If
            Next
            Return Nothing
        End Function

        Private Shared Function Map(row As DataRow) As Tape
            Return New Tape(
                ReadString(row, "Model"),
                ReadInteger(row, "Year"),
                ReadDecimal(row, "Length"),
                ReadString(row, "Region"),
                ReadInteger(row, "Number"),
                ReadString(row, "Identifier"),
                ReadString(row, "IdentifierShort"),
                ReadInteger(row, "Condition"),
                ReadBoolean(row, "Packaged"),
                MapSide(row, "A", ReadBoolean(row, "TapedA")),
                MapSide(row, "B", ReadBoolean(row, "TapedB")),
                ReadDate(row, "Date"),
                ReadString(row, "Notes"))
        End Function

        Private Shared Function MapSide(row As DataRow, suffix As String, isRecorded As Boolean) As TapeSide
            Return New TapeSide(
                isRecorded,
                ReadString(row, "Name" & suffix),
                ReadDate(row, "Recorded" & suffix),
                ReadString(row, "Deck" & suffix),
                ReadString(row, "Input" & suffix),
                ReadInteger(row, "Peak" & suffix),
                ReadString(row, "NR" & suffix),
                ReadBoolean(row, "HX" & suffix),
                ReadBoolean(row, "MPX" & suffix),
                ReadBoolean(row, "Dubbed" & suffix),
                ReadString(row, "Speed" & suffix),
                ReadInteger(row, "Bias" & suffix),
                ReadInteger(row, "BiasCal" & suffix),
                ReadString(row, "EQ" & suffix),
                ReadDecimal(row, "Level" & suffix),
                ReadDecimal(row, "LevelCal" & suffix),
                ReadString(row, "Contents" & suffix),
                ReadString(row, "Artist" & suffix),
                ReadString(row, "Title" & suffix))
        End Function

        Private Shared Sub Write(row As DataRow, value As Tape, includeIdentity As Boolean)
            If includeIdentity Then
                row("Model") = value.ModelIdentifier
                row("Number") = value.Number
                row("IdentifierShort") = value.ShortIdentifier
                row("Date") = value.AddedAt
            End If
            row("Identifier") = value.Identifier
            row("Year") = value.Year
            row("Length") = value.LengthMinutes
            row("Region") = value.Region
            row("Condition") = value.Condition
            row("Packaged") = value.Packaged
            row("TapedA") = value.SideA.IsRecorded
            row("TapedB") = value.SideB.IsRecorded
            WriteSide(row, "A", value.SideA)
            WriteSide(row, "B", value.SideB)
            row("Notes") = value.Notes
        End Sub

        Private Shared Sub WriteSide(row As DataRow, suffix As String, value As TapeSide)
            row("Name" & suffix) = value.Name
            row("Recorded" & suffix) = value.RecordedAt
            row("Deck" & suffix) = value.DeckName
            row("Input" & suffix) = value.InputName
            row("Peak" & suffix) = value.PeakLevel
            row("NR" & suffix) = value.NoiseReduction
            row("HX" & suffix) = value.Hx
            row("MPX" & suffix) = value.Mpx
            row("Dubbed" & suffix) = value.Dubbed
            row("Speed" & suffix) = value.Speed
            row("Bias" & suffix) = value.Bias
            row("BiasCal" & suffix) = value.BiasCalibration
            row("EQ" & suffix) = value.Equalization
            row("Level" & suffix) = value.Level
            row("LevelCal" & suffix) = value.LevelCalibration
            row("Contents" & suffix) = value.Contents
            row("Artist" & suffix) = value.Artist
            row("Title" & suffix) = value.Title
        End Sub

        Private Sub SynchronizeCounts(modelIdentifiers As IList(Of String))
            Dim counter As DataRow = RequireTable("Counters").Rows.Find("Tapes")
            If counter IsNot Nothing Then
                counter("Number") = TapesTable().Rows.Count
            End If
            For Each identifier As String In modelIdentifiers
                Dim model As DataRow = FindModelRow(identifier)
                If model Is Nothing Then
                    Continue For
                End If
                Dim count As Integer = 0
                For Each tape As DataRow In TapesTable().Rows
                    If tape.RowState <> DataRowState.Deleted AndAlso
                            String.Equals(ReadString(tape, "Model"), identifier, StringComparison.OrdinalIgnoreCase) Then
                        count += 1
                    End If
                Next
                model("Number") = count
            Next
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

    End Class

End Namespace
