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
        RunTest("clean file transaction installs exact owned bytes", AddressOf CleanTransactionInstallsOwnedBytes)
        RunTest("repair preserves unknown files", AddressOf RepairPreservesUnknownFiles)
        RunTest("faulted repair rolls back exact prior bytes", AddressOf FaultedRepairRollsBack)
        RunTest("unowned collision is rejected without mutation", AddressOf UnownedCollisionIsRejected)
        RunTest("removal deletes only owned files", AddressOf RemovalDeletesOnlyOwnedFiles)
        RunTest("modified owned file blocks removal", AddressOf ModifiedOwnedFileBlocksRemoval)
        RunTest("faulted removal restores exact installed state", AddressOf FaultedRemovalRestoresState)

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

End Module
