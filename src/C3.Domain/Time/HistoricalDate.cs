using C3.Domain.Values;
using System;
using System.Globalization;

namespace C3.Domain.Time
{
    public enum HistoricalDatePrecision
    {
        Year = 0,
        Month = 1,
        Day = 2
    }

    public readonly struct HistoricalDate :
        IEquatable<HistoricalDate>,
        IComparable<HistoricalDate>
    {
        private HistoricalDate(int year, int? month, int? day)
        {
            if (year < 1 || year > 9999)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }
            if (day.HasValue && !month.HasValue)
            {
                throw new ArgumentException(
                    "A historical day requires a month.",
                    nameof(day));
            }
            if (month.HasValue && (month.Value < 1 || month.Value > 12))
            {
                throw new ArgumentOutOfRangeException(nameof(month));
            }
            if (day.HasValue)
            {
                var maximumDay = DateTime.DaysInMonth(year, month.Value);
                if (day.Value < 1 || day.Value > maximumDay)
                {
                    throw new ArgumentOutOfRangeException(nameof(day));
                }
            }

            Year = year;
            Month = month.HasValue
                ? Optional<int>.Some(month.Value)
                : Optional<int>.None();
            Day = day.HasValue
                ? Optional<int>.Some(day.Value)
                : Optional<int>.None();
        }

        public int Year { get; }

        public Optional<int> Month { get; }

        public Optional<int> Day { get; }

        public HistoricalDatePrecision Precision => Day.HasValue
            ? HistoricalDatePrecision.Day
            : Month.HasValue
                ? HistoricalDatePrecision.Month
                : HistoricalDatePrecision.Year;

        public static HistoricalDate FromYear(int year)
        {
            return new HistoricalDate(year, null, null);
        }

        public static HistoricalDate FromYearMonth(int year, int month)
        {
            return new HistoricalDate(year, month, null);
        }

        public static HistoricalDate FromDate(int year, int month, int day)
        {
            return new HistoricalDate(year, month, day);
        }

        public int CompareTo(HistoricalDate other)
        {
            var result = Year.CompareTo(other.Year);
            if (result != 0)
            {
                return result;
            }

            result = Component(Month).CompareTo(Component(other.Month));
            return result != 0
                ? result
                : Component(Day).CompareTo(Component(other.Day));
        }

        public bool Equals(HistoricalDate other)
        {
            return Year == other.Year &&
                Month.Equals(other.Month) &&
                Day.Equals(other.Day);
        }

        public override bool Equals(object obj)
        {
            return obj is HistoricalDate && Equals((HistoricalDate)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Year * 397) ^ Month.GetHashCode()) * 397 ^
                    Day.GetHashCode();
            }
        }

        public override string ToString()
        {
            var text = Year.ToString("D4", CultureInfo.InvariantCulture);
            if (Month.HasValue)
            {
                text += "-" + Month.Value.ToString("D2", CultureInfo.InvariantCulture);
            }
            if (Day.HasValue)
            {
                text += "-" + Day.Value.ToString("D2", CultureInfo.InvariantCulture);
            }

            return text;
        }

        private static int Component(Optional<int> value)
        {
            return value.HasValue ? value.Value : 0;
        }
    }
}
