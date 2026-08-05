Imports C3.Domain.Profiles
Imports C3.Domain.Validation

Friend Module ProfileRepresentabilityContractTests

    Friend Sub LogicalValidityProfileCapabilityAndExportLossStaySeparate()
        Dim logical As ValidationResult = ValidationResult.Valid
        Dim normalization As New RepresentationIssue(
            "text.normalized",
            "brands[brand-1].name",
            RepresentationEffect.Normalization,
            "The export profile normalizes the comparison form.")
        Dim loss As New RepresentationIssue(
            "identity.omitted",
            "brands[brand-1].id",
            RepresentationEffect.InformationLoss,
            "The export profile cannot retain durable identity.")
        Dim blocked As New RepresentationIssue(
            "relationship.unsupported",
            "recordings[recording-1]",
            RepresentationEffect.Unsupported,
            "The target profile cannot represent this relationship.")

        Dim normalized As New RepresentabilityResult(
            KnownCatalogueProfiles.NativeV2_0,
            RepresentationPurpose.DirectSave,
            {normalization})
        Dim lossyExport As New RepresentabilityResult(
            KnownCatalogueProfiles.LegacyV1_1,
            RepresentationPurpose.ExportCopy,
            {loss})
        Dim refusedExport As New RepresentabilityResult(
            KnownCatalogueProfiles.LegacyV1_1,
            RepresentationPurpose.ExportCopy,
            {blocked})

        AssertEqual(True, logical.IsValid, "logical validity")
        AssertEqual(True, normalized.CanRepresent, "normalization representability")
        AssertEqual(True, normalized.IsLossless, "normalization remains lossless")
        AssertEqual(True, lossyExport.CanRepresent, "lossy export representability")
        AssertEqual(False, lossyExport.IsLossless, "lossy export classification")
        AssertEqual(False, refusedExport.CanRepresent, "unsupported export refusal")
        AssertEqual(False, refusedExport.IsLossless, "unsupported export losslessness")
    End Sub

    Friend Sub PublishedProfilesExposeOnlyProvenCapabilities()
        Dim legacy As CatalogueProfileCapabilities = KnownCatalogueProfiles.LegacyV1_1
        Dim native As CatalogueProfileCapabilities = KnownCatalogueProfiles.NativeV2_0

        AssertEqual("legacy-v1.1", legacy.ProfileCode, "legacy profile code")
        AssertEqual(True, legacy.SupportsDirectSave, "legacy direct save")
        AssertEqual(
            False,
            legacy.Supports(CatalogueProfileCapability.DurableEntityIdentity),
            "legacy durable identity")

        AssertEqual("native-v2.0", native.ProfileCode, "native profile code")
        AssertEqual(True, native.SupportsDirectSave, "native direct save")
        AssertEqual(
            True,
            native.Supports(CatalogueProfileCapability.DurableEntityIdentity),
            "native durable identity")
        AssertEqual(
            False,
            native.Supports(CatalogueProfileCapability.QualifiedValues),
            "frozen native qualified values")
        AssertEqual(
            False,
            native.Supports(CatalogueProfileCapability.PartialHistoricalDates),
            "frozen native partial dates")
    End Sub

    Private Sub AssertEqual(Of TValue)(expected As TValue, actual As TValue, name As String)
        If Not EqualityComparer(Of TValue).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
