Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Xml
Imports C3.Infrastructure.Updates

Namespace Preferences

    ''' <summary>
    ''' Reads only the scalar settings C3 has explicitly promised to migrate.
    ''' The source stream is opened read-only and is never changed or deleted.
    ''' </summary>
    Public NotInheritable Class LegacySettingsProfileReader

        Public Const MaximumProfileBytes As Long = 256L * 1024L

        Private Const SettingsSectionXPath As String =
            "/configuration/userSettings/Compact_Cassette_Catalogue.My.MySettings"

        Public Function Read(
                candidate As LegacySettingsProfileCandidate) As LegacySettingsProfileReadResult

            If candidate Is Nothing Then
                Throw New ArgumentNullException("candidate")
            End If

            Try
                Dim document As New XmlDocument()
                document.XmlResolver = Nothing

                Using stream As New FileStream(
                        candidate.FilePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read)

                    If stream.Length > MaximumProfileBytes Then
                        Return LegacySettingsProfileReadResult.Failed(
                            candidate,
                            LegacySettingsProfileReadFailure.TooLarge,
                            "The legacy settings profile exceeds the 256 KiB safety limit.")
                    End If

                    Using reader As XmlReader = XmlReader.Create(stream, CreateSecureReaderSettings())
                        document.Load(reader)
                    End Using
                End Using

                Return ParseDocument(candidate, document)
            Catch ex As XmlException
                Return LegacySettingsProfileReadResult.Failed(
                    candidate,
                    LegacySettingsProfileReadFailure.MalformedXml,
                    "The legacy settings profile is not safe, well-formed XML.",
                    ex)
            Catch ex As IOException
                Return LegacySettingsProfileReadResult.Failed(
                    candidate,
                    LegacySettingsProfileReadFailure.Unavailable,
                    "The legacy settings profile could not be read.",
                    ex)
            Catch ex As UnauthorizedAccessException
                Return LegacySettingsProfileReadResult.Failed(
                    candidate,
                    LegacySettingsProfileReadFailure.Unavailable,
                    "Access to the legacy settings profile was denied.",
                    ex)
            Catch ex As System.Security.SecurityException
                Return LegacySettingsProfileReadResult.Failed(
                    candidate,
                    LegacySettingsProfileReadFailure.Unavailable,
                    "The legacy settings profile was blocked by the security policy.",
                    ex)
            End Try
        End Function

        Private Shared Function CreateSecureReaderSettings() As XmlReaderSettings
            Dim settings As New XmlReaderSettings()
            settings.DtdProcessing = DtdProcessing.Prohibit
            settings.XmlResolver = Nothing
            settings.MaxCharactersInDocument = MaximumProfileBytes
            settings.MaxCharactersFromEntities = 0L
            settings.IgnoreComments = True
            settings.IgnoreProcessingInstructions = True
            Return settings
        End Function

        Private Shared Function ParseDocument(
                candidate As LegacySettingsProfileCandidate,
                document As XmlDocument) As LegacySettingsProfileReadResult

            If document.DocumentElement Is Nothing OrElse
                    document.DocumentElement.Name <> "configuration" OrElse
                    document.DocumentElement.NamespaceURI.Length <> 0 Then
                Return InvalidStructure(candidate, "The configuration root is not the legacy C3 root.")
            End If

            Dim userSettingsNodes As XmlNodeList = document.SelectNodes("/configuration/userSettings")
            Dim sectionNodes As XmlNodeList = document.SelectNodes(SettingsSectionXPath)
            If userSettingsNodes Is Nothing OrElse userSettingsNodes.Count <> 1 OrElse
                    sectionNodes Is Nothing OrElse sectionNodes.Count <> 1 Then
                Return InvalidStructure(
                    candidate,
                    "The exact C3 1.x user settings section was not found once.")
            End If

            Dim section As XmlNode = sectionNodes.Item(0)
            Dim observedNames As New HashSet(Of String)(StringComparer.Ordinal)
            Dim recognizedCount As Integer

            Dim hasShowMessages As Boolean
            Dim showMessages As Boolean
            Dim hasDefaultDirectory As Boolean
            Dim defaultDirectory As String = Nothing
            Dim hasUpdatePolicy As Boolean
            Dim updatePolicy As UpdateCheckPolicy = UpdateCheckPolicy.Never
            Dim hasLastUpdateCheck As Boolean
            Dim lastUpdateCheck As DateTime = DateTime.MinValue

            For Each child As XmlNode In section.ChildNodes
                If child.NodeType <> XmlNodeType.Element OrElse child.Name <> "setting" Then
                    Continue For
                End If

                Dim setting As XmlElement = DirectCast(child, XmlElement)
                Dim settingName As String = setting.GetAttribute("name")
                If Not IsKnownSetting(settingName) Then
                    Continue For
                End If

                If Not observedNames.Add(settingName) Then
                    Return LegacySettingsProfileReadResult.Failed(
                        candidate,
                        LegacySettingsProfileReadFailure.DuplicateSetting,
                        "The legacy profile contains a duplicate supported setting: " & settingName)
                End If

                If setting.GetAttribute("serializeAs") <> "String" Then
                    Return InvalidValue(
                        candidate,
                        "The legacy setting has an unsupported serialization mode: " & settingName)
                End If

                Dim values As XmlNodeList = setting.SelectNodes("value")
                If values Is Nothing OrElse values.Count <> 1 OrElse HasElementChild(values.Item(0)) Then
                    Return InvalidValue(
                        candidate,
                        "The legacy setting does not contain one scalar value: " & settingName)
                End If

                Dim rawValue As String = values.Item(0).InnerText
                Select Case settingName
                    Case "showMessages"
                        If Not Boolean.TryParse(rawValue.Trim(), showMessages) Then
                            Return InvalidValue(candidate, "showMessages is not a Boolean value.")
                        End If
                        hasShowMessages = True
                    Case "defaultDirectory"
                        If rawValue.Length >
                                UserPreferencesSnapshot.MaximumDefaultDirectoryCharacters Then
                            Return InvalidValue(
                                candidate,
                                "defaultDirectory exceeds the supported safety limit.")
                        End If
                        defaultDirectory = rawValue
                        hasDefaultDirectory = True
                    Case "checkUpdates"
                        If Not TryParseUpdatePolicy(rawValue, updatePolicy) Then
                            Return InvalidValue(candidate, "checkUpdates is not a supported policy value.")
                        End If
                        hasUpdatePolicy = True
                    Case "lastUpdateCheck"
                        If Not TryParseLastUpdateCheck(rawValue, lastUpdateCheck) Then
                            Return InvalidValue(candidate, "lastUpdateCheck is not a supported date value.")
                        End If
                        hasLastUpdateCheck = True
                End Select

                recognizedCount += 1
            Next

            If recognizedCount = 0 Then
                Return InvalidStructure(candidate, "The legacy section contains no supported C3 settings.")
            End If

            Return LegacySettingsProfileReadResult.Succeeded(
                New LegacyUserSettingsProfile(
                    candidate,
                    hasShowMessages,
                    showMessages,
                    hasDefaultDirectory,
                    defaultDirectory,
                    hasUpdatePolicy,
                    updatePolicy,
                    hasLastUpdateCheck,
                    lastUpdateCheck))
        End Function

        Private Shared Function IsKnownSetting(name As String) As Boolean
            Return name = "showMessages" OrElse
                name = "defaultDirectory" OrElse
                name = "checkUpdates" OrElse
                name = "lastUpdateCheck"
        End Function

        Private Shared Function HasElementChild(node As XmlNode) As Boolean
            For Each child As XmlNode In node.ChildNodes
                If child.NodeType = XmlNodeType.Element Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Shared Function TryParseUpdatePolicy(
                rawValue As String,
                ByRef updatePolicy As UpdateCheckPolicy) As Boolean

            Dim legacyBoolean As Boolean
            If Boolean.TryParse(If(rawValue, String.Empty).Trim(), legacyBoolean) Then
                updatePolicy = If(
                    legacyBoolean,
                    UpdateCheckPolicy.Startup,
                    UpdateCheckPolicy.Never)
                Return True
            End If

            Return UpdateCheckSchedule.TryParseStored(rawValue, updatePolicy)
        End Function

        Private Shared Function TryParseLastUpdateCheck(
                rawValue As String,
                ByRef lastUpdateCheck As DateTime) As Boolean

            Dim normalized As String = If(rawValue, String.Empty).Trim()
            If normalized.Length = 0 Then
                lastUpdateCheck = DateTime.MinValue
                Return True
            End If

            Return DateTime.TryParse(
                normalized,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces Or DateTimeStyles.RoundtripKind,
                lastUpdateCheck)
        End Function

        Private Shared Function InvalidStructure(
                candidate As LegacySettingsProfileCandidate,
                message As String) As LegacySettingsProfileReadResult

            Return LegacySettingsProfileReadResult.Failed(
                candidate,
                LegacySettingsProfileReadFailure.InvalidStructure,
                message)
        End Function

        Private Shared Function InvalidValue(
                candidate As LegacySettingsProfileCandidate,
                message As String) As LegacySettingsProfileReadResult

            Return LegacySettingsProfileReadResult.Failed(
                candidate,
                LegacySettingsProfileReadFailure.InvalidValue,
                message)
        End Function

    End Class

End Namespace
