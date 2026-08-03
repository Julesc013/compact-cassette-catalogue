Friend NotInheritable Class UpdateCheckScheduleTryParseStoredTests

    Private Sub New()
    End Sub

    Public Shared Sub AcceptsCanonicalAndLegacyValues()
        AssertAccepted("startup", UpdateCheckPolicy.Startup, "canonical startup")
        AssertAccepted(" STARTUP ", UpdateCheckPolicy.Startup, "normalized startup")
        AssertAccepted("true", UpdateCheckPolicy.Startup, "legacy true")
        AssertAccepted("weekly", UpdateCheckPolicy.Weekly, "canonical weekly")
        AssertAccepted(" MONTHLY ", UpdateCheckPolicy.Monthly, "normalized monthly")
        AssertAccepted("never", UpdateCheckPolicy.Never, "canonical never")
        AssertAccepted("false", UpdateCheckPolicy.Never, "legacy false")
        AssertAccepted("manually", UpdateCheckPolicy.Never, "legacy manually")
    End Sub

    Public Shared Sub RejectsUnknownValues()
        AssertRejected(Nothing, "null policy")
        AssertRejected(String.Empty, "empty policy")
        AssertRejected("   ", "whitespace policy")
        AssertRejected("sometimes", "unknown policy")
        AssertRejected("1", "numeric true policy")
        AssertRejected("0", "numeric false policy")
    End Sub

    Public Shared Sub FutureTimestampsDoNotSuppressScheduledChecks()
        Dim now As New DateTime(2026, 8, 4, 12, 0, 0, DateTimeKind.Utc)
        AssertEqual(
            False,
            UpdateCheckSchedule.ShouldCheck(
                UpdateCheckPolicy.Weekly,
                now.AddMinutes(5),
                now),
            "clock tolerance boundary")
        AssertEqual(
            True,
            UpdateCheckSchedule.ShouldCheck(
                UpdateCheckPolicy.Weekly,
                now.AddMinutes(6),
                now),
            "weekly future timestamp")
        AssertEqual(
            True,
            UpdateCheckSchedule.ShouldCheck(
                UpdateCheckPolicy.Monthly,
                now.AddDays(1),
                now),
            "monthly future timestamp")
        AssertEqual(
            False,
            UpdateCheckSchedule.ShouldCheck(
                UpdateCheckPolicy.Never,
                now.AddMinutes(6),
                now),
            "never future timestamp")
        AssertEqual(
            False,
            UpdateCheckSchedule.ShouldCheck(
                CType(99, UpdateCheckPolicy),
                now.AddMinutes(6),
                now),
            "undefined policy future timestamp")
    End Sub

    Private Shared Sub AssertAccepted(
            storedValue As String,
            expected As UpdateCheckPolicy,
            name As String)

        Dim actual As UpdateCheckPolicy = UpdateCheckPolicy.Never
        Dim accepted As Boolean = UpdateCheckSchedule.TryParseStored(storedValue, actual)
        If Not accepted Then
            Throw New InvalidOperationException(name & " was rejected.")
        End If
        AssertEqual(expected, actual, name)
    End Sub

    Private Shared Sub AssertRejected(storedValue As String, name As String)
        Dim actual As UpdateCheckPolicy = UpdateCheckPolicy.Monthly
        Dim accepted As Boolean = UpdateCheckSchedule.TryParseStored(storedValue, actual)
        AssertEqual(False, accepted, name & " result")
        AssertEqual(UpdateCheckPolicy.Never, actual, name & " fallback")
    End Sub

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Class
