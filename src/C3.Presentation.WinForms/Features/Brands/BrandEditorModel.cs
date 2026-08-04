namespace C3.Presentation.WinForms.Features.Brands
{
    public sealed class BrandEditorModel
    {
        public BrandEditorModel(bool isNew, string name, string code, string notes)
        {
            IsNew = isNew;
            Name = name ?? string.Empty;
            Code = code ?? string.Empty;
            Notes = notes ?? string.Empty;
        }

        public bool IsNew { get; }

        public string Name { get; private set; }

        public string Code { get; private set; }

        public string Notes { get; private set; }

        public void Update(string name, string code, string notes)
        {
            Name = name ?? string.Empty;
            if (IsNew)
            {
                Code = code ?? string.Empty;
            }

            Notes = notes ?? string.Empty;
        }
    }
}
