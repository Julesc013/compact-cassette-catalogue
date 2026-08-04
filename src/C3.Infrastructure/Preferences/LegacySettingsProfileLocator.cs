using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;

namespace C3.Infrastructure.Preferences
{
	public sealed class LegacySettingsProfileLocator
	{
		private sealed class CandidateComparer : IComparer<LegacySettingsProfileCandidate>
		{
			public int Compare(LegacySettingsProfileCandidate left, LegacySettingsProfileCandidate right)
			{
				if (object.ReferenceEquals(left, right))
				{
					return 0;
				}
				if (left == null)
				{
					return 1;
				}
				if (right == null)
				{
					return -1;
				}
				int num = right.ProfileVersion.CompareTo(left.ProfileVersion);
				if (num != 0)
				{
					return num;
				}
				int num2 = right.LastWriteTimeUtc.CompareTo(left.LastWriteTimeUtc);
				if (num2 != 0)
				{
					return num2;
				}
				return StringComparer.OrdinalIgnoreCase.Compare(left.FilePath, right.FilePath);
			}

			int IComparer<LegacySettingsProfileCandidate>.Compare(LegacySettingsProfileCandidate left, LegacySettingsProfileCandidate right)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Compare
				return this.Compare(left, right);
			}
		}

		private static readonly string[] KnownApplicationRootNames = new string[2]
		{
			"Compact_Cassette_Catalogu",
			"Compact_Cassette_Catalogue"
		};

		private static readonly string[] KnownEvidenceNameStems = new string[1]
		{
			"Compact_Cassette_Catalogu"
		};

		private static readonly string[] KnownEvidenceTypes = new string[3]
		{
			"Url",
			"Path",
			"StrongName"
		};

		private const int EvidenceHashLength = 32;

		public IList<LegacySettingsProfileCandidate> Locate()
		{
			return Locate(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
		}

		public IList<LegacySettingsProfileCandidate> Locate(string localApplicationDataDirectory)
		{
			if (string.IsNullOrWhiteSpace(localApplicationDataDirectory))
			{
				throw new ArgumentException("A LocalApplicationData directory is required.", "localApplicationDataDirectory");
			}
			string fullPath = Path.GetFullPath(localApplicationDataDirectory);
			List<LegacySettingsProfileCandidate> list = new List<LegacySettingsProfileCandidate>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string[] knownApplicationRootNames = KnownApplicationRootNames;
			foreach (string path in knownApplicationRootNames)
			{
				DirectoryInfo[] directoriesIfPresent = GetDirectoriesIfPresent(Path.Combine(fullPath, path));
				foreach (DirectoryInfo directoryInfo in directoriesIfPresent)
				{
					if (IsKnownEvidenceDirectory(directoryInfo.Name) && IsSafeDirectoryPresent(directoryInfo))
					{
						DirectoryInfo[] directoriesIfPresent2 = GetDirectoriesIfPresent(directoryInfo.FullName);
						foreach (DirectoryInfo directoryInfo2 in directoriesIfPresent2)
						{
							if (IsSafeDirectoryPresent(directoryInfo2))
							{
								Version profileVersion = null;
								if (IsLegacyVersionDirectory(directoryInfo2.Name, ref profileVersion))
								{
									string text = Path.Combine(directoryInfo2.FullName, "user.config");
									DateTime lastWriteTimeUtc = default(DateTime);
									if (hashSet.Add(text) && TryGetSettingsFile(text, ref lastWriteTimeUtc))
									{
										list.Add(new LegacySettingsProfileCandidate(text, profileVersion, lastWriteTimeUtc));
									}
								}
							}
						}
					}
				}
			}
			list.Sort(new CandidateComparer());
			return list.AsReadOnly();
		}

		private static DirectoryInfo[] GetDirectoriesIfPresent(string path)
		{
			try
			{
				FileAttributes attributes = File.GetAttributes(path);
				if ((attributes & FileAttributes.Directory) == (FileAttributes)0)
				{
					throw new IOException("The expected legacy settings root is not a directory: " + path);
				}
				if ((attributes & FileAttributes.ReparsePoint) != 0)
				{
					throw new IOException("A legacy settings directory is a reparse point: " + path);
				}
				return new DirectoryInfo(path).GetDirectories();
			}
			catch (FileNotFoundException ex)
			{
				ProjectData.SetProjectError(ex);
				FileNotFoundException ex2 = ex;
				DirectoryInfo[] result = new DirectoryInfo[0];
				ProjectData.ClearProjectError();
				return result;
			}
			catch (DirectoryNotFoundException ex3)
			{
				ProjectData.SetProjectError(ex3);
				DirectoryNotFoundException ex4 = ex3;
				DirectoryInfo[] result = new DirectoryInfo[0];
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static bool TryGetSettingsFile(string path, ref DateTime lastWriteTimeUtc)
		{
			try
			{
				FileAttributes attributes = File.GetAttributes(path);
				if ((attributes & FileAttributes.Directory) != 0)
				{
					throw new IOException("The legacy settings profile path is a directory: " + path);
				}
				if ((attributes & FileAttributes.ReparsePoint) != 0)
				{
					throw new IOException("The legacy settings profile is a reparse point: " + path);
				}
				lastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
				return true;
			}
			catch (FileNotFoundException ex)
			{
				ProjectData.SetProjectError(ex);
				FileNotFoundException ex2 = ex;
				bool result = false;
				ProjectData.ClearProjectError();
				return result;
			}
			catch (DirectoryNotFoundException ex3)
			{
				ProjectData.SetProjectError(ex3);
				DirectoryNotFoundException ex4 = ex3;
				bool result = false;
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static bool IsKnownEvidenceDirectory(string name)
		{
			string[] knownEvidenceNameStems = KnownEvidenceNameStems;
			foreach (string str in knownEvidenceNameStems)
			{
				string[] knownEvidenceTypes = KnownEvidenceTypes;
				foreach (string str2 in knownEvidenceTypes)
				{
					string text = str + "_" + str2 + "_";
					if (name.StartsWith(text, StringComparison.OrdinalIgnoreCase) && name.Length == checked(text.Length + 32) && IsFrameworkEvidenceHash(name.Substring(text.Length)))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool IsFrameworkEvidenceHash(string value)
		{
			for (int i = 0; i < value.Length; i = checked(i + 1))
			{
				char c = char.ToLowerInvariant(value[i]);
				if (c >= 'a' && c <= 'z')
				{
					continue;
				}
				if (c < '0' || c > '5')
				{
					return false;
				}
			}
			return value.Length == 32;
		}

		private static bool IsLegacyVersionDirectory(string directoryName, ref Version profileVersion)
		{
			if (Version.TryParse(directoryName, out profileVersion) && profileVersion.Build >= 0 && profileVersion.Revision >= 0)
			{
				if (profileVersion.Major >= 0 && profileVersion.Major <= 1)
				{
					return true;
				}
				profileVersion = null;
				return false;
			}
			profileVersion = null;
			return false;
		}

		private static bool IsSafeDirectoryPresent(FileSystemInfo fileSystemInfo)
		{
			try
			{
				if ((fileSystemInfo.Attributes & FileAttributes.ReparsePoint) != 0)
				{
					throw new IOException("A legacy settings directory is a reparse point: " + fileSystemInfo.FullName);
				}
				return true;
			}
			catch (FileNotFoundException ex)
			{
				ProjectData.SetProjectError(ex);
				FileNotFoundException ex2 = ex;
				bool result = false;
				ProjectData.ClearProjectError();
				return result;
			}
			catch (DirectoryNotFoundException ex3)
			{
				ProjectData.SetProjectError(ex3);
				DirectoryNotFoundException ex4 = ex3;
				bool result = false;
				ProjectData.ClearProjectError();
				return result;
			}
		}
	}
}
