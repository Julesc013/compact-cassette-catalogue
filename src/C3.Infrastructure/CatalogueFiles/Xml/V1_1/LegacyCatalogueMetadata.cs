using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyCatalogueMetadata
	{
		public string FileVersion
		{
			get;
			set;
		}

		public string ProductVersion
		{
			get;
			set;
		}

		public string ProductStage
		{
			get;
			set;
		}

		public DateTime ProductDate
		{
			get;
			set;
		}

		public DateTime CreatedAt
		{
			get;
			set;
		}
	}
}
