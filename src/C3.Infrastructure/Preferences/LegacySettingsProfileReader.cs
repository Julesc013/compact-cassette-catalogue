using C3.Infrastructure.Updates;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Xml;

namespace C3.Infrastructure.Preferences
{
	public sealed class LegacySettingsProfileReader
	{
		public const long MaximumProfileBytes = 262144L;

		private const string SettingsSectionXPath = "/configuration/userSettings/Compact_Cassette_Catalogue.My.MySettings";

		public LegacySettingsProfileReadResult Read(LegacySettingsProfileCandidate candidate)
		{
			if (candidate == null)
			{
				throw new ArgumentNullException("candidate");
			}
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.XmlResolver = null;
				using (FileStream fileStream = new FileStream(candidate.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					if (fileStream.Length > 262144)
					{
						return LegacySettingsProfileReadResult.Failed(candidate, LegacySettingsProfileReadFailure.TooLarge, "The legacy settings profile exceeds the 256 KiB safety limit.", null);
					}
					using (XmlReader reader = XmlReader.Create(fileStream, CreateSecureReaderSettings()))
					{
						xmlDocument.Load(reader);
					}
				}
				return ParseDocument(candidate, xmlDocument);
			}
			catch (XmlException ex)
			{
				ProjectData.SetProjectError(ex);
				XmlException failureException = ex;
				LegacySettingsProfileReadResult result = LegacySettingsProfileReadResult.Failed(candidate, LegacySettingsProfileReadFailure.MalformedXml, "The legacy settings profile is not safe, well-formed XML.", failureException);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (IOException ex2)
			{
				ProjectData.SetProjectError(ex2);
				IOException failureException2 = ex2;
				LegacySettingsProfileReadResult result = LegacySettingsProfileReadResult.Failed(candidate, LegacySettingsProfileReadFailure.Unavailable, "The legacy settings profile could not be read.", failureException2);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ProjectData.SetProjectError(ex3);
				UnauthorizedAccessException failureException3 = ex3;
				LegacySettingsProfileReadResult result = LegacySettingsProfileReadResult.Failed(candidate, LegacySettingsProfileReadFailure.Unavailable, "Access to the legacy settings profile was denied.", failureException3);
				ProjectData.ClearProjectError();
				return result;
			}
			catch (SecurityException ex4)
			{
				ProjectData.SetProjectError(ex4);
				SecurityException failureException4 = ex4;
				LegacySettingsProfileReadResult result = LegacySettingsProfileReadResult.Failed(candidate, LegacySettingsProfileReadFailure.Unavailable, "The legacy settings profile was blocked by the security policy.", failureException4);
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static XmlReaderSettings CreateSecureReaderSettings()
		{
			return new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null,
				MaxCharactersInDocument = 262144,
				MaxCharactersFromEntities = 0,
				IgnoreComments = true,
				IgnoreProcessingInstructions = true
			};
		}

		private static LegacySettingsProfileReadResult ParseDocument(LegacySettingsProfileCandidate candidate, XmlDocument document)
		{
			if (document.DocumentElement != null && Operators.CompareString(document.DocumentElement.Name, "configuration", false) == 0 && document.DocumentElement.NamespaceURI.Length == 0)
			{
				XmlNodeList xmlNodeList = document.SelectNodes("/configuration/userSettings");
				XmlNodeList xmlNodeList2 = document.SelectNodes("/configuration/userSettings/Compact_Cassette_Catalogue.My.MySettings");
				if (xmlNodeList != null && xmlNodeList.Count == 1 && xmlNodeList2 != null && xmlNodeList2.Count == 1)
				{
					XmlNode xmlNode = xmlNodeList2.Item(0);
					HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
					string defaultDirectory = null;
					UpdateCheckPolicy updatePolicy = UpdateCheckPolicy.Never;
					DateTime minValue = DateTime.MinValue;
					IEnumerator enumerator = default(IEnumerator);
					bool hasLastUpdateCheck = default(bool);
					bool hasUpdatePolicy = default(bool);
					bool hasDefaultDirectory = default(bool);
					bool showMessages = default(bool);
					bool hasShowMessages = default(bool);
					int num = default(int);
					try
					{
						enumerator = xmlNode.ChildNodes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlNode xmlNode2 = (XmlNode)enumerator.Current;
							if (xmlNode2.NodeType == XmlNodeType.Element && Operators.CompareString(xmlNode2.Name, "setting", false) == 0)
							{
								XmlElement xmlElement = (XmlElement)xmlNode2;
								string attribute = xmlElement.GetAttribute("name");
								if (IsKnownSetting(attribute))
								{
									if (!hashSet.Add(attribute))
									{
										return LegacySettingsProfileReadResult.Failed(candidate, LegacySettingsProfileReadFailure.DuplicateSetting, "The legacy profile contains a duplicate supported setting: " + attribute, null);
									}
									if (Operators.CompareString(xmlElement.GetAttribute("serializeAs"), "String", false) != 0)
									{
										return InvalidValue(candidate, "The legacy setting has an unsupported serialization mode: " + attribute);
									}
									XmlNodeList xmlNodeList3 = xmlElement.SelectNodes("value");
									if (xmlNodeList3 != null && xmlNodeList3.Count == 1 && !HasElementChild(xmlNodeList3.Item(0)))
									{
										string innerText = xmlNodeList3.Item(0).InnerText;
										if (Operators.CompareString(attribute, "showMessages", false) != 0)
										{
											if (Operators.CompareString(attribute, "defaultDirectory", false) != 0)
											{
												if (Operators.CompareString(attribute, "checkUpdates", false) != 0)
												{
													if (Operators.CompareString(attribute, "lastUpdateCheck", false) == 0)
													{
														if (!TryParseLastUpdateCheck(innerText, ref minValue))
														{
															return InvalidValue(candidate, "lastUpdateCheck is not a supported date value.");
														}
														hasLastUpdateCheck = true;
													}
												}
												else
												{
													if (!TryParseUpdatePolicy(innerText, ref updatePolicy))
													{
														return InvalidValue(candidate, "checkUpdates is not a supported policy value.");
													}
													hasUpdatePolicy = true;
												}
											}
											else
											{
												if (innerText.Length > 32768)
												{
													return InvalidValue(candidate, "defaultDirectory exceeds the supported safety limit.");
												}
												defaultDirectory = innerText;
												hasDefaultDirectory = true;
											}
										}
										else
										{
											if (!bool.TryParse(innerText.Trim(), out showMessages))
											{
												return InvalidValue(candidate, "showMessages is not a Boolean value.");
											}
											hasShowMessages = true;
										}
										num = checked(num + 1);
										continue;
									}
									return InvalidValue(candidate, "The legacy setting does not contain one scalar value: " + attribute);
								}
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
					if (num == 0)
					{
						return InvalidStructure(candidate, "The legacy section contains no supported C3 settings.");
					}
					return LegacySettingsProfileReadResult.Succeeded(new LegacyUserSettingsProfile(candidate, hasShowMessages, showMessages, hasDefaultDirectory, defaultDirectory, hasUpdatePolicy, updatePolicy, hasLastUpdateCheck, minValue));
				}
				return InvalidStructure(candidate, "The exact C3 1.x user settings section was not found once.");
			}
			return InvalidStructure(candidate, "The configuration root is not the legacy C3 root.");
		}

		private static bool IsKnownSetting(string name)
		{
			if (Operators.CompareString(name, "showMessages", false) != 0 && Operators.CompareString(name, "defaultDirectory", false) != 0 && Operators.CompareString(name, "checkUpdates", false) != 0)
			{
				return Operators.CompareString(name, "lastUpdateCheck", false) == 0;
			}
			return true;
		}

		private static bool HasElementChild(XmlNode node)
		{
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = node.ChildNodes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (((XmlNode)enumerator.Current).NodeType == XmlNodeType.Element)
					{
						return true;
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
			return false;
		}

		private static bool TryParseUpdatePolicy(string rawValue, ref UpdateCheckPolicy updatePolicy)
		{
			if (bool.TryParse((rawValue ?? string.Empty).Trim(), out bool flag))
			{
				updatePolicy = (flag ? UpdateCheckPolicy.Startup : UpdateCheckPolicy.Never);
				return true;
			}
			return UpdateCheckSchedule.TryParseStored(rawValue, ref updatePolicy);
		}

		private static bool TryParseLastUpdateCheck(string rawValue, ref DateTime lastUpdateCheck)
		{
			string text = (rawValue ?? string.Empty).Trim();
			if (text.Length == 0)
			{
				lastUpdateCheck = DateTime.MinValue;
				return true;
			}
			return DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.RoundtripKind, out lastUpdateCheck);
		}

		private static LegacySettingsProfileReadResult InvalidStructure(LegacySettingsProfileCandidate candidate, string message)
		{
			return LegacySettingsProfileReadResult.Failed(candidate, LegacySettingsProfileReadFailure.InvalidStructure, message, null);
		}

		private static LegacySettingsProfileReadResult InvalidValue(LegacySettingsProfileCandidate candidate, string message)
		{
			return LegacySettingsProfileReadResult.Failed(candidate, LegacySettingsProfileReadFailure.InvalidValue, message, null);
		}
	}
}
