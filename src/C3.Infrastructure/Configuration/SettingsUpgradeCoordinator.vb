Namespace Configuration

Public NotInheritable Class SettingsUpgradeCoordinator

    Private Sub New()
    End Sub

    Public Shared Function Prepare(store As ISettingsUpgradeStore) As SettingsUpgradeResult
        If store Is Nothing Then
            Throw New ArgumentNullException("store")
        End If

        Dim upgradeRequired As Boolean
        Try
            upgradeRequired = store.UpgradeRequired
            If upgradeRequired Then
                store.UpgradeFromPreviousVersion()
            End If

            store.Normalize()

            If upgradeRequired Then
                ' Persist imported values while the retry marker is still armed.
                ' If the process stops here, the next launch safely repeats the
                ' idempotent framework upgrade instead of losing the migration.
                store.Save()
                store.UpgradeRequired = False
            End If

            store.Save()
            If upgradeRequired Then
                Return SettingsUpgradeResult.Upgraded()
            End If
            Return SettingsUpgradeResult.Current()
        Catch ex As Exception
            If upgradeRequired Then
                Try
                    store.UpgradeRequired = True
                Catch
                    ' Preserve the original failure. A diagnostic record will
                    ' explain that migration could not be completed.
                End Try
            End If
            Return SettingsUpgradeResult.Failed(ex)
        End Try
    End Function

End Class

End Namespace
