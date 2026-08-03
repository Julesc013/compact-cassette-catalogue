Friend NotInheritable Class LegacySettingsProfileLocatorTests

    Private Const TruncatedRoot As String = "Compact_Cassette_Catalogu"
    Private Const FullRoot As String = "Compact_Cassette_Catalogue"
    Private Const EvidenceHash As String = "abcdefghijklmnopqrstuvwxyz012345"

    Private Sub New()
    End Sub

    Public Shared Sub AcceptsExactKnownPathsAndOrdersNewestFirst()
        WithTemporaryDirectory(
            "known-paths",
            Sub(localRoot As String)
                Dim oldest As String = WriteProfile(
                    localRoot,
                    TruncatedRoot,
                    EvidenceName("StrongName"),
                    "0.0.0.0")
                Dim middle As String = WriteProfile(
                    localRoot,
                    TruncatedRoot,
                    EvidenceName("Url"),
                    "1.1.1.0")
                Dim newest As String = WriteProfile(
                    localRoot,
                    FullRoot,
                    EvidenceName("Path"),
                    "1.2.0.0")

                Dim candidates As IList(Of LegacySettingsProfileCandidate) =
                    New LegacySettingsProfileLocator().Locate(localRoot)
                AssertEqual(3, candidates.Count, "known candidate count")
                AssertEqual(Path.GetFullPath(newest), candidates.Item(0).FilePath, "newest path")
                AssertEqual(New Version(1, 2, 0, 0), candidates.Item(0).ProfileVersion, "newest version")
                AssertEqual(Path.GetFullPath(middle), candidates.Item(1).FilePath, "middle path")
                AssertEqual(Path.GetFullPath(oldest), candidates.Item(2).FilePath, "oldest path")
            End Sub)
    End Sub

    Public Shared Sub RejectsUntrustedFullAndDeepLookalikes()
        WithTemporaryDirectory(
            "rejected-paths",
            Sub(localRoot As String)
                Dim accepted As String = WriteProfile(
                    localRoot,
                    TruncatedRoot,
                    EvidenceName("Url"),
                    "1.2.0.0")

                WriteProfile(
                    localRoot,
                    TruncatedRoot,
                    FullRoot & "_Url_" & EvidenceHash,
                    "1.2.0.0")
                WriteProfile(
                    localRoot,
                    TruncatedRoot,
                    TruncatedRoot & "_Zone_" & EvidenceHash,
                    "1.2.0.0")
                WriteProfile(
                    localRoot,
                    TruncatedRoot,
                    TruncatedRoot & "_Url_" & EvidenceHash.Substring(0, 31) & "6",
                    "1.2.0.0")
                WriteProfile(
                    localRoot,
                    "Untrusted_Publisher",
                    EvidenceName("Url"),
                    "1.2.0.0")
                WriteProfile(
                    localRoot,
                    TruncatedRoot,
                    EvidenceName("Path"),
                    "2.0.0.0")
                WriteProfile(
                    localRoot,
                    TruncatedRoot,
                    EvidenceName("StrongName"),
                    "1.2.0")
                WriteProfile(
                    Path.Combine(localRoot, TruncatedRoot, "unexpected-depth"),
                    String.Empty,
                    EvidenceName("Url"),
                    "1.1.2.0")

                Dim candidates As IList(Of LegacySettingsProfileCandidate) =
                    New LegacySettingsProfileLocator().Locate(localRoot)
                AssertEqual(1, candidates.Count, "rejected lookalike count")
                AssertEqual(Path.GetFullPath(accepted), candidates.Item(0).FilePath, "accepted exact path")
            End Sub)
    End Sub

    Private Shared Function EvidenceName(evidenceType As String) As String
        Return TruncatedRoot & "_" & evidenceType & "_" & EvidenceHash
    End Function

    Private Shared Function WriteProfile(
            localRoot As String,
            applicationRoot As String,
            evidenceDirectory As String,
            version As String) As String

        Dim profileDirectory As String = Path.Combine(
            localRoot,
            applicationRoot,
            evidenceDirectory,
            version)
        Directory.CreateDirectory(profileDirectory)
        Dim profilePath As String = Path.Combine(profileDirectory, "user.config")
        File.WriteAllText(profilePath, "<configuration />")
        Return profilePath
    End Function

    Private Shared Sub WithTemporaryDirectory(name As String, action As Action(Of String))
        Dim temporaryPath As String = Path.Combine(
            Path.GetTempPath(),
            "C3-LegacySettingsProfileLocatorTests",
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

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Class
