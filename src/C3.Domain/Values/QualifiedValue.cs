using System;
using System.Collections.Generic;

namespace C3.Domain.Values
{
    public readonly struct QualifiedValue<T> : IEquatable<QualifiedValue<T>>
    {
        private readonly T value;

        private QualifiedValue(ValueKnowledge knowledge, T value, bool hasValue)
        {
            if (hasValue && ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (hasValue && knowledge != ValueKnowledge.Known &&
                knowledge != ValueKnowledge.Estimated &&
                knowledge != ValueKnowledge.Inferred)
            {
                throw new ArgumentException(
                    "A present value must be known, estimated, or inferred.",
                    nameof(knowledge));
            }
            if (!hasValue && knowledge != ValueKnowledge.Unknown &&
                knowledge != ValueKnowledge.NotApplicable)
            {
                throw new ArgumentException(
                    "A valueless qualification must be unknown or not applicable.",
                    nameof(knowledge));
            }

            Knowledge = knowledge;
            this.value = value;
            HasValue = hasValue;
        }

        public ValueKnowledge Knowledge { get; }

        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException(
                        "Unknown and not-applicable values have no payload.");
                }

                return value;
            }
        }

        public static QualifiedValue<T> Unknown()
        {
            return default(QualifiedValue<T>);
        }

        public static QualifiedValue<T> NotApplicable()
        {
            return new QualifiedValue<T>(
                ValueKnowledge.NotApplicable,
                default(T),
                false);
        }

        public static QualifiedValue<T> Known(T value)
        {
            return Present(ValueKnowledge.Known, value);
        }

        public static QualifiedValue<T> Estimated(T value)
        {
            return Present(ValueKnowledge.Estimated, value);
        }

        public static QualifiedValue<T> Inferred(T value)
        {
            return Present(ValueKnowledge.Inferred, value);
        }

        public bool Equals(QualifiedValue<T> other)
        {
            return Knowledge == other.Knowledge &&
                HasValue == other.HasValue &&
                (!HasValue || EqualityComparer<T>.Default.Equals(value, other.value));
        }

        public override bool Equals(object obj)
        {
            return obj is QualifiedValue<T> && Equals((QualifiedValue<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Knowledge * 397) ^
                    (HasValue ? EqualityComparer<T>.Default.GetHashCode(value) : 0);
            }
        }

        private static QualifiedValue<T> Present(
            ValueKnowledge knowledge,
            T value)
        {
            return new QualifiedValue<T>(knowledge, value, true);
        }
    }
}
