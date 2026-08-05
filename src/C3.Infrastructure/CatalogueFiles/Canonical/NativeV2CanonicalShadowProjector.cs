using C3.Catalogue.Canonical;
using C3.Catalogue.Native;
using C3.Domain.Catalogues;
using C3.Domain.Profiles;
using System;
using System.Collections.Generic;

namespace C3.Infrastructure.CatalogueFiles.Canonical
{
    public sealed class NativeV2CanonicalShadowProjector
    {
        public CanonicalShadowProjection Project(
            NativeCatalogue source,
            DocumentSessionId sessionId,
            ContentVersion contentVersion,
            CatalogueResourceBudget budget)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (sessionId == null)
            {
                throw new ArgumentNullException(nameof(sessionId));
            }
            if (contentVersion == null)
            {
                throw new ArgumentNullException(nameof(contentVersion));
            }
            if (budget == null)
            {
                throw new ArgumentNullException(nameof(budget));
            }

            var entries = new List<CatalogueFingerprintEntry>();
            entries.Add(Entry(
                CatalogueEntityKind.CatalogueMetadata,
                source.Id.ToString(),
                writer => Metadata(writer, source.Metadata)));
            foreach (var brand in source.Brands)
            {
                entries.Add(Entry(
                    CatalogueEntityKind.Brand,
                    brand.Id.ToString(),
                    writer => Brand(writer, brand)));
            }
            foreach (var model in source.CassetteModels)
            {
                entries.Add(Entry(
                    CatalogueEntityKind.CassetteModel,
                    model.Id.ToString(),
                    writer => CassetteModel(writer, model)));
            }
            foreach (var model in source.DeckModels)
            {
                entries.Add(Entry(
                    CatalogueEntityKind.DeckModel,
                    model.Id.ToString(),
                    writer => DeckModel(writer, model)));
            }
            foreach (var unit in source.DeckUnits)
            {
                entries.Add(Entry(
                    CatalogueEntityKind.DeckUnit,
                    unit.Id.ToString(),
                    writer => DeckUnit(writer, unit)));
            }

            var recordingCount = 0;
            foreach (var tape in source.Tapes)
            {
                entries.Add(Entry(
                    CatalogueEntityKind.Tape,
                    tape.Id.ToString(),
                    writer => Tape(writer, tape)));
                recordingCount += Recording(
                    entries,
                    tape.SideA);
                recordingCount += Recording(
                    entries,
                    tape.SideB);
            }

            var fingerprints = new CatalogueFingerprintEngine().ComputeFull(entries);
            var snapshot = new CatalogueSnapshot(
                sessionId,
                contentVersion,
                fingerprints.Root,
                new[]
                {
                    new CatalogueEntityCount(CatalogueEntityKind.Brand, source.Brands.Count),
                    new CatalogueEntityCount(CatalogueEntityKind.CassetteModel, source.CassetteModels.Count),
                    new CatalogueEntityCount(CatalogueEntityKind.DeckModel, source.DeckModels.Count),
                    new CatalogueEntityCount(CatalogueEntityKind.DeckUnit, source.DeckUnits.Count),
                    new CatalogueEntityCount(CatalogueEntityKind.Tape, source.Tapes.Count),
                    new CatalogueEntityCount(CatalogueEntityKind.Recording, recordingCount)
                });
            new CatalogueDocument(snapshot, budget);
            return new CanonicalShadowProjection(
                KnownCatalogueProfiles.NativeV2_0,
                snapshot,
                fingerprints);
        }

        private static CatalogueFingerprintEntry Entry(
            CatalogueEntityKind kind,
            string id,
            Action<CanonicalDigestWriter> write)
        {
            using (var writer = new CanonicalDigestWriter())
            {
                writer.String(CatalogueEntityKey.KindCode(kind));
                write(writer);
                return new CatalogueFingerprintEntry(
                    new CatalogueEntityKey(kind, id),
                    writer.Complete());
            }
        }

        private static void Metadata(
            CanonicalDigestWriter writer,
            NativeCatalogueMetadata value)
        {
            writer.String(value.Producer);
            writer.Timestamp(value.CreatedAt);
            writer.Timestamp(value.ModifiedAt);
            writer.Boolean(value.Provenance.HasValue);
            if (value.Provenance.HasValue)
            {
                writer.String(value.Provenance.Value.SourceFormat);
                writer.String(value.Provenance.Value.SourceRevision);
                writer.String(value.Provenance.Value.MigrationProfile);
            }
        }

        private static void Brand(CanonicalDigestWriter writer, NativeBrand value)
        {
            writer.String(value.Name);
            writer.String(value.LegacyCode);
            writer.Timestamp(value.AddedAt);
            writer.String(value.Notes);
        }

        private static void CassetteModel(
            CanonicalDigestWriter writer,
            NativeCassetteModel value)
        {
            writer.String(value.BrandId.ToString());
            writer.Int32(value.TypeNumber);
            writer.String(value.ModelName);
            writer.String(value.LegacyCode);
            writer.String(value.LegacyIdentifier);
            writer.String(value.DisplayName);
            writer.Timestamp(value.AddedAt);
            writer.String(value.Notes);
        }

        private static void DeckModel(
            CanonicalDigestWriter writer,
            NativeDeckModel value)
        {
            writer.String(value.Manufacturer);
            writer.String(value.Model);
            writer.Int32(value.Year);
            var c = value.Capabilities;
            writer.Boolean(c.Type1); writer.Boolean(c.Type2);
            writer.Boolean(c.Type3); writer.Boolean(c.Type4);
            writer.Boolean(c.Hx); writer.Boolean(c.Mpx);
            writer.Boolean(c.DolbyB); writer.Boolean(c.DolbyC);
            writer.Boolean(c.DolbyS); writer.Boolean(c.Dbx1);
            writer.Boolean(c.Dbx2); writer.Boolean(c.Stereo);
            writer.Boolean(c.ProgramSearch); writer.Boolean(c.Reverse);
            writer.Boolean(c.Calibration); writer.Boolean(c.Azimuth);
            writer.Boolean(c.DubbingSlow); writer.Boolean(c.DubbingFast);
            writer.Int32(c.FrequencyLow); writer.Int32(c.FrequencyHigh);
            writer.Int32(c.SignalRatio); writer.String(c.SignalRatioNoiseReduction);
            writer.Decimal(c.WowFlutter); writer.Decimal(c.Distortion);
            writer.Int32(c.Heads); writer.Int32(c.Wells);
            writer.Boolean(c.SpeedSlow); writer.Boolean(c.SpeedNormal);
            writer.Boolean(c.SpeedFast);
        }

        private static void DeckUnit(
            CanonicalDigestWriter writer,
            NativeDeckUnit value)
        {
            writer.String(value.DeckModelId.ToString());
            writer.String(value.Name);
            writer.String(value.LegacyKey);
            writer.Int32(value.Condition);
            writer.Timestamp(value.AddedAt);
            writer.String(value.Notes);
        }

        private static void Tape(CanonicalDigestWriter writer, NativeTape value)
        {
            writer.String(value.CassetteModelId.ToString());
            writer.Int32(value.Year);
            writer.Decimal(value.LengthMinutes);
            writer.String(value.Region);
            writer.Int32(value.Number);
            writer.String(value.LegacyIdentifier);
            writer.String(value.LegacyShortIdentifier);
            writer.Int32(value.Condition);
            writer.Boolean(value.Packaged);
            writer.Timestamp(value.AddedAt);
            writer.String(value.Notes);
            Side(writer, value.SideA);
            Side(writer, value.SideB);
        }

        private static void Side(CanonicalDigestWriter writer, NativeTapeSide value)
        {
            writer.Int32((int)value.Position);
            writer.String(value.Name);
            writer.Boolean(value.Recording.HasValue);
            if (value.Recording.HasValue)
            {
                writer.String(value.Recording.Value.Id.ToString());
            }
        }

        private static int Recording(
            ICollection<CatalogueFingerprintEntry> entries,
            NativeTapeSide side)
        {
            if (!side.Recording.HasValue)
            {
                return 0;
            }

            var value = side.Recording.Value;
            entries.Add(Entry(
                CatalogueEntityKind.Recording,
                value.Id.ToString(),
                writer => RecordingValue(writer, value)));
            return 1;
        }

        private static void RecordingValue(
            CanonicalDigestWriter writer,
            NativeRecording value)
        {
            writer.Boolean(value.DeckUnitId.HasValue);
            if (value.DeckUnitId.HasValue)
            {
                writer.String(value.DeckUnitId.Value.ToString());
            }
            writer.Timestamp(value.RecordedAt);
            writer.String(value.InputName);
            writer.Int32(value.PeakLevel);
            writer.String(value.NoiseReduction);
            writer.Boolean(value.Hx); writer.Boolean(value.Mpx);
            writer.Boolean(value.Dubbed); writer.String(value.Speed);
            writer.Int32(value.Bias); writer.Int32(value.BiasCalibration);
            writer.String(value.Equalization); writer.Decimal(value.Level);
            writer.Decimal(value.LevelCalibration); writer.String(value.Contents);
            writer.String(value.Artist); writer.String(value.Title);
        }
    }
}
