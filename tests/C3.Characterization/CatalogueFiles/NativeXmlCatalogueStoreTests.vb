Imports C3.Catalogue.Catalogues
Imports C3.Catalogue.Native
Imports C3.Domain.Identity
Imports C3.Domain.Time
Imports C3.Infrastructure.CatalogueFiles.Xml.V2_0

Friend NotInheritable Class NativeXmlCatalogueStoreTests

    Private Sub New()
    End Sub

    Public Shared Sub CanonicalWriterMatchesSchemaAndRoundTrips()
        Dim writer As New NativeXmlCatalogueWriter()
        Dim reader As New NativeXmlCatalogueReader()
        Dim expected As Byte() = File.ReadAllBytes(FixturePath("valid", "blank.xml"))
        Dim actual As Byte() = writer.Write(CreateBlank())
        AssertBytesEqual(expected, actual, "canonical blank bytes")
        AssertEqual(False, actual.Length >= 3 AndAlso actual(0) = &HEF AndAlso actual(1) = &HBB, "no BOM")
        AssertEqual(CByte(10), actual(actual.Length - 1), "final LF")

        Dim settings As New XmlReaderSettings()
        settings.ValidationType = ValidationType.Schema
        settings.Schemas.Add(
            NativeXmlCatalogueWriter.NamespaceUri,
            Path.Combine(FindRepositoryRoot(), "spec\catalogue\v2.0.0\catalogue.xsd"))
        settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
        AddHandler settings.ValidationEventHandler,
            Sub(sender As Object, arguments As ValidationEventArgs)
                Throw New InvalidOperationException("Native XSD: " & arguments.Message)
            End Sub
        Using stream As New MemoryStream(actual, False)
            Using xmlReader As XmlReader = XmlReader.Create(stream, settings)
                While xmlReader.Read()
                End While
            End Using
        End Using

        Dim reopened As NativeCatalogue = reader.Read(actual)
        AssertBytesEqual(actual, writer.Write(reopened), "canonical rewrite")
        AssertEqual("C3 native-v2 fixture", reopened.Metadata.Producer, "metadata producer")
    End Sub

    Public Shared Sub ReaderRejectsUnsafeAndUnknownInput()
        Dim store As New NativeXmlCatalogueStore()
        AssertLoadFailure(
            store,
            FixturePath("security", "external-entity.xml"),
            NativeCatalogueFileFailure.UnsafeXml,
            "external entity")
        AssertLoadFailure(
            store,
            FixturePath("invalid", "unknown-core.xml"),
            NativeCatalogueFileFailure.InvalidStructure,
            "unknown core")
        AssertLoadFailure(
            store,
            FixturePath("invalid", "noncanonical-time.xml"),
            NativeCatalogueFileFailure.InvalidValue,
            "offset timestamp")
    End Sub

    Public Shared Sub StoreSavesTransactionallyAndDetectsExternalChanges()
        WithTemporaryDirectory(
            "native-store",
            Sub(workDirectory As String)
                Dim store As New NativeXmlCatalogueStore()
                Dim destination As String = Path.Combine(workDirectory, "catalogue.c3catalogue")
                Dim initial As NativeCatalogueSaveResult = store.Save(destination, CreateBlank(), Nothing)
                AssertEqual(True, initial.IsSuccess, "initial save")
                Dim initialBytes As Byte() = File.ReadAllBytes(destination)

                Dim loaded As NativeCatalogueLoadResult = store.Load(destination)
                AssertEqual(True, loaded.IsSuccess, "initial reopen")
                AssertEqual(initial.Revision, loaded.Revision, "initial revision")

                Dim changed As NativeCatalogue = CreateBlank(
                    New UtcTimestamp(New DateTime(2026, 8, 4, 0, 1, 0, DateTimeKind.Utc)))
                Dim overwritten As NativeCatalogueSaveResult =
                    store.Save(destination, changed, loaded.Revision)
                AssertEqual(True, overwritten.IsSuccess, "verified overwrite")
                AssertEqual(destination & ".bak", overwritten.BackupPath, "backup path")
                AssertBytesEqual(initialBytes, File.ReadAllBytes(overwritten.BackupPath), "backup bytes")

                File.AppendAllText(destination, Environment.NewLine & "<!-- concurrent edit -->")
                Dim concurrentBytes As Byte() = File.ReadAllBytes(destination)
                Dim rejected As NativeCatalogueSaveResult =
                    store.Save(destination, CreateBlank(), overwritten.Revision)
                AssertEqual(False, rejected.IsSuccess, "external edit result")
                AssertEqual(
                    NativeCatalogueFileFailure.ExternalModification,
                    rejected.Failure,
                    "external edit failure")
                AssertBytesEqual(concurrentBytes, File.ReadAllBytes(destination), "external edit bytes")
                AssertEqual(
                    0,
                    Directory.GetFiles(workDirectory, "~c3*.tmp").Length,
                    "owned temporary cleanup")

                Dim copyPath As String = Path.Combine(workDirectory, "copy.c3catalogue")
                Dim createdCopy As NativeCatalogueSaveResult = store.SaveNew(copyPath, CreateBlank())
                AssertEqual(True, createdCopy.IsSuccess, "new-only save")
                Dim copyBytes As Byte() = File.ReadAllBytes(copyPath)
                Dim refusedCopy As NativeCatalogueSaveResult = store.SaveNew(copyPath, changed)
                AssertEqual(False, refusedCopy.IsSuccess, "new-only overwrite")
                AssertEqual(
                    NativeCatalogueFileFailure.ExternalModification,
                    refusedCopy.Failure,
                    "new-only overwrite failure")
                AssertBytesEqual(copyBytes, File.ReadAllBytes(copyPath), "new-only preserved bytes")
            End Sub)
    End Sub

    Private Shared Function CreateBlank() As NativeCatalogue
        Return CreateBlank(
            New UtcTimestamp(New DateTime(2026, 8, 4, 0, 0, 0, DateTimeKind.Utc)))
    End Function

    Private Shared Function CreateBlank(modified As UtcTimestamp) As NativeCatalogue
        Dim created As New UtcTimestamp(New DateTime(2026, 8, 4, 0, 0, 0, DateTimeKind.Utc))
        Return New NativeCatalogue(
            EntityId(Of NativeCatalogue).Parse("11111111111111111111111111111111"),
            New NativeCatalogueMetadata(
                "C3 native-v2 fixture",
                created,
                modified,
                C3.Domain.Values.[Optional](Of NativeCatalogueProvenance).None()),
            New NativeBrand() {},
            New NativeCassetteModel() {},
            New NativeDeckModel() {},
            New NativeDeckUnit() {},
            New NativeTape() {})
    End Function

    Private Shared Sub AssertLoadFailure(
            store As NativeXmlCatalogueStore,
            path As String,
            expected As NativeCatalogueFileFailure,
            name As String)
        Dim result As NativeCatalogueLoadResult = store.Load(path)
        AssertEqual(False, result.IsSuccess, name & " success")
        AssertEqual(expected, result.Failure, name & " failure")
    End Sub

    Private Shared Function FixturePath(group As String, fileName As String) As String
        Return Path.Combine(
            FindRepositoryRoot(),
            "fixtures\catalogues\v2.0.0",
            group,
            fileName)
    End Function

    Private Shared Function FindRepositoryRoot() As String
        Dim current As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While current IsNot Nothing
            If File.Exists(Path.Combine(current.FullName, "VERSION")) AndAlso
                    Directory.Exists(Path.Combine(current.FullName, "spec\catalogue")) Then
                Return current.FullName
            End If
            current = current.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Shared Sub WithTemporaryDirectory(name As String, action As Action(Of String))
        Dim testRoot As String = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "work",
            "native-catalogue-store")
        Dim workDirectory As String = Path.Combine(testRoot, name & "-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(workDirectory)
        Try
            action(workDirectory)
        Finally
            If Directory.Exists(workDirectory) Then
                Directory.Delete(workDirectory, True)
            End If
        End Try
    End Sub

    Private Shared Sub AssertBytesEqual(expected As Byte(), actual As Byte(), name As String)
        AssertEqual(expected.Length, actual.Length, name & " length")
        For index As Integer = 0 To expected.Length - 1
            If expected(index) <> actual(index) Then
                Throw New InvalidOperationException(name & " differs at byte " & index.ToString() & ".")
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
