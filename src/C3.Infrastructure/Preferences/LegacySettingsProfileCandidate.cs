using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Preferences
{
	public sealed class LegacySettingsProfileCandidate
	{
		public string FilePath
		{
			get;
		}

		public Version ProfileVersion
		{
			get;
		}

		public DateTime LastWriteTimeUtc
		{
			get;
		}

		public LegacySettingsProfileCandidate(string filePath, Version profileVersion, DateTime lastWriteTimeUtc)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException("A settings profile path is required.", "filePath");
			}
			if ((object)profileVersion == null)
			{
				throw new ArgumentNullException("profileVersion");
			}
			FilePath = Path.GetFullPath(filePath);
			ProfileVersion = profileVersion;
			LastWriteTimeUtc = lastWriteTimeUtc.ToUniversalTime();
		}
	}
}
