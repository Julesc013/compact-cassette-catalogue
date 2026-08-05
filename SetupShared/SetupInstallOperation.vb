Namespace Global.C3Setup

    Public NotInheritable Class SetupInstallOperation

        Private Sub New()
        End Sub

        Public Shared Function Execute(manifestPath As String,
                                       payloadDirectory As String,
                                       installRoot As String,
                                       setupSourceCommit As String,
                                       setupExecutableSha256 As String,
                                       includeDesktopShortcut As Boolean,
                                       facts As SetupEnvironmentFacts,
                                       shortcutAccess As ISetupShortcutAccess,
                                       registryAccess As ISetupRegistryAccess,
                                       faultInjector As Action(Of String)) As InstalledState
            If shortcutAccess Is Nothing Then Throw New ArgumentNullException("shortcutAccess")
            If registryAccess Is Nothing Then Throw New ArgumentNullException("registryAccess")
            Dim manifest As PayloadManifest = PayloadManifestReader.Read(manifestPath)
            SetupBundleRuntime.RequireCurrentRelease(manifest)
            PayloadVerifier.Verify(manifest, payloadDirectory)
            SetupEnvironment.Validate(manifest, facts, PayloadBytes(manifest))
            Dim canonicalRoot As String = SetupEnvironment.ValidateInstallRoot(facts, installRoot)
            Dim shortcuts As IList(Of InstalledShortcut) = SetupShortcutService.Plan(canonicalRoot,
                                                                                     shortcutAccess.CommonProgramsPath,
                                                                                     shortcutAccess.CommonDesktopPath,
                                                                                     includeDesktopShortcut)

            Return SetupDurableTransaction.Install(manifestPath,
                                                   payloadDirectory,
                                                   canonicalRoot,
                                                   setupSourceCommit,
                                                   setupExecutableSha256,
                                                   shortcuts,
                                                   shortcutAccess,
                                                   registryAccess,
                                                   faultInjector)
        End Function

        Private Shared Function PayloadBytes(manifest As PayloadManifest) As Long
            Dim total As Long = 0
            For Each item As PayloadFile In manifest.Files
                If total > Long.MaxValue - item.Length Then Throw New SetupContractException("Payload size overflowed.")
                total += item.Length
            Next
            Return total
        End Function

    End Class

End Namespace
