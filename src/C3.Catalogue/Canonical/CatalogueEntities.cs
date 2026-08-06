using C3.Domain.Time;
using System;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueMetadataState
    {
        public CatalogueMetadataState(
            CatalogueEntityKey id,
            string producer,
            UtcTimestamp createdAt,
            UtcTimestamp modifiedAt,
            CatalogueProvenanceState provenance)
        {
            Id = RequireKind(id, CatalogueEntityKind.CatalogueMetadata, nameof(id));
            Producer = Required(producer, nameof(producer));
            if (modifiedAt.CompareTo(createdAt) < 0)
            {
                throw new ArgumentException(
                    "Modified time cannot precede created time.",
                    nameof(modifiedAt));
            }

            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            Provenance = provenance;
        }

        public CatalogueEntityKey Id { get; }
        public string Producer { get; }
        public UtcTimestamp CreatedAt { get; }
        public UtcTimestamp ModifiedAt { get; }
        public CatalogueProvenanceState Provenance { get; }

        internal static CatalogueEntityKey RequireKind(
            CatalogueEntityKey value,
            CatalogueEntityKind kind,
            string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            if (value.Kind != kind)
            {
                throw new ArgumentException(
                    "The entity key has the wrong catalogue kind.",
                    parameterName);
            }

            return value;
        }

        internal static string Required(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("A non-empty value is required.", parameterName);
            }

            return value;
        }

        internal static string Optional(string value)
        {
            return value ?? string.Empty;
        }
    }

    public sealed class CatalogueProvenanceState
    {
        public CatalogueProvenanceState(
            string sourceFormat,
            string sourceRevision,
            string migrationProfile)
        {
            SourceFormat = CatalogueMetadataState.Required(
                sourceFormat,
                nameof(sourceFormat));
            SourceRevision = CatalogueMetadataState.Required(
                sourceRevision,
                nameof(sourceRevision));
            MigrationProfile = CatalogueMetadataState.Required(
                migrationProfile,
                nameof(migrationProfile));
        }

        public string SourceFormat { get; }
        public string SourceRevision { get; }
        public string MigrationProfile { get; }
    }

    public sealed class CatalogueBrandState
    {
        public CatalogueBrandState(
            CatalogueEntityKey id,
            string name,
            string legacyCode,
            UtcTimestamp addedAt,
            string notes)
        {
            Id = CatalogueMetadataState.RequireKind(
                id, CatalogueEntityKind.Brand, nameof(id));
            Name = CatalogueMetadataState.Required(name, nameof(name));
            LegacyCode = CatalogueMetadataState.Required(
                legacyCode, nameof(legacyCode));
            AddedAt = addedAt;
            Notes = CatalogueMetadataState.Optional(notes);
        }

        public CatalogueEntityKey Id { get; }
        public string Name { get; }
        public string LegacyCode { get; }
        public UtcTimestamp AddedAt { get; }
        public string Notes { get; }
    }

    public sealed class CatalogueCassetteModelState
    {
        public CatalogueCassetteModelState(
            CatalogueEntityKey id,
            CatalogueEntityKey brandId,
            int typeNumber,
            string modelName,
            string legacyCode,
            string legacyIdentifier,
            string displayName,
            int legacyCounter,
            UtcTimestamp addedAt,
            string notes)
        {
            Id = CatalogueMetadataState.RequireKind(
                id, CatalogueEntityKind.CassetteModel, nameof(id));
            BrandId = CatalogueMetadataState.RequireKind(
                brandId, CatalogueEntityKind.Brand, nameof(brandId));
            TypeNumber = typeNumber;
            ModelName = CatalogueMetadataState.Required(modelName, nameof(modelName));
            LegacyCode = CatalogueMetadataState.Required(legacyCode, nameof(legacyCode));
            LegacyIdentifier = CatalogueMetadataState.Required(
                legacyIdentifier, nameof(legacyIdentifier));
            DisplayName = CatalogueMetadataState.Required(displayName, nameof(displayName));
            LegacyCounter = legacyCounter;
            AddedAt = addedAt;
            Notes = CatalogueMetadataState.Optional(notes);
        }

        public CatalogueEntityKey Id { get; }
        public CatalogueEntityKey BrandId { get; }
        public int TypeNumber { get; }
        public string ModelName { get; }
        public string LegacyCode { get; }
        public string LegacyIdentifier { get; }
        public string DisplayName { get; }
        public int LegacyCounter { get; }
        public UtcTimestamp AddedAt { get; }
        public string Notes { get; }
    }

    public sealed class CatalogueDeckModelState
    {
        public CatalogueDeckModelState(
            CatalogueEntityKey id,
            string manufacturer,
            string model,
            int year,
            CatalogueDeckCapabilitiesState capabilities)
        {
            Id = CatalogueMetadataState.RequireKind(
                id, CatalogueEntityKind.DeckModel, nameof(id));
            Manufacturer = CatalogueMetadataState.Required(
                manufacturer, nameof(manufacturer));
            Model = CatalogueMetadataState.Required(model, nameof(model));
            Year = year;
            Capabilities = capabilities ??
                throw new ArgumentNullException(nameof(capabilities));
        }

        public CatalogueEntityKey Id { get; }
        public string Manufacturer { get; }
        public string Model { get; }
        public int Year { get; }
        public CatalogueDeckCapabilitiesState Capabilities { get; }
    }

    public sealed class CatalogueDeckUnitState
    {
        public CatalogueDeckUnitState(
            CatalogueEntityKey id,
            CatalogueEntityKey deckModelId,
            string name,
            string legacyKey,
            int condition,
            UtcTimestamp addedAt,
            string notes)
        {
            Id = CatalogueMetadataState.RequireKind(
                id, CatalogueEntityKind.DeckUnit, nameof(id));
            DeckModelId = CatalogueMetadataState.RequireKind(
                deckModelId, CatalogueEntityKind.DeckModel, nameof(deckModelId));
            Name = CatalogueMetadataState.Required(name, nameof(name));
            LegacyKey = CatalogueMetadataState.Required(legacyKey, nameof(legacyKey));
            Condition = condition;
            AddedAt = addedAt;
            Notes = CatalogueMetadataState.Optional(notes);
        }

        public CatalogueEntityKey Id { get; }
        public CatalogueEntityKey DeckModelId { get; }
        public string Name { get; }
        public string LegacyKey { get; }
        public int Condition { get; }
        public UtcTimestamp AddedAt { get; }
        public string Notes { get; }
    }

    public sealed class CatalogueDeckCapabilitiesState
    {
        public CatalogueDeckCapabilitiesState(
            bool type1, bool type2, bool type3, bool type4, bool hx, bool mpx,
            bool dolbyB, bool dolbyC, bool dolbyS, bool dbx1, bool dbx2,
            bool stereo, bool programSearch, bool reverse, bool calibration,
            bool azimuth, bool dubbingSlow, bool dubbingFast, int frequencyLow,
            int frequencyHigh, int signalRatio, string signalRatioNoiseReduction,
            decimal wowFlutter, decimal distortion, int heads, int wells,
            bool speedSlow, bool speedNormal, bool speedFast)
        {
            Type1 = type1; Type2 = type2; Type3 = type3; Type4 = type4;
            Hx = hx; Mpx = mpx; DolbyB = dolbyB; DolbyC = dolbyC;
            DolbyS = dolbyS; Dbx1 = dbx1; Dbx2 = dbx2; Stereo = stereo;
            ProgramSearch = programSearch; Reverse = reverse;
            Calibration = calibration; Azimuth = azimuth;
            DubbingSlow = dubbingSlow; DubbingFast = dubbingFast;
            FrequencyLow = frequencyLow; FrequencyHigh = frequencyHigh;
            SignalRatio = signalRatio;
            SignalRatioNoiseReduction = CatalogueMetadataState.Optional(
                signalRatioNoiseReduction);
            WowFlutter = wowFlutter; Distortion = distortion;
            Heads = heads; Wells = wells;
            SpeedSlow = speedSlow; SpeedNormal = speedNormal; SpeedFast = speedFast;
        }

        public bool Type1 { get; } public bool Type2 { get; }
        public bool Type3 { get; } public bool Type4 { get; }
        public bool Hx { get; } public bool Mpx { get; }
        public bool DolbyB { get; } public bool DolbyC { get; }
        public bool DolbyS { get; } public bool Dbx1 { get; }
        public bool Dbx2 { get; } public bool Stereo { get; }
        public bool ProgramSearch { get; } public bool Reverse { get; }
        public bool Calibration { get; } public bool Azimuth { get; }
        public bool DubbingSlow { get; } public bool DubbingFast { get; }
        public int FrequencyLow { get; } public int FrequencyHigh { get; }
        public int SignalRatio { get; }
        public string SignalRatioNoiseReduction { get; }
        public decimal WowFlutter { get; } public decimal Distortion { get; }
        public int Heads { get; } public int Wells { get; }
        public bool SpeedSlow { get; } public bool SpeedNormal { get; }
        public bool SpeedFast { get; }
    }
}
