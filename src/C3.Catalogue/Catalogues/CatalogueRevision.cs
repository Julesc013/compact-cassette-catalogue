using System;

namespace C3.Catalogue.Catalogues
{
    public sealed class CatalogueRevision : IEquatable<CatalogueRevision>
    {
        private readonly C3.Domain.Catalogues.CatalogueRevision value;

        public CatalogueRevision(string token)
        {
            value = new C3.Domain.Catalogues.CatalogueRevision(token);
        }

        public string Token => value.Token;

        internal C3.Domain.Catalogues.CatalogueRevision Value => value;

        public bool Equals(CatalogueRevision other)
        {
            return other != null && value.Equals(other.value);
        }

        public override bool Equals(object value)
        {
            return Equals(value as CatalogueRevision);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
