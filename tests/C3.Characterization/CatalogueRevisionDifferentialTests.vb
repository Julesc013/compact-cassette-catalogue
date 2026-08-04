Imports LegacyRevision = C3.Catalogue.Catalogues.CatalogueRevision
Imports NativeRevision = C3.Domain.Catalogues.CatalogueRevision
Imports System.Reflection

Friend Module CatalogueRevisionDifferentialTests

    Friend Sub NativeRevisionMatchesTheVbOracle()
        Dim valueField As FieldInfo = GetType(LegacyRevision).GetField(
            "_value",
            BindingFlags.Instance Or BindingFlags.NonPublic)
        AssertEqual(False, valueField Is Nothing, "compatibility facade value field")
        AssertEqual(GetType(NativeRevision), valueField.FieldType, "production behavior owner")

        Dim tokens As String() = {
            "fixture-revision",
            "Fixture-Revision",
            " revision tokens remain opaque ",
            "révision-テープ"
        }

        For Each token As String In tokens
            Dim legacy As New LegacyRevision(token)
            Dim native As New NativeRevision(token)
            AssertEqual(legacy.Token, native.Token, "token")
            AssertEqual(legacy.ToString(), native.ToString(), "text")
            AssertEqual(legacy.GetHashCode(), native.GetHashCode(), "ordinal hash")
        Next

        AssertEqual(
            New LegacyRevision("same").Equals(New LegacyRevision("same")),
            New NativeRevision("same").Equals(New NativeRevision("same")),
            "equal tokens")
        AssertEqual(
            New LegacyRevision("case").Equals(New LegacyRevision("CASE")),
            New NativeRevision("case").Equals(New NativeRevision("CASE")),
            "case-sensitive tokens")
        AssertEqual(
            New LegacyRevision("value").Equals(DirectCast(Nothing, LegacyRevision)),
            New NativeRevision("value").Equals(DirectCast(Nothing, NativeRevision)),
            "null equality")
        AssertEqual(
            New LegacyRevision("value").Equals(DirectCast("value", Object)),
            New NativeRevision("value").Equals(DirectCast("value", Object)),
            "other-type equality")

        For Each rejected As String In New String() {Nothing, String.Empty, " " & vbTab}
            Dim legacyFailure As ArgumentException = CaptureArgumentFailure(
                Sub() Consume(New LegacyRevision(rejected)))
            Dim nativeFailure As ArgumentException = CaptureArgumentFailure(
                Sub() Consume(New NativeRevision(rejected)))
            AssertEqual(legacyFailure.ParamName, nativeFailure.ParamName, "failure parameter")
            AssertEqual(
                legacyFailure.Message.StartsWith("A catalogue revision token is required.",
                    StringComparison.Ordinal),
                nativeFailure.Message.StartsWith("A catalogue revision token is required.",
                    StringComparison.Ordinal),
                "failure message")
        Next
    End Sub

    Private Function CaptureArgumentFailure(action As Action) As ArgumentException
        Try
            action()
        Catch ex As ArgumentException
            Return ex
        End Try

        Throw New InvalidOperationException("Expected an ArgumentException.")
    End Function

    Private Sub Consume(value As Object)
    End Sub

    Private Sub AssertEqual(Of TValue)(expected As TValue, actual As TValue, name As String)
        If Not EqualityComparer(Of TValue).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
