Imports System.Threading

Friend NotInheritable Class LegacyXmlCatalogueStoreTests

    Private Shared ReadOnly SupportedVersions As String() = {"1.1.0"}

    Private Sub New()
    End Sub

    Public Shared Sub StoreClassifiesRejectedInput()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()

        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "malformed.xml"),
            LegacyCatalogueFileFailure.InvalidXml,
            "malformed failure")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "missing-version.xml"),
            LegacyCatalogueFileFailure.MissingVersion,
            "missing-version failure")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "unsupported-version.xml"),
            LegacyCatalogueFileFailure.UnsupportedVersion,
            "unsupported-version failure")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("security", "external-entity.xml"),
            LegacyCatalogueFileFailure.InvalidXml,
            "external-entity failure")
    End Sub

    Public Shared Sub StoreSavesTransactionally()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()
        Dim loaded As LegacyCatalogueLoadResult = store.Load(
            FixturePath("valid", "populated.xml"),
            schema,
            SupportedVersions)
        AssertEqual(True, loaded.IsSuccess, "fixture load")

        WithTemporaryDirectory(
            "transactional-save",
            Sub(workDirectory As String)
                Dim destination As String = Path.Combine(workDirectory, "round-trip.xml")
                Dim saved As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    loaded.Document,
                    Nothing,
                    SupportedVersions)
                AssertEqual(True, saved.IsSuccess, "initial transactional save")
                AssertEqual(Of String)(Nothing, saved.BackupPath, "initial save backup path")
                AssertEqual(True, File.Exists(destination), "saved destination exists")

                Dim reopened As LegacyCatalogueLoadResult = store.Load(
                    destination,
                    schema,
                    SupportedVersions)
                AssertEqual(True, reopened.IsSuccess, "saved file reopens")
                AssertEqual(1, reopened.Document.Tables("Tapes").Rows.Count, "saved tape count")

                File.AppendAllText(destination, Environment.NewLine & "<!-- external edit -->")
                Dim externallyEditedBytes As Byte() = File.ReadAllBytes(destination)
                Dim rejected As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    reopened.Document,
                    reopened.Revision,
                    SupportedVersions)
                AssertEqual(False, rejected.IsSuccess, "external edit save result")
                AssertEqual(
                    LegacyCatalogueFileFailure.ExternalModification,
                    rejected.Failure,
                    "external edit failure")
                AssertBytesEqual(
                    externallyEditedBytes,
                    File.ReadAllBytes(destination),
                    "external edit destination")
            End Sub)
    End Sub

    Public Shared Sub SavesNearClassicPathBoundary()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()
        Dim loaded As LegacyCatalogueLoadResult = store.Load(
            FixturePath("valid", "populated.xml"),
            schema,
            SupportedVersions)
        AssertEqual(True, loaded.IsSuccess, "path-boundary fixture load")

        WithTemporaryDirectory(
            "legacy-path-boundary",
            Sub(workDirectory As String)
                Dim destination As String = LegacyPathTestSupport.CreateNearBoundaryDestination(
                    workDirectory,
                    "catalogue.xml")
                Dim historicalTemporaryPath As String =
                    LegacyPathTestSupport.HistoricalTemporaryPath(destination)
                AssertEqual(
                    True,
                    historicalTemporaryPath.Length >
                        LegacyPathTestSupport.ClassicMaximumPathCharacters,
                    "destination-prefixed temporary path exceeds classic limit")

                Dim saved As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    loaded.Document,
                    Nothing,
                    SupportedVersions)
                If Not saved.IsSuccess Then
                    Throw New InvalidOperationException(
                        "Near-boundary catalogue save failed with " &
                            saved.Failure.ToString() &
                            ": " &
                            saved.Message)
                End If

                Dim reopened As LegacyCatalogueLoadResult = store.Load(
                    destination,
                    schema,
                    SupportedVersions)
                AssertEqual(True, reopened.IsSuccess, "near-boundary catalogue reload")
                AssertEqual(1, reopened.Document.Tables("Tapes").Rows.Count, "near-boundary tape count")
                AssertEqual(
                    0,
                    Directory.GetFiles(
                        Path.GetDirectoryName(destination),
                        "~c3*.tmp",
                        SearchOption.TopDirectoryOnly).Length,
                    "near-boundary temporary cleanup")
            End Sub)
    End Sub

    Public Shared Sub RejectsWrongRootsNamespacesAndUnknownStructure()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()

        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "wrong-root.xml"),
            LegacyCatalogueFileFailure.InvalidStructure,
            "wrong root")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "namespaced-root.xml"),
            LegacyCatalogueFileFailure.InvalidStructure,
            "namespaced root")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "namespaced-row.xml"),
            LegacyCatalogueFileFailure.InvalidStructure,
            "namespaced row")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "namespaced-field.xml"),
            LegacyCatalogueFileFailure.InvalidStructure,
            "namespaced field")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "unknown-table.xml"),
            LegacyCatalogueFileFailure.InvalidStructure,
            "unknown table")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "unknown-field.xml"),
            LegacyCatalogueFileFailure.InvalidStructure,
            "unknown field")
    End Sub

    Public Shared Sub RejectsDuplicateKeysInvalidScalarsAndNestedMarkup()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()

        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "duplicate-brand-key.xml"),
            LegacyCatalogueFileFailure.ConstraintViolation,
            "duplicate brand key")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "duplicate-file-version.xml"),
            LegacyCatalogueFileFailure.ConstraintViolation,
            "duplicate File Version row")
        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "invalid-integer.xml"),
            LegacyCatalogueFileFailure.InvalidStructure,
            "invalid integer")

        AssertLoadFailure(
            store,
            schema,
            FixturePath("invalid", "nested-scalar-markup.xml"),
            LegacyCatalogueFileFailure.InvalidStructure,
            "nested scalar markup")
    End Sub

    Public Shared Sub OverwriteCreatesByteExactBackup()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()

        WithTemporaryDirectory(
            "overwrite-backup",
            Sub(workDirectory As String)
                Dim destination As String = Path.Combine(workDirectory, "catalogue.xml")
                File.Copy(FixturePath("valid", "populated.xml"), destination)
                Dim previousBytes As Byte() = File.ReadAllBytes(destination)

                Dim loaded As LegacyCatalogueLoadResult = store.Load(
                    destination,
                    schema,
                    SupportedVersions)
                AssertEqual(True, loaded.IsSuccess, "overwrite source load")
                loaded.Document.Tables("Information").Rows.Find("Program Stage")("Value") =
                    "Changed by overwrite characterization"

                Dim saved As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    loaded.Document,
                    loaded.Revision,
                    SupportedVersions)
                AssertEqual(True, saved.IsSuccess, "overwrite result")
                AssertEqual(destination & ".bak", saved.BackupPath, "reported backup path")
                AssertEqual(True, File.Exists(saved.BackupPath), "backup exists")
                AssertBytesEqual(previousBytes, File.ReadAllBytes(saved.BackupPath), "backup bytes")

                Dim reopened As LegacyCatalogueLoadResult = store.Load(
                    destination,
                    schema,
                    SupportedVersions)
                AssertEqual(True, reopened.IsSuccess, "overwritten destination reopens")
                AssertEqual(
                    "Changed by overwrite characterization",
                    CStr(reopened.Document.Tables("Information").Rows.Find("Program Stage")("Value")),
                    "overwritten value")
            End Sub)
    End Sub

    Public Shared Sub MissingDestinationRejectsExpectedRevision()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()
        Dim loaded As LegacyCatalogueLoadResult = store.Load(
            FixturePath("valid", "blank.xml"),
            schema,
            SupportedVersions)
        AssertEqual(True, loaded.IsSuccess, "missing destination fixture load")

        WithTemporaryDirectory(
            "missing-expected-destination",
            Sub(workDirectory As String)
                Dim destination As String = Path.Combine(workDirectory, "catalogue.xml")
                Dim initial As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    loaded.Document,
                    Nothing,
                    SupportedVersions)
                AssertEqual(True, initial.IsSuccess, "revision seed save")
                File.Delete(destination)

                Dim rejected As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    loaded.Document,
                    initial.Revision,
                    SupportedVersions)
                AssertEqual(False, rejected.IsSuccess, "missing expected destination result")
                AssertEqual(
                    LegacyCatalogueFileFailure.ExternalModification,
                    rejected.Failure,
                    "missing expected destination failure")
                AssertEqual(False, File.Exists(destination), "missing destination remains absent")
                AssertEqual(0, Directory.GetFiles(workDirectory).Length, "missing destination side effects")
            End Sub)
    End Sub

    Public Shared Sub RevisionMismatchPreservesDestinationBytes()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()
        Dim loaded As LegacyCatalogueLoadResult = store.Load(
            FixturePath("valid", "blank.xml"),
            schema,
            SupportedVersions)
        AssertEqual(True, loaded.IsSuccess, "revision fixture load")

        WithTemporaryDirectory(
            "revision-mismatch",
            Sub(workDirectory As String)
                Dim destination As String = Path.Combine(workDirectory, "catalogue.xml")
                Dim initial As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    loaded.Document,
                    Nothing,
                    SupportedVersions)
                AssertEqual(True, initial.IsSuccess, "revision seed save")

                File.AppendAllText(destination, Environment.NewLine & "<!-- concurrent writer -->")
                Dim concurrentBytes As Byte() = File.ReadAllBytes(destination)
                loaded.Document.Tables("Information").Rows.Find("Program Stage")("Value") =
                    "Unsaved local edit"

                Dim rejected As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    loaded.Document,
                    initial.Revision,
                    SupportedVersions)
                AssertEqual(False, rejected.IsSuccess, "revision mismatch result")
                AssertEqual(
                    LegacyCatalogueFileFailure.ExternalModification,
                    rejected.Failure,
                    "revision mismatch failure")
                AssertBytesEqual(
                    concurrentBytes,
                    File.ReadAllBytes(destination),
                    "revision mismatch destination")
                AssertEqual(False, File.Exists(destination & ".bak"), "revision mismatch backup absence")
            End Sub)
    End Sub

    Public Shared Sub RemovesOnlyOwnedTemporaryOutput()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = Program.CreateFixtureSchema()
        Dim loaded As LegacyCatalogueLoadResult = store.Load(
            FixturePath("valid", "blank.xml"),
            schema,
            SupportedVersions)
        AssertEqual(True, loaded.IsSuccess, "temporary cleanup fixture load")

        WithTemporaryDirectory(
            "temporary-cleanup",
            Sub(workDirectory As String)
                Dim destination As String = Path.Combine(workDirectory, "catalogue.xml")
                Dim unrelatedTemporaryPath As String = Path.Combine(
                    workDirectory,
                    ".catalogue.xml.not-owned-by-c3.tmp")
                Dim unrelatedBytes As Byte() = Encoding.UTF8.GetBytes("unrelated temporary content")
                File.WriteAllBytes(unrelatedTemporaryPath, unrelatedBytes)

                ' An unsupported verification set makes the store reject its own
                ' fully written temporary snapshot before it can move the file.
                Dim rejected As LegacyCatalogueSaveResult = store.Save(
                    destination,
                    loaded.Document,
                    Nothing,
                    New String() {"9.9.9"})
                AssertEqual(False, rejected.IsSuccess, "temporary verification failure result")
                AssertEqual(
                    LegacyCatalogueFileFailure.VerificationFailure,
                    rejected.Failure,
                    "temporary verification failure")
                AssertEqual(False, File.Exists(destination), "failed destination absence")
                AssertEqual(True, File.Exists(unrelatedTemporaryPath), "unrelated temporary survives")
                AssertBytesEqual(
                    unrelatedBytes,
                    File.ReadAllBytes(unrelatedTemporaryPath),
                    "unrelated temporary bytes")

                Dim remainingTemporaryFiles As String() = Directory.GetFiles(
                    workDirectory,
                    ".catalogue.xml.*.tmp",
                    SearchOption.TopDirectoryOnly)
                AssertEqual(1, remainingTemporaryFiles.Length, "remaining temporary count")
                AssertEqual(
                    unrelatedTemporaryPath,
                    remainingTemporaryFiles(0),
                    "remaining temporary identity")
                AssertEqual(
                    0,
                    Directory.GetFiles(
                        workDirectory,
                        "~c3*.tmp",
                        SearchOption.TopDirectoryOnly).Length,
                    "owned compact temporary cleanup")
            End Sub)
    End Sub

    Public Shared Sub RoundTripRemainsCultureIndependent()
        Dim originalCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
        Dim originalUiCulture As CultureInfo = Thread.CurrentThread.CurrentUICulture
        Try
            Dim nonXmlCulture As CultureInfo = CultureInfo.GetCultureInfo("de-DE")
            Thread.CurrentThread.CurrentCulture = nonXmlCulture
            Thread.CurrentThread.CurrentUICulture = nonXmlCulture

            Dim store As New LegacyXmlCatalogueStore()
            Dim schema As DataSet = Program.CreateFixtureSchema()
            Dim loaded As LegacyCatalogueLoadResult = store.Load(
                FixturePath("cultures", "decimal-dot.xml"),
                schema,
                SupportedVersions)
            AssertEqual(True, loaded.IsSuccess, "culture fixture load")
            AssertEqual(
                0.04D,
                CDec(loaded.Document.Tables("Decks").Rows(0)("WowFlutter")),
                "culture fixture decimal")

            WithTemporaryDirectory(
                "culture-round-trip",
                Sub(workDirectory As String)
                    Dim destination As String = Path.Combine(workDirectory, "catalogue.xml")
                    Dim saved As LegacyCatalogueSaveResult = store.Save(
                        destination,
                        loaded.Document,
                        Nothing,
                        SupportedVersions)
                    AssertEqual(True, saved.IsSuccess, "culture save")

                    Dim xml As String = File.ReadAllText(destination)
                    AssertEqual(True, xml.Contains("<WowFlutter>0.04</WowFlutter>"), "invariant XML decimal")
                    AssertEqual(True, xml.Contains("<Distortion>0.8</Distortion>"), "second invariant XML decimal")

                    Dim reopened As LegacyCatalogueLoadResult = store.Load(
                        destination,
                        schema,
                        SupportedVersions)
                    AssertEqual(True, reopened.IsSuccess, "culture saved file reopens")
                    AssertEqual(
                        0.04D,
                        CDec(reopened.Document.Tables("Decks").Rows(0)("WowFlutter")),
                        "culture round-trip decimal")
                End Sub)
        Finally
            Thread.CurrentThread.CurrentCulture = originalCulture
            Thread.CurrentThread.CurrentUICulture = originalUiCulture
        End Try
    End Sub

    Private Shared Sub AssertLoadFailure(
            store As LegacyXmlCatalogueStore,
            schema As DataSet,
            path As String,
            expected As LegacyCatalogueFileFailure,
            name As String)

        Dim result As LegacyCatalogueLoadResult = store.Load(path, schema, SupportedVersions)
        AssertEqual(False, result.IsSuccess, name & " success")
        AssertEqual(expected, result.Failure, name & " classification")
    End Sub

    Private Shared Function FixturePath(group As String, fileName As String) As String
        Return Path.Combine(
            FindRepositoryRoot(),
            "fixtures\catalogues\v1.1.0",
            group,
            fileName)
    End Function

    Private Shared Function FindRepositoryRoot() As String
        Dim currentDirectory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While currentDirectory IsNot Nothing
            If File.Exists(Path.Combine(currentDirectory.FullName, "VERSION")) AndAlso
                    Directory.Exists(Path.Combine(currentDirectory.FullName, "spec\catalogue")) Then
                Return currentDirectory.FullName
            End If
            currentDirectory = currentDirectory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Shared Sub WithTemporaryDirectory(name As String, action As Action(Of String))
        Dim testRoot As String = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "work",
            "catalogue-store")
        Dim workDirectory As String = Path.Combine(
            testRoot,
            name & "-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(workDirectory)
        Try
            action(workDirectory)
        Finally
            DeleteOwnedTestDirectory(workDirectory, testRoot)
        End Try
    End Sub

    Private Shared Sub DeleteOwnedTestDirectory(target As String, testRoot As String)
        Dim rootPath As String = Path.GetFullPath(testRoot).TrimEnd(Path.DirectorySeparatorChar) &
            Path.DirectorySeparatorChar
        Dim targetPath As String = Path.GetFullPath(target)
        If Not targetPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase) Then
            Throw New InvalidOperationException(
                "Refusing to clean a catalogue-store test path outside the owned test root.")
        End If
        If Not Directory.Exists(targetPath) Then
            Return
        End If
        DeleteTreeWithoutTraversingReparsePoints(targetPath)
    End Sub

    Private Shared Sub DeleteTreeWithoutTraversingReparsePoints(target As String)
        Dim attributes As FileAttributes = File.GetAttributes(target)
        If (attributes And FileAttributes.ReparsePoint) = FileAttributes.ReparsePoint Then
            Directory.Delete(target, False)
            Return
        End If

        For Each filePath As String In Directory.GetFiles(target, "*", SearchOption.TopDirectoryOnly)
            File.Delete(filePath)
        Next
        For Each directoryPath As String In Directory.GetDirectories(
                target,
                "*",
                SearchOption.TopDirectoryOnly)
            DeleteTreeWithoutTraversingReparsePoints(directoryPath)
        Next
        Directory.Delete(target, False)
    End Sub

    Private Shared Sub AssertBytesEqual(expected As Byte(), actual As Byte(), name As String)
        AssertEqual(expected.Length, actual.Length, name & " length")
        For index As Integer = 0 To expected.Length - 1
            If expected(index) <> actual(index) Then
                Throw New InvalidOperationException(
                    name & " changed at byte " & index.ToString(CultureInfo.InvariantCulture) & ".")
            End If
        Next
    End Sub

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Class
