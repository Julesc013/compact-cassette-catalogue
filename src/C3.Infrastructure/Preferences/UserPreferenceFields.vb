Namespace Preferences

<Flags>
Public Enum UserPreferenceFields
    None = 0
    ShowMessages = 1
    DefaultDirectory = 2
    UpdatePolicy = 4
    LastUpdateCheck = 8
    All = ShowMessages Or DefaultDirectory Or UpdatePolicy Or LastUpdateCheck
End Enum

End Namespace
