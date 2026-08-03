Imports System.Globalization

Namespace FileOperations

    ''' <summary>
    ''' Derives compact, collision-resistant sibling names for file operations
    ''' that must remain below the classic Win32 path boundary.
    ''' </summary>
    Friend NotInheritable Class CompactSiblingFileName

        Private Sub New()
        End Sub

        Public Shared Function CreateTemporary() As String
            Return "~c3" & CreateToken(13) & ".tmp"
        End Function

        Public Shared Function CreateRecovery(stamp As DateTime) As String
            ' Keep the recovery marker and second-resolution UTC chronology
            ' recognizable while using three token characters to distinguish
            ' concurrent recoveries. The complete name is twenty-four chars.
            Return ".bad-" &
                stamp.ToUniversalTime().ToString("yyMMddHHmmss", CultureInfo.InvariantCulture) &
                CreateToken(3) &
                ".xml"
        End Function

        Private Shared Function CreateToken(characterCount As Integer) As String
            Dim token As String = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).
                TrimEnd("="c).
                Replace("+"c, "-"c).
                Replace("/"c, "_"c)
            Return token.Substring(0, characterCount)
        End Function

    End Class

End Namespace
