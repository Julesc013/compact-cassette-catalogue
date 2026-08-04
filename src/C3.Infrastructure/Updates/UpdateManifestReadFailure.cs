namespace C3.Infrastructure.Updates
{
	public enum UpdateManifestReadFailure
	{
		None,
		Empty,
		TooLarge,
		MalformedJson,
		UnsupportedSchema,
		WrongProduct,
		WrongChannel,
		InvalidManifest
	}
}
