Imports C3.Domain.Time
Imports C3.Domain.Values

Friend Module SemanticValueContractTests

    Friend Sub QualifiedValuesKeepAbsenceKnowledgeAndZeroDistinct()
        Dim absent As [Optional](Of QualifiedValue(Of Integer)) =
            [Optional](Of QualifiedValue(Of Integer)).None()
        Dim unknown As QualifiedValue(Of Integer) =
            QualifiedValue(Of Integer).Unknown()
        Dim notApplicable As QualifiedValue(Of Integer) =
            QualifiedValue(Of Integer).NotApplicable()
        Dim zero As QualifiedValue(Of Integer) =
            QualifiedValue(Of Integer).Known(0)
        Dim estimated As QualifiedValue(Of Integer) =
            QualifiedValue(Of Integer).Estimated(90)
        Dim inferred As QualifiedValue(Of Integer) =
            QualifiedValue(Of Integer).Inferred(1985)

        AssertEqual(False, absent.HasValue, "absent field")
        AssertEqual(ValueKnowledge.Unknown, unknown.Knowledge, "unknown state")
        AssertEqual(False, unknown.HasValue, "unknown payload")
        AssertEqual(
            ValueKnowledge.NotApplicable,
            notApplicable.Knowledge,
            "not-applicable state")
        AssertEqual(False, notApplicable.HasValue, "not-applicable payload")
        AssertEqual(ValueKnowledge.Known, zero.Knowledge, "known-zero state")
        AssertEqual(True, zero.HasValue, "known-zero payload presence")
        AssertEqual(0, zero.Value, "known-zero payload")
        AssertEqual(ValueKnowledge.Estimated, estimated.Knowledge, "estimated state")
        AssertEqual(90, estimated.Value, "estimated payload")
        AssertEqual(ValueKnowledge.Inferred, inferred.Knowledge, "inferred state")
        AssertEqual(1985, inferred.Value, "inferred payload")
        AssertEqual(
            False,
            unknown.Equals(notApplicable),
            "unknown differs from not applicable")
        AssertThrows(Of InvalidOperationException)(
            Sub()
                Dim ignored As Integer = unknown.Value
            End Sub,
            "unknown payload access")
    End Sub

    Friend Sub HistoricalDatesPreservePartialPrecision()
        Dim year As HistoricalDate = HistoricalDate.FromYear(1985)
        Dim month As HistoricalDate = HistoricalDate.FromYearMonth(1985, 7)
        Dim day As HistoricalDate = HistoricalDate.FromDate(1985, 7, 13)

        AssertEqual(HistoricalDatePrecision.Year, year.Precision, "year precision")
        AssertEqual("1985", year.ToString(), "year text")
        AssertEqual(HistoricalDatePrecision.Month, month.Precision, "month precision")
        AssertEqual("1985-07", month.ToString(), "month text")
        AssertEqual(HistoricalDatePrecision.Day, day.Precision, "day precision")
        AssertEqual("1985-07-13", day.ToString(), "day text")
        AssertEqual(True, year.CompareTo(month) < 0, "partial date ordering")
        AssertEqual(True, day.Equals(HistoricalDate.FromDate(1985, 7, 13)), "date equality")

        AssertThrows(Of ArgumentOutOfRangeException)(
            Sub() HistoricalDate.FromDate(2026, 2, 29),
            "invalid calendar day")
        AssertThrows(Of ArgumentOutOfRangeException)(
            Sub() HistoricalDate.FromYearMonth(2026, 13),
            "invalid calendar month")
    End Sub

    Private Sub AssertThrows(Of TException As Exception)(action As Action, name As String)
        Try
            action()
        Catch ex As TException
            Return
        End Try

        Throw New InvalidOperationException(
            name & " did not throw " & GetType(TException).Name & ".")
    End Sub

    Private Sub AssertEqual(Of TValue)(expected As TValue, actual As TValue, name As String)
        If Not EqualityComparer(Of TValue).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
