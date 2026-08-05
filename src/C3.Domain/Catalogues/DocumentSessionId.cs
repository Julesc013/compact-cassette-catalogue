using C3.Domain.Identity;
using System;

namespace C3.Domain.Catalogues
{
    /// <summary>
    /// Identifies one exact open-document session. It is never persisted as the
    /// logical catalogue identity and is never reused after that session closes.
    /// </summary>
    public sealed class DocumentSessionId :
        IEquatable<DocumentSessionId>,
        IComparable<DocumentSessionId>
    {
        private readonly string value;

        public DocumentSessionId(Guid value)
        {
            this.value = CanonicalGuidText.Require(value, nameof(value));
        }

        private DocumentSessionId(string value)
        {
            this.value = value;
        }

        public static DocumentSessionId Parse(string value)
        {
            return new DocumentSessionId(
                CanonicalGuidText.Parse(value, nameof(value)));
        }

        public static bool TryParse(string value, out DocumentSessionId result)
        {
            Guid parsed;
            if (!CanonicalGuidText.TryParse(value, out parsed))
            {
                result = null;
                return false;
            }

            result = new DocumentSessionId(parsed.ToString("D"));
            return true;
        }

        public int CompareTo(DocumentSessionId other)
        {
            return other == null
                ? 1
                : StringComparer.Ordinal.Compare(value, other.value);
        }

        public bool Equals(DocumentSessionId other)
        {
            return other != null &&
                string.Equals(value, other.value, StringComparison.Ordinal);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DocumentSessionId);
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
