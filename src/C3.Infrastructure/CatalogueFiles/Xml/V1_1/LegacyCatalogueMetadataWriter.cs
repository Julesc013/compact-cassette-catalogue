using System;
using System.Data;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyCatalogueMetadataWriter
	{
		private readonly Func<DataSet> _documentProvider;

		public LegacyCatalogueMetadataWriter(Func<DataSet> documentProvider)
		{
			if (documentProvider == null)
			{
				throw new ArgumentNullException("documentProvider");
			}
			_documentProvider = documentProvider;
		}

		public void MarkModified(DateTime modifiedAt)
		{
			SetValue("File Modified", modifiedAt.ToString());
		}

		public void RefreshProductMetadata(string productVersion, string productStage, DateTime productDate)
		{
			SetValue("Program Version", productVersion);
			SetValue("Program Stage", productStage);
			SetValue("Program Date", productDate.ToString());
		}

		private void SetValue(string name, string value)
		{
			DataRow dataRow = InformationTable().Rows.Find(name);
			if (dataRow == null)
			{
				throw new InvalidOperationException("Catalogue information row '" + name + "' is missing.");
			}
			dataRow["Value"] = (value ?? string.Empty);
		}

		private DataTable InformationTable()
		{
			DataSet dataSet = _documentProvider();
			if (dataSet == null)
			{
				throw new InvalidOperationException("No active catalogue document is available.");
			}
			DataTable dataTable = dataSet.Tables["Information"];
			if (dataTable == null)
			{
				throw new InvalidOperationException("Catalogue table 'Information' is missing.");
			}
			return dataTable;
		}
	}
}
