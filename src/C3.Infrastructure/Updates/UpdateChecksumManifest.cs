using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateChecksumManifest
	{
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

		internal UpdateChecksumManifest(string fileName, long length, string sha256, string url)
		{
			FileName = fileName;
			Length = length;
			Sha256 = sha256;
			Url = url;
		}
	}
}
