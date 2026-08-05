Imports System.IO

Namespace Global.C3Setup

    Public NotInheritable Class SetupRemovalTransaction

        Private Sub New()
        End Sub

        Public Shared Sub Remove(installRoot As String, faultInjector As Action(Of String))
            Remove(installRoot, Nothing, faultInjector)
        End Sub

        Public Shared Sub Remove(installRoot As String,
                                 systemRemoval As Func(Of InstalledState, Action),
                                 faultInjector As Action(Of String))
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
            Dim rollbackSystemRemoval As Action = Nothing
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

                If systemRemoval IsNot Nothing Then
                    rollbackSystemRemoval = systemRemoval(state)
                    If rollbackSystemRemoval Is Nothing Then Throw New SetupContractException("System removal did not provide a rollback operation.")
                    Inject(faultInjector, "after-system-removal")
                End If

                Directory.Delete(removalRoot, True)
                If Directory.Exists(canonicalRoot) AndAlso New DirectoryInfo(canonicalRoot).GetFileSystemInfos().Length = 0 Then
                    Directory.Delete(canonicalRoot, False)
                End If
            Catch failure As Exception
                Dim rollbackFailures As New List(Of Exception)()
                For Each original As String In moved.Keys
                    Dim originalPath As String = original
                    Dim stagedPath As String = moved(original)
                    TryRollback(Sub()
                                    If File.Exists(stagedPath) Then File.Move(stagedPath, originalPath)
                                End Sub,
                                rollbackFailures)
                Next
                If rollbackSystemRemoval IsNot Nothing Then TryRollback(rollbackSystemRemoval, rollbackFailures)
                TryRollback(Sub()
                                If Directory.Exists(removalRoot) Then Directory.Delete(removalRoot, True)
                            End Sub,
                            rollbackFailures)
                If rollbackFailures.Count <> 0 Then
                    Dim allFailures As New List(Of Exception)()
                    allFailures.Add(failure)
                    allFailures.AddRange(rollbackFailures)
                    Throw New AggregateException("Removal failed and one or more rollback actions also failed.", allFailures)
                End If
                Throw
            End Try
        End Sub

        Private Shared Sub Inject(faultInjector As Action(Of String), point As String)
            If faultInjector IsNot Nothing Then faultInjector(point)
        End Sub

        Private Shared Sub TryRollback(action As Action, failures As IList(Of Exception))
            Try
                action()
            Catch ex As Exception
                failures.Add(ex)
            End Try
        End Sub

    End Class

End Namespace
