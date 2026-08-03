Imports C3.Infrastructure.Updates

Namespace Preferences

    ''' <summary>
    ''' The supported values read from one untouched C3 1.x user.config file.
    ''' </summary>
    Public NotInheritable Class LegacyUserSettingsProfile

        Friend Sub New(
                candidate As LegacySettingsProfileCandidate,
                hasShowMessages As Boolean,
                showMessages As Boolean,
                hasDefaultDirectory As Boolean,
                defaultDirectory As String,
                hasUpdatePolicy As Boolean,
                updatePolicy As UpdateCheckPolicy,
                hasLastUpdateCheck As Boolean,
                lastUpdateCheck As DateTime)

            If candidate Is Nothing Then
                Throw New ArgumentNullException("candidate")
            End If

            Me.Candidate = candidate
            Me.HasShowMessages = hasShowMessages
            Me.ShowMessages = showMessages
            Me.HasDefaultDirectory = hasDefaultDirectory
            Me.DefaultDirectory = defaultDirectory
            Me.HasUpdatePolicy = hasUpdatePolicy
            Me.UpdatePolicy = updatePolicy
            Me.HasLastUpdateCheck = hasLastUpdateCheck
            Me.LastUpdateCheck = lastUpdateCheck
        End Sub

        Public ReadOnly Property Candidate As LegacySettingsProfileCandidate

        Public ReadOnly Property SourcePath As String
            Get
                Return Candidate.FilePath
            End Get
        End Property

        Public ReadOnly Property SourceVersion As Version
            Get
                Return Candidate.ProfileVersion
            End Get
        End Property

        Public ReadOnly Property HasShowMessages As Boolean

        Public ReadOnly Property ShowMessages As Boolean

        Public ReadOnly Property HasDefaultDirectory As Boolean

        Public ReadOnly Property DefaultDirectory As String

        Public ReadOnly Property HasUpdatePolicy As Boolean

        Public ReadOnly Property UpdatePolicy As UpdateCheckPolicy

        Public ReadOnly Property HasLastUpdateCheck As Boolean

        Public ReadOnly Property LastUpdateCheck As DateTime

    End Class

End Namespace
