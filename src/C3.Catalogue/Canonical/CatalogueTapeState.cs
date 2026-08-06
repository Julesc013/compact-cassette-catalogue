using C3.Domain.Time;
using System;

namespace C3.Catalogue.Canonical
{
    public enum CatalogueTapeSidePosition
    {
        A = 0,
        B = 1
    }

    public sealed class CatalogueTapeState
    {
        public CatalogueTapeState(
            CatalogueEntityKey id,
            CatalogueEntityKey cassetteModelId,
            int year,
            decimal lengthMinutes,
            string region,
            int number,
            string legacyIdentifier,
            string legacyShortIdentifier,
            int condition,
            bool packaged,
            UtcTimestamp addedAt,
            string notes,
            CatalogueTapeSideState sideA,
            CatalogueTapeSideState sideB)
        {
            if (lengthMinutes < decimal.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(lengthMinutes));
            }

            Id = CatalogueMetadataState.RequireKind(
                id, CatalogueEntityKind.Tape, nameof(id));
            CassetteModelId = CatalogueMetadataState.RequireKind(
                cassetteModelId,
                CatalogueEntityKind.CassetteModel,
                nameof(cassetteModelId));
            Year = year;
            LengthMinutes = lengthMinutes;
            Region = CatalogueMetadataState.Optional(region);
            Number = number;
            LegacyIdentifier = CatalogueMetadataState.Required(
                legacyIdentifier, nameof(legacyIdentifier));
            LegacyShortIdentifier = CatalogueMetadataState.Required(
                legacyShortIdentifier, nameof(legacyShortIdentifier));
            Condition = condition;
            Packaged = packaged;
            AddedAt = addedAt;
            Notes = CatalogueMetadataState.Optional(notes);
            SideA = sideA ?? throw new ArgumentNullException(nameof(sideA));
            SideB = sideB ?? throw new ArgumentNullException(nameof(sideB));
            if (SideA.Position != CatalogueTapeSidePosition.A ||
                SideB.Position != CatalogueTapeSidePosition.B)
            {
                throw new ArgumentException(
                    "A tape must own exactly sides A and B in canonical order.");
            }
        }

        public CatalogueEntityKey Id { get; }
        public CatalogueEntityKey CassetteModelId { get; }
        public int Year { get; }
        public decimal LengthMinutes { get; }
        public string Region { get; }
        public int Number { get; }
        public string LegacyIdentifier { get; }
        public string LegacyShortIdentifier { get; }
        public int Condition { get; }
        public bool Packaged { get; }
        public UtcTimestamp AddedAt { get; }
        public string Notes { get; }
        public CatalogueTapeSideState SideA { get; }
        public CatalogueTapeSideState SideB { get; }
    }

    public sealed class CatalogueTapeSideState
    {
        public CatalogueTapeSideState(
            CatalogueTapeSidePosition position,
            string name,
            CatalogueEntityKey recordingId)
        {
            if (!Enum.IsDefined(typeof(CatalogueTapeSidePosition), position))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }
            if (recordingId != null)
            {
                CatalogueMetadataState.RequireKind(
                    recordingId,
                    CatalogueEntityKind.Recording,
                    nameof(recordingId));
            }

            Position = position;
            Name = CatalogueMetadataState.Optional(name);
            RecordingId = recordingId;
        }

        public CatalogueTapeSidePosition Position { get; }
        public string Name { get; }
        public CatalogueEntityKey RecordingId { get; }
    }

    public sealed class CatalogueRecordingState
    {
        public CatalogueRecordingState(
            CatalogueEntityKey id,
            CatalogueEntityKey deckUnitId,
            UtcTimestamp recordedAt,
            string inputName,
            int peakLevel,
            string noiseReduction,
            bool hx,
            bool mpx,
            bool dubbed,
            string speed,
            int bias,
            int biasCalibration,
            string equalization,
            decimal level,
            decimal levelCalibration,
            string contents,
            string artist,
            string title)
        {
            Id = CatalogueMetadataState.RequireKind(
                id, CatalogueEntityKind.Recording, nameof(id));
            if (deckUnitId != null)
            {
                CatalogueMetadataState.RequireKind(
                    deckUnitId,
                    CatalogueEntityKind.DeckUnit,
                    nameof(deckUnitId));
            }

            DeckUnitId = deckUnitId;
            RecordedAt = recordedAt;
            InputName = CatalogueMetadataState.Optional(inputName);
            PeakLevel = peakLevel;
            NoiseReduction = CatalogueMetadataState.Optional(noiseReduction);
            Hx = hx; Mpx = mpx; Dubbed = dubbed;
            Speed = CatalogueMetadataState.Optional(speed);
            Bias = bias; BiasCalibration = biasCalibration;
            Equalization = CatalogueMetadataState.Optional(equalization);
            Level = level; LevelCalibration = levelCalibration;
            Contents = CatalogueMetadataState.Optional(contents);
            Artist = CatalogueMetadataState.Optional(artist);
            Title = CatalogueMetadataState.Optional(title);
        }

        public CatalogueEntityKey Id { get; }
        public CatalogueEntityKey DeckUnitId { get; }
        public UtcTimestamp RecordedAt { get; }
        public string InputName { get; }
        public int PeakLevel { get; }
        public string NoiseReduction { get; }
        public bool Hx { get; }
        public bool Mpx { get; }
        public bool Dubbed { get; }
        public string Speed { get; }
        public int Bias { get; }
        public int BiasCalibration { get; }
        public string Equalization { get; }
        public decimal Level { get; }
        public decimal LevelCalibration { get; }
        public string Contents { get; }
        public string Artist { get; }
        public string Title { get; }
    }
}
