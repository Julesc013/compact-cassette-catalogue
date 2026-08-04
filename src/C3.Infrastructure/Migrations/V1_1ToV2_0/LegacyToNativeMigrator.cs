using C3.Catalogue.Native;
using C3.Domain.Identity;
using C3.Domain.Time;
using C3.Domain.Values;
using C3.Infrastructure.CatalogueFiles.Xml.V1_1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace C3.Infrastructure.Migrations.V1_1ToV2_0
{
    public sealed class LegacyToNativeMigrator
    {
        private const string MigrationProfile = "v1.1-to-v2.0/1";
        private readonly LegacyXmlCatalogueStore legacyStore;

        public LegacyToNativeMigrator()
            : this(new LegacyXmlCatalogueStore())
        {
        }

        public LegacyToNativeMigrator(LegacyXmlCatalogueStore legacyStore)
        {
            this.legacyStore = legacyStore ?? throw new ArgumentNullException(nameof(legacyStore));
        }

        public LegacyToNativeMigrationResult DryRun(string sourcePath)
        {
            var fullPath = string.IsNullOrWhiteSpace(sourcePath)
                ? string.Empty
                : Path.GetFullPath(sourcePath);
            var initialReport = new MigrationReport(fullPath, string.Empty);
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                Block(initialReport, "source.path-required", "source", "A legacy source path is required.");
                return new LegacyToNativeMigrationResult(null, initialReport);
            }

            var schema = LegacyCatalogueSchema.Create(new LegacyCatalogueMetadata
            {
                FileVersion = "1.1.0",
                ProductVersion = "2.0.0",
                ProductStage = "Migration",
                ProductDate = new DateTime(2026, 8, 4),
                CreatedAt = new DateTime(2026, 8, 4)
            });
            var loaded = legacyStore.Load(fullPath, schema, new[] { "1.1.0" });
            if (!loaded.IsSuccess)
            {
                Block(
                    initialReport,
                    "source." + loaded.Failure.ToString().ToLowerInvariant(),
                    "source",
                    loaded.Message);
                return new LegacyToNativeMigrationResult(null, initialReport);
            }

            var report = new MigrationReport(fullPath, loaded.Revision.Token);
            try
            {
                var lexemes = LegacyTimestampLexemes.Load(fullPath);
                var document = Map(loaded.Document, lexemes, report);
                return new LegacyToNativeMigrationResult(
                    report.HasBlockingIssues ? null : document,
                    report);
            }
            catch (Exception exception)
            {
                Block(report, "migration.unexpected", "source", exception.Message);
                return new LegacyToNativeMigrationResult(null, report);
            }
        }

        private static NativeCatalogue Map(
            DataSet source,
            LegacyTimestampLexemes lexemes,
            MigrationReport report)
        {
            var identityNamespace = "c3:migration:" + MigrationProfile + ":" + report.SourceRevision;
            var brands = new List<NativeBrand>();
            var models = new List<NativeCassetteModel>();
            var deckModels = new List<NativeDeckModel>();
            var deckUnits = new List<NativeDeckUnit>();
            var tapes = new List<NativeTape>();
            var brandIds = new Dictionary<string, EntityId<NativeBrand>>(StringComparer.Ordinal);
            var modelIds = new Dictionary<string, EntityId<NativeCassetteModel>>(StringComparer.Ordinal);
            var deckUnitIds = new Dictionary<string, EntityId<NativeDeckUnit>>(StringComparer.Ordinal);

            foreach (DataRow row in source.Tables["Brands"].Rows)
            {
                var key = Text(row, "Code");
                if (!RequiredKey(report, "brands", key)) continue;
                var id = DeterministicEntityId.FromCanonicalKey<NativeBrand>(identityNamespace, "brand:" + key);
                brandIds.Add(key, id);
                brands.Add(new NativeBrand(
                    id,
                    Text(row, "Brand"),
                    key,
                    NormalizeTimestamp(
                        lexemes.Get("Brands", key, "Date"),
                        Date(row, "Date"),
                        "brands[" + key + "].addedUtc",
                        report),
                    Text(row, "Notes")));
                Map(report, "brand", key, id.ToString());
            }

            foreach (DataRow row in source.Tables["Models"].Rows)
            {
                var key = Text(row, "Identifier");
                if (!RequiredKey(report, "cassetteModels", key)) continue;
                var brandKey = Text(row, "Brand");
                EntityId<NativeBrand> brandId;
                if (!brandIds.TryGetValue(brandKey, out brandId))
                {
                    Block(report, "reference.brand-unresolved", "cassetteModels[" + key + "].brand", "Brand " + brandKey + " does not resolve.");
                    continue;
                }
                var id = DeterministicEntityId.FromCanonicalKey<NativeCassetteModel>(
                    identityNamespace, "cassette-model:" + key);
                modelIds.Add(key, id);
                models.Add(new NativeCassetteModel(
                    id, brandId, Integer(row, "Type"), Text(row, "Model"),
                    Text(row, "Code"), key, Text(row, "Name"),
                    NormalizeTimestamp(
                        lexemes.Get("Models", key, "Date"),
                        Date(row, "Date"),
                        "cassetteModels[" + key + "].addedUtc",
                        report),
                    Text(row, "Notes")));
                Map(report, "cassetteModel", key, id.ToString());
            }

            foreach (DataRow row in source.Tables["Decks"].Rows)
            {
                var key = Text(row, "Name");
                if (!RequiredKey(report, "deckUnits", key)) continue;
                var modelId = DeterministicEntityId.FromCanonicalKey<NativeDeckModel>(
                    identityNamespace, "deck-model:" + key);
                var unitId = DeterministicEntityId.FromCanonicalKey<NativeDeckUnit>(
                    identityNamespace, "deck-unit:" + key);
                deckUnitIds.Add(key, unitId);
                deckModels.Add(new NativeDeckModel(
                    modelId,
                    Text(row, "Manufacturer"),
                    Text(row, "Model"),
                    Integer(row, "Year"),
                    Capabilities(row)));
                deckUnits.Add(new NativeDeckUnit(
                    unitId,
                    modelId,
                    key,
                    key,
                    Integer(row, "Condition"),
                    NormalizeTimestamp(
                        lexemes.Get("Decks", key, "Date"),
                        Date(row, "Date"),
                        "deckUnits[" + key + "].addedUtc",
                        report),
                    Text(row, "Notes")));
                Map(report, "deckModel", key, modelId.ToString());
                Map(report, "deckUnit", key, unitId.ToString());
            }

            foreach (DataRow row in source.Tables["Tapes"].Rows)
            {
                var key = Text(row, "IdentifierShort");
                if (!RequiredKey(report, "tapes", key)) continue;
                var modelKey = Text(row, "Model");
                EntityId<NativeCassetteModel> modelId;
                if (!modelIds.TryGetValue(modelKey, out modelId))
                {
                    Block(report, "reference.model-unresolved", "tapes[" + key + "].model", "Cassette model " + modelKey + " does not resolve.");
                    continue;
                }
                var tapeId = DeterministicEntityId.FromCanonicalKey<NativeTape>(
                    identityNamespace, "tape:" + key);
                var sideA = Side(row, "A", key, tapeId, deckUnitIds, lexemes, identityNamespace, report);
                var sideB = Side(row, "B", key, tapeId, deckUnitIds, lexemes, identityNamespace, report);
                tapes.Add(new NativeTape(
                    tapeId,
                    modelId,
                    Integer(row, "Year"),
                    Decimal(row, "Length"),
                    Text(row, "Region"),
                    Integer(row, "Number"),
                    Text(row, "Identifier"),
                    key,
                    Integer(row, "Condition"),
                    Boolean(row, "Packaged"),
                    NormalizeTimestamp(
                        lexemes.Get("Tapes", key, "Date"),
                        Date(row, "Date"),
                        "tapes[" + key + "].addedUtc",
                        report),
                    Text(row, "Notes"),
                    sideA,
                    sideB));
                Map(report, "tape", key, tapeId.ToString());
            }

            report.Counts.Brands = brands.Count;
            report.Counts.CassetteModels = models.Count;
            report.Counts.DeckModels = deckModels.Count;
            report.Counts.DeckUnits = deckUnits.Count;
            report.Counts.Tapes = tapes.Count;
            if (report.HasBlockingIssues)
            {
                return null;
            }

            var created = NormalizeTimestamp(
                lexemes.Get("Information", "File Created", "Value"),
                DateTime.MinValue,
                "metadata.createdUtc",
                report);
            var modified = NormalizeTimestamp(
                lexemes.Get("Information", "File Modified", "Value"),
                created.Value,
                "metadata.modifiedUtc",
                report);
            if (modified.CompareTo(created) < 0)
            {
                report.AddNormalization(new MigrationNormalization(
                    "modified-before-created-clamped",
                    "metadata.modifiedUtc",
                    Canonical(modified.Value),
                    Canonical(created.Value)));
                modified = created;
            }
            var catalogueId = DeterministicEntityId.FromCanonicalKey<NativeCatalogue>(
                identityNamespace, "catalogue");
            Map(report, "catalogue", report.SourceRevision, catalogueId.ToString());
            return new NativeCatalogue(
                catalogueId,
                new NativeCatalogueMetadata(
                    "C3 native-v2 migration",
                    created,
                    modified,
                    Optional<NativeCatalogueProvenance>.Some(
                        new NativeCatalogueProvenance("1.1.0", report.SourceRevision, MigrationProfile))),
                brands,
                models,
                deckModels,
                deckUnits,
                tapes);
        }

        private static NativeTapeSide Side(
            DataRow row,
            string suffix,
            string tapeKey,
            EntityId<NativeTape> tapeId,
            IDictionary<string, EntityId<NativeDeckUnit>> deckUnitIds,
            LegacyTimestampLexemes lexemes,
            string identityNamespace,
            MigrationReport report)
        {
            var position = suffix == "A" ? NativeTapeSidePosition.A : NativeTapeSidePosition.B;
            var recorded = Boolean(row, "Taped" + suffix);
            if (!recorded)
            {
                if (HasUnrepresentedSideData(row, suffix))
                {
                    Block(
                        report,
                        "side.unrecorded-data",
                        "tapes[" + tapeKey + "].side" + suffix,
                        "The legacy side is marked unrecorded but contains recording fields.");
                }
                return new NativeTapeSide(
                    position,
                    Text(row, "Name" + suffix),
                    Optional<NativeRecording>.None());
            }

            var deckKey = Text(row, "Deck" + suffix);
            var deckId = Optional<EntityId<NativeDeckUnit>>.None();
            if (!string.IsNullOrEmpty(deckKey))
            {
                EntityId<NativeDeckUnit> resolved;
                if (!deckUnitIds.TryGetValue(deckKey, out resolved))
                {
                    Block(
                        report,
                        "reference.deck-unresolved",
                        "tapes[" + tapeKey + "].side" + suffix + ".deck",
                        "Deck " + deckKey + " does not resolve.");
                }
                else
                {
                    deckId = Optional<EntityId<NativeDeckUnit>>.Some(resolved);
                }
            }
            var recordingId = DeterministicEntityId.FromCanonicalKey<NativeRecording>(
                identityNamespace, "recording:" + tapeKey + ":" + suffix);
            report.Counts.Recordings++;
            Map(report, "recording", tapeKey + ":" + suffix, recordingId.ToString());
            return new NativeTapeSide(
                position,
                Text(row, "Name" + suffix),
                Optional<NativeRecording>.Some(new NativeRecording(
                    recordingId,
                    deckId,
                    NormalizeTimestamp(
                        lexemes.Get("Tapes", tapeKey, "Recorded" + suffix),
                        Date(row, "Recorded" + suffix),
                        "tapes[" + tapeKey + "].side" + suffix + ".recordedUtc",
                        report),
                    Text(row, "Input" + suffix),
                    Integer(row, "Peak" + suffix),
                    Text(row, "NR" + suffix),
                    Boolean(row, "HX" + suffix),
                    Boolean(row, "MPX" + suffix),
                    Boolean(row, "Dubbed" + suffix),
                    Text(row, "Speed" + suffix),
                    Integer(row, "Bias" + suffix),
                    Integer(row, "BiasCal" + suffix),
                    Text(row, "EQ" + suffix),
                    Decimal(row, "Level" + suffix),
                    Decimal(row, "LevelCal" + suffix),
                    Text(row, "Contents" + suffix),
                    Text(row, "Artist" + suffix),
                    Text(row, "Title" + suffix))));
        }

        private static NativeDeckCapabilities Capabilities(DataRow row)
        {
            return new NativeDeckCapabilities(
                Boolean(row, "Type1"), Boolean(row, "Type2"), Boolean(row, "Type3"), Boolean(row, "Type4"),
                Boolean(row, "HX"), Boolean(row, "MPX"), Boolean(row, "DolbyB"), Boolean(row, "DolbyC"),
                Boolean(row, "DolbyS"), Boolean(row, "DBX1"), Boolean(row, "DBX2"), Boolean(row, "Stereo"),
                Boolean(row, "ProgramSearch"), Boolean(row, "Reverse"), Boolean(row, "Calibration"), Boolean(row, "Azimuth"),
                Boolean(row, "DubbingSlow"), Boolean(row, "DubbingFast"), Integer(row, "FrequencyLow"), Integer(row, "FrequencyHigh"),
                Integer(row, "SignalRatio"), Text(row, "SignalRatioNR"), Decimal(row, "WowFlutter"), Decimal(row, "Distortion"),
                Integer(row, "Heads"), Integer(row, "Wells"), Boolean(row, "SpeedSlow"), Boolean(row, "SpeedNorm"), Boolean(row, "SpeedFast"));
        }

        private static bool HasUnrepresentedSideData(DataRow row, string suffix)
        {
            return Date(row, "Recorded" + suffix) != DateTime.MinValue ||
                !string.IsNullOrEmpty(Text(row, "Deck" + suffix)) ||
                !string.IsNullOrEmpty(Text(row, "Input" + suffix)) ||
                Integer(row, "Peak" + suffix) != 0 ||
                !string.IsNullOrEmpty(Text(row, "NR" + suffix)) ||
                Boolean(row, "HX" + suffix) || Boolean(row, "MPX" + suffix) ||
                Boolean(row, "Dubbed" + suffix) ||
                !string.IsNullOrEmpty(Text(row, "Speed" + suffix)) ||
                Integer(row, "Bias" + suffix) != 0 || Integer(row, "BiasCal" + suffix) != 0 ||
                !string.IsNullOrEmpty(Text(row, "EQ" + suffix)) ||
                Decimal(row, "Level" + suffix) != decimal.Zero ||
                Decimal(row, "LevelCal" + suffix) != decimal.Zero ||
                !string.IsNullOrEmpty(Text(row, "Contents" + suffix)) ||
                !string.IsNullOrEmpty(Text(row, "Artist" + suffix)) ||
                !string.IsNullOrEmpty(Text(row, "Title" + suffix));
        }

        private static UtcTimestamp NormalizeTimestamp(
            string lexeme,
            DateTime fallback,
            string path,
            MigrationReport report)
        {
            DateTime normalized;
            string code;
            if (!string.IsNullOrWhiteSpace(lexeme))
            {
                DateTimeOffset offset;
                if (HasExplicitOffset(lexeme) && DateTimeOffset.TryParse(
                    lexeme,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out offset))
                {
                    normalized = offset.UtcDateTime;
                    code = "legacy-offset-normalized-utc";
                }
                else
                {
                    DateTime wallClock;
                    if (!DateTime.TryParse(
                        lexeme,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AllowWhiteSpaces,
                        out wallClock))
                    {
                        Block(report, "timestamp.invalid", path, "Legacy timestamp cannot be interpreted deterministically: " + lexeme);
                        normalized = new DateTime(fallback.Ticks, DateTimeKind.Utc);
                        code = "legacy-timestamp-fallback";
                    }
                    else
                    {
                        normalized = DateTime.SpecifyKind(wallClock, DateTimeKind.Utc);
                        code = "legacy-local-wall-clock-assumed-utc";
                    }
                }
            }
            else
            {
                normalized = DateTime.SpecifyKind(fallback, DateTimeKind.Utc);
                code = "legacy-timestamp-fallback";
            }
            report.AddNormalization(new MigrationNormalization(
                code,
                path,
                lexeme,
                Canonical(normalized)));
            return new UtcTimestamp(normalized);
        }

        private static bool HasExplicitOffset(string value)
        {
            var text = value.Trim();
            if (text.EndsWith("Z", StringComparison.OrdinalIgnoreCase)) return true;
            if (text.Length < 6) return false;
            var marker = text[text.Length - 6];
            return (marker == '+' || marker == '-') && text[text.Length - 3] == ':';
        }

        private static string Canonical(DateTime value)
        {
            return value.ToUniversalTime().ToString(
                "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'",
                CultureInfo.InvariantCulture);
        }

        private static bool RequiredKey(MigrationReport report, string collection, string key)
        {
            if (!string.IsNullOrWhiteSpace(key)) return true;
            Block(report, "legacy.key-missing", collection, "A required legacy key is empty.");
            return false;
        }

        private static void Map(MigrationReport report, string kind, string key, string id)
        {
            report.AddMapping(new MigrationMapping(kind, key, id));
        }

        private static void Block(MigrationReport report, string code, string path, string message)
        {
            report.AddIssue(new MigrationIssue(MigrationIssueSeverity.Blocking, code, path, message));
        }

        private static string Text(DataRow row, string name)
        {
            var value = row[name];
            return value == null || value == DBNull.Value
                ? string.Empty
                : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private static int Integer(DataRow row, string name)
        {
            return Convert.ToInt32(row[name], CultureInfo.InvariantCulture);
        }

        private static decimal Decimal(DataRow row, string name)
        {
            return Convert.ToDecimal(row[name], CultureInfo.InvariantCulture);
        }

        private static bool Boolean(DataRow row, string name)
        {
            return Convert.ToBoolean(row[name], CultureInfo.InvariantCulture);
        }

        private static DateTime Date(DataRow row, string name)
        {
            var value = row[name];
            return value == null || value == DBNull.Value
                ? DateTime.MinValue
                : Convert.ToDateTime(value, CultureInfo.InvariantCulture);
        }
    }
}
