Imports System.Net
Imports System.Threading

Namespace Updates

    ''' <summary>
    ''' Downloads one C3 release manifest through the narrowly scoped endpoint
    ''' policy. Redirects are disabled and decompressed response bytes are bounded
    ''' before JSON parsing.
    ''' </summary>
    Public NotInheritable Class HttpUpdateManifestSource
        Implements IUpdateManifestSource

        Private Const RequestDeadlineMilliseconds As Integer = 15000

        Private ReadOnly _enableLegacyTls12Compatibility As Boolean
        Private ReadOnly _reader As UpdateReleaseManifestReader

        Public Sub New()
            Me.New(False)
        End Sub

        ''' <summary>
        ''' Creates the HTTP source. The compatibility switch is intended only for
        ''' the .NET 4.0 UI lane; modern lanes must leave it disabled.
        ''' </summary>
        Public Sub New(enableLegacyTls12Compatibility As Boolean)
            _enableLegacyTls12Compatibility = enableLegacyTls12Compatibility
            _reader = New UpdateReleaseManifestReader()
        End Sub

        Public ReadOnly Property UsesLegacyTls12Compatibility As Boolean
            Get
                Return _enableLegacyTls12Compatibility
            End Get
        End Property

        Public Function Read(
                feedUri As Uri,
                expectedChannel As String) As UpdateManifestReadResult _
                Implements IUpdateManifestSource.Read

            UpdateEndpointPolicy.Validate(feedUri, expectedChannel)

            Dim request As HttpWebRequest = DirectCast(
                WebRequest.Create(feedUri),
                HttpWebRequest)
            request.Method = "GET"
            request.AllowAutoRedirect = False
            request.AutomaticDecompression = DecompressionMethods.GZip Or
                DecompressionMethods.Deflate
            request.Timeout = RequestDeadlineMilliseconds
            request.ReadWriteTimeout = RequestDeadlineMilliseconds
            request.UserAgent = "C3/2 update manifest client"

            Dim tlsScope As IDisposable = Nothing
            If _enableLegacyTls12Compatibility Then
                tlsScope = LegacyTls12Scope.Enter()
            End If

            Try
                Return ReadResponse(request, expectedChannel)
            Finally
                If tlsScope IsNot Nothing Then
                    tlsScope.Dispose()
                End If
            End Try
        End Function

        Private Function ReadResponse(
                request As HttpWebRequest,
                expectedChannel As String) As UpdateManifestReadResult

            Using deadline As New HttpRequestDeadline(
                    request,
                    RequestDeadlineMilliseconds)

                Try
                    Using response As HttpWebResponse = DirectCast(
                            request.GetResponse(),
                            HttpWebResponse)

                        If response.StatusCode <> HttpStatusCode.OK Then
                            Throw New WebException(
                                "The update manifest endpoint returned HTTP " &
                                    CInt(response.StatusCode).ToString() & ".")
                        End If
                        If response.ContentLength >
                                UpdateReleaseManifestReader.MaximumManifestBytes Then
                            deadline.Complete()
                            Return UpdateManifestReadResult.Failed(
                                UpdateManifestReadFailure.TooLarge,
                                "The update manifest exceeds the 32 KiB safety limit.")
                        End If

                        Using responseStream As Stream = response.GetResponseStream()
                            Dim payload As Byte() = ReadBounded(responseStream)
                            deadline.Complete()
                            If payload Is Nothing Then
                                Return UpdateManifestReadResult.Failed(
                                    UpdateManifestReadFailure.TooLarge,
                                    "The update manifest exceeds the 32 KiB safety limit.")
                            End If
                            Return _reader.Read(payload, expectedChannel)
                        End Using
                    End Using
                Catch ex As Exception
                    If deadline.HasExpired Then
                        If TypeOf ex Is TimeoutException Then
                            Throw
                        End If
                        Throw New TimeoutException(
                            "The update manifest request exceeded its 15-second deadline.",
                            ex)
                    End If
                    Throw
                End Try
            End Using
        End Function

        Private Shared Function ReadBounded(source As Stream) As Byte()
            If source Is Nothing Then
                Return New Byte() {}
            End If

            Using destination As New MemoryStream()
                Dim buffer(4095) As Byte
                Do
                    Dim readCount As Integer = source.Read(buffer, 0, buffer.Length)
                    If readCount = 0 Then
                        Exit Do
                    End If
                    If destination.Length + readCount >
                            UpdateReleaseManifestReader.MaximumManifestBytes Then
                        Return Nothing
                    End If
                    destination.Write(buffer, 0, readCount)
                Loop
                Return destination.ToArray()
            End Using
        End Function

        ''' <summary>
        ''' Uses a timer to abort the request when one deadline covering DNS,
        ''' connection, response headers, and streamed response bytes expires.
        ''' </summary>
        Private NotInheritable Class HttpRequestDeadline
            Implements IDisposable

            Private Const ActiveState As Integer = 0
            Private Const CompletedState As Integer = 1
            Private Const ExpiredState As Integer = 2

            Private ReadOnly _request As HttpWebRequest
            Private ReadOnly _timer As Timer
            Private _state As Integer

            Public Sub New(request As HttpWebRequest, timeoutMilliseconds As Integer)
                _request = request
                _timer = New Timer(
                    AddressOf Expire,
                    Nothing,
                    timeoutMilliseconds,
                    System.Threading.Timeout.Infinite)
            End Sub

            Public ReadOnly Property HasExpired As Boolean
                Get
                    Return Interlocked.CompareExchange(
                        _state,
                        ActiveState,
                        ActiveState) = ExpiredState
                End Get
            End Property

            Public Sub Complete()
                Dim previousState As Integer = Interlocked.CompareExchange(
                    _state,
                    CompletedState,
                    ActiveState)
                _timer.Dispose()

                If previousState = ExpiredState Then
                    Throw New TimeoutException(
                        "The update manifest request exceeded its 15-second deadline.")
                End If
            End Sub

            Private Sub Expire(state As Object)
                If Interlocked.CompareExchange(
                        _state,
                        ExpiredState,
                        ActiveState) <> ActiveState Then
                    Return
                End If

                Try
                    _request.Abort()
                Catch ex As Exception
                    ' Timer callbacks must never terminate the process. The expired
                    ' state still makes the caller report a deterministic timeout.
                End Try
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                Interlocked.CompareExchange(
                    _state,
                    CompletedState,
                    ActiveState)
                _timer.Dispose()
            End Sub

        End Class

        ''' <summary>
        ''' Temporarily selects exactly TLS 1.2 for the legacy .NET 4.0 lane.
        ''' ServicePointManager is process-wide, so updater scopes are serialized.
        ''' A restoration is skipped when another component changed the setting.
        ''' </summary>
        Private NotInheritable Class LegacyTls12Scope
            Implements IDisposable

            Private Shared ReadOnly SynchronizationRoot As New Object()
            Private Shared ReadOnly Tls12 As SecurityProtocolType =
                CType(3072, SecurityProtocolType)

            Private ReadOnly _originalProtocol As SecurityProtocolType
            Private ReadOnly _changedProtocol As Boolean
            Private _disposed As Boolean

            Private Sub New()
                Dim capturedOriginalProtocol As Boolean = False
                Monitor.Enter(SynchronizationRoot)
                Try
                    _originalProtocol = ServicePointManager.SecurityProtocol
                    capturedOriginalProtocol = True
                    If _originalProtocol <> Tls12 Then
                        ServicePointManager.SecurityProtocol = Tls12
                        If ServicePointManager.SecurityProtocol <> Tls12 Then
                            Throw New InvalidOperationException(
                                "The process TLS policy did not accept TLS 1.2.")
                        End If
                        _changedProtocol = True
                    End If
                Catch ex As Exception
                    Try
                        Try
                            If capturedOriginalProtocol AndAlso
                                    ServicePointManager.SecurityProtocol = Tls12 Then
                                ServicePointManager.SecurityProtocol = _originalProtocol
                            End If
                        Catch restoreException As Exception
                            ' Activation already failed; preserve its original
                            ' diagnostic while still releasing the updater lock.
                        End Try
                    Finally
                        Monitor.Exit(SynchronizationRoot)
                    End Try
                    Throw New InvalidOperationException(
                        "TLS 1.2 compatibility mode could not be activated.",
                        ex)
                End Try
            End Sub

            Public Shared Function Enter() As IDisposable
                Return New LegacyTls12Scope()
            End Function

            Public Sub Dispose() Implements IDisposable.Dispose
                If _disposed Then
                    Return
                End If

                Try
                    If _changedProtocol AndAlso
                            ServicePointManager.SecurityProtocol = Tls12 Then
                        ServicePointManager.SecurityProtocol = _originalProtocol
                    End If
                Finally
                    _disposed = True
                    Monitor.Exit(SynchronizationRoot)
                End Try
            End Sub

        End Class

    End Class

End Namespace
