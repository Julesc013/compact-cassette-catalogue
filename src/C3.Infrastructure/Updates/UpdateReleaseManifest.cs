using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateReleaseManifest
	{
		public string Channel
		{
			get;
		}

		public string ProductVersion
		{
			get;
		}

		public string Stage
		{
			get;
		}

		public string InformationalVersion
		{
			get;
		}

		public DateTime ReleaseDate
		{
			get;
		}

		public string CatalogueWriteFormat
		{
			get;
		}

		public bool Published
		{
			get;
		}

		public string ReleaseUrl
		{
			get;
		}

		public UpdateChecksumManifest ChecksumManifest
		{
			get;
		}

		public IList<UpdateReleasePackage> Packages
		{
			get;
		}

		public SemanticVersion ReleaseIdentity
		{
			get;
		}

		internal UpdateReleaseManifest(string channel, string productVersion, string stage, string informationalVersion, DateTime releaseDate, string catalogueWriteFormat, bool published, string releaseUrl, UpdateChecksumManifest checksumManifest, IList<UpdateReleasePackage> packages, SemanticVersion releaseIdentity)
		{
			if (packages == null)
			{
				throw new ArgumentNullException("packages");
			}
			Channel = channel;
			ProductVersion = productVersion;
			Stage = stage;
			InformationalVersion = informationalVersion;
			ReleaseDate = releaseDate;
			CatalogueWriteFormat = catalogueWriteFormat;
			Published = published;
			ReleaseUrl = releaseUrl;
			ChecksumManifest = checksumManifest;
			Packages = new List<UpdateReleasePackage>(packages).AsReadOnly();
			ReleaseIdentity = releaseIdentity;
		}
	}
}
