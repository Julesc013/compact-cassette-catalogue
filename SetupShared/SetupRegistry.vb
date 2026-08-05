Imports System.Globalization
Imports System.IO
Imports Microsoft.Win32

Namespace Global.C3Setup

    Public Interface ISetupRegistryAccess
        Function ReadValues(keyPath As String) As IDictionary(Of String, Object)
        Sub WriteValues(keyPath As String, values As IDictionary(Of String, Object))
        Sub DeleteKey(keyPath As String)
    End Interface

    Public NotInheritable Class WindowsSetupRegistryAccess
        Implements ISetupRegistryAccess

        Public Function ReadValues(keyPath As String) As IDictionary(Of String, Object) Implements ISetupRegistryAccess.ReadValues
            Using baseKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)
                Using key As RegistryKey = baseKey.OpenSubKey(keyPath, False)
                    If key Is Nothing Then Return Nothing
                    Dim values As New Dictionary(Of String, Object)(StringComparer.Ordinal)
                    For Each name As String In key.GetValueNames()
                        values.Add(name, key.GetValue(name, Nothing, RegistryValueOptions.DoNotExpandEnvironmentNames))
                    Next
                    Return values
                End Using
            End Using
        End Function

        Public Sub WriteValues(keyPath As String, values As IDictionary(Of String, Object)) Implements ISetupRegistryAccess.WriteValues
            Using baseKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)
                Using key As RegistryKey = baseKey.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree)
                    If key Is Nothing Then Throw New SetupContractException("Windows refused to create the C3 uninstall registration.")
                    For Each oldName As String In key.GetValueNames()
                        key.DeleteValue(oldName, False)
                    Next
                    For Each pair As KeyValuePair(Of String, Object) In values
                        Dim kind As RegistryValueKind = If(TypeOf pair.Value Is Integer, RegistryValueKind.DWord, RegistryValueKind.String)
                        key.SetValue(pair.Key, pair.Value, kind)
                    Next
                End Using
            End Using
        End Sub

        Public Sub DeleteKey(keyPath As String) Implements ISetupRegistryAccess.DeleteKey
            Using baseKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)
                baseKey.DeleteSubKeyTree(keyPath, False)
            End Using
        End Sub
    End Class

    Public NotInheritable Class SetupRegistryRegistration

        Private Const ProductName As String = "Compact Cassette Catalogue"
        Private Const Publisher As String = "Jules Carboni"
        Private Const InformationUrl As String = "https://github.com/Julesc013/compact-cassette-catalogue"
        Private Const UpdateUrl As String = "https://github.com/Julesc013/compact-cassette-catalogue/releases"

        Private Sub New()
        End Sub

        Public Shared Function Apply(state As InstalledState,
                                     access As ISetupRegistryAccess) As IDictionary(Of String, Object)
            RequireArguments(state, access)
            Dim keyPath As String = InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)
            Dim previous As IDictionary(Of String, Object) = access.ReadValues(keyPath)
            If previous IsNot Nothing AndAlso Not HasOwnershipMarker(previous, state) Then
                Throw New SetupContractException("Setup refuses to replace an uninstall key it does not own.")
            End If
            Dim expected As IDictionary(Of String, Object) = ExpectedValues(state)
            Try
                access.WriteValues(keyPath, expected)
                RequireExactValues(access.ReadValues(keyPath), expected)
                Return CloneValues(previous)
            Catch
                If previous Is Nothing Then
                    access.DeleteKey(keyPath)
                Else
                    access.WriteValues(keyPath, previous)
                End If
                Throw
            End Try
        End Function

        Public Shared Sub Restore(state As InstalledState,
                                  previous As IDictionary(Of String, Object),
                                  access As ISetupRegistryAccess)
            RequireArguments(state, access)
            Dim keyPath As String = InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)
            Dim current As IDictionary(Of String, Object) = access.ReadValues(keyPath)
            If current Is Nothing OrElse Not HasOwnershipMarker(current, state) Then
                Throw New SetupContractException("Rollback refuses to replace an uninstall key that lost its C3 ownership marker.")
            End If
            If previous Is Nothing Then
                access.DeleteKey(keyPath)
                If access.ReadValues(keyPath) IsNot Nothing Then Throw New SetupContractException("Rollback did not remove the new uninstall registration.")
            Else
                access.WriteValues(keyPath, previous)
                RequireExactValues(access.ReadValues(keyPath), previous)
            End If
        End Sub

        Public Shared Sub Remove(state As InstalledState, access As ISetupRegistryAccess)
            RemoveWithSnapshot(state, access)
        End Sub

        Public Shared Function RemoveWithSnapshot(state As InstalledState,
                                                  access As ISetupRegistryAccess) As IDictionary(Of String, Object)
            RequireArguments(state, access)
            Dim keyPath As String = InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)
            Dim previous As IDictionary(Of String, Object) = access.ReadValues(keyPath)
            RequireExactValues(previous, ExpectedValues(state))
            access.DeleteKey(keyPath)
            If access.ReadValues(keyPath) IsNot Nothing Then Throw New SetupContractException("The owned uninstall registration remains after removal.")
            Return CloneValues(previous)
        End Function

        Public Shared Sub RestoreRemoved(state As InstalledState,
                                         previous As IDictionary(Of String, Object),
                                         access As ISetupRegistryAccess)
            RequireArguments(state, access)
            If previous Is Nothing Then Throw New SetupContractException("Removed registry rollback state is missing.")
            Dim keyPath As String = InstalledStateCodec.UninstallKeyForLane(state.Manifest.Lane)
            If access.ReadValues(keyPath) IsNot Nothing Then Throw New SetupContractException("Registry rollback refuses to overwrite a new uninstall key.")
            access.WriteValues(keyPath, previous)
            RequireExactValues(access.ReadValues(keyPath), previous)
        End Sub

        Public Shared Function ExpectedValues(state As InstalledState) As IDictionary(Of String, Object)
            If state Is Nothing Then Throw New ArgumentNullException("state")
            Dim applicationPath As String = Path.Combine(state.InstallRoot, "Compact Cassette Catalogue.exe")
            Dim uninstallPath As String = Path.Combine(state.InstallRoot, "UNINSTALL.exe")
            Dim statePath As String = Path.Combine(state.InstallRoot, InstalledStateCodec.FileName)
            Dim values As New Dictionary(Of String, Object)(StringComparer.Ordinal)
            values.Add("DisplayName", ProductName)
            values.Add("DisplayVersion", state.Manifest.Label)
            values.Add("DisplayIcon", QuotePath(applicationPath) & ",0")
            values.Add("Publisher", Publisher)
            values.Add("InstallLocation", state.InstallRoot)
            values.Add("InstallDate", state.InstalledAtUtc.ToUniversalTime().ToString("yyyyMMdd", CultureInfo.InvariantCulture))
            values.Add("EstimatedSize", EstimatedSizeKib(state.Manifest))
            values.Add("UninstallString", QuotePath(uninstallPath) & " --state " & QuotePath(statePath))
            values.Add("NoModify", 1)
            values.Add("NoRepair", 1)
            values.Add("URLInfoAbout", InformationUrl)
            values.Add("URLUpdateInfo", UpdateUrl)
            values.Add("C3InstalledState", statePath)
            values.Add("C3Lane", state.Manifest.Lane)
            values.Add("C3SourceCommit", state.Manifest.SourceCommit)
            Return values
        End Function

        Private Shared Function HasOwnershipMarker(values As IDictionary(Of String, Object), state As InstalledState) As Boolean
            Return StringValue(values, "C3InstalledState") = Path.Combine(state.InstallRoot, InstalledStateCodec.FileName) AndAlso
                   StringValue(values, "C3Lane") = state.Manifest.Lane
        End Function

        Private Shared Function StringValue(values As IDictionary(Of String, Object), name As String) As String
            If values Is Nothing OrElse Not values.ContainsKey(name) OrElse Not TypeOf values(name) Is String Then Return Nothing
            Return DirectCast(values(name), String)
        End Function

        Private Shared Sub RequireExactValues(actual As IDictionary(Of String, Object), expected As IDictionary(Of String, Object))
            If actual Is Nothing OrElse actual.Count <> expected.Count Then
                Throw New SetupContractException("The uninstall registration is missing or contains an unexpected value set.")
            End If
            For Each pair As KeyValuePair(Of String, Object) In expected
                If Not actual.ContainsKey(pair.Key) OrElse
                        actual(pair.Key) Is Nothing OrElse
                        actual(pair.Key).GetType() IsNot pair.Value.GetType() OrElse
                        Not Object.Equals(actual(pair.Key), pair.Value) Then
                    Throw New SetupContractException("The uninstall registration value is not owned or does not match: " & pair.Key)
                End If
            Next
        End Sub

        Private Shared Function EstimatedSizeKib(manifest As PayloadManifest) As Integer
            Dim bytes As Long = 0
            For Each item As PayloadFile In manifest.Files
                If bytes > Long.MaxValue - item.Length Then Throw New SetupContractException("Installed size overflowed.")
                bytes += item.Length
            Next
            Dim kib As Long = (bytes + 1023L) \ 1024L
            If kib > Integer.MaxValue Then Return Integer.MaxValue
            Return CInt(kib)
        End Function

        Private Shared Function QuotePath(path As String) As String
            If String.IsNullOrWhiteSpace(path) OrElse path.Contains("""") Then Throw New SetupContractException("An executable path cannot be quoted safely.")
            Return """" & path & """"
        End Function

        Private Shared Function CloneValues(values As IDictionary(Of String, Object)) As IDictionary(Of String, Object)
            If values Is Nothing Then Return Nothing
            Return New Dictionary(Of String, Object)(values, StringComparer.Ordinal)
        End Function

        Private Shared Sub RequireArguments(state As InstalledState, access As ISetupRegistryAccess)
            If state Is Nothing Then Throw New ArgumentNullException("state")
            If access Is Nothing Then Throw New ArgumentNullException("access")
        End Sub

    End Class

End Namespace
