using System;

namespace C3.Domain.Catalogues
{
    /// <summary>
    /// Identifies the exact persisted bytes from which a catalogue was loaded.
    /// Revision tokens are opaque, case-sensitive values owned by a persistence
    /// adapter; domain code compares them but never interprets their contents.
    /// </summary>
    public sealed class CatalogueRevision : IEquatable<CatalogueRevision>
    {
        private readonly DiskRevision value;

        public CatalogueRevision(string token)
        {
            value = new DiskRevision(token);
        }

        public string Token => value.Token;

        internal DiskRevision Value => value;

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
