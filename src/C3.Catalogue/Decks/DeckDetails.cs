namespace C3.Catalogue.Decks
{
    public sealed class DeckDetails
    {
        public DeckDetails(
            string manufacturer,
            string model,
            int year,
            int condition,
            bool type1,
            bool type2,
            bool type3,
            bool type4,
            bool hx,
            bool mpx,
            bool dolbyB,
            bool dolbyC,
            bool dolbyS,
            bool dbx1,
            bool dbx2,
            bool stereo,
            bool programSearch,
            bool reverse,
            bool calibration,
            bool azimuth,
            bool dubbingSlow,
            bool dubbingFast,
            int frequencyLow,
            int frequencyHigh,
            int signalRatio,
            string signalRatioNoiseReduction,
            decimal wowFlutter,
            decimal distortion,
            int heads,
            int wells,
            bool speedSlow,
            bool speedNormal,
            bool speedFast,
            string notes)
        {
            Manufacturer = manufacturer;
            Model = model;
            Year = year;
            Condition = condition;
            Type1 = type1;
            Type2 = type2;
            Type3 = type3;
            Type4 = type4;
            Hx = hx;
            Mpx = mpx;
            DolbyB = dolbyB;
            DolbyC = dolbyC;
            DolbyS = dolbyS;
            Dbx1 = dbx1;
            Dbx2 = dbx2;
            Stereo = stereo;
            ProgramSearch = programSearch;
            Reverse = reverse;
            Calibration = calibration;
            Azimuth = azimuth;
            DubbingSlow = dubbingSlow;
            DubbingFast = dubbingFast;
            FrequencyLow = frequencyLow;
            FrequencyHigh = frequencyHigh;
            SignalRatio = signalRatio;
            SignalRatioNoiseReduction = signalRatioNoiseReduction;
            WowFlutter = wowFlutter;
            Distortion = distortion;
            Heads = heads;
            Wells = wells;
            SpeedSlow = speedSlow;
            SpeedNormal = speedNormal;
            SpeedFast = speedFast;
            Notes = notes;
        }

        public string Manufacturer { get; }
        public string Model { get; }
        public int Year { get; }
        public int Condition { get; }
        public bool Type1 { get; }
        public bool Type2 { get; }
        public bool Type3 { get; }
        public bool Type4 { get; }
        public bool Hx { get; }
        public bool Mpx { get; }
        public bool DolbyB { get; }
        public bool DolbyC { get; }
        public bool DolbyS { get; }
        public bool Dbx1 { get; }
        public bool Dbx2 { get; }
        public bool Stereo { get; }
        public bool ProgramSearch { get; }
        public bool Reverse { get; }
        public bool Calibration { get; }
        public bool Azimuth { get; }
        public bool DubbingSlow { get; }
        public bool DubbingFast { get; }
        public int FrequencyLow { get; }
        public int FrequencyHigh { get; }
        public int SignalRatio { get; }
        public string SignalRatioNoiseReduction { get; }
        public decimal WowFlutter { get; }
        public decimal Distortion { get; }
        public int Heads { get; }
        public int Wells { get; }
        public bool SpeedSlow { get; }
        public bool SpeedNormal { get; }
        public bool SpeedFast { get; }
        public string Notes { get; }
    }
}
