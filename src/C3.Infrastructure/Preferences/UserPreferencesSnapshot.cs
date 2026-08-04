using C3.Infrastructure.Updates;
using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Preferences
{
	public sealed class UserPreferencesSnapshot
	{
		public const int MaximumDefaultDirectoryCharacters = 32768;

		public const int CurrentLegacyImportVersion = 1;

		public const string ImportOutcomePending = "pending";

		public const string ImportOutcomeImported = "imported";

		public const string ImportOutcomeNotFound = "not-found";

		public const string ImportOutcomeInvalid = "invalid";
		public bool ShowMessages
		{
			get;
			set;
		}

		public string DefaultDirectory
		{
			get;
			set;
		}

		public UpdateCheckPolicy UpdatePolicy
		{
			get;
			set;
		}

		public DateTime LastUpdateCheck
		{
			get;
			set;
		}

		public int Legacy1xImportVersion
		{
			get;
			set;
		}

		public string Legacy1xImportOutcome
		{
			get;
			set;
		}

		public static UserPreferencesSnapshot CreateDefaults(string myDocumentsPath)
		{
			return new UserPreferencesSnapshot
			{
				ShowMessages = true,
				DefaultDirectory = (myDocumentsPath ?? string.Empty),
				UpdatePolicy = UpdateCheckPolicy.Never,
				LastUpdateCheck = DateTime.MinValue,
				Legacy1xImportVersion = 0,
				Legacy1xImportOutcome = "pending"
			};
		}

		public UserPreferencesSnapshot Clone()
		{
			return new UserPreferencesSnapshot
			{
				ShowMessages = ShowMessages,
				DefaultDirectory = DefaultDirectory,
				UpdatePolicy = UpdatePolicy,
				LastUpdateCheck = LastUpdateCheck,
				Legacy1xImportVersion = Legacy1xImportVersion,
				Legacy1xImportOutcome = Legacy1xImportOutcome
			};
		}
	}
}
