Imports System.IO
Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace Global.C3Setup

    Public NotInheritable Class SetupTransactionRecovery

        Private Const PendingStateName As String = "C3.pending-state.xml"

        Private Sub New()
        End Sub

        Public Shared Function RecoverIncomplete(installRoot As String,
                                                 shortcutAccess As ISetupShortcutAccess,
                                                 registryAccess As ISetupRegistryAccess) As String
            If shortcutAccess Is Nothing Then Throw New ArgumentNullException("shortcutAccess")
            If registryAccess Is Nothing Then Throw New ArgumentNullException("registryAccess")
            Dim canonicalRoot As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            Dim journalPath As String = SetupTransactionJournalCodec.PathForInstallRoot(canonicalRoot)
            If Not File.Exists(journalPath) Then Return "none"
            Dim journal As SetupTransactionJournal = SetupTransactionJournalCodec.Read(journalPath)
            If Not String.Equals(journal.InstallRoot, canonicalRoot, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("The transaction journal does not own the selected install root.")
            End If
            If journal.Phase = SetupTransactionPhases.Complete OrElse journal.Phase = SetupTransactionPhases.RollbackComplete Then
                CleanupOwnedWorkRoots(journal)
                Return journal.Phase
            End If
            If journal.Phase <> SetupTransactionPhases.RollbackStarted Then
                PersistPhase(journal, journalPath, SetupTransactionPhases.RollbackStarted, Nothing)
            End If
            Rollback(journal, shortcutAccess, registryAccess)
            PersistPhase(journal, journalPath, SetupTransactionPhases.RollbackComplete, Nothing)
            CleanupOwnedWorkRoots(journal)
            Return SetupTransactionPhases.RollbackComplete
        End Function

        Friend Shared Sub Rollback(journal As SetupTransactionJournal,
                                   shortcutAccess As ISetupShortcutAccess,
                                   registryAccess As ISetupRegistryAccess)
            If journal.Operation = "install" Then
                RollbackInstall(journal, shortcutAccess, registryAccess)
            ElseIf journal.Operation = "uninstall" Then
                RollbackUninstall(journal, shortcutAccess, registryAccess)
            Else
                Throw New SetupContractException("Cannot recover an unknown transaction operation.")
            End If
        End Sub

        Friend Shared Sub PersistPhase(journal As SetupTransactionJournal,
                                       journalPath As String,
                                       phase As String,
                                       faultInjector As Action(Of String))
            journal.Advance(phase)
            SetupTransactionJournalCodec.Write(journalPath, journal)
            If faultInjector IsNot Nothing Then faultInjector("journal:" & phase)
        End Sub

        Friend Shared Sub CleanupOwnedWorkRoots(journal As SetupTransactionJournal)
            If Directory.Exists(journal.StagingRoot) Then Directory.Delete(journal.StagingRoot, True)
            If Directory.Exists(journal.BackupRoot) Then Directory.Delete(journal.BackupRoot, True)
        End Sub

        Private Shared Sub RollbackInstall(journal As SetupTransactionJournal,
                                           shortcutAccess As ISetupShortcutAccess,
                                           registryAccess As ISetupRegistryAccess)
            Dim intended As InstalledState = ReadMatchingState(Path.Combine(journal.StagingRoot, PendingStateName), journal, True)
            Dim liveStatePath As String = Path.Combine(journal.InstallRoot, InstalledStateCodec.FileName)
            Dim live As InstalledState = ReadMatchingState(liveStatePath, journal, False)
            If intended Is Nothing AndAlso live IsNot Nothing AndAlso live.TransactionId = journal.TransactionId Then intended = live
            Dim previous As InstalledState = ReadStateIfPresent(Path.Combine(journal.BackupRoot, InstalledStateCodec.FileName))
            If previous Is Nothing AndAlso live IsNot Nothing AndAlso live.TransactionId <> journal.TransactionId Then previous = live

            ReconcileRegistry(previous, intended, registryAccess)
            ReconcileShortcuts(previous, intended, shortcutAccess)

            If intended IsNot Nothing Then
                For Each item As PayloadFile In intended.Manifest.Files
                    Dim destination As String = SetupPathPolicy.CombineOwnedFile(journal.InstallRoot, item.Path)
                    Dim backup As String = SetupPathPolicy.CombineOwnedFile(journal.BackupRoot, item.Path)
                    If File.Exists(backup) Then
                        If File.Exists(destination) Then RequireOwnedFile(destination, item, "promoted payload")
                        If File.Exists(destination) Then File.Delete(destination)
                        EnsureParent(destination)
                        File.Move(backup, destination)
                    ElseIf previous Is Nothing AndAlso File.Exists(destination) Then
                        RequireOwnedFile(destination, item, "new payload")
                        File.Delete(destination)
                    ElseIf previous IsNot Nothing Then
                        Dim priorItem As PayloadFile = FindPayload(previous.Manifest, item.Path)
                        If priorItem IsNot Nothing AndAlso File.Exists(destination) Then RequireOwnedFile(destination, priorItem, "prior payload")
                    End If
                Next
            End If

            Dim backupState As String = Path.Combine(journal.BackupRoot, InstalledStateCodec.FileName)
            If File.Exists(backupState) Then
                If File.Exists(liveStatePath) Then
                    Dim current As InstalledState = InstalledStateCodec.Read(liveStatePath)
                    If current.TransactionId <> journal.TransactionId Then Throw New SetupContractException("Recovery refuses to replace an unrelated installed-state file.")
                    File.Delete(liveStatePath)
                End If
                EnsureParent(liveStatePath)
                File.Move(backupState, liveStatePath)
            ElseIf File.Exists(liveStatePath) Then
                Dim current As InstalledState = InstalledStateCodec.Read(liveStatePath)
                If current.TransactionId = journal.TransactionId Then
                    File.Delete(liveStatePath)
                ElseIf previous Is Nothing Then
                    Throw New SetupContractException("Recovery found an unauthenticated installed-state file.")
                End If
            End If
            DeleteIfExists(Path.Combine(journal.StagingRoot, PendingStateName))
            If Not journal.RootExisted AndAlso Directory.Exists(journal.InstallRoot) AndAlso
                    New DirectoryInfo(journal.InstallRoot).GetFileSystemInfos().Length = 0 Then Directory.Delete(journal.InstallRoot, False)
        End Sub

        Private Shared Sub RollbackUninstall(journal As SetupTransactionJournal,
                                             shortcutAccess As ISetupShortcutAccess,
                                             registryAccess As ISetupRegistryAccess)
            Dim recoveryStatePath As String = Path.Combine(journal.BackupRoot, InstalledStateCodec.FileName)
            Dim state As InstalledState = ReadMatchingState(recoveryStatePath, journal, True)
            If state Is Nothing Then
                state = ReadMatchingState(Path.Combine(journal.StagingRoot, InstalledStateCodec.FileName), journal, True)
            End If
            If state Is Nothing Then
                state = ReadMatchingState(Path.Combine(journal.InstallRoot, InstalledStateCodec.FileName), journal, True)
            End If
            If state Is Nothing Then Throw New SetupContractException("Uninstall recovery has no authenticated installed-state evidence.")

            ReconcileRegistry(state, Nothing, registryAccess)
            ReconcileShortcuts(state, Nothing, shortcutAccess)
            If Not Directory.Exists(journal.InstallRoot) Then Directory.CreateDirectory(journal.InstallRoot)
            For Each item As PayloadFile In state.Manifest.Files
                Dim staged As String = SetupPathPolicy.CombineOwnedFile(journal.StagingRoot, item.Path)
                Dim destination As String = SetupPathPolicy.CombineOwnedFile(journal.InstallRoot, item.Path)
                If File.Exists(staged) Then
                    RequireOwnedFile(staged, item, "quarantined payload")
                    If File.Exists(destination) Then RequireOwnedFile(destination, item, "live payload")
                    If Not File.Exists(destination) Then File.Move(staged, destination)
                ElseIf File.Exists(destination) Then
                    RequireOwnedFile(destination, item, "live payload")
                Else
                    Throw New SetupContractException("Uninstall recovery is missing an owned payload file: " & item.Path)
                End If
            Next
            Dim statePath As String = Path.Combine(journal.InstallRoot, InstalledStateCodec.FileName)
            If File.Exists(statePath) Then
                If FileHash.Sha256(statePath) <> journal.IntendedStateSha256 Then Throw New SetupContractException("Live installed-state evidence changed during uninstall recovery.")
            Else
                File.Copy(recoveryStatePath, statePath, False)
            End If
            PayloadVerifier.VerifyOwnedFiles(state.Manifest, journal.InstallRoot)
            InstalledStateCodec.Read(statePath)
        End Sub

        Private Shared Sub ReconcileRegistry(beforeState As InstalledState,
                                             afterState As InstalledState,
                                             access As ISetupRegistryAccess)
            Dim governing As InstalledState = If(afterState, beforeState)
            If governing Is Nothing Then Return
            Dim key As String = InstalledStateCodec.UninstallKeyForLane(governing.Manifest.Lane)
            Dim actual As IDictionary(Of String, Object) = access.ReadValues(key)
            Dim beforeValues As IDictionary(Of String, Object) = If(beforeState Is Nothing, Nothing, SetupRegistryRegistration.ExpectedValues(beforeState))
            Dim afterValues As IDictionary(Of String, Object) = If(afterState Is Nothing, Nothing, SetupRegistryRegistration.ExpectedValues(afterState))
            If EqualRegistry(actual, beforeValues) Then Return
            If Not EqualRegistry(actual, afterValues) Then Throw New SetupContractException("Recovery found registry state that is neither the authenticated before nor after image.")
            If beforeValues Is Nothing Then
                access.DeleteKey(key)
            Else
                access.WriteValues(key, beforeValues)
            End If
            If Not EqualRegistry(access.ReadValues(key), beforeValues) Then Throw New SetupContractException("Registry recovery did not restore the authenticated before image.")
        End Sub

        Private Shared Sub ReconcileShortcuts(beforeState As InstalledState,
                                              afterState As InstalledState,
                                              access As ISetupShortcutAccess)
            Dim beforeMap As IDictionary(Of String, SetupShortcut) = ExpectedShortcutMap(beforeState)
            Dim afterMap As IDictionary(Of String, SetupShortcut) = ExpectedShortcutMap(afterState)
            Dim paths As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            For Each path As String In beforeMap.Keys
                paths(path) = True
            Next
            For Each path As String In afterMap.Keys
                paths(path) = True
            Next
            For Each path As String In paths.Keys
                Dim actual As SetupShortcut = access.ReadShortcut(path)
                Dim beforeValue As SetupShortcut = If(beforeMap.ContainsKey(path), beforeMap(path), Nothing)
                Dim afterValue As SetupShortcut = If(afterMap.ContainsKey(path), afterMap(path), Nothing)
                If EqualShortcut(actual, beforeValue) Then Continue For
                If Not EqualShortcut(actual, afterValue) Then Throw New SetupContractException("Recovery found a shortcut that is neither the authenticated before nor after image: " & path)
                If beforeValue Is Nothing Then
                    access.DeleteShortcut(path)
                Else
                    access.WriteShortcut(beforeValue)
                End If
                If Not EqualShortcut(access.ReadShortcut(path), beforeValue) Then Throw New SetupContractException("Shortcut recovery did not restore the authenticated before image: " & path)
            Next
        End Sub

        Private Shared Function ExpectedShortcutMap(state As InstalledState) As IDictionary(Of String, SetupShortcut)
            Dim result As New Dictionary(Of String, SetupShortcut)(StringComparer.OrdinalIgnoreCase)
            If state Is Nothing Then Return result
            For Each item As InstalledShortcut In state.Shortcuts
                result.Add(item.Path, New SetupShortcut(item.Path, item.Target, state.InstallRoot, "Compact Cassette Catalogue"))
            Next
            Return result
        End Function

        Private Shared Function EqualShortcut(left As SetupShortcut, right As SetupShortcut) As Boolean
            If left Is Nothing OrElse right Is Nothing Then Return left Is Nothing AndAlso right Is Nothing
            Return String.Equals(left.Path, right.Path, StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(left.Target, right.Target, StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(left.WorkingDirectory, right.WorkingDirectory, StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(left.Description, right.Description, StringComparison.Ordinal)
        End Function

        Private Shared Function EqualRegistry(left As IDictionary(Of String, Object), right As IDictionary(Of String, Object)) As Boolean
            If left Is Nothing OrElse right Is Nothing Then Return left Is Nothing AndAlso right Is Nothing
            If left.Count <> right.Count Then Return False
            For Each pair As KeyValuePair(Of String, Object) In right
                If Not left.ContainsKey(pair.Key) OrElse left(pair.Key) Is Nothing OrElse
                        left(pair.Key).GetType() IsNot pair.Value.GetType() OrElse Not Object.Equals(left(pair.Key), pair.Value) Then Return False
            Next
            Return True
        End Function

        Private Shared Function ReadMatchingState(path As String,
                                                  journal As SetupTransactionJournal,
                                                  requireIdentity As Boolean) As InstalledState
            If Not File.Exists(path) Then Return Nothing
            Dim state As InstalledState = InstalledStateCodec.Read(path)
            Dim matches As Boolean = state.Manifest.Lane = journal.Lane AndAlso
                                     state.Manifest.Architecture = journal.Architecture AndAlso
                                     state.Manifest.Framework = journal.Framework AndAlso
                                     state.Manifest.SourceCommit = journal.PayloadSourceCommit AndAlso
                                     state.PayloadManifestSha256 = journal.PayloadManifestSha256 AndAlso
                                     state.SetupSourceCommit = journal.SetupSourceCommit AndAlso
                                     state.SetupExecutableSha256 = journal.SetupExecutableSha256
            If requireIdentity AndAlso Not matches Then Throw New SetupContractException("Installed-state evidence does not match the transaction identity.")
            Return state
        End Function

        Private Shared Function ReadStateIfPresent(path As String) As InstalledState
            If Not File.Exists(path) Then Return Nothing
            Return InstalledStateCodec.Read(path)
        End Function

        Private Shared Function FindPayload(manifest As PayloadManifest, relativePath As String) As PayloadFile
            For Each item As PayloadFile In manifest.Files
                If String.Equals(item.Path, relativePath, StringComparison.OrdinalIgnoreCase) Then Return item
            Next
            Return Nothing
        End Function

        Private Shared Sub RequireOwnedFile(path As String, expected As PayloadFile, context As String)
            Dim info As New FileInfo(path)
            If info.Length <> expected.Length OrElse FileHash.Sha256(path) <> expected.Sha256 Then
                Throw New SetupContractException("Recovery refuses altered " & context & ": " & expected.Path)
            End If
        End Sub

        Private Shared Sub EnsureParent(path As String)
            Dim parent As String = Directory.GetParent(path).FullName
            If Not Directory.Exists(parent) Then Directory.CreateDirectory(parent)
        End Sub

        Private Shared Sub DeleteIfExists(path As String)
            If File.Exists(path) Then File.Delete(path)
        End Sub
    End Class

    Public NotInheritable Class SetupDurableTransaction

        Private Const PendingStateName As String = "C3.pending-state.xml"

        Private Sub New()
        End Sub

        Public Shared Function Install(manifestPath As String,
                                       payloadDirectory As String,
                                       installRoot As String,
                                       setupSourceCommit As String,
                                       setupExecutableSha256 As String,
                                       shortcuts As IList(Of InstalledShortcut),
                                       shortcutAccess As ISetupShortcutAccess,
                                       registryAccess As ISetupRegistryAccess,
                                       faultInjector As Action(Of String)) As InstalledState
            Dim manifest As PayloadManifest = PayloadManifestReader.Read(manifestPath)
            PayloadVerifier.Verify(manifest, payloadDirectory)
            Dim root As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            SetupTransactionRecovery.RecoverIncomplete(root, shortcutAccess, registryAccess)
            Dim previous As InstalledState = ValidateInstallPreconditions(root, manifest)
            Dim mode As String = If(previous Is Nothing, "install", If(previous.Manifest.Label = manifest.Label, "repair", "upgrade"))
            Dim journal As SetupTransactionJournal = SetupTransactionJournal.CreateInstall(root, manifest, FileHash.Sha256(manifestPath), setupSourceCommit, setupExecutableSha256)
            Dim journalPath As String = SetupTransactionJournalCodec.PathForInstallRoot(root)
            SetupTransactionJournalCodec.Write(journalPath, journal)
            Inject(faultInjector, "journal:" & SetupTransactionPhases.Prepared)
            Dim state As New InstalledState(manifest, setupSourceCommit, root, mode, journal.TransactionId, DateTime.UtcNow,
                                            journal.PayloadManifestSha256, setupExecutableSha256, shortcuts)
            Try
                Directory.CreateDirectory(journal.StagingRoot)
                For Each item As PayloadFile In manifest.Files
                    File.Copy(SetupPathPolicy.CombineOwnedFile(payloadDirectory, item.Path),
                              SetupPathPolicy.CombineOwnedFile(journal.StagingRoot, item.Path), False)
                Next
                PayloadVerifier.Verify(manifest, journal.StagingRoot)
                Dim pendingState As String = Path.Combine(journal.StagingRoot, PendingStateName)
                InstalledStateCodec.Write(pendingState, state)
                journal.IntendedStateSha256 = FileHash.Sha256(pendingState)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.Staged, faultInjector)
                Inject(faultInjector, "after-staging")

                If Not Directory.Exists(root) Then Directory.CreateDirectory(root)
                Directory.CreateDirectory(journal.BackupRoot)
                BackupExisting(root, journal.BackupRoot, previous)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.BackupComplete, faultInjector)
                Inject(faultInjector, "after-backup")

                Dim installedCount As Integer = 0
                For Each item As PayloadFile In manifest.Files
                    File.Move(SetupPathPolicy.CombineOwnedFile(journal.StagingRoot, item.Path),
                              SetupPathPolicy.CombineOwnedFile(root, item.Path))
                    installedCount += 1
                    If installedCount = 1 Then Inject(faultInjector, "after-first-file")
                Next
                PayloadVerifier.VerifyOwnedFiles(manifest, root)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.PayloadPromoted, faultInjector)

                SetupShortcutService.Transition(previous, state, shortcutAccess)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.ShortcutsMutated, faultInjector)

                SetupRegistryRegistration.Apply(state, registryAccess)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.RegistryMutated, faultInjector)
                Inject(faultInjector, "after-system-integration")

                Dim statePath As String = Path.Combine(root, InstalledStateCodec.FileName)
                File.Move(pendingState, statePath)
                If FileHash.Sha256(statePath) <> journal.IntendedStateSha256 Then Throw New SetupContractException("Committed installed state changed after staging.")
                PayloadVerifier.VerifyOwnedFiles(manifest, root)
                InstalledStateCodec.Read(statePath)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.StateCommitted, faultInjector)
                Inject(faultInjector, "after-manifest")

                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.Complete, faultInjector)
                SetupTransactionRecovery.CleanupOwnedWorkRoots(journal)
                Return state
            Catch failure As Exception
                If journal.Phase = SetupTransactionPhases.Complete Then Throw
                Dim rollbackFailures As New List(Of Exception)()
                Try
                    SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.RollbackStarted, faultInjector)
                    SetupTransactionRecovery.Rollback(journal, shortcutAccess, registryAccess)
                    SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.RollbackComplete, faultInjector)
                    SetupTransactionRecovery.CleanupOwnedWorkRoots(journal)
                Catch rollbackFailure As Exception
                    rollbackFailures.Add(rollbackFailure)
                End Try
                If rollbackFailures.Count <> 0 Then
                    Dim allFailures As New List(Of Exception)()
                    allFailures.Add(failure)
                    allFailures.AddRange(rollbackFailures)
                    Throw New AggregateException("Setup failed and durable rollback also failed; the journal was retained.", allFailures)
                End If
                Throw
            End Try
        End Function

        Public Shared Sub Uninstall(installRoot As String,
                                    shortcutAccess As ISetupShortcutAccess,
                                    registryAccess As ISetupRegistryAccess,
                                    faultInjector As Action(Of String))
            Dim root As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            SetupTransactionRecovery.RecoverIncomplete(root, shortcutAccess, registryAccess)
            Dim statePath As String = Path.Combine(root, InstalledStateCodec.FileName)
            If Not File.Exists(statePath) Then Throw New SetupContractException("No C3 installed-state manifest exists at the selected root.")
            Dim state As InstalledState = InstalledStateCodec.Read(statePath)
            If Not String.Equals(state.InstallRoot, root, StringComparison.OrdinalIgnoreCase) Then Throw New SetupContractException("Installed-state root does not match the selected removal root.")
            PayloadVerifier.VerifyOwnedFiles(state.Manifest, root)
            SetupShortcutService.ValidateOwned(state, shortcutAccess)
            SetupRegistryRegistration.ValidateOwned(state, registryAccess)
            Dim journal As SetupTransactionJournal = SetupTransactionJournal.CreateUninstall(state)
            Dim journalPath As String = SetupTransactionJournalCodec.PathForInstallRoot(root)
            SetupTransactionJournalCodec.Write(journalPath, journal)
            Inject(faultInjector, "journal:" & SetupTransactionPhases.Prepared)
            Try
                Directory.CreateDirectory(journal.StagingRoot)
                Directory.CreateDirectory(journal.BackupRoot)
                File.Copy(statePath, Path.Combine(journal.BackupRoot, InstalledStateCodec.FileName), False)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.Staged, faultInjector)

                Dim count As Integer = 0
                For Each item As PayloadFile In state.Manifest.Files
                    File.Move(SetupPathPolicy.CombineOwnedFile(root, item.Path),
                              SetupPathPolicy.CombineOwnedFile(journal.StagingRoot, item.Path))
                    count += 1
                    If count = 1 Then Inject(faultInjector, "after-first-file")
                Next
                File.Move(statePath, Path.Combine(journal.StagingRoot, InstalledStateCodec.FileName))
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.BackupComplete, faultInjector)
                Inject(faultInjector, "after-state")
                PayloadVerifier.VerifyOwnedFiles(state.Manifest, journal.StagingRoot)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.PayloadPromoted, faultInjector)

                SetupShortcutService.RemoveTransition(state, shortcutAccess)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.ShortcutsMutated, faultInjector)
                SetupRegistryRegistration.RemoveWithSnapshot(state, registryAccess)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.RegistryMutated, faultInjector)
                Inject(faultInjector, "after-system-removal")

                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.StateCommitted, faultInjector)
                SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.Complete, faultInjector)
                SetupTransactionRecovery.CleanupOwnedWorkRoots(journal)
                If Directory.Exists(root) AndAlso New DirectoryInfo(root).GetFileSystemInfos().Length = 0 Then Directory.Delete(root, False)
            Catch failure As Exception
                If journal.Phase = SetupTransactionPhases.Complete Then Throw
                Dim rollbackFailures As New List(Of Exception)()
                Try
                    SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.RollbackStarted, faultInjector)
                    SetupTransactionRecovery.Rollback(journal, shortcutAccess, registryAccess)
                    SetupTransactionRecovery.PersistPhase(journal, journalPath, SetupTransactionPhases.RollbackComplete, faultInjector)
                    SetupTransactionRecovery.CleanupOwnedWorkRoots(journal)
                Catch rollbackFailure As Exception
                    rollbackFailures.Add(rollbackFailure)
                End Try
                If rollbackFailures.Count <> 0 Then
                    Dim allFailures As New List(Of Exception)()
                    allFailures.Add(failure)
                    allFailures.AddRange(rollbackFailures)
                    Throw New AggregateException("Uninstall failed and durable rollback also failed; the journal was retained.", allFailures)
                End If
                Throw
            End Try
        End Sub

        Private Shared Function ValidateInstallPreconditions(root As String, manifest As PayloadManifest) As InstalledState
            Dim statePath As String = Path.Combine(root, InstalledStateCodec.FileName)
            Dim previous As InstalledState = Nothing
            If Directory.Exists(root) AndAlso File.Exists(statePath) Then
                previous = InstalledStateCodec.Read(statePath)
                If Not String.Equals(previous.InstallRoot, root, StringComparison.OrdinalIgnoreCase) Then Throw New SetupContractException("Installed-state root does not match the selected install root.")
                If previous.Manifest.Lane <> manifest.Lane OrElse previous.Manifest.Architecture <> manifest.Architecture OrElse previous.Manifest.Framework <> manifest.Framework Then
                    Throw New SetupContractException("An installed lane cannot be changed in place.")
                End If
                If CompareReleaseIdentity(previous.Manifest, manifest) > 0 Then Throw New SetupContractException("Downgrading an installed C3 release is prohibited.")
            ElseIf Directory.Exists(root) AndAlso New DirectoryInfo(root).GetFileSystemInfos().Length <> 0 Then
                Throw New SetupContractException("A non-empty directory without valid C3 installed state cannot be adopted.")
            End If
            Dim owned As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            If previous IsNot Nothing Then
                For Each item As PayloadFile In previous.Manifest.Files
                    owned(item.Path) = True
                Next
            End If
            For Each item As PayloadFile In manifest.Files
                Dim destination As String = SetupPathPolicy.CombineOwnedFile(root, item.Path)
                If File.Exists(destination) AndAlso Not owned.ContainsKey(item.Path) Then Throw New SetupContractException("Setup refuses to overwrite an unowned file: " & item.Path)
            Next
            Return previous
        End Function

        Private Shared Function CompareReleaseIdentity(left As PayloadManifest, right As PayloadManifest) As Integer
            Dim leftVersion As Version = Nothing
            Dim rightVersion As Version = Nothing
            If Not Version.TryParse(left.Version, leftVersion) OrElse Not Version.TryParse(right.Version, rightVersion) Then Throw New SetupContractException("Release version is invalid.")
            Dim result As Integer = leftVersion.CompareTo(rightVersion)
            If result <> 0 Then Return result
            Return StageValue(left.Stage).CompareTo(StageValue(right.Stage))
        End Function

        Private Shared Function StageValue(stage As String) As Long
            Dim alpha As Match = Regex.Match(stage, "^Alpha (?<n>[1-9][0-9]*)$", RegexOptions.CultureInvariant)
            If alpha.Success Then Return Long.Parse(alpha.Groups("n").Value, CultureInfo.InvariantCulture)
            Dim beta As Match = Regex.Match(stage, "^Beta (?<n>[1-9][0-9]*)$", RegexOptions.CultureInvariant)
            If beta.Success Then Return 1000000L + Long.Parse(beta.Groups("n").Value, CultureInfo.InvariantCulture)
            If stage = "Release" Then Return 2000000L
            Throw New SetupContractException("Release stage is invalid.")
        End Function

        Private Shared Sub BackupExisting(root As String, backupRoot As String, previous As InstalledState)
            If previous Is Nothing Then Return
            Dim statePath As String = Path.Combine(root, InstalledStateCodec.FileName)
            File.Move(statePath, Path.Combine(backupRoot, InstalledStateCodec.FileName))
            For Each item As PayloadFile In previous.Manifest.Files
                Dim source As String = SetupPathPolicy.CombineOwnedFile(root, item.Path)
                If File.Exists(source) Then File.Move(source, SetupPathPolicy.CombineOwnedFile(backupRoot, item.Path))
            Next
        End Sub

        Private Shared Sub Inject(faultInjector As Action(Of String), point As String)
            If faultInjector IsNot Nothing Then faultInjector(point)
        End Sub
    End Class

End Namespace
