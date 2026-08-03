Friend NotInheritable Class UserPreferencesServiceTests

    Private Const ApplicationRoot As String = "Compact_Cassette_Catalogu"
    Private Const EvidenceDirectory As String =
        "Compact_Cassette_Catalogu_Url_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"

    Private Sub New()
    End Sub

    Public Shared Sub FirstImportIsCheckpointedAndRepeatInitializationIsIdempotent()
        WithTemporaryDirectory(
            "first-import",
            Sub(workDirectory As String)
                Dim localRoot As String = Path.Combine(workDirectory, "LocalAppData")
                Directory.CreateDirectory(localRoot)
                Dim nativePath As String = Path.Combine(workDirectory, "native", "preferences.xml")
                Dim legacyPath As String = WriteLegacyProfile(
                    localRoot,
                    "1.2.0.0",
                    CreateLegacyXml("False", "D:\Legacy Catalogues", "monthly", "2026-08-01T10:00:00Z"))
                Dim sourceBytes As Byte() = File.ReadAllBytes(legacyPath)

                Dim first As UserPreferencesService = CreateService(nativePath, localRoot, "C:\Documents")
                Dim firstResult As UserPreferencesLoadResult = first.Initialize()
                AssertEqual(True, firstResult.IsSuccess, "first import initialization")
                AssertEqual(True, first.IsInitialized, "first service initialized")
                AssertEqual(False, first.ShowMessages, "imported message preference")
                AssertEqual("D:\Legacy Catalogues", first.DefaultDirectory, "imported directory")
                AssertEqual(UpdateCheckPolicy.Monthly, first.UpdatePolicy, "imported update policy")
                AssertEqual(
                    UserPreferencesSnapshot.ImportOutcomeImported,
                    firstResult.MigrationOutcome,
                    "first migration outcome")
                AssertEqual(True, File.Exists(nativePath), "native checkpoint exists")
                AssertBytesEqual(sourceBytes, File.ReadAllBytes(legacyPath), "legacy source")

                File.WriteAllText(
                    legacyPath,
                    CreateLegacyXml("True", "D:\Changed Legacy", "startup", String.Empty))
                Dim repeated As UserPreferencesService = CreateService(nativePath, localRoot, "C:\Documents")
                Dim repeatedResult As UserPreferencesLoadResult = repeated.Initialize()
                AssertEqual(True, repeatedResult.IsSuccess, "repeat initialization")
                AssertEqual(False, repeated.ShowMessages, "checkpointed message preference")
                AssertEqual("D:\Legacy Catalogues", repeated.DefaultDirectory, "checkpointed directory")
                AssertEqual(UpdateCheckPolicy.Monthly, repeated.UpdatePolicy, "checkpointed policy")
                AssertEqual(
                    UserPreferencesSnapshot.CurrentLegacyImportVersion,
                    repeated.Snapshot().Legacy1xImportVersion,
                    "checkpointed import version")
            End Sub)
    End Sub

    Public Shared Sub NotFoundAndInvalidImportsAreCheckpointed()
        WithTemporaryDirectory(
            "import-outcomes",
            Sub(workDirectory As String)
                Dim missingRoot As String = Path.Combine(workDirectory, "missing-legacy")
                Directory.CreateDirectory(missingRoot)
                Dim missingService As UserPreferencesService = CreateService(
                    Path.Combine(workDirectory, "missing-native", "preferences.xml"),
                    missingRoot,
                    "C:\Documents")
                Dim missing As UserPreferencesLoadResult = missingService.Initialize()
                AssertEqual(True, missing.IsSuccess, "not-found initialization")
                AssertEqual(
                    UserPreferencesSnapshot.ImportOutcomeNotFound,
                    missingService.Snapshot().Legacy1xImportOutcome,
                    "not-found outcome")
                AssertEqual(
                    UserPreferencesSnapshot.CurrentLegacyImportVersion,
                    missingService.Snapshot().Legacy1xImportVersion,
                    "not-found checkpoint")

                Dim invalidRoot As String = Path.Combine(workDirectory, "invalid-legacy")
                Directory.CreateDirectory(invalidRoot)
                WriteLegacyProfile(
                    invalidRoot,
                    "1.2.0.0",
                    CreateLegacyXml("not-a-Boolean", "C:\Ignored", "never", String.Empty))
                Dim invalidService As UserPreferencesService = CreateService(
                    Path.Combine(workDirectory, "invalid-native", "preferences.xml"),
                    invalidRoot,
                    "C:\Documents")
                Dim invalid As UserPreferencesLoadResult = invalidService.Initialize()
                AssertEqual(True, invalid.IsSuccess, "invalid-profile initialization")
                AssertEqual(
                    UserPreferencesSnapshot.ImportOutcomeInvalid,
                    invalidService.Snapshot().Legacy1xImportOutcome,
                    "invalid-profile outcome")
                AssertEqual(
                    UserPreferencesSnapshot.CurrentLegacyImportVersion,
                    invalidService.Snapshot().Legacy1xImportVersion,
                    "invalid-profile checkpoint")
            End Sub)
    End Sub

    Public Shared Sub InvalidNativePreferencesRecoverFromBackup()
        WithTemporaryDirectory(
            "backup-recovery",
            Sub(workDirectory As String)
                Dim localRoot As String = Path.Combine(workDirectory, "LocalAppData")
                Directory.CreateDirectory(localRoot)
                Dim nativePath As String = Path.Combine(workDirectory, "native", "preferences.xml")
                Dim store As New XmlUserPreferencesStore(nativePath, Function() New DateTime(2026, 8, 4))
                Dim backupSnapshot As UserPreferencesSnapshot =
                    CreateNativeSnapshot(False, "D:\Backup", UpdateCheckPolicy.Weekly)
                Dim replacedSnapshot As UserPreferencesSnapshot =
                    CreateNativeSnapshot(True, "D:\Primary", UpdateCheckPolicy.Monthly)
                AssertEqual(True, store.Save(backupSnapshot, UserPreferenceFields.All).IsSuccess, "backup seed")
                AssertEqual(True, store.Save(replacedSnapshot, UserPreferenceFields.All).IsSuccess, "primary seed")
                File.WriteAllText(nativePath, "<not-preferences>")

                Dim service As UserPreferencesService = CreateService(nativePath, localRoot, "C:\Documents")
                Dim result As UserPreferencesLoadResult = service.Initialize()
                AssertEqual(True, result.IsSuccess, "backup recovery success")
                AssertEqual(False, service.ShowMessages, "backup recovered message preference")
                AssertEqual("D:\Backup", service.DefaultDirectory, "backup recovered directory")
                AssertEqual(UpdateCheckPolicy.Weekly, service.UpdatePolicy, "backup recovered policy")
                AssertEqual(True, File.Exists(result.RecoveryPath), "quarantined primary exists")
                AssertEqual(True, store.Load().IsSuccess, "recovered primary is readable")
            End Sub)
    End Sub

    Public Shared Sub FutureNativeSchemaIsNotQuarantinedOrReplaced()
        WithTemporaryDirectory(
            "future-schema",
            Sub(workDirectory As String)
                Dim localRoot As String = Path.Combine(workDirectory, "LocalAppData")
                Directory.CreateDirectory(localRoot)
                Dim nativePath As String = Path.Combine(workDirectory, "native", "preferences.xml")
                Directory.CreateDirectory(Path.GetDirectoryName(nativePath))
                File.WriteAllText(
                    nativePath,
                    CreateNativeXml(
                        "2",
                        "C:\Future",
                        " futureAttribute=""preserve-me"""),
                    New UTF8Encoding(False))
                Dim originalBytes As Byte() = File.ReadAllBytes(nativePath)

                Dim service As UserPreferencesService = CreateService(nativePath, localRoot, "C:\Documents")
                Dim result As UserPreferencesLoadResult = service.Initialize()
                AssertEqual(False, result.IsSuccess, "future schema initialization")
                AssertEqual(UserPreferencesFailure.UnsupportedVersion, result.Failure, "future schema failure")
                AssertEqual(False, service.IsInitialized, "future schema service state")
                AssertBytesEqual(originalBytes, File.ReadAllBytes(nativePath), "future native source")
                AssertEqual(
                    0,
                    Directory.GetFiles(Path.GetDirectoryName(nativePath), "*.corrupt-*.xml").Length,
                    "future schema quarantine count")

                File.WriteAllText(
                    nativePath,
                    CreateNativeXml(
                        "2",
                        "C:\Future Namespace",
                        " xmlns=""urn:c3:preferences:future"" futureAttribute=""preserve-me"""),
                    New UTF8Encoding(False))
                Dim namespacedBytes As Byte() = File.ReadAllBytes(nativePath)
                Dim namespacedService As UserPreferencesService =
                    CreateService(nativePath, localRoot, "C:\Documents")
                Dim namespaced As UserPreferencesLoadResult = namespacedService.Initialize()
                AssertEqual(False, namespaced.IsSuccess, "future namespace initialization")
                AssertEqual(
                    UserPreferencesFailure.UnsupportedVersion,
                    namespaced.Failure,
                    "future namespace failure")
                AssertBytesEqual(namespacedBytes, File.ReadAllBytes(nativePath), "future namespace source")
                AssertEqual(
                    0,
                    Directory.GetFiles(Path.GetDirectoryName(nativePath), "*.corrupt-*.xml").Length,
                    "future namespace quarantine count")
            End Sub)
    End Sub

    Public Shared Sub DirtyFieldsMergeAcrossServiceInstances()
        WithTemporaryDirectory(
            "process-merge",
            Sub(workDirectory As String)
                Dim localRoot As String = Path.Combine(workDirectory, "LocalAppData")
                Directory.CreateDirectory(localRoot)
                Dim nativePath As String = Path.Combine(workDirectory, "native", "preferences.xml")
                Dim store As New XmlUserPreferencesStore(nativePath, Function() DateTime.UtcNow)
                AssertEqual(
                    True,
                    store.Save(
                        CreateNativeSnapshot(True, "C:\Initial", UpdateCheckPolicy.Never),
                        UserPreferenceFields.All).IsSuccess,
                    "process merge seed")

                Dim first As UserPreferencesService = CreateService(nativePath, localRoot, "C:\Documents")
                Dim second As UserPreferencesService = CreateService(nativePath, localRoot, "C:\Documents")
                AssertEqual(True, first.Initialize().IsSuccess, "first process initialization")
                AssertEqual(True, second.Initialize().IsSuccess, "second process initialization")

                first.ShowMessages = False
                second.DefaultDirectory = "D:\Second Process"
                AssertEqual(True, first.TrySave().IsSuccess, "first process save")
                AssertEqual(True, second.TrySave().IsSuccess, "second process save")

                Dim merged As UserPreferencesLoadResult = store.Load()
                AssertEqual(True, merged.IsSuccess, "merged preferences load")
                AssertEqual(False, merged.Preferences.ShowMessages, "first process field")
                AssertEqual("D:\Second Process", merged.Preferences.DefaultDirectory, "second process field")
                AssertEqual(UpdateCheckPolicy.Never, merged.Preferences.UpdatePolicy, "unchanged merged field")
            End Sub)
    End Sub

    Public Shared Sub NormalizesOnlyTheHistoricalDocumentsSentinel()
        WithTemporaryDirectory(
            "directory-normalization",
            Sub(workDirectory As String)
                Dim localRoot As String = Path.Combine(workDirectory, "LocalAppData")
                Directory.CreateDirectory(localRoot)
                Dim documentsPath As String = "D:\Injected Documents"

                Dim legitimatePath As String = Path.Combine(workDirectory, "legitimate", "preferences.xml")
                Dim legitimateStore As New XmlUserPreferencesStore(legitimatePath, Function() DateTime.UtcNow)
                AssertEqual(
                    True,
                    legitimateStore.Save(
                        CreateNativeSnapshot(True, "My.Catalogues", UpdateCheckPolicy.Never),
                        UserPreferenceFields.All).IsSuccess,
                    "legitimate directory seed")
                Dim legitimate As UserPreferencesService =
                    CreateService(legitimatePath, localRoot, documentsPath)
                AssertEqual(True, legitimate.Initialize().IsSuccess, "legitimate directory initialization")
                AssertEqual("My.Catalogues", legitimate.DefaultDirectory, "legitimate My directory")

                Dim sentinelPath As String = Path.Combine(workDirectory, "sentinel", "preferences.xml")
                Dim sentinelStore As New XmlUserPreferencesStore(sentinelPath, Function() DateTime.UtcNow)
                AssertEqual(
                    True,
                    sentinelStore.Save(
                        CreateNativeSnapshot(
                            True,
                            "My.Computer.FileSystem.SpecialDirectories.MyDocuments",
                            UpdateCheckPolicy.Never),
                        UserPreferenceFields.All).IsSuccess,
                    "historical sentinel seed")
                Dim sentinel As UserPreferencesService = CreateService(sentinelPath, localRoot, documentsPath)
                AssertEqual(True, sentinel.Initialize().IsSuccess, "sentinel initialization")
                AssertEqual(documentsPath, sentinel.DefaultDirectory, "historical sentinel normalization")
                AssertEqual(
                    documentsPath,
                    sentinelStore.Load().Preferences.DefaultDirectory,
                    "persisted sentinel normalization")
            End Sub)
    End Sub

    Public Shared Sub TransientDiscoveryFailureRemainsRetryable()
        WithTemporaryDirectory(
            "discovery-retry",
            Sub(workDirectory As String)
                Dim localRoot As String = Path.Combine(workDirectory, "LocalAppData")
                Directory.CreateDirectory(localRoot)
                Dim blockedRoot As String = Path.Combine(localRoot, ApplicationRoot)
                File.WriteAllText(blockedRoot, "not a directory")
                Dim nativePath As String = Path.Combine(workDirectory, "native", "preferences.xml")
                Dim service As UserPreferencesService =
                    CreateService(nativePath, localRoot, "C:\Documents")

                Dim failed As UserPreferencesLoadResult = service.Initialize()
                AssertEqual(False, failed.IsSuccess, "discovery failure result")
                AssertEqual(UserPreferencesFailure.IoFailure, failed.Failure, "discovery failure type")
                AssertEqual(False, service.IsInitialized, "discovery failure service state")
                AssertEqual(False, File.Exists(nativePath), "discovery failure checkpoint")

                File.Delete(blockedRoot)
                WriteLegacyProfile(
                    localRoot,
                    "1.2.0.0",
                    CreateLegacyXml("False", "D:\Recovered", "weekly", String.Empty))
                Dim retried As UserPreferencesLoadResult = service.Initialize()
                AssertEqual(True, retried.IsSuccess, "discovery retry result")
                AssertEqual(True, service.IsInitialized, "discovery retry service state")
                AssertEqual("D:\Recovered", service.DefaultDirectory, "discovery retry import")
                AssertEqual(
                    UserPreferencesSnapshot.CurrentLegacyImportVersion,
                    service.Snapshot().Legacy1xImportVersion,
                    "discovery retry checkpoint")
            End Sub)
    End Sub

    Public Shared Sub FailedCheckpointRemainsRetryable()
        WithTemporaryDirectory(
            "checkpoint-retry",
            Sub(workDirectory As String)
                Dim localRoot As String = Path.Combine(workDirectory, "LocalAppData")
                Directory.CreateDirectory(localRoot)
                WriteLegacyProfile(
                    localRoot,
                    "1.2.0.0",
                    CreateLegacyXml("False", "D:\Imported", "monthly", String.Empty))

                Dim nativePath As String = Path.Combine(workDirectory, "native", "preferences.xml")
                Dim store As New XmlUserPreferencesStore(nativePath, Function() DateTime.UtcNow)
                Dim pending As UserPreferencesSnapshot =
                    UserPreferencesSnapshot.CreateDefaults("C:\Documents")
                AssertEqual(
                    True,
                    store.Save(pending, UserPreferenceFields.All).IsSuccess,
                    "pending checkpoint seed")
                Dim pendingBytes As Byte() = File.ReadAllBytes(nativePath)
                Dim service As UserPreferencesService =
                    CreateService(nativePath, localRoot, "C:\Documents")

                Using heldOpen As New FileStream(
                        nativePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read)
                    Dim failed As UserPreferencesLoadResult = service.Initialize()
                    AssertEqual(False, failed.IsSuccess, "checkpoint failure result")
                    AssertEqual(UserPreferencesFailure.IoFailure, failed.Failure, "checkpoint failure type")
                    AssertEqual(False, service.IsInitialized, "checkpoint failure service state")
                    AssertBytesEqual(pendingBytes, File.ReadAllBytes(nativePath), "pending checkpoint source")
                End Using

                Dim retried As UserPreferencesLoadResult = service.Initialize()
                AssertEqual(True, retried.IsSuccess, "checkpoint retry result")
                AssertEqual(True, service.IsInitialized, "checkpoint retry service state")
                AssertEqual("D:\Imported", service.DefaultDirectory, "checkpoint retry import")
                AssertEqual(
                    UserPreferencesSnapshot.CurrentLegacyImportVersion,
                    store.Load().Preferences.Legacy1xImportVersion,
                    "checkpoint retry marker")
            End Sub)
    End Sub

    Private Shared Function CreateService(
            nativePath As String,
            localRoot As String,
            documentsPath As String) As UserPreferencesService

        Return New UserPreferencesService(
            New XmlUserPreferencesStore(nativePath, Function() New DateTime(2026, 8, 4)),
            New LegacyUserSettingsImporter(),
            localRoot,
            documentsPath)
    End Function

    Private Shared Function CreateNativeSnapshot(
            showMessages As Boolean,
            defaultDirectory As String,
            policy As UpdateCheckPolicy) As UserPreferencesSnapshot

        Return New UserPreferencesSnapshot() With {
            .ShowMessages = showMessages,
            .DefaultDirectory = defaultDirectory,
            .UpdatePolicy = policy,
            .LastUpdateCheck = DateTime.MinValue,
            .Legacy1xImportVersion = UserPreferencesSnapshot.CurrentLegacyImportVersion,
            .Legacy1xImportOutcome = UserPreferencesSnapshot.ImportOutcomeImported
        }
    End Function

    Private Shared Function WriteLegacyProfile(
            localRoot As String,
            version As String,
            content As String) As String

        Dim directoryPath As String = Path.Combine(
            localRoot,
            ApplicationRoot,
            EvidenceDirectory,
            version)
        Directory.CreateDirectory(directoryPath)
        Dim profilePath As String = Path.Combine(directoryPath, "user.config")
        File.WriteAllText(profilePath, content, New UTF8Encoding(False))
        Return profilePath
    End Function

    Private Shared Function CreateLegacyXml(
            showMessages As String,
            defaultDirectory As String,
            policy As String,
            lastUpdateCheck As String) As String

        Return "<?xml version=""1.0"" encoding=""utf-8""?>" &
            "<configuration><userSettings>" &
            "<Compact_Cassette_Catalogue.My.MySettings>" &
            "<setting name=""showMessages"" serializeAs=""String""><value>" &
            showMessages & "</value></setting>" &
            "<setting name=""defaultDirectory"" serializeAs=""String""><value>" &
            defaultDirectory & "</value></setting>" &
            "<setting name=""checkUpdates"" serializeAs=""String""><value>" &
            policy & "</value></setting>" &
            "<setting name=""lastUpdateCheck"" serializeAs=""String""><value>" &
            lastUpdateCheck & "</value></setting>" &
            "</Compact_Cassette_Catalogue.My.MySettings>" &
            "</userSettings></configuration>"
    End Function

    Private Shared Function CreateNativeXml(
            schemaVersion As String,
            directoryValue As String,
            Optional extraRootAttributes As String = Nothing) As String

        Return "<?xml version=""1.0"" encoding=""utf-8""?>" &
            "<c3Preferences schemaVersion=""" & schemaVersion &
            """ legacy1xImportVersion=""1"" legacy1xImportOutcome=""imported""" &
            If(extraRootAttributes, String.Empty) & ">" &
            "<showMessages>true</showMessages>" &
            "<defaultDirectory>" & directoryValue & "</defaultDirectory>" &
            "<updatePolicy>never</updatePolicy>" &
            "<lastUpdateCheck></lastUpdateCheck>" &
            "</c3Preferences>"
    End Function

    Private Shared Sub WithTemporaryDirectory(name As String, action As Action(Of String))
        Dim temporaryPath As String = Path.Combine(
            Path.GetTempPath(),
            "C3-UserPreferencesServiceTests",
            name & "-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(temporaryPath)
        Try
            action(temporaryPath)
        Finally
            If Directory.Exists(temporaryPath) Then
                Directory.Delete(temporaryPath, True)
            End If
        End Try
    End Sub

    Private Shared Sub AssertBytesEqual(expected As Byte(), actual As Byte(), name As String)
        AssertEqual(expected.Length, actual.Length, name & " length")
        For index As Integer = 0 To expected.Length - 1
            If expected(index) <> actual(index) Then
                Throw New InvalidOperationException(name & " changed at byte " & index.ToString() & ".")
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
