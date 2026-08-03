Public NotInheritable Class RuntimeInfo

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property BuildLabel As String
        Get
#If C3_NET48 Then
            Return "x64 / .NET Framework 4.8"
#Else
            Return "x86 / .NET Framework 4.0"
#End If
        End Get
    End Property

End Class
