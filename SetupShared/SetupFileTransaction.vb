Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions

Namespace Global.C3Setup

    Public NotInheritable Class SetupFileTransaction

        Private Sub New()
        End Sub

        Public Shared Function Apply(manifestPath As String,
                                     payloadDirectory As String,
                                     installRoot As String,
                                     setupSourceCommit As String,
                                     setupBundleSha256 As String,
                                     faultInjector As Action(Of String)) As InstalledState
            Return Apply(manifestPath,
                         payloadDirectory,
                         installRoot,
                         setupSourceCommit,
                         setupBundleSha256,
                         New List(Of InstalledShortcut)(),
                         Nothing,
                         faultInjector)
        End Function

        Public Shared Function Apply(manifestPath As String,
                                     payloadDirectory As String,
                                     installRoot As String,
                                     setupSourceCommit As String,
                                     setupBundleSha256 As String,
                                     shortcuts As IList(Of InstalledShortcut),
                                     systemIntegration As Func(Of InstalledState, InstalledState, Action),
                                     faultInjector As Action(Of String)) As InstalledState
            If Not Regex.IsMatch(setupSourceCommit, "^[0-9a-f]{40}$", RegexOptions.CultureInvariant) Then
                Throw New SetupContractException("Setup source commit is invalid.")
            End If
            If Not Regex.IsMatch(setupBundleSha256, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant) Then
                Throw New SetupContractException("Setup bundle SHA-256 is invalid.")
            End If

            Dim manifest As PayloadManifest = PayloadManifestReader.Read(manifestPath)
            PayloadVerifier.Verify(manifest, payloadDirectory)
            Dim canonicalRoot As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            Dim parent As String = Directory.GetParent(canonicalRoot).FullName
            Dim transactionId As String = Guid.NewGuid().ToString("N").ToLowerInvariant()
            Dim leaf As String = New DirectoryInfo(canonicalRoot).Name
            Dim stagingRoot As String = Path.Combine(parent, "." & leaf & ".c3stage-" & transactionId)
            Dim backupRoot As String = Path.Combine(parent, "." & leaf & ".c3backup-" & transactionId)
            If Directory.Exists(stagingRoot) OrElse Directory.Exists(backupRoot) Then
                Throw New SetupContractException("Transaction staging paths already exist.")
            End If

            Dim rootExisted As Boolean = Directory.Exists(canonicalRoot)
            Dim previousStatePath As String = Path.Combine(canonicalRoot, InstalledStateCodec.FileName)
            Dim previousState As InstalledState = Nothing
            If rootExisted AndAlso File.Exists(previousStatePath) Then
                previousState = InstalledStateCodec.Read(previousStatePath)
                If Not String.Equals(previousState.InstallRoot, canonicalRoot, StringComparison.OrdinalIgnoreCase) Then
                    Throw New SetupContractException("Installed-state root does not match the selected install root.")
                End If
                If previousState.Manifest.Lane <> manifest.Lane OrElse
                        previousState.Manifest.Architecture <> manifest.Architecture OrElse
                        previousState.Manifest.Framework <> manifest.Framework Then
                    Throw New SetupContractException("An installed lane cannot be changed in place.")
                End If
                If CompareReleaseIdentity(previousState.Manifest, manifest) > 0 Then
                    Throw New SetupContractException("Downgrading an installed C3 release is prohibited.")
                End If
            ElseIf rootExisted AndAlso New DirectoryInfo(canonicalRoot).GetFileSystemInfos().Length <> 0 Then
                Throw New SetupContractException("A non-empty directory without valid C3 installed state cannot be adopted.")
            End If

            Dim mode As String = "install"
            If previousState IsNot Nothing Then
                mode = If(previousState.Manifest.Label = manifest.Label, "repair", "upgrade")
            End If

            Dim previousOwned As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            If previousState IsNot Nothing Then
                For Each item As PayloadFile In previousState.Manifest.Files
                    previousOwned.Add(item.Path, True)
                Next
            End If
            For Each item As PayloadFile In manifest.Files
                Dim destination As String = SetupPathPolicy.CombineOwnedFile(canonicalRoot, item.Path)
                If File.Exists(destination) AndAlso Not previousOwned.ContainsKey(item.Path) Then
                    Throw New SetupContractException("Setup refuses to overwrite an unowned file: " & item.Path)
                End If
            Next

            Dim installedPaths As New List(Of String)()
            Dim backedUpPaths As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Dim rollbackSystemIntegration As Action = Nothing
            Try
                Directory.CreateDirectory(stagingRoot)
                For Each item As PayloadFile In manifest.Files
                    File.Copy(SetupPathPolicy.CombineOwnedFile(payloadDirectory, item.Path),
                              SetupPathPolicy.CombineOwnedFile(stagingRoot, item.Path),
                              False)
                Next
                PayloadVerifier.Verify(manifest, stagingRoot)
                Inject(faultInjector, "after-staging")

                If Not Directory.Exists(canonicalRoot) Then Directory.CreateDirectory(canonicalRoot)
                Directory.CreateDirectory(backupRoot)
                If File.Exists(previousStatePath) Then
                    Dim backupState As String = Path.Combine(backupRoot, InstalledStateCodec.FileName)
                    File.Move(previousStatePath, backupState)
                    backedUpPaths.Add(previousStatePath, backupState)
                End If
                If previousState IsNot Nothing Then
                    For Each item As PayloadFile In previousState.Manifest.Files
                        Dim existing As String = SetupPathPolicy.CombineOwnedFile(canonicalRoot, item.Path)
                        If File.Exists(existing) Then
                            Dim backup As String = SetupPathPolicy.CombineOwnedFile(backupRoot, item.Path)
                            File.Move(existing, backup)
                            backedUpPaths.Add(existing, backup)
                        End If
                    Next
                End If
                Inject(faultInjector, "after-backup")

                Dim installedCount As Integer = 0
                For Each item As PayloadFile In manifest.Files
                    Dim staged As String = SetupPathPolicy.CombineOwnedFile(stagingRoot, item.Path)
                    Dim destination As String = SetupPathPolicy.CombineOwnedFile(canonicalRoot, item.Path)
                    File.Move(staged, destination)
                    installedPaths.Add(destination)
                    installedCount += 1
                    If installedCount = 1 Then Inject(faultInjector, "after-first-file")
                Next

                Dim state As New InstalledState(manifest,
                                                setupSourceCommit,
                                                canonicalRoot,
                                                mode,
                                                transactionId,
                                                DateTime.UtcNow,
                                                FileHash.Sha256(manifestPath),
                                                setupBundleSha256,
                                                shortcuts)
                Dim stagedState As String = Path.Combine(stagingRoot, InstalledStateCodec.FileName)
                InstalledStateCodec.Write(stagedState, state)
                Inject(faultInjector, "before-manifest")
                File.Move(stagedState, previousStatePath)
                PayloadVerifier.VerifyOwnedFiles(manifest, canonicalRoot)
                InstalledStateCodec.Read(previousStatePath)
                Inject(faultInjector, "after-manifest")

                If systemIntegration IsNot Nothing Then
                    rollbackSystemIntegration = systemIntegration(previousState, state)
                    If rollbackSystemIntegration Is Nothing Then
                        Throw New SetupContractException("System integration did not provide a rollback operation.")
                    End If
                    Inject(faultInjector, "after-system-integration")
                End If

                Directory.Delete(stagingRoot, False)
                Directory.Delete(backupRoot, True)
                Return state
            Catch failure As Exception
                Dim rollbackFailures As New List(Of Exception)()
                If rollbackSystemIntegration IsNot Nothing Then TryRollback(rollbackSystemIntegration, rollbackFailures)
                For Each installedPath As String In installedPaths
                    Dim pathToDelete As String = installedPath
                    TryRollback(Sub()
                                    If File.Exists(pathToDelete) Then File.Delete(pathToDelete)
                                End Sub,
                                rollbackFailures)
                Next
                TryRollback(Sub()
                                If File.Exists(previousStatePath) Then File.Delete(previousStatePath)
                            End Sub,
                            rollbackFailures)
                For Each original As String In backedUpPaths.Keys
                    Dim originalPath As String = original
                    Dim backupPath As String = backedUpPaths(original)
                    TryRollback(Sub()
                                    If File.Exists(backupPath) Then File.Move(backupPath, originalPath)
                                End Sub,
                                rollbackFailures)
                Next
                TryRollback(Sub()
                                If Directory.Exists(stagingRoot) Then Directory.Delete(stagingRoot, True)
                            End Sub,
                            rollbackFailures)
                TryRollback(Sub()
                                If Directory.Exists(backupRoot) Then Directory.Delete(backupRoot, True)
                            End Sub,
                            rollbackFailures)
                TryRollback(Sub()
                                If Not rootExisted AndAlso Directory.Exists(canonicalRoot) AndAlso
                                        New DirectoryInfo(canonicalRoot).GetFileSystemInfos().Length = 0 Then
                                    Directory.Delete(canonicalRoot, False)
                                End If
                            End Sub,
                            rollbackFailures)
                If rollbackFailures.Count <> 0 Then
                    Dim allFailures As New List(Of Exception)()
                    allFailures.Add(failure)
                    allFailures.AddRange(rollbackFailures)
                    Throw New AggregateException("Setup failed and one or more rollback actions also failed.", allFailures)
                End If
                Throw
            End Try
        End Function

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

        Private Shared Function CompareReleaseIdentity(left As PayloadManifest, right As PayloadManifest) As Integer
            Dim leftVersion As Version = Nothing
            Dim rightVersion As Version = Nothing
            If Not Version.TryParse(left.Version, leftVersion) OrElse Not Version.TryParse(right.Version, rightVersion) Then
                Throw New SetupContractException("Release version is invalid.")
            End If
            Dim versionResult As Integer = leftVersion.CompareTo(rightVersion)
            If versionResult <> 0 Then Return versionResult
            Dim leftStage As Long = StageValue(left.Stage)
            Dim rightStage As Long = StageValue(right.Stage)
            Return leftStage.CompareTo(rightStage)
        End Function

        Private Shared Function StageValue(stage As String) As Long
            Dim alpha As Match = Regex.Match(stage, "^Alpha (?<n>[1-9][0-9]*)$", RegexOptions.CultureInvariant)
            If alpha.Success Then Return Long.Parse(alpha.Groups("n").Value, CultureInfo.InvariantCulture)
            Dim beta As Match = Regex.Match(stage, "^Beta (?<n>[1-9][0-9]*)$", RegexOptions.CultureInvariant)
            If beta.Success Then Return 1000000L + Long.Parse(beta.Groups("n").Value, CultureInfo.InvariantCulture)
            If stage = "Release" Then Return 2000000L
            Throw New SetupContractException("Release stage is invalid.")
        End Function

    End Class

End Namespace
