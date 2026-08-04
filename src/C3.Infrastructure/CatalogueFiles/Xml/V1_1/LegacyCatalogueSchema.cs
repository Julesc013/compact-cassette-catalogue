using System;
using System.Data;

namespace C3.Infrastructure.CatalogueFiles.Xml.V1_1
{
	public sealed class LegacyCatalogueSchema
	{
		private LegacyCatalogueSchema()
		{
		}

		public static DataSet Create(LegacyCatalogueMetadata metadata)
		{
			if (metadata == null)
			{
				throw new ArgumentNullException("metadata");
			}
			return new DataSet("Catalogue")
			{
				Tables = 
				{
					CreateInformation(metadata),
					CreateCounters(),
					CreateDecks(),
					CreateBrands(),
					CreateModels(),
					CreateTapes()
				}
			};
		}

		private static DataTable CreateInformation(LegacyCatalogueMetadata metadata)
		{
			DataTable dataTable = new DataTable("Information");
			dataTable.Columns.Add("Information", typeof(string));
			dataTable.Columns.Add("Value", typeof(string));
			dataTable.Rows.Add("File Version", metadata.FileVersion);
			dataTable.Rows.Add("Program Version", metadata.ProductVersion);
			dataTable.Rows.Add("Program Stage", metadata.ProductStage);
			DataRowCollection rows = dataTable.Rows;
			object[] obj = new object[2]
			{
				"Program Date",
				null
			};
			DateTime dateTime = metadata.ProductDate;
			obj[1] = dateTime.ToString();
			rows.Add(obj);
			DataRowCollection rows2 = dataTable.Rows;
			object[] obj2 = new object[2]
			{
				"File Created",
				null
			};
			dateTime = metadata.CreatedAt;
			obj2[1] = dateTime.ToString();
			rows2.Add(obj2);
			DataRowCollection rows3 = dataTable.Rows;
			object[] obj3 = new object[2]
			{
				"File Modified",
				null
			};
			dateTime = metadata.CreatedAt;
			obj3[1] = dateTime.ToString();
			rows3.Add(obj3);
			DataRowCollection rows4 = dataTable.Rows;
			object[] obj4 = new object[2]
			{
				"File Updated",
				null
			};
			dateTime = metadata.CreatedAt;
			obj4[1] = dateTime.ToString();
			rows4.Add(obj4);
			dataTable.PrimaryKey = new DataColumn[1]
			{
				dataTable.Columns["Information"]
			};
			return dataTable;
		}

		private static DataTable CreateCounters()
		{
			DataTable dataTable = new DataTable("Counters");
			dataTable.Columns.Add("Counter", typeof(string));
			dataTable.Columns.Add("Number", typeof(int));
			dataTable.Rows.Add("Decks", 0);
			dataTable.Rows.Add("Brands", 0);
			dataTable.Rows.Add("Models", 0);
			dataTable.Rows.Add("Tapes", 0);
			dataTable.PrimaryKey = new DataColumn[1]
			{
				dataTable.Columns["Counter"]
			};
			return dataTable;
		}

		private static DataTable CreateDecks()
		{
			DataTable dataTable = new DataTable("Decks");
			AddColumn(dataTable, "Manufacturer", typeof(string));
			AddColumn(dataTable, "Model", typeof(string));
			AddColumn(dataTable, "Name", typeof(string));
			AddColumn(dataTable, "Year", typeof(int));
			AddColumn(dataTable, "Condition", typeof(int));
			AddColumn(dataTable, "Type1", typeof(bool));
			AddColumn(dataTable, "Type2", typeof(bool));
			AddColumn(dataTable, "Type3", typeof(bool));
			AddColumn(dataTable, "Type4", typeof(bool));
			AddColumn(dataTable, "HX", typeof(bool));
			AddColumn(dataTable, "MPX", typeof(bool));
			AddColumn(dataTable, "DolbyB", typeof(bool));
			AddColumn(dataTable, "DolbyC", typeof(bool));
			AddColumn(dataTable, "DolbyS", typeof(bool));
			AddColumn(dataTable, "DBX1", typeof(bool));
			AddColumn(dataTable, "DBX2", typeof(bool));
			AddColumn(dataTable, "Stereo", typeof(bool));
			AddColumn(dataTable, "ProgramSearch", typeof(bool));
			AddColumn(dataTable, "Reverse", typeof(bool));
			AddColumn(dataTable, "Calibration", typeof(bool));
			AddColumn(dataTable, "Azimuth", typeof(bool));
			AddColumn(dataTable, "DubbingSlow", typeof(bool));
			AddColumn(dataTable, "DubbingFast", typeof(bool));
			AddColumn(dataTable, "FrequencyLow", typeof(int));
			AddColumn(dataTable, "FrequencyHigh", typeof(int));
			AddColumn(dataTable, "SignalRatio", typeof(int));
			AddColumn(dataTable, "SignalRatioNR", typeof(string));
			AddColumn(dataTable, "WowFlutter", typeof(decimal));
			AddColumn(dataTable, "Distortion", typeof(decimal));
			AddColumn(dataTable, "Heads", typeof(int));
			AddColumn(dataTable, "Wells", typeof(int));
			AddColumn(dataTable, "SpeedSlow", typeof(bool));
			AddColumn(dataTable, "SpeedNorm", typeof(bool));
			AddColumn(dataTable, "SpeedFast", typeof(bool));
			AddColumn(dataTable, "Date", typeof(DateTime));
			AddColumn(dataTable, "Notes", typeof(string));
			dataTable.PrimaryKey = new DataColumn[1]
			{
				dataTable.Columns["Name"]
			};
			return dataTable;
		}

		private static DataTable CreateBrands()
		{
			DataTable dataTable = new DataTable("Brands");
			AddColumn(dataTable, "Brand", typeof(string));
			AddColumn(dataTable, "Code", typeof(string));
			AddColumn(dataTable, "Date", typeof(DateTime));
			AddColumn(dataTable, "Notes", typeof(string));
			dataTable.PrimaryKey = new DataColumn[1]
			{
				dataTable.Columns["Code"]
			};
			return dataTable;
		}

		private static DataTable CreateModels()
		{
			DataTable dataTable = new DataTable("Models");
			AddColumn(dataTable, "Brand", typeof(string));
			AddColumn(dataTable, "Type", typeof(int));
			AddColumn(dataTable, "Model", typeof(string));
			AddColumn(dataTable, "Code", typeof(string));
			AddColumn(dataTable, "Identifier", typeof(string));
			AddColumn(dataTable, "Name", typeof(string));
			AddColumn(dataTable, "Number", typeof(int));
			AddColumn(dataTable, "Date", typeof(DateTime));
			AddColumn(dataTable, "Notes", typeof(string));
			dataTable.PrimaryKey = new DataColumn[1]
			{
				dataTable.Columns["Identifier"]
			};
			return dataTable;
		}

		private static DataTable CreateTapes()
		{
			DataTable dataTable = new DataTable("Tapes");
			AddColumn(dataTable, "Model", typeof(string));
			AddColumn(dataTable, "Year", typeof(int));
			AddColumn(dataTable, "Length", typeof(decimal));
			AddColumn(dataTable, "Region", typeof(string));
			AddColumn(dataTable, "Number", typeof(int));
			AddColumn(dataTable, "Identifier", typeof(string));
			AddColumn(dataTable, "IdentifierShort", typeof(string));
			AddColumn(dataTable, "Condition", typeof(int));
			AddColumn(dataTable, "Packaged", typeof(bool));
			AddColumn(dataTable, "TapedA", typeof(bool));
			AddColumn(dataTable, "TapedB", typeof(bool));
			AddTapeSideColumns(dataTable, "A");
			AddTapeSideColumns(dataTable, "B");
			AddColumn(dataTable, "Date", typeof(DateTime));
			AddColumn(dataTable, "Notes", typeof(string));
			dataTable.PrimaryKey = new DataColumn[1]
			{
				dataTable.Columns["IdentifierShort"]
			};
			return dataTable;
		}

		private static void AddTapeSideColumns(DataTable table, string side)
		{
			AddColumn(table, "Name" + side, typeof(string));
			AddColumn(table, "Recorded" + side, typeof(DateTime));
			AddColumn(table, "Deck" + side, typeof(string));
			AddColumn(table, "Input" + side, typeof(string));
			AddColumn(table, "Peak" + side, typeof(int));
			AddColumn(table, "NR" + side, typeof(string));
			AddColumn(table, "HX" + side, typeof(bool));
			AddColumn(table, "MPX" + side, typeof(bool));
			AddColumn(table, "Dubbed" + side, typeof(bool));
			AddColumn(table, "Speed" + side, typeof(string));
			AddColumn(table, "Bias" + side, typeof(int));
			AddColumn(table, "BiasCal" + side, typeof(int));
			AddColumn(table, "EQ" + side, typeof(string));
			AddColumn(table, "Level" + side, typeof(decimal));
			AddColumn(table, "LevelCal" + side, typeof(decimal));
			AddColumn(table, "Contents" + side, typeof(string));
			AddColumn(table, "Artist" + side, typeof(string));
			AddColumn(table, "Title" + side, typeof(string));
		}

		private static void AddColumn(DataTable table, string name, Type dataType)
		{
			table.Columns.Add(new DataColumn(name, dataType));
		}
	}
}
