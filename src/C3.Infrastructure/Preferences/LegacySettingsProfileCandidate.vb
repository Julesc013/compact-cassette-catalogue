Imports System.IO

Namespace Preferences

    ''' <summary>
    ''' Identifies one legacy ApplicationSettingsBase profile without opening it.
    ''' </summary>
    Public NotInheritable Class LegacySettingsProfileCandidate

        Public Sub New(filePath As String, profileVersion As Version, lastWriteTimeUtc As DateTime)
            If String.IsNullOrWhiteSpace(filePath) Then
                Throw New ArgumentException("A settings profile path is required.", "filePath")
            End If
            If profileVersion Is Nothing Then
                Throw New ArgumentNullException("profileVersion")
            End If

            Me.FilePath = Path.GetFullPath(filePath)
            Me.ProfileVersion = profileVersion
            Me.LastWriteTimeUtc = lastWriteTimeUtc.ToUniversalTime()
        End Sub

        Public ReadOnly Property FilePath As String

        Public ReadOnly Property ProfileVersion As Version

        Public ReadOnly Property LastWriteTimeUtc As DateTime

    End Class

End Namespace
