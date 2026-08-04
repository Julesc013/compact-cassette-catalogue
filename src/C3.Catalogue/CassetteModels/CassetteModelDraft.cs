namespace C3.Catalogue.CassetteModels
{
    public sealed class CassetteModelDraft
    {
        public CassetteModelDraft(
            string brandCode,
            int typeNumber,
            string modelName,
            string code,
            string displayName,
            string notes)
        {
            BrandCode = brandCode;
            TypeNumber = typeNumber;
            ModelName = modelName;
            Code = code;
            DisplayName = displayName;
            Notes = notes;
        }

        public string BrandCode { get; }

        public int TypeNumber { get; }

        public string ModelName { get; }

        public string Code { get; }

        public string DisplayName { get; }

        public string Notes { get; }
    }
}
