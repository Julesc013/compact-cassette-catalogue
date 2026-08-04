using C3.Catalogue.Tapes;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyTapeRepository : ITapeRepository
	{
		private readonly Func<DataSet> _documentProvider;

		public LegacyTapeRepository(Func<DataSet> documentProvider)
		{
			if (documentProvider == null)
			{
				throw new ArgumentNullException("documentProvider");
			}
			_documentProvider = documentProvider;
		}

		public IList<Tape> GetAll()
		{
			List<Tape> list = new List<Tape>();
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = TapesTable().Rows.GetEnumerator();
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

		IList<Tape> ITapeRepository.GetAll()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetAll
			return this.GetAll();
		}

		public Tape FindByShortIdentifier(string identifier)
		{
			DataRow dataRow = FindRow(identifier);
			if (dataRow != null)
			{
				return Map(dataRow);
			}
			return null;
		}

		Tape ITapeRepository.FindByShortIdentifier(string identifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in FindByShortIdentifier
			return this.FindByShortIdentifier(identifier);
		}

		public bool ModelExists(string identifier)
		{
			return FindModelRow(identifier) != null;
		}

		bool ITapeRepository.ModelExists(string identifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ModelExists
			return this.ModelExists(identifier);
		}

		public int NextNumberForModel(string identifier)
		{
			int num = -1;
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = TapesTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(ReadString(dataRow, "Model"), identifier, StringComparison.OrdinalIgnoreCase))
					{
						num = Math.Max(num, ReadInteger(dataRow, "Number"));
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
			return checked(num + 1);
		}

		int ITapeRepository.NextNumberForModel(string identifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in NextNumberForModel
			return this.NextNumberForModel(identifier);
		}

		public bool IdentifierExists(string identifier, string shortIdentifier)
		{
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = TapesTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && (string.Equals(ReadString(dataRow, "Identifier"), identifier, StringComparison.OrdinalIgnoreCase) || string.Equals(ReadString(dataRow, "IdentifierShort"), shortIdentifier, StringComparison.OrdinalIgnoreCase)))
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

		bool ITapeRepository.IdentifierExists(string identifier, string shortIdentifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IdentifierExists
			return this.IdentifierExists(identifier, shortIdentifier);
		}

		public void AddRange(IList<Tape> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			List<DataRow> list = new List<DataRow>();
			List<string> list2 = new List<string>();
			checked
			{
				try
				{
					foreach (Tape value in values)
					{
						DataRow dataRow = TapesTable().NewRow();
						Write(dataRow, value, true);
						TapesTable().Rows.Add(dataRow);
						list.Add(dataRow);
						if (!list2.Contains(value.ModelIdentifier))
						{
							list2.Add(value.ModelIdentifier);
						}
					}
					SynchronizeCounts(list2);
				}
				catch (Exception projectError)
				{
					ProjectData.SetProjectError(projectError);
					for (int i = list.Count - 1; i >= 0; i += -1)
					{
						if (list[i].Table != null)
						{
							TapesTable().Rows.Remove(list[i]);
						}
					}
					SynchronizeCounts(list2);
					throw;
				}
			}
		}

		void ITapeRepository.AddRange(IList<Tape> values)
		{
			//ILSpy generated this explicit interface implementation from .override directive in AddRange
			this.AddRange(values);
		}

		public void Update(Tape value)
		{
			DataRow dataRow = FindRow(value.ShortIdentifier);
			if (dataRow == null)
			{
				throw new InvalidOperationException("The selected tape no longer exists.");
			}
			Write(dataRow, value, false);
		}

		void ITapeRepository.Update(Tape value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Update
			this.Update(value);
		}

		public void Delete(string shortIdentifier)
		{
			DataRow dataRow = FindRow(shortIdentifier);
			if (dataRow == null)
			{
				throw new InvalidOperationException("The selected tape no longer exists.");
			}
			string item = ReadString(dataRow, "Model");
			TapesTable().Rows.Remove(dataRow);
			SynchronizeCounts(new List<string>
			{
				item
			});
		}

		void ITapeRepository.Delete(string shortIdentifier)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Delete
			this.Delete(shortIdentifier);
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

		private DataTable TapesTable()
		{
			return RequireTable("Tapes");
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
				enumerator = TapesTable().Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(ReadString(dataRow, "IdentifierShort"), identifier, StringComparison.OrdinalIgnoreCase))
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

		private DataRow FindModelRow(string identifier)
		{
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = RequireTable("Models").Rows.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataRow dataRow = (DataRow)enumerator.Current;
					if (dataRow.RowState != DataRowState.Deleted && string.Equals(ReadString(dataRow, "Identifier"), identifier, StringComparison.OrdinalIgnoreCase))
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

		private static Tape Map(DataRow row)
		{
			return new Tape(ReadString(row, "Model"), ReadInteger(row, "Year"), ReadDecimal(row, "Length"), ReadString(row, "Region"), ReadInteger(row, "Number"), ReadString(row, "Identifier"), ReadString(row, "IdentifierShort"), ReadInteger(row, "Condition"), ReadBoolean(row, "Packaged"), MapSide(row, "A", ReadBoolean(row, "TapedA")), MapSide(row, "B", ReadBoolean(row, "TapedB")), ReadDate(row, "Date"), ReadString(row, "Notes"));
		}

		private static TapeSide MapSide(DataRow row, string suffix, bool isRecorded)
		{
			return new TapeSide(isRecorded, ReadString(row, "Name" + suffix), ReadDate(row, "Recorded" + suffix), ReadString(row, "Deck" + suffix), ReadString(row, "Input" + suffix), ReadInteger(row, "Peak" + suffix), ReadString(row, "NR" + suffix), ReadBoolean(row, "HX" + suffix), ReadBoolean(row, "MPX" + suffix), ReadBoolean(row, "Dubbed" + suffix), ReadString(row, "Speed" + suffix), ReadInteger(row, "Bias" + suffix), ReadInteger(row, "BiasCal" + suffix), ReadString(row, "EQ" + suffix), ReadDecimal(row, "Level" + suffix), ReadDecimal(row, "LevelCal" + suffix), ReadString(row, "Contents" + suffix), ReadString(row, "Artist" + suffix), ReadString(row, "Title" + suffix));
		}

		private static void Write(DataRow row, Tape value, bool includeIdentity)
		{
			if (includeIdentity)
			{
				row["Model"] = value.ModelIdentifier;
				row["Number"] = value.Number;
				row["IdentifierShort"] = value.ShortIdentifier;
				row["Date"] = value.AddedAt;
			}
			row["Identifier"] = value.Identifier;
			row["Year"] = value.Year;
			row["Length"] = value.LengthMinutes;
			row["Region"] = value.Region;
			row["Condition"] = value.Condition;
			row["Packaged"] = value.Packaged;
			row["TapedA"] = value.SideA.IsRecorded;
			row["TapedB"] = value.SideB.IsRecorded;
			WriteSide(row, "A", value.SideA);
			WriteSide(row, "B", value.SideB);
			row["Notes"] = value.Notes;
		}

		private static void WriteSide(DataRow row, string suffix, TapeSide value)
		{
			row["Name" + suffix] = value.Name;
			row["Recorded" + suffix] = value.RecordedAt;
			row["Deck" + suffix] = value.DeckName;
			row["Input" + suffix] = value.InputName;
			row["Peak" + suffix] = value.PeakLevel;
			row["NR" + suffix] = value.NoiseReduction;
			row["HX" + suffix] = value.Hx;
			row["MPX" + suffix] = value.Mpx;
			row["Dubbed" + suffix] = value.Dubbed;
			row["Speed" + suffix] = value.Speed;
			row["Bias" + suffix] = value.Bias;
			row["BiasCal" + suffix] = value.BiasCalibration;
			row["EQ" + suffix] = value.Equalization;
			row["Level" + suffix] = value.Level;
			row["LevelCal" + suffix] = value.LevelCalibration;
			row["Contents" + suffix] = value.Contents;
			row["Artist" + suffix] = value.Artist;
			row["Title" + suffix] = value.Title;
		}

		private void SynchronizeCounts(IList<string> modelIdentifiers)
		{
			DataRow dataRow = RequireTable("Counters").Rows.Find("Tapes");
			if (dataRow != null)
			{
				dataRow["Number"] = TapesTable().Rows.Count;
			}
			foreach (string modelIdentifier in modelIdentifiers)
			{
				DataRow dataRow2 = FindModelRow(modelIdentifier);
				if (dataRow2 != null)
				{
					int num = 0;
					IEnumerator enumerator2 = default(IEnumerator);
					try
					{
						enumerator2 = TapesTable().Rows.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							DataRow dataRow3 = (DataRow)enumerator2.Current;
							if (dataRow3.RowState != DataRowState.Deleted && string.Equals(ReadString(dataRow3, "Model"), modelIdentifier, StringComparison.OrdinalIgnoreCase))
							{
								num = checked(num + 1);
							}
						}
					}
					finally
					{
						if (enumerator2 is IDisposable)
						{
							(enumerator2 as IDisposable).Dispose();
						}
					}
					dataRow2["Number"] = num;
				}
			}
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
	}
}
