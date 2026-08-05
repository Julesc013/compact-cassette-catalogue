namespace C3.Domain.Profiles
{
    public static class KnownCatalogueProfiles
    {
        public static readonly CatalogueProfileCapabilities LegacyV1_1 =
            new CatalogueProfileCapabilities(
                "legacy-v1.1",
                true,
                new CatalogueProfileCapability[0]);

        public static readonly CatalogueProfileCapabilities NativeV2_0 =
            new CatalogueProfileCapabilities(
                "native-v2.0",
                true,
                new[]
                {
                    CatalogueProfileCapability.DurableCatalogueIdentity,
                    CatalogueProfileCapability.DurableEntityIdentity,
                    CatalogueProfileCapability.FieldProvenance,
                    CatalogueProfileCapability.StableRelationshipsAcrossReopen
                });
    }
}
