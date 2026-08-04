using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Preferences
{
	public sealed class UserPreferencesSaveResult
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

		public string BackupPath
		{
			get;
		}

		private UserPreferencesSaveResult(bool isSuccess, UserPreferencesSnapshot preferences, UserPreferencesFailure failure, string message, string backupPath)
		{
			IsSuccess = isSuccess;
			Preferences = preferences;
			Failure = failure;
			Message = (message ?? string.Empty);
			BackupPath = backupPath;
		}

		public static UserPreferencesSaveResult Saved(UserPreferencesSnapshot preferences, string backupPath)
		{
			if (preferences == null)
			{
				throw new ArgumentNullException("preferences");
			}
			return new UserPreferencesSaveResult(true, preferences, UserPreferencesFailure.None, string.Empty, backupPath);
		}

		public static UserPreferencesSaveResult Failed(UserPreferencesFailure failure, string message)
		{
			return new UserPreferencesSaveResult(false, null, failure, message, null);
		}
	}
}
