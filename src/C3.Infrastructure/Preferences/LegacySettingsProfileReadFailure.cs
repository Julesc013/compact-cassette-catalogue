namespace C3.Infrastructure.Preferences
{
	public enum LegacySettingsProfileReadFailure
	{
		None,
		Unavailable,
		TooLarge,
		MalformedXml,
		InvalidStructure,
		DuplicateSetting,
		InvalidValue
	}
}
