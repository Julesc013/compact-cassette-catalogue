Imports C3.Infrastructure.Updates

Namespace Preferences

Public NotInheritable Class UserPreferencesSnapshot

    Public Const MaximumDefaultDirectoryCharacters As Integer = 32768
    Public Const CurrentLegacyImportVersion As Integer = 1
    Public Const ImportOutcomePending As String = "pending"
    Public Const ImportOutcomeImported As String = "imported"
    Public Const ImportOutcomeNotFound As String = "not-found"
    Public Const ImportOutcomeInvalid As String = "invalid"

    Public Property ShowMessages As Boolean

    Public Property DefaultDirectory As String

    Public Property UpdatePolicy As UpdateCheckPolicy

    Public Property LastUpdateCheck As DateTime

    Public Property Legacy1xImportVersion As Integer

    Public Property Legacy1xImportOutcome As String

    Public Shared Function CreateDefaults(myDocumentsPath As String) As UserPreferencesSnapshot
        Return New UserPreferencesSnapshot() With {
            .ShowMessages = True,
            .DefaultDirectory = If(myDocumentsPath, String.Empty),
            .UpdatePolicy = UpdateCheckPolicy.Never,
            .LastUpdateCheck = DateTime.MinValue,
            .Legacy1xImportVersion = 0,
            .Legacy1xImportOutcome = ImportOutcomePending
        }
    End Function

    Public Function Clone() As UserPreferencesSnapshot
        Return New UserPreferencesSnapshot() With {
            .ShowMessages = ShowMessages,
            .DefaultDirectory = DefaultDirectory,
            .UpdatePolicy = UpdatePolicy,
            .LastUpdateCheck = LastUpdateCheck,
            .Legacy1xImportVersion = Legacy1xImportVersion,
            .Legacy1xImportOutcome = Legacy1xImportOutcome
        }
    End Function

End Class

End Namespace
