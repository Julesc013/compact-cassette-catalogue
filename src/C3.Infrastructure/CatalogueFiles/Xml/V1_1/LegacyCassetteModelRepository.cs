using C3.Catalogue.CassetteModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyCassetteModelRepository : ICassetteModelRepository
	{
		private readonly Func<DataSet> _documentProvider;

		public LegacyCassetteModelRepository(Func<DataSet> documentProvider)
		{
			if (documentProvider == null)
			{
				throw new ArgumentNullException("documentProvider");
			}
			_documentProvider = documentProvider;
		}

		public IList<CassetteModel> GetAll()
		{
			List<CassetteModel> list = new List<CassetteModel>();
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = ModelsTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted)
					{
						list.Add(Map(dataRow));
					}
				}
				return list;
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
		}

		IList<CassetteModel> ICassetteModelRepository.GetAll()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetAll
			return this.GetAll();
		}

		public CassetteModel FindByIdentifier(string identifier)
		{
			DataRow dataRow = FindRow(identifier);
			if (dataRow == null)
			{
				return null;
			}
			return Map(dataRow);
		}

		CassetteModel ICassetteModelRepository.FindByIdentifier(string identifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in FindByIdentifier
			return this.FindByIdentifier(identifier);
		}

		public bool BrandExists(string code)
		{
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = RequireTable("Brands").Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow["Code"])), code, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			return false;
		}

		bool ICassetteModelRepository.BrandExists(string code)
		{
			//ILSpy generated this explicit interface implementation from .override directive in BrandExists
			return this.BrandExists(code);
		}

		public bool IdentifierExists(string identifier)
		{
			return FindRow(identifier) != null;
		}

		bool ICassetteModelRepository.IdentifierExists(string identifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IdentifierExists
			return this.IdentifierExists(identifier);
		}

		public bool IsReferencedByTape(string identifier)
		{
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = RequireTable("Tapes").Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow["Model"])), identifier, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			return false;
		}

		bool ICassetteModelRepository.IsReferencedByTape(string identifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IsReferencedByTape
			return this.IsReferencedByTape(identifier);
		}

		public void Add(CassetteModel value)
		{
			ModelsTable().Rows.Add(value.BrandCode, value.TypeNumber, value.ModelName, value.Code, value.Identifier, value.DisplayName, value.TapeCount, value.AddedAt, value.Notes);
			SynchronizeModelCounter();
		}

		void ICassetteModelRepository.Add(CassetteModel value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Add
			this.Add(value);
		}

		public void Update(CassetteModel value)
		{
			DataRow dataRow = FindRow(value.Identifier);
			if (dataRow == null)
			{
				throw new InvalidOperationException("The selected cassette model no longer exists.");
			}
			dataRow["Model"] = value.ModelName;
			dataRow["Name"] = value.DisplayName;
			dataRow["Notes"] = value.Notes;
		}

		void ICassetteModelRepository.Update(CassetteModel value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Update
			this.Update(value);
		}

		public void Delete(string identifier)
		{
			DataRow dataRow = FindRow(identifier);
			if (dataRow == null)
			{
				throw new InvalidOperationException("The selected cassette model no longer exists.");
			}
			ModelsTable().Rows.Remove(dataRow);
			SynchronizeModelCounter();
		}

		void ICassetteModelRepository.Delete(string identifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Delete
			this.Delete(identifier);
		}

		private DataSet Document()
		{
			DataSet dataSet = _documentProvider();
			if (dataSet == null)
			{
				throw new InvalidOperationException("No active catalogue document is available.");
			}
			return dataSet;
		}

		private DataTable ModelsTable()
		{
			return RequireTable("Models");
		}

		private DataTable RequireTable(string name)
		{
			DataTable dataTable = Document().Tables[name];
			if (dataTable == null)
			{
				throw new InvalidOperationException("Catalogue table '" + name + "' is missing.");
			}
			return dataTable;
		}

		private DataRow FindRow(string identifier)
		{
			if (string.IsNullOrWhiteSpace(identifier))
			{
				return null;
			}
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = ModelsTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow["Identifier"])), identifier, StringComparison.OrdinalIgnoreCase))
					{
						return dataRow;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			return null;
		}

		private CassetteModel Map(DataRow row)
		{
			DateTime addedAt = DateTime.MinValue;
			if (!row.IsNull("Date"))
			{
				addedAt = Convert.ToDateTime(RuntimeHelpers.GetObjectValue(row["Date"]));
			}
			return new CassetteModel(ResolveBrandCode(Convert.ToString(RuntimeHelpers.GetObjectValue(row["Brand"]))), ReadInteger(row, "Type"), Convert.ToString(RuntimeHelpers.GetObjectValue(row["Model"])), Convert.ToString(RuntimeHelpers.GetObjectValue(row["Code"])), Convert.ToString(RuntimeHelpers.GetObjectValue(row["Identifier"])), Convert.ToString(RuntimeHelpers.GetObjectValue(row["Name"])), ReadInteger(row, "Number"), addedAt, Convert.ToString(RuntimeHelpers.GetObjectValue(row["Notes"])));
		}

		private string ResolveBrandCode(string storedValue)
		{
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = RequireTable("Brands").Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted)
					{
						string text = Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow["Code"]));
						string b = Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow["Brand"]));
						if (!string.Equals(storedValue, text, StringComparison.OrdinalIgnoreCase) && !string.Equals(storedValue, b, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						return text;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			return storedValue;
		}

		private static int ReadInteger(DataRow row, string columnName)
		{
			if (row.IsNull(columnName))
			{
				return 0;
			}
			return Convert.ToInt32(RuntimeHelpers.GetObjectValue(row[columnName]));
		}

		private void SynchronizeModelCounter()
		{
			DataRow dataRow = RequireTable("Counters").Rows.Find("Models");
			if (dataRow != null)
			{
				dataRow["Number"] = ModelsTable().Rows.Count;
			}
		}
	}
}
