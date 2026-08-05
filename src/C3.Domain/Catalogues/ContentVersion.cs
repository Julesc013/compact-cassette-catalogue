using System;
using System.Globalization;

namespace C3.Domain.Catalogues
{
    /// <summary>
    /// Monotonic version of semantic content within one document session.
    /// Undo and redo create new versions; they never move this value backward.
    /// </summary>
    public sealed class ContentVersion :
        IEquatable<ContentVersion>,
        IComparable<ContentVersion>
    {
        public ContentVersion(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Value = value;
        }

        public long Value { get; }

        public static ContentVersion Zero => new ContentVersion(0);

        public ContentVersion Next()
        {
            if (Value == long.MaxValue)
            {
                throw new InvalidOperationException(
                    "The document content version cannot advance beyond Int64.MaxValue.");
            }

            return new ContentVersion(Value + 1);
        }

        public int CompareTo(ContentVersion other)
        {
            return other == null ? 1 : Value.CompareTo(other.Value);
        }

        public bool Equals(ContentVersion other)
        {
            return other != null && Value == other.Value;
        }

        public override bool Equals(object other)
        {
            return Equals(other as ContentVersion);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
