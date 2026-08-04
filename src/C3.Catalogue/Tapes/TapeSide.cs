using System;

namespace C3.Catalogue.Tapes
{
    public sealed class TapeSide
    {
        public TapeSide(
            bool isRecorded,
            string name,
            DateTime recordedAt,
            string deckName,
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
            IsRecorded = isRecorded;
            Name = name;
            RecordedAt = recordedAt;
            DeckName = deckName;
            InputName = inputName;
            PeakLevel = peakLevel;
            NoiseReduction = noiseReduction;
            Hx = hx;
            Mpx = mpx;
            Dubbed = dubbed;
            Speed = speed;
            Bias = bias;
            BiasCalibration = biasCalibration;
            Equalization = equalization;
            Level = level;
            LevelCalibration = levelCalibration;
            Contents = contents;
            Artist = artist;
            Title = title;
        }

        public bool IsRecorded { get; }
        public string Name { get; }
        public DateTime RecordedAt { get; }
        public string DeckName { get; }
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

        public static TapeSide Empty()
        {
            return new TapeSide(
                false,
                string.Empty,
                DateTime.MinValue,
                string.Empty,
                string.Empty,
                0,
                string.Empty,
                false,
                false,
                false,
                string.Empty,
                0,
                0,
                string.Empty,
                decimal.Zero,
                decimal.Zero,
                string.Empty,
                string.Empty,
                string.Empty);
        }
    }
}
