using C3.Domain.Identity;
using C3.Domain.Time;
using C3.Domain.Values;
using System;

namespace C3.Catalogue.Native
{
    public enum NativeTapeSidePosition
    {
        A = 0,
        B = 1
    }

    public sealed class NativeTape
    {
        public NativeTape(
            EntityId<NativeTape> id,
            EntityId<NativeCassetteModel> cassetteModelId,
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
            NativeTapeSide sideA,
            NativeTapeSide sideB)
        {
            if (lengthMinutes < decimal.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(lengthMinutes));
            }

            Id = id;
            CassetteModelId = cassetteModelId;
            Year = year;
            LengthMinutes = lengthMinutes;
            Region = NativeText.Optional(region);
            Number = number;
            LegacyIdentifier = NativeText.Required(legacyIdentifier, nameof(legacyIdentifier));
            LegacyShortIdentifier = NativeText.Required(legacyShortIdentifier, nameof(legacyShortIdentifier));
            Condition = condition;
            Packaged = packaged;
            AddedAt = addedAt;
            Notes = NativeText.Optional(notes);
            SideA = sideA ?? throw new ArgumentNullException(nameof(sideA));
            SideB = sideB ?? throw new ArgumentNullException(nameof(sideB));
            if (SideA.Position != NativeTapeSidePosition.A || SideB.Position != NativeTapeSidePosition.B)
            {
                throw new ArgumentException("A tape must own exactly sides A and B in canonical order.");
            }
        }

        public EntityId<NativeTape> Id { get; }
        public EntityId<NativeCassetteModel> CassetteModelId { get; }
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
        public NativeTapeSide SideA { get; }
        public NativeTapeSide SideB { get; }
    }

    public sealed class NativeTapeSide
    {
        public NativeTapeSide(
            NativeTapeSidePosition position,
            string name,
            Optional<NativeRecording> recording)
        {
            Position = position;
            Name = NativeText.Optional(name);
            Recording = recording;
        }

        public NativeTapeSidePosition Position { get; }
        public string Name { get; }
        public Optional<NativeRecording> Recording { get; }
    }

    public sealed class NativeRecording
    {
        public NativeRecording(
            EntityId<NativeRecording> id,
            Optional<EntityId<NativeDeckUnit>> deckUnitId,
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
            Id = id;
            DeckUnitId = deckUnitId;
            RecordedAt = recordedAt;
            InputName = NativeText.Optional(inputName);
            PeakLevel = peakLevel;
            NoiseReduction = NativeText.Optional(noiseReduction);
            Hx = hx; Mpx = mpx; Dubbed = dubbed;
            Speed = NativeText.Optional(speed);
            Bias = bias; BiasCalibration = biasCalibration;
            Equalization = NativeText.Optional(equalization);
            Level = level; LevelCalibration = levelCalibration;
            Contents = NativeText.Optional(contents);
            Artist = NativeText.Optional(artist);
            Title = NativeText.Optional(title);
        }

        public EntityId<NativeRecording> Id { get; }
        public Optional<EntityId<NativeDeckUnit>> DeckUnitId { get; }
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
