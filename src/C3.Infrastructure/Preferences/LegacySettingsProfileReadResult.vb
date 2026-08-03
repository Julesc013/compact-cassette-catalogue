Namespace Preferences

    Public Enum LegacySettingsProfileReadFailure
        None = 0
        Unavailable
        TooLarge
        MalformedXml
        InvalidStructure
        DuplicateSetting
        InvalidValue
    End Enum

    Public NotInheritable Class LegacySettingsProfileReadResult

        Private Sub New(
                candidate As LegacySettingsProfileCandidate,
                profile As LegacyUserSettingsProfile,
                failure As LegacySettingsProfileReadFailure,
                failureMessage As String,
                failureException As Exception)

            Me.Candidate = candidate
            Me.Profile = profile
            Me.Failure = failure
            Me.FailureMessage = failureMessage
            Me.FailureException = failureException
        End Sub

        Public ReadOnly Property Candidate As LegacySettingsProfileCandidate

        Public ReadOnly Property Profile As LegacyUserSettingsProfile

        Public ReadOnly Property Failure As LegacySettingsProfileReadFailure

        Public ReadOnly Property FailureMessage As String

        Public ReadOnly Property FailureException As Exception

        Public ReadOnly Property IsSuccess As Boolean
            Get
                Return Profile IsNot Nothing
            End Get
        End Property

        Friend Shared Function Succeeded(
                profile As LegacyUserSettingsProfile) As LegacySettingsProfileReadResult

            Return New LegacySettingsProfileReadResult(
                profile.Candidate,
                profile,
                LegacySettingsProfileReadFailure.None,
                Nothing,
                Nothing)
        End Function

        Friend Shared Function Failed(
                candidate As LegacySettingsProfileCandidate,
                failure As LegacySettingsProfileReadFailure,
                message As String,
                Optional failureException As Exception = Nothing) As LegacySettingsProfileReadResult

            Return New LegacySettingsProfileReadResult(
                candidate,
                Nothing,
                failure,
                message,
                failureException)
        End Function

    End Class

End Namespace
