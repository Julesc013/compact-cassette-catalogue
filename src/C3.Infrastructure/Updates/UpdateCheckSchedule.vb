Namespace Updates

    Public NotInheritable Class UpdateCheckSchedule

        Private Sub New()
        End Sub

        Public Shared Function Parse(value As String) As UpdateCheckPolicy
            Dim policy As UpdateCheckPolicy
            If TryParseStored(value, policy) Then
                Return policy
            End If
            Return UpdateCheckPolicy.Never
        End Function

        Public Shared Function TryParseStored(
                value As String,
                ByRef policy As UpdateCheckPolicy) As Boolean

            Select Case If(value, String.Empty).Trim().ToLowerInvariant()
                Case "true", "startup"
                    policy = UpdateCheckPolicy.Startup
                    Return True
                Case "weekly"
                    policy = UpdateCheckPolicy.Weekly
                    Return True
                Case "monthly"
                    policy = UpdateCheckPolicy.Monthly
                    Return True
                Case "false", "manually", "never"
                    policy = UpdateCheckPolicy.Never
                    Return True
                Case Else
                    policy = UpdateCheckPolicy.Never
                    Return False
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

            If policy = UpdateCheckPolicy.Never OrElse
                    Not [Enum].IsDefined(GetType(UpdateCheckPolicy), policy) Then
                Return False
            End If
            Dim normalizedNow As DateTime = NormalizeUtc(now)
            Dim normalizedLastCheck As DateTime = If(
                lastCheckedAt = DateTime.MinValue,
                DateTime.MinValue,
                NormalizeUtc(lastCheckedAt))
            If normalizedLastCheck <> DateTime.MinValue AndAlso
                    (normalizedLastCheck - normalizedNow).TotalMinutes > 5D Then
                Return True
            End If

            Select Case policy
                Case UpdateCheckPolicy.Startup
                    Return True
                Case UpdateCheckPolicy.Weekly
                    Return normalizedLastCheck = DateTime.MinValue OrElse
                        (normalizedNow - normalizedLastCheck).TotalDays >= 7D
                Case UpdateCheckPolicy.Monthly
                    Return normalizedLastCheck = DateTime.MinValue OrElse
                        (normalizedNow - normalizedLastCheck).TotalDays >= 28D
                Case Else
                    Return False
            End Select
        End Function

        Public Shared Function NormalizeUtc(value As DateTime) As DateTime
            If value = DateTime.MinValue OrElse value.Kind = DateTimeKind.Utc Then
                Return value
            End If
            If value.Kind = DateTimeKind.Unspecified Then
                value = DateTime.SpecifyKind(value, DateTimeKind.Local)
            End If
            Return value.ToUniversalTime()
        End Function

    End Class

End Namespace
