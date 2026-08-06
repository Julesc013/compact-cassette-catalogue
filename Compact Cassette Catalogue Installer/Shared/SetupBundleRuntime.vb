Imports System.IO

Namespace Global.C3Setup

    Public NotInheritable Class SetupBundleContext

        Public Sub New(manifestPath As String,
                       payloadDirectory As String,
                       manifest As PayloadManifest,
                       setupExecutableSha256 As String,
                       payloadBytes As Long)
            Me.ManifestPath = manifestPath
            Me.PayloadDirectory = payloadDirectory
            Me.Manifest = manifest
            Me.SetupExecutableSha256 = setupExecutableSha256
            Me.PayloadBytes = payloadBytes
        End Sub

        Public ReadOnly Property ManifestPath As String
        Public ReadOnly Property PayloadDirectory As String
        Public ReadOnly Property Manifest As PayloadManifest
        Public ReadOnly Property SetupExecutableSha256 As String
        Public ReadOnly Property PayloadBytes As Long
    End Class

    Public NotInheritable Class SetupBundleRuntime

        Public Const ManifestFileName As String = "payload.xml"
        Public Const PayloadDirectoryName As String = "payload"

        Private Sub New()
        End Sub

        Public Shared Function Load(bundleDirectory As String, setupExecutablePath As String) As SetupBundleContext
            Dim root As String = SetupPathPolicy.CanonicalDirectory(bundleDirectory)
            Dim executablePath As String = Path.GetFullPath(setupExecutablePath)
            Dim prefix As String = root.TrimEnd(Path.DirectorySeparatorChar) & Path.DirectorySeparatorChar
            If Not executablePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) OrElse
                    Not File.Exists(executablePath) OrElse
                    (New FileInfo(executablePath).Attributes And FileAttributes.ReparsePoint) <> 0 Then
                Throw New SetupContractException("The executing setup program must be an ordinary file inside its bundle root.")
            End If
            Dim manifestPath As String = Path.Combine(root, ManifestFileName)
            Dim payloadDirectory As String = Path.Combine(root, PayloadDirectoryName)
            If Not File.Exists(manifestPath) Then Throw New SetupContractException("The adjacent setup payload manifest is missing.")
            Dim manifest As PayloadManifest = PayloadManifestReader.Read(manifestPath)
            RequireCurrentRelease(manifest)
            PayloadVerifier.Verify(manifest, payloadDirectory)
            Dim bytes As Long = 0
            For Each item As PayloadFile In manifest.Files
                If bytes > Long.MaxValue - item.Length Then Throw New SetupContractException("The setup payload size overflowed.")
                bytes += item.Length
            Next
            Return New SetupBundleContext(manifestPath,
                                          payloadDirectory,
                                          manifest,
                                          FileHash.Sha256(executablePath),
                                          bytes)
        End Function

        Public Shared Sub RequireCurrentRelease(manifest As PayloadManifest)
            If manifest Is Nothing Then Throw New ArgumentNullException("manifest")
            If manifest.Version <> "1.3.0" OrElse manifest.Stage <> "Alpha 5" OrElse manifest.Label <> "1.3.0a5" Then
                Throw New SetupContractException("This setup executable accepts only the C3 1.3.0 Alpha 5 payload identity.")
            End If
        End Sub

    End Class

End Namespace
