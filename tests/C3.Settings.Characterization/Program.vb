Module Program

    Private _failures As Integer
    Private _repositoryRoot As String

    Sub Main()
        _repositoryRoot = FindRepositoryRoot()

        RunTest("settings schema carries a durable retry marker", AddressOf SettingsSchemaCarriesRetryMarker)
        RunTest("startup migration is guarded and retryable", AddressOf StartupMigrationIsGuardedAndRetryable)
        RunTest("known historical update policies have explicit normalization", AddressOf HistoricalUpdatePoliciesAreNormalized)
        RunTest("migration leaves message and directory preferences intact", AddressOf MigrationPreservesUnrelatedPreferences)
        RunTest("console export resolves a safe configured path", AddressOf ConsoleExportResolvesSafePath)
        RunTest("diagnostic and update failure paths are nonfatal", AddressOf DiagnosticFailuresAreNonfatal)

        If _failures > 0 Then
            Console.Error.WriteLine("{0} settings characterization test(s) failed.", _failures)
            Environment.ExitCode = 1
            Return
        End If

        Console.WriteLine("All C3 settings characterization tests passed.")
    End Sub

    Private Sub RunTest(name As String, test As Action)
        Try
            test()
            Console.WriteLine("PASS: " & name)
        Catch ex As Exception
            _failures += 1
            Console.Error.WriteLine("FAIL: {0}{1}{2}", name, Environment.NewLine, ex.Message)
        End Try
    End Sub

    Private Sub SettingsSchemaCarriesRetryMarker()
        Dim settings As New XmlDocument()
        settings.Load(RepositoryPath("Compact Cassette Catalogue", "My Project", "Settings.settings"))
        Dim namespaceManager As New XmlNamespaceManager(settings.NameTable)
        namespaceManager.AddNamespace("s", "http://schemas.microsoft.com/VisualStudio/2004/01/settings")
        Dim marker As XmlElement = TryCast(settings.SelectSingleNode(
            "/s:SettingsFile/s:Settings/s:Setting[@Name='settingsUpgradeRequired']", namespaceManager), XmlElement)

        AssertNotNothing(marker, "settingsUpgradeRequired setting")
        AssertEqual("System.Boolean", marker.GetAttribute("Type"), "retry marker type")
        AssertEqual("User", marker.GetAttribute("Scope"), "retry marker scope")
        AssertEqual("True", marker.SelectSingleNode("s:Value", namespaceManager).InnerText, "retry marker default")
    End Sub

    Private Sub StartupMigrationIsGuardedAndRetryable()
        Dim source As String = File.ReadAllText(RepositoryPath("Compact Cassette Catalogue", "ApplicationEvents.vb"))

        AssertContains(source, "Handles Me.Startup", "startup event wiring")
        AssertContains(source, "If Not My.Settings.settingsUpgradeRequired Then", "migration guard")
        AssertContains(source, "My.Settings.Upgrade()", "standard profile upgrade")
        AssertContains(source, "My.Settings.settingsUpgradeRequired = False", "successful disarm")
        AssertContains(source, "My.Settings.Save()", "durable migration save")
        AssertContains(source, "My.Settings.settingsUpgradeRequired = True", "failure retry rearm")
        AssertContains(source, "Settings Migration Failed", "visible failure notice")

        Dim upgradeIndex As Integer = source.IndexOf("My.Settings.Upgrade()", StringComparison.Ordinal)
        Dim disarmIndex As Integer = source.IndexOf("My.Settings.settingsUpgradeRequired = False", StringComparison.Ordinal)
        Dim saveIndex As Integer = source.IndexOf("My.Settings.Save()", StringComparison.Ordinal)
        AssertTrue(upgradeIndex < disarmIndex AndAlso disarmIndex < saveIndex,
            "migration must upgrade, disarm, then durably save in that order")
    End Sub

    Private Sub HistoricalUpdatePoliciesAreNormalized()
        Dim source As String = File.ReadAllText(RepositoryPath("Compact Cassette Catalogue", "ApplicationEvents.vb"))
        For Each expectedCase As String In New String() {
                "Case ""startup"", ""true""",
                "Case ""weekly""",
                "Case ""monthly""",
                "Case ""never"", ""manually"", ""false"""}
            AssertContains(source, expectedCase, "known update-policy normalization")
        Next
        AssertContains(source, "Return ""never""", "safe unknown update policy")

        AssertEqual("True", ReadFixtureValue("v1.1.1", "checkUpdates"), "v1.1.1 Boolean policy fixture")
        AssertEqual("startup", ReadFixtureValue("v1.1.2", "checkUpdates"), "v1.1.2 schedule fixture")
        AssertEqual("never", ReadFixtureValue("v1.2.0-beta.1", "checkUpdates"), "v1.2 policy fixture")
    End Sub

    Private Sub MigrationPreservesUnrelatedPreferences()
        Dim source As String = File.ReadAllText(RepositoryPath("Compact Cassette Catalogue", "ApplicationEvents.vb"))
        AssertDoesNotContain(source, "My.Settings.showMessages =", "showMessages migration assignment")
        AssertDoesNotContain(source, "My.Settings.lastUpdateCheck =", "lastUpdateCheck migration assignment")
        AssertContains(source, "normaliseMigratedDirectory(My.Settings.defaultDirectory)", "directory sentinel normalization")
    End Sub

    Private Sub ConsoleExportResolvesSafePath()
        Dim mainSource As String = File.ReadAllText(RepositoryPath("Compact Cassette Catalogue", "frmMain.vb"))
        Dim globalSource As String = File.ReadAllText(RepositoryPath("Compact Cassette Catalogue", "varGlobals.vb"))

        AssertContains(globalSource,
            "Function resolveConsoleOutputDirectory(configuredDirectory As String) As String",
            "console output directory resolver")
        AssertContains(globalSource, "Directory.Exists(configuredDirectory)", "configured directory validation")
        AssertContains(globalSource,
            "My.Computer.FileSystem.SpecialDirectories.MyDocuments",
            "Documents fallback")
        AssertContains(mainSource,
            "resolveConsoleOutputDirectory(My.Settings.defaultDirectory)",
            "configured export directory")
        AssertContains(mainSource,
            "Path.Combine(outputDirectory, outputName)",
            "path-safe export filename")
        AssertDoesNotContain(mainSource, "fileDirectory & outputName", "catalogue-relative export path")
    End Sub

    Private Sub DiagnosticFailuresAreNonfatal()
        Dim mainSource As String = File.ReadAllText(RepositoryPath("Compact Cassette Catalogue", "frmMain.vb"))
        Dim globalSource As String = File.ReadAllText(RepositoryPath("Compact Cassette Catalogue", "varGlobals.vb"))
        Dim consoleSource As String = File.ReadAllText(RepositoryPath("Compact Cassette Catalogue", "frmConsole.vb"))

        AssertContains(globalSource, "Sub showNonfatalMessage(", "nonfatal message helper")
        AssertContains(globalSource, "Function showNonfatalQuestion(", "nonfatal question helper")
        AssertContains(globalSource, "Debug.WriteLine", "headless diagnostic fallback")
        AssertContains(globalSource, "showNonfatalMessage(", "browser failure notice")
        AssertContains(mainSource, "showNonfatalQuestion(boxMessage, boxTitle)", "update-check failure question")
        AssertContains(mainSource, "Failed to output console to log file.", "console write failure")
        AssertContains(mainSource, "showNonfatalMessage(message & messageDetails", "nonfatal console result notice")
        AssertContains(consoleSource, "CStr(folderpath)", "reported configured directory")
    End Sub

    Private Function ReadFixtureValue(version As String, settingName As String) As String
        Dim fixture As New XmlDocument()
        fixture.Load(RepositoryPath("fixtures", "settings", "legacy", version, "user.config"))
        Dim node As XmlNode = fixture.SelectSingleNode(
            "/configuration/userSettings/Compact_Cassette_Catalogue.My.MySettings/setting[@name='" &
            settingName & "']/value")
        AssertNotNothing(node, version & " " & settingName)
        Return node.InnerText
    End Function

    Private Function RepositoryPath(ParamArray segments As String()) As String
        Dim result As String = _repositoryRoot
        For Each segment As String In segments
            result = Path.Combine(result, segment)
        Next
        Return result
    End Function

    Private Function FindRepositoryRoot() As String
        Dim directory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While directory IsNot Nothing
            If System.IO.File.Exists(Path.Combine(directory.FullName, "VERSION")) AndAlso
                    System.IO.Directory.Exists(Path.Combine(directory.FullName, "fixtures\settings")) Then
                Return directory.FullName
            End If
            directory = directory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Sub AssertContains(source As String, expected As String, name As String)
        If source.IndexOf(expected, StringComparison.Ordinal) < 0 Then
            Throw New InvalidOperationException(name & " is missing: " & expected)
        End If
    End Sub

    Private Sub AssertDoesNotContain(source As String, unexpected As String, name As String)
        If source.IndexOf(unexpected, StringComparison.Ordinal) >= 0 Then
            Throw New InvalidOperationException(name & " must not be present: " & unexpected)
        End If
    End Sub

    Private Sub AssertNotNothing(value As Object, name As String)
        If value Is Nothing Then
            Throw New InvalidOperationException(name & " is missing.")
        End If
    End Sub

    Private Sub AssertTrue(value As Boolean, message As String)
        If Not value Then
            Throw New InvalidOperationException(message)
        End If
    End Sub

    Private Sub AssertEqual(expected As String, actual As String, name As String)
        If Not String.Equals(expected, actual, StringComparison.Ordinal) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
