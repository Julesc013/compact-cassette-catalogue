Imports C3.Infrastructure.CatalogueFiles.Xml.V2_0
Imports C3.Infrastructure.Migrations.V1_1ToV2_0

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
End Class
