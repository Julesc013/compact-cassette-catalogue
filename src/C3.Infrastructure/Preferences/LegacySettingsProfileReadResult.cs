using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Preferences
{
	public sealed class LegacySettingsProfileReadResult
	{
		public LegacySettingsProfileCandidate Candidate
		{
			get;
		}

		public LegacyUserSettingsProfile Profile
		{
			get;
		}

		public LegacySettingsProfileReadFailure Failure
		{
			get;
		}

		public string FailureMessage
		{
			get;
		}

		public Exception FailureException
		{
			get;
		}

		public bool IsSuccess => Profile != null;

		private LegacySettingsProfileReadResult(LegacySettingsProfileCandidate candidate, LegacyUserSettingsProfile profile, LegacySettingsProfileReadFailure failure, string failureMessage, Exception failureException)
		{
			Candidate = candidate;
			Profile = profile;
			Failure = failure;
			FailureMessage = failureMessage;
			FailureException = failureException;
		}

		internal static LegacySettingsProfileReadResult Succeeded(LegacyUserSettingsProfile profile)
		{
			return new LegacySettingsProfileReadResult(profile.Candidate, profile, LegacySettingsProfileReadFailure.None, null, null);
		}

		internal static LegacySettingsProfileReadResult Failed(LegacySettingsProfileCandidate candidate, LegacySettingsProfileReadFailure failure, string message, Exception failureException = null)
		{
			return new LegacySettingsProfileReadResult(candidate, null, failure, message, failureException);
		}
	}
}
