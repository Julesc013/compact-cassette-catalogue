using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Preferences
{
	public sealed class UserPreferencesLoadResult
	{
		public bool IsSuccess
		{
			get;
		}

		public UserPreferencesSnapshot Preferences
		{
			get;
		}

		public UserPreferencesFailure Failure
		{
			get;
		}

		public string Message
		{
			get;
		}

		public string RecoveryPath
		{
			get;
		}

		public string MigrationOutcome
		{
			get;
		}

		public bool IsMissing => Failure == UserPreferencesFailure.Missing;

		private UserPreferencesLoadResult(bool isSuccess, UserPreferencesSnapshot preferences, UserPreferencesFailure failure, string message, string recoveryPath, string migrationOutcome)
		{
			IsSuccess = isSuccess;
			Preferences = preferences;
			Failure = failure;
			Message = (message ?? string.Empty);
			RecoveryPath = recoveryPath;
			MigrationOutcome = migrationOutcome;
		}

		public static UserPreferencesLoadResult Loaded(UserPreferencesSnapshot preferences, string recoveryPath = null, string migrationOutcome = null, string message = null)
		{
			if (preferences == null)
			{
				throw new ArgumentNullException("preferences");
			}
			return new UserPreferencesLoadResult(true, preferences, UserPreferencesFailure.None, message, recoveryPath, migrationOutcome);
		}

		public static UserPreferencesLoadResult Missing()
		{
			return new UserPreferencesLoadResult(false, null, UserPreferencesFailure.Missing, "The preferences file does not exist.", null, null);
		}

		public static UserPreferencesLoadResult Failed(UserPreferencesFailure failure, string message, string recoveryPath = null, UserPreferencesSnapshot fallbackPreferences = null, string migrationOutcome = null)
		{
			return new UserPreferencesLoadResult(false, fallbackPreferences, failure, message, recoveryPath, migrationOutcome);
		}
	}
}
