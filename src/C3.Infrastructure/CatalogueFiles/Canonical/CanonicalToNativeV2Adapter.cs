using C3.Catalogue.Canonical;
using C3.Catalogue.Native;
using C3.Domain.Identity;
using C3.Domain.Values;
using System;
using System.Collections.Generic;

namespace C3.Infrastructure.CatalogueFiles.Canonical
{
    public sealed class CanonicalToNativeV2Adapter
    {
        public NativeCatalogue Adapt(CatalogueState source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var brands = new List<NativeBrand>();
            foreach (var value in source.Brands)
            {
                brands.Add(new NativeBrand(
                    Id<NativeBrand>(value.Id, CatalogueEntityKind.Brand),
                    value.Name,
                    value.LegacyCode,
                    value.AddedAt,
                    value.Notes));
            }

            var models = new List<NativeCassetteModel>();
            foreach (var value in source.CassetteModels)
            {
                models.Add(new NativeCassetteModel(
                    Id<NativeCassetteModel>(value.Id, CatalogueEntityKind.CassetteModel),
                    Id<NativeBrand>(value.BrandId, CatalogueEntityKind.Brand),
                    value.TypeNumber,
                    value.ModelName,
                    value.LegacyCode,
                    value.LegacyIdentifier,
                    value.DisplayName,
                    value.AddedAt,
                    value.Notes));
            }

            var deckModels = new List<NativeDeckModel>();
            foreach (var value in source.DeckModels)
            {
                var c = value.Capabilities;
                deckModels.Add(new NativeDeckModel(
                    Id<NativeDeckModel>(value.Id, CatalogueEntityKind.DeckModel),
                    value.Manufacturer,
                    value.Model,
                    value.Year,
                    new NativeDeckCapabilities(
                        c.Type1, c.Type2, c.Type3, c.Type4, c.Hx, c.Mpx,
                        c.DolbyB, c.DolbyC, c.DolbyS, c.Dbx1, c.Dbx2,
                        c.Stereo, c.ProgramSearch, c.Reverse, c.Calibration,
                        c.Azimuth, c.DubbingSlow, c.DubbingFast,
                        c.FrequencyLow, c.FrequencyHigh, c.SignalRatio,
                        c.SignalRatioNoiseReduction, c.WowFlutter, c.Distortion,
                        c.Heads, c.Wells, c.SpeedSlow, c.SpeedNormal, c.SpeedFast)));
            }

            var deckUnits = new List<NativeDeckUnit>();
            foreach (var value in source.DeckUnits)
            {
                deckUnits.Add(new NativeDeckUnit(
                    Id<NativeDeckUnit>(value.Id, CatalogueEntityKind.DeckUnit),
                    Id<NativeDeckModel>(value.DeckModelId, CatalogueEntityKind.DeckModel),
                    value.Name,
                    value.LegacyKey,
                    value.Condition,
                    value.AddedAt,
                    value.Notes));
            }

            var recordings = new Dictionary<string, CatalogueRecordingState>(
                StringComparer.Ordinal);
            foreach (var value in source.Recordings)
            {
                recordings.Add(value.Id.EntityId, value);
            }

            var tapes = new List<NativeTape>();
            foreach (var value in source.Tapes)
            {
                tapes.Add(new NativeTape(
                    Id<NativeTape>(value.Id, CatalogueEntityKind.Tape),
                    Id<NativeCassetteModel>(
                        value.CassetteModelId,
                        CatalogueEntityKind.CassetteModel),
                    value.Year,
                    value.LengthMinutes,
                    value.Region,
                    value.Number,
                    value.LegacyIdentifier,
                    value.LegacyShortIdentifier,
                    value.Condition,
                    value.Packaged,
                    value.AddedAt,
                    value.Notes,
                    Side(value.SideA, recordings),
                    Side(value.SideB, recordings)));
            }

            Optional<NativeCatalogueProvenance> provenance =
                Optional<NativeCatalogueProvenance>.None();
            if (source.Metadata.Provenance != null)
            {
                provenance = Optional<NativeCatalogueProvenance>.Some(
                    new NativeCatalogueProvenance(
                        source.Metadata.Provenance.SourceFormat,
                        source.Metadata.Provenance.SourceRevision,
                        source.Metadata.Provenance.MigrationProfile));
            }

            return new NativeCatalogue(
                Id<NativeCatalogue>(
                    source.Metadata.Id,
                    CatalogueEntityKind.CatalogueMetadata),
                new NativeCatalogueMetadata(
                    source.Metadata.Producer,
                    source.Metadata.CreatedAt,
                    source.Metadata.ModifiedAt,
                    provenance),
                brands,
                models,
                deckModels,
                deckUnits,
                tapes);
        }

        private static NativeTapeSide Side(
            CatalogueTapeSideState source,
            IDictionary<string, CatalogueRecordingState> recordings)
        {
            var recording = Optional<NativeRecording>.None();
            if (source.RecordingId != null)
            {
                var value = recordings[source.RecordingId.EntityId];
                var deckId = Optional<EntityId<NativeDeckUnit>>.None();
                if (value.DeckUnitId != null)
                {
                    deckId = Optional<EntityId<NativeDeckUnit>>.Some(
                        Id<NativeDeckUnit>(
                            value.DeckUnitId,
                            CatalogueEntityKind.DeckUnit));
                }
                recording = Optional<NativeRecording>.Some(new NativeRecording(
                    Id<NativeRecording>(value.Id, CatalogueEntityKind.Recording),
                    deckId,
                    value.RecordedAt,
                    value.InputName,
                    value.PeakLevel,
                    value.NoiseReduction,
                    value.Hx,
                    value.Mpx,
                    value.Dubbed,
                    value.Speed,
                    value.Bias,
                    value.BiasCalibration,
                    value.Equalization,
                    value.Level,
                    value.LevelCalibration,
                    value.Contents,
                    value.Artist,
                    value.Title));
            }

            return new NativeTapeSide(
                source.Position == CatalogueTapeSidePosition.A
                    ? NativeTapeSidePosition.A
                    : NativeTapeSidePosition.B,
                source.Name,
                recording);
        }

        private static EntityId<T> Id<T>(
            CatalogueEntityKey value,
            CatalogueEntityKind expectedKind)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Kind != expectedKind)
            {
                throw new ArgumentException(
                    "The canonical entity key has the wrong kind.",
                    nameof(value));
            }
            return EntityId<T>.Parse(value.EntityId);
        }
    }
}
