Namespace Updates

    Public NotInheritable Class UpdateCheckSchedule

        Private Sub New()
        End Sub

        Public Shared Function Parse(value As String) As UpdateCheckPolicy
            Select Case If(value, String.Empty).Trim().ToLowerInvariant()
                Case "startup"
                    Return UpdateCheckPolicy.Startup
                Case "weekly"
                    Return UpdateCheckPolicy.Weekly
                Case "monthly"
                    Return UpdateCheckPolicy.Monthly
                Case Else
                    Return UpdateCheckPolicy.Never
            End Select
        End Function

        Public Shared Function Serialize(value As UpdateCheckPolicy) As String
            Select Case value
                Case UpdateCheckPolicy.Startup
                    Return "startup"
                Case UpdateCheckPolicy.Weekly
                    Return "weekly"
                Case UpdateCheckPolicy.Monthly
                    Return "monthly"
                Case Else
                    Return "never"
            End Select
        End Function

        Public Shared Function ShouldCheck(
                policy As UpdateCheckPolicy,
                lastCheckedAt As DateTime,
                now As DateTime) As Boolean

            Select Case policy
                Case UpdateCheckPolicy.Startup
                    Return True
                Case UpdateCheckPolicy.Weekly
                    Return lastCheckedAt = DateTime.MinValue OrElse
                        (now - lastCheckedAt).TotalDays >= 7D
                Case UpdateCheckPolicy.Monthly
                    Return lastCheckedAt = DateTime.MinValue OrElse
                        (now - lastCheckedAt).TotalDays >= 28D
                Case Else
                    Return False
            End Select
        End Function

    End Class

End Namespace
