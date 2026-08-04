namespace C3.Infrastructure.Preferences
{
	public enum UserPreferencesFailure
	{
		None,
		Missing,
		Invalid,
		UnsupportedVersion,
		TooLarge,
		AccessDenied,
		IoFailure,
		Busy,
		VerificationFailure
	}
}
