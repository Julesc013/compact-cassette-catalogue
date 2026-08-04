using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace C3.Infrastructure.Updates
{
	[DataContract]
	internal sealed class UpdateChecksumManifestDocument
	{
		[DataMember(Name = "file", IsRequired = true, Order = 1)]
		public string FileName
		{
			get;
			set;
		}

		[DataMember(Name = "length", IsRequired = true, Order = 2)]
		public long Length
		{
			get;
			set;
		}

		[DataMember(Name = "sha256", IsRequired = true, Order = 3)]
		public string Sha256
		{
			get;
			set;
		}

		[DataMember(Name = "url", IsRequired = true, Order = 4)]
		public string Url
		{
			get;
			set;
		}
	}
}
