Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices

Namespace Global.C3Setup

    Public NotInheritable Class SetupShortcut

        Public Sub New(path As String, target As String, workingDirectory As String, description As String)
            Me.Path = path
            Me.Target = target
            Me.WorkingDirectory = workingDirectory
            Me.Description = description
        End Sub

        Public ReadOnly Property Path As String
        Public ReadOnly Property Target As String
        Public ReadOnly Property WorkingDirectory As String
        Public ReadOnly Property Description As String
    End Class

    Public Interface ISetupShortcutAccess
        ReadOnly Property CommonProgramsPath As String
        ReadOnly Property CommonDesktopPath As String
        Function ReadShortcut(path As String) As SetupShortcut
        Sub WriteShortcut(shortcut As SetupShortcut)
        Sub DeleteShortcut(path As String)
    End Interface

    Public NotInheritable Class SetupShortcutTransition

        Public Sub New(before As IDictionary(Of String, SetupShortcut), after As IDictionary(Of String, SetupShortcut))
            Me.Before = New Dictionary(Of String, SetupShortcut)(before, StringComparer.OrdinalIgnoreCase)
            Me.After = New Dictionary(Of String, SetupShortcut)(after, StringComparer.OrdinalIgnoreCase)
        End Sub

        Public ReadOnly Property Before As IDictionary(Of String, SetupShortcut)
        Public ReadOnly Property After As IDictionary(Of String, SetupShortcut)
    End Class

    Public NotInheritable Class WindowsSetupShortcutAccess
        Implements ISetupShortcutAccess

        Public ReadOnly Property CommonProgramsPath As String Implements ISetupShortcutAccess.CommonProgramsPath
            Get
                Return Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms)
            End Get
        End Property

        Public ReadOnly Property CommonDesktopPath As String Implements ISetupShortcutAccess.CommonDesktopPath
            Get
                Return Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
            End Get
        End Property

        Public Function ReadShortcut(path As String) As SetupShortcut Implements ISetupShortcutAccess.ReadShortcut
            If Not File.Exists(path) Then Return Nothing
            Dim shell As Object = Nothing
            Dim shortcut As Object = Nothing
            Try
                shortcut = OpenShortcut(path, shell)
                Return New SetupShortcut(path,
                                         CStr(GetProperty(shortcut, "TargetPath")),
                                         CStr(GetProperty(shortcut, "WorkingDirectory")),
                                         CStr(GetProperty(shortcut, "Description")))
            Finally
                ReleaseComObject(shortcut)
                ReleaseComObject(shell)
            End Try
        End Function

        Public Sub WriteShortcut(value As SetupShortcut) Implements ISetupShortcutAccess.WriteShortcut
            Dim parent As String = Directory.GetParent(value.Path).FullName
            If Not Directory.Exists(parent) Then Directory.CreateDirectory(parent)
            Dim shell As Object = Nothing
            Dim shortcut As Object = Nothing
            Try
                shortcut = OpenShortcut(value.Path, shell)
                SetProperty(shortcut, "TargetPath", value.Target)
                SetProperty(shortcut, "WorkingDirectory", value.WorkingDirectory)
                SetProperty(shortcut, "Description", value.Description)
                shortcut.GetType().InvokeMember("Save", BindingFlags.InvokeMethod, Nothing, shortcut, Nothing)
            Finally
                ReleaseComObject(shortcut)
                ReleaseComObject(shell)
            End Try
            Using stream As New FileStream(value.Path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)
                stream.Flush(True)
            End Using
        End Sub

        Public Sub DeleteShortcut(path As String) Implements ISetupShortcutAccess.DeleteShortcut
            If File.Exists(path) Then File.Delete(path)
        End Sub

        Private Shared Function OpenShortcut(path As String, ByRef shell As Object) As Object
            Dim shellType As Type = Type.GetTypeFromProgID("WScript.Shell", True)
            shell = Activator.CreateInstance(shellType)
            Return shellType.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, Nothing, shell, New Object() {path})
        End Function

        Private Shared Function GetProperty(target As Object, name As String) As Object
            Return target.GetType().InvokeMember(name, BindingFlags.GetProperty, Nothing, target, Nothing)
        End Function

        Private Shared Sub SetProperty(target As Object, name As String, value As Object)
            target.GetType().InvokeMember(name, BindingFlags.SetProperty, Nothing, target, New Object() {value})
        End Sub

        Private Shared Sub ReleaseComObject(value As Object)
            If value IsNot Nothing AndAlso Marshal.IsComObject(value) Then Marshal.FinalReleaseComObject(value)
        End Sub
    End Class

    Public NotInheritable Class SetupShortcutService

        Private Const ProductName As String = "Compact Cassette Catalogue"
        Private Const ProductExecutable As String = "Compact Cassette Catalogue.exe"

        Private Sub New()
        End Sub

        Public Shared Function Plan(installRoot As String,
                                    commonProgramsPath As String,
                                    commonDesktopPath As String,
                                    includeDesktop As Boolean) As IList(Of InstalledShortcut)
            Dim root As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            Dim target As String = Path.Combine(root, ProductExecutable)
            Dim shortcuts As New List(Of InstalledShortcut)()
            shortcuts.Add(New InstalledShortcut(ShortcutPath(commonProgramsPath), target))
            If includeDesktop Then shortcuts.Add(New InstalledShortcut(ShortcutPath(commonDesktopPath), target))
            Return shortcuts.AsReadOnly()
        End Function

        Public Shared Function Apply(state As InstalledState,
                                     access As ISetupShortcutAccess) As IDictionary(Of String, SetupShortcut)
            RequireArguments(state, access)
            ValidateAgainstLocations(state, access)
            Dim previous As New Dictionary(Of String, SetupShortcut)(StringComparer.OrdinalIgnoreCase)
            Dim written As New List(Of String)()
            Try
                For Each item As InstalledShortcut In state.Shortcuts
                    Dim expected As SetupShortcut = ExpectedShortcut(item, state.InstallRoot)
                    Dim existing As SetupShortcut = access.ReadShortcut(item.Path)
                    If existing IsNot Nothing AndAlso Not EqualShortcut(existing, expected) Then
                        Throw New SetupContractException("Setup refuses to overwrite an unowned shortcut: " & item.Path)
                    End If
                    previous.Add(item.Path, existing)
                    access.WriteShortcut(expected)
                    written.Add(item.Path)
                    RequireEqual(access.ReadShortcut(item.Path), expected)
                Next
                Return previous
            Catch
                For index As Integer = written.Count - 1 To 0 Step -1
                    Dim path As String = written(index)
                    If previous(path) Is Nothing Then
                        access.DeleteShortcut(path)
                    Else
                        access.WriteShortcut(previous(path))
                    End If
                Next
                Throw
            End Try
        End Function

        Public Shared Function Transition(previousState As InstalledState,
                                          state As InstalledState,
                                          access As ISetupShortcutAccess) As SetupShortcutTransition
            RequireArguments(state, access)
            If previousState IsNot Nothing Then ValidateAgainstLocations(previousState, access)
            ValidateAgainstLocations(state, access)

            Dim beforeExpected As IDictionary(Of String, SetupShortcut) = ExpectedMap(previousState)
            Dim afterExpected As IDictionary(Of String, SetupShortcut) = ExpectedMap(state)
            Dim allPaths As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            For Each path As String In beforeExpected.Keys
                allPaths(path) = True
            Next
            For Each path As String In afterExpected.Keys
                allPaths(path) = True
            Next

            Dim beforeActual As New Dictionary(Of String, SetupShortcut)(StringComparer.OrdinalIgnoreCase)
            For Each path As String In allPaths.Keys
                Dim existing As SetupShortcut = access.ReadShortcut(path)
                If Not beforeExpected.ContainsKey(path) AndAlso existing IsNot Nothing Then
                    Throw New SetupContractException("Setup refuses to adopt an unowned shortcut: " & path)
                End If
                beforeActual.Add(path, existing)
            Next

            Try
                For Each path As String In beforeExpected.Keys
                    If Not afterExpected.ContainsKey(path) Then
                        RequireEqual(access.ReadShortcut(path), beforeExpected(path))
                        access.DeleteShortcut(path)
                        If access.ReadShortcut(path) IsNot Nothing Then Throw New SetupContractException("A deselected owned shortcut remains: " & path)
                    End If
                Next
                For Each path As String In afterExpected.Keys
                    access.WriteShortcut(afterExpected(path))
                    RequireEqual(access.ReadShortcut(path), afterExpected(path))
                Next
                Return New SetupShortcutTransition(beforeActual, afterExpected)
            Catch
                RestoreSnapshot(beforeActual, access)
                Throw
            End Try
        End Function

        Public Shared Sub RestoreTransition(transition As SetupShortcutTransition,
                                            access As ISetupShortcutAccess)
            If transition Is Nothing Then Throw New ArgumentNullException("transition")
            If access Is Nothing Then Throw New ArgumentNullException("access")
            For Each path As String In transition.Before.Keys
                If transition.After.ContainsKey(path) Then
                    RequireEqual(access.ReadShortcut(path), transition.After(path))
                ElseIf access.ReadShortcut(path) IsNot Nothing Then
                    Throw New SetupContractException("Shortcut rollback found an unexpected link at a removed path.")
                End If
            Next
            RestoreSnapshot(transition.Before, access)
        End Sub

        Public Shared Sub Restore(state As InstalledState,
                                  previous As IDictionary(Of String, SetupShortcut),
                                  access As ISetupShortcutAccess)
            RequireArguments(state, access)
            If previous Is Nothing OrElse previous.Count <> state.Shortcuts.Count Then
                Throw New SetupContractException("Shortcut rollback state is incomplete.")
            End If
            For Each item As InstalledShortcut In state.Shortcuts
                RequireEqual(access.ReadShortcut(item.Path), ExpectedShortcut(item, state.InstallRoot))
            Next
            For Each item As InstalledShortcut In state.Shortcuts
                If previous(item.Path) Is Nothing Then
                    access.DeleteShortcut(item.Path)
                Else
                    access.WriteShortcut(previous(item.Path))
                    RequireEqual(access.ReadShortcut(item.Path), previous(item.Path))
                End If
            Next
        End Sub

        Public Shared Sub Remove(state As InstalledState, access As ISetupShortcutAccess)
            RemoveTransition(state, access)
        End Sub

        Public Shared Sub ValidateOwned(state As InstalledState, access As ISetupShortcutAccess)
            RequireArguments(state, access)
            ValidateAgainstLocations(state, access)
            For Each item As InstalledShortcut In state.Shortcuts
                RequireEqual(access.ReadShortcut(item.Path), ExpectedShortcut(item, state.InstallRoot))
            Next
        End Sub

        Public Shared Function RemoveTransition(state As InstalledState,
                                                access As ISetupShortcutAccess) As SetupShortcutTransition
            RequireArguments(state, access)
            ValidateAgainstLocations(state, access)
            Dim beforeMap As IDictionary(Of String, SetupShortcut) = ExpectedMap(state)
            For Each item As InstalledShortcut In state.Shortcuts
                RequireEqual(access.ReadShortcut(item.Path), ExpectedShortcut(item, state.InstallRoot))
            Next
            Dim removed As New List(Of SetupShortcut)()
            Try
                For Each item As InstalledShortcut In state.Shortcuts
                    Dim expected As SetupShortcut = ExpectedShortcut(item, state.InstallRoot)
                    access.DeleteShortcut(item.Path)
                    If access.ReadShortcut(item.Path) IsNot Nothing Then
                        Throw New SetupContractException("An owned shortcut remains after removal: " & item.Path)
                    End If
                    removed.Add(expected)
                Next
                Return New SetupShortcutTransition(beforeMap, New Dictionary(Of String, SetupShortcut)(StringComparer.OrdinalIgnoreCase))
            Catch
                For index As Integer = removed.Count - 1 To 0 Step -1
                    access.WriteShortcut(removed(index))
                    RequireEqual(access.ReadShortcut(removed(index).Path), removed(index))
                Next
                Throw
            End Try
        End Function

        Public Shared Sub ValidateOwnedShortcuts(state As InstalledState)
            If state.Shortcuts.Count > 2 Then Throw New SetupContractException("Installed state owns too many shortcuts.")
            Dim seen As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            Dim target As String = Path.Combine(state.InstallRoot, ProductExecutable)
            For Each item As InstalledShortcut In state.Shortcuts
                Dim path As String = CanonicalShortcutPath(item.Path)
                If Not String.Equals(path, item.Path, StringComparison.OrdinalIgnoreCase) Then
                    Throw New SetupContractException("Installed state contains a noncanonical shortcut path.")
                End If
                If seen.ContainsKey(path) Then Throw New SetupContractException("Installed state contains a duplicate shortcut.")
                seen.Add(path, True)
                If Not String.Equals(item.Target, target, StringComparison.OrdinalIgnoreCase) Then
                    Throw New SetupContractException("Installed shortcut target is not the owned C3 executable.")
                End If
            Next
        End Sub

        Private Shared Sub ValidateAgainstLocations(state As InstalledState, access As ISetupShortcutAccess)
            ValidateOwnedShortcuts(state)
            Dim allowed As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            allowed.Add(ShortcutPath(access.CommonProgramsPath), True)
            allowed.Add(ShortcutPath(access.CommonDesktopPath), True)
            For Each item As InstalledShortcut In state.Shortcuts
                If Not allowed.ContainsKey(CanonicalShortcutPath(item.Path)) Then
                    Throw New SetupContractException("Installed state claims a shortcut outside the common locations.")
                End If
            Next
            Dim hasStartMenu As Boolean = False
            For Each item As InstalledShortcut In state.Shortcuts
                If String.Equals(CanonicalShortcutPath(item.Path), ShortcutPath(access.CommonProgramsPath), StringComparison.OrdinalIgnoreCase) Then
                    hasStartMenu = True
                End If
            Next
            If Not hasStartMenu Then
                Throw New SetupContractException("The owned common Start Menu shortcut is missing.")
            End If
        End Sub

        Private Shared Function ExpectedShortcut(item As InstalledShortcut, installRoot As String) As SetupShortcut
            Return New SetupShortcut(CanonicalShortcutPath(item.Path),
                                     Path.Combine(installRoot, ProductExecutable),
                                     installRoot,
                                     ProductName)
        End Function

        Private Shared Function ExpectedMap(state As InstalledState) As IDictionary(Of String, SetupShortcut)
            Dim result As New Dictionary(Of String, SetupShortcut)(StringComparer.OrdinalIgnoreCase)
            If state Is Nothing Then Return result
            For Each item As InstalledShortcut In state.Shortcuts
                result.Add(item.Path, ExpectedShortcut(item, state.InstallRoot))
            Next
            Return result
        End Function

        Private Shared Sub RestoreSnapshot(snapshot As IDictionary(Of String, SetupShortcut), access As ISetupShortcutAccess)
            For Each path As String In snapshot.Keys
                If snapshot(path) Is Nothing Then
                    access.DeleteShortcut(path)
                Else
                    access.WriteShortcut(snapshot(path))
                    RequireEqual(access.ReadShortcut(path), snapshot(path))
                End If
            Next
        End Sub

        Private Shared Function ShortcutPath(directoryPath As String) As String
            Dim directory As String = SetupPathPolicy.CanonicalDirectory(directoryPath)
            Return CanonicalShortcutPath(Path.Combine(directory, ProductName & ".lnk"))
        End Function

        Private Shared Function CanonicalShortcutPath(path As String) As String
            If String.IsNullOrWhiteSpace(path) OrElse Not path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("A shortcut must be an absolute .lnk path.")
            End If
            Dim fullPath As String = System.IO.Path.GetFullPath(path)
            If Not System.IO.Path.IsPathRooted(fullPath) OrElse System.IO.Path.GetFileName(fullPath) <> ProductName & ".lnk" Then
                Throw New SetupContractException("The C3 shortcut path is not canonical.")
            End If
            Return fullPath
        End Function

        Private Shared Function EqualShortcut(left As SetupShortcut, right As SetupShortcut) As Boolean
            Return left IsNot Nothing AndAlso right IsNot Nothing AndAlso
                   String.Equals(left.Path, right.Path, StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(left.Target, right.Target, StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(left.WorkingDirectory, right.WorkingDirectory, StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(left.Description, right.Description, StringComparison.Ordinal)
        End Function

        Private Shared Sub RequireEqual(actual As SetupShortcut, expected As SetupShortcut)
            If Not EqualShortcut(actual, expected) Then Throw New SetupContractException("The shortcut is missing, altered, or not owned: " & expected.Path)
        End Sub

        Private Shared Sub RequireArguments(state As InstalledState, access As ISetupShortcutAccess)
            If state Is Nothing Then Throw New ArgumentNullException("state")
            If access Is Nothing Then Throw New ArgumentNullException("access")
        End Sub

    End Class

End Namespace
