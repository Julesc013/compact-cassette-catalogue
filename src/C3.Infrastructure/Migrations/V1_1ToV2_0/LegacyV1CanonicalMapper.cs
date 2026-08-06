using C3.Catalogue.Canonical;
using C3.Catalogue.Native;
using C3.Domain.Identity;
using C3.Domain.Time;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace C3.Infrastructure.Migrations.V1_1ToV2_0
{
    /// <summary>
    /// Owns the one interpretation of an already validated legacy-v1.1
    /// <see cref="DataSet"/> as the format-neutral catalogue graph.
    /// </summary>
    internal sealed class LegacyV1CanonicalMapper
    {
        public CatalogueState Map(
            DataSet source,
            LegacyTimestampLexemes lexemes,
            MigrationReport report)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (lexemes == null) throw new ArgumentNullException(nameof(lexemes));
            if (report == null) throw new ArgumentNullException(nameof(report));

            var identityNamespace = "c3:migration:" + report.Profile + ":" +
                report.SourceRevision;
            var brands = new List<CatalogueBrandState>();
            var models = new List<CatalogueCassetteModelState>();
            var deckModels = new List<CatalogueDeckModelState>();
            var deckUnits = new List<CatalogueDeckUnitState>();
            var tapes = new List<CatalogueTapeState>();
            var recordings = new List<CatalogueRecordingState>();
            var brandIds = new Dictionary<string, CatalogueEntityKey>(
                StringComparer.Ordinal);
            var modelIds = new Dictionary<string, CatalogueEntityKey>(
                StringComparer.Ordinal);
            var deckUnitIds = new Dictionary<string, CatalogueEntityKey>(
                StringComparer.Ordinal);

            foreach (DataRow row in source.Tables["Brands"].Rows)
            {
                var key = Text(row, "Code");
                if (!RequiredKey(report, "brands", key)) continue;
                var id = Key<NativeBrand>(
                    CatalogueEntityKind.Brand,
                    identityNamespace,
                    "brand:" + key);
                brandIds.Add(key, id);
                brands.Add(new CatalogueBrandState(
                    id,
                    Text(row, "Brand"),
                    key,
                    NormalizeTimestamp(
                        lexemes.Get("Brands", key, "Date"),
                        Date(row, "Date"),
                        "brands[" + key + "].addedUtc",
                        report),
                    Text(row, "Notes")));
                Mapping(report, "brand", key, id);
            }

            foreach (DataRow row in source.Tables["Models"].Rows)
            {
                var key = Text(row, "Identifier");
                if (!RequiredKey(report, "cassetteModels", key)) continue;
                CatalogueEntityKey brandId;
                var brandKey = Text(row, "Brand");
                if (!brandIds.TryGetValue(brandKey, out brandId))
                {
                    Block(
                        report,
                        "reference.brand-unresolved",
                        "cassetteModels[" + key + "].brand",
                        "Brand " + brandKey + " does not resolve.");
                    continue;
                }
                var id = Key<NativeCassetteModel>(
                    CatalogueEntityKind.CassetteModel,
                    identityNamespace,
                    "cassette-model:" + key);
                var legacyCounter = Integer(row, "Number");
                modelIds.Add(key, id);
                models.Add(new CatalogueCassetteModelState(
                    id,
                    brandId,
                    Integer(row, "Type"),
                    Text(row, "Model"),
                    Text(row, "Code"),
                    key,
                    Text(row, "Name"),
                    legacyCounter,
                    NormalizeTimestamp(
                        lexemes.Get("Models", key, "Date"),
                        Date(row, "Date"),
                        "cassetteModels[" + key + "].addedUtc",
                        report),
                    Text(row, "Notes")));
                if (legacyCounter != 0)
                {
                    report.AddIssue(new MigrationIssue(
                        MigrationIssueSeverity.Warning,
                        "native-v2.legacy-model-counter-not-represented",
                        "cassetteModels[" + key + "].legacyCounter",
                        "The frozen native-v2 profile does not store the legacy model sequence counter."));
                }
                Mapping(report, "cassetteModel", key, id);
            }

            foreach (DataRow row in source.Tables["Decks"].Rows)
            {
                var key = Text(row, "Name");
                if (!RequiredKey(report, "deckUnits", key)) continue;
                var modelId = Key<NativeDeckModel>(
                    CatalogueEntityKind.DeckModel,
                    identityNamespace,
                    "deck-model:" + key);
                var unitId = Key<NativeDeckUnit>(
                    CatalogueEntityKind.DeckUnit,
                    identityNamespace,
                    "deck-unit:" + key);
                deckUnitIds.Add(key, unitId);
                deckModels.Add(new CatalogueDeckModelState(
                    modelId,
                    Text(row, "Manufacturer"),
                    Text(row, "Model"),
                    Integer(row, "Year"),
                    Capabilities(row)));
                deckUnits.Add(new CatalogueDeckUnitState(
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
                Mapping(report, "deckModel", key, modelId);
                Mapping(report, "deckUnit", key, unitId);
            }

            foreach (DataRow row in source.Tables["Tapes"].Rows)
            {
                var key = Text(row, "IdentifierShort");
                if (!RequiredKey(report, "tapes", key)) continue;
                CatalogueEntityKey modelId;
                var modelKey = Text(row, "Model");
                if (!modelIds.TryGetValue(modelKey, out modelId))
                {
                    Block(
                        report,
                        "reference.model-unresolved",
                        "tapes[" + key + "].model",
                        "Cassette model " + modelKey + " does not resolve.");
                    continue;
                }
                var tapeId = Key<NativeTape>(
                    CatalogueEntityKind.Tape,
                    identityNamespace,
                    "tape:" + key);
                var sideA = Side(
                    row, "A", key, deckUnitIds, lexemes,
                    identityNamespace, report, recordings);
                var sideB = Side(
                    row, "B", key, deckUnitIds, lexemes,
                    identityNamespace, report, recordings);
                tapes.Add(new CatalogueTapeState(
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
                Mapping(report, "tape", key, tapeId);
            }

            report.Counts.Brands = brands.Count;
            report.Counts.CassetteModels = models.Count;
            report.Counts.DeckModels = deckModels.Count;
            report.Counts.DeckUnits = deckUnits.Count;
            report.Counts.Tapes = tapes.Count;
            report.Counts.Recordings = recordings.Count;
            if (report.HasBlockingIssues) return null;

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
            var catalogueId = Key<NativeCatalogue>(
                CatalogueEntityKind.CatalogueMetadata,
                identityNamespace,
                "catalogue");
            Mapping(report, "catalogue", report.SourceRevision, catalogueId);
            return new CatalogueState(
                new CatalogueMetadataState(
                    catalogueId,
                    "C3 native-v2 migration",
                    created,
                    modified,
                    new CatalogueProvenanceState(
                        "1.1.0", report.SourceRevision, report.Profile)),
                brands,
                models,
                deckModels,
                deckUnits,
                tapes,
                recordings);
        }

        private static CatalogueTapeSideState Side(
            DataRow row,
            string suffix,
            string tapeKey,
            IDictionary<string, CatalogueEntityKey> deckUnitIds,
            LegacyTimestampLexemes lexemes,
            string identityNamespace,
            MigrationReport report,
            ICollection<CatalogueRecordingState> recordings)
        {
            var position = suffix == "A"
                ? CatalogueTapeSidePosition.A
                : CatalogueTapeSidePosition.B;
            if (!Boolean(row, "Taped" + suffix))
            {
                if (HasUnrepresentedSideData(row, suffix))
                {
                    Block(
                        report,
                        "side.unrecorded-data",
                        "tapes[" + tapeKey + "].side" + suffix,
                        "The legacy side is marked unrecorded but contains recording fields.");
                }
                return new CatalogueTapeSideState(
                    position,
                    Text(row, "Name" + suffix),
                    null);
            }

            CatalogueEntityKey deckId = null;
            var deckKey = Text(row, "Deck" + suffix);
            if (!string.IsNullOrEmpty(deckKey) &&
                !deckUnitIds.TryGetValue(deckKey, out deckId))
            {
                Block(
                    report,
                    "reference.deck-unresolved",
                    "tapes[" + tapeKey + "].side" + suffix + ".deck",
                    "Deck " + deckKey + " does not resolve.");
                deckId = null;
            }
            var recordingId = Key<NativeRecording>(
                CatalogueEntityKind.Recording,
                identityNamespace,
                "recording:" + tapeKey + ":" + suffix);
            recordings.Add(new CatalogueRecordingState(
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
                Text(row, "Title" + suffix)));
            Mapping(report, "recording", tapeKey + ":" + suffix, recordingId);
            return new CatalogueTapeSideState(
                position,
                Text(row, "Name" + suffix),
                recordingId);
        }

        private static CatalogueDeckCapabilitiesState Capabilities(DataRow row)
        {
            return new CatalogueDeckCapabilitiesState(
                Boolean(row, "Type1"), Boolean(row, "Type2"),
                Boolean(row, "Type3"), Boolean(row, "Type4"),
                Boolean(row, "HX"), Boolean(row, "MPX"),
                Boolean(row, "DolbyB"), Boolean(row, "DolbyC"),
                Boolean(row, "DolbyS"), Boolean(row, "DBX1"),
                Boolean(row, "DBX2"), Boolean(row, "Stereo"),
                Boolean(row, "ProgramSearch"), Boolean(row, "Reverse"),
                Boolean(row, "Calibration"), Boolean(row, "Azimuth"),
                Boolean(row, "DubbingSlow"), Boolean(row, "DubbingFast"),
                Integer(row, "FrequencyLow"), Integer(row, "FrequencyHigh"),
                Integer(row, "SignalRatio"), Text(row, "SignalRatioNR"),
                Decimal(row, "WowFlutter"), Decimal(row, "Distortion"),
                Integer(row, "Heads"), Integer(row, "Wells"),
                Boolean(row, "SpeedSlow"), Boolean(row, "SpeedNorm"),
                Boolean(row, "SpeedFast"));
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
                Integer(row, "Bias" + suffix) != 0 ||
                Integer(row, "BiasCal" + suffix) != 0 ||
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
                        Block(
                            report,
                            "timestamp.invalid",
                            path,
                            "Legacy timestamp cannot be interpreted deterministically: " +
                                lexeme);
                        normalized = new DateTime(fallback.Ticks, DateTimeKind.Utc);
                        code = "legacy-timestamp-fallback";
                    }
                    else
                    {
                        normalized = DateTime.SpecifyKind(
                            wallClock, DateTimeKind.Utc);
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
                code, path, lexeme, Canonical(normalized)));
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

        private static CatalogueEntityKey Key<T>(
            CatalogueEntityKind kind,
            string identityNamespace,
            string canonicalKey)
        {
            return new CatalogueEntityKey(
                kind,
                DeterministicEntityId.FromCanonicalKey<T>(
                    identityNamespace, canonicalKey).ToString());
        }

        private static bool RequiredKey(
            MigrationReport report,
            string collection,
            string key)
        {
            if (!string.IsNullOrWhiteSpace(key)) return true;
            Block(
                report,
                "legacy.key-missing",
                collection,
                "A required legacy key is empty.");
            return false;
        }

        private static void Mapping(
            MigrationReport report,
            string kind,
            string key,
            CatalogueEntityKey id)
        {
            report.AddMapping(new MigrationMapping(kind, key, id.EntityId));
        }

        private static void Block(
            MigrationReport report,
            string code,
            string path,
            string message)
        {
            report.AddIssue(new MigrationIssue(
                MigrationIssueSeverity.Blocking, code, path, message));
        }

        private static string Text(DataRow row, string name)
        {
            var value = row[name];
            return value == null || value == DBNull.Value
                ? string.Empty
                : Convert.ToString(value, CultureInfo.InvariantCulture) ??
                    string.Empty;
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
