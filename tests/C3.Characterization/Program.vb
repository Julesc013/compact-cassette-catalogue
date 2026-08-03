Imports System.Threading

Module Program

    Private _failures As Integer
    Private _repositoryRoot As String

    Sub Main()
        _repositoryRoot = FindRepositoryRoot()

        RunTest("blank catalogue matches the v1.1 contract", AddressOf BlankCatalogueMatchesContract)
        RunTest("populated catalogue matches the v1.1 contract", AddressOf PopulatedCatalogueMatchesContract)
        RunTest("missing file version is detected", AddressOf MissingVersionIsDetected)
        RunTest("unsupported file version remains distinguishable", AddressOf UnsupportedVersionIsDetected)
        RunTest("historical prerelease suffix normalizes to v1.1.0", AddressOf HistoricalVersionSuffixNormalizes)
        RunTest("malformed XML is rejected", AddressOf MalformedXmlIsRejected)
        RunTest("external entities are rejected", AddressOf ExternalEntityIsRejected)
        RunTest("XML decimals remain culture independent", AddressOf XmlDecimalsAreCultureIndependent)
        RunTest("catalogue session owns dirty and revision state", AddressOf CatalogueSessionOwnsDocumentState)
        RunTest("store classifies unsafe and incompatible input", AddressOf StoreClassifiesRejectedInput)
        RunTest("store saves transactionally and detects external edits", AddressOf StoreSavesTransactionally)
        RunTest("brand service validates and protects referenced brands", AddressOf BrandServiceProtectsCatalogueRules)
        RunTest("cassette model service owns identifiers and reference safety", AddressOf CassetteModelServiceOwnsRules)
        RunTest("deck service preserves identity and recording references", AddressOf DeckServiceOwnsRules)
        RunTest("tape service creates batches without identifier reuse", AddressOf TapeServiceOwnsRules)

        If _failures > 0 Then
            Console.Error.WriteLine("{0} characterization test(s) failed.", _failures)
            Environment.ExitCode = 1
            Return
        End If

        Console.WriteLine("All C3 catalogue characterization tests passed.")
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

    Private Sub BlankCatalogueMatchesContract()
        Dim path As String = FixturePath("valid", "blank.xml")
        ValidateAgainstSchema(path)

        Dim document As XmlDocument = LoadSecureDocument(path)
        AssertEqual("1.1.0", ReadFileVersion(document), "blank file version")
        AssertEqual(0, document.SelectNodes("/Catalogue/Brands").Count, "blank brand count")
        AssertEqual(0, document.SelectNodes("/Catalogue/Models").Count, "blank model count")
        AssertEqual(0, document.SelectNodes("/Catalogue/Decks").Count, "blank deck count")
        AssertEqual(0, document.SelectNodes("/Catalogue/Tapes").Count, "blank tape count")
    End Sub

    Private Sub PopulatedCatalogueMatchesContract()
        Dim path As String = FixturePath("valid", "populated.xml")
        ValidateAgainstSchema(path)

        Dim document As XmlDocument = LoadSecureDocument(path)
        AssertEqual("1.1.0", ReadFileVersion(document), "populated file version")
        AssertEqual(1, document.SelectNodes("/Catalogue/Brands").Count, "brand count")
        AssertEqual(1, document.SelectNodes("/Catalogue/Models").Count, "model count")
        AssertEqual(1, document.SelectNodes("/Catalogue/Decks").Count, "deck count")
        AssertEqual(1, document.SelectNodes("/Catalogue/Tapes").Count, "tape count")
        AssertEqual("MAX", NodeText(document, "/Catalogue/Brands/Code"), "brand code")
        AssertEqual("MAX-2-XLII", NodeText(document, "/Catalogue/Models/Identifier"), "model identifier")
        AssertEqual("MAX-2-XLII-1", NodeText(document, "/Catalogue/Tapes/IdentifierShort"), "tape identifier")
    End Sub

    Private Sub MissingVersionIsDetected()
        Dim document As XmlDocument = LoadSecureDocument(FixturePath("invalid", "missing-version.xml"))
        AssertEqual(Nothing, ReadFileVersion(document), "missing file version")
    End Sub

    Private Sub UnsupportedVersionIsDetected()
        Dim document As XmlDocument = LoadSecureDocument(FixturePath("invalid", "unsupported-version.xml"))
        AssertEqual("2.0.0", ReadFileVersion(document), "unsupported file version")
    End Sub

    Private Sub HistoricalVersionSuffixNormalizes()
        AssertEqual("1.1.0", NormalizeVersion(" 1.1.0b1 "), "historical version suffix")
    End Sub

    Private Sub MalformedXmlIsRejected()
        AssertThrowsXmlException(
            Sub() LoadSecureDocument(FixturePath("invalid", "malformed.xml")),
            "malformed catalogue")
    End Sub

    Private Sub ExternalEntityIsRejected()
        AssertThrowsXmlException(
            Sub() LoadSecureDocument(FixturePath("security", "external-entity.xml")),
            "external entity catalogue")
    End Sub

    Private Sub XmlDecimalsAreCultureIndependent()
        Dim originalCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
        Try
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE")
            Dim document As XmlDocument = LoadSecureDocument(FixturePath("cultures", "decimal-dot.xml"))
            Dim parsed As Decimal = XmlConvert.ToDecimal(NodeText(document, "/Catalogue/Decks/WowFlutter"))
            AssertEqual(0.04D, parsed, "invariant decimal")
        Finally
            Thread.CurrentThread.CurrentCulture = originalCulture
        End Try
    End Sub

    Private Sub CatalogueSessionOwnsDocumentState()
        Dim session As New CatalogueSession("New Catalogue")
        Dim eventCount As Integer
        AddHandler session.SessionChanged, Sub(sender As Object, args As EventArgs) eventCount += 1

        AssertEqual(Nothing, session.FilePath, "new session path")
        AssertEqual("New Catalogue", session.DisplayName, "new session display name")
        AssertEqual(False, session.IsDirty, "new session dirty state")
        AssertEqual(0L, session.ChangeSequence, "new session change sequence")

        session.MarkChanged()
        session.MarkChanged()
        AssertEqual(True, session.IsDirty, "changed session dirty state")
        AssertEqual(2L, session.ChangeSequence, "monotonic change sequence")

        Dim revision As New CatalogueRevision("fixture-revision")
        session.MarkSaved("C:\Catalogues\fixture.xml", "fixture.xml", revision)
        AssertEqual(False, session.IsDirty, "saved session dirty state")
        AssertEqual("fixture.xml", session.DisplayName, "saved session display name")
        AssertEqual(revision, session.Revision, "saved session revision")
        AssertEqual(3, eventCount, "session changed event count")
    End Sub

    Private Sub StoreClassifiesRejectedInput()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = CreateFixtureSchema()
        Dim supported As String() = {"1.1.0"}

        AssertEqual(
            LegacyCatalogueFileFailure.InvalidXml,
            store.Load(FixturePath("invalid", "malformed.xml"), schema, supported).Failure,
            "malformed failure")
        AssertEqual(
            LegacyCatalogueFileFailure.MissingVersion,
            store.Load(FixturePath("invalid", "missing-version.xml"), schema, supported).Failure,
            "missing-version failure")
        AssertEqual(
            LegacyCatalogueFileFailure.UnsupportedVersion,
            store.Load(FixturePath("invalid", "unsupported-version.xml"), schema, supported).Failure,
            "unsupported-version failure")
        AssertEqual(
            LegacyCatalogueFileFailure.InvalidXml,
            store.Load(FixturePath("security", "external-entity.xml"), schema, supported).Failure,
            "external-entity failure")
    End Sub

    Private Sub StoreSavesTransactionally()
        Dim store As New LegacyXmlCatalogueStore()
        Dim schema As DataSet = CreateFixtureSchema()
        Dim supported As String() = {"1.1.0"}
        Dim loaded As LegacyCatalogueLoadResult = store.Load(
            FixturePath("valid", "populated.xml"),
            schema,
            supported)
        AssertEqual(True, loaded.IsSuccess, "fixture load")

        Dim workDirectory As String = Path.Combine(_repositoryRoot, "artifacts\tests\work\transactional-store")
        If Directory.Exists(workDirectory) Then
            Directory.Delete(workDirectory, True)
        End If
        Directory.CreateDirectory(workDirectory)

        Try
            Dim destination As String = Path.Combine(workDirectory, "round-trip.xml")
            Dim saved As LegacyCatalogueSaveResult = store.Save(destination, loaded.Document, Nothing, supported)
            AssertEqual(True, saved.IsSuccess, "initial transactional save")
            AssertEqual(True, File.Exists(destination), "saved destination exists")

            Dim reopened As LegacyCatalogueLoadResult = store.Load(destination, schema, supported)
            AssertEqual(True, reopened.IsSuccess, "saved file reopens")
            AssertEqual(1, reopened.Document.Tables("Tapes").Rows.Count, "saved tape count")

            File.AppendAllText(destination, Environment.NewLine & "<!-- external edit -->")
            Dim rejected As LegacyCatalogueSaveResult = store.Save(
                destination,
                reopened.Document,
                reopened.Revision,
                supported)
            AssertEqual(False, rejected.IsSuccess, "external edit save result")
            AssertEqual(LegacyCatalogueFileFailure.ExternalModification, rejected.Failure, "external edit failure")
            AssertEqual(True, File.ReadAllText(destination).Contains("external edit"), "external edit preserved")
        Finally
            If Directory.Exists(workDirectory) Then
                Directory.Delete(workDirectory, True)
            End If
        End Try
    End Sub

    Private Function CreateFixtureSchema() As DataSet
        Return LegacyCatalogueSchema.Create(New LegacyCatalogueMetadata() With {
            .FileVersion = "1.1.0",
            .ProductVersion = "1.2.1",
            .ProductStage = "Test",
            .ProductDate = New DateTime(2026, 8, 4),
            .CreatedAt = New DateTime(2026, 8, 4)
        })
    End Function

    Private Sub BrandServiceProtectsCatalogueRules()
        Dim document As DataSet = CreateFixtureSchema()
        Dim service As New BrandService(New LegacyBrandRepository(Function() document))

        Dim created As BrandOperationResult = service.Create(
            New BrandDraft("Maxell", "mx", "Created by test"),
            New DateTime(2026, 8, 4))
        AssertEqual(True, created.IsSuccess, "brand create")
        AssertEqual("MX", created.Brand.Code, "normalized brand code")
        AssertEqual(1, document.Tables("Brands").Rows.Count, "stored brand count")
        AssertEqual(1, CInt(document.Tables("Counters").Rows.Find("Brands")("Number")), "brand counter")

        Dim duplicate As BrandOperationResult = service.Create(
            New BrandDraft("Duplicate", "MX", String.Empty),
            DateTime.Now)
        AssertEqual(BrandFailure.DuplicateCode, duplicate.Failure, "duplicate brand code")

        Dim updated As BrandOperationResult = service.Update(
            "MX",
            New BrandDraft("Maxell Audio", "ignored", "Updated"))
        AssertEqual(True, updated.IsSuccess, "brand update")
        AssertEqual("MX", updated.Brand.Code, "immutable brand code")
        AssertEqual("Maxell Audio", service.Find("MX").Name, "updated brand name")

        Dim model As DataRow = document.Tables("Models").NewRow()
        model("Brand") = "MX"
        model("Identifier") = "MX-2-XLII"
        document.Tables("Models").Rows.Add(model)
        Dim referencedDelete As BrandOperationResult = service.Delete("MX")
        AssertEqual(BrandFailure.ReferencedByModel, referencedDelete.Failure, "referenced brand delete")

        document.Tables("Models").Rows.Remove(model)
        AssertEqual(True, service.Delete("MX").IsSuccess, "unreferenced brand delete")
        AssertEqual(0, document.Tables("Brands").Rows.Count, "deleted brand count")
    End Sub

    Private Sub CassetteModelServiceOwnsRules()
        Dim document As DataSet = CreateFixtureSchema()
        Dim brands As New BrandService(New LegacyBrandRepository(Function() document))
        AssertEqual(
            True,
            brands.Create(New BrandDraft("Maxell", "MX", String.Empty), DateTime.Now).IsSuccess,
            "model test brand")

        Dim service As New CassetteModelService(
            New LegacyCassetteModelRepository(Function() document))
        Dim created As CassetteModelOperationResult = service.Create(
            New CassetteModelDraft("mx", 2, "XL II", "xl", "Maxell XL II", "Reference model"),
            New DateTime(2026, 8, 4))
        AssertEqual(True, created.IsSuccess, "cassette model create")
        AssertEqual("MX2XL", created.Model.Identifier, "canonical legacy identifier")
        AssertEqual("MX", created.Model.BrandCode, "normalized model brand")
        AssertEqual(1, CInt(document.Tables("Counters").Rows.Find("Models")("Number")), "model counter")

        Dim duplicate As CassetteModelOperationResult = service.Create(
            New CassetteModelDraft("MX", 2, "Duplicate", "XL", String.Empty, String.Empty),
            DateTime.Now)
        AssertEqual(CassetteModelFailure.DuplicateIdentifier, duplicate.Failure, "duplicate model identifier")

        Dim updated As CassetteModelOperationResult = service.Update(
            "MX2XL",
            New CassetteModelDraft("ignored", 4, "XL II-S", "ZZ", "Updated display", "Updated notes"))
        AssertEqual(True, updated.IsSuccess, "cassette model update")
        AssertEqual("MX2XL", updated.Model.Identifier, "immutable model identifier")
        AssertEqual(2, updated.Model.TypeNumber, "immutable model type")
        AssertEqual("Updated notes", service.Find("MX2XL").Notes, "updated model notes")

        Dim tape As DataRow = document.Tables("Tapes").NewRow()
        tape("Model") = "MX2XL"
        tape("IdentifierShort") = "MX2XL-1"
        document.Tables("Tapes").Rows.Add(tape)
        Dim referencedDelete As CassetteModelOperationResult = service.Delete("MX2XL")
        AssertEqual(
            CassetteModelFailure.ReferencedByTape,
            referencedDelete.Failure,
            "referenced model delete")

        document.Tables("Tapes").Rows.Remove(tape)
        AssertEqual(True, service.Delete("MX2XL").IsSuccess, "unreferenced model delete")
        AssertEqual(0, document.Tables("Models").Rows.Count, "deleted model count")
    End Sub

    Private Sub DeckServiceOwnsRules()
        Dim document As DataSet = CreateFixtureSchema()
        Dim service As New DeckService(New LegacyDeckRepository(Function() document))
        Dim created As DeckOperationResult = service.Create(
            CreateValidDeckDetails("Nakamichi", "BX-300", "Original notes"),
            New DateTime(2026, 8, 4))
        AssertEqual(True, created.IsSuccess, "deck create")
        AssertEqual("Nakamichi BX-300", created.Deck.Name, "derived deck name")
        AssertEqual(1, CInt(document.Tables("Counters").Rows.Find("Decks")("Number")), "deck counter")

        Dim duplicate As DeckOperationResult = service.Create(
            CreateValidDeckDetails("Nakamichi", "BX-300", String.Empty),
            DateTime.Now)
        AssertEqual(DeckFailure.DuplicateName, duplicate.Failure, "duplicate deck name")

        Dim updated As DeckOperationResult = service.Update(
            "Nakamichi BX-300",
            CreateValidDeckDetails("Nakamichi", "BX-300 Special", "Updated notes"))
        AssertEqual(True, updated.IsSuccess, "deck update")
        AssertEqual("Nakamichi BX-300", updated.Deck.Name, "immutable deck key")
        AssertEqual("Updated notes", service.Find("Nakamichi BX-300").Details.Notes, "updated deck notes")

        Dim tape As DataRow = document.Tables("Tapes").NewRow()
        tape("DeckA") = "Nakamichi BX-300"
        tape("IdentifierShort") = "TEST-1"
        document.Tables("Tapes").Rows.Add(tape)
        Dim referencedDelete As DeckOperationResult = service.Delete("Nakamichi BX-300")
        AssertEqual(DeckFailure.ReferencedByTape, referencedDelete.Failure, "referenced deck delete")

        document.Tables("Tapes").Rows.Remove(tape)
        AssertEqual(True, service.Delete("Nakamichi BX-300").IsSuccess, "unreferenced deck delete")
        AssertEqual(0, CInt(document.Tables("Counters").Rows.Find("Decks")("Number")), "deleted deck counter")
    End Sub

    Private Function CreateValidDeckDetails(
            manufacturer As String,
            model As String,
            notes As String) As DeckDetails

        Return New DeckDetails(
            manufacturer,
            model,
            1985,
            7,
            True,
            True,
            False,
            True,
            True,
            True,
            True,
            True,
            False,
            False,
            False,
            True,
            True,
            False,
            False,
            True,
            False,
            False,
            20,
            20000,
            70,
            "Dolby C",
            0.04D,
            0.8D,
            3,
            1,
            False,
            True,
            False,
            notes)
    End Function

    Private Sub TapeServiceOwnsRules()
        Dim document As DataSet = CreateFixtureSchema()
        Dim brandService As New BrandService(New LegacyBrandRepository(Function() document))
        AssertEqual(
            True,
            brandService.Create(New BrandDraft("Maxell", "MX", String.Empty), DateTime.Now).IsSuccess,
            "tape test brand")
        Dim modelService As New CassetteModelService(
            New LegacyCassetteModelRepository(Function() document))
        AssertEqual(
            True,
            modelService.Create(
                New CassetteModelDraft("MX", 2, "XL II", "XL", String.Empty, String.Empty),
                DateTime.Now).IsSuccess,
            "tape test model")

        Dim service As New TapeService(New LegacyTapeRepository(Function() document))
        Dim sideA As New TapeSide(
            True,
            "Compilation",
            New DateTime(2026, 8, 1),
            String.Empty,
            "Line",
            0,
            "Dolby B",
            False,
            False,
            False,
            "1 7/8",
            2,
            0,
            "70us",
            5D,
            0D,
            "Music",
            "Various",
            "Test")
        Dim draft As New TapeDraft(
            "MX2XL",
            1990,
            90D,
            "Europe",
            7,
            False,
            sideA,
            TapeSide.Empty(),
            "Batch")
        Dim created As TapeOperationResult = service.CreateMany(draft, 2, New DateTime(2026, 8, 4))
        AssertEqual(True, created.IsSuccess, "tape batch create")
        AssertEqual("MX2XL9090000", created.Tapes(0).Identifier, "full tape identifier")
        AssertEqual("MX2XL000", created.Tapes(0).ShortIdentifier, "first short identifier")
        AssertEqual("MX2XL001", created.Tapes(1).ShortIdentifier, "second short identifier")
        AssertEqual(1, created.Tapes(1).Number, "per-tape sequence number")
        AssertEqual(2, CInt(document.Tables("Counters").Rows.Find("Tapes")("Number")), "tape counter")
        AssertEqual(2, modelService.Find("MX2XL").TapeCount, "model tape count")

        Dim revisedDraft As New TapeDraft(
            "ignored",
            1991,
            60D,
            "Japan",
            8,
            False,
            sideA,
            TapeSide.Empty(),
            "Updated")
        Dim updated As TapeOperationResult = service.Update("MX2XL001", revisedDraft)
        AssertEqual(True, updated.IsSuccess, "tape update")
        AssertEqual("MX2XL9160001", updated.Tapes(0).Identifier, "recomputed long identifier")
        AssertEqual("MX2XL001", updated.Tapes(0).ShortIdentifier, "immutable short identifier")
        AssertEqual(New DateTime(2026, 8, 4), updated.Tapes(0).AddedAt, "immutable tape creation date")

        AssertEqual(True, service.Delete("MX2XL000").IsSuccess, "tape delete")
        Dim afterDelete As TapeOperationResult = service.CreateMany(draft, 1, DateTime.Now)
        AssertEqual(True, afterDelete.IsSuccess, "tape create after gap")
        AssertEqual("MX2XL002", afterDelete.Tapes(0).ShortIdentifier, "identifier is not reused")
        AssertEqual(2, CInt(document.Tables("Counters").Rows.Find("Tapes")("Number")), "stable tape count")
        AssertEqual(2, modelService.Find("MX2XL").TapeCount, "stable model tape count")
    End Sub

    Private Sub ValidateAgainstSchema(xmlPath As String)
        Dim validationMessages As New List(Of String)()
        Dim settings As XmlReaderSettings = CreateSecureReaderSettings()
        settings.ValidationType = ValidationType.Schema
        settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
        settings.Schemas.Add(Nothing, Path.Combine(_repositoryRoot, "spec\catalogue\v1.1.0\catalogue.xsd"))
        AddHandler settings.ValidationEventHandler,
            Sub(sender As Object, args As ValidationEventArgs)
                validationMessages.Add(args.Message)
            End Sub

        Using reader As XmlReader = XmlReader.Create(xmlPath, settings)
            While reader.Read()
            End While
        End Using

        If validationMessages.Count > 0 Then
            Throw New InvalidOperationException(String.Join(Environment.NewLine, validationMessages.ToArray()))
        End If
    End Sub

    Private Function LoadSecureDocument(path As String) As XmlDocument
        Dim document As New XmlDocument()
        document.XmlResolver = Nothing
        Using reader As XmlReader = XmlReader.Create(path, CreateSecureReaderSettings())
            document.Load(reader)
        End Using
        Return document
    End Function

    Private Function CreateSecureReaderSettings() As XmlReaderSettings
        Dim settings As New XmlReaderSettings()
        settings.DtdProcessing = DtdProcessing.Prohibit
        settings.XmlResolver = Nothing
        settings.MaxCharactersInDocument = 16L * 1024L * 1024L
        settings.MaxCharactersFromEntities = 0L
        Return settings
    End Function

    Private Function ReadFileVersion(document As XmlDocument) As String
        Dim node As XmlNode = document.SelectSingleNode(
            "/Catalogue/Information[normalize-space(Information)='File Version']/Value")
        If node Is Nothing Then
            Return Nothing
        End If
        Return NormalizeVersion(node.InnerText)
    End Function

    Private Function NormalizeVersion(value As String) As String
        If value Is Nothing Then
            Return Nothing
        End If

        Dim match As Match = Regex.Match(value.Trim(), "^(\d+)\.(\d+)\.(\d+)")
        If match.Success Then
            Return match.Groups(1).Value & "." & match.Groups(2).Value & "." & match.Groups(3).Value
        End If
        Return value.Trim()
    End Function

    Private Function NodeText(document As XmlDocument, xpath As String) As String
        Dim node As XmlNode = document.SelectSingleNode(xpath)
        If node Is Nothing Then
            Throw New InvalidOperationException("Missing expected node: " & xpath)
        End If
        Return node.InnerText
    End Function

    Private Function FixturePath(group As String, fileName As String) As String
        Return Path.Combine(_repositoryRoot, "fixtures\catalogues\v1.1.0", group, fileName)
    End Function

    Private Function FindRepositoryRoot() As String
        Dim directory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While directory IsNot Nothing
            If System.IO.File.Exists(Path.Combine(directory.FullName, "VERSION")) AndAlso
                    System.IO.Directory.Exists(Path.Combine(directory.FullName, "spec\catalogue")) Then
                Return directory.FullName
            End If
            directory = directory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Sub AssertThrowsXmlException(action As Action, name As String)
        Try
            action()
        Catch ex As XmlException
            Return
        End Try
        Throw New InvalidOperationException(name & " did not throw XmlException.")
    End Sub

    Private Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
