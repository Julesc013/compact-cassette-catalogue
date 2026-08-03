Friend NotInheritable Class LegacySettingsProfileReaderTests

    Private Sub New()
    End Sub

    Public Shared Sub ReadsBooleanAndStringSchemasWithoutChangingSources()
        Dim repositoryRoot As String = FindRepositoryRoot()
        Dim booleanPath As String = Path.Combine(
            repositoryRoot,
            "fixtures\settings\legacy\v1.1.1\user.config")
        Dim stringPath As String = Path.Combine(
            repositoryRoot,
            "fixtures\settings\legacy\v1.2.0-beta.1\user.config")
        Dim booleanBytes As Byte() = File.ReadAllBytes(booleanPath)
        Dim stringBytes As Byte() = File.ReadAllBytes(stringPath)
        Dim reader As New LegacySettingsProfileReader()

        Dim booleanResult As LegacySettingsProfileReadResult = reader.Read(
            New LegacySettingsProfileCandidate(
                booleanPath,
                New Version(1, 1, 1, 0),
                File.GetLastWriteTimeUtc(booleanPath)))
        AssertEqual(True, booleanResult.IsSuccess, "Boolean profile read")
        AssertEqual(True, booleanResult.Profile.HasShowMessages, "Boolean showMessages presence")
        AssertEqual(True, booleanResult.Profile.ShowMessages, "Boolean showMessages value")
        AssertEqual(UpdateCheckPolicy.Startup, booleanResult.Profile.UpdatePolicy, "Boolean policy")
        AssertEqual(False, booleanResult.Profile.HasLastUpdateCheck, "Boolean date absence")

        Dim stringResult As LegacySettingsProfileReadResult = reader.Read(
            New LegacySettingsProfileCandidate(
                stringPath,
                New Version(1, 2, 0, 0),
                File.GetLastWriteTimeUtc(stringPath)))
        AssertEqual(True, stringResult.IsSuccess, "String profile read")
        AssertEqual(UpdateCheckPolicy.Never, stringResult.Profile.UpdatePolicy, "String policy")
        AssertEqual(True, stringResult.Profile.HasLastUpdateCheck, "String date presence")
        AssertEqual(DateTime.MinValue, stringResult.Profile.LastUpdateCheck, "empty String date")

        AssertBytesEqual(booleanBytes, File.ReadAllBytes(booleanPath), "Boolean source bytes")
        AssertBytesEqual(stringBytes, File.ReadAllBytes(stringPath), "String source bytes")
    End Sub

    Public Shared Sub RejectsNestedAndOversizedValues()
        WithTemporaryDirectory(
            "invalid-values",
            Sub(workDirectory As String)
                Dim nestedPath As String = Path.Combine(workDirectory, "nested.config")
                File.WriteAllText(
                    nestedPath,
                    "<configuration><userSettings>" &
                        "<Compact_Cassette_Catalogue.My.MySettings>" &
                        "<setting name=""showMessages"" serializeAs=""String"">" &
                        "<value><nested>true</nested></value></setting>" &
                        "</Compact_Cassette_Catalogue.My.MySettings>" &
                        "</userSettings></configuration>")
                Dim nested As LegacySettingsProfileReadResult =
                    New LegacySettingsProfileReader().Read(CreateCandidate(nestedPath))
                AssertEqual(False, nested.IsSuccess, "nested value success")
                AssertEqual(
                    LegacySettingsProfileReadFailure.InvalidValue,
                    nested.Failure,
                    "nested value failure")

                Dim oversizedPath As String = Path.Combine(workDirectory, "oversized.config")
                File.WriteAllText(
                    oversizedPath,
                    New String("x"c, CInt(LegacySettingsProfileReader.MaximumProfileBytes + 1L)),
                    New UTF8Encoding(False))
                Dim oversized As LegacySettingsProfileReadResult =
                    New LegacySettingsProfileReader().Read(CreateCandidate(oversizedPath))
                AssertEqual(False, oversized.IsSuccess, "oversized profile success")
                AssertEqual(
                    LegacySettingsProfileReadFailure.TooLarge,
                    oversized.Failure,
                    "oversized profile failure")

                Dim oversizedDirectoryPath As String = Path.Combine(
                    workDirectory,
                    "oversized-directory.config")
                File.WriteAllText(
                    oversizedDirectoryPath,
                    "<configuration><userSettings>" &
                        "<Compact_Cassette_Catalogue.My.MySettings>" &
                        "<setting name=""defaultDirectory"" serializeAs=""String""><value>" &
                        New String(
                            "d"c,
                            UserPreferencesSnapshot.MaximumDefaultDirectoryCharacters + 1) &
                        "</value></setting>" &
                        "</Compact_Cassette_Catalogue.My.MySettings>" &
                        "</userSettings></configuration>",
                    New UTF8Encoding(False))
                Dim oversizedDirectory As LegacySettingsProfileReadResult =
                    New LegacySettingsProfileReader().Read(CreateCandidate(oversizedDirectoryPath))
                AssertEqual(False, oversizedDirectory.IsSuccess, "oversized directory success")
                AssertEqual(
                    LegacySettingsProfileReadFailure.InvalidValue,
                    oversizedDirectory.Failure,
                    "oversized directory failure")
            End Sub)
    End Sub

    Private Shared Function CreateCandidate(path As String) As LegacySettingsProfileCandidate
        Return New LegacySettingsProfileCandidate(
            path,
            New Version(1, 2, 0, 0),
            File.GetLastWriteTimeUtc(path))
    End Function

    Private Shared Function FindRepositoryRoot() As String
        Dim directory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While directory IsNot Nothing
            If File.Exists(Path.Combine(directory.FullName, "VERSION")) AndAlso
                    System.IO.Directory.Exists(
                        Path.Combine(directory.FullName, "fixtures\settings")) Then
                Return directory.FullName
            End If
            directory = directory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Shared Sub WithTemporaryDirectory(name As String, action As Action(Of String))
        Dim temporaryPath As String = Path.Combine(
            Path.GetTempPath(),
            "C3-LegacySettingsProfileReaderTests",
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
