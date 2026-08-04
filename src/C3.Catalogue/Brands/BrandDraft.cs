namespace C3.Catalogue.Brands
{
    public sealed class BrandDraft
    {
        public BrandDraft(string name, string code, string notes)
        {
            Name = name;
            Code = code;
            Notes = notes;
        }

        public string Name { get; }

        public string Code { get; }

        public string Notes { get; }
    }
}
