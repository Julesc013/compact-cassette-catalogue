Friend NotInheritable Class LegacyPathTestSupport

    Public Const ClassicMaximumPathCharacters As Integer = 259
    Private Const TestDestinationCharacters As Integer = 250

    Private Sub New()
    End Sub

    Public Shared Function CreateNearBoundaryDestination(
            workDirectory As String,
            fileName As String) As String

        Dim paddingCharacters As Integer =
            TestDestinationCharacters - workDirectory.Length - fileName.Length - 2
        If paddingCharacters < 1 OrElse paddingCharacters > 200 Then
            Throw New InvalidOperationException(
                "The test root cannot construct the intended legacy-path fixture.")
        End If

        Dim destinationDirectory As String = Path.Combine(
            workDirectory,
            New String("p"c, paddingCharacters))
        Directory.CreateDirectory(destinationDirectory)

        Dim destination As String = Path.Combine(destinationDirectory, fileName)
        If destination.Length <> TestDestinationCharacters Then
            Throw New InvalidOperationException(
                "The legacy-path fixture has an unexpected destination length.")
        End If
        Return destination
    End Function

    Public Shared Function HistoricalTemporaryPath(destination As String) As String
        Return Path.Combine(
            Path.GetDirectoryName(destination),
            "." & Path.GetFileName(destination) & "." & New String("0"c, 32) & ".tmp")
    End Function

    Public Shared Function HistoricalPreferencesRecoveryPath(
            destination As String,
            stamp As DateTime) As String

        Return Path.Combine(
            Path.GetDirectoryName(destination),
            Path.GetFileNameWithoutExtension(destination) &
                ".corrupt-" &
                stamp.ToUniversalTime().ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture) &
                "-" &
                New String("0"c, 32) &
                ".xml")
    End Function

End Class
