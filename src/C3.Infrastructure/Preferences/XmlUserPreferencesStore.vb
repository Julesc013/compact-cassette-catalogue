Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Threading
Imports System.Xml
Imports C3.Infrastructure.FileOperations
Imports C3.Infrastructure.Updates

Namespace Preferences

Public NotInheritable Class XmlUserPreferencesStore

    Private Const MaximumFileBytes As Long = 256L * 1024L
    Private Const LockAttemptCount As Integer = 40
    Private Const LockRetryMilliseconds As Integer = 50
    Private Const RecoveryPathAttemptCount As Integer = 32
    Private ReadOnly _clock As Func(Of DateTime)

    Public Sub New(preferencesPath As String, clock As Func(Of DateTime))
        If String.IsNullOrWhiteSpace(preferencesPath) Then
            Throw New ArgumentException("A preferences path is required.", "preferencesPath")
        End If
        If clock Is Nothing Then
            Throw New ArgumentNullException("clock")
        End If

        Me.PreferencesPath = Path.GetFullPath(preferencesPath)
        _clock = clock
    End Sub

    Public ReadOnly Property PreferencesPath As String

    Public ReadOnly Property BackupPath As String
        Get
            Return PreferencesPath & ".bak"
        End Get
    End Property

    Public Function Load() As UserPreferencesLoadResult
        Try
            Using lockHandle As IDisposable = AcquireExclusiveLock()
                Return LoadPrimaryUnlocked()
            End Using
        Catch ex As TimeoutException
            Return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Busy, ex.Message)
        Catch ex As UnauthorizedAccessException
            Return UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex.Message)
        Catch ex As SecurityException
            Return UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex.Message)
        Catch ex As IOException
            Return UserPreferencesLoadResult.Failed(UserPreferencesFailure.IoFailure, ex.Message)
        End Try
    End Function

    Public Function Save(
            preferences As UserPreferencesSnapshot,
            dirtyFields As UserPreferenceFields) As UserPreferencesSaveResult

        If preferences Is Nothing Then
            Throw New ArgumentNullException("preferences")
        End If
        If (CInt(dirtyFields) And Not CInt(UserPreferenceFields.All)) <> 0 Then
            Return UserPreferencesSaveResult.Failed(
                UserPreferencesFailure.Invalid,
                "The preferences dirty-field mask is invalid.")
        End If

        Try
            Using lockHandle As IDisposable = AcquireExclusiveLock()
                Dim current As UserPreferencesLoadResult = LoadPrimaryUnlocked()
                If Not current.IsSuccess AndAlso Not current.IsMissing Then
                    Return UserPreferencesSaveResult.Failed(current.Failure, current.Message)
                End If

                Dim merged As UserPreferencesSnapshot
                If current.IsSuccess Then
                    merged = Merge(current.Preferences, preferences, dirtyFields)
                Else
                    merged = preferences.Clone()
                End If

                If preferences.Legacy1xImportVersion > merged.Legacy1xImportVersion Then
                    merged.Legacy1xImportVersion = preferences.Legacy1xImportVersion
                    merged.Legacy1xImportOutcome = preferences.Legacy1xImportOutcome
                End If
                Return SaveExactUnlocked(merged)
            End Using
        Catch ex As TimeoutException
            Return UserPreferencesSaveResult.Failed(UserPreferencesFailure.Busy, ex.Message)
        Catch ex As UnauthorizedAccessException
            Return UserPreferencesSaveResult.Failed(UserPreferencesFailure.AccessDenied, ex.Message)
        Catch ex As SecurityException
            Return UserPreferencesSaveResult.Failed(UserPreferencesFailure.AccessDenied, ex.Message)
        Catch ex As IOException
            Return UserPreferencesSaveResult.Failed(UserPreferencesFailure.IoFailure, ex.Message)
        End Try
    End Function

    Friend Function AcquireExclusiveLock() As IDisposable
        Dim directoryPath As String = Path.GetDirectoryName(PreferencesPath)
        If String.IsNullOrWhiteSpace(directoryPath) Then
            Throw New IOException("The preferences directory could not be resolved.")
        End If
        Directory.CreateDirectory(directoryPath)

        Dim lockPath As String = Path.Combine(directoryPath, "preferences.lock")
        For attempt As Integer = 1 To LockAttemptCount
            Try
                Return New FileStream(
                    lockPath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None)
            Catch ex As IOException
                If Not IsLockContention(ex) Then
                    Throw
                End If
                If attempt = LockAttemptCount Then
                    Throw New TimeoutException(
                        "Another C3 process is using the shared preferences file.",
                        ex)
                End If
                Thread.Sleep(LockRetryMilliseconds)
            End Try
        Next

        Throw New TimeoutException("Another C3 process is using the shared preferences file.")
    End Function

    Friend Function LoadPrimaryUnlocked() As UserPreferencesLoadResult
        Return LoadPathUnlocked(PreferencesPath)
    End Function

    Friend Function LoadBackupUnlocked() As UserPreferencesLoadResult
        Return LoadPathUnlocked(BackupPath)
    End Function

    Friend Function QuarantinePrimaryUnlocked() As String
        If Not File.Exists(PreferencesPath) Then
            Return Nothing
        End If

        Dim directoryPath As String = Path.GetDirectoryName(PreferencesPath)
        Dim stamp As DateTime = _clock().ToUniversalTime()
        For attempt As Integer = 1 To RecoveryPathAttemptCount
            Dim quarantinePath As String = Path.Combine(
                directoryPath,
                CompactSiblingFileName.CreateRecovery(stamp))
            Try
                File.Move(PreferencesPath, quarantinePath)
                Return quarantinePath
            Catch ex As IOException
                ' A pre-existing recovery path is only a generated-name
                ' collision. Preserve it and retry without masking other I/O.
                If Not File.Exists(PreferencesPath) OrElse
                        (Not File.Exists(quarantinePath) AndAlso
                            Not Directory.Exists(quarantinePath)) Then
                    Throw
                End If
            End Try
        Next

        Throw New IOException("C3 could not reserve a unique preferences recovery path.")
    End Function

    Friend Function SaveExactUnlocked(
            preferences As UserPreferencesSnapshot) As UserPreferencesSaveResult

        If preferences Is Nothing Then
            Throw New ArgumentNullException("preferences")
        End If
        Dim normalized As UserPreferencesSnapshot = NormalizeForPersistence(preferences)
        Dim validationMessage As String = ValidateSnapshot(normalized)
        If validationMessage IsNot Nothing Then
            Return UserPreferencesSaveResult.Failed(
                UserPreferencesFailure.Invalid,
                validationMessage)
        End If

        Dim directoryPath As String = Path.GetDirectoryName(PreferencesPath)
        Directory.CreateDirectory(directoryPath)
        Dim backupFilePath As String = Me.BackupPath

        Try
            Dim writerSettings As New XmlWriterSettings() With {
                .Encoding = New UTF8Encoding(False),
                .Indent = True,
                .NewLineChars = vbLf,
                .NewLineHandling = NewLineHandling.Replace,
                .CloseOutput = False
            }

            Using temporaryFile As OwnedSiblingTemporaryFile =
                    OwnedSiblingTemporaryFile.Create(PreferencesPath)
                Using stream As FileStream = temporaryFile.Stream
                    Using writer As XmlWriter = XmlWriter.Create(stream, writerSettings)
                        WriteSnapshot(writer, normalized)
                    End Using
                    stream.Flush(True)
                End Using

                Dim verification As UserPreferencesLoadResult = LoadPathUnlocked(temporaryFile.Path)
                If Not verification.IsSuccess OrElse
                        Not AreEquivalent(normalized, verification.Preferences) Then
                    Dim details As String = If(
                        verification.IsSuccess,
                        "The preferences snapshot changed during round-trip verification.",
                        verification.Message)
                    Return UserPreferencesSaveResult.Failed(
                        UserPreferencesFailure.VerificationFailure,
                        details)
                End If

                If File.Exists(PreferencesPath) Then
                    File.Replace(temporaryFile.Path, PreferencesPath, backupFilePath, True)
                Else
                    File.Move(temporaryFile.Path, PreferencesPath)
                    backupFilePath = Nothing
                End If

                Return UserPreferencesSaveResult.Saved(normalized.Clone(), backupFilePath)
            End Using
        Catch ex As UnauthorizedAccessException
            Return UserPreferencesSaveResult.Failed(UserPreferencesFailure.AccessDenied, ex.Message)
        Catch ex As SecurityException
            Return UserPreferencesSaveResult.Failed(UserPreferencesFailure.AccessDenied, ex.Message)
        Catch ex As IOException
            Return UserPreferencesSaveResult.Failed(UserPreferencesFailure.IoFailure, ex.Message)
        End Try
    End Function

    Private Shared Function LoadPathUnlocked(pathValue As String) As UserPreferencesLoadResult
        Try
            Dim settings As New XmlReaderSettings() With {
                .DtdProcessing = DtdProcessing.Prohibit,
                .XmlResolver = Nothing,
                .MaxCharactersInDocument = MaximumFileBytes,
                .MaxCharactersFromEntities = 0
            }
            Dim document As New XmlDocument()
            document.XmlResolver = Nothing
            Using stream As New FileStream(pathValue, FileMode.Open, FileAccess.Read, FileShare.Read)
                If stream.Length > MaximumFileBytes Then
                    Return UserPreferencesLoadResult.Failed(
                        UserPreferencesFailure.TooLarge,
                        "The preferences file exceeds the 256 KiB safety limit.")
                End If
                Using reader As XmlReader = XmlReader.Create(stream, settings)
                    document.Load(reader)
                End Using
            End Using

            Return ParseDocument(document)
        Catch ex As FileNotFoundException
            Return UserPreferencesLoadResult.Missing()
        Catch ex As DirectoryNotFoundException
            Return UserPreferencesLoadResult.Missing()
        Catch ex As UnauthorizedAccessException
            Return UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex.Message)
        Catch ex As SecurityException
            Return UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex.Message)
        Catch ex As IOException
            Return UserPreferencesLoadResult.Failed(UserPreferencesFailure.IoFailure, ex.Message)
        Catch ex As XmlException
            Return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, ex.Message)
        End Try
    End Function

    Private Shared Function ParseDocument(document As XmlDocument) As UserPreferencesLoadResult
        Dim root As XmlElement = document.DocumentElement
        If root Is Nothing Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                "The preferences root element is invalid.")
        End If
        If root.LocalName = "c3Preferences" AndAlso root.NamespaceURI.Length > 0 Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.UnsupportedVersion,
                "The preferences namespace was written by a newer C3 version.")
        End If
        If root.Name <> "c3Preferences" Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                "The preferences root element is invalid.")
        End If

        Dim schemaVersion As Integer
        If Not Integer.TryParse(
                root.GetAttribute("schemaVersion"),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                schemaVersion) OrElse schemaVersion < 1 Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                "The preferences schema version is invalid.")
        End If
        If schemaVersion > 1 Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.UnsupportedVersion,
                "The preferences schema was written by a newer C3 version.")
        End If
        If root.Attributes.Count <> 3 Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                "The preferences schema metadata is invalid.")
        End If

        Dim importVersion As Integer
        If Not Integer.TryParse(
                root.GetAttribute("legacy1xImportVersion"),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                importVersion) OrElse
                importVersion < 0 Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                "The legacy settings import marker is invalid.")
        End If
        If importVersion > UserPreferencesSnapshot.CurrentLegacyImportVersion Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.UnsupportedVersion,
                "The preferences migration marker was written by a newer C3 version.")
        End If

        Dim importOutcome As String = root.GetAttribute("legacy1xImportOutcome")
        If Not IsImportOutcomeValid(importVersion, importOutcome) Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                "The legacy settings import outcome is invalid.")
        End If

        Dim values As New Dictionary(Of String, String)(StringComparer.Ordinal)
        For Each child As XmlNode In root.ChildNodes
            If child.NodeType = XmlNodeType.Comment OrElse
                    child.NodeType = XmlNodeType.Whitespace OrElse
                    child.NodeType = XmlNodeType.SignificantWhitespace Then
                Continue For
            End If
            If child.NodeType <> XmlNodeType.Element OrElse
                    child.NamespaceURI.Length > 0 OrElse
                    Not IsKnownElement(child.Name) OrElse
                    values.ContainsKey(child.Name) Then
                Return UserPreferencesLoadResult.Failed(
                    UserPreferencesFailure.Invalid,
                    "The preferences file contains an unknown or duplicate field.")
            End If
            Dim element As XmlElement = DirectCast(child, XmlElement)
            Dim scalarValue As String = Nothing
            If Not TryReadScalar(element, scalarValue) Then
                Return UserPreferencesLoadResult.Failed(
                    UserPreferencesFailure.Invalid,
                    "The preferences file contains a non-scalar field.")
            End If
            values.Add(child.Name, scalarValue)
        Next

        For Each requiredName As String In {
                "showMessages",
                "defaultDirectory",
                "updatePolicy",
                "lastUpdateCheck"}
            If Not values.ContainsKey(requiredName) Then
                Return UserPreferencesLoadResult.Failed(
                    UserPreferencesFailure.Invalid,
                    "The preferences file is missing '" & requiredName & "'.")
            End If
        Next

        Dim showMessages As Boolean
        If Not Boolean.TryParse(values("showMessages"), showMessages) Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                "The showMessages preference is invalid.")
        End If

        Dim updatePolicy As UpdateCheckPolicy
        If Not UpdateCheckSchedule.TryParseStored(values("updatePolicy"), updatePolicy) Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                "The updatePolicy preference is invalid.")
        End If

        Dim lastUpdateCheck As DateTime = DateTime.MinValue
        If Not String.IsNullOrWhiteSpace(values("lastUpdateCheck")) Then
            Try
                lastUpdateCheck = XmlConvert.ToDateTime(
                    values("lastUpdateCheck").Trim(),
                    XmlDateTimeSerializationMode.RoundtripKind)
            Catch ex As FormatException
                Return UserPreferencesLoadResult.Failed(
                    UserPreferencesFailure.Invalid,
                    "The lastUpdateCheck preference is invalid.")
            End Try
        End If

        Dim preferences As New UserPreferencesSnapshot() With {
            .ShowMessages = showMessages,
            .DefaultDirectory = values("defaultDirectory"),
            .UpdatePolicy = updatePolicy,
            .LastUpdateCheck = lastUpdateCheck,
            .Legacy1xImportVersion = importVersion,
            .Legacy1xImportOutcome = importOutcome
        }
        Dim validationMessage As String = ValidateSnapshot(preferences)
        If validationMessage IsNot Nothing Then
            Return UserPreferencesLoadResult.Failed(
                UserPreferencesFailure.Invalid,
                validationMessage)
        End If
        Return UserPreferencesLoadResult.Loaded(preferences)
    End Function

    Private Shared Sub WriteSnapshot(writer As XmlWriter, preferences As UserPreferencesSnapshot)
        writer.WriteStartDocument()
        writer.WriteStartElement("c3Preferences")
        writer.WriteAttributeString("schemaVersion", "1")
        writer.WriteAttributeString(
            "legacy1xImportVersion",
            preferences.Legacy1xImportVersion.ToString(CultureInfo.InvariantCulture))
        writer.WriteAttributeString("legacy1xImportOutcome", preferences.Legacy1xImportOutcome)
        writer.WriteElementString("showMessages", preferences.ShowMessages.ToString().ToLowerInvariant())
        writer.WriteElementString("defaultDirectory", If(preferences.DefaultDirectory, String.Empty))
        writer.WriteElementString("updatePolicy", UpdateCheckSchedule.Serialize(preferences.UpdatePolicy))
        writer.WriteElementString(
            "lastUpdateCheck",
            If(
                preferences.LastUpdateCheck = DateTime.MinValue,
                String.Empty,
                XmlConvert.ToString(
                    preferences.LastUpdateCheck,
                    XmlDateTimeSerializationMode.RoundtripKind)))
        writer.WriteEndElement()
        writer.WriteEndDocument()
    End Sub

    Private Shared Function ValidateSnapshot(preferences As UserPreferencesSnapshot) As String
        If preferences Is Nothing Then
            Return "Preferences are required."
        End If
        If preferences.Legacy1xImportVersion < 0 OrElse
                preferences.Legacy1xImportVersion > UserPreferencesSnapshot.CurrentLegacyImportVersion Then
            Return "The legacy settings import marker is invalid."
        End If
        If Not IsImportOutcomeValid(
                preferences.Legacy1xImportVersion,
                preferences.Legacy1xImportOutcome) Then
            Return "The legacy settings import outcome is invalid."
        End If
        If [Enum].IsDefined(GetType(UpdateCheckPolicy), preferences.UpdatePolicy) = False Then
            Return "The update policy is invalid."
        End If
        If preferences.DefaultDirectory Is Nothing Then
            Return "The default directory cannot be null."
        End If
        If preferences.DefaultDirectory.Length >
                UserPreferencesSnapshot.MaximumDefaultDirectoryCharacters Then
            Return "The default directory exceeds the safety limit."
        End If
        Try
            XmlConvert.VerifyXmlChars(preferences.DefaultDirectory)
        Catch ex As XmlException
            Return "The default directory contains invalid XML characters."
        End Try
        Return Nothing
    End Function

    Private Shared Function NormalizeForPersistence(
            preferences As UserPreferencesSnapshot) As UserPreferencesSnapshot

        Dim normalized As UserPreferencesSnapshot = preferences.Clone()
        normalized.DefaultDirectory = If(normalized.DefaultDirectory, String.Empty)
        If normalized.LastUpdateCheck <> DateTime.MinValue Then
            normalized.LastUpdateCheck = Updates.UpdateCheckSchedule.NormalizeUtc(
                normalized.LastUpdateCheck)
        End If
        Return normalized
    End Function

    Private Shared Function TryReadScalar(
            element As XmlElement,
            ByRef value As String) As Boolean

        If element.Attributes.Count <> 0 Then
            Return False
        End If
        For Each child As XmlNode In element.ChildNodes
            If child.NodeType <> XmlNodeType.Text Then
                Return False
            End If
        Next
        value = element.InnerText
        Return True
    End Function

    Private Shared Function IsKnownElement(name As String) As Boolean
        Return name = "showMessages" OrElse
            name = "defaultDirectory" OrElse
            name = "updatePolicy" OrElse
            name = "lastUpdateCheck"
    End Function

    Private Shared Function IsImportOutcomeValid(version As Integer, outcome As String) As Boolean
        If version = 0 Then
            Return outcome = UserPreferencesSnapshot.ImportOutcomePending
        End If
        Return outcome = UserPreferencesSnapshot.ImportOutcomeImported OrElse
            outcome = UserPreferencesSnapshot.ImportOutcomeNotFound OrElse
            outcome = UserPreferencesSnapshot.ImportOutcomeInvalid
    End Function

    Private Shared Function Merge(
            current As UserPreferencesSnapshot,
            incoming As UserPreferencesSnapshot,
            dirtyFields As UserPreferenceFields) As UserPreferencesSnapshot

        Dim merged As UserPreferencesSnapshot = current.Clone()
        If (dirtyFields And UserPreferenceFields.ShowMessages) <> 0 Then
            merged.ShowMessages = incoming.ShowMessages
        End If
        If (dirtyFields And UserPreferenceFields.DefaultDirectory) <> 0 Then
            merged.DefaultDirectory = incoming.DefaultDirectory
        End If
        If (dirtyFields And UserPreferenceFields.UpdatePolicy) <> 0 Then
            merged.UpdatePolicy = incoming.UpdatePolicy
        End If
        If (dirtyFields And UserPreferenceFields.LastUpdateCheck) <> 0 Then
            merged.LastUpdateCheck = incoming.LastUpdateCheck
        End If
        Return merged
    End Function

    Private Shared Function AreEquivalent(
            expected As UserPreferencesSnapshot,
            actual As UserPreferencesSnapshot) As Boolean

        Return expected.ShowMessages = actual.ShowMessages AndAlso
            String.Equals(expected.DefaultDirectory, actual.DefaultDirectory, StringComparison.Ordinal) AndAlso
            expected.UpdatePolicy = actual.UpdatePolicy AndAlso
            expected.LastUpdateCheck.Equals(actual.LastUpdateCheck) AndAlso
            expected.Legacy1xImportVersion = actual.Legacy1xImportVersion AndAlso
            String.Equals(
                expected.Legacy1xImportOutcome,
                actual.Legacy1xImportOutcome,
                StringComparison.Ordinal)
    End Function

    Private Shared Function IsLockContention(failure As IOException) As Boolean
        Dim nativeCode As Integer = Marshal.GetHRForException(failure) And &HFFFF
        Return nativeCode = 32 OrElse nativeCode = 33
    End Function

End Class

End Namespace
