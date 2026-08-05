Imports System.Globalization
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml

Namespace Global.C3Setup

    Public NotInheritable Class PayloadFile

        Public Sub New(path As String, length As Long, sha256 As String)
            Me.Path = path
            Me.Length = length
            Me.Sha256 = sha256
        End Sub

        Public ReadOnly Property Path As String
        Public ReadOnly Property Length As Long
        Public ReadOnly Property Sha256 As String

    End Class

    Public NotInheritable Class PayloadManifest

        Public Sub New(version As String,
                       stage As String,
                       label As String,
                       lane As String,
                       architecture As String,
                       framework As String,
                       sourceCommit As String,
                       files As IList(Of PayloadFile))
            Me.Version = version
            Me.Stage = stage
            Me.Label = label
            Me.Lane = lane
            Me.Architecture = architecture
            Me.Framework = framework
            Me.SourceCommit = sourceCommit
            Me.Files = New List(Of PayloadFile)(files).AsReadOnly()
        End Sub

        Public ReadOnly Property Version As String
        Public ReadOnly Property Stage As String
        Public ReadOnly Property Label As String
        Public ReadOnly Property Lane As String
        Public ReadOnly Property Architecture As String
        Public ReadOnly Property Framework As String
        Public ReadOnly Property SourceCommit As String
        Public ReadOnly Property Files As IList(Of PayloadFile)

    End Class

    Public NotInheritable Class SetupContractException
        Inherits Exception

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub
    End Class

    Public NotInheritable Class PayloadManifestReader

        Private Shared ReadOnly AllowedPayloadNames As String() = {
            "BUILD.txt",
            "Compact Cassette Catalogue.exe",
            "Compact Cassette Catalogue.exe.config",
            "README.txt",
            "RELEASE_NOTES.txt",
            "UNINSTALL.exe",
            "UNINSTALL.exe.config"
        }

        Private Sub New()
        End Sub

        Public Shared Function Read(manifestPath As String) As PayloadManifest
            If String.IsNullOrWhiteSpace(manifestPath) Then
                Throw New SetupContractException("The payload manifest path is required.")
            End If

            Dim settings As New XmlReaderSettings()
            settings.DtdProcessing = DtdProcessing.Prohibit
            settings.XmlResolver = Nothing
            settings.IgnoreComments = False
            settings.IgnoreProcessingInstructions = False

            Dim document As New XmlDocument()
            document.XmlResolver = Nothing
            Try
                Using reader As XmlReader = XmlReader.Create(manifestPath, settings)
                    document.Load(reader)
                End Using
            Catch ex As XmlException
                Throw New SetupContractException("The payload manifest is not safe, well-formed XML.", ex)
            End Try

            Dim root As XmlElement = document.DocumentElement
            RequireElement(root, "C3SetupPayload")
            RequireAttributes(root, New String() {"schemaVersion"})
            If root.GetAttribute("schemaVersion") <> "1" Then
                Throw New SetupContractException("The payload manifest schemaVersion must be 1.")
            End If

            Dim rootChildren As IList(Of XmlElement) = ElementChildren(root)
            If rootChildren.Count <> 2 OrElse
                    rootChildren(0).Name <> "Product" OrElse
                    rootChildren(1).Name <> "Files" Then
                Throw New SetupContractException("The payload manifest must contain Product then Files exactly once.")
            End If

            Dim product As XmlElement = rootChildren(0)
            RequireAttributes(product, New String() {
                "architecture", "framework", "label", "lane", "sourceCommit", "stage", "version"
            })
            If ElementChildren(product).Count <> 0 OrElse Not String.IsNullOrWhiteSpace(product.InnerText) Then
                Throw New SetupContractException("Product must not contain child content.")
            End If

            Dim version As String = RequiredValue(product, "version")
            Dim stage As String = RequiredValue(product, "stage")
            Dim label As String = RequiredValue(product, "label")
            Dim lane As String = RequiredValue(product, "lane")
            Dim architecture As String = RequiredValue(product, "architecture")
            Dim framework As String = RequiredValue(product, "framework")
            Dim sourceCommit As String = RequiredValue(product, "sourceCommit")
            If Not Regex.IsMatch(sourceCommit, "^[0-9a-f]{40}$", RegexOptions.CultureInvariant) Then
                Throw New SetupContractException("Product sourceCommit must be 40 lowercase hexadecimal characters.")
            End If

            Dim filesElement As XmlElement = rootChildren(1)
            RequireAttributes(filesElement, New String() {})
            Dim fileElements As IList(Of XmlElement) = ElementChildren(filesElement)
            If fileElements.Count <> AllowedPayloadNames.Length Then
                Throw New SetupContractException("The payload manifest must contain exactly seven files.")
            End If

            Dim seen As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            Dim files As New List(Of PayloadFile)()
            For Each fileElement As XmlElement In fileElements
                RequireElement(fileElement, "File")
                RequireAttributes(fileElement, New String() {"path", "sha256", "size"})
                If ElementChildren(fileElement).Count <> 0 OrElse Not String.IsNullOrWhiteSpace(fileElement.InnerText) Then
                    Throw New SetupContractException("File must not contain child content.")
                End If

                Dim relativePath As String = RequiredValue(fileElement, "path")
                SetupPathPolicy.RequirePayloadFileName(relativePath)
                If Array.IndexOf(AllowedPayloadNames, relativePath) < 0 Then
                    Throw New SetupContractException("Unexpected payload file name: " & relativePath)
                End If
                If seen.ContainsKey(relativePath) Then
                    Throw New SetupContractException("Duplicate or case-colliding payload file: " & relativePath)
                End If
                seen.Add(relativePath, True)

                Dim length As Long
                If Not Long.TryParse(RequiredValue(fileElement, "size"), NumberStyles.None, CultureInfo.InvariantCulture, length) OrElse length < 0 Then
                    Throw New SetupContractException("File size must be a non-negative invariant integer: " & relativePath)
                End If
                Dim sha256 As String = RequiredValue(fileElement, "sha256")
                If Not Regex.IsMatch(sha256, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant) Then
                    Throw New SetupContractException("File SHA-256 must be 64 lowercase hexadecimal characters: " & relativePath)
                End If
                files.Add(New PayloadFile(relativePath, length, sha256))
            Next

            For Each requiredName As String In AllowedPayloadNames
                If Not seen.ContainsKey(requiredName) Then
                    Throw New SetupContractException("The payload manifest is missing: " & requiredName)
                End If
            Next

            Return New PayloadManifest(version, stage, label, lane, architecture, framework, sourceCommit, files)
        End Function

        Private Shared Function ElementChildren(parent As XmlElement) As IList(Of XmlElement)
            Dim children As New List(Of XmlElement)()
            For Each node As XmlNode In parent.ChildNodes
                If node.NodeType = XmlNodeType.Element Then
                    children.Add(DirectCast(node, XmlElement))
                ElseIf node.NodeType <> XmlNodeType.Whitespace AndAlso
                        node.NodeType <> XmlNodeType.SignificantWhitespace Then
                    Throw New SetupContractException("Unexpected XML node below " & parent.Name & ".")
                End If
            Next
            Return children
        End Function

        Private Shared Sub RequireElement(element As XmlElement, expectedName As String)
            If element Is Nothing OrElse element.Name <> expectedName OrElse element.NamespaceURI <> String.Empty Then
                Throw New SetupContractException("Expected unqualified element " & expectedName & ".")
            End If
        End Sub

        Private Shared Sub RequireAttributes(element As XmlElement, allowedNames As String())
            If element.Attributes.Count <> allowedNames.Length Then
                Throw New SetupContractException("Element " & element.Name & " has missing or unexpected attributes.")
            End If
            For Each attribute As XmlAttribute In element.Attributes
                If attribute.NamespaceURI <> String.Empty OrElse Array.IndexOf(allowedNames, attribute.Name) < 0 Then
                    Throw New SetupContractException("Unexpected attribute on " & element.Name & ": " & attribute.Name)
                End If
            Next
        End Sub

        Private Shared Function RequiredValue(element As XmlElement, attributeName As String) As String
            Dim value As String = element.GetAttribute(attributeName)
            If String.IsNullOrWhiteSpace(value) OrElse value <> value.Trim() Then
                Throw New SetupContractException(element.Name & " attribute " & attributeName & " is missing or not canonical.")
            End If
            Return value
        End Function

    End Class

    Public NotInheritable Class PayloadVerifier

        Private Sub New()
        End Sub

        Public Shared Sub Verify(manifest As PayloadManifest, payloadDirectory As String)
            If manifest Is Nothing Then
                Throw New ArgumentNullException("manifest")
            End If
            Dim root As String = SetupPathPolicy.CanonicalDirectory(payloadDirectory)
            If Not Directory.Exists(root) Then
                Throw New SetupContractException("The payload directory does not exist.")
            End If

            Dim expected As New Dictionary(Of String, PayloadFile)(StringComparer.OrdinalIgnoreCase)
            For Each item As PayloadFile In manifest.Files
                expected.Add(item.Path, item)
            Next

            Dim actualFiles As FileInfo() = New DirectoryInfo(root).GetFiles()
            Dim actualDirectories As DirectoryInfo() = New DirectoryInfo(root).GetDirectories()
            If actualDirectories.Length <> 0 OrElse actualFiles.Length <> expected.Count Then
                Throw New SetupContractException("The payload directory is not the closed seven-file set.")
            End If
            For Each actual As FileInfo In actualFiles
                If (actual.Attributes And FileAttributes.ReparsePoint) <> 0 OrElse Not expected.ContainsKey(actual.Name) Then
                    Throw New SetupContractException("Unexpected or unsafe payload entry: " & actual.Name)
                End If
            Next

            For Each item As PayloadFile In manifest.Files
                Dim fullPath As String = SetupPathPolicy.CombineOwnedFile(root, item.Path)
                Dim file As New FileInfo(fullPath)
                If Not file.Exists OrElse file.Length <> item.Length Then
                    Throw New SetupContractException("Payload file length mismatch: " & item.Path)
                End If
                Dim actualHash As String = FileHash.Sha256(fullPath)
                If Not String.Equals(actualHash, item.Sha256, StringComparison.Ordinal) Then
                    Throw New SetupContractException("Payload file hash mismatch: " & item.Path)
                End If
            Next
        End Sub

    End Class

    Public NotInheritable Class SetupPathPolicy

        Private Sub New()
        End Sub

        Public Shared Sub RequirePayloadFileName(relativePath As String)
            If String.IsNullOrWhiteSpace(relativePath) OrElse
                    relativePath <> Path.GetFileName(relativePath) OrElse
                    relativePath.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 OrElse
                    relativePath.Contains(":") OrElse
                    relativePath = "." OrElse relativePath = ".." Then
                Throw New SetupContractException("Payload paths must be safe root-level file names.")
            End If
        End Sub

        Public Shared Function CanonicalDirectory(directoryPath As String) As String
            If String.IsNullOrWhiteSpace(directoryPath) OrElse directoryPath.StartsWith("\\", StringComparison.Ordinal) OrElse
                    directoryPath.StartsWith("\\?\", StringComparison.Ordinal) OrElse directoryPath.StartsWith("\\.\", StringComparison.Ordinal) Then
                Throw New SetupContractException("A local absolute directory is required.")
            End If
            Dim fullPath As String = System.IO.Path.GetFullPath(directoryPath).TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
            If String.IsNullOrWhiteSpace(fullPath) OrElse Not System.IO.Path.IsPathRooted(fullPath) Then
                Throw New SetupContractException("A local absolute directory is required.")
            End If
            Return fullPath
        End Function

        Public Shared Function ValidateInstallRoot(directoryPath As String) As String
            Dim fullPath As String = CanonicalDirectory(directoryPath)
            Dim prohibited As New List(Of String)()
            prohibited.Add(Path.GetPathRoot(fullPath).TrimEnd(Path.DirectorySeparatorChar))
            prohibited.Add(Environment.GetFolderPath(Environment.SpecialFolder.Windows).TrimEnd(Path.DirectorySeparatorChar))
            prohibited.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).TrimEnd(Path.DirectorySeparatorChar))
            prohibited.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).TrimEnd(Path.DirectorySeparatorChar))
            prohibited.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).TrimEnd(Path.DirectorySeparatorChar))
            For Each prohibitedPath As String In prohibited
                If Not String.IsNullOrWhiteSpace(prohibitedPath) AndAlso
                        String.Equals(fullPath, prohibitedPath, StringComparison.OrdinalIgnoreCase) Then
                    Throw New SetupContractException("The selected install root is too broad or system-owned.")
                End If
            Next

            Dim current As DirectoryInfo = New DirectoryInfo(fullPath)
            Do While current IsNot Nothing AndAlso current.Exists
                If (current.Attributes And FileAttributes.ReparsePoint) <> 0 Then
                    Throw New SetupContractException("The install path traverses a reparse point.")
                End If
                current = current.Parent
            Loop
            Return fullPath
        End Function

        Public Shared Function CombineOwnedFile(root As String, relativeName As String) As String
            RequirePayloadFileName(relativeName)
            Dim canonicalRoot As String = CanonicalDirectory(root)
            Dim fullPath As String = Path.GetFullPath(Path.Combine(canonicalRoot, relativeName))
            Dim prefix As String = canonicalRoot.TrimEnd(Path.DirectorySeparatorChar) & Path.DirectorySeparatorChar
            If Not fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("Owned file escapes its root.")
            End If
            Return fullPath
        End Function

    End Class

    Public NotInheritable Class FileHash

        Private Sub New()
        End Sub

        Public Shared Function Sha256(path As String) As String
            Using stream As FileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                Using algorithm As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
                    Dim bytes As Byte() = algorithm.ComputeHash(stream)
                    Dim result As New StringBuilder(bytes.Length * 2)
                    For Each value As Byte In bytes
                        result.Append(value.ToString("x2", CultureInfo.InvariantCulture))
                    Next
                    Return result.ToString()
                End Using
            End Using
        End Function

    End Class

End Namespace
