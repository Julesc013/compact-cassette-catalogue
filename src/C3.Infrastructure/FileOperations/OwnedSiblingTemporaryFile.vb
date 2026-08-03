Namespace FileOperations

    ''' <summary>
    ''' Owns a uniquely-created temporary file beside a destination so a later
    ''' replace or move remains atomic on legacy Windows file systems.
    ''' </summary>
    Friend NotInheritable Class OwnedSiblingTemporaryFile
        Implements IDisposable

        Private Const MaximumCreationAttempts As Integer = 32
        Private ReadOnly _path As String
        Private ReadOnly _stream As FileStream
        Private _disposed As Boolean

        Private Sub New(pathValue As String, streamValue As FileStream)
            _path = pathValue
            _stream = streamValue
        End Sub

        Public ReadOnly Property Path As String
            Get
                Return _path
            End Get
        End Property

        Public ReadOnly Property Stream As FileStream
            Get
                If _disposed Then
                    Throw New ObjectDisposedException("OwnedSiblingTemporaryFile")
                End If
                Return _stream
            End Get
        End Property

        Public Shared Function Create(destinationPath As String) As OwnedSiblingTemporaryFile
            If String.IsNullOrWhiteSpace(destinationPath) Then
                Throw New ArgumentException("A destination path is required.", "destinationPath")
            End If

            Dim fullPath As String = System.IO.Path.GetFullPath(destinationPath)
            Dim directoryPath As String = System.IO.Path.GetDirectoryName(fullPath)
            If String.IsNullOrWhiteSpace(directoryPath) Then
                Throw New DirectoryNotFoundException("The destination directory could not be determined.")
            End If

            For attempt As Integer = 1 To MaximumCreationAttempts
                Dim candidatePath As String = System.IO.Path.Combine(
                    directoryPath,
                    CompactSiblingFileName.CreateTemporary())
                Try
                    Dim stream As New FileStream(
                        candidatePath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None)
                    Return New OwnedSiblingTemporaryFile(candidatePath, stream)
                Catch ex As IOException
                    ' CreateNew proves ownership. Retry only when a genuine name
                    ' collision exists; path, media, and other I/O failures must
                    ' retain their original classification and message.
                    If Not File.Exists(candidatePath) AndAlso
                            Not Directory.Exists(candidatePath) Then
                        Throw
                    End If
                End Try
            Next

            Throw New IOException("C3 could not reserve a unique sibling temporary file.")
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then
                Return
            End If
            _disposed = True

            Try
                _stream.Dispose()
            Finally
                Try
                    If File.Exists(_path) Then
                        File.Delete(_path)
                    End If
                Catch
                    ' Cleanup must not replace the transaction's real result.
                End Try
            End Try
        End Sub

    End Class

End Namespace
