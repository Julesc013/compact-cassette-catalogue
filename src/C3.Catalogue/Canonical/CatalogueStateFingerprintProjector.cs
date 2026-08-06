using System;
using System.Collections.Generic;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueStateFingerprintProjector
    {
        public CatalogueFingerprintIndex Project(CatalogueState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            var entries = new List<CatalogueFingerprintEntry>
            {
                Entry(state.Metadata.Id, writer => Metadata(writer, state.Metadata))
            };
            foreach (var value in state.Brands)
                entries.Add(Entry(value.Id, writer => Brand(writer, value)));
            foreach (var value in state.CassetteModels)
                entries.Add(Entry(value.Id, writer => CassetteModel(writer, value)));
            foreach (var value in state.DeckModels)
                entries.Add(Entry(value.Id, writer => DeckModel(writer, value)));
            foreach (var value in state.DeckUnits)
                entries.Add(Entry(value.Id, writer => DeckUnit(writer, value)));
            foreach (var value in state.Tapes)
                entries.Add(Entry(value.Id, writer => Tape(writer, value)));
            foreach (var value in state.Recordings)
                entries.Add(Entry(value.Id, writer => Recording(writer, value)));
            return new CatalogueFingerprintEngine().ComputeFull(entries);
        }

        private static CatalogueFingerprintEntry Entry(
            CatalogueEntityKey key,
            Action<CanonicalDigestWriter> write)
        {
            using (var writer = new CanonicalDigestWriter())
            {
                writer.String(CatalogueEntityKey.KindCode(key.Kind));
                write(writer);
                return new CatalogueFingerprintEntry(key, writer.Complete());
            }
        }

        private static void Metadata(CanonicalDigestWriter writer, CatalogueMetadataState value)
        {
            writer.String(value.Producer);
            writer.Timestamp(value.CreatedAt);
            writer.Timestamp(value.ModifiedAt);
            writer.Boolean(value.Provenance != null);
            if (value.Provenance != null)
            {
                writer.String(value.Provenance.SourceFormat);
                writer.String(value.Provenance.SourceRevision);
                writer.String(value.Provenance.MigrationProfile);
            }
        }

        private static void Brand(CanonicalDigestWriter writer, CatalogueBrandState value)
        {
            writer.String(value.Name); writer.String(value.LegacyCode);
            writer.Timestamp(value.AddedAt); writer.String(value.Notes);
        }

        private static void CassetteModel(CanonicalDigestWriter writer, CatalogueCassetteModelState value)
        {
            writer.String(value.BrandId.EntityId); writer.Int32(value.TypeNumber);
            writer.String(value.ModelName); writer.String(value.LegacyCode);
            writer.String(value.LegacyIdentifier); writer.String(value.DisplayName);
            writer.Int32(value.LegacyCounter); writer.Timestamp(value.AddedAt);
            writer.String(value.Notes);
        }

        private static void DeckModel(CanonicalDigestWriter writer, CatalogueDeckModelState value)
        {
            writer.String(value.Manufacturer); writer.String(value.Model); writer.Int32(value.Year);
            var c = value.Capabilities;
            writer.Boolean(c.Type1); writer.Boolean(c.Type2); writer.Boolean(c.Type3); writer.Boolean(c.Type4);
            writer.Boolean(c.Hx); writer.Boolean(c.Mpx); writer.Boolean(c.DolbyB); writer.Boolean(c.DolbyC);
            writer.Boolean(c.DolbyS); writer.Boolean(c.Dbx1); writer.Boolean(c.Dbx2); writer.Boolean(c.Stereo);
            writer.Boolean(c.ProgramSearch); writer.Boolean(c.Reverse); writer.Boolean(c.Calibration);
            writer.Boolean(c.Azimuth); writer.Boolean(c.DubbingSlow); writer.Boolean(c.DubbingFast);
            writer.Int32(c.FrequencyLow); writer.Int32(c.FrequencyHigh); writer.Int32(c.SignalRatio);
            writer.String(c.SignalRatioNoiseReduction); writer.Decimal(c.WowFlutter);
            writer.Decimal(c.Distortion); writer.Int32(c.Heads); writer.Int32(c.Wells);
            writer.Boolean(c.SpeedSlow); writer.Boolean(c.SpeedNormal); writer.Boolean(c.SpeedFast);
        }

        private static void DeckUnit(CanonicalDigestWriter writer, CatalogueDeckUnitState value)
        {
            writer.String(value.DeckModelId.EntityId); writer.String(value.Name);
            writer.String(value.LegacyKey); writer.Int32(value.Condition);
            writer.Timestamp(value.AddedAt); writer.String(value.Notes);
        }

        private static void Tape(CanonicalDigestWriter writer, CatalogueTapeState value)
        {
            writer.String(value.CassetteModelId.EntityId); writer.Int32(value.Year);
            writer.Decimal(value.LengthMinutes); writer.String(value.Region); writer.Int32(value.Number);
            writer.String(value.LegacyIdentifier); writer.String(value.LegacyShortIdentifier);
            writer.Int32(value.Condition); writer.Boolean(value.Packaged);
            writer.Timestamp(value.AddedAt); writer.String(value.Notes);
            Side(writer, value.SideA); Side(writer, value.SideB);
        }

        private static void Side(CanonicalDigestWriter writer, CatalogueTapeSideState value)
        {
            writer.Int32((int)value.Position); writer.String(value.Name);
            writer.Boolean(value.RecordingId != null);
            if (value.RecordingId != null) writer.String(value.RecordingId.EntityId);
        }

        private static void Recording(CanonicalDigestWriter writer, CatalogueRecordingState value)
        {
            writer.Boolean(value.DeckUnitId != null);
            if (value.DeckUnitId != null) writer.String(value.DeckUnitId.EntityId);
            writer.Timestamp(value.RecordedAt); writer.String(value.InputName); writer.Int32(value.PeakLevel);
            writer.String(value.NoiseReduction); writer.Boolean(value.Hx); writer.Boolean(value.Mpx);
            writer.Boolean(value.Dubbed); writer.String(value.Speed); writer.Int32(value.Bias);
            writer.Int32(value.BiasCalibration); writer.String(value.Equalization);
            writer.Decimal(value.Level); writer.Decimal(value.LevelCalibration);
            writer.String(value.Contents); writer.String(value.Artist); writer.String(value.Title);
        }
    }
}
