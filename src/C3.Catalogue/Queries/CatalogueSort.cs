using System;

namespace C3.Catalogue.Queries
{
    public sealed class CatalogueSort
    {
        public CatalogueSort(CatalogueFieldId field, bool descending)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
            Descending = descending;
        }

        public CatalogueFieldId Field { get; }
        public bool Descending { get; }
    }
}
