using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateReleaseManifestReader
	{
		public const int MaximumManifestBytes = 32768;

		private const int MaximumFieldCharacters = 512;

		private const int MaximumPackages = 16;

		private const string ExpectedProductName = "Compact Cassette Catalogue";

		private const string ExpectedProductId = "c3";

		private const string RepositoryUrl = "https://github.com/Julesc013/compact-cassette-catalogue";

		private const string ChecksumFileName = "SHA256SUMS.txt";

		public UpdateManifestReadResult Read(byte[] payload, string expectedChannel)
		{
			if (payload != null && payload.Length != 0)
			{
				if (payload.Length > 32768)
				{
					return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.TooLarge, "The update manifest exceeds the 32 KiB safety limit.", null);
				}
				if (!IsSupportedChannel(expectedChannel))
				{
					throw new ArgumentException("The expected update channel is invalid.", "expectedChannel");
				}
				UpdateReleaseManifestDocument document = default(UpdateReleaseManifestDocument);
				try
				{
					string text = ValidatePropertyShape(payload);
					if (text != null)
					{
						return Invalid(text);
					}
					XmlDictionaryReaderQuotas xmlDictionaryReaderQuotas = new XmlDictionaryReaderQuotas();
					xmlDictionaryReaderQuotas.MaxDepth = 8;
					xmlDictionaryReaderQuotas.MaxStringContentLength = 512;
					xmlDictionaryReaderQuotas.MaxArrayLength = 16;
					xmlDictionaryReaderQuotas.MaxBytesPerRead = 4096;
					xmlDictionaryReaderQuotas.MaxNameTableCharCount = 2048;
					DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(UpdateReleaseManifestDocument));
					UTF8Encoding encoding = new UTF8Encoding(false, true);
					using (XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(payload, 0, payload.Length, encoding, xmlDictionaryReaderQuotas, null))
					{
						document = (UpdateReleaseManifestDocument)dataContractJsonSerializer.ReadObject(reader, true);
					}
				}
				catch (SerializationException ex)
				{
					ProjectData.SetProjectError(ex);
					SerializationException exception = ex;
					UpdateManifestReadResult result = Malformed(exception);
					ProjectData.ClearProjectError();
					return result;
				}
				catch (XmlException ex2)
				{
					ProjectData.SetProjectError(ex2);
					XmlException exception2 = ex2;
					UpdateManifestReadResult result = Malformed(exception2);
					ProjectData.ClearProjectError();
					return result;
				}
				catch (DecoderFallbackException ex3)
				{
					ProjectData.SetProjectError(ex3);
					DecoderFallbackException exception3 = ex3;
					UpdateManifestReadResult result = Malformed(exception3);
					ProjectData.ClearProjectError();
					return result;
				}
				catch (ArgumentException ex4)
				{
					ProjectData.SetProjectError(ex4);
					ArgumentException exception4 = ex4;
					UpdateManifestReadResult result = Malformed(exception4);
					ProjectData.ClearProjectError();
					return result;
				}
				return Validate(document, expectedChannel);
			}
			return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.Empty, "The update manifest is empty.", null);
		}

		private static string ValidatePropertyShape(byte[] payload)
		{
			XmlDictionaryReaderQuotas xmlDictionaryReaderQuotas = new XmlDictionaryReaderQuotas();
			xmlDictionaryReaderQuotas.MaxDepth = 8;
			xmlDictionaryReaderQuotas.MaxStringContentLength = 512;
			xmlDictionaryReaderQuotas.MaxArrayLength = 16;
			xmlDictionaryReaderQuotas.MaxBytesPerRead = 4096;
			xmlDictionaryReaderQuotas.MaxNameTableCharCount = 2048;
			UTF8Encoding encoding = new UTF8Encoding(false, true);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			HashSet<string> hashSet2 = null;
			HashSet<string> hashSet3 = null;
			string text = null;
			string text2 = null;
			bool flag = false;
			using (XmlDictionaryReader xmlDictionaryReader = JsonReaderWriterFactory.CreateJsonReader(payload, 0, payload.Length, encoding, xmlDictionaryReaderQuotas, null))
			{
				while (xmlDictionaryReader.Read())
				{
					if (xmlDictionaryReader.NodeType == XmlNodeType.Element)
					{
						switch (xmlDictionaryReader.Depth)
						{
						case 0:
							if (!flag && Operators.CompareString(xmlDictionaryReader.LocalName, "root", false) == 0 && HasJsonType(xmlDictionaryReader, "object"))
							{
								flag = true;
								break;
							}
							return "The update manifest JSON root is invalid.";
						case 1:
						{
							text = xmlDictionaryReader.LocalName;
							if (!IsRootProperty(text))
							{
								return "The update manifest contains an unsupported property.";
							}
							if (!hashSet.Add(text))
							{
								throw new SerializationException("The update manifest contains a duplicate property.");
							}
							string text4 = ValidateRootPropertyType(xmlDictionaryReader);
							if (text4 != null)
							{
								return text4;
							}
							if (Operators.CompareString(text, "checksumManifest", false) == 0 && HasJsonType(xmlDictionaryReader, "object"))
							{
								hashSet2 = new HashSet<string>(StringComparer.Ordinal);
							}
							break;
						}
						case 2:
							text2 = xmlDictionaryReader.LocalName;
							if (Operators.CompareString(text, "checksumManifest", false) == 0)
							{
								if (hashSet2 == null)
								{
									return "The checksum manifest container is invalid.";
								}
								if (!IsChecksumProperty(text2))
								{
									return "The checksum manifest contains an unsupported property.";
								}
								if (!hashSet2.Add(text2))
								{
									throw new SerializationException("The checksum manifest contains a duplicate property.");
								}
								string text5 = ValidateChecksumPropertyType(xmlDictionaryReader);
								if (text5 == null)
								{
									break;
								}
								return text5;
							}
							if (Operators.CompareString(text, "packages", false) == 0)
							{
								if (Operators.CompareString(text2, "item", false) == 0 && HasJsonType(xmlDictionaryReader, "object"))
								{
									hashSet3 = new HashSet<string>(StringComparer.Ordinal);
									break;
								}
								return "The update manifest package array is invalid.";
							}
							return "The update manifest contains unsupported nested data.";
						case 3:
							if (Operators.CompareString(text, "packages", false) == 0 && Operators.CompareString(text2, "item", false) == 0 && IsPackageProperty(xmlDictionaryReader.LocalName))
							{
								if (hashSet3 == null)
								{
									return "The update manifest package container is invalid.";
								}
								if (!hashSet3.Add(xmlDictionaryReader.LocalName))
								{
									throw new SerializationException("An update package contains a duplicate property.");
								}
								string text3 = ValidatePackagePropertyType(xmlDictionaryReader);
								if (text3 == null)
								{
									break;
								}
								return text3;
							}
							return "An update package contains an unsupported property.";
						default:
							return "The update manifest exceeds the supported object shape.";
						}
					}
					else if (xmlDictionaryReader.NodeType == XmlNodeType.EndElement)
					{
						if (xmlDictionaryReader.Depth == 2)
						{
							if (Operators.CompareString(text, "packages", false) == 0 && Operators.CompareString(text2, "item", false) == 0)
							{
								if (hashSet3 == null || hashSet3.Count != 6)
								{
									throw new SerializationException("An update package is missing a required property.");
								}
								hashSet3 = null;
							}
							text2 = null;
						}
						else if (xmlDictionaryReader.Depth == 1)
						{
							if (Operators.CompareString(text, "checksumManifest", false) == 0 && hashSet2 != null && hashSet2.Count != 4)
							{
								throw new SerializationException("The checksum manifest is missing a required property.");
							}
							hashSet2 = null;
							text = null;
						}
					}
				}
			}
			if (flag && hashSet.Count == 13)
			{
				return null;
			}
			throw new SerializationException("The update manifest is missing a required property.");
		}

		private static string ValidateRootPropertyType(XmlDictionaryReader reader)
		{
			switch (reader.LocalName)
			{
			case "schemaVersion":
				if (!HasJsonType(reader, "number"))
				{
					return "The schemaVersion field must be a JSON number.";
				}
				break;
			case "product":
			case "productId":
			case "channel":
			case "version":
			case "stage":
			case "informationalVersion":
			case "releaseDate":
			case "catalogueWriteFormat":
				if (!HasJsonType(reader, "string"))
				{
					return "An update manifest text field has the wrong JSON type.";
				}
				break;
			case "published":
				if (!HasJsonType(reader, "boolean"))
				{
					return "The published field must be a JSON Boolean.";
				}
				break;
			case "releaseUrl":
				if (!HasJsonType(reader, "string", "null"))
				{
					return "The releaseUrl field must be a JSON string or null.";
				}
				break;
			case "checksumManifest":
				if (!HasJsonType(reader, "object", "null"))
				{
					return "The checksumManifest field must be a JSON object or null.";
				}
				break;
			case "packages":
				if (!HasJsonType(reader, "array"))
				{
					return "The packages field must be a JSON array.";
				}
				break;
			}
			return null;
		}

		private static string ValidateChecksumPropertyType(XmlDictionaryReader reader)
		{
			if (Operators.CompareString(reader.LocalName, "length", false) == 0)
			{
				if (!HasJsonType(reader, "number"))
				{
					return "The checksum manifest length must be a JSON number.";
				}
			}
			else if (!HasJsonType(reader, "string"))
			{
				return "A checksum manifest text field has the wrong JSON type.";
			}
			return null;
		}

		private static string ValidatePackagePropertyType(XmlDictionaryReader reader)
		{
			if (Operators.CompareString(reader.LocalName, "length", false) == 0)
			{
				if (!HasJsonType(reader, "number"))
				{
					return "An update package length must be a JSON number.";
				}
			}
			else if (!HasJsonType(reader, "string"))
			{
				return "An update package text field has the wrong JSON type.";
			}
			return null;
		}

		private static bool HasJsonType(XmlDictionaryReader reader, params string[] expectedTypes)
		{
			string attribute = reader.GetAttribute("type");
			foreach (string right in expectedTypes)
			{
				if (Operators.CompareString(attribute, right, false) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsRootProperty(string name)
		{
			switch (name)
			{
			case "schemaVersion":
			case "product":
			case "productId":
			case "channel":
			case "version":
			case "stage":
			case "informationalVersion":
			case "releaseDate":
			case "catalogueWriteFormat":
			case "published":
			case "releaseUrl":
			case "checksumManifest":
			case "packages":
				return true;
			default:
				return false;
			}
		}

		private static bool IsChecksumProperty(string name)
		{
			if (Operators.CompareString(name, "file", false) != 0 && Operators.CompareString(name, "length", false) != 0 && Operators.CompareString(name, "sha256", false) != 0)
			{
				return Operators.CompareString(name, "url", false) == 0;
			}
			return true;
		}

		private static bool IsPackageProperty(string name)
		{
			if (Operators.CompareString(name, "lane", false) != 0 && Operators.CompareString(name, "distribution", false) != 0 && Operators.CompareString(name, "file", false) != 0 && Operators.CompareString(name, "length", false) != 0 && Operators.CompareString(name, "sha256", false) != 0)
			{
				return Operators.CompareString(name, "url", false) == 0;
			}
			return true;
		}

		private static UpdateManifestReadResult Validate(UpdateReleaseManifestDocument document, string expectedChannel)
		{
			if (document == null)
			{
				return Invalid("The update manifest does not contain a JSON object.");
			}
			if (document.SchemaVersion != 1)
			{
				return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.UnsupportedSchema, "The update manifest schema is not supported.", null);
			}
			if (Operators.CompareString(document.Product, "Compact Cassette Catalogue", false) == 0 && Operators.CompareString(document.ProductId, "c3", false) == 0)
			{
				if (Operators.CompareString(document.Channel, expectedChannel, false) != 0)
				{
					return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.WrongChannel, "The update manifest belongs to a different channel.", null);
				}
				if (Operators.CompareString(document.Channel, "alpha", false) == 0 && document.Published)
				{
					return Invalid("Alpha update manifests must remain unpublished.");
				}
				if (!FieldsAreBounded(document))
				{
					return Invalid("The update manifest contains a missing or oversized field.");
				}
				SemanticVersion semanticVersion = null;
				if (SemanticVersion.TryParse(document.Version, ref semanticVersion) && !semanticVersion.HasPrerelease && Operators.CompareString(semanticVersion.ReleaseLabel, semanticVersion.CoreVersion, false) == 0 && Operators.CompareString(document.Version, semanticVersion.CoreVersion, false) == 0)
				{
					SemanticVersion semanticVersion2 = null;
					if (SemanticVersion.TryParse(document.InformationalVersion, ref semanticVersion2) && Operators.CompareString(semanticVersion2.CoreVersion, document.Version, false) == 0)
					{
						string text = null;
						string stageFamily = null;
						if (!TryGetStageIdentity(document.Stage, ref text, ref stageFamily))
						{
							return Invalid("The update manifest stage is invalid.");
						}
						string text2 = document.Version;
						if (text != null)
						{
							text2 = text2 + "-" + text;
						}
						if (Operators.CompareString(semanticVersion2.ReleaseLabel, text2, false) != 0)
						{
							return Invalid("The update manifest stage and release identity disagree.");
						}
						if (!ChannelAcceptsStage(document.Channel, stageFamily))
						{
							return Invalid("The update manifest stage does not belong to its channel.");
						}
						if (!DateTime.TryParseExact(document.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value))
						{
							return Invalid("The update manifest release date is invalid.");
						}
						SemanticVersion semanticVersion3 = null;
						if (SemanticVersion.TryParse(document.CatalogueWriteFormat, ref semanticVersion3) && !semanticVersion3.HasPrerelease && Operators.CompareString(semanticVersion3.ReleaseLabel, semanticVersion3.CoreVersion, false) == 0 && Operators.CompareString(document.CatalogueWriteFormat, semanticVersion3.CoreVersion, false) == 0)
						{
							string releaseUrl = null;
							UpdateChecksumManifest checksumManifest = null;
							IList<UpdateReleasePackage> packages = null;
							string text3 = ValidatePublication(document, semanticVersion2, ref releaseUrl, ref checksumManifest, ref packages);
							if (text3 != null)
							{
								return Invalid(text3);
							}
							return UpdateManifestReadResult.Succeeded(new UpdateReleaseManifest(document.Channel, document.Version, document.Stage, document.InformationalVersion, DateTime.SpecifyKind(value, DateTimeKind.Utc), document.CatalogueWriteFormat, document.Published, releaseUrl, checksumManifest, packages, semanticVersion2));
						}
						return Invalid("The update manifest catalogue format is invalid.");
					}
					return Invalid("The update manifest release identity is invalid.");
				}
				return Invalid("The update manifest product version is invalid.");
			}
			return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.WrongProduct, "The update manifest belongs to a different product.", null);
		}

		private static bool FieldsAreBounded(UpdateReleaseManifestDocument document)
		{
			if (IsBounded(document.Product) && IsBounded(document.ProductId) && IsBounded(document.Channel) && IsBounded(document.Version) && IsBounded(document.Stage) && IsBounded(document.InformationalVersion) && IsBounded(document.ReleaseDate))
			{
				return IsBounded(document.CatalogueWriteFormat);
			}
			return false;
		}

		private static bool IsBounded(string value)
		{
			if (value != null && value.Length > 0 && value.Length <= 512)
			{
				return Operators.CompareString(value, value.Trim(), false) == 0;
			}
			return false;
		}

		private static string ValidatePublication(UpdateReleaseManifestDocument document, SemanticVersion releaseIdentity, ref string releaseUrl, ref UpdateChecksumManifest checksumManifest, ref IList<UpdateReleasePackage> packages)
		{
			releaseUrl = null;
			checksumManifest = null;
			packages = null;
			if (document.Packages == null)
			{
				return "The update manifest packages field must be an array.";
			}
			if (!document.Published)
			{
				if (document.ReleaseUrl == null && document.ChecksumManifest == null && document.Packages.Length == 0)
				{
					packages = new List<UpdateReleasePackage>();
					return null;
				}
				return "An unpublished update manifest must not identify release assets.";
			}
			if (!IsBounded(document.ReleaseUrl))
			{
				return "A published update manifest must identify its release URL.";
			}
			string str = "v" + releaseIdentity.ReleaseLabel;
			string right = "https://github.com/Julesc013/compact-cassette-catalogue/releases/tag/" + str;
			if (Operators.CompareString(document.ReleaseUrl, right, false) != 0)
			{
				return "The published release URL does not match the release identity.";
			}
			if (document.ChecksumManifest == null)
			{
				return "A published update manifest must identify SHA256SUMS.txt.";
			}
			string str2 = "https://github.com/Julesc013/compact-cassette-catalogue/releases/download/" + str + "/";
			if (Operators.CompareString(document.ChecksumManifest.FileName, "SHA256SUMS.txt", false) == 0 && Operators.CompareString(document.ChecksumManifest.Url, str2 + "SHA256SUMS.txt", false) == 0)
			{
				if (document.ChecksumManifest.Length <= 0)
				{
					return "The checksum manifest length must be positive.";
				}
				if (!IsLowercaseSha256(document.ChecksumManifest.Sha256))
				{
					return "The checksum manifest SHA-256 value is invalid.";
				}
				if (document.Packages.Length != 0 && document.Packages.Length <= 16)
				{
					HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
					HashSet<string> hashSet2 = new HashSet<string>(StringComparer.Ordinal);
					List<UpdateReleasePackage> list = new List<UpdateReleasePackage>();
					UpdateReleasePackageDocument[] packages2 = document.Packages;
					int num = 0;
					string result;
					while (true)
					{
						if (num < packages2.Length)
						{
							UpdateReleasePackageDocument updateReleasePackageDocument = packages2[num];
							if (updateReleasePackageDocument == null)
							{
								return "A published update manifest contains an empty package entry.";
							}
							if (!IsValidLane(updateReleasePackageDocument.Lane))
							{
								return "An update package lane is invalid.";
							}
							if (!hashSet.Add(updateReleasePackageDocument.Lane))
							{
								return "The update manifest contains a duplicate package lane.";
							}
							if (Operators.CompareString(updateReleasePackageDocument.Distribution, "portable", false) != 0)
							{
								return "An update package distribution is invalid.";
							}
							string right2 = "C3-v" + releaseIdentity.ReleaseLabel + "-" + updateReleasePackageDocument.Lane + "-portable.zip";
							if (Operators.CompareString(updateReleasePackageDocument.FileName, right2, false) != 0)
							{
								return "An update package filename does not match its release and lane.";
							}
							if (!hashSet2.Add(updateReleasePackageDocument.FileName))
							{
								return "The update manifest contains a duplicate package filename.";
							}
							if (updateReleasePackageDocument.Length <= 0)
							{
								return "An update package length must be positive.";
							}
							if (!IsLowercaseSha256(updateReleasePackageDocument.Sha256))
							{
								result = "An update package SHA-256 value is invalid.";
								break;
							}
							string right3 = str2 + updateReleasePackageDocument.FileName;
							if (Operators.CompareString(updateReleasePackageDocument.Url, right3, false) != 0)
							{
								return "An update package URL does not match its release asset.";
							}
							list.Add(new UpdateReleasePackage(updateReleasePackageDocument.Lane, updateReleasePackageDocument.Distribution, updateReleasePackageDocument.FileName, updateReleasePackageDocument.Length, updateReleasePackageDocument.Sha256, updateReleasePackageDocument.Url));
							num = checked(num + 1);
							continue;
						}
						releaseUrl = document.ReleaseUrl;
						checksumManifest = new UpdateChecksumManifest(document.ChecksumManifest.FileName, document.ChecksumManifest.Length, document.ChecksumManifest.Sha256, document.ChecksumManifest.Url);
						packages = list;
						return null;
					}
					return result;
				}
				return "A published update manifest must identify between one and 16 packages.";
			}
			return "The checksum manifest identity or URL is invalid.";
		}

		private static bool IsValidLane(string value)
		{
			checked
			{
				if (value != null && value.Length != 0 && value.Length <= 64)
				{
					int num = value.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						char c = value[i];
						bool num2 = c >= 'a' && c <= 'z';
						bool flag = c >= '0' && c <= '9';
						if (!num2 && !flag && (i == 0 || (c != '.' && c != '-')))
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		private static bool IsLowercaseSha256(string value)
		{
			if (value != null && value.Length == 64)
			{
				foreach (char c in value)
				{
					if (c >= '0' && c <= '9')
					{
						continue;
					}
					if (c < 'a' || c > 'f')
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private static bool TryGetStageIdentity(string stage, ref string prerelease, ref string family)
		{
			prerelease = null;
			family = null;
			if (Operators.CompareString(stage, "Release", false) == 0)
			{
				family = "stable";
				return true;
			}
			int num = stage.LastIndexOf(' ');
			checked
			{
				if (num > 0 && num != stage.Length - 1)
				{
					string left = stage.Substring(0, num);
					string text = stage.Substring(num + 1);
					if (!IsPositiveCanonicalInteger(text))
					{
						return false;
					}
					if (Operators.CompareString(left, "Alpha", false) != 0)
					{
						if (Operators.CompareString(left, "Beta", false) != 0)
						{
							if (Operators.CompareString(left, "Release Candidate", false) == 0)
							{
								family = "rc";
								prerelease = "rc." + text;
								goto IL_00d2;
							}
							return false;
						}
						family = "beta";
						prerelease = "beta." + text;
					}
					else
					{
						family = "alpha";
						prerelease = "alpha." + text;
					}
					goto IL_00d2;
				}
				return false;
			}
			IL_00d2:
			return true;
		}

		private static bool IsPositiveCanonicalInteger(string value)
		{
			if (value.Length != 0 && value[0] != '0')
			{
				foreach (char c in value)
				{
					if (c < '0' || c > '9')
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private static bool ChannelAcceptsStage(string channel, string stageFamily)
		{
			if (Operators.CompareString(channel, "alpha", false) != 0)
			{
				if (Operators.CompareString(channel, "beta", false) != 0)
				{
					if (Operators.CompareString(channel, "stable", false) == 0)
					{
						return Operators.CompareString(stageFamily, "stable", false) == 0;
					}
					return false;
				}
				return Operators.CompareString(stageFamily, "beta", false) == 0 || Operators.CompareString(stageFamily, "rc", false) == 0;
			}
			return Operators.CompareString(stageFamily, "alpha", false) == 0;
		}

		private static bool IsSupportedChannel(string channel)
		{
			if (Operators.CompareString(channel, "alpha", false) != 0 && Operators.CompareString(channel, "beta", false) != 0)
			{
				return Operators.CompareString(channel, "stable", false) == 0;
			}
			return true;
		}

		private static UpdateManifestReadResult Malformed(Exception exception)
		{
			return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.MalformedJson, "The update manifest is not safe, well-formed JSON.", exception);
		}

		private static UpdateManifestReadResult Invalid(string message)
		{
			return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.InvalidManifest, message, null);
		}
	}
}
