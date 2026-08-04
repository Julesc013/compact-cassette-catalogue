Imports LegacyRevision = C3.Catalogue.Catalogues.CatalogueRevision
Imports LegacySession = C3.Catalogue.Catalogues.CatalogueSession
Imports NativeRevision = C3.Domain.Catalogues.CatalogueRevision
Imports NativeSession = C3.Domain.Catalogues.CatalogueSession

Friend Module CatalogueSessionDifferentialTests

    Friend Sub NativeSessionMatchesTheVbOracle()
        Dim legacy As New LegacySession("New Catalogue")
        Dim native As New NativeSession("New Catalogue")
        Dim legacyEvents As Integer
        Dim nativeEvents As Integer
        AddHandler legacy.SessionChanged, Sub(sender, args) legacyEvents += 1
        AddHandler native.SessionChanged, Sub(sender, args) nativeEvents += 1

        AssertState(legacy, native, "initial")

        legacy.MarkChanged()
        native.MarkChanged()
        AssertState(legacy, native, "changed")

        legacy.SetDirtyForMigration(True)
        native.SetDirtyForMigration(True)
        AssertState(legacy, native, "migration dirty")

        legacy.SetDirtyForMigration(False)
        native.SetDirtyForMigration(False)
        AssertState(legacy, native, "migration clean")

        legacy.SetDirtyForMigration(False)
        native.SetDirtyForMigration(False)
        AssertState(legacy, native, "already clean")

        legacy.SetDocumentLocation("C:\Catalogues\one.xml", "one.xml")
        native.SetDocumentLocation("C:\Catalogues\one.xml", "one.xml")
        AssertState(legacy, native, "document location")

        legacy.MarkLoaded(
            "C:\Catalogues\loaded.xml",
            "loaded.xml",
            New LegacyRevision("loaded"))
        native.MarkLoaded(
            "C:\Catalogues\loaded.xml",
            "loaded.xml",
            New NativeRevision("loaded"))
        AssertState(legacy, native, "loaded")

        legacy.MarkChanged()
        native.MarkChanged()
        legacy.MarkSaved(
            "C:\Catalogues\saved.xml",
            "saved.xml",
            New LegacyRevision("saved"))
        native.MarkSaved(
            "C:\Catalogues\saved.xml",
            "saved.xml",
            New NativeRevision("saved"))
        AssertState(legacy, native, "saved")

        legacy.BeginNew("Another Catalogue")
        native.BeginNew("Another Catalogue")
        AssertState(legacy, native, "begin new")
        AssertEqual(legacyEvents, nativeEvents, "event count")

        AssertConstructorFailure(Nothing)
        AssertConstructorFailure(String.Empty)
        AssertConstructorFailure(" " & vbTab)
        AssertDisplayNameFailure(
            Sub() legacy.BeginNew(Nothing),
            Sub() native.BeginNew(Nothing),
            "begin new")
        AssertDisplayNameFailure(
            Sub() legacy.SetDocumentLocation(Nothing, ""),
            Sub() native.SetDocumentLocation(Nothing, ""),
            "set location")
        AssertDisplayNameFailure(
            Sub() legacy.MarkLoaded(Nothing, " ", Nothing),
            Sub() native.MarkLoaded(Nothing, " ", Nothing),
            "mark loaded")
        AssertDisplayNameFailure(
            Sub() legacy.MarkSaved(Nothing, vbTab, Nothing),
            Sub() native.MarkSaved(Nothing, vbTab, Nothing),
            "mark saved")
    End Sub

    Private Sub AssertState(legacy As LegacySession, native As NativeSession, name As String)
        AssertEqual(legacy.FilePath, native.FilePath, name & " path")
        AssertEqual(legacy.DisplayName, native.DisplayName, name & " display name")
        AssertEqual(legacy.IsDirty, native.IsDirty, name & " dirty")
        AssertEqual(legacy.ChangeSequence, native.ChangeSequence, name & " sequence")
        AssertEqual(
            If(legacy.Revision Is Nothing, Nothing, legacy.Revision.Token),
            If(native.Revision Is Nothing, Nothing, native.Revision.Token),
            name & " revision")
    End Sub

    Private Sub AssertConstructorFailure(value As String)
        Dim legacyFailure As ArgumentException = CaptureArgumentFailure(
            Sub() Consume(New LegacySession(value)))
        Dim nativeFailure As ArgumentException = CaptureArgumentFailure(
            Sub() Consume(New NativeSession(value)))
        AssertEqual(legacyFailure.ParamName, nativeFailure.ParamName, "constructor parameter")
    End Sub

    Private Sub AssertDisplayNameFailure(
            legacyAction As Action,
            nativeAction As Action,
            name As String)
        Dim legacyFailure As ArgumentException = CaptureArgumentFailure(legacyAction)
        Dim nativeFailure As ArgumentException = CaptureArgumentFailure(nativeAction)
        AssertEqual(legacyFailure.ParamName, nativeFailure.ParamName, name & " parameter")
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
