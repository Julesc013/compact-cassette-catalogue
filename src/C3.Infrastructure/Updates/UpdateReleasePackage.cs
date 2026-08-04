using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateReleasePackage
	{
		public string Lane
		{
			get;
		}

		public string Distribution
		{
			get;
		}

		public string FileName
		{
			get;
		}

		public long Length
		{
			get;
		}

		public string Sha256
		{
			get;
		}

		public string Url
		{
			get;
		}

		internal UpdateReleasePackage(string lane, string distribution, string fileName, long length, string sha256, string url)
		{
			Lane = lane;
			Distribution = distribution;
			FileName = fileName;
			Length = length;
			Sha256 = sha256;
			Url = url;
		}
	}
}
