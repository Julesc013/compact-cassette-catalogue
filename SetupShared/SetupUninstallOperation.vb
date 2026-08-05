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
            SetupDurableTransaction.Uninstall(canonicalRoot, shortcutAccess, registryAccess, faultInjector)
        End Sub

    End Class

End Namespace
