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
        private readonly string token;

        public CatalogueRevision(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(
                    "A catalogue revision token is required.",
                    nameof(token));
            }

            this.token = token;
        }

        public string Token => token;

        public bool Equals(CatalogueRevision other)
        {
            return other != null &&
                string.Equals(token, other.token, StringComparison.Ordinal);
        }

        public override bool Equals(object value)
        {
            return Equals(value as CatalogueRevision);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(token);
        }

        public override string ToString()
        {
            return token;
        }
    }
}
