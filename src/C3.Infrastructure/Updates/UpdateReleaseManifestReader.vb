Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Xml

Namespace Updates

    Public Enum UpdateManifestReadFailure
        None = 0
        Empty
        TooLarge
        MalformedJson
        UnsupportedSchema
        WrongProduct
        WrongChannel
        InvalidManifest
    End Enum

    Public NotInheritable Class UpdateManifestReadResult

        Private Sub New(
                manifest As UpdateReleaseManifest,
                failure As UpdateManifestReadFailure,
                failureMessage As String,
                failureException As Exception)

            Me.Manifest = manifest
            Me.Failure = failure
            Me.FailureMessage = If(failureMessage, String.Empty)
            Me.FailureException = failureException
        End Sub

        Public ReadOnly Property Manifest As UpdateReleaseManifest

        Public ReadOnly Property Failure As UpdateManifestReadFailure

        Public ReadOnly Property FailureMessage As String

        Public ReadOnly Property FailureException As Exception

        Public ReadOnly Property IsSuccess As Boolean
            Get
                Return Manifest IsNot Nothing
            End Get
        End Property

        Friend Shared Function Succeeded(
                manifest As UpdateReleaseManifest) As UpdateManifestReadResult

            Return New UpdateManifestReadResult(
                manifest,
                UpdateManifestReadFailure.None,
                Nothing,
                Nothing)
        End Function

        Friend Shared Function Failed(
                failure As UpdateManifestReadFailure,
                message As String,
                Optional failureException As Exception = Nothing) As UpdateManifestReadResult

            Return New UpdateManifestReadResult(Nothing, failure, message, failureException)
        End Function

    End Class

    ''' <summary>
    ''' Reads the small, versioned JSON document used by C3 2.x update channels.
    ''' Input size, JSON depth, string size, and object graph size are bounded.
    ''' </summary>
    Public NotInheritable Class UpdateReleaseManifestReader

        Public Const MaximumManifestBytes As Integer = 32 * 1024

        Private Const MaximumFieldCharacters As Integer = 512
        Private Const MaximumPackages As Integer = 16
        Private Const ExpectedProductName As String = "Compact Cassette Catalogue"
        Private Const ExpectedProductId As String = "c3"
        Private Const RepositoryUrl As String =
            "https://github.com/Julesc013/compact-cassette-catalogue"
        Private Const ChecksumFileName As String = "SHA256SUMS.txt"

        Public Function Read(
                payload As Byte(),
                expectedChannel As String) As UpdateManifestReadResult

            If payload Is Nothing OrElse payload.Length = 0 Then
                Return UpdateManifestReadResult.Failed(
                    UpdateManifestReadFailure.Empty,
                    "The update manifest is empty.")
            End If
            If payload.Length > MaximumManifestBytes Then
                Return UpdateManifestReadResult.Failed(
                    UpdateManifestReadFailure.TooLarge,
                    "The update manifest exceeds the 32 KiB safety limit.")
            End If
            If Not IsSupportedChannel(expectedChannel) Then
                Throw New ArgumentException("The expected update channel is invalid.", "expectedChannel")
            End If

            Dim document As UpdateReleaseManifestDocument
            Try
                Dim shapeFailure As String = ValidatePropertyShape(payload)
                If shapeFailure IsNot Nothing Then
                    Return Invalid(shapeFailure)
                End If

                Dim quotas As New XmlDictionaryReaderQuotas()
                quotas.MaxDepth = 8
                quotas.MaxStringContentLength = MaximumFieldCharacters
                quotas.MaxArrayLength = MaximumPackages
                quotas.MaxBytesPerRead = 4096
                quotas.MaxNameTableCharCount = 2048

                Dim serializer As New DataContractJsonSerializer(
                    GetType(UpdateReleaseManifestDocument))
                Dim utf8 As New UTF8Encoding(False, True)
                Using jsonReader As XmlDictionaryReader =
                        JsonReaderWriterFactory.CreateJsonReader(
                            payload,
                            0,
                            payload.Length,
                            utf8,
                            quotas,
                            Nothing)

                    document = DirectCast(
                        serializer.ReadObject(jsonReader, True),
                        UpdateReleaseManifestDocument)
                End Using
            Catch ex As SerializationException
                Return Malformed(ex)
            Catch ex As XmlException
                Return Malformed(ex)
            Catch ex As DecoderFallbackException
                Return Malformed(ex)
            Catch ex As ArgumentException
                Return Malformed(ex)
            End Try

            Return Validate(document, expectedChannel)
        End Function

        ''' <summary>
        ''' Validates the JSON token shape before DataContract deserialization.
        ''' DataContractJsonSerializer can coerce some primitive JSON values, so
        ''' the wire contract is checked here while the original token types are
        ''' still available from the JSON reader.
        ''' </summary>
        Private Shared Function ValidatePropertyShape(payload As Byte()) As String
            Dim quotas As New XmlDictionaryReaderQuotas()
            quotas.MaxDepth = 8
            quotas.MaxStringContentLength = MaximumFieldCharacters
            quotas.MaxArrayLength = MaximumPackages
            quotas.MaxBytesPerRead = 4096
            quotas.MaxNameTableCharCount = 2048

            Dim utf8 As New UTF8Encoding(False, True)
            Dim rootProperties As New HashSet(Of String)(StringComparer.Ordinal)
            Dim checksumProperties As HashSet(Of String) = Nothing
            Dim packageProperties As HashSet(Of String) = Nothing
            Dim levelOneName As String = Nothing
            Dim levelTwoName As String = Nothing
            Dim sawRoot As Boolean = False
            Using reader As XmlDictionaryReader =
                    JsonReaderWriterFactory.CreateJsonReader(
                        payload,
                        0,
                        payload.Length,
                        utf8,
                        quotas,
                        Nothing)

                While reader.Read()
                    If reader.NodeType = XmlNodeType.Element Then
                        Select Case reader.Depth
                            Case 0
                                If sawRoot OrElse reader.LocalName <> "root" OrElse
                                        Not HasJsonType(reader, "object") Then
                                    Return "The update manifest JSON root is invalid."
                                End If
                                sawRoot = True
                            Case 1
                                levelOneName = reader.LocalName
                                If Not IsRootProperty(levelOneName) Then
                                    Return "The update manifest contains an unsupported property."
                                End If
                                If Not rootProperties.Add(levelOneName) Then
                                    Throw New SerializationException(
                                        "The update manifest contains a duplicate property.")
                                End If

                                Dim rootTypeFailure As String = ValidateRootPropertyType(reader)
                                If rootTypeFailure IsNot Nothing Then
                                    Return rootTypeFailure
                                End If
                                If levelOneName = "checksumManifest" AndAlso
                                        HasJsonType(reader, "object") Then
                                    checksumProperties = New HashSet(Of String)(
                                        StringComparer.Ordinal)
                                End If
                            Case 2
                                levelTwoName = reader.LocalName
                                If levelOneName = "checksumManifest" Then
                                    If checksumProperties Is Nothing Then
                                        Return "The checksum manifest container is invalid."
                                    End If
                                    If Not IsChecksumProperty(levelTwoName) Then
                                        Return "The checksum manifest contains an unsupported property."
                                    End If
                                    If Not checksumProperties.Add(levelTwoName) Then
                                        Throw New SerializationException(
                                            "The checksum manifest contains a duplicate property.")
                                    End If

                                    Dim checksumTypeFailure As String =
                                        ValidateChecksumPropertyType(reader)
                                    If checksumTypeFailure IsNot Nothing Then
                                        Return checksumTypeFailure
                                    End If
                                ElseIf levelOneName = "packages" Then
                                    If levelTwoName <> "item" OrElse
                                            Not HasJsonType(reader, "object") Then
                                        Return "The update manifest package array is invalid."
                                    End If
                                    packageProperties = New HashSet(Of String)(
                                        StringComparer.Ordinal)
                                Else
                                    Return "The update manifest contains unsupported nested data."
                                End If
                            Case 3
                                If levelOneName <> "packages" OrElse
                                        levelTwoName <> "item" OrElse
                                        Not IsPackageProperty(reader.LocalName) Then
                                    Return "An update package contains an unsupported property."
                                End If
                                If packageProperties Is Nothing Then
                                    Return "The update manifest package container is invalid."
                                End If
                                If Not packageProperties.Add(reader.LocalName) Then
                                    Throw New SerializationException(
                                        "An update package contains a duplicate property.")
                                End If

                                Dim packageTypeFailure As String =
                                    ValidatePackagePropertyType(reader)
                                If packageTypeFailure IsNot Nothing Then
                                    Return packageTypeFailure
                                End If
                            Case Else
                                Return "The update manifest exceeds the supported object shape."
                        End Select
                    ElseIf reader.NodeType = XmlNodeType.EndElement Then
                        If reader.Depth = 2 Then
                            If levelOneName = "packages" AndAlso levelTwoName = "item" Then
                                If packageProperties Is Nothing OrElse
                                        packageProperties.Count <> 6 Then
                                    Throw New SerializationException(
                                        "An update package is missing a required property.")
                                End If
                                packageProperties = Nothing
                            End If
                            levelTwoName = Nothing
                        ElseIf reader.Depth = 1 Then
                            If levelOneName = "checksumManifest" AndAlso
                                    checksumProperties IsNot Nothing AndAlso
                                    checksumProperties.Count <> 4 Then
                                Throw New SerializationException(
                                    "The checksum manifest is missing a required property.")
                            End If
                            checksumProperties = Nothing
                            levelOneName = Nothing
                        End If
                    End If
                End While
            End Using

            If Not sawRoot OrElse rootProperties.Count <> 13 Then
                Throw New SerializationException(
                    "The update manifest is missing a required property.")
            End If
            Return Nothing
        End Function

        Private Shared Function ValidateRootPropertyType(
                reader As XmlDictionaryReader) As String

            Select Case reader.LocalName
                Case "schemaVersion"
                    If Not HasJsonType(reader, "number") Then
                        Return "The schemaVersion field must be a JSON number."
                    End If
                Case "product",
                        "productId",
                        "channel",
                        "version",
                        "stage",
                        "informationalVersion",
                        "releaseDate",
                        "catalogueWriteFormat"
                    If Not HasJsonType(reader, "string") Then
                        Return "An update manifest text field has the wrong JSON type."
                    End If
                Case "published"
                    If Not HasJsonType(reader, "boolean") Then
                        Return "The published field must be a JSON Boolean."
                    End If
                Case "releaseUrl"
                    If Not HasJsonType(reader, "string", "null") Then
                        Return "The releaseUrl field must be a JSON string or null."
                    End If
                Case "checksumManifest"
                    If Not HasJsonType(reader, "object", "null") Then
                        Return "The checksumManifest field must be a JSON object or null."
                    End If
                Case "packages"
                    If Not HasJsonType(reader, "array") Then
                        Return "The packages field must be a JSON array."
                    End If
            End Select
            Return Nothing
        End Function

        Private Shared Function ValidateChecksumPropertyType(
                reader As XmlDictionaryReader) As String

            If reader.LocalName = "length" Then
                If Not HasJsonType(reader, "number") Then
                    Return "The checksum manifest length must be a JSON number."
                End If
            ElseIf Not HasJsonType(reader, "string") Then
                Return "A checksum manifest text field has the wrong JSON type."
            End If
            Return Nothing
        End Function

        Private Shared Function ValidatePackagePropertyType(
                reader As XmlDictionaryReader) As String

            If reader.LocalName = "length" Then
                If Not HasJsonType(reader, "number") Then
                    Return "An update package length must be a JSON number."
                End If
            ElseIf Not HasJsonType(reader, "string") Then
                Return "An update package text field has the wrong JSON type."
            End If
            Return Nothing
        End Function

        Private Shared Function HasJsonType(
                reader As XmlDictionaryReader,
                ParamArray expectedTypes As String()) As Boolean

            Dim actualType As String = reader.GetAttribute("type")
            For Each expectedType As String In expectedTypes
                If actualType = expectedType Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Shared Function IsRootProperty(name As String) As Boolean
            Select Case name
                Case "schemaVersion",
                        "product",
                        "productId",
                        "channel",
                        "version",
                        "stage",
                        "informationalVersion",
                        "releaseDate",
                        "catalogueWriteFormat",
                        "published",
                        "releaseUrl",
                        "checksumManifest",
                        "packages"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Shared Function IsChecksumProperty(name As String) As Boolean
            Return name = "file" OrElse
                name = "length" OrElse
                name = "sha256" OrElse
                name = "url"
        End Function

        Private Shared Function IsPackageProperty(name As String) As Boolean
            Return name = "lane" OrElse
                name = "distribution" OrElse
                name = "file" OrElse
                name = "length" OrElse
                name = "sha256" OrElse
                name = "url"
        End Function

        Private Shared Function Validate(
                document As UpdateReleaseManifestDocument,
                expectedChannel As String) As UpdateManifestReadResult

            If document Is Nothing Then
                Return Invalid("The update manifest does not contain a JSON object.")
            End If
            If document.SchemaVersion <> 1 Then
                Return UpdateManifestReadResult.Failed(
                    UpdateManifestReadFailure.UnsupportedSchema,
                    "The update manifest schema is not supported.")
            End If
            If document.Product <> ExpectedProductName OrElse
                    document.ProductId <> ExpectedProductId Then
                Return UpdateManifestReadResult.Failed(
                    UpdateManifestReadFailure.WrongProduct,
                    "The update manifest belongs to a different product.")
            End If
            If document.Channel <> expectedChannel Then
                Return UpdateManifestReadResult.Failed(
                    UpdateManifestReadFailure.WrongChannel,
                    "The update manifest belongs to a different channel.")
            End If
            If document.Channel = "alpha" AndAlso document.Published Then
                Return Invalid("Alpha update manifests must remain unpublished.")
            End If
            If Not FieldsAreBounded(document) Then
                Return Invalid("The update manifest contains a missing or oversized field.")
            End If

            Dim productIdentity As SemanticVersion = Nothing
            If Not SemanticVersion.TryParse(document.Version, productIdentity) OrElse
                    productIdentity.HasPrerelease OrElse
                    productIdentity.ReleaseLabel <> productIdentity.CoreVersion OrElse
                    document.Version <> productIdentity.CoreVersion Then
                Return Invalid("The update manifest product version is invalid.")
            End If

            Dim releaseIdentity As SemanticVersion = Nothing
            If Not SemanticVersion.TryParse(document.InformationalVersion, releaseIdentity) OrElse
                    releaseIdentity.CoreVersion <> document.Version Then
                Return Invalid("The update manifest release identity is invalid.")
            End If

            Dim expectedPrerelease As String = Nothing
            Dim stageFamily As String = Nothing
            If Not TryGetStageIdentity(document.Stage, expectedPrerelease, stageFamily) Then
                Return Invalid("The update manifest stage is invalid.")
            End If

            Dim expectedReleaseLabel As String = document.Version
            If expectedPrerelease IsNot Nothing Then
                expectedReleaseLabel &= "-" & expectedPrerelease
            End If
            If releaseIdentity.ReleaseLabel <> expectedReleaseLabel Then
                Return Invalid("The update manifest stage and release identity disagree.")
            End If
            If Not ChannelAcceptsStage(document.Channel, stageFamily) Then
                Return Invalid("The update manifest stage does not belong to its channel.")
            End If

            Dim releaseDate As DateTime
            If Not DateTime.TryParseExact(
                    document.ReleaseDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    releaseDate) Then
                Return Invalid("The update manifest release date is invalid.")
            End If

            Dim catalogueIdentity As SemanticVersion = Nothing
            If Not SemanticVersion.TryParse(
                    document.CatalogueWriteFormat,
                    catalogueIdentity) OrElse
                    catalogueIdentity.HasPrerelease OrElse
                    catalogueIdentity.ReleaseLabel <> catalogueIdentity.CoreVersion OrElse
                    document.CatalogueWriteFormat <> catalogueIdentity.CoreVersion Then
                Return Invalid("The update manifest catalogue format is invalid.")
            End If

            Dim releaseUrl As String = Nothing
            Dim checksumManifest As UpdateChecksumManifest = Nothing
            Dim packages As IList(Of UpdateReleasePackage) = Nothing
            Dim publicationFailure As String = ValidatePublication(
                document,
                releaseIdentity,
                releaseUrl,
                checksumManifest,
                packages)
            If publicationFailure IsNot Nothing Then
                Return Invalid(publicationFailure)
            End If

            Return UpdateManifestReadResult.Succeeded(
                New UpdateReleaseManifest(
                    document.Channel,
                    document.Version,
                    document.Stage,
                    document.InformationalVersion,
                    DateTime.SpecifyKind(releaseDate, DateTimeKind.Utc),
                    document.CatalogueWriteFormat,
                    document.Published,
                    releaseUrl,
                    checksumManifest,
                    packages,
                    releaseIdentity))
        End Function

        Private Shared Function FieldsAreBounded(
                document As UpdateReleaseManifestDocument) As Boolean

            Return IsBounded(document.Product) AndAlso
                IsBounded(document.ProductId) AndAlso
                IsBounded(document.Channel) AndAlso
                IsBounded(document.Version) AndAlso
                IsBounded(document.Stage) AndAlso
                IsBounded(document.InformationalVersion) AndAlso
                IsBounded(document.ReleaseDate) AndAlso
                IsBounded(document.CatalogueWriteFormat)
        End Function

        Private Shared Function IsBounded(value As String) As Boolean
            Return value IsNot Nothing AndAlso value.Length > 0 AndAlso
                value.Length <= MaximumFieldCharacters AndAlso value = value.Trim()
        End Function

        Private Shared Function ValidatePublication(
                document As UpdateReleaseManifestDocument,
                releaseIdentity As SemanticVersion,
                ByRef releaseUrl As String,
                ByRef checksumManifest As UpdateChecksumManifest,
                ByRef packages As IList(Of UpdateReleasePackage)) As String

            releaseUrl = Nothing
            checksumManifest = Nothing
            packages = Nothing

            If document.Packages Is Nothing Then
                Return "The update manifest packages field must be an array."
            End If

            If Not document.Published Then
                If document.ReleaseUrl IsNot Nothing OrElse
                        document.ChecksumManifest IsNot Nothing OrElse
                        document.Packages.Length <> 0 Then
                    Return "An unpublished update manifest must not identify release assets."
                End If
                packages = New List(Of UpdateReleasePackage)()
                Return Nothing
            End If

            If Not IsBounded(document.ReleaseUrl) Then
                Return "A published update manifest must identify its release URL."
            End If

            Dim tagName As String = "v" & releaseIdentity.ReleaseLabel
            Dim expectedReleaseUrl As String = RepositoryUrl & "/releases/tag/" & tagName
            If document.ReleaseUrl <> expectedReleaseUrl Then
                Return "The published release URL does not match the release identity."
            End If

            If document.ChecksumManifest Is Nothing Then
                Return "A published update manifest must identify SHA256SUMS.txt."
            End If
            Dim assetRoot As String = RepositoryUrl & "/releases/download/" & tagName & "/"
            If document.ChecksumManifest.FileName <> ChecksumFileName OrElse
                    document.ChecksumManifest.Url <> assetRoot & ChecksumFileName Then
                Return "The checksum manifest identity or URL is invalid."
            End If
            If document.ChecksumManifest.Length <= 0L Then
                Return "The checksum manifest length must be positive."
            End If
            If Not IsLowercaseSha256(document.ChecksumManifest.Sha256) Then
                Return "The checksum manifest SHA-256 value is invalid."
            End If

            If document.Packages.Length = 0 OrElse
                    document.Packages.Length > MaximumPackages Then
                Return "A published update manifest must identify between one and 16 packages."
            End If

            Dim observedLanes As New HashSet(Of String)(StringComparer.Ordinal)
            Dim observedFiles As New HashSet(Of String)(StringComparer.Ordinal)
            Dim validatedPackages As New List(Of UpdateReleasePackage)()
            For Each package As UpdateReleasePackageDocument In document.Packages
                If package Is Nothing Then
                    Return "A published update manifest contains an empty package entry."
                End If
                If Not IsValidLane(package.Lane) Then
                    Return "An update package lane is invalid."
                End If
                If Not observedLanes.Add(package.Lane) Then
                    Return "The update manifest contains a duplicate package lane."
                End If
                If package.Distribution <> "portable" Then
                    Return "An update package distribution is invalid."
                End If

                Dim expectedFileName As String = "C3-v" &
                    releaseIdentity.ReleaseLabel & "-" & package.Lane & "-portable.zip"
                If package.FileName <> expectedFileName Then
                    Return "An update package filename does not match its release and lane."
                End If
                If Not observedFiles.Add(package.FileName) Then
                    Return "The update manifest contains a duplicate package filename."
                End If
                If package.Length <= 0L Then
                    Return "An update package length must be positive."
                End If
                If Not IsLowercaseSha256(package.Sha256) Then
                    Return "An update package SHA-256 value is invalid."
                End If

                Dim expectedPackageUrl As String = assetRoot & package.FileName
                If package.Url <> expectedPackageUrl Then
                    Return "An update package URL does not match its release asset."
                End If

                validatedPackages.Add(
                    New UpdateReleasePackage(
                        package.Lane,
                        package.Distribution,
                        package.FileName,
                        package.Length,
                        package.Sha256,
                        package.Url))
            Next

            releaseUrl = document.ReleaseUrl
            checksumManifest = New UpdateChecksumManifest(
                document.ChecksumManifest.FileName,
                document.ChecksumManifest.Length,
                document.ChecksumManifest.Sha256,
                document.ChecksumManifest.Url)
            packages = validatedPackages
            Return Nothing
        End Function

        Private Shared Function IsValidLane(value As String) As Boolean
            If value Is Nothing OrElse value.Length = 0 OrElse value.Length > 64 Then
                Return False
            End If
            For index As Integer = 0 To value.Length - 1
                Dim character As Char = value(index)
                Dim isLowercaseLetter As Boolean = character >= "a"c AndAlso character <= "z"c
                Dim isDigit As Boolean = character >= "0"c AndAlso character <= "9"c
                If Not isLowercaseLetter AndAlso Not isDigit AndAlso
                        (index = 0 OrElse (character <> "."c AndAlso character <> "-"c)) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Shared Function IsLowercaseSha256(value As String) As Boolean
            If value Is Nothing OrElse value.Length <> 64 Then
                Return False
            End If
            For Each character As Char In value
                If Not (character >= "0"c AndAlso character <= "9"c) AndAlso
                        Not (character >= "a"c AndAlso character <= "f"c) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Shared Function TryGetStageIdentity(
                stage As String,
                ByRef prerelease As String,
                ByRef family As String) As Boolean

            prerelease = Nothing
            family = Nothing
            If stage = "Release" Then
                family = "stable"
                Return True
            End If

            Dim separatorIndex As Integer = stage.LastIndexOf(" "c)
            If separatorIndex <= 0 OrElse separatorIndex = stage.Length - 1 Then
                Return False
            End If
            Dim stageName As String = stage.Substring(0, separatorIndex)
            Dim sequence As String = stage.Substring(separatorIndex + 1)
            If Not IsPositiveCanonicalInteger(sequence) Then
                Return False
            End If

            Select Case stageName
                Case "Alpha"
                    family = "alpha"
                    prerelease = "alpha." & sequence
                Case "Beta"
                    family = "beta"
                    prerelease = "beta." & sequence
                Case "Release Candidate"
                    family = "rc"
                    prerelease = "rc." & sequence
                Case Else
                    Return False
            End Select
            Return True
        End Function

        Private Shared Function IsPositiveCanonicalInteger(value As String) As Boolean
            If value.Length = 0 OrElse value(0) = "0"c Then
                Return False
            End If
            For Each character As Char In value
                If character < "0"c OrElse character > "9"c Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Shared Function ChannelAcceptsStage(
                channel As String,
                stageFamily As String) As Boolean

            Select Case channel
                Case "alpha"
                    Return stageFamily = "alpha"
                Case "beta"
                    Return stageFamily = "beta" OrElse stageFamily = "rc"
                Case "stable"
                    Return stageFamily = "stable"
                Case Else
                    Return False
            End Select
        End Function

        Private Shared Function IsSupportedChannel(channel As String) As Boolean
            Return channel = "alpha" OrElse channel = "beta" OrElse channel = "stable"
        End Function

        Private Shared Function Malformed(
                exception As Exception) As UpdateManifestReadResult

            Return UpdateManifestReadResult.Failed(
                UpdateManifestReadFailure.MalformedJson,
                "The update manifest is not safe, well-formed JSON.",
                exception)
        End Function

        Private Shared Function Invalid(message As String) As UpdateManifestReadResult
            Return UpdateManifestReadResult.Failed(
                UpdateManifestReadFailure.InvalidManifest,
                message)
        End Function

    End Class

    <DataContract>
    Friend NotInheritable Class UpdateReleaseManifestDocument

        <DataMember(Name:="schemaVersion", IsRequired:=True, Order:=1)>
        Public Property SchemaVersion As Integer

        <DataMember(Name:="product", IsRequired:=True, Order:=2)>
        Public Property Product As String

        <DataMember(Name:="productId", IsRequired:=True, Order:=3)>
        Public Property ProductId As String

        <DataMember(Name:="channel", IsRequired:=True, Order:=4)>
        Public Property Channel As String

        <DataMember(Name:="version", IsRequired:=True, Order:=5)>
        Public Property Version As String

        <DataMember(Name:="stage", IsRequired:=True, Order:=6)>
        Public Property Stage As String

        <DataMember(Name:="informationalVersion", IsRequired:=True, Order:=7)>
        Public Property InformationalVersion As String

        <DataMember(Name:="releaseDate", IsRequired:=True, Order:=8)>
        Public Property ReleaseDate As String

        <DataMember(Name:="catalogueWriteFormat", IsRequired:=True, Order:=9)>
        Public Property CatalogueWriteFormat As String

        <DataMember(Name:="published", IsRequired:=True, Order:=10)>
        Public Property Published As Boolean

        <DataMember(Name:="releaseUrl", IsRequired:=True, Order:=11)>
        Public Property ReleaseUrl As String

        <DataMember(Name:="checksumManifest", IsRequired:=True, Order:=12)>
        Public Property ChecksumManifest As UpdateChecksumManifestDocument

        <DataMember(Name:="packages", IsRequired:=True, Order:=13)>
        Public Property Packages As UpdateReleasePackageDocument()

    End Class

    <DataContract>
    Friend NotInheritable Class UpdateChecksumManifestDocument

        <DataMember(Name:="file", IsRequired:=True, Order:=1)>
        Public Property FileName As String

        <DataMember(Name:="length", IsRequired:=True, Order:=2)>
        Public Property Length As Long

        <DataMember(Name:="sha256", IsRequired:=True, Order:=3)>
        Public Property Sha256 As String

        <DataMember(Name:="url", IsRequired:=True, Order:=4)>
        Public Property Url As String

    End Class

    <DataContract>
    Friend NotInheritable Class UpdateReleasePackageDocument

        <DataMember(Name:="lane", IsRequired:=True, Order:=1)>
        Public Property Lane As String

        <DataMember(Name:="distribution", IsRequired:=True, Order:=2)>
        Public Property Distribution As String

        <DataMember(Name:="file", IsRequired:=True, Order:=3)>
        Public Property FileName As String

        <DataMember(Name:="length", IsRequired:=True, Order:=4)>
        Public Property Length As Long

        <DataMember(Name:="sha256", IsRequired:=True, Order:=5)>
        Public Property Sha256 As String

        <DataMember(Name:="url", IsRequired:=True, Order:=6)>
        Public Property Url As String

    End Class

End Namespace
