Friend NotInheritable Class UpdateEndpointTransportTests

    Private Sub New()
    End Sub

    Public Shared Sub AcceptsOnlyExactChannelEndpoints()
        AssertAccepted(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json",
            "alpha",
            "alpha endpoint")
        AssertAccepted(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/release/feeds/beta/release.json",
            "beta",
            "beta endpoint")
        AssertAccepted(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/release/feeds/stable/release.json",
            "stable",
            "stable endpoint")

        AssertRejected(DirectCast(Nothing, Uri), "alpha", "missing URI")
        AssertRejected(
            New Uri("release/feeds/alpha/release.json", UriKind.Relative),
            "alpha",
            "relative URI")
        AssertRejected(
            "http://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json",
            "alpha",
            "HTTP")
        AssertRejected(
            "https://github.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json",
            "alpha",
            "wrong host")
        AssertRejected(
            "https://raw.githubusercontent.com:444/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json",
            "alpha",
            "alternate port")
        AssertRejected(
            "https://raw.githubusercontent.com:443/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json",
            "alpha",
            "explicit default port")
        AssertRejected(
            "https://user@raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json",
            "alpha",
            "credentials")
        AssertRejected(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json?raw=1",
            "alpha",
            "query")
        AssertRejected(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json#manifest",
            "alpha",
            "fragment")
        AssertRejected(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/release/feeds/alpha/release.json",
            "alpha",
            "alpha wrong branch")
        AssertRejected(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/beta/release.json",
            "beta",
            "beta wrong branch")
        AssertRejected(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/release/feeds/beta/release.json",
            "stable",
            "wrong channel path")
        AssertRejected(
            "https://raw.githubusercontent.com/julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json",
            "alpha",
            "path casing drift")
        AssertRejected(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/%72elease.json",
            "alpha",
            "encoded path drift")
        AssertRejected(
            "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/dev/release/feeds/alpha/release.json",
            "preview",
            "unsupported channel")
    End Sub

    Public Shared Sub ServiceUsesInjectedManifestSource()
        Dim manifestBytes As Byte() = File.ReadAllBytes(
            Path.Combine(
                FindRepositoryRoot(),
                "release\feeds\alpha\release.json"))
        Dim successfulRead As UpdateManifestReadResult =
            New UpdateReleaseManifestReader().Read(manifestBytes, "alpha")
        If Not successfulRead.IsSuccess Then
            Throw New InvalidOperationException(
                "The generated alpha manifest is not a valid service fixture: " &
                successfulRead.FailureMessage)
        End If

        Dim source As New StubManifestSource(successfulRead, Nothing)
        Dim service As New UpdateCheckService(source)
        Dim feedUrl As String = UpdateEndpointPolicy.ExpectedUrl("alpha")
        Dim result As UpdateCheckResult = service.Check(
            feedUrl,
            "2.0.0-alpha.1",
            "alpha")

        AssertEqual(True, result.IsSuccess, "injected source success")
        AssertEqual(
            UpdateCheckOutcome.NoPublishedRelease,
            result.Outcome,
            "unpublished source outcome")
        AssertEqual(1, source.ReadCount, "injected source call count")
        AssertEqual(feedUrl, source.LastUri.OriginalString, "injected source URI")
        AssertEqual("alpha", source.LastChannel, "injected source channel")

        Dim callsBeforeRejectedEndpoint As Integer = source.ReadCount
        Dim rejectedEndpoint As UpdateCheckResult = service.Check(
            feedUrl & "?unexpected=true",
            "2.0.0-alpha.1",
            "alpha")
        AssertEqual(False, rejectedEndpoint.IsSuccess, "endpoint rejection")
        AssertEqual(
            callsBeforeRejectedEndpoint,
            source.ReadCount,
            "endpoint rejected before source")

        Dim callsBeforeRejectedIdentity As Integer = source.ReadCount
        Dim rejectedIdentity As UpdateCheckResult = service.Check(
            feedUrl,
            "not-a-version",
            "alpha")
        AssertEqual(False, rejectedIdentity.IsSuccess, "identity rejection")
        AssertEqual(
            callsBeforeRejectedIdentity,
            source.ReadCount,
            "identity rejected before source")

        Dim malformedRead As UpdateManifestReadResult =
            New UpdateReleaseManifestReader().Read(
                Encoding.UTF8.GetBytes("{""published"":"),
                "alpha")
        Dim malformedResult As UpdateCheckResult =
            New UpdateCheckService(
                New StubManifestSource(malformedRead, Nothing)).Check(
                    feedUrl,
                    "2.0.0-alpha.1",
                    "alpha")
        AssertEqual(False, malformedResult.IsSuccess, "source read failure")
        AssertEqual(
            malformedRead.FailureMessage,
            malformedResult.Message,
            "source read failure message")

        Dim retrievalException As New InvalidOperationException("fixture transport failure")
        Dim thrownResult As UpdateCheckResult =
            New UpdateCheckService(
                New StubManifestSource(Nothing, retrievalException)).Check(
                    feedUrl,
                    "2.0.0-alpha.1",
                    "alpha")
        AssertEqual(False, thrownResult.IsSuccess, "source exception failure")
        AssertEqual(
            retrievalException,
            thrownResult.FailureException,
            "source exception evidence")

        AssertThrowsArgumentNull(
            Sub()
                Dim unused As New UpdateCheckService(Nothing)
            End Sub,
            "missing source")
    End Sub

    Public Shared Sub HttpSourceRequiresExplicitLegacyTlsMode()
        AssertEqual(
            False,
            New HttpUpdateManifestSource().UsesLegacyTls12Compatibility,
            "modern source default")
        AssertEqual(
            True,
            New HttpUpdateManifestSource(True).UsesLegacyTls12Compatibility,
            "legacy source opt-in")

        ' Endpoint validation happens before the optional process-wide TLS scope.
        ' This gives the failure path a deterministic, network-free regression
        ' check and proves untrusted URLs cannot trigger a protocol mutation.
        Dim originalProtocol As System.Net.SecurityProtocolType =
            System.Net.ServicePointManager.SecurityProtocol
        Try
            Dim unusedResult As UpdateManifestReadResult =
                New HttpUpdateManifestSource(True).Read(
                New Uri("https://example.invalid/release.json"),
                "alpha")
            Throw New InvalidOperationException(
                "untrusted transport endpoint was accepted.")
        Catch ex As ArgumentException
            ' Expected.
        End Try
        AssertEqual(
            originalProtocol,
            System.Net.ServicePointManager.SecurityProtocol,
            "rejected endpoint TLS policy")
    End Sub

    Private Shared Sub AssertAccepted(url As String, channel As String, name As String)
        AssertAccepted(New Uri(url, UriKind.Absolute), channel, name)
    End Sub

    Private Shared Sub AssertAccepted(uri As Uri, channel As String, name As String)
        Dim failureMessage As String = Nothing
        If Not UpdateEndpointPolicy.TryValidate(uri, channel, failureMessage) Then
            Throw New InvalidOperationException(
                name & " was rejected: " & failureMessage)
        End If
    End Sub

    Private Shared Sub AssertRejected(url As String, channel As String, name As String)
        AssertRejected(New Uri(url, UriKind.Absolute), channel, name)
    End Sub

    Private Shared Sub AssertRejected(uri As Uri, channel As String, name As String)
        Dim failureMessage As String = Nothing
        If UpdateEndpointPolicy.TryValidate(uri, channel, failureMessage) Then
            Throw New InvalidOperationException(name & " was accepted.")
        End If
        If String.IsNullOrEmpty(failureMessage) Then
            Throw New InvalidOperationException(name & " did not explain its rejection.")
        End If
    End Sub

    Private Shared Sub AssertThrowsArgumentNull(action As Action, name As String)
        Try
            action()
        Catch ex As ArgumentNullException
            Return
        End Try
        Throw New InvalidOperationException(name & " did not throw ArgumentNullException.")
    End Sub

    Private Shared Function FindRepositoryRoot() As String
        Dim directory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While directory IsNot Nothing
            If System.IO.File.Exists(Path.Combine(directory.FullName, "VERSION")) AndAlso
                    System.IO.Directory.Exists(
                        Path.Combine(directory.FullName, "release\feeds")) Then
                Return directory.FullName
            End If
            directory = directory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format(
                    "{0}: expected '{1}', found '{2}'.",
                    name,
                    expected,
                    actual))
        End If
    End Sub

    Private NotInheritable Class StubManifestSource
        Implements IUpdateManifestSource

        Private ReadOnly _exceptionToThrow As Exception
        Private ReadOnly _result As UpdateManifestReadResult

        Public Sub New(
                result As UpdateManifestReadResult,
                exceptionToThrow As Exception)

            _result = result
            _exceptionToThrow = exceptionToThrow
        End Sub

        Public ReadOnly Property ReadCount As Integer

        Public ReadOnly Property LastUri As Uri

        Public ReadOnly Property LastChannel As String

        Public Function Read(
                feedUri As Uri,
                expectedChannel As String) As UpdateManifestReadResult _
                Implements IUpdateManifestSource.Read

            _ReadCount += 1
            _LastUri = feedUri
            _LastChannel = expectedChannel

            If _exceptionToThrow IsNot Nothing Then
                Throw _exceptionToThrow
            End If
            Return _result
        End Function

    End Class

End Class
