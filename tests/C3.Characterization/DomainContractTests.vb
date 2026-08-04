Imports C3.Domain.Commands
Imports C3.Domain.Identity
Imports C3.Domain.Time
Imports C3.Domain.Validation
Imports C3.Domain.Values

Friend Module DomainContractTests

    Private NotInheritable Class BrandAggregate
    End Class

    Private NotInheritable Class TapeAggregate
    End Class

    Friend Sub EntityIdsAreOpaqueTypedAndCanonical()
        Dim generator As New DeterministicEntityIdGenerator("identity-contract")
        Dim brandId As EntityId(Of BrandAggregate) = generator.Next(Of BrandAggregate)()
        Dim text As String = brandId.ToString()

        AssertEqual(32, text.Length, "canonical identifier length")
        AssertEqual(text, EntityId(Of BrandAggregate).Parse(text).ToString(), "identifier round trip")
        AssertEqual(False, brandId.IsEmpty, "generated identifier is non-empty")

        Dim parsed As EntityId(Of BrandAggregate)
        AssertEqual(False, EntityId(Of BrandAggregate).TryParse(text.ToUpperInvariant(), parsed), "uppercase rejected")
        AssertEqual(False, EntityId(Of BrandAggregate).TryParse(New String("0"c, 32), parsed), "empty identifier rejected")
    End Sub

    Friend Sub DeterministicIdsAreRepeatableAndTypeSeparated()
        Dim first As New DeterministicEntityIdGenerator("fixture-seed")
        Dim second As New DeterministicEntityIdGenerator("fixture-seed")
        Dim seen As New HashSet(Of String)(StringComparer.Ordinal)

        For index As Integer = 1 To 1000
            Dim left As String = first.Next(Of BrandAggregate)().ToString()
            Dim right As String = second.Next(Of BrandAggregate)().ToString()
            AssertEqual(left, right, "deterministic sequence " & index.ToString(CultureInfo.InvariantCulture))
            If Not seen.Add(left) Then
                Throw New InvalidOperationException("deterministic generator repeated an identifier")
            End If
        Next

        Dim brandGenerator As New DeterministicEntityIdGenerator("typed-seed")
        Dim tapeGenerator As New DeterministicEntityIdGenerator("typed-seed")
        AssertEqual(
            False,
            brandGenerator.Next(Of BrandAggregate)().ToString() = tapeGenerator.Next(Of TapeAggregate)().ToString(),
            "aggregate type separates deterministic identity")
    End Sub

    Friend Sub UtcAndOptionalValuesRejectAmbiguity()
        Dim instant As New DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc)
        Dim timestamp As New UtcTimestamp(instant)
        AssertEqual(instant, timestamp.Value, "UTC timestamp round trip")
        AssertThrows(Of ArgumentException)(
            Sub()
                Dim ignored As New UtcTimestamp(
                    DateTime.SpecifyKind(instant, DateTimeKind.Local))
            End Sub,
            "local timestamp rejection")

        Dim missing As [Optional](Of String) = [Optional](Of String).None()
        AssertEqual(False, missing.HasValue, "missing optional")
        AssertThrows(Of InvalidOperationException)(Sub() Consume(missing.Value), "missing optional access")
        Dim present As [Optional](Of String) = [Optional](Of String).Some("value")
        AssertEqual("value", present.Value, "present optional")
        AssertThrows(Of ArgumentNullException)(
            Sub()
                Dim ignored As [Optional](Of String) = [Optional](Of String).Some(Nothing)
            End Sub,
            "null optional rejection")
    End Sub

    Friend Sub CommandResultsSeparateChangesFromRejections()
        Dim change As New Change("brand", "0123456789abcdef0123456789abcdef", ChangeKind.Created)
        Dim changes As New ChangeSet(4L, 5L, New Change() {change})
        Dim success As CommandResult(Of String) = CommandResult(Of String).Success("created", changes)
        AssertEqual(True, success.IsSuccess, "command success")
        AssertEqual(1, success.ChangeSet.Changes.Count, "command change count")
        AssertEqual(0, success.Issues.Count, "successful command issues")

        Dim issue As New ValidationIssue("brand.name.required", "brand.name", "Brand name is required.")
        Dim rejected As CommandResult(Of String) = CommandResult(Of String).Rejected(
            New ValidationIssue() {issue})
        AssertEqual(False, rejected.IsSuccess, "command rejection")
        AssertEqual(Nothing, rejected.ChangeSet, "rejected command change set")
        AssertEqual("brand.name.required", rejected.Issues(0).Code, "stable issue code")
    End Sub

    Private Sub Consume(value As Object)
    End Sub

    Private Sub AssertThrows(Of TException As Exception)(action As Action, name As String)
        Try
            action()
        Catch ex As TException
            Return
        End Try
        Throw New InvalidOperationException(name & " did not throw " & GetType(TException).Name & ".")
    End Sub

    Private Sub AssertEqual(Of TValue)(expected As TValue, actual As TValue, name As String)
        If Not EqualityComparer(Of TValue).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
