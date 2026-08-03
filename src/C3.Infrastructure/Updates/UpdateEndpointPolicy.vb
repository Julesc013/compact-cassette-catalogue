Namespace Updates

    ''' <summary>
    ''' Defines the only remote update-feed endpoints trusted by C3 2.x.
    ''' Keeping this rule in one place prevents configuration drift from widening
    ''' the updater's network authority.
    ''' </summary>
    Public NotInheritable Class UpdateEndpointPolicy

        Private Const AlphaChannel As String = "alpha"
        Private Const BetaChannel As String = "beta"
        Private Const StableChannel As String = "stable"
        Private Const RepositoryPath As String =
            "/Julesc013/compact-cassette-catalogue/"
        Private Const FeedHost As String = "raw.githubusercontent.com"

        Private Sub New()
        End Sub

        Public Shared Function ExpectedUrl(channel As String) As String
            Dim branch As String = BranchForChannel(channel)
            Return "https://" & FeedHost & RepositoryPath & branch &
                "/release/feeds/" & channel & "/release.json"
        End Function

        Public Shared Function TryValidate(
                feedUri As Uri,
                expectedChannel As String,
                ByRef failureMessage As String) As Boolean

            failureMessage = Nothing

            Dim expectedUrl As String = Nothing
            Try
                expectedUrl = UpdateEndpointPolicy.ExpectedUrl(expectedChannel)
            Catch ex As ArgumentException
                failureMessage = ex.Message
                Return False
            End Try

            If feedUri Is Nothing OrElse Not feedUri.IsAbsoluteUri Then
                failureMessage = "The update manifest endpoint must be an absolute URI."
                Return False
            End If
            If Not String.Equals(
                    feedUri.Scheme,
                    Uri.UriSchemeHttps,
                    StringComparison.Ordinal) Then
                failureMessage = "The update manifest endpoint must use HTTPS."
                Return False
            End If
            If Not String.Equals(
                    feedUri.Host,
                    FeedHost,
                    StringComparison.Ordinal) Then
                failureMessage = "The update manifest endpoint host is not trusted."
                Return False
            End If
            If feedUri.Port <> 443 OrElse Not feedUri.IsDefaultPort Then
                failureMessage = "The update manifest endpoint must use the default HTTPS port."
                Return False
            End If
            If feedUri.UserInfo.Length <> 0 OrElse
                    feedUri.Query.Length <> 0 OrElse
                    feedUri.Fragment.Length <> 0 Then
                failureMessage =
                    "The update manifest endpoint must not contain credentials, a query, or a fragment."
                Return False
            End If
            If Not String.Equals(
                    feedUri.OriginalString,
                    expectedUrl,
                    StringComparison.Ordinal) Then
                failureMessage =
                    "The update manifest endpoint does not match the configured C3 channel."
                Return False
            End If

            Return True
        End Function

        Public Shared Sub Validate(feedUri As Uri, expectedChannel As String)
            Dim failureMessage As String = Nothing
            If Not TryValidate(feedUri, expectedChannel, failureMessage) Then
                Throw New ArgumentException(failureMessage, "feedUri")
            End If
        End Sub

        Private Shared Function BranchForChannel(channel As String) As String
            Select Case channel
                Case AlphaChannel
                    Return "dev"
                Case BetaChannel, StableChannel
                    Return "master"
                Case Else
                    Throw New ArgumentException(
                        "The expected update channel is invalid.",
                        "channel")
            End Select
        End Function

    End Class

End Namespace
