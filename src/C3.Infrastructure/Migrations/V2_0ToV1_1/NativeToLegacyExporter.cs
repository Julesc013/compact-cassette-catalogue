using C3.Catalogue.Native;
using C3.Infrastructure.CatalogueFiles.Xml.V1_1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace C3.Infrastructure.Migrations.V2_0ToV1_1
{
    public sealed class NativeToLegacyExporter
    {
        private readonly LegacyXmlCatalogueStore store;
        private readonly LegacyExportReportWriter reportWriter;

        public NativeToLegacyExporter()
            : this(new LegacyXmlCatalogueStore(), new LegacyExportReportWriter())
        {
        }

        public NativeToLegacyExporter(LegacyXmlCatalogueStore store, LegacyExportReportWriter reportWriter)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.reportWriter = reportWriter ?? throw new ArgumentNullException(nameof(reportWriter));
        }

        public LegacyExportPreview Preview(NativeCatalogue source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var report = new LegacyExportReport();
            var document = LegacyCatalogueSchema.Create(new LegacyCatalogueMetadata
            {
                FileVersion = "1.1.0",
                ProductVersion = "2.0.0",
                ProductStage = "Legacy Export",
                ProductDate = source.Metadata.ModifiedAt.Value,
                CreatedAt = source.Metadata.CreatedAt.Value
            });
            SetInformation(document, "File Created", Canonical(source.Metadata.CreatedAt.Value));
            SetInformation(document, "File Modified", Canonical(source.Metadata.ModifiedAt.Value));
            SetInformation(document, "File Updated", Canonical(source.Metadata.ModifiedAt.Value));
            report.Add(LegacyExportIssueSeverity.Warning, "identity.omitted", "catalogue", "The v1.1 format cannot preserve opaque native entity identifiers.");
            report.Add(LegacyExportIssueSeverity.Warning, "timestamp.utc-semantics", "catalogue", "The v1.1 format does not guarantee explicit UTC timestamp semantics in every historical reader.");
            if (source.Metadata.Provenance.HasValue)
                report.Add(LegacyExportIssueSeverity.Warning, "provenance.omitted", "metadata.provenance", "Native migration provenance is not representable in v1.1.");

            var brandKeys = Keys(source.Brands, item => item.Id.ToString(), item => item.LegacyCode, "brands", report);
            var modelKeys = Keys(source.CassetteModels, item => item.Id.ToString(), item => item.LegacyIdentifier, "cassetteModels", report);
            var deckKeys = Keys(source.DeckUnits, item => item.Id.ToString(), item => item.LegacyKey, "deckUnits", report);
            Keys(source.Tapes, item => item.Id.ToString(), item => item.LegacyShortIdentifier, "tapes", report);
            if (report.HasBlockingIssues) return new LegacyExportPreview(null, report);

            foreach (var brand in source.Brands)
                document.Tables["Brands"].Rows.Add(brand.Name, brand.LegacyCode, brand.AddedAt.Value, brand.Notes);
            foreach (var model in source.CassetteModels)
                document.Tables["Models"].Rows.Add(
                    brandKeys[model.BrandId.ToString()], model.TypeNumber, model.ModelName, model.LegacyCode,
                    model.LegacyIdentifier, model.DisplayName, 0, model.AddedAt.Value, model.Notes);

            var deckModels = Index(source.DeckModels, item => item.Id.ToString());
            var usedDeckModels = new HashSet<string>(StringComparer.Ordinal);
            foreach (var unit in source.DeckUnits)
            {
                var model = deckModels[unit.DeckModelId.ToString()];
                usedDeckModels.Add(model.Id.ToString());
                if (!string.Equals(unit.Name, unit.LegacyKey, StringComparison.Ordinal))
                    report.Add(LegacyExportIssueSeverity.Warning, "deck-unit.name-flattened", "deckUnits[" + unit.Id + "]", "The display name is replaced by the legacy key in v1.1.");
                AddDeck(document.Tables["Decks"], unit, model);
            }
            foreach (var model in source.DeckModels)
                if (!usedDeckModels.Contains(model.Id.ToString()))
                    report.Add(LegacyExportIssueSeverity.Warning, "deck-model.omitted", "deckModels[" + model.Id + "]", "A deck model without a physical unit has no v1.1 representation.");

            foreach (var tape in source.Tapes)
            {
                var row = document.Tables["Tapes"].NewRow();
                row["Model"] = modelKeys[tape.CassetteModelId.ToString()];
                row["Year"] = tape.Year; row["Length"] = tape.LengthMinutes; row["Region"] = tape.Region;
                row["Number"] = tape.Number; row["Identifier"] = tape.LegacyIdentifier;
                row["IdentifierShort"] = tape.LegacyShortIdentifier; row["Condition"] = tape.Condition;
                row["Packaged"] = tape.Packaged; row["Date"] = tape.AddedAt.Value; row["Notes"] = tape.Notes;
                AddSide(row, tape.SideA, "A", deckKeys);
                AddSide(row, tape.SideB, "B", deckKeys);
                document.Tables["Tapes"].Rows.Add(row);
            }
            return new LegacyExportPreview(document, report);
        }

        public LegacyExportResult ExportCopy(NativeCatalogue source, string destinationPath)
        {
            var preview = Preview(source);
            string destination;
            try { destination = Path.GetFullPath(destinationPath ?? string.Empty); }
            catch (Exception exception) { return new LegacyExportResult(false, preview.Report, null, string.Empty, exception.Message); }
            var reportPath = destination + ".export-loss.json";
            preview.Report.DestinationPath = destination;
            if (!preview.IsExportable)
                return new LegacyExportResult(false, preview.Report, null, reportPath, "Native content cannot be represented safely as v1.1.");
            if (File.Exists(destination) || File.Exists(reportPath))
                return new LegacyExportResult(false, preview.Report, null, reportPath, "Legacy export refuses to overwrite its destination or report.");
            var saved = store.SaveNew(destination, preview.Document, new[] { "1.1.0" });
            if (!saved.IsSuccess)
                return new LegacyExportResult(false, preview.Report, null, reportPath, saved.Message);
            preview.Report.DestinationRevision = saved.Revision.Token;
            try
            {
                var payload = reportWriter.WriteJson(preview.Report);
                using (var stream = new FileStream(reportPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(payload, 0, payload.Length);
                    stream.Flush(true);
                }
                return new LegacyExportResult(true, preview.Report, saved.Revision, reportPath, "Legacy copy and loss report were verified.");
            }
            catch (Exception exception)
            {
                try
                {
                    if (File.Exists(reportPath) && new FileInfo(reportPath).Length == 0) File.Delete(reportPath);
                    var loaded = store.Load(destination, preview.Document.Clone(), new[] { "1.1.0" });
                    if (loaded.IsSuccess && saved.Revision.Equals(loaded.Revision)) File.Delete(destination);
                }
                catch
                {
                    // Preserve any uncertain output for manual recovery rather than deleting unverified data.
                }
                return new LegacyExportResult(false, preview.Report, saved.Revision, reportPath, exception.Message);
            }
        }

        private static Dictionary<string, string> Keys<T>(IEnumerable<T> values, Func<T, string> id, Func<T, string> key, string path, LegacyExportReport report)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            var unique = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in values)
            {
                var legacyKey = key(value);
                if (string.IsNullOrWhiteSpace(legacyKey) || !unique.Add(legacyKey))
                    report.Add(LegacyExportIssueSeverity.Blocking, "legacy-key.invalid", path + "[" + id(value) + "]", "A non-empty unique legacy key is required for v1.1 export.");
                else result.Add(id(value), legacyKey);
            }
            return result;
        }

        private static Dictionary<string, T> Index<T>(IEnumerable<T> values, Func<T, string> id)
        {
            var result = new Dictionary<string, T>(StringComparer.Ordinal);
            foreach (var value in values) result.Add(id(value), value);
            return result;
        }

        private static void AddDeck(DataTable table, NativeDeckUnit unit, NativeDeckModel model)
        {
            var c = model.Capabilities;
            table.Rows.Add(model.Manufacturer, model.Model, unit.LegacyKey, model.Year, unit.Condition,
                c.Type1, c.Type2, c.Type3, c.Type4, c.Hx, c.Mpx, c.DolbyB, c.DolbyC, c.DolbyS,
                c.Dbx1, c.Dbx2, c.Stereo, c.ProgramSearch, c.Reverse, c.Calibration, c.Azimuth,
                c.DubbingSlow, c.DubbingFast, c.FrequencyLow, c.FrequencyHigh, c.SignalRatio,
                c.SignalRatioNoiseReduction, c.WowFlutter, c.Distortion, c.Heads, c.Wells,
                c.SpeedSlow, c.SpeedNormal, c.SpeedFast, unit.AddedAt.Value, unit.Notes);
        }

        private static void AddSide(DataRow row, NativeTapeSide side, string suffix, IDictionary<string, string> deckKeys)
        {
            row["Name" + suffix] = side.Name;
            row["Taped" + suffix] = side.Recording.HasValue;
            if (!side.Recording.HasValue) return;
            var recording = side.Recording.Value;
            row["Recorded" + suffix] = recording.RecordedAt.Value;
            row["Deck" + suffix] = recording.DeckUnitId.HasValue ? deckKeys[recording.DeckUnitId.Value.ToString()] : string.Empty;
            row["Input" + suffix] = recording.InputName; row["Peak" + suffix] = recording.PeakLevel;
            row["NR" + suffix] = recording.NoiseReduction; row["HX" + suffix] = recording.Hx;
            row["MPX" + suffix] = recording.Mpx; row["Dubbed" + suffix] = recording.Dubbed;
            row["Speed" + suffix] = recording.Speed; row["Bias" + suffix] = recording.Bias;
            row["BiasCal" + suffix] = recording.BiasCalibration; row["EQ" + suffix] = recording.Equalization;
            row["Level" + suffix] = recording.Level; row["LevelCal" + suffix] = recording.LevelCalibration;
            row["Contents" + suffix] = recording.Contents; row["Artist" + suffix] = recording.Artist;
            row["Title" + suffix] = recording.Title;
        }

        private static void SetInformation(DataSet document, string name, string value)
        {
            document.Tables["Information"].Rows.Find(name)["Value"] = value;
        }

        private static string Canonical(DateTime value)
        {
            return value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'", CultureInfo.InvariantCulture);
        }
    }
}
