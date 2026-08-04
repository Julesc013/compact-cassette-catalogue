using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Diagnostics
{
	public sealed class CrashReportContext
	{
		public string ProductVersion
		{
			get;
			set;
		}

		public string ReleaseStage
		{
			get;
			set;
		}

		public string BuildLane
		{
			get;
			set;
		}

		public string OperatingSystem
		{
			get;
			set;
		}

		public string ClrVersion
		{
			get;
			set;
		}

		public string ProcessBitness
		{
			get;
			set;
		}

		public string CataloguePath
		{
			get;
			set;
		}

		public string LastAction
		{
			get;
			set;
		}
	}
}
