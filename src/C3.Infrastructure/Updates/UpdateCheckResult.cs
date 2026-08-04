using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateCheckResult
	{
		public UpdateCheckOutcome Outcome
		{
			get;
		}

		public UpdateReleaseManifest Manifest
		{
			get;
		}

		public string Message
		{
			get;
		}

		public Exception FailureException
		{
			get;
		}

		public bool IsSuccess => Outcome != UpdateCheckOutcome.Failed;

		public bool IsUpdateAvailable => Outcome == UpdateCheckOutcome.UpdateAvailable;

		private UpdateCheckResult(UpdateCheckOutcome outcome, UpdateReleaseManifest manifest, string message, Exception failureException)
		{
			Outcome = outcome;
			Manifest = manifest;
			Message = (message ?? string.Empty);
			FailureException = failureException;
		}

		internal static UpdateCheckResult Completed(UpdateCheckOutcome outcome, UpdateReleaseManifest manifest, string message)
		{
			return new UpdateCheckResult(outcome, manifest, message, null);
		}

		internal static UpdateCheckResult Failed(string message, Exception failureException = null)
		{
			return new UpdateCheckResult(UpdateCheckOutcome.Failed, null, message, failureException);
		}
	}
}
