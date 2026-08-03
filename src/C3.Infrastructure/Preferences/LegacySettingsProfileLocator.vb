Imports System.Collections.Generic
Imports System.IO

Namespace Preferences

    ''' <summary>
    ''' Locates C3 1.x settings at the exact directory depth used by the
    ''' .NET Framework local settings provider. It never scans LocalAppData
    ''' recursively or follows directory reparse points.
    ''' </summary>
    Public NotInheritable Class LegacySettingsProfileLocator

        ' Every public 1.x tag has an empty AssemblyCompany and the
        ' Compact_Cassette_Catalogue root namespace. .NET Framework normally
        ' truncates that component to 25 characters; retain the untruncated
        ' observed-compatible spelling as the only additional root.
        Private Shared ReadOnly KnownApplicationRootNames As String() = {
            "Compact_Cassette_Catalogu",
            "Compact_Cassette_Catalogue"
        }

        ' ClientConfigPaths applies the same 25-character limit to the validated
        ' AppDomain friendly name before appending its evidence suffix.
        Private Shared ReadOnly KnownEvidenceNameStems As String() = {
            "Compact_Cassette_Catalogu"
        }

        Private Shared ReadOnly KnownEvidenceTypes As String() = {
            "Url",
            "Path",
            "StrongName"
        }

        Private Const EvidenceHashLength As Integer = 32

        Public Function Locate() As IList(Of LegacySettingsProfileCandidate)
            Return Locate(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
        End Function

        ''' <summary>
        ''' Uses an injected LocalAppData directory so discovery can be tested
        ''' without reading or changing a real Windows profile.
        ''' </summary>
        Public Function Locate(
                localApplicationDataDirectory As String) As IList(Of LegacySettingsProfileCandidate)

            If String.IsNullOrWhiteSpace(localApplicationDataDirectory) Then
                Throw New ArgumentException(
                    "A LocalApplicationData directory is required.",
                    "localApplicationDataDirectory")
            End If

            Dim localRoot As String = Path.GetFullPath(localApplicationDataDirectory)
            Dim candidates As New List(Of LegacySettingsProfileCandidate)()
            Dim observedPaths As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each applicationRootName As String In KnownApplicationRootNames
                Dim applicationRoot As String = Path.Combine(localRoot, applicationRootName)
                For Each evidenceDirectory As DirectoryInfo In GetDirectoriesIfPresent(applicationRoot)
                    If Not IsKnownEvidenceDirectory(evidenceDirectory.Name) Then
                        Continue For
                    End If
                    If Not IsSafeDirectoryPresent(evidenceDirectory) Then
                        Continue For
                    End If

                    For Each versionDirectory As DirectoryInfo In GetDirectoriesIfPresent(
                            evidenceDirectory.FullName)
                        If Not IsSafeDirectoryPresent(versionDirectory) Then
                            Continue For
                        End If

                        Dim profileVersion As Version = Nothing
                        If Not IsLegacyVersionDirectory(versionDirectory.Name, profileVersion) Then
                            Continue For
                        End If

                        Dim settingsPath As String = Path.Combine(
                            versionDirectory.FullName,
                            "user.config")
                        If Not observedPaths.Add(settingsPath) Then
                            Continue For
                        End If

                        Dim lastWriteTimeUtc As DateTime
                        If TryGetSettingsFile(settingsPath, lastWriteTimeUtc) Then
                            candidates.Add(
                                New LegacySettingsProfileCandidate(
                                    settingsPath,
                                    profileVersion,
                                    lastWriteTimeUtc))
                        End If
                    Next
                Next
            Next

            candidates.Sort(New CandidateComparer())
            Return candidates.AsReadOnly()
        End Function

        Private Shared Function GetDirectoriesIfPresent(path As String) As DirectoryInfo()
            Try
                Dim attributes As FileAttributes = File.GetAttributes(path)
                If (attributes And FileAttributes.Directory) = 0 Then
                    Throw New IOException("The expected legacy settings root is not a directory: " & path)
                End If
                If (attributes And FileAttributes.ReparsePoint) <> 0 Then
                    Throw New IOException("A legacy settings directory is a reparse point: " & path)
                End If
                Return New DirectoryInfo(path).GetDirectories()
            Catch ex As FileNotFoundException
                Return New DirectoryInfo() {}
            Catch ex As DirectoryNotFoundException
                Return New DirectoryInfo() {}
            End Try
        End Function

        Private Shared Function TryGetSettingsFile(
                path As String,
                ByRef lastWriteTimeUtc As DateTime) As Boolean

            Try
                Dim attributes As FileAttributes = File.GetAttributes(path)
                If (attributes And FileAttributes.Directory) <> 0 Then
                    Throw New IOException("The legacy settings profile path is a directory: " & path)
                End If
                If (attributes And FileAttributes.ReparsePoint) <> 0 Then
                    Throw New IOException("The legacy settings profile is a reparse point: " & path)
                End If

                lastWriteTimeUtc = File.GetLastWriteTimeUtc(path)
                Return True
            Catch ex As FileNotFoundException
                Return False
            Catch ex As DirectoryNotFoundException
                Return False
            End Try
        End Function

        Private Shared Function IsKnownEvidenceDirectory(name As String) As Boolean
            For Each stem As String In KnownEvidenceNameStems
                For Each evidenceType As String In KnownEvidenceTypes
                    Dim prefix As String = stem & "_" & evidenceType & "_"
                    If name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) AndAlso
                            name.Length = prefix.Length + EvidenceHashLength AndAlso
                            IsFrameworkEvidenceHash(name.Substring(prefix.Length)) Then
                        Return True
                    End If
                Next
            Next
            Return False
        End Function

        Private Shared Function IsFrameworkEvidenceHash(value As String) As Boolean
            For Each character As Char In value
                Dim normalized As Char = Char.ToLowerInvariant(character)
                If (normalized < "a"c OrElse normalized > "z"c) AndAlso
                        (normalized < "0"c OrElse normalized > "5"c) Then
                    Return False
                End If
            Next
            Return value.Length = EvidenceHashLength
        End Function

        Private Shared Function IsLegacyVersionDirectory(
                directoryName As String,
                ByRef profileVersion As Version) As Boolean

            If Not Version.TryParse(directoryName, profileVersion) OrElse
                    profileVersion.Build < 0 OrElse
                    profileVersion.Revision < 0 Then
                profileVersion = Nothing
                Return False
            End If

            ' Public C3 1.x profiles used assembly versions 0.0.0.0 and 1.x.
            If profileVersion.Major < 0 OrElse profileVersion.Major > 1 Then
                profileVersion = Nothing
                Return False
            End If
            Return True
        End Function

        Private Shared Function IsSafeDirectoryPresent(
                fileSystemInfo As FileSystemInfo) As Boolean

            Try
                If (fileSystemInfo.Attributes And FileAttributes.ReparsePoint) <> 0 Then
                    Throw New IOException(
                        "A legacy settings directory is a reparse point: " &
                            fileSystemInfo.FullName)
                End If
                Return True
            Catch ex As FileNotFoundException
                Return False
            Catch ex As DirectoryNotFoundException
                Return False
            End Try
        End Function

        Private NotInheritable Class CandidateComparer
            Implements IComparer(Of LegacySettingsProfileCandidate)

            Public Function Compare(
                    left As LegacySettingsProfileCandidate,
                    right As LegacySettingsProfileCandidate) As Integer _
                    Implements IComparer(Of LegacySettingsProfileCandidate).Compare

                If Object.ReferenceEquals(left, right) Then
                    Return 0
                End If
                If left Is Nothing Then
                    Return 1
                End If
                If right Is Nothing Then
                    Return -1
                End If

                Dim byVersion As Integer = right.ProfileVersion.CompareTo(left.ProfileVersion)
                If byVersion <> 0 Then
                    Return byVersion
                End If

                Dim byWriteTime As Integer = right.LastWriteTimeUtc.CompareTo(left.LastWriteTimeUtc)
                If byWriteTime <> 0 Then
                    Return byWriteTime
                End If

                Return StringComparer.OrdinalIgnoreCase.Compare(left.FilePath, right.FilePath)
            End Function
        End Class

    End Class

End Namespace
