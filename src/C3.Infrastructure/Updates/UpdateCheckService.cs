using Microsoft.VisualBasic.CompilerServices;
using System;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateCheckService
	{
		private readonly IUpdateManifestSource _source;

		public UpdateCheckService()
			: this(new HttpUpdateManifestSource())
		{
		}

		public UpdateCheckService(IUpdateManifestSource source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			_source = source;
		}

		public UpdateCheckResult Check(string feedUrl, string currentInformationalVersion, string expectedChannel)
		{
			SemanticVersion currentIdentity = null;
			if (!SemanticVersion.TryParse(currentInformationalVersion, ref currentIdentity))
			{
				return UpdateCheckResult.Failed("The current C3 release identity is invalid.", null);
			}
			Uri feedUri = null;
			if (!Uri.TryCreate(feedUrl, UriKind.Absolute, out feedUri))
			{
				return UpdateCheckResult.Failed("The configured update manifest endpoint is invalid.", null);
			}
			string message = null;
			if (!UpdateEndpointPolicy.TryValidate(feedUri, expectedChannel, ref message))
			{
				return UpdateCheckResult.Failed(message, null);
			}
			try
			{
				UpdateManifestReadResult updateManifestReadResult = _source.Read(feedUri, expectedChannel);
				if (!updateManifestReadResult.IsSuccess)
				{
					return UpdateCheckResult.Failed(updateManifestReadResult.FailureMessage, updateManifestReadResult.FailureException);
				}
				return Evaluate(currentIdentity, updateManifestReadResult.Manifest);
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception failureException = ex;
				UpdateCheckResult result = UpdateCheckResult.Failed("The update manifest could not be downloaded.", failureException);
				ProjectData.ClearProjectError();
				return result;
			}
		}

		public static UpdateCheckResult Evaluate(string currentInformationalVersion, UpdateReleaseManifest manifest)
		{
			SemanticVersion currentIdentity = null;
			if (!SemanticVersion.TryParse(currentInformationalVersion, ref currentIdentity))
			{
				return UpdateCheckResult.Failed("The current C3 release identity is invalid.", null);
			}
			return Evaluate(currentIdentity, manifest);
		}

		private static UpdateCheckResult Evaluate(SemanticVersion currentIdentity, UpdateReleaseManifest manifest)
		{
			if (manifest == null)
			{
				return UpdateCheckResult.Failed("The update manifest is missing.", null);
			}
			if (!manifest.Published)
			{
				return UpdateCheckResult.Completed(UpdateCheckOutcome.NoPublishedRelease, manifest, "The update channel does not currently advertise a published release.");
			}
			if (manifest.ReleaseIdentity.CompareTo(currentIdentity) > 0)
			{
				return UpdateCheckResult.Completed(UpdateCheckOutcome.UpdateAvailable, manifest, "A newer published release is available.");
			}
			return UpdateCheckResult.Completed(UpdateCheckOutcome.UpToDate, manifest, "No newer published release is available.");
		}
	}
}
