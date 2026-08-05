Imports Compact_Cassette_Catalogue

Module Program

    Private _failures As Integer
    Private _repositoryRoot As String

    Sub Main()
        _repositoryRoot = FindRepositoryRoot()

        RunTest("referenced entities are detected", AddressOf ReferencedEntitiesAreDetected)
        RunTest("display-name references rename atomically", AddressOf DisplayNameReferencesRenameAtomically)
        RunTest("entity counters derive from rows", AddressOf EntityCountersDeriveFromRows)
        RunTest("bulk tape commit rolls back completely", AddressOf BulkTapeCommitRollsBackCompletely)
        RunTest("tape sequence never reuses an allocated value", AddressOf TapeSequenceNeverReusesAllocatedValue)
        RunTest("integrity source corrections remain present", AddressOf IntegritySourceCorrectionsRemainPresent)

        If _failures > 0 Then
            Console.Error.WriteLine("{0} integrity characterization test(s) failed.", _failures)
            Environment.ExitCode = 1
            Return
        End If

        Console.WriteLine("All C3 catalogue integrity characterization tests passed.")
    End Sub

    Private Sub RunTest(name As String, test As Action)
        Try
            test()
            Console.WriteLine("PASS: " & name)
        Catch ex As Exception
            _failures += 1
            Console.Error.WriteLine("FAIL: {0}{1}{2}", name, Environment.NewLine, ex.ToString())
        End Try
    End Sub

    Private Sub ReferencedEntitiesAreDetected()
        Dim modelRows As DataTable = NewTable("Models", "Brand", "Identifier")
        modelRows.Rows.Add("Maxell", "MAX2XL")
        Dim tapeRows As DataTable = NewTable("Tapes", "Model", "DeckA", "DeckB")
        tapeRows.Rows.Add("MAX2XL", "Nakamichi Dragon", "")

        AssertTrue(varGlobals.IsBrandReferenced(modelRows, "Maxell"), "brand reference")
        AssertTrue(varGlobals.IsModelReferenced(tapeRows, "MAX2XL"), "model reference")
        AssertTrue(varGlobals.IsDeckReferenced(tapeRows, "Nakamichi Dragon"), "deck reference")
        AssertFalse(varGlobals.IsBrandReferenced(modelRows, "TDK"), "unreferenced brand")
        AssertFalse(varGlobals.IsDeckReferenced(tapeRows, "Revox B215"), "unreferenced deck")
    End Sub

    Private Sub DisplayNameReferencesRenameAtomically()
        Dim modelRows As DataTable = NewTable("Models", "Brand", "Identifier")
        modelRows.Rows.Add("National", "NAT1LN")
        modelRows.Rows.Add("TDK", "TDK2SA")
        Dim tapeRows As DataTable = NewTable("Tapes", "Model", "DeckA", "DeckB")
        tapeRows.Rows.Add("NAT1LN", "National RS-263", "TDK DA-3826")
        tapeRows.Rows.Add("TDK2SA", "TDK DA-3826", "National RS-263")

        varGlobals.RenameBrandReferences(modelRows, "National", "Panasonic")
        varGlobals.RenameDeckReferences(tapeRows, "National RS-263", "Panasonic RS-263")

        AssertEqual("Panasonic", CStr(modelRows.Rows(0)("Brand")), "renamed historical brand")
        AssertEqual("TDK", CStr(modelRows.Rows(1)("Brand")), "unrelated brand")
        AssertEqual("Panasonic RS-263", CStr(tapeRows.Rows(0)("DeckA")), "renamed side A deck")
        AssertEqual("Panasonic RS-263", CStr(tapeRows.Rows(1)("DeckB")), "renamed side B deck")
    End Sub

    Private Sub EntityCountersDeriveFromRows()
        Dim counterRows As DataTable = NewTable("Counters", "Counter", "Number")
        counterRows.Columns("Number").DataType = GetType(Integer)
        counterRows.Rows.Add("Decks", 99)
        counterRows.Rows.Add("Brands", 99)
        counterRows.Rows.Add("Models", 99)
        counterRows.Rows.Add("Tapes", 99)

        varGlobals.SynchronizeEntityCounters(
            counterRows,
            Rows(2),
            Rows(3),
            Rows(4),
            Rows(5))

        AssertEqual(2, CInt(counterRows.Rows(0)("Number")), "deck counter")
        AssertEqual(3, CInt(counterRows.Rows(1)("Number")), "brand counter")
        AssertEqual(4, CInt(counterRows.Rows(2)("Number")), "model counter")
        AssertEqual(5, CInt(counterRows.Rows(3)("Number")), "tape counter")
    End Sub

    Private Sub BulkTapeCommitRollsBackCompletely()
        Dim tapeRows As DataTable = NewTable("Tapes", "IdentifierShort", "Model", "Number")
        tapeRows.Columns("IdentifierShort").Unique = True
        tapeRows.Columns("Number").DataType = GetType(Integer)
        Dim modelRows As DataTable = NewTable("Models", "Identifier", "Number")
        modelRows.Columns("Number").DataType = GetType(Integer)
        modelRows.Rows.Add("MAX2XL", 7)
        Dim counterRows As DataTable = NewTable("Counters", "Counter", "Number")
        counterRows.Columns("Number").DataType = GetType(Integer)
        counterRows.Rows.Add("Decks", 0)
        counterRows.Rows.Add("Brands", 0)
        counterRows.Rows.Add("Models", 1)
        counterRows.Rows.Add("Tapes", 0)

        Dim first As DataRow = tapeRows.NewRow()
        first("IdentifierShort") = "MAX2XL007"
        first("Model") = "MAX2XL"
        first("Number") = 7
        Dim duplicate As DataRow = tapeRows.NewRow()
        duplicate("IdentifierShort") = "MAX2XL007"
        duplicate("Model") = "MAX2XL"
        duplicate("Number") = 8

        AssertThrows(
            Sub()
                varGlobals.CommitTapeBatch(
                    tapeRows,
                    modelRows.Rows(0),
                    counterRows,
                    New DataRow() {first, duplicate},
                    9)
            End Sub,
            "duplicate batch")

        AssertEqual(0, tapeRows.Rows.Count, "rolled-back rows")
        AssertEqual(0, CInt(counterRows.Rows(3)("Number")), "rolled-back tape counter")
        AssertEqual(7, CInt(modelRows.Rows(0)("Number")), "rolled-back sequence")
    End Sub

    Private Sub TapeSequenceNeverReusesAllocatedValue()
        Dim tapeRows As DataTable = NewTable("Tapes", "Model", "Number")
        tapeRows.Columns("Number").DataType = GetType(Integer)
        tapeRows.Rows.Add("MAX2XL", 0)
        tapeRows.Rows.Add("MAX2XL", 4)
        tapeRows.Rows.Add("TDK2SA", 50)

        AssertEqual(7, varGlobals.NextTapeSequence(tapeRows, "MAX2XL", 7), "stored high-water mark")
        AssertEqual(5, varGlobals.NextTapeSequence(tapeRows, "MAX2XL", 2), "observed high-water mark")
    End Sub

    Private Sub IntegritySourceCorrectionsRemainPresent()
        Dim modelEdit As String = Source("frmModelEdit.vb")
        Dim deckDelete As String = Source("frmDecks.vb")
        Dim tapeNew As String = Source("frmTapeNew.vb")

        AssertContains(modelEdit, "modelNotes = CStr(models.Rows(modelIndex)(""Notes""))", "model notes source")
        AssertContains(deckDelete, "counters.Rows(0)(""Number"") = deckCount", "deck counter source")
        AssertContains(tapeNew, "cmbDeckB.Items.Clear()", "side B deck choices cleared")
        AssertContains(tapeNew, "peakA = CInt(numPeakA.Value)", "side A peak captured")
        AssertContains(tapeNew, "biasCalB = CInt(numBiasCalB.Value)", "side B bias calibration captured")
        AssertContains(tapeNew, "CommitTapeBatch", "atomic batch commit")
    End Sub

    Private Function Rows(count As Integer) As DataTable
        Dim table As New DataTable()
        table.Columns.Add("Value", GetType(Integer))
        For index As Integer = 1 To count
            table.Rows.Add(index)
        Next
        Return table
    End Function

    Private Function NewTable(name As String, ParamArray columns As String()) As DataTable
        Dim table As New DataTable(name)
        For Each columnName As String In columns
            table.Columns.Add(columnName, GetType(String))
        Next
        Return table
    End Function

    Private Function Source(fileName As String) As String
        Return File.ReadAllText(Path.Combine(_repositoryRoot, "Compact Cassette Catalogue", fileName))
    End Function

    Private Function FindRepositoryRoot() As String
        Dim directory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While directory IsNot Nothing
            If File.Exists(Path.Combine(directory.FullName, "VERSION")) AndAlso
                    System.IO.Directory.Exists(Path.Combine(directory.FullName, "Compact Cassette Catalogue")) Then
                Return directory.FullName
            End If
            directory = directory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Sub AssertThrows(action As Action, name As String)
        Try
            action()
        Catch
            Return
        End Try
        Throw New InvalidOperationException(name & " did not fail.")
    End Sub

    Private Sub AssertContains(value As String, expected As String, name As String)
        If value.IndexOf(expected, StringComparison.Ordinal) < 0 Then
            Throw New InvalidOperationException(name & " is absent.")
        End If
    End Sub

    Private Sub AssertTrue(value As Boolean, name As String)
        If Not value Then
            Throw New InvalidOperationException(name & " should be true.")
        End If
    End Sub

    Private Sub AssertFalse(value As Boolean, name As String)
        If value Then
            Throw New InvalidOperationException(name & " should be false.")
        End If
    End Sub

    Private Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
