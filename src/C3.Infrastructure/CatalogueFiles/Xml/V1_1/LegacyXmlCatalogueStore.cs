using C3.Catalogue.Catalogues;
using C3.Infrastructure.FileOperations;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyXmlCatalogueStore
	{
		private const long MaximumCatalogueBytes = 67108864L;

		public LegacyCatalogueLoadResult Load(string path, DataSet schema, IEnumerable<string> supportedVersions)
		{
			if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
			{
				if (schema != null && schema.Tables.Count != 0)
				{
					try
					{
						if (new FileInfo(path).Length > 67108864)
						{
							return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.FileTooLarge, "The catalogue exceeds the 64 MiB safety limit.");
						}
						XmlDocument xmlDocument = LoadSecureDocument(path);
						string text = ValidateStructure(xmlDocument, schema);
						if (text != null)
						{
							return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.InvalidStructure, text);
						}
						string text2 = ReadFileVersion(xmlDocument);
						if (string.IsNullOrWhiteSpace(text2))
						{
							return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.MissingVersion, "The catalogue does not contain a file-format version.");
						}
						if (!supportedVersions.Contains(text2, StringComparer.Ordinal))
						{
							return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.UnsupportedVersion, "Catalogue format " + text2 + " is not supported by this version of C3.");
						}
						DataSet dataSet = schema.Clone();
						dataSet.DataSetName = schema.DataSetName;
						dataSet.EnforceConstraints = false;
						using (XmlReader reader = new XmlNodeReader(xmlDocument))
						{
							dataSet.ReadXml(reader, XmlReadMode.IgnoreSchema);
						}
						NormalizeCounters(dataSet);
						dataSet.EnforceConstraints = true;
						return LegacyCatalogueLoadResult.Success(dataSet, CalculateRevision(path), text2);
					}
					catch (XmlException ex)
					{
						ProjectData.SetProjectError(ex);
						XmlException ex2 = ex;
						LegacyCatalogueLoadResult result = LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.InvalidXml, ex2.Message);
						ProjectData.ClearProjectError();
						return result;
					}
					catch (ConstraintException ex3)
					{
						ProjectData.SetProjectError(ex3);
						ConstraintException ex4 = ex3;
						LegacyCatalogueLoadResult result = LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.ConstraintViolation, ex4.Message);
						ProjectData.ClearProjectError();
						return result;
					}
					catch (UnauthorizedAccessException ex5)
					{
						ProjectData.SetProjectError(ex5);
						UnauthorizedAccessException ex6 = ex5;
						LegacyCatalogueLoadResult result = LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.AccessDenied, ex6.Message);
						ProjectData.ClearProjectError();
						return result;
					}
					catch (IOException ex7)
					{
						ProjectData.SetProjectError(ex7);
						IOException ex8 = ex7;
						LegacyCatalogueLoadResult result = LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.IoFailure, ex8.Message);
						ProjectData.ClearProjectError();
						return result;
					}
					catch (Exception ex9)
					{
						ProjectData.SetProjectError(ex9);
						Exception ex10 = ex9;
						LegacyCatalogueLoadResult result = LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.InvalidStructure, ex10.Message);
						ProjectData.ClearProjectError();
						return result;
					}
				}
				return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.InvalidStructure, "A catalogue schema is required before loading.");
			}
			return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.FileNotFound, "The selected catalogue file does not exist.");
		}

		public LegacyCatalogueSaveResult Save(string path, DataSet document, CatalogueRevision expectedRevision, IEnumerable<string> supportedVersions)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.IoFailure, "A destination path is required.");
			}
			if (document != null && document.Tables.Count != 0)
			{
				string fullPath = Path.GetFullPath(path);
				string directoryName = Path.GetDirectoryName(fullPath);
				if (!string.IsNullOrWhiteSpace(directoryName) && Directory.Exists(directoryName))
				{
					string text = fullPath + ".bak";
					try
					{
						if (expectedRevision != null && (!File.Exists(fullPath) || !expectedRevision.Equals(CalculateRevision(fullPath))))
						{
							return LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.ExternalModification, "The catalogue changed on disk after it was opened. Save As a new file or reopen it before overwriting.");
						}
						DataSet dataSet = document.Copy();
						NormalizeCounters(dataSet);
						using (OwnedSiblingTemporaryFile ownedSiblingTemporaryFile = OwnedSiblingTemporaryFile.Create(fullPath))
						{
							using (FileStream fileStream = ownedSiblingTemporaryFile.Stream)
							{
								dataSet.WriteXml(fileStream, XmlWriteMode.IgnoreSchema);
								fileStream.Flush(true);
							}
							LegacyCatalogueLoadResult legacyCatalogueLoadResult = Load(ownedSiblingTemporaryFile.Path, dataSet.Clone(), supportedVersions);
							if (legacyCatalogueLoadResult.IsSuccess && AreEquivalent(dataSet, legacyCatalogueLoadResult.Document))
							{
								if (File.Exists(fullPath))
								{
									File.Replace(ownedSiblingTemporaryFile.Path, fullPath, text, true);
								}
								else
								{
									File.Move(ownedSiblingTemporaryFile.Path, fullPath);
									text = null;
								}
								return LegacyCatalogueSaveResult.Success(CalculateRevision(fullPath), text);
							}
							string str = legacyCatalogueLoadResult.IsSuccess ? "The saved snapshot did not round-trip without changes." : legacyCatalogueLoadResult.Message;
							return LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.VerificationFailure, "C3 verified the temporary output before replacement and rejected it. " + str);
						}
					}
					catch (UnauthorizedAccessException ex)
					{
						ProjectData.SetProjectError(ex);
						UnauthorizedAccessException ex2 = ex;
						LegacyCatalogueSaveResult result = LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.AccessDenied, ex2.Message);
						ProjectData.ClearProjectError();
						return result;
					}
					catch (IOException ex3)
					{
						ProjectData.SetProjectError(ex3);
						IOException ex4 = ex3;
						LegacyCatalogueSaveResult result = LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.IoFailure, ex4.Message);
						ProjectData.ClearProjectError();
						return result;
					}
					catch (Exception ex5)
					{
						ProjectData.SetProjectError(ex5);
						Exception ex6 = ex5;
						LegacyCatalogueSaveResult result = LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.VerificationFailure, ex6.Message);
						ProjectData.ClearProjectError();
						return result;
					}
				}
				return LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.IoFailure, "The destination directory does not exist.");
			}
			return LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.InvalidStructure, "There is no catalogue document to save.");
		}

		private static XmlDocument LoadSecureDocument(string path)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
			xmlReaderSettings.XmlResolver = null;
			xmlReaderSettings.MaxCharactersInDocument = 67108864L;
			xmlReaderSettings.MaxCharactersFromEntities = 0L;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.XmlResolver = null;
			using (XmlReader reader = XmlReader.Create(path, xmlReaderSettings))
			{
				xmlDocument.Load(reader);
				return xmlDocument;
			}
		}

		private static string ValidateStructure(XmlDocument document, DataSet schema)
		{
			if (document.DocumentElement != null && string.Equals(document.DocumentElement.Name, "Catalogue", StringComparison.Ordinal) && string.IsNullOrEmpty(document.DocumentElement.NamespaceURI))
			{
				IEnumerator enumerator = default(IEnumerator);
				try
				{
					enumerator = document.DocumentElement.ChildNodes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						XmlNode xmlNode = (XmlNode)enumerator.Current;
						if (xmlNode.NodeType == XmlNodeType.Element)
						{
							if (!string.IsNullOrEmpty(xmlNode.NamespaceURI))
							{
								return "Catalogue row '" + xmlNode.Name + "' must be unqualified.";
							}
							DataTable dataTable = schema.Tables[xmlNode.Name];
							if (dataTable == null)
							{
								return "Unknown catalogue table '" + xmlNode.Name + "'.";
							}
							IEnumerator enumerator2 = default(IEnumerator);
							try
							{
								enumerator2 = xmlNode.ChildNodes.GetEnumerator();
								while (enumerator2.MoveNext())
								{
									XmlNode xmlNode2 = (XmlNode)enumerator2.Current;
									if (xmlNode2.NodeType == XmlNodeType.Element)
									{
										if (!string.IsNullOrEmpty(xmlNode2.NamespaceURI) || dataTable.Columns[xmlNode2.Name] == null)
										{
											return "Unknown field '" + xmlNode2.Name + "' in table '" + xmlNode.Name + "'.";
										}
										IEnumerator enumerator3 = default(IEnumerator);
										try
										{
											enumerator3 = xmlNode2.ChildNodes.GetEnumerator();
											while (enumerator3.MoveNext())
											{
												if (((XmlNode)enumerator3.Current).NodeType == XmlNodeType.Element)
												{
													return "Field '" + xmlNode2.Name + "' in table '" + xmlNode.Name + "' must contain a scalar value.";
												}
											}
										}
										finally
										{
											if (enumerator3 is IDisposable)
											{
												(enumerator3 as IDisposable).Dispose();
											}
										}
									}
								}
							}
							finally
							{
								if (enumerator2 is IDisposable)
								{
									(enumerator2 as IDisposable).Dispose();
								}
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
				return null;
			}
			return "The document root must be an unqualified Catalogue element.";
		}

		private static string ReadFileVersion(XmlDocument document)
		{
			XmlNode xmlNode = document.SelectSingleNode("/Catalogue/Information[normalize-space(Information)='File Version']/Value");
			if (xmlNode == null)
			{
				return null;
			}
			string text = xmlNode.InnerText.Trim();
			Match match = Regex.Match(text, "^(\\d+)\\.(\\d+)\\.(\\d+)");
			if (match.Success)
			{
				return match.Groups[1].Value + "." + match.Groups[2].Value + "." + match.Groups[3].Value;
			}
			return text;
		}

		private static void NormalizeCounters(DataSet document)
		{
			DataTable dataTable = document.Tables["Counters"];
			if (dataTable != null)
			{
				SetCounter(dataTable, "Decks", RowCount(document, "Decks"));
				SetCounter(dataTable, "Brands", RowCount(document, "Brands"));
				SetCounter(dataTable, "Models", RowCount(document, "Models"));
				SetCounter(dataTable, "Tapes", RowCount(document, "Tapes"));
			}
		}

		private static int RowCount(DataSet document, string tableName)
		{
			DataTable dataTable = document.Tables[tableName];
			return dataTable?.Rows.Count ?? 0;
		}

		private static void SetCounter(DataTable table, string name, int value)
		{
			DataRow dataRow = null;
			if (table.PrimaryKey.Length > 0)
			{
				dataRow = table.Rows.Find(name);
			}
			else
			{
				IEnumerator enumerator = default(IEnumerator);
				try
				{
					enumerator = table.Rows.GetEnumerator();
					while (enumerator.MoveNext())
					{
						DataRow dataRow2 = (DataRow)enumerator.Current;
						if (string.Equals(Conversions.ToString(dataRow2["Counter"]), name, StringComparison.Ordinal))
						{
							dataRow = dataRow2;
							break;
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
			}
			if (dataRow == null)
			{
				dataRow = table.NewRow();
				dataRow["Counter"] = name;
				table.Rows.Add(dataRow);
			}
			dataRow["Number"] = value;
		}

		private static CatalogueRevision CalculateRevision(string path)
		{
			checked
			{
				using (FileStream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (SHA256 sHA = SHA256.Create())
					{
						byte[] array = sHA.ComputeHash(inputStream);
						StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
						byte[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							byte b = array2[i];
							stringBuilder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
						}
						return new CatalogueRevision(stringBuilder.ToString());
					}
				}
			}
		}

		private static bool AreEquivalent(DataSet expected, DataSet actual)
		{
			if (expected.Tables.Count != actual.Tables.Count)
			{
				return false;
			}
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = expected.Tables.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataTable dataTable = (DataTable)enumerator.Current;
					DataTable dataTable2 = actual.Tables[dataTable.TableName];
					if (dataTable2 == null || dataTable.Columns.Count != dataTable2.Columns.Count || dataTable.Rows.Count != dataTable2.Rows.Count)
					{
						return false;
					}
					int num = checked(dataTable.Rows.Count - 1);
					for (int i = 0; i <= num; i = checked(i + 1))
					{
						IEnumerator enumerator2 = default(IEnumerator);
						try
						{
							enumerator2 = dataTable.Columns.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								DataColumn dataColumn = (DataColumn)enumerator2.Current;
								if (!object.Equals(RuntimeHelpers.GetObjectValue(dataTable.Rows[i][dataColumn.ColumnName]), RuntimeHelpers.GetObjectValue(dataTable2.Rows[i][dataColumn.ColumnName])))
								{
									return false;
								}
							}
						}
						finally
						{
							if (enumerator2 is IDisposable)
							{
								(enumerator2 as IDisposable).Dispose();
							}
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
			return true;
		}
	}
}
