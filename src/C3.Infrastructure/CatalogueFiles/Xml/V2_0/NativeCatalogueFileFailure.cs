namespace C3.Infrastructure.CatalogueFiles.Xml.V2_0
{
    public enum NativeCatalogueFileFailure
    {
        None = 0,
        FileNotFound = 1,
        FileTooLarge = 2,
        UnsafeXml = 3,
        InvalidStructure = 4,
        UnsupportedFormat = 5,
        InvalidValue = 6,
        DuplicateIdentity = 7,
        UnresolvedReference = 8,
        ExternalModification = 9,
        AccessDenied = 10,
        IoFailure = 11,
        VerificationFailure = 12
    }
}
