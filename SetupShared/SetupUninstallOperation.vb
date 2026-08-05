Imports System.IO

Namespace Global.C3Setup

    Public NotInheritable Class SetupUninstallOperation

        Private Sub New()
        End Sub

        Public Shared Sub Execute(installRoot As String,
                                  shortcutAccess As ISetupShortcutAccess,
                                  registryAccess As ISetupRegistryAccess,
                                  faultInjector As Action(Of String))
            If shortcutAccess Is Nothing Then Throw New ArgumentNullException("shortcutAccess")
            If registryAccess Is Nothing Then Throw New ArgumentNullException("registryAccess")
            Dim canonicalRoot As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            Dim statePath As String = Path.Combine(canonicalRoot, InstalledStateCodec.FileName)
            If Not File.Exists(statePath) Then Throw New SetupContractException("No C3 installed-state manifest exists at the selected root.")
            Dim discovered As InstalledState = InstalledStateCodec.Read(statePath)
            SetupBundleRuntime.RequireCurrentRelease(discovered.Manifest)
            If Not String.Equals(discovered.InstallRoot, canonicalRoot, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("Installed-state root does not match the selected removal root.")
            End If

            Dim removeSystem As Func(Of InstalledState, Action) =
                Function(state As InstalledState) As Action
                    Dim shortcutTransition As SetupShortcutTransition = Nothing
                    Dim registrySnapshot As IDictionary(Of String, Object) = Nothing
                    Try
                        shortcutTransition = SetupShortcutService.RemoveTransition(state, shortcutAccess)
                        registrySnapshot = SetupRegistryRegistration.RemoveWithSnapshot(state, registryAccess)
                        Return Sub()
                                   SetupRegistryRegistration.RestoreRemoved(state, registrySnapshot, registryAccess)
                                   SetupShortcutService.RestoreTransition(shortcutTransition, shortcutAccess)
                               End Sub
                    Catch
                        If registrySnapshot IsNot Nothing Then
                            SetupRegistryRegistration.RestoreRemoved(state, registrySnapshot, registryAccess)
                        End If
                        If shortcutTransition IsNot Nothing Then
                            SetupShortcutService.RestoreTransition(shortcutTransition, shortcutAccess)
                        End If
                        Throw
                    End Try
                End Function

            SetupRemovalTransaction.Remove(canonicalRoot, removeSystem, faultInjector)
        End Sub

    End Class

End Namespace
