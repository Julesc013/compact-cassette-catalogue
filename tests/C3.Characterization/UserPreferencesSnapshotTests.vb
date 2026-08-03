Imports C3.Infrastructure.Updates

Friend NotInheritable Class UserPreferencesSnapshotTests

    Private Sub New()
    End Sub

    Public Shared Sub DefaultsAndClonePreserveIndependentValues()
        Dim defaults As UserPreferencesSnapshot =
            UserPreferencesSnapshot.CreateDefaults("C:\Users\Jules\Documents")

        AssertEqual(True, defaults.ShowMessages, "default message preference")
        AssertEqual(
            "C:\Users\Jules\Documents",
            defaults.DefaultDirectory,
            "default catalogue directory")
        AssertEqual(UpdateCheckPolicy.Never, defaults.UpdatePolicy, "default update policy")
        AssertEqual(DateTime.MinValue, defaults.LastUpdateCheck, "default last update check")
        AssertEqual(0, defaults.Legacy1xImportVersion, "default legacy import version")
        AssertEqual(
            UserPreferencesSnapshot.ImportOutcomePending,
            defaults.Legacy1xImportOutcome,
            "default legacy import outcome")

        defaults.ShowMessages = False
        defaults.UpdatePolicy = UpdateCheckPolicy.Weekly
        defaults.LastUpdateCheck = New DateTime(2026, 8, 4, 1, 2, 3, DateTimeKind.Utc)
        defaults.Legacy1xImportVersion = UserPreferencesSnapshot.CurrentLegacyImportVersion
        defaults.Legacy1xImportOutcome = UserPreferencesSnapshot.ImportOutcomeImported

        Dim clone As UserPreferencesSnapshot = defaults.Clone()
        AssertEqual(False, Object.ReferenceEquals(defaults, clone), "clone identity")
        AssertEqual(defaults.ShowMessages, clone.ShowMessages, "cloned message preference")
        AssertEqual(defaults.DefaultDirectory, clone.DefaultDirectory, "cloned directory")
        AssertEqual(defaults.UpdatePolicy, clone.UpdatePolicy, "cloned update policy")
        AssertEqual(defaults.LastUpdateCheck, clone.LastUpdateCheck, "cloned update date")
        AssertEqual(
            defaults.Legacy1xImportVersion,
            clone.Legacy1xImportVersion,
            "cloned legacy import version")
        AssertEqual(
            defaults.Legacy1xImportOutcome,
            clone.Legacy1xImportOutcome,
            "cloned legacy import outcome")

        clone.DefaultDirectory = "D:\Changed"
        clone.Legacy1xImportOutcome = UserPreferencesSnapshot.ImportOutcomeNotFound
        AssertEqual(
            "C:\Users\Jules\Documents",
            defaults.DefaultDirectory,
            "clone directory isolation")
        AssertEqual(
            UserPreferencesSnapshot.ImportOutcomeImported,
            defaults.Legacy1xImportOutcome,
            "clone import outcome isolation")

        Dim nullPathDefaults As UserPreferencesSnapshot =
            UserPreferencesSnapshot.CreateDefaults(Nothing)
        AssertEqual(String.Empty, nullPathDefaults.DefaultDirectory, "null default directory")
    End Sub

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Class
