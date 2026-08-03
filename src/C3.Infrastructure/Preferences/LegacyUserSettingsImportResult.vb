Imports System.Collections.Generic

Namespace Preferences

    Public Enum LegacyUserSettingsImportStatus
        NotFound = 0
        Imported
        Failed
    End Enum

    Public NotInheritable Class LegacyUserSettingsImportResult

        Private Sub New(
                status As LegacyUserSettingsImportStatus,
                profile As LegacyUserSettingsProfile,
                rejectedCandidates As IList(Of LegacySettingsProfileReadResult))

            Me.Status = status
            Me.Profile = profile
            Me.RejectedCandidates = New List(Of LegacySettingsProfileReadResult)(
                rejectedCandidates).AsReadOnly()
        End Sub

        Public ReadOnly Property Status As LegacyUserSettingsImportStatus

        Public ReadOnly Property Profile As LegacyUserSettingsProfile

        Public ReadOnly Property RejectedCandidates As IList(Of LegacySettingsProfileReadResult)

        ''' <summary>
        ''' No profile is a successful no-op; only discovered-but-unreadable
        ''' candidates make the import attempt fail.
        ''' </summary>
        Public ReadOnly Property IsSuccess As Boolean
            Get
                Return Status <> LegacyUserSettingsImportStatus.Failed
            End Get
        End Property

        Friend Shared Function NotFound() As LegacyUserSettingsImportResult
            Return New LegacyUserSettingsImportResult(
                LegacyUserSettingsImportStatus.NotFound,
                Nothing,
                New List(Of LegacySettingsProfileReadResult)())
        End Function

        Friend Shared Function Imported(
                profile As LegacyUserSettingsProfile,
                rejectedCandidates As IList(Of LegacySettingsProfileReadResult)) _
                As LegacyUserSettingsImportResult

            Return New LegacyUserSettingsImportResult(
                LegacyUserSettingsImportStatus.Imported,
                profile,
                rejectedCandidates)
        End Function

        Friend Shared Function Failed(
                rejectedCandidates As IList(Of LegacySettingsProfileReadResult)) _
                As LegacyUserSettingsImportResult

            Return New LegacyUserSettingsImportResult(
                LegacyUserSettingsImportStatus.Failed,
                Nothing,
                rejectedCandidates)
        End Function

    End Class

End Namespace
