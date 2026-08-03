Friend NotInheritable Class PreferenceFormatContractTests

    Private Sub New()
    End Sub

    Public Shared Sub CanonicalExampleMatchesSchemaAndRuntime()
        Dim repositoryRoot As String = FindRepositoryRoot()
        Dim examplePath As String = Path.Combine(
            repositoryRoot,
            "spec\preferences\v1\example.xml")
        Dim schemaPath As String = Path.Combine(
            repositoryRoot,
            "spec\preferences\v1\preferences.xsd")
        Dim validationMessages As New List(Of String)()
        Dim settings As New XmlReaderSettings() With {
            .DtdProcessing = DtdProcessing.Prohibit,
            .XmlResolver = Nothing,
            .ValidationType = ValidationType.Schema,
            .ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
        }
        settings.Schemas.Add(Nothing, schemaPath)
        AddHandler settings.ValidationEventHandler,
            Sub(sender As Object, args As ValidationEventArgs)
                validationMessages.Add(args.Message)
            End Sub

        Using reader As XmlReader = XmlReader.Create(examplePath, settings)
            While reader.Read()
            End While
        End Using
        If validationMessages.Count > 0 Then
            Throw New InvalidOperationException(
                String.Join(Environment.NewLine, validationMessages.ToArray()))
        End If

        Dim temporaryDirectory As String = Path.Combine(
            Path.GetTempPath(),
            "C3-PreferenceFormatContractTests",
            Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(temporaryDirectory)
        Try
            Dim copiedExample As String = Path.Combine(temporaryDirectory, "preferences.xml")
            File.Copy(examplePath, copiedExample)
            Dim result As UserPreferencesLoadResult =
                New XmlUserPreferencesStore(
                    copiedExample,
                    Function() DateTime.UtcNow).Load()
            AssertEqual(True, result.IsSuccess, "canonical example runtime load")
            AssertEqual(True, result.Preferences.ShowMessages, "canonical message preference")
            AssertEqual("C:\Catalogues", result.Preferences.DefaultDirectory, "canonical directory")
            AssertEqual(UpdateCheckPolicy.Never, result.Preferences.UpdatePolicy, "canonical policy")
            AssertEqual(
                UserPreferencesSnapshot.ImportOutcomeNotFound,
                result.Preferences.Legacy1xImportOutcome,
                "canonical import outcome")
        Finally
            If Directory.Exists(temporaryDirectory) Then
                Directory.Delete(temporaryDirectory, True)
            End If
        End Try
    End Sub

    Private Shared Function FindRepositoryRoot() As String
        Dim directory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While directory IsNot Nothing
            If File.Exists(Path.Combine(directory.FullName, "C3.sln")) AndAlso
                    System.IO.Directory.Exists(
                        Path.Combine(directory.FullName, "spec\preferences")) Then
                Return directory.FullName
            End If
            directory = directory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Class
