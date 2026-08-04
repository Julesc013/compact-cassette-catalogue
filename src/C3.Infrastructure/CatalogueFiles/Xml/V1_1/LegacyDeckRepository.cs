using C3.Catalogue.Decks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyDeckRepository : IDeckRepository
	{
		private readonly Func<DataSet> _documentProvider;

		public LegacyDeckRepository(Func<DataSet> documentProvider)
		{
			if (documentProvider == null)
			{
				throw new ArgumentNullException("documentProvider");
			}
			_documentProvider = documentProvider;
		}

		public IList<Deck> GetAll()
		{
			List<Deck> list = new List<Deck>();
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = DecksTable().Rows.GetEnumerator();
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

		IList<Deck> IDeckRepository.GetAll()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetAll
			return this.GetAll();
		}

		public Deck FindByName(string name)
		{
			DataRow dataRow = FindRow(name);
			if (dataRow == null)
			{
				return null;
			}
			return Map(dataRow);
		}

		Deck IDeckRepository.FindByName(string name)
		{
			//ILSpy generated this explicit interface implementation from .override directive in FindByName
			return this.FindByName(name);
		}

		public bool NameExists(string name)
		{
			return FindRow(name) != null;
		}

		bool IDeckRepository.NameExists(string name)
		{
			//ILSpy generated this explicit interface implementation from .override directive in NameExists
			return this.NameExists(name);
		}

		public bool IsReferencedByTape(string name)
		{
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = RequireTable("Tapes").Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && (string.Equals(ReadString(dataRow, "DeckA"), name, StringComparison.OrdinalIgnoreCase) || string.Equals(ReadString(dataRow, "DeckB"), name, StringComparison.OrdinalIgnoreCase)))
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

		bool IDeckRepository.IsReferencedByTape(string name)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IsReferencedByTape
			return this.IsReferencedByTape(name);
		}

		public void Add(Deck value)
		{
			DataRow dataRow = DecksTable().NewRow();
			dataRow["Name"] = value.Name;
			dataRow["Date"] = value.AddedAt;
			WriteDetails(dataRow, value.Details);
			DecksTable().Rows.Add(dataRow);
			SynchronizeDeckCounter();
		}

		void IDeckRepository.Add(Deck value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Add
			this.Add(value);
		}

		public void Update(Deck value)
		{
			DataRow dataRow = FindRow(value.Name);
			if (dataRow == null)
			{
				throw new InvalidOperationException("The selected deck no longer exists.");
			}
			WriteDetails(dataRow, value.Details);
		}

		void IDeckRepository.Update(Deck value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Update
			this.Update(value);
		}

		public void Delete(string name)
		{
			DataRow dataRow = FindRow(name);
			if (dataRow == null)
			{
				throw new InvalidOperationException("The selected deck no longer exists.");
			}
			DecksTable().Rows.Remove(dataRow);
			SynchronizeDeckCounter();
		}

		void IDeckRepository.Delete(string name)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Delete
			this.Delete(name);
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

		private DataTable DecksTable()
		{
			return RequireTable("Decks");
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

		private DataRow FindRow(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = DecksTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(ReadString(dataRow, "Name"), name, StringComparison.OrdinalIgnoreCase))
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

		private static Deck Map(DataRow row)
		{
			DeckDetails details = new DeckDetails(ReadString(row, "Manufacturer"), ReadString(row, "Model"), ReadInteger(row, "Year"), ReadInteger(row, "Condition"), ReadBoolean(row, "Type1"), ReadBoolean(row, "Type2"), ReadBoolean(row, "Type3"), ReadBoolean(row, "Type4"), ReadBoolean(row, "HX"), ReadBoolean(row, "MPX"), ReadBoolean(row, "DolbyB"), ReadBoolean(row, "DolbyC"), ReadBoolean(row, "DolbyS"), ReadBoolean(row, "DBX1"), ReadBoolean(row, "DBX2"), ReadBoolean(row, "Stereo"), ReadBoolean(row, "ProgramSearch"), ReadBoolean(row, "Reverse"), ReadBoolean(row, "Calibration"), ReadBoolean(row, "Azimuth"), ReadBoolean(row, "DubbingSlow"), ReadBoolean(row, "DubbingFast"), ReadInteger(row, "FrequencyLow"), ReadInteger(row, "FrequencyHigh"), ReadInteger(row, "SignalRatio"), ReadString(row, "SignalRatioNR"), ReadDecimal(row, "WowFlutter"), ReadDecimal(row, "Distortion"), ReadInteger(row, "Heads"), ReadInteger(row, "Wells"), ReadBoolean(row, "SpeedSlow"), ReadBoolean(row, "SpeedNorm"), ReadBoolean(row, "SpeedFast"), ReadString(row, "Notes"));
			return new Deck(ReadString(row, "Name"), ReadDate(row, "Date"), details);
		}

		private static void WriteDetails(DataRow row, DeckDetails value)
		{
			row["Manufacturer"] = value.Manufacturer;
			row["Model"] = value.Model;
			row["Year"] = value.Year;
			row["Condition"] = value.Condition;
			row["Type1"] = value.Type1;
			row["Type2"] = value.Type2;
			row["Type3"] = value.Type3;
			row["Type4"] = value.Type4;
			row["HX"] = value.Hx;
			row["MPX"] = value.Mpx;
			row["DolbyB"] = value.DolbyB;
			row["DolbyC"] = value.DolbyC;
			row["DolbyS"] = value.DolbyS;
			row["DBX1"] = value.Dbx1;
			row["DBX2"] = value.Dbx2;
			row["Stereo"] = value.Stereo;
			row["ProgramSearch"] = value.ProgramSearch;
			row["Reverse"] = value.Reverse;
			row["Calibration"] = value.Calibration;
			row["Azimuth"] = value.Azimuth;
			row["DubbingSlow"] = value.DubbingSlow;
			row["DubbingFast"] = value.DubbingFast;
			row["FrequencyLow"] = value.FrequencyLow;
			row["FrequencyHigh"] = value.FrequencyHigh;
			row["SignalRatio"] = value.SignalRatio;
			row["SignalRatioNR"] = value.SignalRatioNoiseReduction;
			row["WowFlutter"] = value.WowFlutter;
			row["Distortion"] = value.Distortion;
			row["Heads"] = value.Heads;
			row["Wells"] = value.Wells;
			row["SpeedSlow"] = value.SpeedSlow;
			row["SpeedNorm"] = value.SpeedNormal;
			row["SpeedFast"] = value.SpeedFast;
			row["Notes"] = value.Notes;
		}

		private static string ReadString(DataRow row, string name)
		{
			if (!row.IsNull(name))
			{
				return Convert.ToString(RuntimeHelpers.GetObjectValue(row[name]));
			}
			return string.Empty;
		}

		private static int ReadInteger(DataRow row, string name)
		{
			if (!row.IsNull(name))
			{
				return Convert.ToInt32(RuntimeHelpers.GetObjectValue(row[name]));
			}
			return 0;
		}

		private static bool ReadBoolean(DataRow row, string name)
		{
			if (!row.IsNull(name))
			{
				return Convert.ToBoolean(RuntimeHelpers.GetObjectValue(row[name]));
			}
			return false;
		}

		private static decimal ReadDecimal(DataRow row, string name)
		{
			if (!row.IsNull(name))
			{
				return Convert.ToDecimal(RuntimeHelpers.GetObjectValue(row[name]));
			}
			return decimal.Zero;
		}

		private static DateTime ReadDate(DataRow row, string name)
		{
			if (!row.IsNull(name))
			{
				return Convert.ToDateTime(RuntimeHelpers.GetObjectValue(row[name]));
			}
			return DateTime.MinValue;
		}

		private void SynchronizeDeckCounter()
		{
			DataRow dataRow = RequireTable("Counters").Rows.Find("Decks");
			if (dataRow != null)
			{
				dataRow["Number"] = DecksTable().Rows.Count;
			}
		}
	}
}
