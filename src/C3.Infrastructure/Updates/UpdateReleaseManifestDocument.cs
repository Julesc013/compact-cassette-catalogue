using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace C3.Infrastructure.Updates
{
	[DataContract]
	internal sealed class UpdateReleaseManifestDocument
	{
		[DataMember(Name = "schemaVersion", IsRequired = true, Order = 1)]
		public int SchemaVersion
		{
			get;
			set;
		}

		[DataMember(Name = "product", IsRequired = true, Order = 2)]
		public string Product
		{
			get;
			set;
		}

		[DataMember(Name = "productId", IsRequired = true, Order = 3)]
		public string ProductId
		{
			get;
			set;
		}

		[DataMember(Name = "channel", IsRequired = true, Order = 4)]
		public string Channel
		{
			get;
			set;
		}

		[DataMember(Name = "version", IsRequired = true, Order = 5)]
		public string Version
		{
			get;
			set;
		}

		[DataMember(Name = "stage", IsRequired = true, Order = 6)]
		public string Stage
		{
			get;
			set;
		}

		[DataMember(Name = "informationalVersion", IsRequired = true, Order = 7)]
		public string InformationalVersion
		{
			get;
			set;
		}

		[DataMember(Name = "releaseDate", IsRequired = true, Order = 8)]
		public string ReleaseDate
		{
			get;
			set;
		}

		[DataMember(Name = "catalogueWriteFormat", IsRequired = true, Order = 9)]
		public string CatalogueWriteFormat
		{
			get;
			set;
		}

		[DataMember(Name = "published", IsRequired = true, Order = 10)]
		public bool Published
		{
			get;
			set;
		}

		[DataMember(Name = "releaseUrl", IsRequired = true, Order = 11)]
		public string ReleaseUrl
		{
			get;
			set;
		}

		[DataMember(Name = "checksumManifest", IsRequired = true, Order = 12)]
		public UpdateChecksumManifestDocument ChecksumManifest
		{
			get;
			set;
		}

		[DataMember(Name = "packages", IsRequired = true, Order = 13)]
		public UpdateReleasePackageDocument[] Packages
		{
			get;
			set;
		}
	}
}
