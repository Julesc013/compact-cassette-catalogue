Imports C3.Infrastructure.Updates

Friend NotInheritable Class XmlUserPreferencesStoreTests

    Private Sub New()
    End Sub

    Public Shared Sub ReportsMissingFilesAndRoundTripsValues()
        WithTemporaryDirectory(
            "round-trip",
            Sub(workDirectory As String)
                Dim preferencesPath As String = Path.Combine(workDirectory, "preferences.xml")
                Dim store As New XmlUserPreferencesStore(
                    preferencesPath,
                    Function() New DateTime(2026, 8, 4, 12, 0, 0, DateTimeKind.Utc))

                AssertEqual(Path.GetFullPath(preferencesPath), store.PreferencesPath, "normalized path")
                AssertEqual(preferencesPath & ".bak", store.BackupPath, "backup path")

                Dim missing As UserPreferencesLoadResult = store.Load()
                AssertEqual(False, missing.IsSuccess, "missing load success")
                AssertEqual(True, missing.IsMissing, "missing load classification")
                AssertEqual(UserPreferencesFailure.Missing, missing.Failure, "missing load failure")

                Dim preferences As UserPreferencesSnapshot = CreateImportedPreferences(
                    False,
                    "D:\Cassettes & Mixes",
                    UpdateCheckPolicy.Weekly,
                    New DateTime(2026, 8, 4, 1, 2, 3, DateTimeKind.Utc))
                Dim saved As UserPreferencesSaveResult =
                    store.Save(preferences, UserPreferenceFields.All)

                AssertEqual(True, saved.IsSuccess, "initial save success")
                AssertEqual(UserPreferencesFailure.None, saved.Failure, "initial save failure")
                AssertEqual(Nothing, saved.BackupPath, "initial save backup")
                AssertEqual(True, File.Exists(preferencesPath), "preferences file exists")
                AssertEqual(
                    False,
                    Object.ReferenceEquals(preferences, saved.Preferences),
                    "saved result snapshot identity")

                Dim loaded As UserPreferencesLoadResult = store.Load()
                AssertEqual(True, loaded.IsSuccess, "round-trip load success")
                AssertSnapshot(preferences, loaded.Preferences, "round-trip")
            End Sub)
    End Sub

    Public Shared Sub SavesNearClassicPathBoundary()
        WithTemporaryDirectory(
            "legacy-path-boundary",
            Sub(workDirectory As String)
                Dim preferencesPath As String =
                    LegacyPathTestSupport.CreateNearBoundaryDestination(
                        workDirectory,
                        "preferences.xml")
                Dim historicalTemporaryPath As String =
                    LegacyPathTestSupport.HistoricalTemporaryPath(preferencesPath)
                AssertEqual(
                    True,
                    historicalTemporaryPath.Length >
                        LegacyPathTestSupport.ClassicMaximumPathCharacters,
                    "destination-prefixed preferences temporary exceeds classic limit")

                Dim store As New XmlUserPreferencesStore(
                    preferencesPath,
                    Function() New DateTime(2026, 8, 4, 12, 0, 0, DateTimeKind.Utc))
                Dim preferences As UserPreferencesSnapshot = CreateImportedPreferences(
                    False,
                    "D:\Near Boundary",
                    UpdateCheckPolicy.Monthly,
                    New DateTime(2026, 8, 4, 1, 2, 3, DateTimeKind.Utc))
                Dim saved As UserPreferencesSaveResult =
                    store.Save(preferences, UserPreferenceFields.All)
                If Not saved.IsSuccess Then
                    Throw New InvalidOperationException(
                        "Near-boundary preferences save failed with " &
                            saved.Failure.ToString() &
                            ": " &
                            saved.Message)
                End If

                Dim reopened As UserPreferencesLoadResult = store.Load()
                AssertEqual(True, reopened.IsSuccess, "near-boundary preferences reload")
                AssertSnapshot(preferences, reopened.Preferences, "near-boundary preferences")
                AssertEqual(
                    0,
                    Directory.GetFiles(
                        Path.GetDirectoryName(preferencesPath),
                        "~c3*.tmp",
                        SearchOption.TopDirectoryOnly).Length,
                    "near-boundary preferences temporary cleanup")
            End Sub)
    End Sub

    Public Shared Sub MergesDirtyFieldsAndCreatesBackup()
        WithTemporaryDirectory(
            "merge",
            Sub(workDirectory As String)
                Dim preferencesPath As String = Path.Combine(workDirectory, "preferences.xml")
                Dim store As New XmlUserPreferencesStore(preferencesPath, Function() DateTime.UtcNow)
                Dim original As UserPreferencesSnapshot = CreateImportedPreferences(
                    True,
                    "C:\Original",
                    UpdateCheckPolicy.Weekly,
                    New DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc))
                AssertEqual(
                    True,
                    store.Save(original, UserPreferenceFields.All).IsSuccess,
                    "merge fixture save")

                Dim incoming As UserPreferencesSnapshot = UserPreferencesSnapshot.CreateDefaults("D:\Ignored")
                incoming.ShowMessages = False
                incoming.UpdatePolicy = UpdateCheckPolicy.Monthly
                incoming.LastUpdateCheck = New DateTime(2026, 8, 3, 11, 0, 0, DateTimeKind.Utc)
                incoming.Legacy1xImportVersion = UserPreferencesSnapshot.CurrentLegacyImportVersion
                incoming.Legacy1xImportOutcome = UserPreferencesSnapshot.ImportOutcomeNotFound

                Dim saved As UserPreferencesSaveResult = store.Save(
                    incoming,
                    UserPreferenceFields.ShowMessages Or UserPreferenceFields.UpdatePolicy)
                AssertEqual(True, saved.IsSuccess, "merged save success")
                AssertEqual(store.BackupPath, saved.BackupPath, "merged save backup path")
                AssertEqual(True, File.Exists(store.BackupPath), "merged save backup exists")

                Dim loaded As UserPreferencesLoadResult = store.Load()
                AssertEqual(True, loaded.IsSuccess, "merged load success")
                AssertEqual(False, loaded.Preferences.ShowMessages, "merged message preference")
                AssertEqual("C:\Original", loaded.Preferences.DefaultDirectory, "preserved directory")
                AssertEqual(UpdateCheckPolicy.Monthly, loaded.Preferences.UpdatePolicy, "merged policy")
                AssertEqual(original.LastUpdateCheck, loaded.Preferences.LastUpdateCheck, "preserved date")
                AssertEqual(
                    UserPreferencesSnapshot.CurrentLegacyImportVersion,
                    loaded.Preferences.Legacy1xImportVersion,
                    "preserved import version")
                AssertEqual(
                    UserPreferencesSnapshot.ImportOutcomeImported,
                    loaded.Preferences.Legacy1xImportOutcome,
                    "preserved import outcome")

                Dim backupStore As New XmlUserPreferencesStore(store.BackupPath, Function() DateTime.UtcNow)
                Dim backup As UserPreferencesLoadResult = backupStore.Load()
                AssertEqual(True, backup.IsSuccess, "backup load success")
                AssertSnapshot(original, backup.Preferences, "backup")
            End Sub)
    End Sub

    Public Shared Sub RejectsInvalidSnapshotsAndUnsafeXml()
        WithTemporaryDirectory(
            "invalid",
            Sub(workDirectory As String)
                Dim invalidPath As String = Path.Combine(workDirectory, "invalid.xml")
                Dim invalidStore As New XmlUserPreferencesStore(invalidPath, Function() DateTime.UtcNow)
                Dim invalidSnapshot As UserPreferencesSnapshot =
                    UserPreferencesSnapshot.CreateDefaults("C:\Catalogues")
                invalidSnapshot.Legacy1xImportOutcome = UserPreferencesSnapshot.ImportOutcomeImported

                Dim rejectedSave As UserPreferencesSaveResult = invalidStore.Save(
                    invalidSnapshot,
                    UserPreferenceFields.All)
                AssertEqual(False, rejectedSave.IsSuccess, "invalid snapshot save success")
                AssertEqual(UserPreferencesFailure.Invalid, rejectedSave.Failure, "invalid snapshot failure")
                AssertEqual(False, File.Exists(invalidPath), "invalid snapshot file absence")

                File.WriteAllText(
                    invalidPath,
                    "<?xml version=""1.0""?><!DOCTYPE c3Preferences [" &
                        "<!ENTITY xxe SYSTEM ""file:///C:/Windows/win.ini"">]>" &
                        "<c3Preferences schemaVersion=""1"" legacy1xImportVersion=""0"" " &
                        "legacy1xImportOutcome=""pending"">" &
                        "<showMessages>true</showMessages>" &
                        "<defaultDirectory>&xxe;</defaultDirectory>" &
                        "<updatePolicy>never</updatePolicy>" &
                        "<lastUpdateCheck></lastUpdateCheck>" &
                        "</c3Preferences>",
                    New UTF8Encoding(False))
                Dim unsafeLoad As UserPreferencesLoadResult = invalidStore.Load()
                AssertEqual(False, unsafeLoad.IsSuccess, "unsafe XML load success")
                AssertEqual(UserPreferencesFailure.Invalid, unsafeLoad.Failure, "unsafe XML failure")

                File.WriteAllText(
                    invalidPath,
                    New String("x"c, (256 * 1024) + 1),
                    New UTF8Encoding(False))
                Dim oversizedLoad As UserPreferencesLoadResult = invalidStore.Load()
                AssertEqual(False, oversizedLoad.IsSuccess, "oversized load success")
                AssertEqual(UserPreferencesFailure.TooLarge, oversizedLoad.Failure, "oversized failure")
            End Sub)
    End Sub

    Public Shared Sub FutureSchemaIsRejectedWithoutBeingOverwritten()
        WithTemporaryDirectory(
            "future-schema",
            Sub(workDirectory As String)
                Dim preferencesPath As String = Path.Combine(workDirectory, "preferences.xml")
                Dim store As New XmlUserPreferencesStore(preferencesPath, Function() DateTime.UtcNow)
                File.WriteAllText(
                    preferencesPath,
                    CreateNativeXml(
                        "2",
                        "<defaultDirectory>C:\Future</defaultDirectory>",
                        " futureAttribute=""preserve-me"""))
                Dim originalBytes As Byte() = File.ReadAllBytes(preferencesPath)

                Dim loaded As UserPreferencesLoadResult = store.Load()
                AssertEqual(False, loaded.IsSuccess, "future schema load success")
                AssertEqual(UserPreferencesFailure.UnsupportedVersion, loaded.Failure, "future schema load")

                Dim saveResult As UserPreferencesSaveResult = store.Save(
                    CreateImportedPreferences(True, "C:\Current", UpdateCheckPolicy.Never, DateTime.MinValue),
                    UserPreferenceFields.All)
                AssertEqual(False, saveResult.IsSuccess, "future schema save success")
                AssertEqual(UserPreferencesFailure.UnsupportedVersion, saveResult.Failure, "future schema save")
                AssertBytesEqual(originalBytes, File.ReadAllBytes(preferencesPath), "future schema source")

                Dim namespacedXml As String = CreateNativeXml(
                    "2",
                    "<defaultDirectory>C:\Future Namespace</defaultDirectory>",
                    " xmlns=""urn:c3:preferences:future"" futureAttribute=""preserve-me""")
                File.WriteAllText(preferencesPath, namespacedXml, New UTF8Encoding(False))
                Dim namespacedBytes As Byte() = File.ReadAllBytes(preferencesPath)
                Dim namespaced As UserPreferencesLoadResult = store.Load()
                AssertEqual(False, namespaced.IsSuccess, "future namespace load success")
                AssertEqual(
                    UserPreferencesFailure.UnsupportedVersion,
                    namespaced.Failure,
                    "future namespace load")
                AssertBytesEqual(
                    namespacedBytes,
                    File.ReadAllBytes(preferencesPath),
                    "future namespace source")
            End Sub)
    End Sub

    Public Shared Sub ScalarFieldsRejectAttributesAndNestedMarkup()
        WithTemporaryDirectory(
            "scalar-fields",
            Sub(workDirectory As String)
                Dim preferencesPath As String = Path.Combine(workDirectory, "preferences.xml")
                Dim store As New XmlUserPreferencesStore(preferencesPath, Function() DateTime.UtcNow)

                File.WriteAllText(
                    preferencesPath,
                    CreateNativeXml(
                        "1",
                        "<defaultDirectory unexpected=""true"">C:\Catalogues</defaultDirectory>"))
                Dim attributed As UserPreferencesLoadResult = store.Load()
                AssertEqual(False, attributed.IsSuccess, "attributed scalar success")
                AssertEqual(UserPreferencesFailure.Invalid, attributed.Failure, "attributed scalar failure")

                File.WriteAllText(
                    preferencesPath,
                    CreateNativeXml(
                        "1",
                        "<defaultDirectory><path>C:\Catalogues</path></defaultDirectory>"))
                Dim nested As UserPreferencesLoadResult = store.Load()
                AssertEqual(False, nested.IsSuccess, "nested scalar success")
                AssertEqual(UserPreferencesFailure.Invalid, nested.Failure, "nested scalar failure")

                File.WriteAllText(
                    preferencesPath,
                    CreateNativeXml(
                        "1",
                        "<defaultDirectory>" &
                            New String(
                                "d"c,
                                UserPreferencesSnapshot.MaximumDefaultDirectoryCharacters + 1) &
                            "</defaultDirectory>"))
                Dim oversizedDirectory As UserPreferencesLoadResult = store.Load()
                AssertEqual(False, oversizedDirectory.IsSuccess, "oversized directory success")
                AssertEqual(
                    UserPreferencesFailure.Invalid,
                    oversizedDirectory.Failure,
                    "oversized directory failure")
            End Sub)
    End Sub

    Public Shared Sub NullDirectoriesNormalizeAndUnknownDirtyBitsAreRejected()
        WithTemporaryDirectory(
            "normalization",
            Sub(workDirectory As String)
                Dim preferencesPath As String = Path.Combine(workDirectory, "preferences.xml")
                Dim store As New XmlUserPreferencesStore(preferencesPath, Function() DateTime.UtcNow)
                Dim preferences As UserPreferencesSnapshot = CreateImportedPreferences(
                    True,
                    Nothing,
                    UpdateCheckPolicy.Never,
                    DateTime.MinValue)

                Dim saved As UserPreferencesSaveResult = store.Save(preferences, UserPreferenceFields.All)
                AssertEqual(True, saved.IsSuccess, "null directory save")
                AssertEqual(String.Empty, saved.Preferences.DefaultDirectory, "normalized save directory")
                AssertEqual(Nothing, preferences.DefaultDirectory, "input snapshot remains unchanged")
                AssertEqual(String.Empty, store.Load().Preferences.DefaultDirectory, "normalized load directory")

                Dim originalBytes As Byte() = File.ReadAllBytes(preferencesPath)
                Dim invalidMask As UserPreferenceFields = CType(16, UserPreferenceFields)
                Dim rejected As UserPreferencesSaveResult = store.Save(preferences, invalidMask)
                AssertEqual(False, rejected.IsSuccess, "invalid dirty mask success")
                AssertEqual(UserPreferencesFailure.Invalid, rejected.Failure, "invalid dirty mask failure")
                AssertBytesEqual(originalBytes, File.ReadAllBytes(preferencesPath), "invalid dirty mask source")
            End Sub)
    End Sub

    Private Shared Function CreateImportedPreferences(
            showMessages As Boolean,
            defaultDirectory As String,
            updatePolicy As UpdateCheckPolicy,
            lastUpdateCheck As DateTime) As UserPreferencesSnapshot

        Return New UserPreferencesSnapshot() With {
            .ShowMessages = showMessages,
            .DefaultDirectory = defaultDirectory,
            .UpdatePolicy = updatePolicy,
            .LastUpdateCheck = lastUpdateCheck,
            .Legacy1xImportVersion = UserPreferencesSnapshot.CurrentLegacyImportVersion,
            .Legacy1xImportOutcome = UserPreferencesSnapshot.ImportOutcomeImported
        }
    End Function

    Private Shared Function CreateNativeXml(
            schemaVersion As String,
            defaultDirectoryElement As String,
            Optional extraRootAttributes As String = Nothing) As String

        Return "<?xml version=""1.0"" encoding=""utf-8""?>" &
            "<c3Preferences schemaVersion=""" & schemaVersion &
            """ legacy1xImportVersion=""1"" legacy1xImportOutcome=""imported""" &
            If(extraRootAttributes, String.Empty) & ">" &
            "<showMessages>true</showMessages>" &
            defaultDirectoryElement &
            "<updatePolicy>never</updatePolicy>" &
            "<lastUpdateCheck></lastUpdateCheck>" &
            "</c3Preferences>"
    End Function

    Private Shared Sub AssertBytesEqual(expected As Byte(), actual As Byte(), name As String)
        AssertEqual(expected.Length, actual.Length, name & " length")
        For index As Integer = 0 To expected.Length - 1
            If expected(index) <> actual(index) Then
                Throw New InvalidOperationException(name & " changed at byte " & index.ToString() & ".")
            End If
        Next
    End Sub

    Private Shared Sub AssertSnapshot(
            expected As UserPreferencesSnapshot,
            actual As UserPreferencesSnapshot,
            name As String)

        AssertEqual(expected.ShowMessages, actual.ShowMessages, name & " message preference")
        AssertEqual(expected.DefaultDirectory, actual.DefaultDirectory, name & " directory")
        AssertEqual(expected.UpdatePolicy, actual.UpdatePolicy, name & " update policy")
        AssertEqual(expected.LastUpdateCheck, actual.LastUpdateCheck, name & " last update check")
        AssertEqual(
            expected.Legacy1xImportVersion,
            actual.Legacy1xImportVersion,
            name & " legacy import version")
        AssertEqual(
            expected.Legacy1xImportOutcome,
            actual.Legacy1xImportOutcome,
            name & " legacy import outcome")
    End Sub

    Private Shared Sub WithTemporaryDirectory(name As String, action As Action(Of String))
        Dim workDirectory As String = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "work",
            "preferences",
            name & "-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(workDirectory)
        Try
            action(workDirectory)
        Finally
            If Directory.Exists(workDirectory) Then
                Directory.Delete(workDirectory, True)
            End If
        End Try
    End Sub

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Class
