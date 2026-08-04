Imports C3.Catalogue.Native
Imports C3.Domain.Identity
Imports C3.Domain.Time
Imports C3.Domain.Values

Friend NotInheritable Class NativeCatalogueContractTests

    Private Sub New()
    End Sub

    Public Shared Sub NativeGraphUsesStableTypedReferencesAndCanonicalOrder()
        Dim sourceRevision As String = New String("a"c, 64)
        Dim namespaceId As String = "c3:migration:v1.1-to-v2.0/1:" & sourceRevision
        Dim brandId As EntityId(Of NativeBrand) =
            DeterministicEntityId.FromCanonicalKey(Of NativeBrand)(namespaceId, "brand:MAX")
        Dim modelId As EntityId(Of NativeCassetteModel) =
            DeterministicEntityId.FromCanonicalKey(Of NativeCassetteModel)(namespaceId, "model:MAX-2-XLII")
        Dim deckModelId As EntityId(Of NativeDeckModel) =
            DeterministicEntityId.FromCanonicalKey(Of NativeDeckModel)(namespaceId, "deck-model:Nakamichi BX-300")
        Dim deckUnitId As EntityId(Of NativeDeckUnit) =
            DeterministicEntityId.FromCanonicalKey(Of NativeDeckUnit)(namespaceId, "deck-unit:Nakamichi BX-300")
        Dim tapeId As EntityId(Of NativeTape) =
            DeterministicEntityId.FromCanonicalKey(Of NativeTape)(namespaceId, "tape:MAX-2-XLII-1")
        Dim recordingId As EntityId(Of NativeRecording) =
            DeterministicEntityId.FromCanonicalKey(Of NativeRecording)(namespaceId, "recording:MAX-2-XLII-1:A")
        Dim catalogueId As EntityId(Of NativeCatalogue) =
            DeterministicEntityId.FromCanonicalKey(Of NativeCatalogue)(namespaceId, "catalogue")
        Dim stamp As New UtcTimestamp(New DateTime(2026, 8, 4, 0, 0, 0, DateTimeKind.Utc))

        Dim brand As New NativeBrand(brandId, "Maxell", "MAX", stamp, "")
        Dim model As New NativeCassetteModel(
            modelId, brandId, 2, "XLII", "XLII", "MAX-2-XLII", "Maxell XLII", stamp, "")
        Dim capabilities As New NativeDeckCapabilities(
            True, True, False, True, True, True, True, True, False, False, False,
            True, False, False, True, False, False, False, 20, 20000, 72,
            "Dolby C", 0.04D, 0.8D, 3, 1, False, True, False)
        Dim deckModel As New NativeDeckModel(
            deckModelId, "Nakamichi", "BX-300", 1984, capabilities)
        Dim deckUnit As New NativeDeckUnit(
            deckUnitId, deckModelId, "Nakamichi BX-300", "Nakamichi BX-300", 7, stamp, "")
        Dim recording As New NativeRecording(
            recordingId,
            C3.Domain.Values.[Optional](Of EntityId(Of NativeDeckUnit)).Some(deckUnitId),
            stamp,
            "Line", 0, "Dolby C", True, True, False, "Normal", 2, 0,
            "70", 0.5D, 0D, "Album", "Various", "Fixture")
        Dim tape As New NativeTape(
            tapeId, modelId, 1990, 90D, "Japan", 1,
            "MAX-2-XLII-1990-90-1", "MAX-2-XLII-1", 8, False, stamp, "",
            New NativeTapeSide(
                NativeTapeSidePosition.A,
                "Side A",
                C3.Domain.Values.[Optional](Of NativeRecording).Some(recording)),
            New NativeTapeSide(
                NativeTapeSidePosition.B,
                "Side B",
                C3.Domain.Values.[Optional](Of NativeRecording).None()))
        Dim metadata As New NativeCatalogueMetadata(
            "C3 2.0.0 Alpha 4",
            stamp,
            stamp,
            C3.Domain.Values.[Optional](Of NativeCatalogueProvenance).Some(
                New NativeCatalogueProvenance("1.1.0", sourceRevision, "v1.1-to-v2.0/1")))
        Dim document As New NativeCatalogue(
            catalogueId,
            metadata,
            New NativeBrand() {brand},
            New NativeCassetteModel() {model},
            New NativeDeckModel() {deckModel},
            New NativeDeckUnit() {deckUnit},
            New NativeTape() {tape})

        AssertEqual("MAX", document.Brands(0).LegacyCode, "native brand")
        AssertEqual(deckUnitId, document.Tapes(0).SideA.Recording.Value.DeckUnitId.Value, "recording deck")
        AssertEqual(
            brandId,
            DeterministicEntityId.FromCanonicalKey(Of NativeBrand)(namespaceId, "brand:MAX"),
            "stable key mapping")

        Dim rejected As Boolean = False
        Try
            Dim missingBrandId = DeterministicEntityId.FromCanonicalKey(Of NativeBrand)(namespaceId, "brand:MISSING")
            Dim invalidModel As New NativeCassetteModel(
                modelId, missingBrandId, 2, "XLII", "XLII", "MAX-2-XLII",
                "Maxell XLII", stamp, "")
            Dim unused As New NativeCatalogue(
                catalogueId,
                metadata,
                New NativeBrand() {brand},
                New NativeCassetteModel() {invalidModel},
                New NativeDeckModel() {},
                New NativeDeckUnit() {},
                New NativeTape() {})
        Catch ex As ArgumentException
            rejected = True
        End Try
        AssertEqual(True, rejected, "unresolved brand reference")
    End Sub

    Private Shared Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub
End Class
