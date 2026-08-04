using C3.Infrastructure.FileOperations;
using C3.Infrastructure.Updates;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml;

namespace C3.Infrastructure.Preferences
{
	public sealed class XmlUserPreferencesStore
	{
		private const long MaximumFileBytes = 262144L;

		private const int LockAttemptCount = 40;

		private const int LockRetryMilliseconds = 50;

		private const int RecoveryPathAttemptCount = 32;

		private readonly Func<DateTime> _clock;
		public string PreferencesPath
		{
			get;
		}

		public string BackupPath => PreferencesPath + ".bak";

		public XmlUserPreferencesStore(string preferencesPath, Func<DateTime> clock)
		{
			if (string.IsNullOrWhiteSpace(preferencesPath))
			{
				throw new ArgumentException("A preferences path is required.", "preferencesPath");
			}
			if (clock == null)
			{
				throw new ArgumentNullException("clock");
			}
			PreferencesPath = Path.GetFullPath(preferencesPath);
			_clock = clock;
		}

		public UserPreferencesLoadResult Load()
		{
			try
			{
				using (AcquireExclusiveLock())
				{
					return LoadPrimaryUnlocked();
				}
			}
			catch (TimeoutException ex)
			{
				ProjectData.SetProjectError(ex);
				TimeoutException ex2 = ex;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.Busy, ex2.Message, null, null, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ProjectData.SetProjectError(ex3);
				UnauthorizedAccessException ex4 = ex3;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex4.Message, null, null, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (SecurityException ex5)
			{
				ProjectData.SetProjectError(ex5);
				SecurityException ex6 = ex5;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex6.Message, null, null, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (IOException ex7)
			{
				ProjectData.SetProjectError(ex7);
				IOException ex8 = ex7;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.IoFailure, ex8.Message, null, null, null);
				ProjectData.ClearProjectError();
				return result;
			}
		}

		public UserPreferencesSaveResult Save(UserPreferencesSnapshot preferences, UserPreferenceFields dirtyFields)
		{
			if (preferences == null)
			{
				throw new ArgumentNullException("preferences");
			}
			if ((dirtyFields & ~(UserPreferenceFields.ShowMessages | UserPreferenceFields.DefaultDirectory | UserPreferenceFields.UpdatePolicy | UserPreferenceFields.LastUpdateCheck)) != 0)
			{
				return UserPreferencesSaveResult.Failed(UserPreferencesFailure.Invalid, "The preferences dirty-field mask is invalid.");
			}
			try
			{
				using (AcquireExclusiveLock())
				{
					UserPreferencesLoadResult userPreferencesLoadResult = LoadPrimaryUnlocked();
					if (!userPreferencesLoadResult.IsSuccess && !userPreferencesLoadResult.IsMissing)
					{
						return UserPreferencesSaveResult.Failed(userPreferencesLoadResult.Failure, userPreferencesLoadResult.Message);
					}
					UserPreferencesSnapshot userPreferencesSnapshot = (!userPreferencesLoadResult.IsSuccess) ? preferences.Clone() : Merge(userPreferencesLoadResult.Preferences, preferences, dirtyFields);
					if (preferences.Legacy1xImportVersion > userPreferencesSnapshot.Legacy1xImportVersion)
					{
						userPreferencesSnapshot.Legacy1xImportVersion = preferences.Legacy1xImportVersion;
						userPreferencesSnapshot.Legacy1xImportOutcome = preferences.Legacy1xImportOutcome;
					}
					return SaveExactUnlocked(userPreferencesSnapshot);
				}
			}
			catch (TimeoutException ex)
			{
				ProjectData.SetProjectError(ex);
				TimeoutException ex2 = ex;
				UserPreferencesSaveResult result = UserPreferencesSaveResult.Failed(UserPreferencesFailure.Busy, ex2.Message);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ProjectData.SetProjectError(ex3);
				UnauthorizedAccessException ex4 = ex3;
				UserPreferencesSaveResult result = UserPreferencesSaveResult.Failed(UserPreferencesFailure.AccessDenied, ex4.Message);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (SecurityException ex5)
			{
				ProjectData.SetProjectError(ex5);
				SecurityException ex6 = ex5;
				UserPreferencesSaveResult result = UserPreferencesSaveResult.Failed(UserPreferencesFailure.AccessDenied, ex6.Message);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (IOException ex7)
			{
				ProjectData.SetProjectError(ex7);
				IOException ex8 = ex7;
				UserPreferencesSaveResult result = UserPreferencesSaveResult.Failed(UserPreferencesFailure.IoFailure, ex8.Message);
				ProjectData.ClearProjectError();
				return result;
			}
		}

		internal IDisposable AcquireExclusiveLock()
		{
			string directoryName = Path.GetDirectoryName(PreferencesPath);
			if (string.IsNullOrWhiteSpace(directoryName))
			{
				throw new IOException("The preferences directory could not be resolved.");
			}
			Directory.CreateDirectory(directoryName);
			string path = Path.Combine(directoryName, "preferences.lock");
			int num = 1;
			do
			{
				try
				{
					return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
				}
				catch (IOException ex)
				{
					ProjectData.SetProjectError(ex);
					IOException ex2 = ex;
					if (!IsLockContention(ex2))
					{
						throw;
					}
					if (num == 40)
					{
						throw new TimeoutException("Another C3 process is using the shared preferences file.", ex2);
					}
					Thread.Sleep(50);
					ProjectData.ClearProjectError();
				}
				num = checked(num + 1);
			}
			while (num <= 40);
			throw new TimeoutException("Another C3 process is using the shared preferences file.");
		}

		internal UserPreferencesLoadResult LoadPrimaryUnlocked()
		{
			return LoadPathUnlocked(PreferencesPath);
		}

		internal UserPreferencesLoadResult LoadBackupUnlocked()
		{
			return LoadPathUnlocked(BackupPath);
		}

		internal string QuarantinePrimaryUnlocked()
		{
			if (!File.Exists(PreferencesPath))
			{
				return null;
			}
			string directoryName = Path.GetDirectoryName(PreferencesPath);
			DateTime stamp = _clock().ToUniversalTime();
			int num = 1;
			do
			{
				string text = Path.Combine(directoryName, CompactSiblingFileName.CreateRecovery(stamp));
				try
				{
					File.Move(PreferencesPath, text);
					return text;
				}
				catch (IOException ex)
				{
					ProjectData.SetProjectError(ex);
					IOException ex2 = ex;
					if (File.Exists(PreferencesPath) && (File.Exists(text) || Directory.Exists(text)))
					{
						ProjectData.ClearProjectError();
						goto end_IL_0057;
					}
					throw;
					end_IL_0057:;
				}
				num = checked(num + 1);
			}
			while (num <= 32);
			throw new IOException("C3 could not reserve a unique preferences recovery path.");
		}

		internal UserPreferencesSaveResult SaveExactUnlocked(UserPreferencesSnapshot preferences)
		{
			if (preferences == null)
			{
				throw new ArgumentNullException("preferences");
			}
			UserPreferencesSnapshot userPreferencesSnapshot = NormalizeForPersistence(preferences);
			string text = ValidateSnapshot(userPreferencesSnapshot);
			if (text != null)
			{
				return UserPreferencesSaveResult.Failed(UserPreferencesFailure.Invalid, text);
			}
			Directory.CreateDirectory(Path.GetDirectoryName(PreferencesPath));
			string text2 = BackupPath;
			try
			{
				XmlWriterSettings settings = new XmlWriterSettings
				{
					Encoding = new UTF8Encoding(false),
					Indent = true,
					NewLineChars = "\n",
					NewLineHandling = NewLineHandling.Replace,
					CloseOutput = false
				};
				using (OwnedSiblingTemporaryFile ownedSiblingTemporaryFile = OwnedSiblingTemporaryFile.Create(PreferencesPath))
				{
					using (FileStream fileStream = ownedSiblingTemporaryFile.Stream)
					{
						using (XmlWriter writer = XmlWriter.Create(fileStream, settings))
						{
							WriteSnapshot(writer, userPreferencesSnapshot);
						}
						fileStream.Flush(true);
					}
					UserPreferencesLoadResult userPreferencesLoadResult = LoadPathUnlocked(ownedSiblingTemporaryFile.Path);
					if (userPreferencesLoadResult.IsSuccess && AreEquivalent(userPreferencesSnapshot, userPreferencesLoadResult.Preferences))
					{
						if (File.Exists(PreferencesPath))
						{
							File.Replace(ownedSiblingTemporaryFile.Path, PreferencesPath, text2, true);
						}
						else
						{
							File.Move(ownedSiblingTemporaryFile.Path, PreferencesPath);
							text2 = null;
						}
						return UserPreferencesSaveResult.Saved(userPreferencesSnapshot.Clone(), text2);
					}
					string message = userPreferencesLoadResult.IsSuccess ? "The preferences snapshot changed during round-trip verification." : userPreferencesLoadResult.Message;
					return UserPreferencesSaveResult.Failed(UserPreferencesFailure.VerificationFailure, message);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				ProjectData.SetProjectError(ex);
				UnauthorizedAccessException ex2 = ex;
				UserPreferencesSaveResult result = UserPreferencesSaveResult.Failed(UserPreferencesFailure.AccessDenied, ex2.Message);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (SecurityException ex3)
			{
				ProjectData.SetProjectError(ex3);
				SecurityException ex4 = ex3;
				UserPreferencesSaveResult result = UserPreferencesSaveResult.Failed(UserPreferencesFailure.AccessDenied, ex4.Message);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (IOException ex5)
			{
				ProjectData.SetProjectError(ex5);
				IOException ex6 = ex5;
				UserPreferencesSaveResult result = UserPreferencesSaveResult.Failed(UserPreferencesFailure.IoFailure, ex6.Message);
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static UserPreferencesLoadResult LoadPathUnlocked(string pathValue)
		{
			try
			{
				XmlReaderSettings settings = new XmlReaderSettings
				{
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null,
					MaxCharactersInDocument = 262144,
					MaxCharactersFromEntities = 0
				};
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.XmlResolver = null;
				using (FileStream fileStream = new FileStream(pathValue, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					if (fileStream.Length > 262144)
					{
						return UserPreferencesLoadResult.Failed(UserPreferencesFailure.TooLarge, "The preferences file exceeds the 256 KiB safety limit.", null, null, null);
					}
					using (XmlReader reader = XmlReader.Create(fileStream, settings))
					{
						xmlDocument.Load(reader);
					}
				}
				return ParseDocument(xmlDocument);
			}
			catch (FileNotFoundException ex)
			{
				ProjectData.SetProjectError(ex);
				FileNotFoundException ex2 = ex;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Missing();
				ProjectData.ClearProjectError();
				return result;
			}
			catch (DirectoryNotFoundException ex3)
			{
				ProjectData.SetProjectError(ex3);
				DirectoryNotFoundException ex4 = ex3;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Missing();
				ProjectData.ClearProjectError();
				return result;
			}
			catch (UnauthorizedAccessException ex5)
			{
				ProjectData.SetProjectError(ex5);
				UnauthorizedAccessException ex6 = ex5;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex6.Message, null, null, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (SecurityException ex7)
			{
				ProjectData.SetProjectError(ex7);
				SecurityException ex8 = ex7;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.AccessDenied, ex8.Message, null, null, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (IOException ex9)
			{
				ProjectData.SetProjectError(ex9);
				IOException ex10 = ex9;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.IoFailure, ex10.Message, null, null, null);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (XmlException ex11)
			{
				ProjectData.SetProjectError(ex11);
				XmlException ex12 = ex11;
				UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, ex12.Message, null, null, null);
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static UserPreferencesLoadResult ParseDocument(XmlDocument document)
		{
			XmlElement documentElement = document.DocumentElement;
			if (documentElement == null)
			{
				return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The preferences root element is invalid.", null, null, null);
			}
			if (Operators.CompareString(documentElement.LocalName, "c3Preferences", false) == 0 && documentElement.NamespaceURI.Length > 0)
			{
				return UserPreferencesLoadResult.Failed(UserPreferencesFailure.UnsupportedVersion, "The preferences namespace was written by a newer C3 version.", null, null, null);
			}
			if (Operators.CompareString(documentElement.Name, "c3Preferences", false) != 0)
			{
				return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The preferences root element is invalid.", null, null, null);
			}
			if (int.TryParse(documentElement.GetAttribute("schemaVersion"), NumberStyles.None, CultureInfo.InvariantCulture, out int num) && num >= 1)
			{
				if (num > 1)
				{
					return UserPreferencesLoadResult.Failed(UserPreferencesFailure.UnsupportedVersion, "The preferences schema was written by a newer C3 version.", null, null, null);
				}
				if (documentElement.Attributes.Count != 3)
				{
					return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The preferences schema metadata is invalid.", null, null, null);
				}
				if (int.TryParse(documentElement.GetAttribute("legacy1xImportVersion"), NumberStyles.None, CultureInfo.InvariantCulture, out int num2) && num2 >= 0)
				{
					if (num2 > 1)
					{
						return UserPreferencesLoadResult.Failed(UserPreferencesFailure.UnsupportedVersion, "The preferences migration marker was written by a newer C3 version.", null, null, null);
					}
					string attribute = documentElement.GetAttribute("legacy1xImportOutcome");
					if (!IsImportOutcomeValid(num2, attribute))
					{
						return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The legacy settings import outcome is invalid.", null, null, null);
					}
					Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
					IEnumerator enumerator = default(IEnumerator);
					try
					{
						enumerator = documentElement.ChildNodes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlNode xmlNode = (XmlNode)enumerator.Current;
							if (xmlNode.NodeType != XmlNodeType.Comment && xmlNode.NodeType != XmlNodeType.Whitespace && xmlNode.NodeType != XmlNodeType.SignificantWhitespace)
							{
								if (xmlNode.NodeType != XmlNodeType.Element || xmlNode.NamespaceURI.Length > 0 || !IsKnownElement(xmlNode.Name) || dictionary.ContainsKey(xmlNode.Name))
								{
									return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The preferences file contains an unknown or duplicate field.", null, null, null);
								}
								XmlElement element = (XmlElement)xmlNode;
								string value = null;
								if (!TryReadScalar(element, ref value))
								{
									return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The preferences file contains a non-scalar field.", null, null, null);
								}
								dictionary.Add(xmlNode.Name, value);
							}
						}
					}
					finally
					{
						if (enumerator is IDisposable)
						{
							(enumerator as IDisposable).Dispose();
						}
					}
					string[] array = new string[4]
					{
						"showMessages",
						"defaultDirectory",
						"updatePolicy",
						"lastUpdateCheck"
					};
					foreach (string text in array)
					{
						if (!dictionary.ContainsKey(text))
						{
							return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The preferences file is missing '" + text + "'.", null, null, null);
						}
					}
					if (!bool.TryParse(dictionary["showMessages"], out bool showMessages))
					{
						return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The showMessages preference is invalid.", null, null, null);
					}
					UpdateCheckPolicy updatePolicy = default(UpdateCheckPolicy);
					if (!UpdateCheckSchedule.TryParseStored(dictionary["updatePolicy"], ref updatePolicy))
					{
						return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The updatePolicy preference is invalid.", null, null, null);
					}
					DateTime lastUpdateCheck = DateTime.MinValue;
					if (!string.IsNullOrWhiteSpace(dictionary["lastUpdateCheck"]))
					{
						try
						{
							lastUpdateCheck = XmlConvert.ToDateTime(dictionary["lastUpdateCheck"].Trim(), XmlDateTimeSerializationMode.RoundtripKind);
						}
						catch (FormatException ex)
						{
							ProjectData.SetProjectError(ex);
							FormatException ex2 = ex;
							UserPreferencesLoadResult result = UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The lastUpdateCheck preference is invalid.", null, null, null);
							ProjectData.ClearProjectError();
							return result;
						}
					}
					UserPreferencesSnapshot preferences = new UserPreferencesSnapshot
					{
						ShowMessages = showMessages,
						DefaultDirectory = dictionary["defaultDirectory"],
						UpdatePolicy = updatePolicy,
						LastUpdateCheck = lastUpdateCheck,
						Legacy1xImportVersion = num2,
						Legacy1xImportOutcome = attribute
					};
					string text2 = ValidateSnapshot(preferences);
					if (text2 != null)
					{
						return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, text2, null, null, null);
					}
					return UserPreferencesLoadResult.Loaded(preferences, null, null, null);
				}
				return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The legacy settings import marker is invalid.", null, null, null);
			}
			return UserPreferencesLoadResult.Failed(UserPreferencesFailure.Invalid, "The preferences schema version is invalid.", null, null, null);
		}

		private static void WriteSnapshot(XmlWriter writer, UserPreferencesSnapshot preferences)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("c3Preferences");
			writer.WriteAttributeString("schemaVersion", "1");
			writer.WriteAttributeString("legacy1xImportVersion", preferences.Legacy1xImportVersion.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("legacy1xImportOutcome", preferences.Legacy1xImportOutcome);
			writer.WriteElementString("showMessages", preferences.ShowMessages.ToString().ToLowerInvariant());
			writer.WriteElementString("defaultDirectory", preferences.DefaultDirectory ?? string.Empty);
			writer.WriteElementString("updatePolicy", UpdateCheckSchedule.Serialize(preferences.UpdatePolicy));
			writer.WriteElementString("lastUpdateCheck", (DateTime.Compare(preferences.LastUpdateCheck, DateTime.MinValue) == 0) ? string.Empty : XmlConvert.ToString(preferences.LastUpdateCheck, XmlDateTimeSerializationMode.RoundtripKind));
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}

		private static string ValidateSnapshot(UserPreferencesSnapshot preferences)
		{
			if (preferences == null)
			{
				return "Preferences are required.";
			}
			if (preferences.Legacy1xImportVersion >= 0 && preferences.Legacy1xImportVersion <= 1)
			{
				if (!IsImportOutcomeValid(preferences.Legacy1xImportVersion, preferences.Legacy1xImportOutcome))
				{
					return "The legacy settings import outcome is invalid.";
				}
				if (!Enum.IsDefined(typeof(UpdateCheckPolicy), preferences.UpdatePolicy))
				{
					return "The update policy is invalid.";
				}
				if (preferences.DefaultDirectory == null)
				{
					return "The default directory cannot be null.";
				}
				if (preferences.DefaultDirectory.Length > 32768)
				{
					return "The default directory exceeds the safety limit.";
				}
				try
				{
					XmlConvert.VerifyXmlChars(preferences.DefaultDirectory);
				}
				catch (XmlException ex)
				{
					ProjectData.SetProjectError(ex);
					XmlException ex2 = ex;
					string result = "The default directory contains invalid XML characters.";
					ProjectData.ClearProjectError();
					return result;
				}
				return null;
			}
			return "The legacy settings import marker is invalid.";
		}

		private static UserPreferencesSnapshot NormalizeForPersistence(UserPreferencesSnapshot preferences)
		{
			UserPreferencesSnapshot userPreferencesSnapshot = preferences.Clone();
			userPreferencesSnapshot.DefaultDirectory = (userPreferencesSnapshot.DefaultDirectory ?? string.Empty);
			if (DateTime.Compare(userPreferencesSnapshot.LastUpdateCheck, DateTime.MinValue) != 0)
			{
				userPreferencesSnapshot.LastUpdateCheck = UpdateCheckSchedule.NormalizeUtc(userPreferencesSnapshot.LastUpdateCheck);
			}
			return userPreferencesSnapshot;
		}

		private static bool TryReadScalar(XmlElement element, ref string value)
		{
			if (element.Attributes.Count != 0)
			{
				return false;
			}
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = element.ChildNodes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (((XmlNode)enumerator.Current).NodeType != XmlNodeType.Text)
					{
						return false;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			value = element.InnerText;
			return true;
		}

		private static bool IsKnownElement(string name)
		{
			if (Operators.CompareString(name, "showMessages", false) != 0 && Operators.CompareString(name, "defaultDirectory", false) != 0 && Operators.CompareString(name, "updatePolicy", false) != 0)
			{
				return Operators.CompareString(name, "lastUpdateCheck", false) == 0;
			}
			return true;
		}

		private static bool IsImportOutcomeValid(int version, string outcome)
		{
			if (version == 0)
			{
				return Operators.CompareString(outcome, "pending", false) == 0;
			}
			return Operators.CompareString(outcome, "imported", false) == 0 || Operators.CompareString(outcome, "not-found", false) == 0 || Operators.CompareString(outcome, "invalid", false) == 0;
		}

		private static UserPreferencesSnapshot Merge(UserPreferencesSnapshot current, UserPreferencesSnapshot incoming, UserPreferenceFields dirtyFields)
		{
			UserPreferencesSnapshot userPreferencesSnapshot = current.Clone();
			if ((dirtyFields & UserPreferenceFields.ShowMessages) != 0)
			{
				userPreferencesSnapshot.ShowMessages = incoming.ShowMessages;
			}
			if ((dirtyFields & UserPreferenceFields.DefaultDirectory) != 0)
			{
				userPreferencesSnapshot.DefaultDirectory = incoming.DefaultDirectory;
			}
			if ((dirtyFields & UserPreferenceFields.UpdatePolicy) != 0)
			{
				userPreferencesSnapshot.UpdatePolicy = incoming.UpdatePolicy;
			}
			if ((dirtyFields & UserPreferenceFields.LastUpdateCheck) != 0)
			{
				userPreferencesSnapshot.LastUpdateCheck = incoming.LastUpdateCheck;
			}
			return userPreferencesSnapshot;
		}

		private static bool AreEquivalent(UserPreferencesSnapshot expected, UserPreferencesSnapshot actual)
		{
			if (expected.ShowMessages == actual.ShowMessages && string.Equals(expected.DefaultDirectory, actual.DefaultDirectory, StringComparison.Ordinal) && expected.UpdatePolicy == actual.UpdatePolicy && expected.LastUpdateCheck.Equals(actual.LastUpdateCheck) && expected.Legacy1xImportVersion == actual.Legacy1xImportVersion)
			{
				return string.Equals(expected.Legacy1xImportOutcome, actual.Legacy1xImportOutcome, StringComparison.Ordinal);
			}
			return false;
		}

		private static bool IsLockContention(IOException failure)
		{
			int num = Marshal.GetHRForException(failure) & 0xFFFF;
			if (num != 32)
			{
				return num == 33;
			}
			return true;
		}
	}
}
