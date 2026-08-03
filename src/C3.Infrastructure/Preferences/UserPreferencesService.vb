Imports System.Security

Namespace Preferences

    ''' <summary>
    ''' Owns C3's in-memory preferences and coordinates the stable native store
    ''' with the bounded, read-only C3 1.x importer.
    ''' </summary>
    Public NotInheritable Class UserPreferencesService

        Private Const LegacyMyDocumentsExpression As String =
            "My.Computer.FileSystem.SpecialDirectories.MyDocuments"
        Private ReadOnly _store As XmlUserPreferencesStore
        Private ReadOnly _legacyImporter As LegacyUserSettingsImporter
        Private ReadOnly _localApplicationDataDirectory As String
        Private ReadOnly _myDocumentsDirectory As String
        Private ReadOnly _warningSink As Action(Of String)
        Private ReadOnly _gate As New Object()
        Private _current As UserPreferencesSnapshot
        Private _dirtyFields As UserPreferenceFields
        Private _isInitialized As Boolean

        Public Sub New(
                store As XmlUserPreferencesStore,
                legacyImporter As LegacyUserSettingsImporter,
                localApplicationDataDirectory As String,
                myDocumentsDirectory As String,
                Optional warningSink As Action(Of String) = Nothing)

            If store Is Nothing Then
                Throw New ArgumentNullException("store")
            End If
            If legacyImporter Is Nothing Then
                Throw New ArgumentNullException("legacyImporter")
            End If
            If String.IsNullOrWhiteSpace(localApplicationDataDirectory) Then
                Throw New ArgumentException(
                    "A LocalApplicationData directory is required.",
                    "localApplicationDataDirectory")
            End If

            _store = store
            _legacyImporter = legacyImporter
            _localApplicationDataDirectory = Path.GetFullPath(localApplicationDataDirectory)
            _myDocumentsDirectory = If(myDocumentsDirectory, String.Empty)
            _warningSink = warningSink
            _current = UserPreferencesSnapshot.CreateDefaults(_myDocumentsDirectory)
        End Sub

        Public ReadOnly Property PreferencesPath As String
            Get
                Return _store.PreferencesPath
            End Get
        End Property

        Public ReadOnly Property IsInitialized As Boolean
            Get
                SyncLock _gate
                    Return _isInitialized
                End SyncLock
            End Get
        End Property

        Public ReadOnly Property HasUnsavedChanges As Boolean
            Get
                SyncLock _gate
                    Return _dirtyFields <> UserPreferenceFields.None
                End SyncLock
            End Get
        End Property

        Public Property ShowMessages As Boolean
            Get
                SyncLock _gate
                    Return _current.ShowMessages
                End SyncLock
            End Get
            Set(value As Boolean)
                SyncLock _gate
                    If _current.ShowMessages <> value Then
                        _current.ShowMessages = value
                        _dirtyFields = _dirtyFields Or UserPreferenceFields.ShowMessages
                    End If
                End SyncLock
            End Set
        End Property

        Public Property DefaultDirectory As String
            Get
                SyncLock _gate
                    Return _current.DefaultDirectory
                End SyncLock
            End Get
            Set(value As String)
                Dim normalized As String = If(value, String.Empty)
                SyncLock _gate
                    If Not String.Equals(
                            _current.DefaultDirectory,
                            normalized,
                            StringComparison.Ordinal) Then
                        _current.DefaultDirectory = normalized
                        _dirtyFields = _dirtyFields Or UserPreferenceFields.DefaultDirectory
                    End If
                End SyncLock
            End Set
        End Property

        Public Property UpdatePolicy As Updates.UpdateCheckPolicy
            Get
                SyncLock _gate
                    Return _current.UpdatePolicy
                End SyncLock
            End Get
            Set(value As Updates.UpdateCheckPolicy)
                If Not [Enum].IsDefined(GetType(Updates.UpdateCheckPolicy), value) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                SyncLock _gate
                    If _current.UpdatePolicy <> value Then
                        _current.UpdatePolicy = value
                        _dirtyFields = _dirtyFields Or UserPreferenceFields.UpdatePolicy
                    End If
                End SyncLock
            End Set
        End Property

        Public Property LastUpdateCheck As DateTime
            Get
                SyncLock _gate
                    Return _current.LastUpdateCheck
                End SyncLock
            End Get
            Set(value As DateTime)
                SyncLock _gate
                    If Not _current.LastUpdateCheck.Equals(value) Then
                        _current.LastUpdateCheck = value
                        _dirtyFields = _dirtyFields Or UserPreferenceFields.LastUpdateCheck
                    End If
                End SyncLock
            End Set
        End Property

        Public Function Initialize() As UserPreferencesLoadResult
            SyncLock _gate
                If _isInitialized Then
                    Return UserPreferencesLoadResult.Loaded(_current.Clone())
                End If

                Dim pending As UserPreferencesSnapshot = _current.Clone()
                Dim pendingFields As UserPreferenceFields = _dirtyFields
                Dim result As UserPreferencesLoadResult = InitializeCore()
                If result.Preferences IsNot Nothing Then
                    _current = result.Preferences.Clone()
                    ApplyFields(_current, pending, pendingFields)
                    If result.IsSuccess Then
                        result = UserPreferencesLoadResult.Loaded(
                            _current.Clone(),
                            result.RecoveryPath,
                            result.MigrationOutcome,
                            result.Message)
                    Else
                        result = UserPreferencesLoadResult.Failed(
                            result.Failure,
                            result.Message,
                            result.RecoveryPath,
                            _current.Clone(),
                            result.MigrationOutcome)
                    End If
                End If
                _dirtyFields = pendingFields
                _isInitialized = result.IsSuccess
                Return result
            End SyncLock
        End Function

        ''' <summary>
        ''' Compatibility entry point for existing forms. Failures are retained
        ''' in memory for retry and sent to the injected diagnostics sink.
        ''' </summary>
        Public Sub Save()
            Dim result As UserPreferencesSaveResult = TrySave()
            If Not result.IsSuccess AndAlso _warningSink IsNot Nothing Then
                _warningSink("Preferences could not be saved: " & result.Message)
            End If
        End Sub

        Public Function TrySave() As UserPreferencesSaveResult
            SyncLock _gate
                If Not _isInitialized Then
                    Dim initialization As UserPreferencesLoadResult = Initialize()
                    If Not initialization.IsSuccess Then
                        Return UserPreferencesSaveResult.Failed(
                            initialization.Failure,
                            initialization.Message)
                    End If
                End If

                If _dirtyFields = UserPreferenceFields.None Then
                    Return UserPreferencesSaveResult.Saved(_current.Clone(), Nothing)
                End If

                Dim saveResult As UserPreferencesSaveResult = _store.Save(
                    _current.Clone(),
                    _dirtyFields)
                If saveResult.IsSuccess Then
                    _current = saveResult.Preferences.Clone()
                    _dirtyFields = UserPreferenceFields.None
                End If
                Return saveResult
            End SyncLock
        End Function

        Public Function Snapshot() As UserPreferencesSnapshot
            SyncLock _gate
                Return _current.Clone()
            End SyncLock
        End Function

        Private Function InitializeCore() As UserPreferencesLoadResult
            Dim fallback As UserPreferencesSnapshot =
                UserPreferencesSnapshot.CreateDefaults(_myDocumentsDirectory)

            Try
                Using lockHandle As IDisposable = _store.AcquireExclusiveLock()
                    Dim nativeResult As UserPreferencesLoadResult = _store.LoadPrimaryUnlocked()
                    Dim snapshot As UserPreferencesSnapshot = Nothing
                    Dim recoveryPath As String = Nothing
                    Dim message As String = String.Empty
                    Dim mustPersist As Boolean = False

                    If nativeResult.IsSuccess Then
                        snapshot = nativeResult.Preferences.Clone()
                    ElseIf nativeResult.IsMissing Then
                        Dim backupResult As UserPreferencesLoadResult = _store.LoadBackupUnlocked()
                        If backupResult.IsSuccess Then
                            snapshot = backupResult.Preferences.Clone()
                            mustPersist = True
                            message = "Recovered preferences from the last known-good backup."
                        ElseIf IsUnsafeToReplace(backupResult) Then
                            Return UserPreferencesLoadResult.Failed(
                                backupResult.Failure,
                                "The preferences backup could not be read safely: " &
                                    backupResult.Message,
                                Nothing,
                                fallback)
                        Else
                            snapshot = fallback.Clone()
                            mustPersist = True
                        End If
                    ElseIf CanQuarantine(nativeResult) Then
                        Dim backupResult As UserPreferencesLoadResult = _store.LoadBackupUnlocked()
                        If IsUnsafeToReplace(backupResult) Then
                            Return UserPreferencesLoadResult.Failed(
                                backupResult.Failure,
                                "The native preferences and its backup could not be read safely: " &
                                    backupResult.Message,
                                Nothing,
                                fallback)
                        End If

                        recoveryPath = _store.QuarantinePrimaryUnlocked()
                        If backupResult.IsSuccess Then
                            snapshot = backupResult.Preferences.Clone()
                            message = "Quarantined invalid preferences and recovered the backup."
                        Else
                            snapshot = fallback.Clone()
                            message = "Quarantined invalid preferences and restored safe defaults."
                        End If
                        mustPersist = True
                    Else
                        Return UserPreferencesLoadResult.Failed(
                            nativeResult.Failure,
                            nativeResult.Message,
                            Nothing,
                            fallback)
                    End If

                    If Normalize(snapshot) Then
                        mustPersist = True
                    End If

                    Dim migrationOutcome As String = Nothing
                    If snapshot.Legacy1xImportVersion <
                            UserPreferencesSnapshot.CurrentLegacyImportVersion Then
                        Dim importResult As LegacyUserSettingsImportResult =
                            _legacyImporter.Import(_localApplicationDataDirectory)
                        ApplyLegacyImport(snapshot, importResult)
                        migrationOutcome = snapshot.Legacy1xImportOutcome
                        mustPersist = True

                        If importResult.Status = LegacyUserSettingsImportStatus.Imported Then
                            message = AppendMessage(
                                message,
                                "Imported C3 1.x preferences from " &
                                    importResult.Profile.SourcePath & ".")
                        ElseIf importResult.Status = LegacyUserSettingsImportStatus.NotFound Then
                            message = AppendMessage(
                                message,
                                "No supported C3 1.x preferences profile was found.")
                        Else
                            message = AppendMessage(
                                message,
                                "C3 1.x preference profiles were found but none was valid.")
                        End If
                    End If

                    If mustPersist Then
                        Dim saveResult As UserPreferencesSaveResult =
                            _store.SaveExactUnlocked(snapshot)
                        If Not saveResult.IsSuccess Then
                            Return UserPreferencesLoadResult.Failed(
                                saveResult.Failure,
                                AppendMessage(
                                    message,
                                    "The native preferences checkpoint failed: " &
                                        saveResult.Message),
                                recoveryPath,
                                snapshot,
                                migrationOutcome)
                        End If
                        snapshot = saveResult.Preferences.Clone()
                    End If

                    Return UserPreferencesLoadResult.Loaded(
                        snapshot,
                        recoveryPath,
                        migrationOutcome,
                        message)
                End Using
            Catch ex As TimeoutException
                Return UserPreferencesLoadResult.Failed(
                    UserPreferencesFailure.Busy,
                    ex.Message,
                    Nothing,
                    fallback)
            Catch ex As UnauthorizedAccessException
                Return UserPreferencesLoadResult.Failed(
                    UserPreferencesFailure.AccessDenied,
                    ex.Message,
                    Nothing,
                    fallback)
            Catch ex As SecurityException
                Return UserPreferencesLoadResult.Failed(
                    UserPreferencesFailure.AccessDenied,
                    ex.Message,
                    Nothing,
                    fallback)
            Catch ex As IOException
                Return UserPreferencesLoadResult.Failed(
                    UserPreferencesFailure.IoFailure,
                    ex.Message,
                    Nothing,
                    fallback)
            End Try
        End Function

        Private Sub ApplyLegacyImport(
                snapshot As UserPreferencesSnapshot,
                importResult As LegacyUserSettingsImportResult)

            If importResult.Status = LegacyUserSettingsImportStatus.Imported Then
                Dim profile As LegacyUserSettingsProfile = importResult.Profile
                If profile.HasShowMessages Then
                    snapshot.ShowMessages = profile.ShowMessages
                End If
                If profile.HasDefaultDirectory Then
                    snapshot.DefaultDirectory = profile.DefaultDirectory
                End If
                If profile.HasUpdatePolicy Then
                    snapshot.UpdatePolicy = profile.UpdatePolicy
                End If
                If profile.HasLastUpdateCheck Then
                    snapshot.LastUpdateCheck = profile.LastUpdateCheck
                End If
                snapshot.Legacy1xImportOutcome =
                    UserPreferencesSnapshot.ImportOutcomeImported
            ElseIf importResult.Status = LegacyUserSettingsImportStatus.NotFound Then
                snapshot.Legacy1xImportOutcome =
                    UserPreferencesSnapshot.ImportOutcomeNotFound
            Else
                snapshot.Legacy1xImportOutcome =
                    UserPreferencesSnapshot.ImportOutcomeInvalid
            End If

            snapshot.Legacy1xImportVersion =
                UserPreferencesSnapshot.CurrentLegacyImportVersion
            Normalize(snapshot)
        End Sub

        Private Function Normalize(snapshot As UserPreferencesSnapshot) As Boolean
            Dim requiresDefault As Boolean =
                String.IsNullOrWhiteSpace(snapshot.DefaultDirectory) OrElse
                String.Equals(
                    snapshot.DefaultDirectory,
                    LegacyMyDocumentsExpression,
                    StringComparison.Ordinal)
            If requiresDefault AndAlso
                    Not String.Equals(
                        snapshot.DefaultDirectory,
                        _myDocumentsDirectory,
                        StringComparison.Ordinal) Then
                snapshot.DefaultDirectory = _myDocumentsDirectory
                Return True
            End If
            Return False
        End Function

        Private Shared Sub ApplyFields(
                target As UserPreferencesSnapshot,
                source As UserPreferencesSnapshot,
                fields As UserPreferenceFields)

            If (fields And UserPreferenceFields.ShowMessages) <> 0 Then
                target.ShowMessages = source.ShowMessages
            End If
            If (fields And UserPreferenceFields.DefaultDirectory) <> 0 Then
                target.DefaultDirectory = source.DefaultDirectory
            End If
            If (fields And UserPreferenceFields.UpdatePolicy) <> 0 Then
                target.UpdatePolicy = source.UpdatePolicy
            End If
            If (fields And UserPreferenceFields.LastUpdateCheck) <> 0 Then
                target.LastUpdateCheck = source.LastUpdateCheck
            End If
        End Sub

        Private Shared Function CanQuarantine(result As UserPreferencesLoadResult) As Boolean
            Return result.Failure = UserPreferencesFailure.Invalid OrElse
                result.Failure = UserPreferencesFailure.TooLarge
        End Function

        Private Shared Function IsUnsafeToReplace(result As UserPreferencesLoadResult) As Boolean
            Return Not result.IsSuccess AndAlso
                Not result.IsMissing AndAlso
                Not CanQuarantine(result)
        End Function

        Private Shared Function AppendMessage(existing As String, addition As String) As String
            If String.IsNullOrWhiteSpace(existing) Then
                Return If(addition, String.Empty)
            End If
            If String.IsNullOrWhiteSpace(addition) Then
                Return existing
            End If
            Return existing & " " & addition
        End Function

    End Class

End Namespace
