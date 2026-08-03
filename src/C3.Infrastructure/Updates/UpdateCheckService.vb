Namespace Updates

    Public Enum UpdateCheckOutcome
        Failed = 0
        NoPublishedRelease
        UpToDate
        UpdateAvailable
    End Enum

    Public NotInheritable Class UpdateCheckResult

        Private Sub New(
                outcome As UpdateCheckOutcome,
                manifest As UpdateReleaseManifest,
                message As String,
                failureException As Exception)

            Me.Outcome = outcome
            Me.Manifest = manifest
            Me.Message = If(message, String.Empty)
            Me.FailureException = failureException
        End Sub

        Public ReadOnly Property Outcome As UpdateCheckOutcome

        Public ReadOnly Property Manifest As UpdateReleaseManifest

        Public ReadOnly Property Message As String

        Public ReadOnly Property FailureException As Exception

        Public ReadOnly Property IsSuccess As Boolean
            Get
                Return Outcome <> UpdateCheckOutcome.Failed
            End Get
        End Property

        Public ReadOnly Property IsUpdateAvailable As Boolean
            Get
                Return Outcome = UpdateCheckOutcome.UpdateAvailable
            End Get
        End Property

        Friend Shared Function Completed(
                outcome As UpdateCheckOutcome,
                manifest As UpdateReleaseManifest,
                message As String) As UpdateCheckResult

            Return New UpdateCheckResult(outcome, manifest, message, Nothing)
        End Function

        Friend Shared Function Failed(
                message As String,
                Optional failureException As Exception = Nothing) As UpdateCheckResult

            Return New UpdateCheckResult(
                UpdateCheckOutcome.Failed,
                Nothing,
                message,
                failureException)
        End Function

    End Class

    ''' <summary>
    ''' Coordinates manifest retrieval and applies the publication and SemVer
    ''' precedence rules shared by both Windows build lanes.
    ''' </summary>
    Public NotInheritable Class UpdateCheckService

        Private ReadOnly _source As IUpdateManifestSource

        Public Sub New()
            Me.New(New HttpUpdateManifestSource())
        End Sub

        Public Sub New(source As IUpdateManifestSource)
            If source Is Nothing Then
                Throw New ArgumentNullException("source")
            End If
            _source = source
        End Sub

        Public Function Check(
                feedUrl As String,
                currentInformationalVersion As String,
                expectedChannel As String) As UpdateCheckResult

            Dim currentIdentity As SemanticVersion = Nothing
            If Not SemanticVersion.TryParse(currentInformationalVersion, currentIdentity) Then
                Return UpdateCheckResult.Failed(
                    "The current C3 release identity is invalid.")
            End If

            Dim feedUri As Uri = Nothing
            If Not Uri.TryCreate(feedUrl, UriKind.Absolute, feedUri) Then
                Return UpdateCheckResult.Failed(
                    "The configured update manifest endpoint is invalid.")
            End If

            Dim endpointFailure As String = Nothing
            If Not UpdateEndpointPolicy.TryValidate(
                    feedUri,
                    expectedChannel,
                    endpointFailure) Then
                Return UpdateCheckResult.Failed(endpointFailure)
            End If

            Try
                Dim readResult As UpdateManifestReadResult = _source.Read(
                    feedUri,
                    expectedChannel)
                If Not readResult.IsSuccess Then
                    Return UpdateCheckResult.Failed(
                        readResult.FailureMessage,
                        readResult.FailureException)
                End If
                Return Evaluate(currentIdentity, readResult.Manifest)
            Catch ex As Exception
                Return UpdateCheckResult.Failed(
                    "The update manifest could not be downloaded.",
                    ex)
            End Try
        End Function

        Public Shared Function Evaluate(
                currentInformationalVersion As String,
                manifest As UpdateReleaseManifest) As UpdateCheckResult

            Dim currentIdentity As SemanticVersion = Nothing
            If Not SemanticVersion.TryParse(currentInformationalVersion, currentIdentity) Then
                Return UpdateCheckResult.Failed(
                    "The current C3 release identity is invalid.")
            End If
            Return Evaluate(currentIdentity, manifest)
        End Function

        Private Shared Function Evaluate(
                currentIdentity As SemanticVersion,
                manifest As UpdateReleaseManifest) As UpdateCheckResult

            If manifest Is Nothing Then
                Return UpdateCheckResult.Failed("The update manifest is missing.")
            End If
            If Not manifest.Published Then
                Return UpdateCheckResult.Completed(
                    UpdateCheckOutcome.NoPublishedRelease,
                    manifest,
                    "The update channel does not currently advertise a published release.")
            End If
            If manifest.ReleaseIdentity.CompareTo(currentIdentity) > 0 Then
                Return UpdateCheckResult.Completed(
                    UpdateCheckOutcome.UpdateAvailable,
                    manifest,
                    "A newer published release is available.")
            End If
            Return UpdateCheckResult.Completed(
                UpdateCheckOutcome.UpToDate,
                manifest,
                "No newer published release is available.")
        End Function

    End Class

End Namespace
