using System;
using System.Collections.Generic;
using System.IO;

namespace C3.Infrastructure.Preferences
{
	public sealed class LegacyUserSettingsImporter
	{
		private readonly LegacySettingsProfileLocator _locator;

		private readonly LegacySettingsProfileReader _reader;

		public LegacyUserSettingsImporter()
			: this(new LegacySettingsProfileLocator(), new LegacySettingsProfileReader())
		{
		}

		public LegacyUserSettingsImporter(LegacySettingsProfileLocator locator, LegacySettingsProfileReader reader)
		{
			if (locator == null)
			{
				throw new ArgumentNullException("locator");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			_locator = locator;
			_reader = reader;
		}

		public LegacyUserSettingsImportResult Import()
		{
			return Import(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
		}

		public LegacyUserSettingsImportResult Import(string localApplicationDataDirectory)
		{
			IList<LegacySettingsProfileCandidate> list = _locator.Locate(localApplicationDataDirectory);
			if (list.Count == 0)
			{
				return LegacyUserSettingsImportResult.NotFound();
			}
			List<LegacySettingsProfileReadResult> list2 = new List<LegacySettingsProfileReadResult>();
			foreach (LegacySettingsProfileCandidate item in list)
			{
				LegacySettingsProfileReadResult legacySettingsProfileReadResult = _reader.Read(item);
				if (legacySettingsProfileReadResult.IsSuccess)
				{
					return LegacyUserSettingsImportResult.Imported(legacySettingsProfileReadResult.Profile, list2);
				}
				list2.Add(legacySettingsProfileReadResult);
				if (legacySettingsProfileReadResult.Failure == LegacySettingsProfileReadFailure.Unavailable)
				{
					if (legacySettingsProfileReadResult.FailureException != null)
					{
						throw legacySettingsProfileReadResult.FailureException;
					}
					throw new IOException(legacySettingsProfileReadResult.FailureMessage);
				}
			}
			return LegacyUserSettingsImportResult.Failed(list2);
		}
	}
}
