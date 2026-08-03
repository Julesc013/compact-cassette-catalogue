Public NotInheritable Class RuntimeInfo

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property BuildLabel As String
        Get
            If IntPtr.Size = 8 Then
                Return "x64 / .NET Framework 4.0 (transition)"
            End If
            Return "x86 / .NET Framework 4.0"
        End Get
    End Property

End Class

