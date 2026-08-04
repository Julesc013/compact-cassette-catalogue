using System;

namespace C3.Domain.Identity
{
    /// <summary>
    /// A stable opaque identifier whose generic argument prevents accidental
    /// relationships between different aggregate types.
    /// </summary>
    public readonly struct EntityId<TAggregate> :
        IEquatable<EntityId<TAggregate>>,
        IComparable<EntityId<TAggregate>>
    {
        private readonly Guid value;

        public EntityId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("An entity identifier cannot be empty.", nameof(value));
            }

            this.value = value;
        }

        public bool IsEmpty => value == Guid.Empty;

        public static EntityId<TAggregate> Parse(string text)
        {
            EntityId<TAggregate> result;
            if (!TryParse(text, out result))
            {
                throw new FormatException("Entity identifiers must contain exactly 32 lowercase hexadecimal characters.");
            }

            return result;
        }

        public static bool TryParse(string text, out EntityId<TAggregate> result)
        {
            result = default(EntityId<TAggregate>);
            if (text == null || text.Length != 32)
            {
                return false;
            }

            for (var index = 0; index < text.Length; index++)
            {
                var character = text[index];
                if (!((character >= '0' && character <= '9') ||
                    (character >= 'a' && character <= 'f')))
                {
                    return false;
                }
            }

            Guid parsed;
            if (!Guid.TryParseExact(text, "N", out parsed) || parsed == Guid.Empty)
            {
                return false;
            }

            result = new EntityId<TAggregate>(parsed);
            return true;
        }

        public int CompareTo(EntityId<TAggregate> other)
        {
            return StringComparer.Ordinal.Compare(ToString(), other.ToString());
        }

        public bool Equals(EntityId<TAggregate> other)
        {
            return value.Equals(other.value);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityId<TAggregate> && Equals((EntityId<TAggregate>)obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString("N");
        }

        public static bool operator ==(EntityId<TAggregate> left, EntityId<TAggregate> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityId<TAggregate> left, EntityId<TAggregate> right)
        {
            return !left.Equals(right);
        }
    }
}
