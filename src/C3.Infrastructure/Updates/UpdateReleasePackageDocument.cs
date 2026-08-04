using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace C3.Infrastructure.Updates
{
	[DataContract]
	internal sealed class UpdateReleasePackageDocument
	{
		[DataMember(Name = "lane", IsRequired = true, Order = 1)]
		public string Lane
		{
			get;
			set;
		}

		[DataMember(Name = "distribution", IsRequired = true, Order = 2)]
		public string Distribution
		{
			get;
			set;
		}

		[DataMember(Name = "file", IsRequired = true, Order = 3)]
		public string FileName
		{
			get;
			set;
		}

		[DataMember(Name = "length", IsRequired = true, Order = 4)]
		public long Length
		{
			get;
			set;
		}

		[DataMember(Name = "sha256", IsRequired = true, Order = 5)]
		public string Sha256
		{
			get;
			set;
		}

		[DataMember(Name = "url", IsRequired = true, Order = 6)]
		public string Url
		{
			get;
			set;
		}
	}
}
