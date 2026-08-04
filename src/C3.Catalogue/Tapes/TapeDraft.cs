namespace C3.Catalogue.Tapes
{
    public sealed class TapeDraft
    {
        public TapeDraft(
            string modelIdentifier,
            int year,
            decimal lengthMinutes,
            string region,
            int condition,
            bool packaged,
            TapeSide sideA,
            TapeSide sideB,
            string notes)
        {
            ModelIdentifier = modelIdentifier;
            Year = year;
            LengthMinutes = lengthMinutes;
            Region = region;
            Condition = condition;
            Packaged = packaged;
            SideA = sideA;
            SideB = sideB;
            Notes = notes;
        }

        public string ModelIdentifier { get; }
        public int Year { get; }
        public decimal LengthMinutes { get; }
        public string Region { get; }
        public int Condition { get; }
        public bool Packaged { get; }
        public TapeSide SideA { get; }
        public TapeSide SideB { get; }
        public string Notes { get; }
    }
}
