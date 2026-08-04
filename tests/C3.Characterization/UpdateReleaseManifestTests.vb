Friend NotInheritable Class UpdateReleaseManifestTests

    Private Sub New()
    End Sub

    Public Shared Sub UnpublishedManifestNeverAdvertisesAvailability()
        Dim manifest As UpdateReleaseManifest = ReadManifest(
            ManifestJson(
                "alpha",
                "2.0.0",
                "Alpha 2",
                "2.0.0-alpha.2",
                False))

        Dim result As UpdateCheckResult = UpdateCheckService.Evaluate(
            "2.0.0-alpha.1",
            manifest)

        AssertEqual(True, result.IsSuccess, "unpublished result success")
        AssertEqual(False, result.IsUpdateAvailable, "unpublished availability")
        AssertEqual(
            UpdateCheckOutcome.NoPublishedRelease,
            result.Outcome,
            "unpublished outcome")
    End Sub

    Public Shared Sub ComparesCompletePrereleaseAndStableIdentity()
        AssertIdentityBefore("2.0.0-alpha.1", "2.0.0-alpha.2", "next alpha")
        AssertIdentityBefore("2.0.0-alpha.99", "2.0.0-beta.1", "beta after alpha")
        AssertIdentityBefore("2.0.0-beta.999", "2.0.0-rc.1", "release candidate after beta")
        AssertIdentityBefore("2.0.0-rc.999", "2.0.0", "stable after release candidate")
        AssertIdentityBefore("2.0.0", "2.0.1-alpha.1", "next patch prerelease")
        AssertIdentityBefore("2.0.0-alpha.2", "2.0.0-alpha.10", "numeric prerelease")
        AssertIdentityBefore("2.0.0-1", "2.0.0-alpha", "numeric before text")
        AssertIdentityBefore("2.0.0-alpha", "2.0.0-alpha.1", "prefix ordering")
        AssertIdentityBefore(
            "2.0.0-" & New String("9"c, 80),
            "2.0.0-1" & New String("0"c, 80),
            "huge numeric prerelease")
        AssertIdentityEqual(
            "2.0.0-alpha.2+local.1",
            "2.0.0-alpha.2+published.9",
            "build metadata does not affect precedence")

        AssertRejectedIdentity("2.0.0-alpha.01", "leading-zero numeric prerelease")
        AssertRejectedIdentity("2.0.0-alpha..1", "empty prerelease identifier")
        AssertRejectedIdentity("2.0.0+build..9", "empty build identifier")
        AssertRejectedIdentity("2.0.0-alpha_1", "invalid prerelease character")
        AssertRejectedIdentity("2.0.0+build/9", "invalid build character")
        AssertRejectedIdentity("02.0.0", "leading-zero core identifier")

        Dim maximumIdentity As String = "2.0.0+" &
            New String("a"c, SemanticVersion.MaximumTextCharacters - 6)
        AssertEqual(
            SemanticVersion.MaximumTextCharacters,
            maximumIdentity.Length,
            "maximum identity fixture length")
        AssertParsedIdentity(maximumIdentity, "128-character identity")
        AssertRejectedIdentity(maximumIdentity & "a", "129-character identity")

        ' Exercise the availability service only through publishable channels.
        ' Alpha manifests are deliberately and unconditionally unpublished.
        AssertAvailable("2.0.0-alpha.99", "Beta 1", "2.0.0-beta.1", "beta after alpha", "beta")
        AssertAvailable(
            "2.0.0-beta.999",
            "Release Candidate 1",
            "2.0.0-rc.1",
            "release candidate after beta",
            "beta")
        AssertAvailable(
            "2.0.0-rc.999",
            "Release",
            "2.0.0",
            "stable after release candidate",
            "stable")
        AssertAvailable(
            "2.0.0-beta.2",
            "Beta 10",
            "2.0.0-beta.10",
            "numeric beta precedence",
            "beta")
        AssertNotAvailable(
            "2.0.0-beta.2",
            "Beta 2",
            "2.0.0-beta.2",
            "same beta",
            "beta")
        AssertNotAvailable(
            "2.0.0",
            "Release Candidate 9",
            "2.0.0-rc.9",
            "stable is newer than release candidate",
            "beta")
        AssertNotAvailable(
            "2.0.0-beta.2+local.1",
            "Beta 2",
            "2.0.0-beta.2+published.9",
            "published build metadata does not affect precedence",
            "beta")
    End Sub

    Public Shared Sub RejectsUnsafeAndInconsistentJson()
        Dim reader As New UpdateReleaseManifestReader()

        Dim oversized(UpdateReleaseManifestReader.MaximumManifestBytes) As Byte
        AssertEqual(
            UpdateManifestReadFailure.TooLarge,
            reader.Read(oversized, "alpha").Failure,
            "oversized payload")

        AssertEqual(
            UpdateManifestReadFailure.MalformedJson,
            reader.Read(Encoding.UTF8.GetBytes("{""published"":"), "alpha").Failure,
            "malformed JSON")

        Dim missingPublication As String = ManifestJson(
            "alpha",
            "2.0.0",
            "Alpha 2",
            "2.0.0-alpha.2",
            True).Replace(",""published"":true", String.Empty)
        AssertEqual(
            UpdateManifestReadFailure.MalformedJson,
            reader.Read(Encoding.UTF8.GetBytes(missingPublication), "alpha").Failure,
            "missing publication state")

        Dim missingReleaseUrl As String = ManifestJson(
            "alpha",
            "2.0.0",
            "Alpha 2",
            "2.0.0-alpha.2",
            False).Replace("""releaseUrl"":null,", String.Empty)
        AssertEqual(
            UpdateManifestReadFailure.MalformedJson,
            reader.Read(Encoding.UTF8.GetBytes(missingReleaseUrl), "alpha").Failure,
            "missing release URL field")

        Dim duplicatePublication As String = ManifestJson(
            "alpha",
            "2.0.0",
            "Alpha 2",
            "2.0.0-alpha.2",
            False).Replace(
                """published"":false",
                """published"":false,""published"":true")
        AssertEqual(
            UpdateManifestReadFailure.MalformedJson,
            reader.Read(Encoding.UTF8.GetBytes(duplicatePublication), "alpha").Failure,
            "duplicate publication state")

        AssertEqual(
            UpdateManifestReadFailure.MalformedJson,
            reader.Read(New Byte() {&H7B, &HFF, &H7D}, "alpha").Failure,
            "invalid UTF-8")

        AssertEqual(
            UpdateManifestReadFailure.WrongChannel,
            reader.Read(
                Encoding.UTF8.GetBytes(
                    ManifestJson(
                        "beta",
                        "2.0.0",
                        "Beta 1",
                        "2.0.0-beta.1",
                        True)),
                "alpha").Failure,
            "cross-channel manifest")

        Dim foreignProduct As String = ManifestJson(
            "alpha",
            "2.0.0",
            "Alpha 2",
            "2.0.0-alpha.2",
            False).Replace("""productId"":""c3""", """productId"":""other""")
        AssertEqual(
            UpdateManifestReadFailure.WrongProduct,
            reader.Read(Encoding.UTF8.GetBytes(foreignProduct), "alpha").Failure,
            "foreign product manifest")

        AssertEqual(
            UpdateManifestReadFailure.InvalidManifest,
            reader.Read(
                Encoding.UTF8.GetBytes(
                    ManifestJson(
                        "alpha",
                        "2.0.0",
                        "Alpha 2",
                        "2.0.0-alpha.3",
                        False)),
                "alpha").Failure,
            "stage and informational identity mismatch")

        AssertEqual(
            UpdateManifestReadFailure.InvalidManifest,
            reader.Read(
                Encoding.UTF8.GetBytes(
                    ManifestJson(
                        "alpha",
                        "2.0.0",
                        "Beta 1",
                        "2.0.0-beta.1",
                        False)),
                "alpha").Failure,
            "stage outside channel")

        Dim qualifiedCatalogueFormat As String = ManifestJson(
            "alpha",
            "2.0.0",
            "Alpha 2",
            "2.0.0-alpha.2",
            False).Replace(
                """catalogueWriteFormat"":""1.1.0""",
                """catalogueWriteFormat"":""1.1.0+local""")
        AssertEqual(
            UpdateManifestReadFailure.InvalidManifest,
            reader.Read(Encoding.UTF8.GetBytes(qualifiedCatalogueFormat), "alpha").Failure,
            "catalogue writer requires a canonical numeric identity")
    End Sub

    Public Shared Sub RejectsWrongJsonTypesAndAlphaPublication()
        Const releaseLabel As String = "2.0.0-beta.1"
        Dim validBeta As String = ManifestJson(
            "beta",
            "2.0.0",
            "Beta 1",
            releaseLabel,
            True)

        AssertInvalidManifest("[]", "array JSON root", "beta")
        AssertWrongType(
            validBeta,
            """schemaVersion"":1",
            """schemaVersion"":""1""",
            "schema version string")
        AssertWrongType(
            validBeta,
            """product"":""Compact Cassette Catalogue""",
            """product"":1",
            "product number")
        AssertWrongType(
            validBeta,
            """productId"":""c3""",
            """productId"":{}",
            "product ID object")
        AssertWrongType(
            validBeta,
            """channel"":""beta""",
            """channel"":[]",
            "channel array")
        AssertWrongType(
            validBeta,
            """version"":""2.0.0""",
            """version"":2",
            "version number")
        AssertWrongType(
            validBeta,
            """stage"":""Beta 1""",
            """stage"":true",
            "stage Boolean")
        AssertWrongType(
            validBeta,
            """informationalVersion"":""2.0.0-beta.1""",
            """informationalVersion"":null",
            "informational version null")
        AssertWrongType(
            validBeta,
            """releaseDate"":""2026-08-04""",
            """releaseDate"":{}",
            "release date object")
        AssertWrongType(
            validBeta,
            """catalogueWriteFormat"":""1.1.0""",
            """catalogueWriteFormat"":[]",
            "catalogue format array")
        AssertWrongType(
            validBeta,
            """published"":true",
            """published"":1",
            "published number")

        Dim releaseUrl As String =
            "https://github.com/Julesc013/compact-cassette-catalogue/releases/tag/v" &
            releaseLabel
        AssertWrongType(
            validBeta,
            """releaseUrl"":""" & releaseUrl & """",
            """releaseUrl"":42",
            "release URL number")
        AssertWrongType(
            validBeta,
            """checksumManifest"":" & ChecksumJson(releaseLabel),
            """checksumManifest"":[]",
            "checksum array container")
        AssertWrongType(
            validBeta,
            """packages"":" & PackagesJson(releaseLabel),
            """packages"":{}",
            "packages object container")

        AssertWrongType(
            validBeta,
            """file"":""SHA256SUMS.txt""",
            """file"":9",
            "checksum filename number")
        AssertWrongType(
            validBeta,
            """length"":45",
            """length"":""45""",
            "checksum length string")
        AssertWrongType(
            validBeta,
            """sha256"":""" & New String("b"c, 64) & """",
            """sha256"":false",
            "checksum hash Boolean")
        AssertWrongType(
            validBeta,
            """url"":""" & AssetRoot(releaseLabel) & "SHA256SUMS.txt""",
            """url"":{}",
            "checksum URL object")

        AssertWrongType(
            validBeta,
            """lane"":""win-x86-net40""",
            """lane"":1",
            "package lane number")
        AssertWrongType(
            validBeta,
            """distribution"":""portable""",
            """distribution"":false",
            "package distribution Boolean")
        AssertWrongType(
            validBeta,
            """file"":""C3-v" & releaseLabel & "-win-x86-net40-portable.zip""",
            """file"":[]",
            "package filename array")
        AssertWrongType(
            validBeta,
            """length"":123",
            """length"":{}",
            "package length object")
        AssertWrongType(
            validBeta,
            """sha256"":""" & New String("a"c, 64) & """",
            """sha256"":null",
            "package hash null")
        AssertWrongType(
            validBeta,
            """url"":""" & AssetRoot(releaseLabel) &
                "C3-v" & releaseLabel & "-win-x86-net40-portable.zip""",
            """url"":17",
            "package URL number")
        AssertWrongType(
            validBeta,
            """packages"":" & PackagesJson(releaseLabel),
            """packages"":[null]",
            "null package item")

        AssertMalformedMutation(
            validBeta,
            """checksumManifest"":{""file"":""SHA256SUMS.txt""",
            """checksumManifest"":{""file"":""SHA256SUMS.txt""," &
                """file"":""SHA256SUMS.txt""",
            "duplicate checksum property")
        AssertMalformedMutation(
            validBeta,
            """packages"":[{""lane"":""win-x86-net40""",
            """packages"":[{""lane"":""win-x86-net40""," &
                """lane"":""win-x86-net40""",
            "duplicate package property")

        AssertInvalidManifest(
            ManifestJson("alpha", "2.0.0", "Alpha 1", "2.0.0-alpha.1", True),
            "published alpha manifest")
    End Sub

    Public Shared Sub AcceptsCurrentGeneratedManifestContract()
        Dim repositoryRoot As String = FindRepositoryRoot()
        Dim manifestPath As String = Path.Combine(
            repositoryRoot,
            "release\feeds\alpha\release.json")
        Dim readResult As UpdateManifestReadResult =
            New UpdateReleaseManifestReader().Read(
                File.ReadAllBytes(manifestPath),
                "alpha")
        If Not readResult.IsSuccess Then
            Throw New InvalidOperationException(
                "The generated alpha manifest was rejected: " & readResult.FailureMessage,
                readResult.FailureException)
        End If
        Dim manifest As UpdateReleaseManifest = readResult.Manifest

        Dim versionDocument As New Xml.XmlDocument()
        versionDocument.Load(Path.Combine(repositoryRoot, "build\Version.props"))
        Dim namespaces As New Xml.XmlNamespaceManager(versionDocument.NameTable)
        namespaces.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003")
        Dim productVersion As String = VersionProperty(
            versionDocument,
            namespaces,
            "C3ProductVersion")
        Dim releaseStage As String = VersionProperty(
            versionDocument,
            namespaces,
            "C3ReleaseStage")
        Dim releaseDate As DateTime = DateTime.ParseExact(
            VersionProperty(versionDocument, namespaces, "C3ReleaseDate"),
            "yyyy-MM-dd",
            Globalization.CultureInfo.InvariantCulture,
            Globalization.DateTimeStyles.AssumeUniversal Or
                Globalization.DateTimeStyles.AdjustToUniversal)

        AssertEqual("alpha", manifest.Channel, "manifest channel")
        AssertEqual(productVersion, manifest.ProductVersion, "manifest product version")
        AssertEqual(releaseStage, manifest.Stage, "manifest stage")
        AssertEqual(
            ExpectedReleaseLabel(productVersion, releaseStage),
            manifest.InformationalVersion,
            "manifest identity")
        AssertEqual(False, manifest.Published, "manifest publication state")
        AssertEqual(Nothing, manifest.ReleaseUrl, "unpublished release URL")
        AssertEqual(Nothing, manifest.ChecksumManifest, "unpublished checksum manifest")
        AssertEqual(0, manifest.Packages.Count, "unpublished package count")
        AssertEqual(
            releaseDate,
            manifest.ReleaseDate,
            "manifest release date")
    End Sub

    Private Shared Function VersionProperty(
            document As Xml.XmlDocument,
            namespaces As Xml.XmlNamespaceManager,
            propertyName As String) As String

        Dim node As Xml.XmlNode = document.SelectSingleNode(
            "//msb:" & propertyName,
            namespaces)
        If node Is Nothing OrElse String.IsNullOrWhiteSpace(node.InnerText) Then
            Throw New InvalidOperationException(
                "Version.props is missing " & propertyName & ".")
        End If
        Return node.InnerText
    End Function

    Private Shared Function ExpectedReleaseLabel(
            productVersion As String,
            releaseStage As String) As String

        If releaseStage.StartsWith("Alpha ", StringComparison.Ordinal) Then
            Return productVersion & "-alpha." & releaseStage.Substring(6)
        End If
        If releaseStage.StartsWith("Beta ", StringComparison.Ordinal) Then
            Return productVersion & "-beta." & releaseStage.Substring(5)
        End If
        If releaseStage.StartsWith(
                "Release Candidate ",
                StringComparison.Ordinal) Then
            Return productVersion & "-rc." & releaseStage.Substring(18)
        End If
        If String.Equals(releaseStage, "Release", StringComparison.Ordinal) Then
            Return productVersion
        End If
        Throw New InvalidOperationException(
            "Version.props contains unsupported release stage '" &
            releaseStage & "'.")
    End Function

    Public Shared Sub PublishedManifestRequiresExactReleaseAssets()
        Const releaseLabel As String = "2.0.0-beta.1"
        Dim validJson As String = ManifestJson(
            "beta",
            "2.0.0",
            "Beta 1",
            releaseLabel,
            True)
        Dim manifest As UpdateReleaseManifest = ReadManifest(validJson, "beta")
        Dim expectedReleaseUrl As String =
            "https://github.com/Julesc013/compact-cassette-catalogue/releases/tag/v" &
            releaseLabel

        AssertEqual(expectedReleaseUrl, manifest.ReleaseUrl, "published release URL")
        AssertEqual("SHA256SUMS.txt", manifest.ChecksumManifest.FileName, "checksum filename")
        AssertEqual(45L, manifest.ChecksumManifest.Length, "checksum length")
        AssertEqual(New String("b"c, 64), manifest.ChecksumManifest.Sha256, "checksum hash")
        AssertEqual(1, manifest.Packages.Count, "published package count")
        AssertEqual("win-x86-net40", manifest.Packages(0).Lane, "package lane")
        AssertEqual(123L, manifest.Packages(0).Length, "package length")

        Dim unpublishedJson As String = ManifestJson(
            "beta",
            "2.0.0",
            "Beta 1",
            releaseLabel,
            False)
        AssertInvalidManifest(
            unpublishedJson.Replace("""published"":false", """published"":true"),
            "published manifest without assets",
            "beta")
        AssertInvalidManifest(
            validJson.Replace("""published"":true", """published"":false"),
            "unpublished manifest with assets",
            "beta")
        AssertInvalidManifest(
            validJson.Replace(
                "/releases/tag/v" & releaseLabel,
                "/releases/tag/v9.9.9"),
            "wrong tagged release URL",
            "beta")
        AssertInvalidManifest(
            validJson.Replace("""file"":""SHA256SUMS.txt""", """file"":""OTHER.txt"""),
            "wrong checksum filename",
            "beta")
        AssertInvalidManifest(
            validJson.Replace("""length"":45", """length"":0"),
            "non-positive checksum length",
            "beta")
        AssertInvalidManifest(
            validJson.Replace(New String("b"c, 64), New String("B"c, 64)),
            "uppercase checksum hash",
            "beta")
        AssertInvalidManifest(
            validJson.Replace("/SHA256SUMS.txt", "/OTHER.txt"),
            "wrong checksum URL",
            "beta")
        AssertInvalidManifest(
            validJson.Replace(
                """packages"":[" & PackageJson(releaseLabel, "win-x86-net40") & "]",
                """packages"":[]"),
            "published manifest without packages",
            "beta")

        Dim duplicatePackage As String = validJson.Substring(0, validJson.Length - 2) &
            "," & PackageJson(releaseLabel, "win-x86-net40") & "]}"
        AssertInvalidManifest(duplicatePackage, "duplicate package lane", "beta")
        AssertInvalidManifest(
            validJson.Replace("""distribution"":""portable""", """distribution"":""installer"""),
            "wrong package distribution",
            "beta")
        AssertInvalidManifest(
            validJson.Replace("-portable.zip", "-other.zip"),
            "wrong package filename",
            "beta")
        AssertInvalidManifest(
            validJson.Replace("""length"":123", """length"":0"),
            "non-positive package length",
            "beta")
        AssertInvalidManifest(
            validJson.Replace(New String("a"c, 64), New String("A"c, 64)),
            "uppercase package hash",
            "beta")
        AssertInvalidManifest(
            validJson.Replace(
                "/releases/download/v" & releaseLabel & "/C3-v",
                "/releases/download/v9.9.9/C3-v"),
            "wrong package asset URL",
            "beta")

        AssertInvalidManifest(
            validJson.Insert(validJson.Length - 1, ",""unexpected"":true"),
            "unknown root property",
            "beta")
        AssertInvalidManifest(
            validJson.Replace(
                """checksumManifest"":{""file""",
                """checksumManifest"":{""unexpected"":true,""file"""),
            "unknown checksum property",
            "beta")
        AssertInvalidManifest(
            validJson.Replace(
                """packages"":[{",
                """packages"":[{""unexpected"":true,"),
            "unknown package property",
            "beta")
    End Sub

    Private Shared Sub AssertAvailable(
            currentIdentity As String,
            latestStage As String,
            latestIdentity As String,
            name As String,
            Optional channel As String = "alpha")

        Dim result As UpdateCheckResult = UpdateCheckService.Evaluate(
            currentIdentity,
            ReadManifest(
                ManifestJson(
                    channel,
                    CoreVersion(latestIdentity),
                    latestStage,
                    latestIdentity,
                    True),
                channel))
        AssertEqual(True, result.IsUpdateAvailable, name)
    End Sub

    Private Shared Sub AssertNotAvailable(
            currentIdentity As String,
            latestStage As String,
            latestIdentity As String,
            name As String,
            Optional channel As String = "alpha")

        Dim result As UpdateCheckResult = UpdateCheckService.Evaluate(
            currentIdentity,
            ReadManifest(
                ManifestJson(
                    channel,
                    CoreVersion(latestIdentity),
                    latestStage,
                    latestIdentity,
                    True),
                channel))
        AssertEqual(False, result.IsUpdateAvailable, name)
        AssertEqual(UpdateCheckOutcome.UpToDate, result.Outcome, name & " outcome")
    End Sub

    Private Shared Sub AssertIdentityBefore(
            olderIdentity As String,
            newerIdentity As String,
            name As String)

        Dim older As SemanticVersion = AssertParsedIdentity(olderIdentity, name & " older")
        Dim newer As SemanticVersion = AssertParsedIdentity(newerIdentity, name & " newer")
        AssertEqual(-1, Math.Sign(older.CompareTo(newer)), name & " forward")
        AssertEqual(1, Math.Sign(newer.CompareTo(older)), name & " reverse")
    End Sub

    Private Shared Sub AssertIdentityEqual(
            leftIdentity As String,
            rightIdentity As String,
            name As String)

        Dim left As SemanticVersion = AssertParsedIdentity(leftIdentity, name & " left")
        Dim right As SemanticVersion = AssertParsedIdentity(rightIdentity, name & " right")
        AssertEqual(0, left.CompareTo(right), name & " forward")
        AssertEqual(0, right.CompareTo(left), name & " reverse")
    End Sub

    Private Shared Function AssertParsedIdentity(
            identity As String,
            name As String) As SemanticVersion

        Dim parsed As SemanticVersion = Nothing
        AssertEqual(True, SemanticVersion.TryParse(identity, parsed), name & " parsed")
        If parsed Is Nothing Then
            Throw New InvalidOperationException(name & " returned no parsed identity.")
        End If
        Return parsed
    End Function

    Private Shared Sub AssertRejectedIdentity(identity As String, name As String)
        Dim parsed As SemanticVersion = Nothing
        AssertEqual(False, SemanticVersion.TryParse(identity, parsed), name & " rejected")
        AssertEqual(Nothing, parsed, name & " result")
    End Sub

    Private Shared Function ReadManifest(
            json As String,
            Optional expectedChannel As String = "alpha") As UpdateReleaseManifest

        Dim result As UpdateManifestReadResult = New UpdateReleaseManifestReader().Read(
            Encoding.UTF8.GetBytes(json),
            expectedChannel)
        If Not result.IsSuccess Then
            Throw New InvalidOperationException(
                "Fixture manifest was rejected: " & result.FailureMessage,
                result.FailureException)
        End If
        Return result.Manifest
    End Function

    Private Shared Sub AssertInvalidManifest(
            json As String,
            name As String,
            Optional expectedChannel As String = "alpha")

        Dim result As UpdateManifestReadResult = New UpdateReleaseManifestReader().Read(
            Encoding.UTF8.GetBytes(json),
            expectedChannel)
        AssertEqual(False, result.IsSuccess, name & " success")
        AssertEqual(UpdateManifestReadFailure.InvalidManifest, result.Failure, name & " failure")
    End Sub

    Private Shared Sub AssertWrongType(
            json As String,
            original As String,
            replacement As String,
            name As String)

        AssertInvalidManifest(ReplaceFirst(json, original, replacement), name, "beta")
    End Sub

    Private Shared Sub AssertMalformedMutation(
            json As String,
            original As String,
            replacement As String,
            name As String)

        Dim result As UpdateManifestReadResult = New UpdateReleaseManifestReader().Read(
            Encoding.UTF8.GetBytes(ReplaceFirst(json, original, replacement)),
            "beta")
        AssertEqual(False, result.IsSuccess, name & " success")
        AssertEqual(UpdateManifestReadFailure.MalformedJson, result.Failure, name & " failure")
    End Sub

    Private Shared Function ReplaceFirst(
            value As String,
            original As String,
            replacement As String) As String

        Dim position As Integer = value.IndexOf(original, StringComparison.Ordinal)
        If position < 0 Then
            Throw New InvalidOperationException("Could not find JSON mutation target: " & original)
        End If
        Return value.Substring(0, position) & replacement &
            value.Substring(position + original.Length)
    End Function

    Private Shared Function ManifestJson(
            channel As String,
            productVersion As String,
            stage As String,
            informationalVersion As String,
            published As Boolean) As String

        Dim releaseLabel As String = informationalVersion
        Dim buildSeparator As Integer = releaseLabel.IndexOf("+"c)
        If buildSeparator >= 0 Then
            releaseLabel = releaseLabel.Substring(0, buildSeparator)
        End If
        Dim tagName As String = "v" & releaseLabel
        Dim publicationFields As String
        If published Then
            publicationFields =
                """releaseUrl"":""https://github.com/Julesc013/compact-cassette-catalogue/releases/tag/" &
                    tagName & """," &
                """checksumManifest"":" & ChecksumJson(releaseLabel) & "," &
                """packages"":" & PackagesJson(releaseLabel)
        Else
            publicationFields =
                """releaseUrl"":null," &
                """checksumManifest"":null," &
                """packages"":[]"
        End If

        Return "{" &
            """schemaVersion"":1," &
            """product"":""Compact Cassette Catalogue""," &
            """productId"":""c3""," &
            """channel"":""" & channel & """," &
            """version"":""" & productVersion & """," &
            """stage"":""" & stage & """," &
            """informationalVersion"":""" & informationalVersion & """," &
            """releaseDate"":""2026-08-04""," &
            """catalogueWriteFormat"":""1.1.0""," &
            """published"":" & published.ToString().ToLowerInvariant() & "," &
            publicationFields &
            "}"
    End Function

    Private Shared Function ChecksumJson(releaseLabel As String) As String
        Return "{" &
            """file"":""SHA256SUMS.txt""," &
            """length"":45," &
            """sha256"":""" & New String("b"c, 64) & """," &
            """url"":""" & AssetRoot(releaseLabel) & "SHA256SUMS.txt""}"
    End Function

    Private Shared Function PackagesJson(releaseLabel As String) As String
        Return "[" & PackageJson(releaseLabel, "win-x86-net40") & "]"
    End Function

    Private Shared Function PackageJson(releaseLabel As String, lane As String) As String
        Dim fileName As String = "C3-v" & releaseLabel & "-" & lane & "-portable.zip"
        Dim assetUrl As String = AssetRoot(releaseLabel) & fileName
        Return "{" &
            """lane"":""" & lane & """," &
            """distribution"":""portable""," &
            """file"":""" & fileName & """," &
            """length"":123," &
            """sha256"":""" & New String("a"c, 64) & """," &
            """url"":""" & assetUrl & """}"
    End Function

    Private Shared Function AssetRoot(releaseLabel As String) As String
        Return "https://github.com/Julesc013/compact-cassette-catalogue/releases/download/v" &
            releaseLabel & "/"
    End Function

    Private Shared Function CoreVersion(informationalVersion As String) As String
        Dim separatorIndex As Integer = informationalVersion.IndexOf("-"c)
        If separatorIndex < 0 Then
            Return informationalVersion
        End If
        Return informationalVersion.Substring(0, separatorIndex)
    End Function

    Private Shared Function FindRepositoryRoot() As String
        Dim directory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While directory IsNot Nothing
            If File.Exists(Path.Combine(directory.FullName, "VERSION")) AndAlso
                    System.IO.Directory.Exists(
                        Path.Combine(directory.FullName, "release\feeds")) Then
                Return directory.FullName
            End If
            directory = directory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Class
