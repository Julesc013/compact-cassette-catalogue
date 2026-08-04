using System;

namespace C3.Catalogue.Tapes
{
    public sealed class Tape
    {
        public Tape(
            string modelIdentifier,
            int year,
            decimal lengthMinutes,
            string region,
            int number,
            string identifier,
            string shortIdentifier,
            int condition,
            bool packaged,
            TapeSide sideA,
            TapeSide sideB,
            DateTime addedAt,
            string notes)
        {
            ModelIdentifier = modelIdentifier;
            Year = year;
            LengthMinutes = lengthMinutes;
            Region = region;
            Number = number;
            Identifier = identifier;
            ShortIdentifier = shortIdentifier;
            Condition = condition;
            Packaged = packaged;
            SideA = sideA;
            SideB = sideB;
            AddedAt = addedAt;
            Notes = notes;
        }

        public string ModelIdentifier { get; }
        public int Year { get; }
        public decimal LengthMinutes { get; }
        public string Region { get; }
        public int Number { get; }
        public string Identifier { get; }
        public string ShortIdentifier { get; }
        public int Condition { get; }
        public bool Packaged { get; }
        public TapeSide SideA { get; }
        public TapeSide SideB { get; }
        public DateTime AddedAt { get; }
        public string Notes { get; }
    }
}
