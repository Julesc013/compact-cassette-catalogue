using System;

namespace C3.Catalogue.CassetteModels
{
    public sealed class CassetteModel
    {
        public CassetteModel(
            string brandCode,
            int typeNumber,
            string modelName,
            string code,
            string identifier,
            string displayName,
            int tapeCount,
            DateTime addedAt,
            string notes)
        {
            BrandCode = brandCode;
            TypeNumber = typeNumber;
            ModelName = modelName;
            Code = code;
            Identifier = identifier;
            DisplayName = displayName;
            TapeCount = tapeCount;
            AddedAt = addedAt;
            Notes = notes;
        }

        public string BrandCode { get; }

        public int TypeNumber { get; }

        public string ModelName { get; }

        public string Code { get; }

        public string Identifier { get; }

        public string DisplayName { get; }

        public int TapeCount { get; }

        public DateTime AddedAt { get; }

        public string Notes { get; }
    }
}
