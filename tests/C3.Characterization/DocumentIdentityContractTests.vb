Imports C3.Domain.Catalogues
Imports C3.Domain.Identity

Friend Module DocumentIdentityContractTests

    Private Const CanonicalIdentity As String = "01234567-89ab-cdef-0123-456789abcdef"

    Friend Sub SessionAndCatalogueIdentitiesAreNominalAndCanonical()
        Dim sessionId As DocumentSessionId = DocumentSessionId.Parse(CanonicalIdentity)
        Dim catalogueId As CatalogueId = CatalogueId.Parse(CanonicalIdentity)

        AssertEqual(CanonicalIdentity, sessionId.ToString(), "session identity text")
        AssertEqual(CanonicalIdentity, catalogueId.ToString(), "catalogue identity text")
        AssertEqual(
            True,
            sessionId.Equals(DocumentSessionId.Parse(CanonicalIdentity)),
            "session identity equality")
        AssertEqual(
            True,
            catalogueId.Equals(CatalogueId.Parse(CanonicalIdentity)),
            "catalogue identity equality")

        Dim parsedSession As DocumentSessionId = Nothing
        Dim parsedCatalogue As CatalogueId = Nothing
        AssertEqual(
            True,
            DocumentSessionId.TryParse(CanonicalIdentity, parsedSession),
            "session identity TryParse")
        AssertEqual(
            True,
            CatalogueId.TryParse(CanonicalIdentity, parsedCatalogue),
            "catalogue identity TryParse")
        AssertEqual(
            False,
            DocumentSessionId.TryParse(CanonicalIdentity.ToUpperInvariant(), parsedSession),
            "uppercase session identity rejection")
        AssertEqual(
            False,
            CatalogueId.TryParse(Guid.Empty.ToString("D"), parsedCatalogue),
            "empty catalogue identity rejection")
        AssertThrows(Of ArgumentException)(
            Sub() DocumentSessionId.Parse("{" & CanonicalIdentity & "}"),
            "non-canonical session identity")
        AssertThrows(Of ArgumentException)(
            Sub()
                Dim ignored As New CatalogueId(Guid.Empty)
            End Sub,
            "empty catalogue identity")

        AssertEqual(
            0,
            CInt(EntityIdentityDurability.SessionScoped),
            "session-scoped durability value")
        AssertEqual(
            1,
            CInt(EntityIdentityDurability.Durable),
            "durable identity value")
    End Sub

    Friend Sub ContentVersionsAdvanceMonotonically()
        Dim zero As ContentVersion = ContentVersion.Zero
        Dim first As ContentVersion = zero.Next()
        Dim second As ContentVersion = first.Next()

        AssertEqual(0L, zero.Value, "zero content version")
        AssertEqual(1L, first.Value, "first content version")
        AssertEqual(2L, second.Value, "second content version")
        AssertEqual(True, first.CompareTo(zero) > 0, "content version ordering")
        AssertEqual(True, first.Equals(New ContentVersion(1L)), "content version equality")
        AssertEqual("2", second.ToString(), "invariant content version text")

        AssertThrows(Of ArgumentOutOfRangeException)(
            Sub()
                Dim ignored As New ContentVersion(-1L)
            End Sub,
            "negative content version")
        AssertThrows(Of InvalidOperationException)(
            Sub()
                Dim ignored As ContentVersion = New ContentVersion(Long.MaxValue).Next()
            End Sub,
            "content version overflow")
    End Sub

    Friend Sub StateFingerprintsAreSchemeBoundAndStrict()
        Dim digest As String = New String("a"c, 64)
        Dim fingerprint As StateFingerprint = StateFingerprint.Sha256V1(digest)
        Dim parsed As StateFingerprint = StateFingerprint.Parse(fingerprint.ToString())

        AssertEqual(StateFingerprint.Sha256V1Scheme, fingerprint.Scheme, "fingerprint scheme")
        AssertEqual(digest, fingerprint.Digest, "fingerprint digest")
        AssertEqual(True, fingerprint.Equals(parsed), "fingerprint round trip")
        AssertEqual(
            False,
            fingerprint.Equals(New StateFingerprint("c3-logical-state-sha256-v2", digest)),
            "fingerprint scheme participates in equality")

        Dim attempted As StateFingerprint = Nothing
        AssertEqual(
            False,
            StateFingerprint.TryParse(
                StateFingerprint.Sha256V1Scheme & ":" & digest.ToUpperInvariant(),
                attempted),
            "uppercase digest rejection")
        AssertEqual(
            False,
            StateFingerprint.TryParse("unsafe_scheme:" & digest, attempted),
            "unsafe scheme rejection")
        AssertThrows(Of ArgumentException)(
            Sub() StateFingerprint.Parse(StateFingerprint.Sha256V1Scheme),
            "missing fingerprint separator")
    End Sub

    Friend Sub DiskRevisionsRemainOpaqueAndCaseSensitive()
        Dim lower As New DiskRevision("sha256:abcdef")
        Dim upper As New DiskRevision("sha256:ABCDEF")

        AssertEqual("sha256:abcdef", lower.Token, "disk revision token")
        AssertEqual(False, lower.Equals(upper), "disk revision case sensitivity")
        AssertEqual("sha256:abcdef", lower.ToString(), "disk revision round trip")

        Dim compatibility As New C3.Domain.Catalogues.CatalogueRevision("sha256:abcdef")
        AssertEqual(lower.Token, compatibility.Token, "catalogue revision compatibility")
        AssertThrows(Of ArgumentException)(
            Sub()
                Dim ignored As New DiskRevision("   ")
            End Sub,
            "blank disk revision")
    End Sub

    Private Sub AssertThrows(Of TException As Exception)(action As Action, name As String)
        Try
            action()
        Catch ex As TException
            Return
        End Try

        Throw New InvalidOperationException(
            name & " did not throw " & GetType(TException).Name & ".")
    End Sub

    Private Sub AssertEqual(Of TValue)(expected As TValue, actual As TValue, name As String)
        If Not EqualityComparer(Of TValue).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
