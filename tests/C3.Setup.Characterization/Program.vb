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
