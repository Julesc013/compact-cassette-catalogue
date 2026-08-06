using C3.Catalogue.Canonical;
using C3.Catalogue.Native;
using System;
using System.Collections.Generic;

namespace C3.Infrastructure.CatalogueFiles.Canonical
{
    /// <summary>
    /// Owns adaptation from the frozen native-v2 persistence DTO graph to the
    /// format-neutral catalogue state. It performs no I/O and no mutation.
    /// </summary>
    public sealed class NativeV2ToCanonicalAdapter
    {
        public CatalogueState Adapt(NativeCatalogue source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var brands = new List<CatalogueBrandState>();
            foreach (var value in source.Brands)
            {
                brands.Add(new CatalogueBrandState(
                    Key(CatalogueEntityKind.Brand, value.Id.ToString()),
                    value.Name,
                    value.LegacyCode,
                    value.AddedAt,
                    value.Notes));
            }

            var models = new List<CatalogueCassetteModelState>();
            foreach (var value in source.CassetteModels)
            {
                models.Add(new CatalogueCassetteModelState(
                    Key(CatalogueEntityKind.CassetteModel, value.Id.ToString()),
                    Key(CatalogueEntityKind.Brand, value.BrandId.ToString()),
                    value.TypeNumber,
                    value.ModelName,
                    value.LegacyCode,
                    value.LegacyIdentifier,
                    value.DisplayName,
                    0,
                    value.AddedAt,
                    value.Notes));
            }

            var deckModels = new List<CatalogueDeckModelState>();
            foreach (var value in source.DeckModels)
            {
                var c = value.Capabilities;
                deckModels.Add(new CatalogueDeckModelState(
                    Key(CatalogueEntityKind.DeckModel, value.Id.ToString()),
                    value.Manufacturer,
                    value.Model,
                    value.Year,
                    new CatalogueDeckCapabilitiesState(
                        c.Type1, c.Type2, c.Type3, c.Type4, c.Hx, c.Mpx,
                        c.DolbyB, c.DolbyC, c.DolbyS, c.Dbx1, c.Dbx2,
                        c.Stereo, c.ProgramSearch, c.Reverse, c.Calibration,
                        c.Azimuth, c.DubbingSlow, c.DubbingFast,
                        c.FrequencyLow, c.FrequencyHigh, c.SignalRatio,
                        c.SignalRatioNoiseReduction, c.WowFlutter, c.Distortion,
                        c.Heads, c.Wells, c.SpeedSlow, c.SpeedNormal,
                        c.SpeedFast)));
            }

            var deckUnits = new List<CatalogueDeckUnitState>();
            foreach (var value in source.DeckUnits)
            {
                deckUnits.Add(new CatalogueDeckUnitState(
                    Key(CatalogueEntityKind.DeckUnit, value.Id.ToString()),
                    Key(CatalogueEntityKind.DeckModel, value.DeckModelId.ToString()),
                    value.Name,
                    value.LegacyKey,
                    value.Condition,
                    value.AddedAt,
                    value.Notes));
            }

            var tapes = new List<CatalogueTapeState>();
            var recordings = new List<CatalogueRecordingState>();
            foreach (var value in source.Tapes)
            {
                tapes.Add(new CatalogueTapeState(
                    Key(CatalogueEntityKind.Tape, value.Id.ToString()),
                    Key(
                        CatalogueEntityKind.CassetteModel,
                        value.CassetteModelId.ToString()),
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

            CatalogueProvenanceState provenance = null;
            if (source.Metadata.Provenance.HasValue)
            {
                var value = source.Metadata.Provenance.Value;
                provenance = new CatalogueProvenanceState(
                    value.SourceFormat,
                    value.SourceRevision,
                    value.MigrationProfile);
            }
            return new CatalogueState(
                new CatalogueMetadataState(
                    Key(CatalogueEntityKind.CatalogueMetadata, source.Id.ToString()),
                    source.Metadata.Producer,
                    source.Metadata.CreatedAt,
                    source.Metadata.ModifiedAt,
                    provenance),
                brands,
                models,
                deckModels,
                deckUnits,
                tapes,
                recordings);
        }

        private static CatalogueTapeSideState Side(
            NativeTapeSide source,
            ICollection<CatalogueRecordingState> recordings)
        {
            CatalogueEntityKey recordingId = null;
            if (source.Recording.HasValue)
            {
                var value = source.Recording.Value;
                recordingId = Key(
                    CatalogueEntityKind.Recording,
                    value.Id.ToString());
                CatalogueEntityKey deckId = null;
                if (value.DeckUnitId.HasValue)
                {
                    deckId = Key(
                        CatalogueEntityKind.DeckUnit,
                        value.DeckUnitId.Value.ToString());
                }
                recordings.Add(new CatalogueRecordingState(
                    recordingId,
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
            return new CatalogueTapeSideState(
                source.Position == NativeTapeSidePosition.A
                    ? CatalogueTapeSidePosition.A
                    : CatalogueTapeSidePosition.B,
                source.Name,
                recordingId);
        }

        private static CatalogueEntityKey Key(
            CatalogueEntityKind kind,
            string id)
        {
            return new CatalogueEntityKey(kind, id);
        }
    }
}
