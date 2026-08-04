using System;

namespace C3.Domain.Values
{
    public readonly struct Optional<T> : IEquatable<Optional<T>>
    {
        private readonly T value;

        private Optional(T value)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.value = value;
            HasValue = true;
        }

        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("The optional value is absent.");
                }

                return value;
            }
        }

        public static Optional<T> None()
        {
            return default(Optional<T>);
        }

        public static Optional<T> Some(T value)
        {
            return new Optional<T>(value);
        }

        public bool Equals(Optional<T> other)
        {
            return HasValue == other.HasValue &&
                (!HasValue || Equals(value, other.value));
        }

        public override bool Equals(object obj)
        {
            return obj is Optional<T> && Equals((Optional<T>)obj);
        }

        public override int GetHashCode()
        {
            return HasValue ? value.GetHashCode() : 0;
        }
    }
}
