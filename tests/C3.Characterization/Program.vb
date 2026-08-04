Imports System.Threading

Module Program

    Private _failures As Integer
    Private _repositoryRoot As String

    Sub Main(arguments As String())
        _repositoryRoot = FindRepositoryRoot()

        If arguments.Length > 0 Then
            Try
                RunCompatibilityCommand(arguments)
            Catch ex As Exception
                Console.Error.WriteLine(ex.ToString())
                Environment.ExitCode = 1
            End Try
            Return
        End If

        RunTest("blank catalogue matches the v1.1 contract", AddressOf BlankCatalogueMatchesContract)
        RunTest("populated catalogue matches the v1.1 contract", AddressOf PopulatedCatalogueMatchesContract)
        RunTest(
            "supported historical 1.x writers load through the production adapter",
            AddressOf SupportedHistoricalWritersLoadThroughProductionAdapter)
        RunTest("missing file version is detected", AddressOf MissingVersionIsDetected)
        RunTest("unsupported file version remains distinguishable", AddressOf UnsupportedVersionIsDetected)
        RunTest("historical prerelease suffix normalizes to v1.1.0", AddressOf HistoricalVersionSuffixNormalizes)
        RunTest("malformed XML is rejected", AddressOf MalformedXmlIsRejected)
        RunTest("external entities are rejected", AddressOf ExternalEntityIsRejected)
        RunTest("XML decimals remain culture independent", AddressOf XmlDecimalsAreCultureIndependent)
        RunTest("catalogue session owns dirty and revision state", AddressOf CatalogueSessionOwnsDocumentState)
        RunTest(
            "C# catalogue revision matches the VB oracle",
            AddressOf CatalogueRevisionDifferentialTests.NativeRevisionMatchesTheVbOracle)
        RunTest(
            "C# catalogue session matches the VB oracle",
            AddressOf CatalogueSessionDifferentialTests.NativeSessionMatchesTheVbOracle)
        RunTest(
            "domain identifiers are opaque typed and canonical",
            AddressOf DomainContractTests.EntityIdsAreOpaqueTypedAndCanonical)
        RunTest(
            "deterministic identifiers are repeatable unique and type-separated",
            AddressOf DomainContractTests.DeterministicIdsAreRepeatableAndTypeSeparated)
        RunTest(
            "domain timestamps and optional values reject ambiguity",
            AddressOf DomainContractTests.UtcAndOptionalValuesRejectAmbiguity)
        RunTest(
            "command results separate changes from rejections",
            AddressOf DomainContractTests.CommandResultsSeparateChangesFromRejections)
        RunTest(
            "native catalogue graph uses stable typed references",
            AddressOf NativeCatalogueContractTests.NativeGraphUsesStableTypedReferencesAndCanonicalOrder)
        RunTest(
            "store classifies unsafe and incompatible input",
            AddressOf LegacyXmlCatalogueStoreTests.StoreClassifiesRejectedInput)
        RunTest(
            "store saves transactionally and detects external edits",
            AddressOf LegacyXmlCatalogueStoreTests.StoreSavesTransactionally)
        RunTest(
            "store saves below the classic Windows path boundary",
            AddressOf LegacyXmlCatalogueStoreTests.SavesNearClassicPathBoundary)
        RunTest(
            "store rejects wrong roots namespaces and unknown structure",
            AddressOf LegacyXmlCatalogueStoreTests.RejectsWrongRootsNamespacesAndUnknownStructure)
        RunTest(
            "store rejects duplicate keys invalid scalars and nested markup",
            AddressOf LegacyXmlCatalogueStoreTests.RejectsDuplicateKeysInvalidScalarsAndNestedMarkup)
        RunTest(
            "store overwrite creates a byte-exact backup",
            AddressOf LegacyXmlCatalogueStoreTests.OverwriteCreatesByteExactBackup)
        RunTest(
            "store rejects a missing destination when a revision is expected",
            AddressOf LegacyXmlCatalogueStoreTests.MissingDestinationRejectsExpectedRevision)
        RunTest(
            "store preserves destination bytes after a revision mismatch",
            AddressOf LegacyXmlCatalogueStoreTests.RevisionMismatchPreservesDestinationBytes)
        RunTest(
            "store removes only its owned temporary output",
            AddressOf LegacyXmlCatalogueStoreTests.RemovesOnlyOwnedTemporaryOutput)
        RunTest(
            "store round-trip remains culture independent",
            AddressOf LegacyXmlCatalogueStoreTests.RoundTripRemainsCultureIndependent)
        RunTest("brand service validates and protects referenced brands", AddressOf BrandServiceProtectsCatalogueRules)
        RunTest("cassette model service owns identifiers and reference safety", AddressOf CassetteModelServiceOwnsRules)
        RunTest("deck service preserves identity and recording references", AddressOf DeckServiceOwnsRules)
        RunTest("tape service creates batches without identifier reuse", AddressOf TapeServiceOwnsRules)
        RunTest("update schedule normalizes and evaluates policies", AddressOf UpdateScheduleOwnsPolicy)
        RunTest(
            "preference format example matches schema and runtime",
            AddressOf PreferenceFormatContractTests.CanonicalExampleMatchesSchemaAndRuntime)
        RunTest(
            "user preference defaults and clones preserve independent values",
            AddressOf UserPreferencesSnapshotTests.DefaultsAndClonePreserveIndependentValues)
        RunTest(
            "stored update policy parsing accepts canonical and legacy values",
            AddressOf UpdateCheckScheduleTryParseStoredTests.AcceptsCanonicalAndLegacyValues)
        RunTest(
            "stored update policy parsing rejects unknown values",
            AddressOf UpdateCheckScheduleTryParseStoredTests.RejectsUnknownValues)
        RunTest(
            "future update timestamps do not suppress scheduled checks",
            AddressOf UpdateCheckScheduleTryParseStoredTests.FutureTimestampsDoNotSuppressScheduledChecks)
        RunTest(
            "unpublished update manifests never advertise availability",
            AddressOf UpdateReleaseManifestTests.UnpublishedManifestNeverAdvertisesAvailability)
        RunTest(
            "update identities compare alpha beta release candidate and stable precedence",
            AddressOf UpdateReleaseManifestTests.ComparesCompletePrereleaseAndStableIdentity)
        RunTest(
            "update manifest reader rejects unsafe and inconsistent JSON",
            AddressOf UpdateReleaseManifestTests.RejectsUnsafeAndInconsistentJson)
        RunTest(
            "update manifest reader rejects wrong JSON token types and published alpha",
            AddressOf UpdateReleaseManifestTests.RejectsWrongJsonTypesAndAlphaPublication)
        RunTest(
            "update manifest reader accepts the generated manifest contract",
            AddressOf UpdateReleaseManifestTests.AcceptsCurrentGeneratedManifestContract)
        RunTest(
            "published update manifests require exact tagged release assets",
            AddressOf UpdateReleaseManifestTests.PublishedManifestRequiresExactReleaseAssets)
        RunTest(
            "update endpoints are exact and channel-bound",
            AddressOf UpdateEndpointTransportTests.AcceptsOnlyExactChannelEndpoints)
        RunTest(
            "update service isolates injected retrieval failures",
            AddressOf UpdateEndpointTransportTests.ServiceUsesInjectedManifestSource)
        RunTest(
            "HTTP update source defaults to the modern TLS policy",
            AddressOf UpdateEndpointTransportTests.HttpSourceRequiresExplicitLegacyTlsMode)
        RunTest(
            "legacy settings locator accepts exact known paths in newest-first order",
            AddressOf LegacySettingsProfileLocatorTests.AcceptsExactKnownPathsAndOrdersNewestFirst)
        RunTest(
            "legacy settings locator rejects untrusted full and deep lookalikes",
            AddressOf LegacySettingsProfileLocatorTests.RejectsUntrustedFullAndDeepLookalikes)
        RunTest(
            "legacy settings reader accepts Boolean and String schemas without mutation",
            AddressOf LegacySettingsProfileReaderTests.ReadsBooleanAndStringSchemasWithoutChangingSources)
        RunTest(
            "legacy settings reader rejects nested and oversized values",
            AddressOf LegacySettingsProfileReaderTests.RejectsNestedAndOversizedValues)
        RunTest(
            "legacy settings importer distinguishes absence from discovery failure",
            AddressOf LegacyUserSettingsImporterTests.AbsenceIsNotFoundButDiscoveryIoFailurePropagates)
        RunTest(
            "legacy settings importer falls back from invalid newer content",
            AddressOf LegacyUserSettingsImporterTests.InvalidNewerContentFallsBackWithEvidence)
        RunTest(
            "legacy settings importer stops fallback when newer content is unavailable",
            AddressOf LegacyUserSettingsImporterTests.UnavailableNewerContentStopsFallback)
        RunTest(
            "XML user preferences store reports missing files and round-trips values",
            AddressOf XmlUserPreferencesStoreTests.ReportsMissingFilesAndRoundTripsValues)
        RunTest(
            "XML user preferences store saves below the classic Windows path boundary",
            AddressOf XmlUserPreferencesStoreTests.SavesNearClassicPathBoundary)
        RunTest(
            "XML user preferences store merges dirty fields and creates a backup",
            AddressOf XmlUserPreferencesStoreTests.MergesDirtyFieldsAndCreatesBackup)
        RunTest(
            "XML user preferences store rejects invalid snapshots and unsafe XML",
            AddressOf XmlUserPreferencesStoreTests.RejectsInvalidSnapshotsAndUnsafeXml)
        RunTest(
            "XML user preferences store preserves unsupported future schemas",
            AddressOf XmlUserPreferencesStoreTests.FutureSchemaIsRejectedWithoutBeingOverwritten)
        RunTest(
            "XML user preferences store rejects attributes and nested scalar markup",
            AddressOf XmlUserPreferencesStoreTests.ScalarFieldsRejectAttributesAndNestedMarkup)
        RunTest(
            "XML user preferences store normalizes nulls and rejects unknown dirty bits",
            AddressOf XmlUserPreferencesStoreTests.NullDirectoriesNormalizeAndUnknownDirtyBitsAreRejected)
        RunTest(
            "user preferences service checkpoints first import and remains idempotent",
            AddressOf UserPreferencesServiceTests.FirstImportIsCheckpointedAndRepeatInitializationIsIdempotent)
        RunTest(
            "user preferences service checkpoints not-found and invalid import outcomes",
            AddressOf UserPreferencesServiceTests.NotFoundAndInvalidImportsAreCheckpointed)
        RunTest(
            "user preferences service recovers invalid native preferences from backup",
            AddressOf UserPreferencesServiceTests.InvalidNativePreferencesRecoverFromBackup)
        RunTest(
            "user preferences service preserves unsupported future native schemas",
            AddressOf UserPreferencesServiceTests.FutureNativeSchemaIsNotQuarantinedOrReplaced)
        RunTest(
            "user preferences service merges dirty fields across instances",
            AddressOf UserPreferencesServiceTests.DirtyFieldsMergeAcrossServiceInstances)
        RunTest(
            "user preferences service normalizes only the historical documents sentinel",
            AddressOf UserPreferencesServiceTests.NormalizesOnlyTheHistoricalDocumentsSentinel)
        RunTest(
            "user preferences service retries transient legacy discovery failures",
            AddressOf UserPreferencesServiceTests.TransientDiscoveryFailureRemainsRetryable)
        RunTest(
            "user preferences service retries failed native checkpoints",
            AddressOf UserPreferencesServiceTests.FailedCheckpointRemainsRetryable)
        RunTest("public 1.x settings schemas remain captured", AddressOf PublicSettingsSchemasRemainCaptured)

        If _failures > 0 Then
            Console.Error.WriteLine("{0} characterization test(s) failed.", _failures)
            Environment.ExitCode = 1
            Return
        End If

        Console.WriteLine("All C3 catalogue characterization tests passed.")
    End Sub

    Private Sub RunCompatibilityCommand(arguments As String())
        If arguments.Length <> 2 Then
            Throw New ArgumentException(
                "Compatibility commands require an operation and one catalogue path.")
        End If

        Select Case arguments(0)
            Case "--write-current-v1.1"
                WriteCurrentCompatibilityCatalogue(arguments(1))
            Case "--validate-v1.1"
                ValidateCompatibilityCatalogue(arguments(1))
            Case Else
                Throw New ArgumentException("Unknown compatibility command: " & arguments(0))
        End Select
    End Sub

    Private Sub WriteCurrentCompatibilityCatalogue(destination As String)
        Dim store As New LegacyXmlCatalogueStore()
        Dim versions As String() = {"1.1.0"}
        Dim loaded As LegacyCatalogueLoadResult = store.Load(
            FixturePath("valid", "populated.xml"),
            CreateFixtureSchema(),
            versions)
        If Not loaded.IsSuccess Then
            Throw New InvalidOperationException(
                "Could not load the canonical source fixture: " & loaded.Message)
        End If

        Dim versionProperties As XmlDocument = LoadSecureDocument(
            Path.Combine(_repositoryRoot, "build\Version.props"))
        Dim productVersion As String = VersionProperty(
            versionProperties,
            "C3ProductVersion")
        Dim releaseStage As String = VersionProperty(
            versionProperties,
            "C3ReleaseStage")
        Dim releaseDate As DateTime = DateTime.ParseExact(
            VersionProperty(versionProperties, "C3ReleaseDate"),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None)

        Dim metadata As New LegacyCatalogueMetadataWriter(Function() loaded.Document)
        metadata.RefreshProductMetadata(productVersion, releaseStage, releaseDate)
        metadata.MarkModified(releaseDate)

        Dim fullDestination As String = Path.GetFullPath(destination)
        Dim parent As String = Path.GetDirectoryName(fullDestination)
        If String.IsNullOrWhiteSpace(parent) Then
            Throw New ArgumentException("Compatibility output requires a parent directory.")
        End If
        Directory.CreateDirectory(parent)
        If File.Exists(fullDestination) Then
            File.Delete(fullDestination)
        End If

        Dim saved As LegacyCatalogueSaveResult = store.Save(
            fullDestination,
            loaded.Document,
            Nothing,
            versions)
        If Not saved.IsSuccess Then
            Throw New InvalidOperationException(
                "Current v1.1 writer failed: " & saved.Failure.ToString() & ": " & saved.Message)
        End If
        Console.WriteLine("CURRENT_V1_1_WRITER_PASS|" & fullDestination)
    End Sub

    Private Sub ValidateCompatibilityCatalogue(path As String)
        Dim store As New LegacyXmlCatalogueStore()
        Dim loaded As LegacyCatalogueLoadResult = store.Load(
            IO.Path.GetFullPath(path),
            CreateFixtureSchema(),
            New String() {"1.1.0"})
        If Not loaded.IsSuccess Then
            Throw New InvalidOperationException(
                "Current v1.1 reader failed: " & loaded.Failure.ToString() & ": " & loaded.Message)
        End If
        AssertEqual(1, loaded.Document.Tables("Brands").Rows.Count, "compatibility brand count")
        AssertEqual(1, loaded.Document.Tables("Models").Rows.Count, "compatibility model count")
        AssertEqual(1, loaded.Document.Tables("Decks").Rows.Count, "compatibility deck count")
        AssertEqual(1, loaded.Document.Tables("Tapes").Rows.Count, "compatibility tape count")
        Console.WriteLine("CURRENT_V1_1_READER_PASS|" & IO.Path.GetFullPath(path))
    End Sub

    Private Function VersionProperty(document As XmlDocument, name As String) As String
        Dim node As XmlNode = document.SelectSingleNode(
            "/*[local-name()='Project']/*[local-name()='PropertyGroup']/*[local-name()='" &
            name & "']")
        If node Is Nothing OrElse String.IsNullOrWhiteSpace(node.InnerText) Then
            Throw New InvalidOperationException("Missing build version property: " & name)
        End If
        Return node.InnerText.Trim()
    End Function

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

    Private Sub SupportedHistoricalWritersLoadThroughProductionAdapter()
        Dim producers As New Dictionary(Of String, String)(StringComparer.Ordinal) From {
            {"v1.0.0", "1.0.0"},
            {"v1.1.0", "1.1.0"},
            {"v1.1.1", "1.1.1"},
            {"v1.1.2", "1.1.2"},
            {"v1.2.0-beta.1", "1.2.0"}
        }
        Dim store As New LegacyXmlCatalogueStore()

        For Each producer As KeyValuePair(Of String, String) In producers
            Dim historicalFixturePath As String = IO.Path.Combine(
                _repositoryRoot,
                "fixtures\catalogues\v1.1.0\historical",
                producer.Key,
                "blank.xml")
            Dim loaded As LegacyCatalogueLoadResult = store.Load(
                historicalFixturePath,
                CreateFixtureSchema(),
                New String() {"1.1.0"})
            If Not loaded.IsSuccess Then
                Throw New InvalidOperationException(
                    producer.Key & " fixture failed production load: " & loaded.Message)
            End If

            AssertEqual("1.1.0", loaded.FileVersion, producer.Key & " file version")
            AssertEqual(
                producer.Value,
                CStr(loaded.Document.Tables("Information").Rows.Find("Program Version")("Value")),
                producer.Key & " producer version")
            AssertEqual(0, loaded.Document.Tables("Brands").Rows.Count, producer.Key & " brand count")
            AssertEqual(0, loaded.Document.Tables("Models").Rows.Count, producer.Key & " model count")
            AssertEqual(0, loaded.Document.Tables("Decks").Rows.Count, producer.Key & " deck count")
            AssertEqual(0, loaded.Document.Tables("Tapes").Rows.Count, producer.Key & " tape count")
        Next
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

    Friend Function CreateFixtureSchema() As DataSet
        Return LegacyCatalogueSchema.Create(New LegacyCatalogueMetadata() With {
            .FileVersion = "1.1.0",
            .ProductVersion = "9.9.9-test",
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
        ' Historical C3 releases stored the brand display name in Models.Brand.
        model("Brand") = "Maxell Audio"
        model("Identifier") = "MX-2-XLII"
        document.Tables("Models").Rows.Add(model)
        Dim modelRepository As New LegacyCassetteModelRepository(Function() document)
        AssertEqual("MX", modelRepository.GetAll()(0).BrandCode, "legacy model brand resolves to code")
        Dim referencedDelete As BrandOperationResult = service.Delete("MX")
        AssertEqual(
            BrandFailure.ReferencedByModel,
            referencedDelete.Failure,
            "legacy-name referenced brand delete")

        Dim renamed As BrandOperationResult = service.Update(
            "MX",
            New BrandDraft("Maxell International", "ignored", "Renamed"))
        AssertEqual(True, renamed.IsSuccess, "referenced brand rename")
        AssertEqual("MX", CStr(model("Brand")), "legacy brand reference migrates to stable code")

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

    Private Sub UpdateScheduleOwnsPolicy()
        AssertEqual(UpdateCheckPolicy.Startup, UpdateCheckSchedule.Parse("STARTUP"), "startup policy")
        AssertEqual(UpdateCheckPolicy.Never, UpdateCheckSchedule.Parse("manually"), "legacy manual policy")
        AssertEqual(UpdateCheckPolicy.Never, UpdateCheckSchedule.Parse("unexpected"), "unknown policy")
        AssertEqual("monthly", UpdateCheckSchedule.Serialize(UpdateCheckPolicy.Monthly), "policy serialization")

        Dim now As New DateTime(2026, 8, 4, 12, 0, 0)
        AssertEqual(
            True,
            UpdateCheckSchedule.ShouldCheck(UpdateCheckPolicy.Startup, now, now),
            "startup check")
        AssertEqual(
            False,
            UpdateCheckSchedule.ShouldCheck(UpdateCheckPolicy.Weekly, now.AddDays(-6), now),
            "weekly check before interval")
        AssertEqual(
            True,
            UpdateCheckSchedule.ShouldCheck(UpdateCheckPolicy.Weekly, now.AddDays(-7), now),
            "weekly check at interval")
        AssertEqual(
            True,
            UpdateCheckSchedule.ShouldCheck(UpdateCheckPolicy.Monthly, DateTime.MinValue, now),
            "first monthly check")
        AssertEqual(
            False,
            UpdateCheckSchedule.ShouldCheck(UpdateCheckPolicy.Never, DateTime.MinValue, now),
            "never policy")
    End Sub

    Private Sub PublicSettingsSchemasRemainCaptured()
        Dim v100 As XmlDocument = LoadSecureDocument(SettingsFixturePath("v1.0.0"))
        AssertEqual(
            "True",
            NodeText(v100, "//setting[@name='showMessages']/value"),
            "v1.0 message default")
        AssertEqual(
            0,
            v100.SelectNodes("//setting[@name='checkUpdates']").Count,
            "v1.0 update setting absence")

        Dim v111 As XmlDocument = LoadSecureDocument(SettingsFixturePath("v1.1.1"))
        AssertEqual(
            "True",
            NodeText(v111, "//setting[@name='checkUpdates']/value"),
            "v1.1 Boolean update value")

        Dim v112 As XmlDocument = LoadSecureDocument(SettingsFixturePath("v1.1.2"))
        AssertEqual(
            "startup",
            NodeText(v112, "//setting[@name='checkUpdates']/value"),
            "v1.1.2 String update policy")
        AssertEqual(
            1,
            v112.SelectNodes("//setting[@name='lastUpdateCheck']").Count,
            "v1.1.2 last-check setting")

        Dim v120 As XmlDocument = LoadSecureDocument(SettingsFixturePath("v1.2.0-beta.1"))
        AssertEqual(
            "never",
            NodeText(v120, "//setting[@name='checkUpdates']/value"),
            "v1.2 default update policy")
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

    Private Function SettingsFixturePath(version As String) As String
        Return Path.Combine(_repositoryRoot, "fixtures\settings\legacy", version, "user.config")
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
