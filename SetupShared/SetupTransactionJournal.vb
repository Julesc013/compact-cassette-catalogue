Imports System.Globalization
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml

Namespace Global.C3Setup

    Public NotInheritable Class SetupTransactionPhases
        Public Const Prepared As String = "prepared"
        Public Const Staged As String = "staged"
        Public Const BackupComplete As String = "backup-complete"
        Public Const PayloadPromoted As String = "payload-promoted"
        Public Const ShortcutsMutated As String = "shortcuts-mutated"
        Public Const RegistryMutated As String = "registry-mutated"
        Public Const StateCommitted As String = "state-committed"
        Public Const Complete As String = "complete"
        Public Const RollbackStarted As String = "rollback-started"
        Public Const RollbackComplete As String = "rollback-complete"

        Private Shared ReadOnly ClosedSet As String() = {
            Prepared,
            Staged,
            BackupComplete,
            PayloadPromoted,
            ShortcutsMutated,
            RegistryMutated,
            StateCommitted,
            Complete,
            RollbackStarted,
            RollbackComplete
        }

        Private Sub New()
        End Sub

        Public Shared Function All() As String()
            Return DirectCast(ClosedSet.Clone(), String())
        End Function

        Public Shared Function Contains(value As String) As Boolean
            Return Array.IndexOf(ClosedSet, value) >= 0
        End Function
    End Class

    Public NotInheritable Class SetupTransactionJournal
        Private _phase As String
        Private _updatedAtUtc As DateTime

        Friend Sub New(operation As String,
                       phase As String,
                       transactionId As String,
                       installRoot As String,
                       stagingRoot As String,
                       backupRoot As String,
                       rootExisted As Boolean,
                       lane As String,
                       architecture As String,
                       framework As String,
                       payloadSourceCommit As String,
                       payloadManifestSha256 As String,
                       setupSourceCommit As String,
                       setupExecutableSha256 As String,
                       intendedStateSha256 As String,
                       createdAtUtc As DateTime,
                       updatedAtUtc As DateTime)
            Me.Operation = operation
            _phase = phase
            Me.TransactionId = transactionId
            Me.InstallRoot = installRoot
            Me.StagingRoot = stagingRoot
            Me.BackupRoot = backupRoot
            Me.RootExisted = rootExisted
            Me.Lane = lane
            Me.Architecture = architecture
            Me.Framework = framework
            Me.PayloadSourceCommit = payloadSourceCommit
            Me.PayloadManifestSha256 = payloadManifestSha256
            Me.SetupSourceCommit = setupSourceCommit
            Me.SetupExecutableSha256 = setupExecutableSha256
            Me.IntendedStateSha256 = intendedStateSha256
            Me.CreatedAtUtc = createdAtUtc
            _updatedAtUtc = updatedAtUtc
            Validate()
        End Sub

        Public Shared Function CreateInstall(installRoot As String,
                                             manifest As PayloadManifest,
                                             payloadManifestSha256 As String,
                                             setupSourceCommit As String,
                                             setupExecutableSha256 As String) As SetupTransactionJournal
            If manifest Is Nothing Then Throw New ArgumentNullException("manifest")
            Dim root As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            Dim transactionId As String = Guid.NewGuid().ToString("N").ToLowerInvariant()
            Dim parent As String = Directory.GetParent(root).FullName
            Dim leaf As String = New DirectoryInfo(root).Name
            Dim now As DateTime = DateTime.UtcNow
            Return New SetupTransactionJournal("install",
                                               SetupTransactionPhases.Prepared,
                                               transactionId,
                                               root,
                                               Path.Combine(parent, "." & leaf & ".c3stage-" & transactionId),
                                               Path.Combine(parent, "." & leaf & ".c3backup-" & transactionId),
                                               Directory.Exists(root),
                                               manifest.Lane,
                                               manifest.Architecture,
                                               manifest.Framework,
                                               manifest.SourceCommit,
                                               payloadManifestSha256,
                                               setupSourceCommit,
                                               setupExecutableSha256,
                                               String.Empty,
                                               now,
                                               now)
        End Function

        Public Shared Function CreateUninstall(state As InstalledState) As SetupTransactionJournal
            If state Is Nothing Then Throw New ArgumentNullException("state")
            Dim root As String = SetupPathPolicy.ValidateInstallRoot(state.InstallRoot)
            Dim transactionId As String = Guid.NewGuid().ToString("N").ToLowerInvariant()
            Dim parent As String = Directory.GetParent(root).FullName
            Dim leaf As String = New DirectoryInfo(root).Name
            Dim now As DateTime = DateTime.UtcNow
            Return New SetupTransactionJournal("uninstall",
                                               SetupTransactionPhases.Prepared,
                                               transactionId,
                                               root,
                                               Path.Combine(parent, "." & leaf & ".c3remove-" & transactionId),
                                               Path.Combine(parent, "." & leaf & ".c3restore-" & transactionId),
                                               True,
                                               state.Manifest.Lane,
                                               state.Manifest.Architecture,
                                               state.Manifest.Framework,
                                               state.Manifest.SourceCommit,
                                               state.PayloadManifestSha256,
                                               state.SetupSourceCommit,
                                               state.SetupExecutableSha256,
                                               FileHash.Sha256(Path.Combine(root, InstalledStateCodec.FileName)),
                                               now,
                                               now)
        End Function

        Public ReadOnly Property Operation As String
        Public ReadOnly Property TransactionId As String
        Public ReadOnly Property InstallRoot As String
        Public ReadOnly Property StagingRoot As String
        Public ReadOnly Property BackupRoot As String
        Public ReadOnly Property RootExisted As Boolean
        Public ReadOnly Property Lane As String
        Public ReadOnly Property Architecture As String
        Public ReadOnly Property Framework As String
        Public ReadOnly Property PayloadSourceCommit As String
        Public ReadOnly Property PayloadManifestSha256 As String
        Public ReadOnly Property SetupSourceCommit As String
        Public ReadOnly Property SetupExecutableSha256 As String
        Public Property IntendedStateSha256 As String
        Public ReadOnly Property CreatedAtUtc As DateTime

        Public ReadOnly Property Phase As String
            Get
                Return _phase
            End Get
        End Property

        Public ReadOnly Property UpdatedAtUtc As DateTime
            Get
                Return _updatedAtUtc
            End Get
        End Property

        Public ReadOnly Property IdentitySha256 As String
            Get
                Return HashText(IdentityProjection())
            End Get
        End Property

        Public Sub Advance(phase As String)
            If Not SetupTransactionPhases.Contains(phase) Then Throw New SetupContractException("Transaction journal phase is invalid.")
            _phase = phase
            _updatedAtUtc = DateTime.UtcNow
            Validate()
        End Sub

        Friend Function RecordSha256() As String
            Return HashText(IdentityProjection() & "|" & _phase & "|" &
                            _updatedAtUtc.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture) & "|" & IntendedStateSha256)
        End Function

        Friend Sub Validate()
            If Operation <> "install" AndAlso Operation <> "uninstall" Then Throw New SetupContractException("Transaction journal operation is invalid.")
            If Not SetupTransactionPhases.Contains(_phase) Then Throw New SetupContractException("Transaction journal phase is invalid.")
            If Not Regex.IsMatch(TransactionId, "^[0-9a-f]{32}$", RegexOptions.CultureInvariant) Then Throw New SetupContractException("Transaction journal ID is invalid.")
            Dim root As String = SetupPathPolicy.ValidateInstallRoot(InstallRoot)
            If Not String.Equals(root, InstallRoot, StringComparison.OrdinalIgnoreCase) Then Throw New SetupContractException("Transaction journal install root is noncanonical.")
            Dim parent As String = Directory.GetParent(root).FullName
            If Not String.Equals(Directory.GetParent(StagingRoot).FullName, parent, StringComparison.OrdinalIgnoreCase) OrElse
                    Not String.Equals(Directory.GetParent(BackupRoot).FullName, parent, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("Transaction journal work roots do not share the governed parent.")
            End If
            Dim leaf As String = New DirectoryInfo(root).Name
            Dim expectedStaging As String = If(Operation = "install",
                                               "." & leaf & ".c3stage-" & TransactionId,
                                               "." & leaf & ".c3remove-" & TransactionId)
            Dim expectedBackup As String = If(Operation = "install",
                                              "." & leaf & ".c3backup-" & TransactionId,
                                              "." & leaf & ".c3restore-" & TransactionId)
            If New DirectoryInfo(StagingRoot).Name <> expectedStaging OrElse New DirectoryInfo(BackupRoot).Name <> expectedBackup Then
                Throw New SetupContractException("Transaction journal work roots are not transaction-owned.")
            End If
            If String.IsNullOrWhiteSpace(Lane) OrElse String.IsNullOrWhiteSpace(Architecture) OrElse String.IsNullOrWhiteSpace(Framework) Then
                Throw New SetupContractException("Transaction journal lane identity is incomplete.")
            End If
            RequireHash(PayloadSourceCommit, 40, "payload source commit")
            RequireHash(PayloadManifestSha256, 64, "payload manifest")
            RequireHash(SetupSourceCommit, 40, "setup source commit")
            RequireHash(SetupExecutableSha256, 64, "setup executable")
            If IntendedStateSha256.Length <> 0 Then RequireHash(IntendedStateSha256, 64, "intended installed state")
            If CreatedAtUtc.Kind <> DateTimeKind.Utc OrElse _updatedAtUtc.Kind <> DateTimeKind.Utc OrElse _updatedAtUtc < CreatedAtUtc Then
                Throw New SetupContractException("Transaction journal timestamps are invalid.")
            End If
        End Sub

        Private Function IdentityProjection() As String
            Return String.Join("|", New String() {
                "C3SetupTransaction/1",
                Operation,
                TransactionId,
                InstallRoot,
                StagingRoot,
                BackupRoot,
                If(RootExisted, "true", "false"),
                Lane,
                Architecture,
                Framework,
                PayloadSourceCommit,
                PayloadManifestSha256,
                SetupSourceCommit,
                SetupExecutableSha256,
                CreatedAtUtc.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture)
            })
        End Function

        Private Shared Sub RequireHash(value As String, length As Integer, name As String)
            If value Is Nothing OrElse Not Regex.IsMatch(value, "^[0-9a-f]{" & length.ToString(CultureInfo.InvariantCulture) & "}$", RegexOptions.CultureInvariant) Then
                Throw New SetupContractException("Transaction journal " & name & " hash is invalid.")
            End If
        End Sub

        Private Shared Function HashText(value As String) As String
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(value)
            Using algorithm As New SHA256CryptoServiceProvider()
                Return BitConverter.ToString(algorithm.ComputeHash(bytes)).Replace("-", String.Empty).ToLowerInvariant()
            End Using
        End Function
    End Class

    Public NotInheritable Class SetupTransactionJournalCodec
        Public Const FileNameSuffix As String = ".c3-transaction.xml"

        Private Sub New()
        End Sub

        Public Shared Function PathForInstallRoot(installRoot As String) As String
            Dim root As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            Return Path.Combine(Directory.GetParent(root).FullName, "." & New DirectoryInfo(root).Name & FileNameSuffix)
        End Function

        Public Shared Sub Write(path As String, journal As SetupTransactionJournal)
            If journal Is Nothing Then Throw New ArgumentNullException("journal")
            journal.Validate()
            If Not String.Equals(IO.Path.GetFullPath(path), PathForInstallRoot(journal.InstallRoot), StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("Transaction journal path is outside its governed location.")
            End If
            Dim settings As New XmlWriterSettings()
            settings.Encoding = New UTF8Encoding(False)
            settings.Indent = True
            settings.NewLineChars = Environment.NewLine
            settings.NewLineHandling = NewLineHandling.Replace
            settings.OmitXmlDeclaration = False
            Dim bytes As Byte()
            Using memory As New MemoryStream()
                Using writer As XmlWriter = XmlWriter.Create(memory, settings)
                    writer.WriteStartDocument()
                    writer.WriteStartElement("C3SetupTransaction")
                    writer.WriteAttributeString("schemaVersion", "1")
                    writer.WriteAttributeString("operation", journal.Operation)
                    writer.WriteAttributeString("phase", journal.Phase)
                    writer.WriteAttributeString("transactionId", journal.TransactionId)
                    writer.WriteAttributeString("installRoot", journal.InstallRoot)
                    writer.WriteAttributeString("stagingRoot", journal.StagingRoot)
                    writer.WriteAttributeString("backupRoot", journal.BackupRoot)
                    writer.WriteAttributeString("rootExisted", If(journal.RootExisted, "true", "false"))
                    writer.WriteAttributeString("lane", journal.Lane)
                    writer.WriteAttributeString("architecture", journal.Architecture)
                    writer.WriteAttributeString("framework", journal.Framework)
                    writer.WriteAttributeString("payloadSourceCommit", journal.PayloadSourceCommit)
                    writer.WriteAttributeString("payloadManifestSha256", journal.PayloadManifestSha256)
                    writer.WriteAttributeString("setupSourceCommit", journal.SetupSourceCommit)
                    writer.WriteAttributeString("setupExecutableSha256", journal.SetupExecutableSha256)
                    writer.WriteAttributeString("intendedStateSha256", journal.IntendedStateSha256)
                    writer.WriteAttributeString("createdAtUtc", journal.CreatedAtUtc.ToString("o", CultureInfo.InvariantCulture))
                    writer.WriteAttributeString("updatedAtUtc", journal.UpdatedAtUtc.ToString("o", CultureInfo.InvariantCulture))
                    writer.WriteAttributeString("identitySha256", journal.IdentitySha256)
                    writer.WriteAttributeString("recordSha256", journal.RecordSha256())
                    writer.WriteEndElement()
                    writer.WriteEndDocument()
                End Using
                bytes = memory.ToArray()
            End Using
            DurableReplace(path, bytes)
        End Sub

        Public Shared Function Read(path As String) As SetupTransactionJournal
            Dim document As New XmlDocument()
            document.XmlResolver = Nothing
            Dim settings As New XmlReaderSettings()
            settings.DtdProcessing = DtdProcessing.Prohibit
            settings.XmlResolver = Nothing
            settings.IgnoreComments = False
            settings.IgnoreProcessingInstructions = False
            Try
                Using reader As XmlReader = XmlReader.Create(path, settings)
                    document.Load(reader)
                End Using
            Catch ex As Exception
                If TypeOf ex Is SetupContractException Then Throw
                Throw New SetupContractException("The transaction journal is not safe, well-formed XML.", ex)
            End Try
            Dim root As XmlElement = document.DocumentElement
            If root Is Nothing OrElse root.Name <> "C3SetupTransaction" OrElse root.NamespaceURI <> String.Empty OrElse
                    root.ChildNodes.Count <> 0 Then Throw New SetupContractException("Transaction journal root is invalid.")
            Dim names As String() = {
                "schemaVersion", "operation", "phase", "transactionId", "installRoot", "stagingRoot", "backupRoot", "rootExisted",
                "lane", "architecture", "framework", "payloadSourceCommit", "payloadManifestSha256", "setupSourceCommit",
                "setupExecutableSha256", "intendedStateSha256", "createdAtUtc", "updatedAtUtc", "identitySha256", "recordSha256"
            }
            If root.Attributes.Count <> names.Length Then Throw New SetupContractException("Transaction journal attribute set is not closed.")
            For Each attribute As XmlAttribute In root.Attributes
                If attribute.NamespaceURI <> String.Empty OrElse Array.IndexOf(names, attribute.Name) < 0 Then
                    Throw New SetupContractException("Transaction journal contains an unexpected attribute.")
                End If
            Next
            If root.GetAttribute("schemaVersion") <> "1" Then Throw New SetupContractException("Transaction journal schema is unsupported.")
            Dim rootExisted As Boolean
            If root.GetAttribute("rootExisted") = "true" Then
                rootExisted = True
            ElseIf root.GetAttribute("rootExisted") <> "false" Then
                Throw New SetupContractException("Transaction journal root-existence marker is invalid.")
            End If
            Dim created As DateTime
            Dim updated As DateTime
            If Not DateTime.TryParseExact(root.GetAttribute("createdAtUtc"), "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, created) OrElse
                    Not DateTime.TryParseExact(root.GetAttribute("updatedAtUtc"), "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, updated) Then
                Throw New SetupContractException("Transaction journal timestamp is invalid.")
            End If
            created = created.ToUniversalTime()
            updated = updated.ToUniversalTime()
            Dim journal As New SetupTransactionJournal(root.GetAttribute("operation"),
                                                       root.GetAttribute("phase"),
                                                       root.GetAttribute("transactionId"),
                                                       root.GetAttribute("installRoot"),
                                                       root.GetAttribute("stagingRoot"),
                                                       root.GetAttribute("backupRoot"),
                                                       rootExisted,
                                                       root.GetAttribute("lane"),
                                                       root.GetAttribute("architecture"),
                                                       root.GetAttribute("framework"),
                                                       root.GetAttribute("payloadSourceCommit"),
                                                       root.GetAttribute("payloadManifestSha256"),
                                                       root.GetAttribute("setupSourceCommit"),
                                                       root.GetAttribute("setupExecutableSha256"),
                                                       root.GetAttribute("intendedStateSha256"),
                                                       created,
                                                       updated)
            If root.GetAttribute("identitySha256") <> journal.IdentitySha256 OrElse root.GetAttribute("recordSha256") <> journal.RecordSha256() Then
                Throw New SetupContractException("Transaction journal identity or record authentication failed.")
            End If
            If Not String.Equals(IO.Path.GetFullPath(path), PathForInstallRoot(journal.InstallRoot), StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("Transaction journal was read from an unowned location.")
            End If
            Return journal
        End Function

        Private Shared Sub DurableReplace(path As String, bytes As Byte())
            Dim parent As String = Directory.GetParent(path).FullName
            If Not Directory.Exists(parent) Then Directory.CreateDirectory(parent)
            Dim temporary As String = path & ".new-" & Guid.NewGuid().ToString("N")
            Try
                Using stream As New FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough)
                    stream.Write(bytes, 0, bytes.Length)
                    stream.Flush(True)
                End Using
                If File.Exists(path) Then
                    File.Replace(temporary, path, Nothing, True)
                Else
                    File.Move(temporary, path)
                End If
            Finally
                If File.Exists(temporary) Then File.Delete(temporary)
            End Try
        End Sub
    End Class

End Namespace
