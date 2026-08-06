Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml

Namespace Global.C3Setup

    Public NotInheritable Class InstalledShortcut

        Public Sub New(path As String, target As String)
            Me.Path = path
            Me.Target = target
        End Sub

        Public ReadOnly Property Path As String
        Public ReadOnly Property Target As String

    End Class

    Public NotInheritable Class InstalledState

        Public Sub New(manifest As PayloadManifest,
                       setupSourceCommit As String,
                       installRoot As String,
                       mode As String,
                       transactionId As String,
                       installedAtUtc As DateTime,
                       payloadManifestSha256 As String,
                       setupExecutableSha256 As String,
                       shortcuts As IList(Of InstalledShortcut))
            Me.Manifest = manifest
            Me.SetupSourceCommit = setupSourceCommit
            Me.InstallRoot = installRoot
            Me.Mode = mode
            Me.TransactionId = transactionId
            Me.InstalledAtUtc = installedAtUtc
            Me.PayloadManifestSha256 = payloadManifestSha256
            Me.SetupExecutableSha256 = setupExecutableSha256
            Me.Shortcuts = New List(Of InstalledShortcut)(shortcuts).AsReadOnly()
        End Sub

        Public ReadOnly Property Manifest As PayloadManifest
        Public ReadOnly Property SetupSourceCommit As String
        Public ReadOnly Property InstallRoot As String
        Public ReadOnly Property Mode As String
        Public ReadOnly Property TransactionId As String
        Public ReadOnly Property InstalledAtUtc As DateTime
        Public ReadOnly Property PayloadManifestSha256 As String
        Public ReadOnly Property SetupExecutableSha256 As String
        Public ReadOnly Property Shortcuts As IList(Of InstalledShortcut)

    End Class

    Public NotInheritable Class InstalledStateCodec

        Public Const FileName As String = "C3.installed.xml"
        Private Const UninstallKeyPrefix As String = "Software\Microsoft\Windows\CurrentVersion\Uninstall\CompactCassetteCatalogue-1x-"

        Private Sub New()
        End Sub

        Public Shared Sub Write(path As String, state As InstalledState)
            ValidateState(state)
            Dim settings As New XmlWriterSettings()
            settings.Encoding = New UTF8Encoding(False)
            settings.Indent = True
            settings.NewLineChars = Environment.NewLine
            settings.NewLineHandling = NewLineHandling.Replace
            Using writer As XmlWriter = XmlWriter.Create(path, settings)
                writer.WriteStartDocument()
                writer.WriteStartElement("C3InstalledState")
                writer.WriteAttributeString("schemaVersion", "1")
                writer.WriteAttributeString("complete", "true")

                writer.WriteStartElement("Product")
                writer.WriteAttributeString("version", state.Manifest.Version)
                writer.WriteAttributeString("stage", state.Manifest.Stage)
                writer.WriteAttributeString("label", state.Manifest.Label)
                writer.WriteAttributeString("lane", state.Manifest.Lane)
                writer.WriteAttributeString("architecture", state.Manifest.Architecture)
                writer.WriteAttributeString("framework", state.Manifest.Framework)
                writer.WriteAttributeString("sourceCommit", state.Manifest.SourceCommit)
                writer.WriteAttributeString("setupSourceCommit", state.SetupSourceCommit)
                writer.WriteEndElement()

                writer.WriteStartElement("Installation")
                writer.WriteAttributeString("scope", "perMachine")
                writer.WriteAttributeString("root", state.InstallRoot)
                writer.WriteAttributeString("mode", state.Mode)
                writer.WriteAttributeString("transactionId", state.TransactionId)
                writer.WriteAttributeString("installedAtUtc", state.InstalledAtUtc.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture))
                writer.WriteAttributeString("payloadManifestSha256", state.PayloadManifestSha256)
                writer.WriteAttributeString("setupExecutableSha256", state.SetupExecutableSha256)
                writer.WriteEndElement()

                writer.WriteStartElement("Files")
                For Each item As PayloadFile In state.Manifest.Files
                    writer.WriteStartElement("File")
                    writer.WriteAttributeString("path", item.Path)
                    writer.WriteAttributeString("size", item.Length.ToString(CultureInfo.InvariantCulture))
                    writer.WriteAttributeString("sha256", item.Sha256)
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                writer.WriteStartElement("Registry")
                writer.WriteAttributeString("hive", "HKLM")
                writer.WriteAttributeString("view", "native")
                writer.WriteAttributeString("uninstallKey", UninstallKeyForLane(state.Manifest.Lane))
                writer.WriteEndElement()

                writer.WriteStartElement("Shortcuts")
                For Each shortcut As InstalledShortcut In state.Shortcuts
                    writer.WriteStartElement("Shortcut")
                    writer.WriteAttributeString("path", shortcut.Path)
                    writer.WriteAttributeString("target", shortcut.Target)
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                writer.WriteEndElement()
                writer.WriteEndDocument()
            End Using
        End Sub

        Public Shared Function Read(path As String) As InstalledState
            Dim settings As New XmlReaderSettings()
            settings.DtdProcessing = DtdProcessing.Prohibit
            settings.XmlResolver = Nothing
            settings.IgnoreComments = False
            settings.IgnoreProcessingInstructions = False
            Dim document As New XmlDocument()
            document.XmlResolver = Nothing
            Try
                Using reader As XmlReader = XmlReader.Create(path, settings)
                    document.Load(reader)
                End Using
            Catch ex As XmlException
                Throw New SetupContractException("The installed-state manifest is not safe, well-formed XML.", ex)
            End Try

            Dim root As XmlElement = document.DocumentElement
            RequireElement(root, "C3InstalledState")
            RequireAttributes(root, New String() {"complete", "schemaVersion"})
            If root.GetAttribute("schemaVersion") <> "1" OrElse root.GetAttribute("complete") <> "true" Then
                Throw New SetupContractException("Installed state must be complete schema version 1.")
            End If
            Dim children As IList(Of XmlElement) = ElementChildren(root)
            Dim expectedChildren As String() = {"Product", "Installation", "Files", "Registry", "Shortcuts"}
            If children.Count <> expectedChildren.Length Then
                Throw New SetupContractException("Installed state has an incomplete child set.")
            End If
            For index As Integer = 0 To expectedChildren.Length - 1
                RequireElement(children(index), expectedChildren(index))
            Next

            Dim product As XmlElement = children(0)
            RequireAttributes(product, New String() {"architecture", "framework", "label", "lane", "setupSourceCommit", "sourceCommit", "stage", "version"})
            RequireEmpty(product)
            Dim sourceCommit As String = RequiredHash(product, "sourceCommit", 40)
            Dim setupSourceCommit As String = RequiredHash(product, "setupSourceCommit", 40)

            Dim installation As XmlElement = children(1)
            RequireAttributes(installation, New String() {"installedAtUtc", "mode", "payloadManifestSha256", "root", "scope", "setupExecutableSha256", "transactionId"})
            RequireEmpty(installation)
            If installation.GetAttribute("scope") <> "perMachine" Then
                Throw New SetupContractException("Installed state scope must be perMachine.")
            End If
            Dim installRoot As String = SetupPathPolicy.ValidateInstallRoot(RequiredValue(installation, "root"))
            Dim mode As String = RequiredValue(installation, "mode")
            If Array.IndexOf(New String() {"install", "repair", "upgrade"}, mode) < 0 Then
                Throw New SetupContractException("Installed state mode is invalid.")
            End If
            Dim transactionId As String = RequiredValue(installation, "transactionId")
            If Not Regex.IsMatch(transactionId, "^[0-9a-f]{32}$", RegexOptions.CultureInvariant) Then
                Throw New SetupContractException("Installed state transactionId is invalid.")
            End If
            Dim installedAtUtc As DateTime
            If Not DateTime.TryParseExact(RequiredValue(installation, "installedAtUtc"), "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, installedAtUtc) Then
                Throw New SetupContractException("Installed state timestamp is invalid.")
            End If
            Dim payloadManifestHash As String = RequiredHash(installation, "payloadManifestSha256", 64)
            Dim setupExecutableHash As String = RequiredHash(installation, "setupExecutableSha256", 64)

            Dim filesElement As XmlElement = children(2)
            RequireAttributes(filesElement, New String() {})
            Dim files As New List(Of PayloadFile)()
            Dim seen As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            For Each fileElement As XmlElement In ElementChildren(filesElement)
                RequireElement(fileElement, "File")
                RequireAttributes(fileElement, New String() {"path", "sha256", "size"})
                RequireEmpty(fileElement)
                Dim relativePath As String = RequiredValue(fileElement, "path")
                SetupPathPolicy.RequirePayloadFileName(relativePath)
                If Not SetupContractNames.IsPayloadFileName(relativePath) OrElse seen.ContainsKey(relativePath) Then
                    Throw New SetupContractException("Installed state has an unexpected or duplicate owned file.")
                End If
                seen.Add(relativePath, True)
                Dim length As Long
                If Not Long.TryParse(RequiredValue(fileElement, "size"), NumberStyles.None, CultureInfo.InvariantCulture, length) OrElse length < 0 Then
                    Throw New SetupContractException("Installed-state file size is invalid.")
                End If
                files.Add(New PayloadFile(relativePath, length, RequiredHash(fileElement, "sha256", 64)))
            Next
            If files.Count <> SetupContractNames.PayloadFileNames().Length Then
                Throw New SetupContractException("Installed state does not own the exact payload set.")
            End If

            Dim registry As XmlElement = children(3)
            RequireAttributes(registry, New String() {"hive", "uninstallKey", "view"})
            RequireEmpty(registry)
            If registry.GetAttribute("hive") <> "HKLM" OrElse registry.GetAttribute("view") <> "native" OrElse
                    registry.GetAttribute("uninstallKey") <> UninstallKeyForLane(RequiredValue(product, "lane")) Then
                Throw New SetupContractException("Installed-state registry ownership is invalid.")
            End If

            Dim shortcuts As New List(Of InstalledShortcut)()
            Dim shortcutsElement As XmlElement = children(4)
            RequireAttributes(shortcutsElement, New String() {})
            For Each shortcutElement As XmlElement In ElementChildren(shortcutsElement)
                RequireElement(shortcutElement, "Shortcut")
                RequireAttributes(shortcutElement, New String() {"path", "target"})
                RequireEmpty(shortcutElement)
                shortcuts.Add(New InstalledShortcut(RequiredValue(shortcutElement, "path"), RequiredValue(shortcutElement, "target")))
            Next

            Dim manifest As New PayloadManifest(RequiredValue(product, "version"),
                                                RequiredValue(product, "stage"),
                                                RequiredValue(product, "label"),
                                                RequiredValue(product, "lane"),
                                                RequiredValue(product, "architecture"),
                                                RequiredValue(product, "framework"),
                                                sourceCommit,
                                                files)
            Dim state As New InstalledState(manifest, setupSourceCommit, installRoot, mode, transactionId,
                                            installedAtUtc, payloadManifestHash, setupExecutableHash, shortcuts)
            ValidateState(state)
            Return state
        End Function

        Private Shared Sub ValidateState(state As InstalledState)
            If state Is Nothing OrElse state.Manifest Is Nothing Then Throw New SetupContractException("Installed state is required.")
            If Not Regex.IsMatch(state.SetupSourceCommit, "^[0-9a-f]{40}$", RegexOptions.CultureInvariant) Then Throw New SetupContractException("Setup source commit is invalid.")
            SetupPathPolicy.ValidateInstallRoot(state.InstallRoot)
            If Array.IndexOf(New String() {"install", "repair", "upgrade"}, state.Mode) < 0 Then Throw New SetupContractException("Install mode is invalid.")
            If Not Regex.IsMatch(state.TransactionId, "^[0-9a-f]{32}$", RegexOptions.CultureInvariant) Then Throw New SetupContractException("Transaction ID is invalid.")
            If Not Regex.IsMatch(state.PayloadManifestSha256, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant) OrElse
                    Not Regex.IsMatch(state.SetupExecutableSha256, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant) Then Throw New SetupContractException("Installed-state hashes are invalid.")
            SetupShortcutService.ValidateOwnedShortcuts(state)
        End Sub

        Public Shared Function UninstallKeyForLane(lane As String) As String
            Select Case lane
                Case "win-x86-net40"
                    Return UninstallKeyPrefix & "x86"
                Case "win-x64-net48"
                    Return UninstallKeyPrefix & "x64"
                Case "win-arm64-net481"
                    Return UninstallKeyPrefix & "arm64"
                Case Else
                    Throw New SetupContractException("Installed state has no governed uninstall key for its lane.")
            End Select
        End Function

        Private Shared Function ElementChildren(parent As XmlElement) As IList(Of XmlElement)
            Dim result As New List(Of XmlElement)()
            For Each node As XmlNode In parent.ChildNodes
                If node.NodeType = XmlNodeType.Element Then
                    result.Add(DirectCast(node, XmlElement))
                ElseIf node.NodeType <> XmlNodeType.Whitespace AndAlso node.NodeType <> XmlNodeType.SignificantWhitespace Then
                    Throw New SetupContractException("Unexpected XML node below " & parent.Name & ".")
                End If
            Next
            Return result
        End Function

        Private Shared Sub RequireElement(element As XmlElement, expectedName As String)
            If element Is Nothing OrElse element.Name <> expectedName OrElse element.NamespaceURI <> String.Empty Then Throw New SetupContractException("Expected unqualified element " & expectedName & ".")
        End Sub

        Private Shared Sub RequireAttributes(element As XmlElement, names As String())
            If element.Attributes.Count <> names.Length Then Throw New SetupContractException("Element " & element.Name & " has missing or unexpected attributes.")
            For Each attribute As XmlAttribute In element.Attributes
                If attribute.NamespaceURI <> String.Empty OrElse Array.IndexOf(names, attribute.Name) < 0 Then Throw New SetupContractException("Unexpected attribute on " & element.Name & ".")
            Next
        End Sub

        Private Shared Sub RequireEmpty(element As XmlElement)
            If ElementChildren(element).Count <> 0 OrElse Not String.IsNullOrWhiteSpace(element.InnerText) Then Throw New SetupContractException(element.Name & " must be empty.")
        End Sub

        Private Shared Function RequiredValue(element As XmlElement, name As String) As String
            Dim value As String = element.GetAttribute(name)
            If String.IsNullOrWhiteSpace(value) OrElse value <> value.Trim() Then Throw New SetupContractException(element.Name & " attribute " & name & " is invalid.")
            Return value
        End Function

        Private Shared Function RequiredHash(element As XmlElement, name As String, length As Integer) As String
            Dim value As String = RequiredValue(element, name)
            If Not Regex.IsMatch(value, "^[0-9a-f]{" & length.ToString(CultureInfo.InvariantCulture) & "}$", RegexOptions.CultureInvariant) Then Throw New SetupContractException(element.Name & " attribute " & name & " is not canonical hexadecimal.")
            Return value
        End Function

    End Class

End Namespace
