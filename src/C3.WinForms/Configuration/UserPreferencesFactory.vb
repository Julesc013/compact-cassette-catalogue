Friend NotInheritable Class UserPreferencesFactory

    Private Sub New()
    End Sub

    Public Shared Function CreateDefault() As UserPreferencesService
        Dim localApplicationData As String = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData)
        Dim preferencesPath As String = System.IO.Path.Combine(
            localApplicationData,
            "Jules Carboni",
            "C3",
            "2",
            "preferences.xml")
        Dim store As New XmlUserPreferencesStore(
            preferencesPath,
            Function() DateTime.UtcNow)

        Return New UserPreferencesService(
            store,
            New LegacyUserSettingsImporter(),
            localApplicationData,
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            AddressOf BufferedLogger.Warning)
    End Function

End Class
