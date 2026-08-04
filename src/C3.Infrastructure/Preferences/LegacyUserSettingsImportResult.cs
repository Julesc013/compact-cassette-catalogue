using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Preferences
{
	public sealed class LegacyUserSettingsImportResult
	{
		public LegacyUserSettingsImportStatus Status
		{
			get;
		}

		public LegacyUserSettingsProfile Profile
		{
			get;
		}

		public IList<LegacySettingsProfileReadResult> RejectedCandidates
		{
			get;
		}

		public bool IsSuccess => Status != LegacyUserSettingsImportStatus.Failed;

		private LegacyUserSettingsImportResult(LegacyUserSettingsImportStatus status, LegacyUserSettingsProfile profile, IList<LegacySettingsProfileReadResult> rejectedCandidates)
		{
			Status = status;
			Profile = profile;
			RejectedCandidates = new List<LegacySettingsProfileReadResult>(rejectedCandidates).AsReadOnly();
		}

		internal static LegacyUserSettingsImportResult NotFound()
		{
			return new LegacyUserSettingsImportResult(LegacyUserSettingsImportStatus.NotFound, null, new List<LegacySettingsProfileReadResult>());
		}

		internal static LegacyUserSettingsImportResult Imported(LegacyUserSettingsProfile profile, IList<LegacySettingsProfileReadResult> rejectedCandidates)
		{
			return new LegacyUserSettingsImportResult(LegacyUserSettingsImportStatus.Imported, profile, rejectedCandidates);
		}

		internal static LegacyUserSettingsImportResult Failed(IList<LegacySettingsProfileReadResult> rejectedCandidates)
		{
			return new LegacyUserSettingsImportResult(LegacyUserSettingsImportStatus.Failed, null, rejectedCandidates);
		}
	}
}
