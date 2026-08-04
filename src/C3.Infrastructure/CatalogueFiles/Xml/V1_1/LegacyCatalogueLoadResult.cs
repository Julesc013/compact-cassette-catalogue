using C3.Catalogue.Catalogues;
using System.Data;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyCatalogueLoadResult
	{
		public bool IsSuccess
		{
			get;
			set;
		}

		public DataSet Document
		{
			get;
			set;
		}

		public CatalogueRevision Revision
		{
			get;
			set;
		}

		public string FileVersion
		{
			get;
			set;
		}

		public LegacyCatalogueFileFailure Failure
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		private LegacyCatalogueLoadResult()
		{
		}

		public static LegacyCatalogueLoadResult Success(DataSet document, CatalogueRevision revision, string fileVersion)
		{
			return new LegacyCatalogueLoadResult
			{
				IsSuccess = true,
				Document = document,
				Revision = revision,
				FileVersion = fileVersion,
				Failure = LegacyCatalogueFileFailure.None,
				Message = string.Empty
			};
		}

		public static LegacyCatalogueLoadResult Failed(LegacyCatalogueFileFailure failure, string message)
		{
			return new LegacyCatalogueLoadResult
			{
				IsSuccess = false,
				Failure = failure,
				Message = message
			};
		}
	}
}
