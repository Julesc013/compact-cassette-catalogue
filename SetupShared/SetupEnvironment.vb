Imports System.Diagnostics
Imports System.IO
Imports System.Security.Principal
Imports Microsoft.Win32

Namespace Global.C3Setup

    Public NotInheritable Class SetupEnvironmentFacts

        Public Sub New(processArchitecture As String,
                       nativeArchitecture As String,
                       isElevated As Boolean,
                       frameworkFullInstalled As Boolean,
                       frameworkRelease As Long,
                       programFilesPath As String,
                       applicationRunning As Boolean,
                       availableBytes As Long)
            Me.ProcessArchitecture = processArchitecture
            Me.NativeArchitecture = nativeArchitecture
            Me.IsElevated = isElevated
            Me.FrameworkFullInstalled = frameworkFullInstalled
            Me.FrameworkRelease = frameworkRelease
            Me.ProgramFilesPath = programFilesPath
            Me.ApplicationRunning = applicationRunning
            Me.AvailableBytes = availableBytes
        End Sub

        Public ReadOnly Property ProcessArchitecture As String
        Public ReadOnly Property NativeArchitecture As String
        Public ReadOnly Property IsElevated As Boolean
        Public ReadOnly Property FrameworkFullInstalled As Boolean
        Public ReadOnly Property FrameworkRelease As Long
        Public ReadOnly Property ProgramFilesPath As String
        Public ReadOnly Property ApplicationRunning As Boolean
        Public ReadOnly Property AvailableBytes As Long

    End Class

    Public NotInheritable Class SetupEnvironment

        Private Const ProductDirectoryName As String = "Compact Cassette Catalogue"

        Private Sub New()
        End Sub

        Public Shared Function Capture() As SetupEnvironmentFacts
            Dim programFilesPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            If String.IsNullOrWhiteSpace(programFilesPath) Then
                Throw New SetupContractException("Windows did not provide a Program Files path.")
            End If
            Dim canonicalProgramFiles As String = SetupPathPolicy.CanonicalDirectory(programFilesPath)
            Dim drive As New DriveInfo(Path.GetPathRoot(canonicalProgramFiles))
            Dim frameworkInstalled As Boolean = False
            Dim frameworkRelease As Long = 0
            ReadFrameworkFacts(frameworkInstalled, frameworkRelease)

            Return New SetupEnvironmentFacts(ProcessArchitecture(),
                                             NativeArchitecture(),
                                             IsElevated(),
                                             frameworkInstalled,
                                             frameworkRelease,
                                             canonicalProgramFiles,
                                             IsApplicationRunning(),
                                             drive.AvailableFreeSpace)
        End Function

        Public Shared Sub Validate(manifest As PayloadManifest,
                                   facts As SetupEnvironmentFacts,
                                   transactionBytes As Long)
            If manifest Is Nothing Then Throw New ArgumentNullException("manifest")
            If facts Is Nothing Then Throw New ArgumentNullException("facts")
            If transactionBytes < 0 Then Throw New ArgumentOutOfRangeException("transactionBytes")

            Dim expectedArchitecture As String
            Dim minimumRelease As Long
            Select Case manifest.Lane
                Case "win-x86-net40"
                    expectedArchitecture = "x86"
                    minimumRelease = 0
                    RequireIdentity(manifest, "x86", "v4.0")
                Case "win-x64-net48"
                    expectedArchitecture = "x64"
                    minimumRelease = 528049
                    RequireIdentity(manifest, "x64", "v4.8")
                Case "win-arm64-net481"
                    expectedArchitecture = "ARM64"
                    minimumRelease = 533320
                    RequireIdentity(manifest, "ARM64", "v4.8.1")
                Case Else
                    Throw New SetupContractException("The payload lane is not a C3 1.3 setup lane.")
            End Select

            If facts.ProcessArchitecture <> expectedArchitecture OrElse facts.NativeArchitecture <> expectedArchitecture Then
                Throw New SetupContractException("Setup must execute natively on the payload architecture; emulation and cross-architecture installation are prohibited.")
            End If
            If Not facts.FrameworkFullInstalled OrElse facts.FrameworkRelease < minimumRelease Then
                Throw New SetupContractException("The payload lane's full .NET Framework prerequisite is not installed.")
            End If
            If Not facts.IsElevated Then
                Throw New SetupContractException("Per-machine C3 setup requires an elevated administrator token.")
            End If
            If facts.ApplicationRunning Then
                Throw New SetupContractException("Compact Cassette Catalogue is running and must be closed before setup continues.")
            End If
            Dim canonicalProgramFiles As String = SetupPathPolicy.CanonicalDirectory(facts.ProgramFilesPath)
            If Not Directory.Exists(canonicalProgramFiles) Then
                Throw New SetupContractException("The operating-system Program Files directory does not exist.")
            End If
            Dim requiredBytes As Long
            Try
                requiredBytes = checkedMultiply(transactionBytes, 3L)
            Catch ex As OverflowException
                Throw New SetupContractException("The payload size cannot be represented safely.", ex)
            End Try
            If facts.AvailableBytes < requiredBytes Then
                Throw New SetupContractException("The destination volume does not have enough free space for staging and rollback.")
            End If
        End Sub

        Public Shared Function DefaultInstallRoot(facts As SetupEnvironmentFacts) As String
            If facts Is Nothing Then Throw New ArgumentNullException("facts")
            Return SetupPathPolicy.ValidateInstallRoot(Path.Combine(facts.ProgramFilesPath, ProductDirectoryName))
        End Function

        Public Shared Function ValidateInstallRoot(facts As SetupEnvironmentFacts, installRoot As String) As String
            If facts Is Nothing Then Throw New ArgumentNullException("facts")
            Dim programFiles As String = SetupPathPolicy.CanonicalDirectory(facts.ProgramFilesPath)
            Dim root As String = SetupPathPolicy.ValidateInstallRoot(installRoot)
            Dim prefix As String = programFiles.TrimEnd(Path.DirectorySeparatorChar) & Path.DirectorySeparatorChar
            If Not root.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                Throw New SetupContractException("Per-machine C3 setup must install below the operating-system Program Files directory.")
            End If
            Return root
        End Function

        Private Shared Sub RequireIdentity(manifest As PayloadManifest, architecture As String, framework As String)
            If manifest.Architecture <> architecture OrElse manifest.Framework <> framework Then
                Throw New SetupContractException("Payload lane, architecture, and framework identity do not agree.")
            End If
        End Sub

        Private Shared Function checkedMultiply(left As Long, right As Long) As Long
            If left <> 0 AndAlso left > Long.MaxValue \ right Then Throw New OverflowException()
            Return left * right
        End Function

        Private Shared Function ProcessArchitecture() As String
            If IntPtr.Size = 4 Then Return "x86"
            Return NormalizeArchitecture(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"))
        End Function

        Private Shared Function NativeArchitecture() As String
            Dim value As String = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")
            If String.IsNullOrWhiteSpace(value) Then value = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")
            Return NormalizeArchitecture(value)
        End Function

        Private Shared Function NormalizeArchitecture(value As String) As String
            If String.Equals(value, "x86", StringComparison.OrdinalIgnoreCase) Then Return "x86"
            If String.Equals(value, "AMD64", StringComparison.OrdinalIgnoreCase) OrElse
                    String.Equals(value, "x64", StringComparison.OrdinalIgnoreCase) Then Return "x64"
            If String.Equals(value, "ARM64", StringComparison.OrdinalIgnoreCase) Then Return "ARM64"
            Throw New SetupContractException("Windows reported an unsupported processor architecture.")
        End Function

        Private Shared Function IsElevated() As Boolean
            Using identity As WindowsIdentity = WindowsIdentity.GetCurrent()
                Return New WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator)
            End Using
        End Function

        Private Shared Function IsApplicationRunning() As Boolean
            Dim processes As Process() = Process.GetProcessesByName(ProductDirectoryName)
            Try
                Return processes.Length <> 0
            Finally
                For Each process As Process In processes
                    process.Dispose()
                Next
            End Try
        End Function

        Private Shared Sub ReadFrameworkFacts(ByRef installed As Boolean, ByRef release As Long)
            Using baseKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                Using frameworkKey As RegistryKey = baseKey.OpenSubKey("SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", False)
                    If frameworkKey Is Nothing Then Return
                    Dim installValue As Object = frameworkKey.GetValue("Install", 0)
                    installed = Convert.ToInt32(installValue, Globalization.CultureInfo.InvariantCulture) = 1
                    Dim releaseValue As Object = frameworkKey.GetValue("Release", 0)
                    release = Convert.ToInt64(releaseValue, Globalization.CultureInfo.InvariantCulture)
                End Using
            End Using
        End Sub

    End Class

End Namespace
