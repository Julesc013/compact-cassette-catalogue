Module Program

    Private _failures As Integer
    Private ReadOnly PayloadNames As String() = {
        "BUILD.txt",
        "Compact Cassette Catalogue.exe",
        "Compact Cassette Catalogue.exe.config",
        "README.txt",
        "RELEASE_NOTES.txt",
        "UNINSTALL.exe",
        "UNINSTALL.exe.config"
    }

    Sub Main()
        Dim commandLine As String() = Environment.GetCommandLineArgs()
        If commandLine.Length > 1 AndAlso commandLine(1) = "--journal-crash-child" Then
            RunJournalCrashChild(commandLine)
            Return
        End If
        RunTest("valid closed payload passes", AddressOf ValidClosedPayloadPasses)
        RunTest("altered payload file is rejected", AddressOf AlteredPayloadIsRejected)
        RunTest("unexpected payload file is rejected", AddressOf UnexpectedPayloadIsRejected)
        RunTest("traversal payload name is rejected", AddressOf TraversalNameIsRejected)
        RunTest("DTD payload manifest is rejected", AddressOf DtdIsRejected)
        RunTest("noncanonical SHA-256 is rejected", AddressOf NoncanonicalHashIsRejected)
        RunTest("volume root is rejected as install root", AddressOf VolumeRootIsRejected)
        RunTest("ordinary local descendant is accepted", AddressOf OrdinaryDescendantIsAccepted)
        RunTest("installed-state manifest round-trips", AddressOf InstalledStateRoundTrips)
        RunTest("installed-state traversal is rejected", AddressOf InstalledStateTraversalIsRejected)
        RunTest("installed-state unexpected attribute is rejected", AddressOf InstalledStateUnexpectedAttributeIsRejected)
        RunTest("transaction journal phase set is closed", AddressOf TransactionJournalPhaseSetIsClosed)
        RunTest("transaction journal round-trips authenticated identity", AddressOf TransactionJournalRoundTrips)
        RunTest("altered transaction journal is rejected", AddressOf AlteredTransactionJournalIsRejected)
        RunTest("clean file transaction installs exact owned bytes", AddressOf CleanTransactionInstallsOwnedBytes)
        RunTest("repair preserves unknown files", AddressOf RepairPreservesUnknownFiles)
        RunTest("faulted repair rolls back exact prior bytes", AddressOf FaultedRepairRollsBack)
        RunTest("unowned collision is rejected without mutation", AddressOf UnownedCollisionIsRejected)
        RunTest("removal deletes only owned files", AddressOf RemovalDeletesOnlyOwnedFiles)
        RunTest("modified owned file blocks removal", AddressOf ModifiedOwnedFileBlocksRemoval)
        RunTest("faulted removal restores exact installed state", AddressOf FaultedRemovalRestoresState)
        RunTest("matching native setup environment passes", AddressOf MatchingEnvironmentPasses)
        RunTest("emulated setup environment is rejected", AddressOf EmulatedEnvironmentIsRejected)
        RunTest("non-elevated setup environment is rejected", AddressOf NonElevatedEnvironmentIsRejected)
        RunTest("wrong framework setup environment is rejected", AddressOf WrongFrameworkEnvironmentIsRejected)
        RunTest("running application setup environment is rejected", AddressOf RunningApplicationIsRejected)
        RunTest("insufficient setup transaction space is rejected", AddressOf InsufficientSpaceIsRejected)
        RunTest("default install root uses operating-system Program Files", AddressOf DefaultRootUsesProgramFiles)
        RunTest("registry registration uses lane-owned closed values", AddressOf RegistryRegistrationUsesClosedValues)
        RunTest("unowned registry collision is rejected", AddressOf RegistryCollisionIsRejected)
        RunTest("altered registry registration blocks removal", AddressOf AlteredRegistryBlocksRemoval)
        RunTest("registry rollback restores prior owned values", AddressOf RegistryRollbackRestoresOwnedValues)
        RunTest("owned shortcut plan applies and removes", AddressOf OwnedShortcutPlanAppliesAndRemoves)
        RunTest("unowned shortcut collision is rejected", AddressOf ShortcutCollisionIsRejected)
        RunTest("altered shortcut blocks removal", AddressOf AlteredShortcutBlocksRemoval)
        RunTest("faulted shortcut removal restores owned links", AddressOf FaultedShortcutRemovalRestoresLinks)
        RunTest("coordinated clean install commits files registry and shortcuts", AddressOf CoordinatedInstallCommitsAllSurfaces)
        RunTest("faulted coordinated install rolls back every surface", AddressOf FaultedCoordinatedInstallRollsBack)
        RunTest("coordinated repair changes owned shortcut selection", AddressOf CoordinatedRepairChangesShortcutSelection)
        RunTest("post-integration fault rolls back every surface", AddressOf PostIntegrationFaultRollsBack)
        RunTest("coordinated uninstall removes only owned surfaces", AddressOf CoordinatedUninstallRemovesOwnedSurfaces)
        RunTest("post-system uninstall fault restores every surface", AddressOf PostSystemUninstallFaultRestores)
        RunTest("altered registry blocks coordinated uninstall", AddressOf AlteredRegistryBlocksCoordinatedUninstall)
        RunTest("uninstaller relocation copies exact owned bytes", AddressOf RelocationCopiesExactOwnedBytes)
        RunTest("altered relocated uninstaller is rejected", AddressOf AlteredRelocatedUninstallerIsRejected)
        RunTest("matching native uninstall environment passes", AddressOf MatchingUninstallEnvironmentPasses)
        RunTest("running application blocks uninstall", AddressOf RunningApplicationBlocksUninstall)
        RunTest("adjacent Alpha 3 setup bundle loads exact bytes", AddressOf AdjacentBundleLoadsExactBytes)
        RunTest("wrong setup release identity is rejected", AddressOf WrongSetupReleaseIdentityIsRejected)
        For Each phase As String In C3Setup.SetupTransactionPhases.All()
            Dim crashPhase As String = phase
            RunTest("process death during install " & crashPhase & " recovers", Sub() InstallProcessDeathRecovers(crashPhase))
        Next
        For Each phase As String In C3Setup.SetupTransactionPhases.All()
            Dim crashPhase As String = phase
            RunTest("process death during repair " & crashPhase & " preserves user files", Sub() RepairProcessDeathRecovers(crashPhase))
        Next
        For Each phase As String In C3Setup.SetupTransactionPhases.All()
            Dim crashPhase As String = phase
            RunTest("process death during uninstall " & crashPhase & " recovers", Sub() UninstallProcessDeathRecovers(crashPhase))
        Next
        RunTest("new setup invocation recovers an interrupted predecessor", AddressOf SetupStartupRecoversInterruptedPredecessor)
        RunTest("recovery fails closed on altered promoted bytes", AddressOf RecoveryRejectsAlteredPromotedBytes)
        RunTest("installed state remains hidden until external surfaces are durable", AddressOf InstalledStateIsCommittedLast)

        If _failures > 0 Then
            Console.Error.WriteLine("{0} setup characterization test(s) failed.", _failures)
            Environment.ExitCode = 1
            Return
        End If
        Console.WriteLine("All C3 setup contract characterization tests passed.")
    End Sub

    Private Sub RunTest(name As String, test As Action)
        Try
            test()
            Console.WriteLine("PASS: " & name)
        Catch ex As Exception
            _failures += 1
            Console.Error.WriteLine("FAIL: {0}{1}{2}", name, Environment.NewLine, ex.ToString())
        End Try
    End Sub

    Private Sub TransactionJournalPhaseSetIsClosed()
        Dim expected As String() = {
            "prepared",
            "staged",
            "backup-complete",
            "payload-promoted",
            "shortcuts-mutated",
            "registry-mutated",
            "state-committed",
            "complete",
            "rollback-started",
            "rollback-complete"
        }
        Dim actual As String() = C3Setup.SetupTransactionPhases.All()
        If expected.Length <> actual.Length Then Throw New Exception("Transaction journal phase count is not closed.")
        For index As Integer = 0 To expected.Length - 1
            AssertEqual(expected(index), actual(index), "transaction phase " & index.ToString(CultureInfo.InvariantCulture))
        Next
    End Sub

    Private Sub TransactionJournalRoundTrips()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim installRoot As String = CoordinatedInstallRoot(root)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        Dim journal As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournal.CreateInstall(
                            installRoot,
                            manifest,
                            C3Setup.FileHash.Sha256(manifestPath),
                            "89abcdef0123456789abcdef0123456789abcdef",
                            "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789")
                        Dim path As String = C3Setup.SetupTransactionJournalCodec.PathForInstallRoot(installRoot)
                        C3Setup.SetupTransactionJournalCodec.Write(path, journal)
                        Dim actual As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournalCodec.Read(path)
                        AssertEqual(journal.TransactionId, actual.TransactionId, "journal transaction")
                        AssertEqual("install", actual.Operation, "journal operation")
                        AssertEqual("prepared", actual.Phase, "journal phase")
                        AssertEqual(manifest.Lane, actual.Lane, "journal lane")
                        AssertEqual(manifest.SourceCommit, actual.PayloadSourceCommit, "journal payload source")
                        AssertEqual(journal.IdentitySha256, actual.IdentitySha256, "journal identity authenticator")
                    End Sub)
    End Sub

    Private Sub AlteredTransactionJournalIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim installRoot As String = CoordinatedInstallRoot(root)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        Dim journal As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournal.CreateInstall(
                            installRoot,
                            manifest,
                            C3Setup.FileHash.Sha256(manifestPath),
                            "89abcdef0123456789abcdef0123456789abcdef",
                            "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789")
                        Dim path As String = C3Setup.SetupTransactionJournalCodec.PathForInstallRoot(installRoot)
                        C3Setup.SetupTransactionJournalCodec.Write(path, journal)
                        File.WriteAllText(path, File.ReadAllText(path).Replace("phase=""prepared""", "phase=""complete"""))
                        AssertContractFailure(Sub() C3Setup.SetupTransactionJournalCodec.Read(path))
                    End Sub)
    End Sub

    Private Sub ValidClosedPayloadPasses()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        C3Setup.PayloadVerifier.Verify(manifest, Path.Combine(root, "payload"))
                    End Sub)
    End Sub

    Private Sub AlteredPayloadIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        File.AppendAllText(Path.Combine(root, "payload", "README.txt"), "altered")
                        AssertContractFailure(Sub()
                                                  Dim manifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                                                  C3Setup.PayloadVerifier.Verify(manifest, Path.Combine(root, "payload"))
                                              End Sub)
                    End Sub)
    End Sub

    Private Sub UnexpectedPayloadIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        File.WriteAllText(Path.Combine(root, "payload", "extra.dll"), "unexpected")
                        AssertContractFailure(Sub()
                                                  Dim manifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                                                  C3Setup.PayloadVerifier.Verify(manifest, Path.Combine(root, "payload"))
                                              End Sub)
                    End Sub)
    End Sub

    Private Sub TraversalNameIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim xml As String = File.ReadAllText(manifestPath).Replace("BUILD.txt", "..\BUILD.txt")
                        File.WriteAllText(manifestPath, xml)
                        AssertContractFailure(Sub() C3Setup.PayloadManifestReader.Read(manifestPath))
                    End Sub)
    End Sub

    Private Sub DtdIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim xml As String = File.ReadAllText(manifestPath)
                        File.WriteAllText(manifestPath, "<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///c:/windows/win.ini'>]>" & xml)
                        AssertContractFailure(Sub() C3Setup.PayloadManifestReader.Read(manifestPath))
                    End Sub)
    End Sub

    Private Sub NoncanonicalHashIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim xml As String = File.ReadAllText(manifestPath)
                        Dim marker As String = "sha256="""
                        Dim index As Integer = xml.IndexOf(marker, StringComparison.Ordinal)
                        index += marker.Length
                        Dim value As String = xml.Substring(index, 64)
                        xml = xml.Substring(0, index) & value.ToUpperInvariant() & xml.Substring(index + 64)
                        File.WriteAllText(manifestPath, xml)
                        AssertContractFailure(Sub() C3Setup.PayloadManifestReader.Read(manifestPath))
                    End Sub)
    End Sub

    Private Sub VolumeRootIsRejected()
        Dim root As String = Path.GetPathRoot(Path.GetFullPath(Path.GetTempPath()))
        AssertContractFailure(Sub() C3Setup.SetupPathPolicy.ValidateInstallRoot(root))
    End Sub

    Private Sub OrdinaryDescendantIsAccepted()
        Dim candidate As String = Path.Combine(Path.GetTempPath(), "C3 Setup Test", "Product")
        Dim actual As String = C3Setup.SetupPathPolicy.ValidateInstallRoot(candidate)
        AssertEqual(Path.GetFullPath(candidate).TrimEnd(Path.DirectorySeparatorChar), actual, "canonical install root")
    End Sub

    Private Sub InstalledStateRoundTrips()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim statePath As String = Path.Combine(root, C3Setup.InstalledStateCodec.FileName)
                        Dim expected As C3Setup.InstalledState = CreateInstalledState(root, manifestPath)
                        C3Setup.InstalledStateCodec.Write(statePath, expected)
                        Dim actual As C3Setup.InstalledState = C3Setup.InstalledStateCodec.Read(statePath)
                        AssertEqual(expected.Manifest.Label, actual.Manifest.Label, "installed label")
                        AssertEqual(expected.InstallRoot, actual.InstallRoot, "installed root")
                        AssertEqual(expected.TransactionId, actual.TransactionId, "transaction ID")
                        If actual.Manifest.Files.Count <> PayloadNames.Length Then
                            Throw New Exception("Installed state did not retain exact owned files.")
                        End If
                    End Sub)
    End Sub

    Private Sub InstalledStateTraversalIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim statePath As String = Path.Combine(root, C3Setup.InstalledStateCodec.FileName)
                        C3Setup.InstalledStateCodec.Write(statePath, CreateInstalledState(root, manifestPath))
                        Dim xml As String = File.ReadAllText(statePath).Replace("path=""BUILD.txt""", "path=""..\BUILD.txt""")
                        File.WriteAllText(statePath, xml)
                        AssertContractFailure(Sub() C3Setup.InstalledStateCodec.Read(statePath))
                    End Sub)
    End Sub

    Private Sub InstalledStateUnexpectedAttributeIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim statePath As String = Path.Combine(root, C3Setup.InstalledStateCodec.FileName)
                        C3Setup.InstalledStateCodec.Write(statePath, CreateInstalledState(root, manifestPath))
                        Dim xml As String = File.ReadAllText(statePath).Replace("complete=""true""", "complete=""true"" unexpected=""1""")
                        File.WriteAllText(statePath, xml)
                        AssertContractFailure(Sub() C3Setup.InstalledStateCodec.Read(statePath))
                    End Sub)
    End Sub

    Private Function CreateInstalledState(root As String, manifestPath As String) As C3Setup.InstalledState
        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
        Return New C3Setup.InstalledState(manifest,
                                          "89abcdef0123456789abcdef0123456789abcdef",
                                          Path.Combine(root, "installed", "Compact Cassette Catalogue"),
                                          "install",
                                          "0123456789abcdef0123456789abcdef",
                                          New DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc),
                                          C3Setup.FileHash.Sha256(manifestPath),
                                          "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789",
                                          New List(Of C3Setup.InstalledShortcut)())
    End Function

    Private Sub CleanTransactionInstallsOwnedBytes()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim installRoot As String = GetInstallRoot(root)
                        Dim state As C3Setup.InstalledState = ApplyTransaction(root, manifestPath, Nothing)
                        AssertEqual("install", state.Mode, "clean mode")
                        C3Setup.PayloadVerifier.VerifyOwnedFiles(state.Manifest, installRoot)
                        If Not File.Exists(Path.Combine(installRoot, C3Setup.InstalledStateCodec.FileName)) Then
                            Throw New Exception("Installed-state manifest is missing after clean install.")
                        End If
                    End Sub)
    End Sub

    Private Sub RepairPreservesUnknownFiles()
        WithPayload(Sub(root As String, manifestPath As String)
                        ApplyTransaction(root, manifestPath, Nothing)
                        Dim unknownPath As String = Path.Combine(GetInstallRoot(root), "user-catalogue.xml")
                        File.WriteAllText(unknownPath, "user data")
                        Dim state As C3Setup.InstalledState = ApplyTransaction(root, manifestPath, Nothing)
                        AssertEqual("repair", state.Mode, "repair mode")
                        AssertEqual("user data", File.ReadAllText(unknownPath), "unknown file content")
                    End Sub)
    End Sub

    Private Sub FaultedRepairRollsBack()
        WithPayload(Sub(root As String, manifestPath As String)
                        ApplyTransaction(root, manifestPath, Nothing)
                        Dim installedReadme As String = Path.Combine(GetInstallRoot(root), "README.txt")
                        Dim originalReadme As String = File.ReadAllText(installedReadme)
                        File.WriteAllText(Path.Combine(root, "payload", "README.txt"), "new repaired bytes")
                        File.WriteAllText(manifestPath, BuildManifest(Path.Combine(root, "payload")))
                        Try
                            ApplyTransaction(root,
                                             manifestPath,
                                             Sub(point As String)
                                                 If point = "after-first-file" Then Throw New InvalidOperationException("injected")
                                             End Sub)
                            Throw New Exception("Expected injected transaction failure.")
                        Catch ex As InvalidOperationException
                            If ex.Message <> "injected" Then Throw
                        End Try
                        AssertEqual(originalReadme, File.ReadAllText(installedReadme), "rolled-back README")
                        C3Setup.InstalledStateCodec.Read(Path.Combine(GetInstallRoot(root), C3Setup.InstalledStateCodec.FileName))
                    End Sub)
    End Sub

    Private Sub UnownedCollisionIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim installRoot As String = GetInstallRoot(root)
                        Directory.CreateDirectory(installRoot)
                        Dim collision As String = Path.Combine(installRoot, "README.txt")
                        File.WriteAllText(collision, "unowned")
                        AssertContractFailure(Sub() ApplyTransaction(root, manifestPath, Nothing))
                        AssertEqual("unowned", File.ReadAllText(collision), "unowned collision")
                    End Sub)
    End Sub

    Private Function ApplyTransaction(root As String,
                                      manifestPath As String,
                                      faultInjector As Action(Of String)) As C3Setup.InstalledState
        Return C3Setup.SetupFileTransaction.Apply(manifestPath,
                                                  Path.Combine(root, "payload"),
                                                  GetInstallRoot(root),
                                                  "89abcdef0123456789abcdef0123456789abcdef",
                                                  "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789",
                                                  faultInjector)
    End Function

    Private Function GetInstallRoot(root As String) As String
        Return Path.Combine(root, "installed", "Compact Cassette Catalogue")
    End Function

    Private Sub RemovalDeletesOnlyOwnedFiles()
        WithPayload(Sub(root As String, manifestPath As String)
                        ApplyTransaction(root, manifestPath, Nothing)
                        Dim installRoot As String = GetInstallRoot(root)
                        Dim unknownPath As String = Path.Combine(installRoot, "catalogue.xml")
                        File.WriteAllText(unknownPath, "preserve")
                        C3Setup.SetupRemovalTransaction.Remove(installRoot, Nothing)
                        AssertEqual("preserve", File.ReadAllText(unknownPath), "preserved unknown catalogue")
                        For Each name As String In PayloadNames
                            If File.Exists(Path.Combine(installRoot, name)) Then Throw New Exception("Owned file survived removal: " & name)
                        Next
                        If File.Exists(Path.Combine(installRoot, C3Setup.InstalledStateCodec.FileName)) Then Throw New Exception("Installed-state file survived removal.")
                    End Sub)
    End Sub

    Private Sub ModifiedOwnedFileBlocksRemoval()
        WithPayload(Sub(root As String, manifestPath As String)
                        ApplyTransaction(root, manifestPath, Nothing)
                        Dim installRoot As String = GetInstallRoot(root)
                        Dim readme As String = Path.Combine(installRoot, "README.txt")
                        File.AppendAllText(readme, "modified")
                        AssertContractFailure(Sub() C3Setup.SetupRemovalTransaction.Remove(installRoot, Nothing))
                        If Not File.Exists(Path.Combine(installRoot, "Compact Cassette Catalogue.exe")) Then Throw New Exception("Removal mutated files before rejecting modified ownership.")
                    End Sub)
    End Sub

    Private Sub FaultedRemovalRestoresState()
        WithPayload(Sub(root As String, manifestPath As String)
                        ApplyTransaction(root, manifestPath, Nothing)
                        Dim installRoot As String = GetInstallRoot(root)
                        Try
                            C3Setup.SetupRemovalTransaction.Remove(installRoot,
                                                                  Sub(point As String)
                                                                      If point = "after-first-file" Then Throw New InvalidOperationException("remove-injected")
                                                                  End Sub)
                            Throw New Exception("Expected injected removal failure.")
                        Catch ex As InvalidOperationException
                            If ex.Message <> "remove-injected" Then Throw
                        End Try
                        Dim state As C3Setup.InstalledState = C3Setup.InstalledStateCodec.Read(Path.Combine(installRoot, C3Setup.InstalledStateCodec.FileName))
                        C3Setup.PayloadVerifier.VerifyOwnedFiles(state.Manifest, installRoot)
                    End Sub)
    End Sub

    Private Sub MatchingEnvironmentPasses()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        C3Setup.SetupEnvironment.Validate(manifest, EnvironmentFacts(root, "x86", "x86", True, True, 0, False, 3000), 1000)
                    End Sub)
    End Sub

    Private Sub EmulatedEnvironmentIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        AssertContractFailure(Sub() C3Setup.SetupEnvironment.Validate(manifest, EnvironmentFacts(root, "x86", "x64", True, True, 0, False, 3000), 1000))
                    End Sub)
    End Sub

    Private Sub NonElevatedEnvironmentIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        AssertContractFailure(Sub() C3Setup.SetupEnvironment.Validate(manifest, EnvironmentFacts(root, "x86", "x86", False, True, 0, False, 3000), 1000))
                    End Sub)
    End Sub

    Private Sub WrongFrameworkEnvironmentIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        AssertContractFailure(Sub() C3Setup.SetupEnvironment.Validate(manifest, EnvironmentFacts(root, "x86", "x86", True, False, 0, False, 3000), 1000))
                    End Sub)
    End Sub

    Private Sub RunningApplicationIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        AssertContractFailure(Sub() C3Setup.SetupEnvironment.Validate(manifest, EnvironmentFacts(root, "x86", "x86", True, True, 0, True, 3000), 1000))
                    End Sub)
    End Sub

    Private Sub InsufficientSpaceIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        AssertContractFailure(Sub() C3Setup.SetupEnvironment.Validate(manifest, EnvironmentFacts(root, "x86", "x86", True, True, 0, False, 2999), 1000))
                    End Sub)
    End Sub

    Private Sub DefaultRootUsesProgramFiles()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim programFiles As String = Path.Combine(root, "Program Files")
                        Directory.CreateDirectory(programFiles)
                        Dim facts As C3Setup.SetupEnvironmentFacts = New C3Setup.SetupEnvironmentFacts("x86", "x86", True, True, 0, programFiles, False, 3000)
                        AssertEqual(Path.Combine(programFiles, "Compact Cassette Catalogue"), C3Setup.SetupEnvironment.DefaultInstallRoot(facts), "default install root")
                    End Sub)
    End Sub

    Private Function EnvironmentFacts(root As String,
                                      processArchitecture As String,
                                      nativeArchitecture As String,
                                      elevated As Boolean,
                                      frameworkInstalled As Boolean,
                                      frameworkRelease As Long,
                                      applicationRunning As Boolean,
                                      availableBytes As Long) As C3Setup.SetupEnvironmentFacts
        Dim programFiles As String = Path.Combine(root, "Program Files")
        Directory.CreateDirectory(programFiles)
        Return New C3Setup.SetupEnvironmentFacts(processArchitecture, nativeArchitecture, elevated, frameworkInstalled,
                                                  frameworkRelease, programFiles, applicationRunning, availableBytes)
    End Function

    Private Sub RegistryRegistrationUsesClosedValues()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim state As C3Setup.InstalledState = CreateInstalledState(root, manifestPath)
                        Dim access As New MemoryRegistryAccess()
                        Dim prior As IDictionary(Of String, Object) = C3Setup.SetupRegistryRegistration.Apply(state, access)
                        If prior IsNot Nothing Then Throw New Exception("Clean registration unexpectedly returned prior state.")
                        Dim key As String = C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)
                        AssertEqual("Software\Microsoft\Windows\CurrentVersion\Uninstall\CompactCassetteCatalogue-1x-x86", key, "lane uninstall key")
                        Dim values As IDictionary(Of String, Object) = access.ReadValues(key)
                        AssertEqual("1.3.0a3", DirectCast(values("DisplayVersion"), String), "registered display version")
                        If values.ContainsKey("QuietUninstallString") Then Throw New Exception("Untested quiet uninstall was registered.")
                        C3Setup.SetupRegistryRegistration.Remove(state, access)
                        If access.ReadValues(key) IsNot Nothing Then Throw New Exception("Owned registration survived removal.")
                    End Sub)
    End Sub

    Private Sub RegistryCollisionIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim state As C3Setup.InstalledState = CreateInstalledState(root, manifestPath)
                        Dim access As New MemoryRegistryAccess()
                        Dim collision As New Dictionary(Of String, Object)(StringComparer.Ordinal)
                        collision.Add("DisplayName", "Unrelated product")
                        access.WriteValues(C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane), collision)
                        AssertContractFailure(Sub() C3Setup.SetupRegistryRegistration.Apply(state, access))
                        AssertEqual("Unrelated product", DirectCast(access.ReadValues(C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane))("DisplayName"), String), "unowned registry collision")
                    End Sub)
    End Sub

    Private Sub AlteredRegistryBlocksRemoval()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim state As C3Setup.InstalledState = CreateInstalledState(root, manifestPath)
                        Dim access As New MemoryRegistryAccess()
                        C3Setup.SetupRegistryRegistration.Apply(state, access)
                        Dim key As String = C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)
                        Dim altered As IDictionary(Of String, Object) = access.ReadValues(key)
                        altered("UninstallString") = "unexpected.exe"
                        access.WriteValues(key, altered)
                        AssertContractFailure(Sub() C3Setup.SetupRegistryRegistration.Remove(state, access))
                        If access.ReadValues(key) Is Nothing Then Throw New Exception("Altered registration was deleted.")
                    End Sub)
    End Sub

    Private Sub RegistryRollbackRestoresOwnedValues()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim state As C3Setup.InstalledState = CreateInstalledState(root, manifestPath)
                        Dim access As New MemoryRegistryAccess()
                        Dim key As String = C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)
                        Dim prior As IDictionary(Of String, Object) = C3Setup.SetupRegistryRegistration.ExpectedValues(state)
                        prior("DisplayVersion") = "prior-owned-version"
                        access.WriteValues(key, prior)
                        Dim snapshot As IDictionary(Of String, Object) = C3Setup.SetupRegistryRegistration.Apply(state, access)
                        C3Setup.SetupRegistryRegistration.Restore(state, snapshot, access)
                        AssertEqual("prior-owned-version", DirectCast(access.ReadValues(key)("DisplayVersion"), String), "restored registry value")
                    End Sub)
    End Sub

    Private Sub OwnedShortcutPlanAppliesAndRemoves()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim programs As String = Path.Combine(root, "Common Programs")
                        Dim desktop As String = Path.Combine(root, "Common Desktop")
                        Directory.CreateDirectory(programs)
                        Directory.CreateDirectory(desktop)
                        Dim access As New MemoryShortcutAccess(programs, desktop)
                        Dim state As C3Setup.InstalledState = CreateInstalledStateWithShortcuts(root, manifestPath, programs, desktop, True)
                        Dim previous As IDictionary(Of String, C3Setup.SetupShortcut) = C3Setup.SetupShortcutService.Apply(state, access)
                        If previous.Count <> 2 Then Throw New Exception("Shortcut transaction did not cover both owned links.")
                        C3Setup.SetupShortcutService.Remove(state, access)
                        For Each item As C3Setup.InstalledShortcut In state.Shortcuts
                            If access.ReadShortcut(item.Path) IsNot Nothing Then Throw New Exception("Owned shortcut survived removal.")
                        Next
                    End Sub)
    End Sub

    Private Sub ShortcutCollisionIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim programs As String = Path.Combine(root, "Common Programs")
                        Dim desktop As String = Path.Combine(root, "Common Desktop")
                        Directory.CreateDirectory(programs)
                        Directory.CreateDirectory(desktop)
                        Dim access As New MemoryShortcutAccess(programs, desktop)
                        Dim state As C3Setup.InstalledState = CreateInstalledStateWithShortcuts(root, manifestPath, programs, desktop, False)
                        Dim owned As C3Setup.InstalledShortcut = state.Shortcuts(0)
                        access.WriteShortcut(New C3Setup.SetupShortcut(owned.Path, "C:\Unrelated.exe", "C:\", "Unrelated"))
                        AssertContractFailure(Sub() C3Setup.SetupShortcutService.Apply(state, access))
                        AssertEqual("C:\Unrelated.exe", access.ReadShortcut(owned.Path).Target, "unowned shortcut target")
                    End Sub)
    End Sub

    Private Sub AlteredShortcutBlocksRemoval()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim programs As String = Path.Combine(root, "Common Programs")
                        Dim desktop As String = Path.Combine(root, "Common Desktop")
                        Directory.CreateDirectory(programs)
                        Directory.CreateDirectory(desktop)
                        Dim access As New MemoryShortcutAccess(programs, desktop)
                        Dim state As C3Setup.InstalledState = CreateInstalledStateWithShortcuts(root, manifestPath, programs, desktop, False)
                        C3Setup.SetupShortcutService.Apply(state, access)
                        Dim owned As C3Setup.InstalledShortcut = state.Shortcuts(0)
                        access.WriteShortcut(New C3Setup.SetupShortcut(owned.Path, "C:\Altered.exe", "C:\", "Altered"))
                        AssertContractFailure(Sub() C3Setup.SetupShortcutService.Remove(state, access))
                        If access.ReadShortcut(owned.Path) Is Nothing Then Throw New Exception("Altered shortcut was deleted.")
                    End Sub)
    End Sub

    Private Sub FaultedShortcutRemovalRestoresLinks()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim programs As String = Path.Combine(root, "Common Programs")
                        Dim desktop As String = Path.Combine(root, "Common Desktop")
                        Directory.CreateDirectory(programs)
                        Directory.CreateDirectory(desktop)
                        Dim access As New MemoryShortcutAccess(programs, desktop)
                        Dim state As C3Setup.InstalledState = CreateInstalledStateWithShortcuts(root, manifestPath, programs, desktop, True)
                        C3Setup.SetupShortcutService.Apply(state, access)
                        access.FailDeletePath = state.Shortcuts(1).Path
                        Try
                            C3Setup.SetupShortcutService.Remove(state, access)
                            Throw New Exception("Expected injected shortcut removal failure.")
                        Catch ex As InvalidOperationException
                            If ex.Message <> "shortcut-delete-injected" Then Throw
                        End Try
                        For Each item As C3Setup.InstalledShortcut In state.Shortcuts
                            If access.ReadShortcut(item.Path) Is Nothing Then Throw New Exception("Shortcut removal rollback did not restore all owned links.")
                        Next
                    End Sub)
    End Sub

    Private Function CreateInstalledStateWithShortcuts(root As String,
                                                       manifestPath As String,
                                                       programs As String,
                                                       desktop As String,
                                                       includeDesktop As Boolean) As C3Setup.InstalledState
        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
        Dim installRoot As String = Path.Combine(root, "installed", "Compact Cassette Catalogue")
        Return New C3Setup.InstalledState(manifest,
                                          "89abcdef0123456789abcdef0123456789abcdef",
                                          installRoot,
                                          "install",
                                          "0123456789abcdef0123456789abcdef",
                                          New DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc),
                                          C3Setup.FileHash.Sha256(manifestPath),
                                          "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789",
                                          C3Setup.SetupShortcutService.Plan(installRoot, programs, desktop, includeDesktop))
    End Function

    Private Sub CoordinatedInstallCommitsAllSurfaces()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New MemoryRegistryAccess()
                        Dim shortcuts As MemoryShortcutAccess = CreateShortcutAccess(root)
                        Dim state As C3Setup.InstalledState = ExecuteCoordinatedInstall(root, manifestPath, registry, shortcuts, True, Nothing)
                        C3Setup.PayloadVerifier.VerifyOwnedFiles(state.Manifest, state.InstallRoot)
                        If registry.ReadValues(C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)) Is Nothing Then
                            Throw New Exception("Coordinated install did not create the owned registry key.")
                        End If
                        For Each item As C3Setup.InstalledShortcut In state.Shortcuts
                            If shortcuts.ReadShortcut(item.Path) Is Nothing Then Throw New Exception("Coordinated install did not create an owned shortcut.")
                        Next
                    End Sub)
    End Sub

    Private Sub FaultedCoordinatedInstallRollsBack()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New MemoryRegistryAccess()
                        registry.FailNextWrite = True
                        Dim shortcuts As MemoryShortcutAccess = CreateShortcutAccess(root)
                        Try
                            ExecuteCoordinatedInstall(root, manifestPath, registry, shortcuts, True, Nothing)
                            Throw New Exception("Expected injected registry failure.")
                        Catch ex As InvalidOperationException
                            If ex.Message <> "registry-write-injected" Then Throw
                        End Try
                        Dim installRoot As String = CoordinatedInstallRoot(root)
                        If Directory.Exists(installRoot) Then Throw New Exception("Faulted coordinated install left its product directory.")
                        Dim key As String = C3Setup.InstalledStateCodec.UninstallKeyForLane("win-x86-net40")
                        If registry.ReadValues(key) IsNot Nothing Then Throw New Exception("Faulted coordinated install left its registry key.")
                        Dim expected As IList(Of C3Setup.InstalledShortcut) = C3Setup.SetupShortcutService.Plan(installRoot, shortcuts.CommonProgramsPath, shortcuts.CommonDesktopPath, True)
                        For Each item As C3Setup.InstalledShortcut In expected
                            If shortcuts.ReadShortcut(item.Path) IsNot Nothing Then Throw New Exception("Faulted coordinated install left a shortcut.")
                        Next
                    End Sub)
    End Sub

    Private Sub CoordinatedRepairChangesShortcutSelection()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New MemoryRegistryAccess()
                        Dim shortcuts As MemoryShortcutAccess = CreateShortcutAccess(root)
                        ExecuteCoordinatedInstall(root, manifestPath, registry, shortcuts, True, Nothing)
                        Dim state As C3Setup.InstalledState = ExecuteCoordinatedInstall(root, manifestPath, registry, shortcuts, False, Nothing)
                        AssertEqual("repair", state.Mode, "coordinated repair mode")
                        If state.Shortcuts.Count <> 1 Then Throw New Exception("Repair did not record the deselected desktop shortcut.")
                        Dim desktopPath As String = C3Setup.SetupShortcutService.Plan(state.InstallRoot, shortcuts.CommonProgramsPath, shortcuts.CommonDesktopPath, True)(1).Path
                        If shortcuts.ReadShortcut(desktopPath) IsNot Nothing Then Throw New Exception("Repair left the deselected owned desktop shortcut.")
                    End Sub)
    End Sub

    Private Sub PostIntegrationFaultRollsBack()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New MemoryRegistryAccess()
                        Dim shortcuts As MemoryShortcutAccess = CreateShortcutAccess(root)
                        Try
                            ExecuteCoordinatedInstall(root,
                                                      manifestPath,
                                                      registry,
                                                      shortcuts,
                                                      True,
                                                      Sub(point As String)
                                                          If point = "after-system-integration" Then Throw New InvalidOperationException("post-integration-injected")
                                                      End Sub)
                            Throw New Exception("Expected post-integration failure.")
                        Catch ex As InvalidOperationException
                            If ex.Message <> "post-integration-injected" Then Throw
                        End Try
                        If Directory.Exists(CoordinatedInstallRoot(root)) Then Throw New Exception("Post-integration failure left its product directory.")
                        If registry.ReadValues(C3Setup.InstalledStateCodec.UninstallKeyForLane("win-x86-net40")) IsNot Nothing Then Throw New Exception("Post-integration failure left its registry key.")
                        Dim expected As IList(Of C3Setup.InstalledShortcut) = C3Setup.SetupShortcutService.Plan(CoordinatedInstallRoot(root), shortcuts.CommonProgramsPath, shortcuts.CommonDesktopPath, True)
                        For Each item As C3Setup.InstalledShortcut In expected
                            If shortcuts.ReadShortcut(item.Path) IsNot Nothing Then Throw New Exception("Post-integration failure left a shortcut.")
                        Next
                    End Sub)
    End Sub

    Private Sub CoordinatedUninstallRemovesOwnedSurfaces()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New MemoryRegistryAccess()
                        Dim shortcuts As MemoryShortcutAccess = CreateShortcutAccess(root)
                        Dim state As C3Setup.InstalledState = ExecuteCoordinatedInstall(root, manifestPath, registry, shortcuts, True, Nothing)
                        Dim unknownPath As String = Path.Combine(state.InstallRoot, "catalogue.xml")
                        File.WriteAllText(unknownPath, "preserve")
                        C3Setup.SetupUninstallOperation.Execute(state.InstallRoot, shortcuts, registry, Nothing)
                        AssertEqual("preserve", File.ReadAllText(unknownPath), "coordinated uninstall unknown file")
                        AssertCoordinatedSurfacesRemoved(state, registry, shortcuts)
                    End Sub)
    End Sub

    Private Sub PostSystemUninstallFaultRestores()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New MemoryRegistryAccess()
                        Dim shortcuts As MemoryShortcutAccess = CreateShortcutAccess(root)
                        Dim state As C3Setup.InstalledState = ExecuteCoordinatedInstall(root, manifestPath, registry, shortcuts, True, Nothing)
                        Try
                            C3Setup.SetupUninstallOperation.Execute(state.InstallRoot,
                                                                   shortcuts,
                                                                   registry,
                                                                   Sub(point As String)
                                                                       If point = "after-system-removal" Then Throw New InvalidOperationException("uninstall-post-system-injected")
                                                                   End Sub)
                            Throw New Exception("Expected post-system uninstall failure.")
                        Catch ex As InvalidOperationException
                            If ex.Message <> "uninstall-post-system-injected" Then Throw
                        End Try
                        C3Setup.PayloadVerifier.VerifyOwnedFiles(state.Manifest, state.InstallRoot)
                        If registry.ReadValues(C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)) Is Nothing Then Throw New Exception("Uninstall rollback did not restore registry state.")
                        For Each item As C3Setup.InstalledShortcut In state.Shortcuts
                            If shortcuts.ReadShortcut(item.Path) Is Nothing Then Throw New Exception("Uninstall rollback did not restore a shortcut.")
                        Next
                    End Sub)
    End Sub

    Private Sub AlteredRegistryBlocksCoordinatedUninstall()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New MemoryRegistryAccess()
                        Dim shortcuts As MemoryShortcutAccess = CreateShortcutAccess(root)
                        Dim state As C3Setup.InstalledState = ExecuteCoordinatedInstall(root, manifestPath, registry, shortcuts, True, Nothing)
                        Dim key As String = C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)
                        Dim altered As IDictionary(Of String, Object) = registry.ReadValues(key)
                        altered("DisplayName") = "Altered registration"
                        registry.WriteValues(key, altered)
                        AssertContractFailure(Sub() C3Setup.SetupUninstallOperation.Execute(state.InstallRoot, shortcuts, registry, Nothing))
                        C3Setup.PayloadVerifier.VerifyOwnedFiles(state.Manifest, state.InstallRoot)
                        For Each item As C3Setup.InstalledShortcut In state.Shortcuts
                            If shortcuts.ReadShortcut(item.Path) Is Nothing Then Throw New Exception("Blocked uninstall did not restore shortcuts.")
                        Next
                    End Sub)
    End Sub

    Private Sub AssertCoordinatedSurfacesRemoved(state As C3Setup.InstalledState,
                                                 registry As MemoryRegistryAccess,
                                                 shortcuts As MemoryShortcutAccess)
        For Each item As C3Setup.PayloadFile In state.Manifest.Files
            If File.Exists(Path.Combine(state.InstallRoot, item.Path)) Then Throw New Exception("Owned file survived coordinated uninstall: " & item.Path)
        Next
        If File.Exists(Path.Combine(state.InstallRoot, C3Setup.InstalledStateCodec.FileName)) Then Throw New Exception("Installed state survived coordinated uninstall.")
        If registry.ReadValues(C3Setup.InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)) IsNot Nothing Then Throw New Exception("Registry state survived coordinated uninstall.")
        For Each item As C3Setup.InstalledShortcut In state.Shortcuts
            If shortcuts.ReadShortcut(item.Path) IsNot Nothing Then Throw New Exception("Owned shortcut survived coordinated uninstall.")
        Next
    End Sub

    Private Sub RelocationCopiesExactOwnedBytes()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim installedRoot As String = Path.Combine(root, "installed", "Compact Cassette Catalogue")
                        Directory.CreateDirectory(installedRoot)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        For Each item As C3Setup.PayloadFile In manifest.Files
                            File.Copy(Path.Combine(root, "payload", item.Path), Path.Combine(installedRoot, item.Path))
                        Next
                        Dim state As C3Setup.InstalledState = CreateInstalledState(root, manifestPath)
                        Dim statePath As String = Path.Combine(installedRoot, C3Setup.InstalledStateCodec.FileName)
                        C3Setup.InstalledStateCodec.Write(statePath, state)
                        Dim temporaryBase As String = Path.Combine(root, "temp")
                        Directory.CreateDirectory(temporaryBase)
                        Dim launcher As New MemoryProcessLauncher()
                        Dim context As C3Setup.SetupRelocationContext = C3Setup.SetupSelfRelocation.PrepareAndLaunch(
                            Path.Combine(installedRoot, "UNINSTALL.exe"), statePath, temporaryBase, launcher)
                        AssertEqual(context.ExecutablePath, launcher.ExecutablePath, "relocated launch executable")
                        Dim validated As C3Setup.SetupRelocationContext = C3Setup.SetupSelfRelocation.ValidateRelocatedInvocation(
                            New String() {"--state", statePath, "--relocation-root", context.RelocationRoot},
                            context.ExecutablePath)
                        AssertEqual(installedRoot, validated.InstallRoot, "relocated install root")
                    End Sub)
    End Sub

    Private Sub AlteredRelocatedUninstallerIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim installedRoot As String = Path.Combine(root, "installed", "Compact Cassette Catalogue")
                        Directory.CreateDirectory(installedRoot)
                        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
                        For Each item As C3Setup.PayloadFile In manifest.Files
                            File.Copy(Path.Combine(root, "payload", item.Path), Path.Combine(installedRoot, item.Path))
                        Next
                        Dim statePath As String = Path.Combine(installedRoot, C3Setup.InstalledStateCodec.FileName)
                        C3Setup.InstalledStateCodec.Write(statePath, CreateInstalledState(root, manifestPath))
                        Dim temporaryBase As String = Path.Combine(root, "temp")
                        Directory.CreateDirectory(temporaryBase)
                        Dim context As C3Setup.SetupRelocationContext = C3Setup.SetupSelfRelocation.PrepareAndLaunch(
                            Path.Combine(installedRoot, "UNINSTALL.exe"), statePath, temporaryBase, New MemoryProcessLauncher())
                        File.AppendAllText(context.ExecutablePath, "altered")
                        AssertContractFailure(Sub() C3Setup.SetupSelfRelocation.ValidateRelocatedInvocation(
                                                  New String() {"--state", statePath, "--relocation-root", context.RelocationRoot},
                                                  context.ExecutablePath))
                    End Sub)
    End Sub

    Private Sub MatchingUninstallEnvironmentPasses()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim programFiles As String = Path.Combine(root, "Program Files")
                        Directory.CreateDirectory(programFiles)
                        Dim state As C3Setup.InstalledState = CreateInstalledStateForRoot(Path.Combine(programFiles, "Compact Cassette Catalogue"), manifestPath)
                        Dim facts As New C3Setup.SetupEnvironmentFacts("x86", "x86", True, True, 0, programFiles, False, 0)
                        C3Setup.SetupEnvironment.ValidateRemoval(state, facts)
                    End Sub)
    End Sub

    Private Sub RunningApplicationBlocksUninstall()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim programFiles As String = Path.Combine(root, "Program Files")
                        Directory.CreateDirectory(programFiles)
                        Dim state As C3Setup.InstalledState = CreateInstalledStateForRoot(Path.Combine(programFiles, "Compact Cassette Catalogue"), manifestPath)
                        Dim facts As New C3Setup.SetupEnvironmentFacts("x86", "x86", True, True, 0, programFiles, True, 0)
                        AssertContractFailure(Sub() C3Setup.SetupEnvironment.ValidateRemoval(state, facts))
                    End Sub)
    End Sub

    Private Function CreateInstalledStateForRoot(installRoot As String, manifestPath As String) As C3Setup.InstalledState
        Dim manifest As C3Setup.PayloadManifest = C3Setup.PayloadManifestReader.Read(manifestPath)
        Return New C3Setup.InstalledState(manifest,
                                          "89abcdef0123456789abcdef0123456789abcdef",
                                          installRoot,
                                          "install",
                                          "0123456789abcdef0123456789abcdef",
                                          New DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc),
                                          C3Setup.FileHash.Sha256(manifestPath),
                                          "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789",
                                          New List(Of C3Setup.InstalledShortcut)())
    End Function

    Private Sub AdjacentBundleLoadsExactBytes()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim setupPath As String = Path.Combine(root, "SETUP.exe")
                        File.WriteAllText(setupPath, "setup-executable")
                        Dim context As C3Setup.SetupBundleContext = C3Setup.SetupBundleRuntime.Load(root, setupPath)
                        AssertEqual("1.3.0a3", context.Manifest.Label, "bundle release label")
                        AssertEqual(C3Setup.FileHash.Sha256(setupPath), context.SetupExecutableSha256, "setup executable hash")
                        If context.PayloadBytes <= 0 Then Throw New Exception("Bundle payload byte total was not retained.")
                    End Sub)
    End Sub

    Private Sub WrongSetupReleaseIdentityIsRejected()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim setupPath As String = Path.Combine(root, "SETUP.exe")
                        File.WriteAllText(setupPath, "setup-executable")
                        File.WriteAllText(manifestPath, File.ReadAllText(manifestPath).Replace("label=""1.3.0a3""", "label=""1.3.0"""))
                        AssertContractFailure(Sub() C3Setup.SetupBundleRuntime.Load(root, setupPath))
                    End Sub)
    End Sub

    Private Sub InstallProcessDeathRecovers(phase As String)
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New DirectoryRegistryAccess(Path.Combine(root, "registry"))
                        Dim shortcuts As DirectoryShortcutAccess = CreateDirectoryShortcutAccess(root)
                        RunCrashChild("install", phase, root, manifestPath)
                        Dim installRoot As String = CoordinatedInstallRoot(root)
                        Dim journalPath As String = C3Setup.SetupTransactionJournalCodec.PathForInstallRoot(installRoot)
                        Dim interrupted As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournalCodec.Read(journalPath)
                        AssertEqual(phase, interrupted.Phase, "interrupted install phase")
                        Dim result As String = C3Setup.SetupTransactionRecovery.RecoverIncomplete(installRoot, shortcuts, registry)
                        If phase = C3Setup.SetupTransactionPhases.Complete Then
                            AssertEqual(C3Setup.SetupTransactionPhases.Complete, result, "completed install recovery result")
                            AssertInstalledSurfaces(installRoot, registry, shortcuts)
                        Else
                            AssertEqual(C3Setup.SetupTransactionPhases.RollbackComplete, result, "interrupted install recovery result")
                            AssertAbsentSurfaces(installRoot, registry, shortcuts)
                        End If
                        AssertJournalSettled(journalPath, phase = C3Setup.SetupTransactionPhases.Complete)
                    End Sub)
    End Sub

    Private Sub UninstallProcessDeathRecovers(phase As String)
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New DirectoryRegistryAccess(Path.Combine(root, "registry"))
                        Dim shortcuts As DirectoryShortcutAccess = CreateDirectoryShortcutAccess(root)
                        ExecutePersistentInstall(root, manifestPath, registry, shortcuts)
                        Dim installRoot As String = CoordinatedInstallRoot(root)
                        Dim unknownPath As String = Path.Combine(installRoot, "catalogue.xml")
                        File.WriteAllText(unknownPath, "preserve")
                        RunCrashChild("uninstall", phase, root, manifestPath)
                        Dim journalPath As String = C3Setup.SetupTransactionJournalCodec.PathForInstallRoot(installRoot)
                        Dim interrupted As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournalCodec.Read(journalPath)
                        AssertEqual(phase, interrupted.Phase, "interrupted uninstall phase")
                        Dim result As String = C3Setup.SetupTransactionRecovery.RecoverIncomplete(installRoot, shortcuts, registry)
                        If phase = C3Setup.SetupTransactionPhases.Complete Then
                            AssertEqual(C3Setup.SetupTransactionPhases.Complete, result, "completed uninstall recovery result")
                            AssertAbsentSurfaces(installRoot, registry, shortcuts)
                        Else
                            AssertEqual(C3Setup.SetupTransactionPhases.RollbackComplete, result, "interrupted uninstall recovery result")
                            AssertInstalledSurfaces(installRoot, registry, shortcuts)
                        End If
                        AssertEqual("preserve", File.ReadAllText(unknownPath), "unknown catalogue after uninstall recovery")
                        AssertJournalSettled(journalPath, phase = C3Setup.SetupTransactionPhases.Complete)
                    End Sub)
    End Sub

    Private Sub RepairProcessDeathRecovers(phase As String)
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New DirectoryRegistryAccess(Path.Combine(root, "registry"))
                        Dim shortcuts As DirectoryShortcutAccess = CreateDirectoryShortcutAccess(root)
                        Dim original As C3Setup.InstalledState = ExecutePersistentInstall(root, manifestPath, registry, shortcuts)
                        Dim cataloguePath As String = Path.Combine(original.InstallRoot, "catalogue.xml")
                        Dim settingsPath As String = Path.Combine(original.InstallRoot, "user.settings")
                        File.WriteAllText(cataloguePath, "catalogue-preserve")
                        File.WriteAllText(settingsPath, "settings-preserve")
                        RunCrashChild("install", phase, root, manifestPath)
                        Dim journalPath As String = C3Setup.SetupTransactionJournalCodec.PathForInstallRoot(original.InstallRoot)
                        Dim interrupted As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournalCodec.Read(journalPath)
                        AssertEqual(phase, interrupted.Phase, "interrupted repair phase")
                        C3Setup.SetupTransactionRecovery.RecoverIncomplete(original.InstallRoot, shortcuts, registry)
                        AssertInstalledSurfaces(original.InstallRoot, registry, shortcuts)
                        Dim recovered As C3Setup.InstalledState = C3Setup.InstalledStateCodec.Read(Path.Combine(original.InstallRoot, C3Setup.InstalledStateCodec.FileName))
                        If phase = C3Setup.SetupTransactionPhases.Complete Then
                            AssertEqual(interrupted.TransactionId, recovered.TransactionId, "completed repair transaction")
                        Else
                            AssertEqual(original.TransactionId, recovered.TransactionId, "rolled-back repair transaction")
                        End If
                        AssertEqual("catalogue-preserve", File.ReadAllText(cataloguePath), "catalogue preserved across repair crash")
                        AssertEqual("settings-preserve", File.ReadAllText(settingsPath), "settings preserved across repair crash")
                    End Sub)
    End Sub

    Private Sub SetupStartupRecoversInterruptedPredecessor()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New DirectoryRegistryAccess(Path.Combine(root, "registry"))
                        Dim shortcuts As DirectoryShortcutAccess = CreateDirectoryShortcutAccess(root)
                        RunCrashChild("install", C3Setup.SetupTransactionPhases.PayloadPromoted, root, manifestPath)
                        Dim state As C3Setup.InstalledState = ExecutePersistentInstall(root, manifestPath, registry, shortcuts)
                        AssertInstalledSurfaces(state.InstallRoot, registry, shortcuts)
                        Dim journal As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournalCodec.Read(C3Setup.SetupTransactionJournalCodec.PathForInstallRoot(state.InstallRoot))
                        AssertEqual(C3Setup.SetupTransactionPhases.Complete, journal.Phase, "successor setup journal phase")
                    End Sub)
    End Sub

    Private Sub RecoveryRejectsAlteredPromotedBytes()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New DirectoryRegistryAccess(Path.Combine(root, "registry"))
                        Dim shortcuts As DirectoryShortcutAccess = CreateDirectoryShortcutAccess(root)
                        RunCrashChild("install", C3Setup.SetupTransactionPhases.PayloadPromoted, root, manifestPath)
                        Dim installRoot As String = CoordinatedInstallRoot(root)
                        File.AppendAllText(Path.Combine(installRoot, "Compact Cassette Catalogue.exe"), "altered")
                        AssertContractFailure(Sub() C3Setup.SetupTransactionRecovery.RecoverIncomplete(installRoot, shortcuts, registry))
                        Dim journal As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournalCodec.Read(C3Setup.SetupTransactionJournalCodec.PathForInstallRoot(installRoot))
                        AssertEqual(C3Setup.SetupTransactionPhases.RollbackStarted, journal.Phase, "failed-closed retained journal")
                    End Sub)
    End Sub

    Private Sub InstalledStateIsCommittedLast()
        WithPayload(Sub(root As String, manifestPath As String)
                        Dim registry As New MemoryRegistryAccess()
                        Dim shortcuts As MemoryShortcutAccess = CreateShortcutAccess(root)
                        Dim statePath As String = Path.Combine(CoordinatedInstallRoot(root), C3Setup.InstalledStateCodec.FileName)
                        Dim assertHidden As Action = Sub()
                                                           If File.Exists(statePath) Then Throw New Exception("Complete installed state was exposed before external surfaces were durable.")
                                                       End Sub
                        registry.BeforeWrite = assertHidden
                        shortcuts.BeforeWrite = assertHidden
                        Dim state As C3Setup.InstalledState = ExecuteCoordinatedInstall(root, manifestPath, registry, shortcuts, True, Nothing)
                        If Not File.Exists(statePath) Then Throw New Exception("Complete installed state was not exposed after durable integration.")
                        AssertInstalledSurfaces(state.InstallRoot, registry, shortcuts)
                    End Sub)
    End Sub

    Private Sub RunJournalCrashChild(arguments As String())
        If arguments.Length <> 6 Then Environment.Exit(98)
        Dim operation As String = arguments(2)
        Dim phase As String = arguments(3)
        Dim root As String = arguments(4)
        Dim manifestPath As String = arguments(5)
        Dim registry As New DirectoryRegistryAccess(Path.Combine(root, "registry"))
        Dim shortcuts As DirectoryShortcutAccess = CreateDirectoryShortcutAccess(root)
        Dim injector As Action(Of String) =
            Sub(point As String)
                If (phase = C3Setup.SetupTransactionPhases.RollbackStarted OrElse phase = C3Setup.SetupTransactionPhases.RollbackComplete) AndAlso
                        point = "after-first-file" Then Throw New InvalidOperationException("force-durable-rollback")
                If point = "journal:" & phase Then Environment.Exit(97)
            End Sub
        If operation = "install" Then
            ExecutePersistentInstall(root, manifestPath, registry, shortcuts, injector)
        ElseIf operation = "uninstall" Then
            C3Setup.SetupUninstallOperation.Execute(CoordinatedInstallRoot(root), shortcuts, registry, injector)
        Else
            Environment.Exit(98)
        End If
        Environment.Exit(99)
    End Sub

    Private Sub RunCrashChild(operation As String, phase As String, root As String, manifestPath As String)
        Dim executable As String = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim arguments As String = String.Join(" ", New String() {
            "--journal-crash-child",
            QuoteArgument(operation),
            QuoteArgument(phase),
            QuoteArgument(root),
            QuoteArgument(manifestPath)
        })
        Dim info As New System.Diagnostics.ProcessStartInfo(executable, arguments)
        info.UseShellExecute = False
        info.CreateNoWindow = True
        Dim child As System.Diagnostics.Process = System.Diagnostics.Process.Start(info)
        If Not child.WaitForExit(30000) Then
            child.Kill()
            Throw New Exception("Journal crash child timed out at " & operation & " " & phase & ".")
        End If
        If child.ExitCode <> 97 Then Throw New Exception("Journal crash child exited " & child.ExitCode.ToString(CultureInfo.InvariantCulture) & " at " & operation & " " & phase & ".")
    End Sub

    Private Function QuoteArgument(value As String) As String
        If value.Contains("""") Then Throw New Exception("Test argument contains a quote.")
        Return """" & value & """"
    End Function

    Private Function ExecutePersistentInstall(root As String,
                                              manifestPath As String,
                                              registry As DirectoryRegistryAccess,
                                              shortcuts As DirectoryShortcutAccess,
                                              Optional injector As Action(Of String) = Nothing) As C3Setup.InstalledState
        Dim programFiles As String = Path.Combine(root, "Program Files")
        Directory.CreateDirectory(programFiles)
        Dim facts As New C3Setup.SetupEnvironmentFacts("x86", "x86", True, True, 0, programFiles, False, 1048576)
        Return C3Setup.SetupInstallOperation.Execute(manifestPath,
                                                     Path.Combine(root, "payload"),
                                                     CoordinatedInstallRoot(root),
                                                     "89abcdef0123456789abcdef0123456789abcdef",
                                                     "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789",
                                                     True,
                                                     facts,
                                                     shortcuts,
                                                     registry,
                                                     injector)
    End Function

    Private Function CreateDirectoryShortcutAccess(root As String) As DirectoryShortcutAccess
        Dim programs As String = Path.Combine(root, "Common Programs")
        Dim desktop As String = Path.Combine(root, "Common Desktop")
        Directory.CreateDirectory(programs)
        Directory.CreateDirectory(desktop)
        Return New DirectoryShortcutAccess(programs, desktop)
    End Function

    Private Sub AssertInstalledSurfaces(installRoot As String,
                                        registry As C3Setup.ISetupRegistryAccess,
                                        shortcuts As C3Setup.ISetupShortcutAccess)
        Dim statePath As String = Path.Combine(installRoot, C3Setup.InstalledStateCodec.FileName)
        If Not File.Exists(statePath) Then Throw New Exception("Recovered install has no complete installed state.")
        Dim state As C3Setup.InstalledState = C3Setup.InstalledStateCodec.Read(statePath)
        C3Setup.PayloadVerifier.VerifyOwnedFiles(state.Manifest, installRoot)
        C3Setup.SetupRegistryRegistration.ValidateOwned(state, registry)
        C3Setup.SetupShortcutService.ValidateOwned(state, shortcuts)
    End Sub

    Private Sub AssertAbsentSurfaces(installRoot As String,
                                     registry As C3Setup.ISetupRegistryAccess,
                                     shortcuts As C3Setup.ISetupShortcutAccess)
        Dim statePath As String = Path.Combine(installRoot, C3Setup.InstalledStateCodec.FileName)
        If File.Exists(statePath) Then Throw New Exception("Rolled-back transaction exposed installed state.")
        If registry.ReadValues(C3Setup.InstalledStateCodec.UninstallKeyForLane("win-x86-net40")) IsNot Nothing Then Throw New Exception("Rolled-back transaction left HKLM registration state.")
        Dim expected As IList(Of C3Setup.InstalledShortcut) = C3Setup.SetupShortcutService.Plan(installRoot, shortcuts.CommonProgramsPath, shortcuts.CommonDesktopPath, True)
        For Each item As C3Setup.InstalledShortcut In expected
            If shortcuts.ReadShortcut(item.Path) IsNot Nothing Then Throw New Exception("Rolled-back transaction left a common shortcut.")
        Next
        For Each name As String In PayloadNames
            If File.Exists(Path.Combine(installRoot, name)) Then Throw New Exception("Rolled-back transaction left owned payload: " & name)
        Next
    End Sub

    Private Sub AssertJournalSettled(journalPath As String, completed As Boolean)
        Dim journal As C3Setup.SetupTransactionJournal = C3Setup.SetupTransactionJournalCodec.Read(journalPath)
        AssertEqual(If(completed, C3Setup.SetupTransactionPhases.Complete, C3Setup.SetupTransactionPhases.RollbackComplete), journal.Phase, "settled journal phase")
        If Directory.Exists(journal.StagingRoot) OrElse Directory.Exists(journal.BackupRoot) Then Throw New Exception("Settled journal retained a mutable work root.")
        Dim evidencePath As String = Path.Combine(C3Setup.SetupTransactionJournalCodec.EvidenceDirectoryForInstallRoot(journal.InstallRoot),
                                                  journal.TransactionId & "-" & journal.Phase & ".xml")
        If Not File.Exists(evidencePath) OrElse C3Setup.FileHash.Sha256(evidencePath) <> C3Setup.FileHash.Sha256(journalPath) Then
            Throw New Exception("Settled journal evidence was not retained byte-for-byte.")
        End If
    End Sub

    Private Function ExecuteCoordinatedInstall(root As String,
                                               manifestPath As String,
                                               registry As MemoryRegistryAccess,
                                               shortcuts As MemoryShortcutAccess,
                                               includeDesktop As Boolean,
                                               faultInjector As Action(Of String)) As C3Setup.InstalledState
        Dim programFiles As String = Path.Combine(root, "Program Files")
        Directory.CreateDirectory(programFiles)
        Dim facts As New C3Setup.SetupEnvironmentFacts("x86", "x86", True, True, 0, programFiles, False, 1048576)
        Return C3Setup.SetupInstallOperation.Execute(manifestPath,
                                                     Path.Combine(root, "payload"),
                                                     CoordinatedInstallRoot(root),
                                                     "89abcdef0123456789abcdef0123456789abcdef",
                                                     "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789",
                                                     includeDesktop,
                                                     facts,
                                                     shortcuts,
                                                     registry,
                                                     faultInjector)
    End Function

    Private Function CoordinatedInstallRoot(root As String) As String
        Return Path.Combine(root, "Program Files", "Compact Cassette Catalogue")
    End Function

    Private Function CreateShortcutAccess(root As String) As MemoryShortcutAccess
        Dim programs As String = Path.Combine(root, "Common Programs")
        Dim desktop As String = Path.Combine(root, "Common Desktop")
        Directory.CreateDirectory(programs)
        Directory.CreateDirectory(desktop)
        Return New MemoryShortcutAccess(programs, desktop)
    End Function

    Private Sub WithPayload(action As Action(Of String, String))
        Dim root As String = Path.Combine(Path.GetTempPath(), "C3SetupTests-" & Guid.NewGuid().ToString("N"))
        Dim payload As String = Path.Combine(root, "payload")
        Directory.CreateDirectory(payload)
        Try
            For index As Integer = 0 To PayloadNames.Length - 1
                File.WriteAllText(Path.Combine(payload, PayloadNames(index)), "payload-" & index.ToString(CultureInfo.InvariantCulture))
            Next
            Dim manifestPath As String = Path.Combine(root, "payload.xml")
            File.WriteAllText(manifestPath, BuildManifest(payload))
            action(root, manifestPath)
        Finally
            If Directory.Exists(root) Then
                Directory.Delete(root, True)
            End If
        End Try
    End Sub

    Private Function BuildManifest(payload As String) As String
        Dim builder As New StringBuilder()
        builder.AppendLine("<C3SetupPayload schemaVersion=""1"">")
        builder.AppendLine("  <Product version=""1.3.0"" stage=""Alpha 3"" label=""1.3.0a3"" lane=""win-x86-net40"" architecture=""x86"" framework=""v4.0"" sourceCommit=""0123456789abcdef0123456789abcdef01234567"" />")
        builder.AppendLine("  <Files>")
        For Each name As String In PayloadNames
            Dim filePath As String = Path.Combine(payload, name)
            builder.AppendFormat(CultureInfo.InvariantCulture,
                                 "    <File path=""{0}"" size=""{1}"" sha256=""{2}"" />{3}",
                                 name,
                                 New FileInfo(filePath).Length,
                                 C3Setup.FileHash.Sha256(filePath),
                                 Environment.NewLine)
        Next
        builder.AppendLine("  </Files>")
        builder.AppendLine("</C3SetupPayload>")
        Return builder.ToString()
    End Function

    Private Sub AssertContractFailure(action As Action)
        Try
            action()
        Catch ex As C3Setup.SetupContractException
            Return
        End Try
        Throw New Exception("Expected SetupContractException.")
    End Sub

    Private Sub AssertEqual(expected As String, actual As String, name As String)
        If Not String.Equals(expected, actual, StringComparison.Ordinal) Then
            Throw New Exception(String.Format(CultureInfo.InvariantCulture,
                                              "{0}: expected '{1}', found '{2}'.",
                                              name,
                                              expected,
                                              actual))
        End If
    End Sub

    Private NotInheritable Class MemoryRegistryAccess
        Implements C3Setup.ISetupRegistryAccess

        Private ReadOnly _keys As New Dictionary(Of String, IDictionary(Of String, Object))(StringComparer.Ordinal)
        Public Property FailNextWrite As Boolean
        Public Property BeforeWrite As Action

        Public Function ReadValues(keyPath As String) As IDictionary(Of String, Object) Implements C3Setup.ISetupRegistryAccess.ReadValues
            If Not _keys.ContainsKey(keyPath) Then Return Nothing
            Return New Dictionary(Of String, Object)(_keys(keyPath), StringComparer.Ordinal)
        End Function

        Public Sub WriteValues(keyPath As String, values As IDictionary(Of String, Object)) Implements C3Setup.ISetupRegistryAccess.WriteValues
            If BeforeWrite IsNot Nothing Then BeforeWrite.Invoke()
            If FailNextWrite Then
                FailNextWrite = False
                Throw New InvalidOperationException("registry-write-injected")
            End If
            _keys(keyPath) = New Dictionary(Of String, Object)(values, StringComparer.Ordinal)
        End Sub

        Public Sub DeleteKey(keyPath As String) Implements C3Setup.ISetupRegistryAccess.DeleteKey
            _keys.Remove(keyPath)
        End Sub
    End Class

    Private NotInheritable Class DirectoryRegistryAccess
        Implements C3Setup.ISetupRegistryAccess

        Private ReadOnly _root As String

        Public Sub New(root As String)
            _root = root
            If Not Directory.Exists(_root) Then Directory.CreateDirectory(_root)
        End Sub

        Public Function ReadValues(keyPath As String) As IDictionary(Of String, Object) Implements C3Setup.ISetupRegistryAccess.ReadValues
            Dim path As String = PathForKey(keyPath)
            If Not File.Exists(path) Then Return Nothing
            Dim result As New Dictionary(Of String, Object)(StringComparer.Ordinal)
            For Each line As String In File.ReadAllLines(path)
                Dim parts As String() = line.Split(New Char() {"|"c}, 3)
                If parts.Length <> 3 Then Throw New Exception("Persistent registry fixture is malformed.")
                Dim name As String = Encoding.UTF8.GetString(Convert.FromBase64String(parts(0)))
                If parts(1) = "I" Then
                    result.Add(name, Integer.Parse(parts(2), CultureInfo.InvariantCulture))
                ElseIf parts(1) = "S" Then
                    result.Add(name, Encoding.UTF8.GetString(Convert.FromBase64String(parts(2))))
                Else
                    Throw New Exception("Persistent registry fixture type is malformed.")
                End If
            Next
            Return result
        End Function

        Public Sub WriteValues(keyPath As String, values As IDictionary(Of String, Object)) Implements C3Setup.ISetupRegistryAccess.WriteValues
            Dim names As New List(Of String)(values.Keys)
            names.Sort(StringComparer.Ordinal)
            Dim lines As New List(Of String)()
            For Each name As String In names
                Dim encodedName As String = Convert.ToBase64String(Encoding.UTF8.GetBytes(name))
                If TypeOf values(name) Is Integer Then
                    lines.Add(encodedName & "|I|" & DirectCast(values(name), Integer).ToString(CultureInfo.InvariantCulture))
                ElseIf TypeOf values(name) Is String Then
                    lines.Add(encodedName & "|S|" & Convert.ToBase64String(Encoding.UTF8.GetBytes(DirectCast(values(name), String))))
                Else
                    Throw New Exception("Persistent registry fixture received an unsupported type.")
                End If
            Next
            File.WriteAllLines(PathForKey(keyPath), lines.ToArray())
        End Sub

        Public Sub DeleteKey(keyPath As String) Implements C3Setup.ISetupRegistryAccess.DeleteKey
            Dim path As String = PathForKey(keyPath)
            If File.Exists(path) Then File.Delete(path)
        End Sub

        Private Function PathForKey(keyPath As String) As String
            Return Path.Combine(_root, keyPath.Replace("\", "_") & ".txt")
        End Function
    End Class

    Private NotInheritable Class DirectoryShortcutAccess
        Implements C3Setup.ISetupShortcutAccess

        Public Sub New(programs As String, desktop As String)
            Me.CommonProgramsPath = programs
            Me.CommonDesktopPath = desktop
        End Sub

        Public ReadOnly Property CommonProgramsPath As String Implements C3Setup.ISetupShortcutAccess.CommonProgramsPath
        Public ReadOnly Property CommonDesktopPath As String Implements C3Setup.ISetupShortcutAccess.CommonDesktopPath

        Public Function ReadShortcut(path As String) As C3Setup.SetupShortcut Implements C3Setup.ISetupShortcutAccess.ReadShortcut
            If Not File.Exists(path) Then Return Nothing
            Dim lines As String() = File.ReadAllLines(path)
            If lines.Length <> 3 Then Throw New Exception("Persistent shortcut fixture is malformed.")
            Return New C3Setup.SetupShortcut(path,
                                             Encoding.UTF8.GetString(Convert.FromBase64String(lines(0))),
                                             Encoding.UTF8.GetString(Convert.FromBase64String(lines(1))),
                                             Encoding.UTF8.GetString(Convert.FromBase64String(lines(2))))
        End Function

        Public Sub WriteShortcut(value As C3Setup.SetupShortcut) Implements C3Setup.ISetupShortcutAccess.WriteShortcut
            Dim parent As String = Directory.GetParent(value.Path).FullName
            If Not Directory.Exists(parent) Then Directory.CreateDirectory(parent)
            File.WriteAllLines(value.Path, New String() {
                Convert.ToBase64String(Encoding.UTF8.GetBytes(value.Target)),
                Convert.ToBase64String(Encoding.UTF8.GetBytes(value.WorkingDirectory)),
                Convert.ToBase64String(Encoding.UTF8.GetBytes(value.Description))
            })
        End Sub

        Public Sub DeleteShortcut(path As String) Implements C3Setup.ISetupShortcutAccess.DeleteShortcut
            If File.Exists(path) Then File.Delete(path)
        End Sub
    End Class

    Private NotInheritable Class MemoryShortcutAccess
        Implements C3Setup.ISetupShortcutAccess

        Private ReadOnly _shortcuts As New Dictionary(Of String, C3Setup.SetupShortcut)(StringComparer.OrdinalIgnoreCase)

        Public Sub New(programs As String, desktop As String)
            Me.CommonProgramsPath = programs
            Me.CommonDesktopPath = desktop
        End Sub

        Public ReadOnly Property CommonProgramsPath As String Implements C3Setup.ISetupShortcutAccess.CommonProgramsPath
        Public ReadOnly Property CommonDesktopPath As String Implements C3Setup.ISetupShortcutAccess.CommonDesktopPath
        Public Property FailDeletePath As String
        Public Property BeforeWrite As Action

        Public Function ReadShortcut(path As String) As C3Setup.SetupShortcut Implements C3Setup.ISetupShortcutAccess.ReadShortcut
            If Not _shortcuts.ContainsKey(path) Then Return Nothing
            Dim value As C3Setup.SetupShortcut = _shortcuts(path)
            Return New C3Setup.SetupShortcut(value.Path, value.Target, value.WorkingDirectory, value.Description)
        End Function

        Public Sub WriteShortcut(value As C3Setup.SetupShortcut) Implements C3Setup.ISetupShortcutAccess.WriteShortcut
            If BeforeWrite IsNot Nothing Then BeforeWrite.Invoke()
            _shortcuts(value.Path) = New C3Setup.SetupShortcut(value.Path, value.Target, value.WorkingDirectory, value.Description)
        End Sub

        Public Sub DeleteShortcut(path As String) Implements C3Setup.ISetupShortcutAccess.DeleteShortcut
            If String.Equals(path, FailDeletePath, StringComparison.OrdinalIgnoreCase) Then
                FailDeletePath = Nothing
                Throw New InvalidOperationException("shortcut-delete-injected")
            End If
            _shortcuts.Remove(path)
        End Sub
    End Class

    Private NotInheritable Class MemoryProcessLauncher
        Implements C3Setup.ISetupProcessLauncher

        Public Property ExecutablePath As String
        Public Property Arguments As String
        Public Property WorkingDirectory As String

        Public Sub Start(executablePath As String, arguments As String, workingDirectory As String) Implements C3Setup.ISetupProcessLauncher.Start
            Me.ExecutablePath = executablePath
            Me.Arguments = arguments
            Me.WorkingDirectory = workingDirectory
        End Sub
    End Class

End Module
