using C3.Catalogue.Brands;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyBrandRepository : IBrandRepository
	{
		private readonly Func<DataSet> _documentProvider;

		public LegacyBrandRepository(Func<DataSet> documentProvider)
		{
			if (documentProvider == null)
			{
				throw new ArgumentNullException("documentProvider");
			}
			_documentProvider = documentProvider;
		}

		public IList<Brand> GetAll()
		{
			List<Brand> list = new List<Brand>();
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = BrandsTable().Rows.GetEnumerator();
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

		IList<Brand> IBrandRepository.GetAll()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetAll
			return this.GetAll();
		}

		public Brand FindByCode(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				return null;
			}
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = BrandsTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(Conversions.ToString(dataRow["Code"]), code, StringComparison.OrdinalIgnoreCase))
					{
						return Map(dataRow);
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

		Brand IBrandRepository.FindByCode(string code)
		{
			//ILSpy generated this explicit interface implementation from .override directive in FindByCode
			return this.FindByCode(code);
		}

		public bool IsCodeInUse(string code)
		{
			return FindByCode(code) != null;
		}

		bool IBrandRepository.IsCodeInUse(string code)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IsCodeInUse
			return this.IsCodeInUse(code);
		}

		public bool IsReferencedByModel(string code)
		{
			DataRow dataRow = FindRow(code);
			string text = string.Empty;
			if (dataRow != null)
			{
				text = Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow["Brand"]));
			}
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = ModelsTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow2 = (DataRow)enumerator.Current;
					if (dataRow2.RowState != DataRowState.Deleted)
					{
						string a = Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow2["Brand"]));
						if (!string.Equals(a, code, StringComparison.OrdinalIgnoreCase) && (text.Length <= 0 || !string.Equals(a, text, StringComparison.OrdinalIgnoreCase)))
						{
							continue;
						}
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

		bool IBrandRepository.IsReferencedByModel(string code)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IsReferencedByModel
			return this.IsReferencedByModel(code);
		}

		public void Add(Brand value)
		{
			BrandsTable().Rows.Add(value.Name, value.Code, value.AddedAt, value.Notes);
			SynchronizeBrandCounter();
		}

		void IBrandRepository.Add(Brand value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Add
			this.Add(value);
		}

		public void Update(Brand value)
		{
			DataRow dataRow = FindRow(value.Code);
			if (dataRow == null)
			{
				throw new InvalidOperationException("The selected brand no longer exists.");
			}
			string b = Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow["Brand"]));
			dataRow["Brand"] = value.Name;
			dataRow["Date"] = value.AddedAt;
			dataRow["Notes"] = value.Notes;
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = ModelsTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow2 = (DataRow)enumerator.Current;
					if (dataRow2.RowState != DataRowState.Deleted && string.Equals(Convert.ToString(RuntimeHelpers.GetObjectValue(dataRow2["Brand"])), b, StringComparison.OrdinalIgnoreCase))
					{
						dataRow2["Brand"] = value.Code;
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
		}

		void IBrandRepository.Update(Brand value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Update
			this.Update(value);
		}

		public void Delete(string code)
		{
			DataRow dataRow = FindRow(code);
			if (dataRow == null)
			{
				throw new InvalidOperationException("The selected brand no longer exists.");
			}
			BrandsTable().Rows.Remove(dataRow);
			SynchronizeBrandCounter();
		}

		void IBrandRepository.Delete(string code)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Delete
			this.Delete(code);
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

		private DataTable BrandsTable()
		{
			return RequireTable("Brands");
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

		private DataRow FindRow(string code)
		{
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = BrandsTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(Conversions.ToString(dataRow["Code"]), code, StringComparison.OrdinalIgnoreCase))
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

		private static Brand Map(DataRow row)
		{
			DateTime addedAt = DateTime.MinValue;
			if (!row.IsNull("Date"))
			{
				addedAt = Convert.ToDateTime(RuntimeHelpers.GetObjectValue(row["Date"]));
			}
			return new Brand(Convert.ToString(RuntimeHelpers.GetObjectValue(row["Brand"])), Convert.ToString(RuntimeHelpers.GetObjectValue(row["Code"])), addedAt, Convert.ToString(RuntimeHelpers.GetObjectValue(row["Notes"])));
		}

		private void SynchronizeBrandCounter()
		{
			DataRow dataRow = RequireTable("Counters").Rows.Find("Brands");
			if (dataRow != null)
			{
				dataRow["Number"] = BrandsTable().Rows.Count;
			}
		}
	}
}
