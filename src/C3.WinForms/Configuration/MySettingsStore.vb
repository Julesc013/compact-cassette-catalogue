Friend NotInheritable Class MySettingsStore

    Public Property ShowMessages As Boolean
        Get
            Return My.Settings.showMessages
        End Get
        Set(value As Boolean)
            My.Settings.showMessages = value
        End Set
    End Property

    Public Property DefaultDirectory As String
        Get
            Return My.Settings.defaultDirectory
        End Get
        Set(value As String)
            My.Settings.defaultDirectory = value
        End Set
    End Property

    Public Property UpdatePolicy As UpdateCheckPolicy
        Get
            Return UpdateCheckSchedule.Parse(My.Settings.checkUpdates)
        End Get
        Set(value As UpdateCheckPolicy)
            My.Settings.checkUpdates = UpdateCheckSchedule.Serialize(value)
        End Set
    End Property

    Public Property LastUpdateCheck As DateTime
        Get
            Return My.Settings.lastUpdateCheck
        End Get
        Set(value As DateTime)
            My.Settings.lastUpdateCheck = value
        End Set
    End Property

    Public Sub Normalize()
        My.Settings.checkUpdates = UpdateCheckSchedule.Serialize(UpdatePolicy)
        If String.IsNullOrWhiteSpace(DefaultDirectory) OrElse
                DefaultDirectory.StartsWith("My.", StringComparison.Ordinal) Then
            DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        End If
    End Sub

    Public Sub Save()
        My.Settings.Save()
    End Sub

End Class
