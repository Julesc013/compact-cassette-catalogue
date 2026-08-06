Imports C3.Infrastructure.CatalogueFiles.Xml.V2_0
Imports C3.Infrastructure.CatalogueFiles.Canonical
Imports C3.Infrastructure.Migrations.V1_1ToV2_0
Imports C3.Infrastructure.Migrations.V2_0ToV1_1
Imports C3.Catalogue.Canonical
Imports C3.Catalogue.Native
Imports C3.Domain.Catalogues

Friend NotInheritable Class LegacyToNativeMigratorTests

    Private Sub New()
    End Sub

    Public Shared Sub DryRunIsRepeatableAndReadOnly()
        Dim sourcePath As String = FixturePath("valid", "populated.xml")
        Dim sourceBytes As Byte() = File.ReadAllBytes(sourcePath)
        Dim migrator As New LegacyToNativeMigrator()
        Dim first As LegacyToNativeMigrationResult = migrator.DryRun(sourcePath)
        Dim second As LegacyToNativeMigrationResult = migrator.DryRun(sourcePath)

        AssertEqual(True, first.IsSuccess, "first dry run")
        AssertEqual(True, second.IsSuccess, "second dry run")
        AssertBytesEqual(sourceBytes, File.ReadAllBytes(sourcePath), "source bytes")
        AssertEqual(1, first.Report.Counts.Brands, "brand count")
        AssertEqual(1, first.Report.Counts.CassetteModels, "model count")
        AssertEqual(1, first.Report.Counts.DeckModels, "deck-model count")
        AssertEqual(1, first.Report.Counts.DeckUnits, "deck-unit count")
        AssertEqual(1, first.Report.Counts.Tapes, "tape count")
        AssertEqual(2, first.Report.Counts.Recordings, "recording count")
        AssertEqual(64, first.Report.SourceRevision.Length, "source revision length")
        AssertEqual(False, first.Report.HasBlockingIssues, "blocking issues")
        AssertEqual(
            New DateTime(2026, 8, 3, 14, 0, 0, DateTimeKind.Utc),
            first.Document.Brands(0).AddedAt.Value,
            "offset timestamp normalization")
        AssertEqual(
            New DateTime(2026, 8, 4, 0, 0, 0, DateTimeKind.Utc),
            first.Document.Metadata.CreatedAt.Value,
            "wall-clock timestamp normalization")
        AssertEqual(True, HasNormalization(first.Report, "legacy-offset-normalized-utc"), "offset evidence")
        AssertEqual(True, HasNormalization(first.Report, "legacy-local-wall-clock-assumed-utc"), "wall-clock evidence")

        Dim writer As New NativeXmlCatalogueWriter()
        AssertBytesEqual(
            writer.Write(first.Document),
            writer.Write(second.Document),
            "repeat migration bytes")
        AssertEqual(first.Report.Mappings.Count, second.Report.Mappings.Count, "mapping count")
        For index As Integer = 0 To first.Report.Mappings.Count - 1
            AssertEqual(first.Report.Mappings(index).EntityKind, second.Report.Mappings(index).EntityKind, "mapping kind")
            AssertEqual(first.Report.Mappings(index).LegacyKey, second.Report.Mappings(index).LegacyKey, "mapping key")
            AssertEqual(first.Report.Mappings(index).NativeId, second.Report.Mappings(index).NativeId, "mapping ID")
        Next
    End Sub

    Public Shared Sub DryRunBlocksUnresolvedRelationships()
        Dim sourcePath As String = FixturePath("migration", "unresolved-model.xml")
        Dim sourceBytes As Byte() = File.ReadAllBytes(sourcePath)
        Dim result As LegacyToNativeMigrationResult =
            New LegacyToNativeMigrator().DryRun(sourcePath)
        AssertEqual(False, result.IsSuccess, "unresolved result")
        AssertEqual(True, result.Report.HasBlockingIssues, "unresolved blocking state")
        AssertEqual(True, HasIssue(result.Report, "reference.model-unresolved"), "unresolved issue")
        AssertBytesEqual(sourceBytes, File.ReadAllBytes(sourcePath), "unresolved source bytes")
    End Sub

    Public Shared Sub NativeShadowProjectionCoversCompleteMigratedGraph()
        Dim migrated As LegacyToNativeMigrationResult =
            New LegacyToNativeMigrator().DryRun(FixturePath("valid", "populated.xml"))
        AssertEqual(True, migrated.IsSuccess, "shadow migration source")

        Dim bytes As Byte() = New NativeXmlCatalogueWriter().Write(migrated.Document)
        Dim reopened = New NativeXmlCatalogueReader().Read(bytes)
        Dim sessionId As New DocumentSessionId(
            Guid.Parse("77777777-7777-7777-7777-777777777777"))
        Dim budget As New CatalogueResourceBudget(100, 10, 5, 20, 20, 2)
        Dim projector As New NativeV2CanonicalShadowProjector()
        Dim first As CanonicalShadowProjection = projector.Project(
            migrated.Document,
            sessionId,
            ContentVersion.Zero,
            budget)
        Dim second As CanonicalShadowProjection = projector.Project(
            reopened,
            sessionId,
            ContentVersion.Zero,
            budget)
        Dim adapted As NativeCatalogue =
            New CanonicalToNativeV2Adapter().Adapt(first.State)
        Dim adaptedProjection As CanonicalShadowProjection = projector.Project(
            adapted,
            sessionId,
            ContentVersion.Zero,
            budget)

        AssertEqual("native-v2.0", first.SourceProfile.ProfileCode, "shadow source profile")
        AssertEqual(7L, first.Snapshot.TotalEntities, "shadow entity count")
        AssertEqual(8, first.Fingerprints.Entries.Count, "shadow fingerprint coverage")
        AssertEqual(1, first.State.Brands.Count, "shadow Brand state")
        AssertEqual(1, first.State.CassetteModels.Count, "shadow Model state")
        AssertEqual(1, first.State.DeckModels.Count, "shadow deck-model state")
        AssertEqual(1, first.State.DeckUnits.Count, "shadow deck-unit state")
        AssertEqual(1, first.State.Tapes.Count, "shadow Tape state")
        AssertEqual(2, first.State.Recordings.Count, "shadow recording state")
        AssertEqual(
            first.State.Brands(0).Id,
            first.State.CassetteModels(0).BrandId,
            "shadow Model-to-Brand relationship")
        AssertEqual(
            first.State.CassetteModels(0).Id,
            first.State.Tapes(0).CassetteModelId,
            "shadow Tape-to-Model relationship")
        AssertEqual(
            True,
            first.State.Tapes(0).SideA.RecordingId IsNot Nothing,
            "shadow side-to-recording relationship")
        AssertEqual(first.Snapshot.Fingerprint, second.Snapshot.Fingerprint, "native round-trip fingerprint")
        AssertEqual(
            first.Snapshot.Fingerprint,
            adaptedProjection.Snapshot.Fingerprint,
            "canonical-to-native adapter fingerprint")
        AssertBytesEqual(
            New NativeXmlCatalogueWriter().Write(migrated.Document),
            New NativeXmlCatalogueWriter().Write(adapted),
            "canonical-to-native adapter bytes")
        AssertEqual(
            True,
            New CatalogueFingerprintEngine().Verify(
                first.Fingerprints,
                second.Fingerprints.Entries),
            "shadow full verification")

        Dim originalTape As NativeTape = migrated.Document.Tapes(0)
        Dim scaledTape As New NativeTape(
            originalTape.Id,
            originalTape.CassetteModelId,
            originalTape.Year,
            Decimal.Parse("90.00", CultureInfo.InvariantCulture),
            originalTape.Region,
            originalTape.Number,
            originalTape.LegacyIdentifier,
            originalTape.LegacyShortIdentifier,
            originalTape.Condition,
            originalTape.Packaged,
            originalTape.AddedAt,
            originalTape.Notes,
            originalTape.SideA,
            originalTape.SideB)
        Dim scaledDocument As New NativeCatalogue(
            migrated.Document.Id,
            migrated.Document.Metadata,
            migrated.Document.Brands,
            migrated.Document.CassetteModels,
            migrated.Document.DeckModels,
            migrated.Document.DeckUnits,
            {scaledTape})
        Dim scaled As CanonicalShadowProjection = projector.Project(
            scaledDocument,
            sessionId,
            ContentVersion.Zero,
            budget)
        AssertEqual(
            first.Snapshot.Fingerprint,
            scaled.Snapshot.Fingerprint,
            "equivalent decimal scale")
    End Sub

    Public Shared Sub ConvertCopyWritesVerifiedReportsWithoutOverwriting()
        WithTemporaryDirectory(
            "convert-copy",
            Sub(workDirectory As String)
                Dim sourcePath As String = FixturePath("valid", "populated.xml")
                Dim sourceBytes As Byte() = File.ReadAllBytes(sourcePath)
                Dim destination As String = Path.Combine(workDirectory, "converted.c3catalogue")
                Dim service As New LegacyToNativeConversionService()
                Dim converted As MigrationConversionResult = service.ConvertCopy(sourcePath, destination)

                AssertEqual(True, converted.IsSuccess, "convert-copy result")
                AssertEqual(True, File.Exists(destination), "native destination")
                AssertEqual(True, File.Exists(destination & ".migration.json"), "machine report")
                AssertEqual(True, File.Exists(destination & ".migration.txt"), "human report")
                AssertEqual(False, File.Exists(destination & ".migration.recovery.xml"), "completed journal")
                AssertBytesEqual(sourceBytes, File.ReadAllBytes(sourcePath), "convert-copy source")
                AssertEqual(
                    converted.Report.DestinationRevision,
                    New NativeXmlCatalogueStore().Load(destination).Revision.Token,
                    "destination revision")

                Dim jsonBytes As Byte() = File.ReadAllBytes(destination & ".migration.json")
                AssertEqual(False, jsonBytes.Length >= 3 AndAlso jsonBytes(0) = &HEF AndAlso jsonBytes(1) = &HBB, "report BOM")
                Dim json As String = File.ReadAllText(destination & ".migration.json")
                AssertEqual(True, json.Contains("""schemaVersion"": 1"), "report schema")
                AssertEqual(True, json.Contains("""status"": ""completed"""), "report status")

                Dim refused As MigrationConversionResult = service.ConvertCopy(sourcePath, destination)
                AssertEqual(MigrationConversionStatus.Blocked, refused.Status, "existing destination")
                AssertBytesEqual(sourceBytes, File.ReadAllBytes(sourcePath), "refused source")
            End Sub)
    End Sub

    Public Shared Sub InterruptedConvertCopyRecoversFromVerifiedCheckpoint()
        WithTemporaryDirectory(
            "recover-copy",
            Sub(workDirectory As String)
                Dim sourcePath As String = FixturePath("valid", "populated.xml")
                Dim sourceBytes As Byte() = File.ReadAllBytes(sourcePath)
                Dim destination As String = Path.Combine(workDirectory, "interrupted.c3catalogue")
                Dim service As New LegacyToNativeConversionService()
                Dim interrupted As MigrationConversionResult = service.ConvertCopy(
                    sourcePath,
                    destination,
                    New StopAfterProgress(MigrationCheckpoint.NativeWritten))

                AssertEqual(MigrationConversionStatus.Interrupted, interrupted.Status, "interrupted status")
                AssertEqual(True, File.Exists(destination), "interrupted native destination")
                AssertEqual(True, File.Exists(interrupted.RecoveryPath), "interrupted journal")
                AssertEqual(False, File.Exists(destination & ".migration.json"), "interrupted JSON report")
                AssertBytesEqual(sourceBytes, File.ReadAllBytes(sourcePath), "interrupted source")

                Dim recovered As MigrationConversionResult = service.Recover(interrupted.RecoveryPath)
                AssertEqual(True, recovered.IsSuccess, "recovery result")
                AssertEqual(False, File.Exists(interrupted.RecoveryPath), "recovery journal cleanup")
                AssertEqual(True, File.Exists(destination & ".migration.json"), "recovered JSON report")
                AssertEqual(True, File.Exists(destination & ".migration.txt"), "recovered text report")
                AssertBytesEqual(sourceBytes, File.ReadAllBytes(sourcePath), "recovered source")
            End Sub)
    End Sub

    Public Shared Sub NativeExportIsLossAwareAndLegacyReadable()
        WithTemporaryDirectory(
            "legacy-export",
            Sub(workDirectory As String)
                Dim nativeResult As LegacyToNativeMigrationResult =
                    New LegacyToNativeMigrator().DryRun(FixturePath("valid", "populated.xml"))
                AssertEqual(True, nativeResult.IsSuccess, "native export source")
                Dim exporter As New NativeToLegacyExporter()
                Dim preview As LegacyExportPreview = exporter.Preview(nativeResult.Document)
                AssertEqual(True, preview.IsExportable, "legacy export preview")
                AssertEqual(True, HasExportIssue(preview.Report, "identity.omitted"), "identity loss")
                AssertEqual(True, HasExportIssue(preview.Report, "provenance.omitted"), "provenance loss")
                AssertEqual(True, HasExportIssue(preview.Report, "timestamp.utc-semantics"), "timestamp loss")

                Dim destination As String = Path.Combine(workDirectory, "legacy-export.xml")
                Dim exported As LegacyExportResult = exporter.ExportCopy(nativeResult.Document, destination)
                AssertEqual(True, exported.IsSuccess, "legacy export")
                AssertEqual(True, File.Exists(exported.ReportPath), "loss report")
                Dim reopened As LegacyToNativeMigrationResult =
                    New LegacyToNativeMigrator().DryRun(destination)
                AssertEqual(True, reopened.IsSuccess, "legacy export reader")
                AssertEqual(1, reopened.Report.Counts.Brands, "exported brand count")
                AssertEqual(1, reopened.Report.Counts.CassetteModels, "exported model count")
                AssertEqual(1, reopened.Report.Counts.DeckUnits, "exported deck count")
                AssertEqual(1, reopened.Report.Counts.Tapes, "exported tape count")
                AssertEqual(2, reopened.Report.Counts.Recordings, "exported recording count")

                Dim bytes As Byte() = File.ReadAllBytes(destination)
                Dim refused As LegacyExportResult = exporter.ExportCopy(nativeResult.Document, destination)
                AssertEqual(False, refused.IsSuccess, "legacy overwrite refusal")
                AssertBytesEqual(bytes, File.ReadAllBytes(destination), "legacy overwrite bytes")
            End Sub)
    End Sub

    Private Shared Function HasNormalization(report As MigrationReport, code As String) As Boolean
        For Each item As MigrationNormalization In report.Normalizations
            If String.Equals(item.Code, code, StringComparison.Ordinal) Then Return True
        Next
        Return False
    End Function

    Private Shared Function HasIssue(report As MigrationReport, code As String) As Boolean
        For Each item As MigrationIssue In report.Issues
            If String.Equals(item.Code, code, StringComparison.Ordinal) Then Return True
        Next
        Return False
    End Function

    Private Shared Function HasExportIssue(report As LegacyExportReport, code As String) As Boolean
        For Each item As LegacyExportIssue In report.Issues
            If String.Equals(item.Code, code, StringComparison.Ordinal) Then Return True
        Next
        Return False
    End Function

    Private Shared Function FixturePath(group As String, fileName As String) As String
        Return Path.Combine(
            FindRepositoryRoot(),
            "fixtures\catalogues\v1.1.0",
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
        ' Keep product-path tests independent from the absolute source/build root.
        ' The second reproducibility checkout is deliberately much longer and
        ' must not consume the classic Windows path budget intended for the
        ' destination, recovery journal, and two reports under test.
        Dim tempRoot As String = Path.GetFullPath(Path.GetTempPath())
        Dim testRoot As String = Path.GetFullPath(Path.Combine(tempRoot, "C3-MigrationTests"))
        If Not testRoot.StartsWith(tempRoot, StringComparison.OrdinalIgnoreCase) Then
            Throw New InvalidOperationException("Migration test root escaped the OS temporary directory.")
        End If
        Dim workDirectory As String = Path.Combine(testRoot, name & "-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(workDirectory)
        Try
            action(workDirectory)
        Finally
            Dim resolvedWorkDirectory As String = Path.GetFullPath(workDirectory)
            Dim expectedPrefix As String = testRoot.TrimEnd(Path.DirectorySeparatorChar) &
                Path.DirectorySeparatorChar
            If Not resolvedWorkDirectory.StartsWith(
                    expectedPrefix,
                    StringComparison.OrdinalIgnoreCase) OrElse
                    Not String.Equals(
                        Path.GetDirectoryName(resolvedWorkDirectory),
                        testRoot,
                        StringComparison.OrdinalIgnoreCase) Then
                Throw New InvalidOperationException(
                    "Refusing to clean an unsafe migration-test directory: " &
                    resolvedWorkDirectory)
            End If
            If Directory.Exists(resolvedWorkDirectory) Then
                Directory.Delete(resolvedWorkDirectory, True)
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

    Private NotInheritable Class StopAfterProgress
        Implements IMigrationProgress

        Private ReadOnly stopAfter As MigrationCheckpoint

        Public Sub New(value As MigrationCheckpoint)
            stopAfter = value
        End Sub

        Public Function ShouldContinue(checkpoint As MigrationCheckpoint) As Boolean _
                Implements IMigrationProgress.ShouldContinue
            Return checkpoint <> stopAfter
        End Function
    End Class
End Class
