namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public enum LegacyCatalogueFileFailure
	{
		None,
		FileNotFound,
		FileTooLarge,
		InvalidXml,
		MissingVersion,
		UnsupportedVersion,
		InvalidStructure,
		ConstraintViolation,
		ExternalModification,
		AccessDenied,
		IoFailure,
		VerificationFailure
	}
}
