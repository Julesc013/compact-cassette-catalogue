using System;

namespace C3.Catalogue.Brands
{
    public sealed class Brand
    {
        public Brand(string name, string code, DateTime addedAt, string notes)
        {
            Name = name;
            Code = code;
            AddedAt = addedAt;
            Notes = notes;
        }

        public string Name { get; }

        public string Code { get; }

        public DateTime AddedAt { get; }

        public string Notes { get; }
    }
}
