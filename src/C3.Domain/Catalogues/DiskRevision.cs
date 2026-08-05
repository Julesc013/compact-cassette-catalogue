using System;

namespace C3.Domain.Catalogues
{
    /// <summary>
    /// Opaque, case-sensitive identity of externally observed persisted bytes.
    /// Persistence adapters own token construction; other layers only compare it.
    /// </summary>
    public sealed class DiskRevision : IEquatable<DiskRevision>
    {
        private readonly string token;

        public DiskRevision(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(
                    "A disk revision token is required.",
                    nameof(token));
            }

            this.token = token;
        }

        public string Token => token;

        public bool Equals(DiskRevision other)
        {
            return other != null &&
                string.Equals(token, other.token, StringComparison.Ordinal);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DiskRevision);
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
