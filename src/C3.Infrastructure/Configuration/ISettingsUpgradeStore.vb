Namespace Configuration

Public Interface ISettingsUpgradeStore

    Property UpgradeRequired As Boolean

    Sub UpgradeFromPreviousVersion()

    Sub Normalize()

    Sub Save()

End Interface

End Namespace
