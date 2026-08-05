using System;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueEntityCount
    {
        public CatalogueEntityCount(CatalogueEntityKind kind, int count)
        {
            if (!Enum.IsDefined(typeof(CatalogueEntityKind), kind))
            {
                throw new ArgumentOutOfRangeException(nameof(kind));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            Kind = kind;
            Count = count;
        }

        public CatalogueEntityKind Kind { get; }
        public int Count { get; }
    }
}
