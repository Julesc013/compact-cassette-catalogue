using C3.Domain.Identity;
using System;

namespace C3.Domain.Catalogues
{
    /// <summary>
    /// Identifies one logical catalogue when its persistence profile supports a
    /// durable catalogue identity. Legacy profiles may legitimately omit it.
    /// </summary>
    public sealed class CatalogueId :
        IEquatable<CatalogueId>,
        IComparable<CatalogueId>
    {
        private readonly string value;

        public CatalogueId(Guid value)
        {
            this.value = CanonicalGuidText.Require(value, nameof(value));
        }

        private CatalogueId(string value)
        {
            this.value = value;
        }

        public static CatalogueId Parse(string value)
        {
            return new CatalogueId(CanonicalGuidText.Parse(value, nameof(value)));
        }

        public static bool TryParse(string value, out CatalogueId result)
        {
            Guid parsed;
            if (!CanonicalGuidText.TryParse(value, out parsed))
            {
                result = null;
                return false;
            }

            result = new CatalogueId(parsed.ToString("D"));
            return true;
        }

        public int CompareTo(CatalogueId other)
        {
            return other == null
                ? 1
                : StringComparer.Ordinal.Compare(value, other.value);
        }

        public bool Equals(CatalogueId other)
        {
            return other != null &&
                string.Equals(value, other.value, StringComparison.Ordinal);
        }

        public override bool Equals(object other)
        {
            return Equals(other as CatalogueId);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(value);
        }

        public override string ToString()
        {
            return value;
        }
    }
}
