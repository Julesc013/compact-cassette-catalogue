Namespace Updates

    Public NotInheritable Class UpdateChecksumManifest

        Friend Sub New(
                fileName As String,
                length As Long,
                sha256 As String,
                url As String)

            Me.FileName = fileName
            Me.Length = length
            Me.Sha256 = sha256
            Me.Url = url
        End Sub

        Public ReadOnly Property FileName As String

        Public ReadOnly Property Length As Long

        Public ReadOnly Property Sha256 As String

        Public ReadOnly Property Url As String

    End Class

    Public NotInheritable Class UpdateReleasePackage

        Friend Sub New(
                lane As String,
                distribution As String,
                fileName As String,
                length As Long,
                sha256 As String,
                url As String)

            Me.Lane = lane
            Me.Distribution = distribution
            Me.FileName = fileName
            Me.Length = length
            Me.Sha256 = sha256
            Me.Url = url
        End Sub

        Public ReadOnly Property Lane As String

        Public ReadOnly Property Distribution As String

        Public ReadOnly Property FileName As String

        Public ReadOnly Property Length As Long

        Public ReadOnly Property Sha256 As String

        Public ReadOnly Property Url As String

    End Class

    ''' <summary>
    ''' A validated, channel-specific C3 release-feed document.
    ''' </summary>
    Public NotInheritable Class UpdateReleaseManifest

        Friend Sub New(
                channel As String,
                productVersion As String,
                stage As String,
                informationalVersion As String,
                releaseDate As DateTime,
                catalogueWriteFormat As String,
                published As Boolean,
                releaseUrl As String,
                checksumManifest As UpdateChecksumManifest,
                packages As IList(Of UpdateReleasePackage),
                releaseIdentity As SemanticVersion)

            If packages Is Nothing Then
                Throw New ArgumentNullException("packages")
            End If
            Me.Channel = channel
            Me.ProductVersion = productVersion
            Me.Stage = stage
            Me.InformationalVersion = informationalVersion
            Me.ReleaseDate = releaseDate
            Me.CatalogueWriteFormat = catalogueWriteFormat
            Me.Published = published
            Me.ReleaseUrl = releaseUrl
            Me.ChecksumManifest = checksumManifest
            Me.Packages = New List(Of UpdateReleasePackage)(packages).AsReadOnly()
            Me.ReleaseIdentity = releaseIdentity
        End Sub

        Public ReadOnly Property Channel As String

        Public ReadOnly Property ProductVersion As String

        Public ReadOnly Property Stage As String

        Public ReadOnly Property InformationalVersion As String

        Public ReadOnly Property ReleaseDate As DateTime

        Public ReadOnly Property CatalogueWriteFormat As String

        Public ReadOnly Property Published As Boolean

        Public ReadOnly Property ReleaseUrl As String

        Public ReadOnly Property ChecksumManifest As UpdateChecksumManifest

        Public ReadOnly Property Packages As IList(Of UpdateReleasePackage)

        Public ReadOnly Property ReleaseIdentity As SemanticVersion

    End Class

End Namespace
