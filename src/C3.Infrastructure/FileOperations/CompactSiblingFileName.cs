using System;
using System.Globalization;

namespace C3.Infrastructure.FileOperations
{
	internal sealed class CompactSiblingFileName
	{
		private CompactSiblingFileName()
		{
		}

		public static string CreateTemporary()
		{
			return "~c3" + CreateToken(13) + ".tmp";
		}

		public static string CreateRecovery(DateTime stamp)
		{
			return ".bad-" + stamp.ToUniversalTime().ToString("yyMMddHHmmss", CultureInfo.InvariantCulture) + CreateToken(3) + ".xml";
		}

		private static string CreateToken(int characterCount)
		{
			return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace('+', '-')
				.Replace('/', '_')
				.Substring(0, characterCount);
		}
	}
}
