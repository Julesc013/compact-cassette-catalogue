using C3.Infrastructure.Updates;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;

namespace C3.Infrastructure.Preferences
{
	public sealed class UserPreferencesService
	{
		private const string LegacyMyDocumentsExpression = "My.Computer.FileSystem.SpecialDirectories.MyDocuments";

		private readonly XmlUserPreferencesStore _store;

		private readonly LegacyUserSettingsImporter _legacyImporter;

		private readonly string _localApplicationDataDirectory;

		private readonly string _myDocumentsDirectory;

		private readonly Action<string> _warningSink;

		private readonly object _gate;

		private UserPreferencesSnapshot _current;

		private UserPreferenceFields _dirtyFields;

		private bool _isInitialized;

		public string PreferencesPath => _store.PreferencesPath;

		public bool IsInitialized
		{
			get
			{
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					return _isInitialized;
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
		}

		public bool HasUnsavedChanges
		{
			get
			{
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					return _dirtyFields != UserPreferenceFields.None;
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
		}

		public bool ShowMessages
		{
			get
			{
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					return _current.ShowMessages;
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
			set
			{
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					if (_current.ShowMessages != value)
					{
						_current.ShowMessages = value;
						_dirtyFields |= UserPreferenceFields.ShowMessages;
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
		}

		public string DefaultDirectory
		{
			get
			{
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					return _current.DefaultDirectory;
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
			set
			{
				string text = value ?? string.Empty;
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					if (!string.Equals(_current.DefaultDirectory, text, StringComparison.Ordinal))
					{
						_current.DefaultDirectory = text;
						_dirtyFields |= UserPreferenceFields.DefaultDirectory;
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
		}

		public UpdateCheckPolicy UpdatePolicy
		{
			get
			{
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					return _current.UpdatePolicy;
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
			set
			{
				if (!Enum.IsDefined(typeof(UpdateCheckPolicy), value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					if (_current.UpdatePolicy != value)
					{
						_current.UpdatePolicy = value;
						_dirtyFields |= UserPreferenceFields.UpdatePolicy;
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
		}

		public DateTime LastUpdateCheck
		{
			get
			{
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					return _current.LastUpdateCheck;
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
			set
			{
				object gate = _gate;
				ObjectFlowControl.CheckForSyncLockOnValueType(gate);
				bool flag = false;
				try
				{
					Monitor.Enter(gate, ref flag);
					if (!_current.LastUpdateCheck.Equals(value))
					{
						_current.LastUpdateCheck = value;
						_dirtyFields |= UserPreferenceFields.LastUpdateCheck;
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(gate);
					}
				}
			}
		}

		public UserPreferencesService(XmlUserPreferencesStore store, LegacyUserSettingsImporter legacyImporter, string localApplicationDataDirectory, string myDocumentsDirectory, Action<string> warningSink = null)
		{
			_gate = RuntimeHelpers.GetObjectValue(new object());
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			if (legacyImporter == null)
			{
				throw new ArgumentNullException("legacyImporter");
			}
			if (string.IsNullOrWhiteSpace(localApplicationDataDirectory))
			{
				throw new ArgumentException("A LocalApplicationData directory is required.", "localApplicationDataDirectory");
			}
			_store = store;
			_legacyImporter = legacyImporter;
			_localApplicationDataDirectory = Path.GetFullPath(localApplicationDataDirectory);
			_myDocumentsDirectory = (myDocumentsDirectory ?? string.Empty);
			_warningSink = warningSink;
			_current = UserPreferencesSnapshot.CreateDefaults(_myDocumentsDirectory);
		}

		public UserPreferencesLoadResult Initialize()
		{
			object gate = _gate;
			ObjectFlowControl.CheckForSyncLockOnValueType(gate);
			bool flag = false;
			try
			{
				Monitor.Enter(gate, ref flag);
				if (_isInitialized)
				{
					return UserPreferencesLoadResult.Loaded(_current.Clone(), null, null, null);
				}
				UserPreferencesSnapshot source = _current.Clone();
				UserPreferenceFields dirtyFields = _dirtyFields;
				UserPreferencesLoadResult userPreferencesLoadResult = InitializeCore();
				if (userPreferencesLoadResult.Preferences != null)
				{
					_current = userPreferencesLoadResult.Preferences.Clone();
					ApplyFields(_current, source, dirtyFields);
					userPreferencesLoadResult = ((!userPreferencesLoadResult.IsSuccess) ? UserPreferencesLoadResult.Failed(userPreferencesLoadResult.Failure, userPreferencesLoadResult.Message, userPreferencesLoadResult.RecoveryPath, _current.Clone(), userPreferencesLoadResult.MigrationOutcome) : UserPreferencesLoadResult.Loaded(_current.Clone(), userPreferencesLoadResult.RecoveryPath, userPreferencesLoadResult.MigrationOutcome, userPreferencesLoadResult.Message));
				}
				_dirtyFields = dirtyFields;
				_isInitialized = userPreferencesLoadResult.IsSuccess;
				return userPreferencesLoadResult;
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(gate);
				}
			}
		}

		public void Save()
		{
			UserPreferencesSaveResult userPreferencesSaveResult = TrySave();
			if (!userPreferencesSaveResult.IsSuccess && _warningSink != null)
			{
				_warningSink("Preferences could not be saved: " + userPreferencesSaveResult.Message);
			}
		}

		public UserPreferencesSaveResult TrySave()
		{
			object gate = _gate;
			ObjectFlowControl.CheckForSyncLockOnValueType(gate);
			bool flag = false;
			try
			{
				Monitor.Enter(gate, ref flag);
				if (!_isInitialized)
				{
					UserPreferencesLoadResult userPreferencesLoadResult = Initialize();
					if (!userPreferencesLoadResult.IsSuccess)
					{
						return UserPreferencesSaveResult.Failed(userPreferencesLoadResult.Failure, userPreferencesLoadResult.Message);
					}
				}
				if (_dirtyFields == UserPreferenceFields.None)
				{
					return UserPreferencesSaveResult.Saved(_current.Clone(), null);
				}
				UserPreferencesSaveResult userPreferencesSaveResult = _store.Save(_current.Clone(), _dirtyFields);
				if (userPreferencesSaveResult.IsSuccess)
				{
					_current = userPreferencesSaveResult.Preferences.Clone();
					_dirtyFields = UserPreferenceFields.None;
				}
				return userPreferencesSaveResult;
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(gate);
				}
			}
		}

		public UserPreferencesSnapshot Snapshot()
		{
			object gate = _gate;
			ObjectFlowControl.CheckForSyncLockOnValueType(gate);
			bool flag = false;
			try
			{
				Monitor.Enter(gate, ref flag);
				return _current.Clone();
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(gate);
				}
			}
		}

		private UserPreferencesLoadResult InitializeCore()
		{
			UserPreferencesSnapshot userPreferencesSnapshot = UserPreferencesSnapshot.CreateDefaults(_myDocumentsDirectory);
			try
			{
				using (_store.AcquireExclusiveLock())
				{
					UserPreferencesLoadResult userPreferencesLoadResult = _store.LoadPrimaryUnlocked();
					UserPreferencesSnapshot userPreferencesSnapshot2 = null;
					string recoveryPath = null;
					string text = string.Empty;
					bool flag = false;
					if (userPreferencesLoadResult.IsSuccess)
					{
						userPreferencesSnapshot2 = userPreferencesLoadResult.Preferences.Clone();
						goto IL_0167;
					}
					if (userPreferencesLoadResult.IsMissing)
					{
						UserPreferencesLoadResult userPreferencesLoadResult2 = _store.LoadBackupUnlocked();
						if (userPreferencesLoadResult2.IsSuccess)
						{
							userPreferencesSnapshot2 = userPreferencesLoadResult2.Preferences.Clone();
							flag = true;
							text = "Recovered preferences from the last known-good backup.";
						}
						else
						{
							if (IsUnsafeToReplace(userPreferencesLoadResult2))
							{
								return UserPreferencesLoadResult.Failed(userPreferencesLoadResult2.Failure, "The preferences backup could not be read safely: " + userPreferencesLoadResult2.Message, null, userPreferencesSnapshot, null);
							}
							userPreferencesSnapshot2 = userPreferencesSnapshot.Clone();
							flag = true;
						}
						goto IL_0167;
					}
					if (CanQuarantine(userPreferencesLoadResult))
					{
						UserPreferencesLoadResult userPreferencesLoadResult3 = _store.LoadBackupUnlocked();
						if (IsUnsafeToReplace(userPreferencesLoadResult3))
						{
							return UserPreferencesLoadResult.Failed(userPreferencesLoadResult3.Failure, "The native preferences and its backup could not be read safely: " + userPreferencesLoadResult3.Message, null, userPreferencesSnapshot, null);
						}
						recoveryPath = _store.QuarantinePrimaryUnlocked();
						if (userPreferencesLoadResult3.IsSuccess)
						{
							userPreferencesSnapshot2 = userPreferencesLoadResult3.Preferences.Clone();
							text = "Quarantined invalid preferences and recovered the backup.";
						}
						else
						{
							userPreferencesSnapshot2 = userPreferencesSnapshot.Clone();
							text = "Quarantined invalid preferences and restored safe defaults.";
						}
						flag = true;
						goto IL_0167;
					}
					return UserPreferencesLoadResult.Failed(userPreferencesLoadResult.Failure, userPreferencesLoadResult.Message, null, userPreferencesSnapshot, null);
					IL_0167:
					if (Normalize(userPreferencesSnapshot2))
					{
						flag = true;
					}
					string migrationOutcome = null;
					if (userPreferencesSnapshot2.Legacy1xImportVersion < 1)
					{
						LegacyUserSettingsImportResult legacyUserSettingsImportResult = _legacyImporter.Import(_localApplicationDataDirectory);
						ApplyLegacyImport(userPreferencesSnapshot2, legacyUserSettingsImportResult);
						migrationOutcome = userPreferencesSnapshot2.Legacy1xImportOutcome;
						flag = true;
						text = ((legacyUserSettingsImportResult.Status != LegacyUserSettingsImportStatus.Imported) ? ((legacyUserSettingsImportResult.Status != 0) ? AppendMessage(text, "C3 1.x preference profiles were found but none was valid.") : AppendMessage(text, "No supported C3 1.x preferences profile was found.")) : AppendMessage(text, "Imported C3 1.x preferences from " + legacyUserSettingsImportResult.Profile.SourcePath + "."));
					}
					if (flag)
					{
						UserPreferencesSaveResult userPreferencesSaveResult = _store.SaveExactUnlocked(userPreferencesSnapshot2);
						if (!userPreferencesSaveResult.IsSuccess)
						{
							return UserPreferencesLoadResult.Failed(userPreferencesSaveResult.Failure, AppendMessage(text, "The native preferences checkpoint failed: " + userPreferencesSaveResult.Message), recoveryPath, userPreferencesSnapshot2, migrationOutcome);
						}
						userPreferencesSnapshot2 = userPreferencesSaveResult.Preferences.Clone();
					}
					return UserPreferencesLoadResult.Loaded(userPreferencesSnapshot2, recoveryPath, migrationOutcome, text);
				}
			}
			catch (TimeoutException ex)
			{
				ProjectData.SetProjectError(ex);
				TimeoutException ex2 = ex;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.Busy, ex2.Message, null, userPreferencesSnapshot, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ProjectData.SetProjectError(ex3);
				UnauthorizedAccessException ex4 = ex3;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex4.Message, null, userPreferencesSnapshot, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (SecurityException ex5)
			{
				ProjectData.SetProjectError(ex5);
				SecurityException ex6 = ex5;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex6.Message, null, userPreferencesSnapshot, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (IOException ex7)
			{
				ProjectData.SetProjectError(ex7);
				IOException ex8 = ex7;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.IoFailure, ex8.Message, null, userPreferencesSnapshot, null);
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private void ApplyLegacyImport(UserPreferencesSnapshot snapshot, LegacyUserSettingsImportResult importResult)
		{
			if (importResult.Status == LegacyUserSettingsImportStatus.Imported)
			{
				LegacyUserSettingsProfile profile = importResult.Profile;
				if (profile.HasShowMessages)
				{
					snapshot.ShowMessages = profile.ShowMessages;
				}
				if (profile.HasDefaultDirectory)
				{
					snapshot.DefaultDirectory = profile.DefaultDirectory;
				}
				if (profile.HasUpdatePolicy)
				{
					snapshot.UpdatePolicy = profile.UpdatePolicy;
				}
				if (profile.HasLastUpdateCheck)
				{
					snapshot.LastUpdateCheck = profile.LastUpdateCheck;
				}
				snapshot.Legacy1xImportOutcome = "imported";
			}
			else if (importResult.Status == LegacyUserSettingsImportStatus.NotFound)
			{
				snapshot.Legacy1xImportOutcome = "not-found";
			}
			else
			{
				snapshot.Legacy1xImportOutcome = "invalid";
			}
			snapshot.Legacy1xImportVersion = 1;
			Normalize(snapshot);
		}

		private bool Normalize(UserPreferencesSnapshot snapshot)
		{
			if ((string.IsNullOrWhiteSpace(snapshot.DefaultDirectory) || string.Equals(snapshot.DefaultDirectory, "My.Computer.FileSystem.SpecialDirectories.MyDocuments", StringComparison.Ordinal)) && !string.Equals(snapshot.DefaultDirectory, _myDocumentsDirectory, StringComparison.Ordinal))
			{
				snapshot.DefaultDirectory = _myDocumentsDirectory;
				return true;
			}
			return false;
		}

		private static void ApplyFields(UserPreferencesSnapshot target, UserPreferencesSnapshot source, UserPreferenceFields fields)
		{
			if ((fields & UserPreferenceFields.ShowMessages) != 0)
			{
				target.ShowMessages = source.ShowMessages;
			}
			if ((fields & UserPreferenceFields.DefaultDirectory) != 0)
			{
				target.DefaultDirectory = source.DefaultDirectory;
			}
			if ((fields & UserPreferenceFields.UpdatePolicy) != 0)
			{
				target.UpdatePolicy = source.UpdatePolicy;
			}
			if ((fields & UserPreferenceFields.LastUpdateCheck) != 0)
			{
				target.LastUpdateCheck = source.LastUpdateCheck;
			}
		}

		private static bool CanQuarantine(UserPreferencesLoadResult result)
		{
			if (result.Failure != UserPreferencesFailure.Invalid)
			{
				return result.Failure == UserPreferencesFailure.TooLarge;
			}
			return true;
		}

		private static bool IsUnsafeToReplace(UserPreferencesLoadResult result)
		{
			if (!result.IsSuccess && !result.IsMissing)
			{
				return !CanQuarantine(result);
			}
			return false;
		}

		private static string AppendMessage(string existing, string addition)
		{
			if (string.IsNullOrWhiteSpace(existing))
			{
				return addition ?? string.Empty;
			}
			if (string.IsNullOrWhiteSpace(addition))
			{
				return existing;
			}
			return existing + " " + addition;
		}
	}
}
