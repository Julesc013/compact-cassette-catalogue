Imports C3.Catalogue.Canonical
Imports C3.Catalogue.Queries
Imports C3.Domain.Catalogues
Imports C3.Domain.Commands
Imports C3.Domain.Values

Friend Module CanonicalDocumentContractTests

    Friend Sub DocumentSnapshotsAndTransactionsAreVersionBoundAndBudgeted()
        Dim sessionId As New DocumentSessionId(
            Guid.Parse("11111111-1111-1111-1111-111111111111"))
        Dim version As New ContentVersion(7)
        Dim stateFingerprint As StateFingerprint = Fingerprint("a"c)
        Dim snapshot As New CatalogueSnapshot(
            sessionId,
            version,
            stateFingerprint,
            {
                New CatalogueEntityCount(CatalogueEntityKind.Brand, 2),
                New CatalogueEntityCount(CatalogueEntityKind.Tape, 3)
            })
        Dim budget As New CatalogueResourceBudget(10, 2, 3, 4, 5, 2)
        Dim document As New CatalogueDocument(snapshot, budget)
        Dim transaction As CatalogueTransaction = document.BeginTransaction(
            version,
            {
                New CatalogueMutationIntent(
                    "brand.create",
                    CatalogueEntityKind.Brand,
                    String.Empty)
            })

        AssertEqual(5L, snapshot.TotalEntities, "snapshot entity count")
        AssertEqual(7L, transaction.ExpectedVersion.Value, "transaction version")
        AssertEqual(1, transaction.Intents.Count, "transaction intent count")

        AssertThrows(Of InvalidOperationException)(
            Sub()
                document.BeginTransaction(
                    New ContentVersion(6),
                    {New CatalogueMutationIntent(
                        "brand.create",
                        CatalogueEntityKind.Brand,
                        String.Empty)})
            End Sub,
            "stale transaction")
        AssertThrows(Of ArgumentOutOfRangeException)(
            Sub()
                document.BeginTransaction(
                    version,
                    {
                        New CatalogueMutationIntent("brand.create", CatalogueEntityKind.Brand, ""),
                        New CatalogueMutationIntent("brand.update", CatalogueEntityKind.Brand, "1"),
                        New CatalogueMutationIntent("brand.delete", CatalogueEntityKind.Brand, "2")
                    })
            End Sub,
            "transaction operation budget")
    End Sub

    Friend Sub QueriesAndCursorsCannotMixDocumentVersions()
        Dim sessionId As New DocumentSessionId(
            Guid.Parse("22222222-2222-2222-2222-222222222222"))
        Dim version As New ContentVersion(9)
        Dim queryFingerprint As StateFingerprint = Fingerprint("b"c)
        Dim budget As New CatalogueResourceBudget(100, 10, 3, 4, 2, 2)
        Dim nameField As New CatalogueFieldId("brand.name")
        Dim filter As CatalogueFilter = CatalogueFilter.All(
            {
                CatalogueFilter.Value(
                    CatalogueFilterOperator.StartsWith,
                    nameField,
                    "max"),
                CatalogueFilter.Not(
                    CatalogueFilter.Knowledge(
                        CatalogueFilterOperator.IsUnknown,
                        nameField))
            })
        Dim query As New CatalogueQuery(
            CatalogueEntityKind.Brand,
            [Optional](Of CatalogueFilter).Some(filter),
            {New CatalogueSort(nameField, False)},
            2,
            queryFingerprint,
            budget)
        Dim cursor As New CatalogueCursor(
            sessionId,
            version,
            queryFingerprint,
            "maxell",
            "33333333333333333333333333333333")
        Dim projection As New CatalogueProjection(Of String)(
            sessionId,
            version,
            {"Maxell", "Memorex"},
            [Optional](Of CatalogueCursor).Some(cursor))

        AssertEqual(2, query.PageSize, "query page size")
        AssertEqual(2, projection.Items.Count, "projection item count")
        AssertEqual(True, projection.NextCursor.HasValue, "continuation cursor")

        AssertThrows(Of ArgumentException)(
            Sub()
                Dim ignored As New CatalogueProjection(Of String)(
                    sessionId,
                    New ContentVersion(10),
                    {"TDK"},
                    [Optional](Of CatalogueCursor).Some(cursor))
            End Sub,
            "mixed-version cursor")
        AssertThrows(Of ArgumentOutOfRangeException)(
            Sub()
                Dim ignored As New CatalogueQuery(
                    CatalogueEntityKind.Brand,
                    [Optional](Of CatalogueFilter).Some(filter),
                    New CatalogueSort() {},
                    3,
                    queryFingerprint,
                    budget)
            End Sub,
            "query page budget")
    End Sub

    Friend Sub CanonicalChangeSetsAdvanceVersionAndFingerprint()
        Dim changes = {
            New Change("brand", "33333333333333333333333333333333", ChangeKind.Created)
        }
        Dim changeSet As New CatalogueChangeSet(
            New ContentVersion(4),
            New ContentVersion(5),
            Fingerprint("c"c),
            Fingerprint("d"c),
            changes)

        AssertEqual(4L, changeSet.VersionBefore.Value, "change version before")
        AssertEqual(5L, changeSet.VersionAfter.Value, "change version after")
        AssertEqual(1, changeSet.Changes.Count, "change count")
        AssertEqual(
            False,
            changeSet.FingerprintBefore.Equals(changeSet.FingerprintAfter),
            "fingerprint advance")
    End Sub

    Private Function Fingerprint(character As Char) As StateFingerprint
        Return StateFingerprint.Sha256V1(New String(character, 64))
    End Function

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
