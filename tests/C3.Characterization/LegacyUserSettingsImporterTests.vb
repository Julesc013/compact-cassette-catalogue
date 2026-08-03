Friend NotInheritable Class LegacyUserSettingsImporterTests

    Private Const TruncatedApplicationRoot As String = "Compact_Cassette_Catalogu"
    Private Const EvidenceDirectory As String =
        "Compact_Cassette_Catalogu_Url_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"

    Private Sub New()
    End Sub

    Public Shared Sub AbsenceIsNotFoundButDiscoveryIoFailurePropagates()
        WithTemporaryDirectory(
            "absent",
            Sub(localApplicationData As String)
                Dim result As LegacyUserSettingsImportResult =
                    New LegacyUserSettingsImporter().Import(localApplicationData)
                AssertEqual(
                    LegacyUserSettingsImportStatus.NotFound,
                    result.Status,
                    "absent legacy roots")
            End Sub)

        WithTemporaryDirectory(
            "discovery-io",
            Sub(localApplicationData As String)
                File.WriteAllText(
                    Path.Combine(localApplicationData, TruncatedApplicationRoot),
                    "This path deliberately prevents directory enumeration.")

                AssertThrowsIOException(
                    Sub()
                        Dim ignored As LegacyUserSettingsImportResult =
                            New LegacyUserSettingsImporter().Import(localApplicationData)
                    End Sub,
                    "non-directory legacy root")
            End Sub)
    End Sub

    Public Shared Sub InvalidNewerContentFallsBackWithEvidence()
        WithTemporaryDirectory(
            "invalid-fallback",
            Sub(localApplicationData As String)
                WriteProfile(
                    localApplicationData,
                    "1.2.0.0",
                    CreateProfileXml("not-a-Boolean", "never"))
                WriteProfile(
                    localApplicationData,
                    "0.0.0.0",
                    CreateProfileXml("True", "startup"))

                Dim result As LegacyUserSettingsImportResult =
                    New LegacyUserSettingsImporter().Import(localApplicationData)

                AssertEqual(
                    LegacyUserSettingsImportStatus.Imported,
                    result.Status,
                    "fallback import status")
                AssertEqual(
                    "0.0.0.0",
                    result.Profile.SourceVersion.ToString(),
                    "fallback selected version")
                AssertEqual(1, result.RejectedCandidates.Count, "fallback evidence count")
                AssertEqual(
                    LegacySettingsProfileReadFailure.InvalidValue,
                    result.RejectedCandidates.Item(0).Failure,
                    "fallback evidence classification")
            End Sub)
    End Sub

    Public Shared Sub UnavailableNewerContentStopsFallback()
        WithTemporaryDirectory(
            "unavailable-stop",
            Sub(localApplicationData As String)
                Dim newerPath As String = WriteProfile(
                    localApplicationData,
                    "1.2.0.0",
                    CreateProfileXml("False", "never"))
                WriteProfile(
                    localApplicationData,
                    "0.0.0.0",
                    CreateProfileXml("True", "startup"))

                Using heldOpen As New FileStream(
                        newerPath,
                        FileMode.Open,
                        FileAccess.ReadWrite,
                        FileShare.None)

                    AssertThrowsIOException(
                        Sub()
                            Dim ignored As LegacyUserSettingsImportResult =
                                New LegacyUserSettingsImporter().Import(localApplicationData)
                        End Sub,
                        "locked newest legacy profile")
                End Using
            End Sub)
    End Sub

    Private Shared Function WriteProfile(
            localApplicationData As String,
            version As String,
            content As String) As String

        Dim profileDirectory As String = Path.Combine(
            localApplicationData,
            TruncatedApplicationRoot,
            EvidenceDirectory,
            version)
        Directory.CreateDirectory(profileDirectory)

        Dim profilePath As String = Path.Combine(profileDirectory, "user.config")
        File.WriteAllText(profilePath, content)
        Return profilePath
    End Function

    Private Shared Function CreateProfileXml(
            showMessages As String,
            checkUpdates As String) As String

        Return "<?xml version=""1.0"" encoding=""utf-8""?>" &
            "<configuration><userSettings>" &
            "<Compact_Cassette_Catalogue.My.MySettings>" &
            "<setting name=""showMessages"" serializeAs=""String"">" &
            "<value>" & showMessages & "</value></setting>" &
            "<setting name=""checkUpdates"" serializeAs=""String"">" &
            "<value>" & checkUpdates & "</value></setting>" &
            "</Compact_Cassette_Catalogue.My.MySettings>" &
            "</userSettings></configuration>"
    End Function

    Private Shared Sub WithTemporaryDirectory(name As String, action As Action(Of String))
        Dim temporaryDirectory As String = Path.Combine(
            Path.GetTempPath(),
            "C3-LegacySettingsImporterTests",
            name & "-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(temporaryDirectory)
        Try
            action(temporaryDirectory)
        Finally
            If Directory.Exists(temporaryDirectory) Then
                Directory.Delete(temporaryDirectory, True)
            End If
        End Try
    End Sub

    Private Shared Sub AssertThrowsIOException(action As Action, name As String)
        Try
            action()
        Catch ex As IOException
            Return
        End Try
        Throw New InvalidOperationException(name & " did not throw IOException.")
    End Sub

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Class
