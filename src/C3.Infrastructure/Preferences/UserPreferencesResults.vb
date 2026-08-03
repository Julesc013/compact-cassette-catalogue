Namespace Preferences

Public Enum UserPreferencesFailure
    None
    Missing
    Invalid
    UnsupportedVersion
    TooLarge
    AccessDenied
    IoFailure
    Busy
    VerificationFailure
End Enum

Public NotInheritable Class UserPreferencesLoadResult

    Private Sub New(
            isSuccess As Boolean,
            preferences As UserPreferencesSnapshot,
            failure As UserPreferencesFailure,
            message As String,
            recoveryPath As String,
            migrationOutcome As String)

        Me.IsSuccess = isSuccess
        Me.Preferences = preferences
        Me.Failure = failure
        Me.Message = If(message, String.Empty)
        Me.RecoveryPath = recoveryPath
        Me.MigrationOutcome = migrationOutcome
    End Sub

    Public ReadOnly Property IsSuccess As Boolean

    Public ReadOnly Property Preferences As UserPreferencesSnapshot

    Public ReadOnly Property Failure As UserPreferencesFailure

    Public ReadOnly Property Message As String

    Public ReadOnly Property RecoveryPath As String

    Public ReadOnly Property MigrationOutcome As String

    Public ReadOnly Property IsMissing As Boolean
        Get
            Return Failure = UserPreferencesFailure.Missing
        End Get
    End Property

    Public Shared Function Loaded(
            preferences As UserPreferencesSnapshot,
            Optional recoveryPath As String = Nothing,
            Optional migrationOutcome As String = Nothing,
            Optional message As String = Nothing) As UserPreferencesLoadResult

        If preferences Is Nothing Then
            Throw New ArgumentNullException("preferences")
        End If
        Return New UserPreferencesLoadResult(
            True,
            preferences,
            UserPreferencesFailure.None,
            message,
            recoveryPath,
            migrationOutcome)
    End Function

    Public Shared Function Missing() As UserPreferencesLoadResult
        Return New UserPreferencesLoadResult(
            False,
            Nothing,
            UserPreferencesFailure.Missing,
            "The preferences file does not exist.",
            Nothing,
            Nothing)
    End Function

    Public Shared Function Failed(
            failure As UserPreferencesFailure,
            message As String,
            Optional recoveryPath As String = Nothing,
            Optional fallbackPreferences As UserPreferencesSnapshot = Nothing,
            Optional migrationOutcome As String = Nothing) As UserPreferencesLoadResult

        Return New UserPreferencesLoadResult(
            False,
            fallbackPreferences,
            failure,
            message,
            recoveryPath,
            migrationOutcome)
    End Function

End Class

Public NotInheritable Class UserPreferencesSaveResult

    Private Sub New(
            isSuccess As Boolean,
            preferences As UserPreferencesSnapshot,
            failure As UserPreferencesFailure,
            message As String,
            backupPath As String)

        Me.IsSuccess = isSuccess
        Me.Preferences = preferences
        Me.Failure = failure
        Me.Message = If(message, String.Empty)
        Me.BackupPath = backupPath
    End Sub

    Public ReadOnly Property IsSuccess As Boolean

    Public ReadOnly Property Preferences As UserPreferencesSnapshot

    Public ReadOnly Property Failure As UserPreferencesFailure

    Public ReadOnly Property Message As String

    Public ReadOnly Property BackupPath As String

    Public Shared Function Saved(
            preferences As UserPreferencesSnapshot,
            backupPath As String) As UserPreferencesSaveResult

        If preferences Is Nothing Then
            Throw New ArgumentNullException("preferences")
        End If
        Return New UserPreferencesSaveResult(
            True,
            preferences,
            UserPreferencesFailure.None,
            String.Empty,
            backupPath)
    End Function

    Public Shared Function Failed(
            failure As UserPreferencesFailure,
            message As String) As UserPreferencesSaveResult

        Return New UserPreferencesSaveResult(False, Nothing, failure, message, Nothing)
    End Function

End Class

End Namespace
