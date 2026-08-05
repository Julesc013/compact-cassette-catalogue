Imports System.IO

Namespace Global.C3Setup

    Public NotInheritable Class SetupRemovalTransaction

        Private Sub New()
        End Sub

        Public Shared Sub Remove(installRoot As String, faultInjector As Action(Of String))
            Dim canonicalRoot As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            Dim statePath As String = Path.Combine(canonicalRoot, InstalledStateCodec.FileName)
            If Not File.Exists(statePath) Then
                Throw New SetupContractException("No C3 installed-state manifest exists at the selected root.")
            End If
            Dim state As InstalledState = InstalledStateCodec.Read(statePath)
            If Not String.Equals(state.InstallRoot, canonicalRoot, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("Installed-state root does not match the selected removal root.")
            End If

            PayloadVerifier.VerifyOwnedFiles(state.Manifest, canonicalRoot)
            Dim parent As String = Directory.GetParent(canonicalRoot).FullName
            Dim transactionId As String = Guid.NewGuid().ToString("N").ToLowerInvariant()
            Dim removalRoot As String = Path.Combine(parent, "." & New DirectoryInfo(canonicalRoot).Name & ".c3remove-" & transactionId)
            If Directory.Exists(removalRoot) Then Throw New SetupContractException("Removal staging path already exists.")

            Dim moved As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Try
                Directory.CreateDirectory(removalRoot)
                Dim count As Integer = 0
                For Each item As PayloadFile In state.Manifest.Files
                    Dim original As String = SetupPathPolicy.CombineOwnedFile(canonicalRoot, item.Path)
                    Dim staged As String = SetupPathPolicy.CombineOwnedFile(removalRoot, item.Path)
                    File.Move(original, staged)
                    moved.Add(original, staged)
                    count += 1
                    If count = 1 Then Inject(faultInjector, "after-first-file")
                Next
                Dim stagedState As String = Path.Combine(removalRoot, InstalledStateCodec.FileName)
                File.Move(statePath, stagedState)
                moved.Add(statePath, stagedState)
                Inject(faultInjector, "after-state")

                Directory.Delete(removalRoot, True)
                If Directory.Exists(canonicalRoot) AndAlso New DirectoryInfo(canonicalRoot).GetFileSystemInfos().Length = 0 Then
                    Directory.Delete(canonicalRoot, False)
                End If
            Catch
                For Each original As String In moved.Keys
                    Dim staged As String = moved(original)
                    If File.Exists(staged) Then File.Move(staged, original)
                Next
                If Directory.Exists(removalRoot) Then Directory.Delete(removalRoot, True)
                Throw
            End Try
        End Sub

        Private Shared Sub Inject(faultInjector As Action(Of String), point As String)
            If faultInjector IsNot Nothing Then faultInjector(point)
        End Sub

    End Class

End Namespace
