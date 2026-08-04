using C3.Infrastructure.Updates;
using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Preferences
{
	public sealed class LegacyUserSettingsProfile
	{
		public LegacySettingsProfileCandidate Candidate
		{
			get;
		}

		public string SourcePath => Candidate.FilePath;

		public Version SourceVersion => Candidate.ProfileVersion;

		public bool HasShowMessages
		{
			get;
		}

		public bool ShowMessages
		{
			get;
		}

		public bool HasDefaultDirectory
		{
			get;
		}

		public string DefaultDirectory
		{
			get;
		}

		public bool HasUpdatePolicy
		{
			get;
		}

		public UpdateCheckPolicy UpdatePolicy
		{
			get;
		}

		public bool HasLastUpdateCheck
		{
			get;
		}

		public DateTime LastUpdateCheck
		{
			get;
		}

		internal LegacyUserSettingsProfile(LegacySettingsProfileCandidate candidate, bool hasShowMessages, bool showMessages, bool hasDefaultDirectory, string defaultDirectory, bool hasUpdatePolicy, UpdateCheckPolicy updatePolicy, bool hasLastUpdateCheck, DateTime lastUpdateCheck)
		{
			if (candidate == null)
			{
				throw new ArgumentNullException("candidate");
			}
			Candidate = candidate;
			HasShowMessages = hasShowMessages;
			ShowMessages = showMessages;
			HasDefaultDirectory = hasDefaultDirectory;
			DefaultDirectory = defaultDirectory;
			HasUpdatePolicy = hasUpdatePolicy;
			UpdatePolicy = updatePolicy;
			HasLastUpdateCheck = hasLastUpdateCheck;
			LastUpdateCheck = lastUpdateCheck;
		}
	}
}
