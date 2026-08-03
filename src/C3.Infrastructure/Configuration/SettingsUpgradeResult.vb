Namespace Configuration

Public Enum SettingsUpgradeStatus
    Current
    Upgraded
    Failed
End Enum

Public NotInheritable Class SettingsUpgradeResult

    Private Sub New(status As SettingsUpgradeStatus, failure As Exception)
        Me.Status = status
        Me.Failure = failure
    End Sub

    Public ReadOnly Property Status As SettingsUpgradeStatus

    Public ReadOnly Property Failure As Exception

    Public ReadOnly Property IsSuccess As Boolean
        Get
            Return Status <> SettingsUpgradeStatus.Failed
        End Get
    End Property

    Friend Shared Function Current() As SettingsUpgradeResult
        Return New SettingsUpgradeResult(SettingsUpgradeStatus.Current, Nothing)
    End Function

    Friend Shared Function Upgraded() As SettingsUpgradeResult
        Return New SettingsUpgradeResult(SettingsUpgradeStatus.Upgraded, Nothing)
    End Function

    Friend Shared Function Failed(failure As Exception) As SettingsUpgradeResult
        Return New SettingsUpgradeResult(SettingsUpgradeStatus.Failed, failure)
    End Function

End Class

End Namespace
