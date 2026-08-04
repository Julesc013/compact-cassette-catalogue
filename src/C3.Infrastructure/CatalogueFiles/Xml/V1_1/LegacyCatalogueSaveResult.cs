using C3.Catalogue.Catalogues;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyCatalogueSaveResult
	{
		public bool IsSuccess
		{
			get;
			set;
		}

		public CatalogueRevision Revision
		{
			get;
			set;
		}

		public string BackupPath
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

		private LegacyCatalogueSaveResult()
		{
		}

		public static LegacyCatalogueSaveResult Success(CatalogueRevision revision, string backupPath)
		{
			return new LegacyCatalogueSaveResult
			{
				IsSuccess = true,
				Revision = revision,
				BackupPath = backupPath,
				Failure = LegacyCatalogueFileFailure.None,
				Message = string.Empty
			};
		}

		public static LegacyCatalogueSaveResult Failed(LegacyCatalogueFileFailure failure, string message)
		{
			return new LegacyCatalogueSaveResult
			{
				IsSuccess = false,
				Failure = failure,
				Message = message
			};
		}
	}
}
