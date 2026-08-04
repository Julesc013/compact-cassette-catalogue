using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateManifestReadResult
	{
		public UpdateReleaseManifest Manifest
		{
			get;
		}

		public UpdateManifestReadFailure Failure
		{
			get;
		}

		public string FailureMessage
		{
			get;
		}

		public Exception FailureException
		{
			get;
		}

		public bool IsSuccess => Manifest != null;

		private UpdateManifestReadResult(UpdateReleaseManifest manifest, UpdateManifestReadFailure failure, string failureMessage, Exception failureException)
		{
			Manifest = manifest;
			Failure = failure;
			FailureMessage = (failureMessage ?? string.Empty);
			FailureException = failureException;
		}

		internal static UpdateManifestReadResult Succeeded(UpdateReleaseManifest manifest)
		{
			return new UpdateManifestReadResult(manifest, UpdateManifestReadFailure.None, null, null);
		}

		internal static UpdateManifestReadResult Failed(UpdateManifestReadFailure failure, string message, Exception failureException = null)
		{
			return new UpdateManifestReadResult(null, failure, message, failureException);
		}
	}
}
