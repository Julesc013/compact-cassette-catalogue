using Microsoft.VisualBasic.CompilerServices;
using System;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateEndpointPolicy
	{
		private const string AlphaChannel = "alpha";

		private const string BetaChannel = "beta";

		private const string StableChannel = "stable";

		private const string RepositoryPath = "/Julesc013/compact-cassette-catalogue/";

		private const string FeedHost = "raw.githubusercontent.com";

		private UpdateEndpointPolicy()
		{
		}

		public static string ExpectedUrl(string channel)
		{
			string text = BranchForChannel(channel);
			return "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/" + text + "/release/feeds/" + channel + "/release.json";
		}

		public static bool TryValidate(Uri feedUri, string expectedChannel, ref string failureMessage)
		{
			failureMessage = null;
			string text = null;
			try
			{
				text = ExpectedUrl(expectedChannel);
			}
			catch (ArgumentException ex)
			{
				ProjectData.SetProjectError(ex);
				ArgumentException ex2 = ex;
				failureMessage = ex2.Message;
				bool result = false;
				ProjectData.ClearProjectError();
				return result;
			}
			if ((object)feedUri != null && feedUri.IsAbsoluteUri)
			{
				if (!string.Equals(feedUri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal))
				{
					failureMessage = "The update manifest endpoint must use HTTPS.";
					return false;
				}
				if (!string.Equals(feedUri.Host, "raw.githubusercontent.com", StringComparison.Ordinal))
				{
					failureMessage = "The update manifest endpoint host is not trusted.";
					return false;
				}
				if (feedUri.Port == 443 && feedUri.IsDefaultPort)
				{
					if (feedUri.UserInfo.Length == 0 && feedUri.Query.Length == 0 && feedUri.Fragment.Length == 0)
					{
						if (!string.Equals(feedUri.OriginalString, text, StringComparison.Ordinal))
						{
							failureMessage = "The update manifest endpoint does not match the configured C3 channel.";
							return false;
						}
						return true;
					}
					failureMessage = "The update manifest endpoint must not contain credentials, a query, or a fragment.";
					return false;
				}
				failureMessage = "The update manifest endpoint must use the default HTTPS port.";
				return false;
			}
			failureMessage = "The update manifest endpoint must be an absolute URI.";
			return false;
		}

		public static void Validate(Uri feedUri, string expectedChannel)
		{
			string message = null;
			if (TryValidate(feedUri, expectedChannel, ref message))
			{
				return;
			}
			throw new ArgumentException(message, "feedUri");
		}

		private static string BranchForChannel(string channel)
		{
			if (Operators.CompareString(channel, "alpha", false) != 0)
			{
				if (Operators.CompareString(channel, "beta", false) != 0 && Operators.CompareString(channel, "stable", false) != 0)
				{
					throw new ArgumentException("The expected update channel is invalid.", "channel");
				}
				return "master";
			}
			return "dev/2.x";
		}
	}
}
