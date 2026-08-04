using System;

namespace C3.Domain.Time
{
    public readonly struct UtcTimestamp : IEquatable<UtcTimestamp>, IComparable<UtcTimestamp>
    {
        private readonly long ticks;

        public UtcTimestamp(DateTime value)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Domain timestamps must be explicitly UTC.", nameof(value));
            }

            ticks = value.Ticks;
        }

        public DateTime Value => new DateTime(ticks, DateTimeKind.Utc);

        public int CompareTo(UtcTimestamp other)
        {
            return ticks.CompareTo(other.ticks);
        }

        public bool Equals(UtcTimestamp other)
        {
            return ticks == other.ticks;
        }

        public override bool Equals(object obj)
        {
            return obj is UtcTimestamp && Equals((UtcTimestamp)obj);
        }

        public override int GetHashCode()
        {
            return ticks.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("o");
        }
    }
}
