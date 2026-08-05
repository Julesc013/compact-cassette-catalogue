using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Catalogue.Queries
{
    public enum CatalogueFilterOperator
    {
        Equals = 0,
        Contains = 1,
        StartsWith = 2,
        IsKnown = 3,
        IsUnknown = 4,
        Range = 5,
        And = 6,
        Or = 7,
        Not = 8
    }

    public sealed class CatalogueFilter
    {
        private CatalogueFilter(
            CatalogueFilterOperator operation,
            CatalogueFieldId field,
            string firstValue,
            string secondValue,
            IEnumerable<CatalogueFilter> children)
        {
            Operation = operation;
            Field = field;
            FirstValue = firstValue ?? string.Empty;
            SecondValue = secondValue ?? string.Empty;
            Children = new ReadOnlyCollection<CatalogueFilter>(children.ToList());
        }

        public CatalogueFilterOperator Operation { get; }
        public CatalogueFieldId Field { get; }
        public string FirstValue { get; }
        public string SecondValue { get; }
        public ReadOnlyCollection<CatalogueFilter> Children { get; }

        public static CatalogueFilter Value(
            CatalogueFilterOperator operation,
            CatalogueFieldId field,
            string value)
        {
            if (operation != CatalogueFilterOperator.Equals &&
                operation != CatalogueFilterOperator.Contains &&
                operation != CatalogueFilterOperator.StartsWith)
            {
                throw new ArgumentOutOfRangeException(nameof(operation));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Leaf(operation, field, value, string.Empty);
        }

        public static CatalogueFilter Knowledge(
            CatalogueFilterOperator operation,
            CatalogueFieldId field)
        {
            if (operation != CatalogueFilterOperator.IsKnown &&
                operation != CatalogueFilterOperator.IsUnknown)
            {
                throw new ArgumentOutOfRangeException(nameof(operation));
            }

            return Leaf(operation, field, string.Empty, string.Empty);
        }

        public static CatalogueFilter Range(
            CatalogueFieldId field,
            string minimum,
            string maximum)
        {
            if (minimum == null)
            {
                throw new ArgumentNullException(nameof(minimum));
            }
            if (maximum == null)
            {
                throw new ArgumentNullException(nameof(maximum));
            }

            return Leaf(CatalogueFilterOperator.Range, field, minimum, maximum);
        }

        public static CatalogueFilter All(IEnumerable<CatalogueFilter> children)
        {
            return Group(CatalogueFilterOperator.And, children);
        }

        public static CatalogueFilter Any(IEnumerable<CatalogueFilter> children)
        {
            return Group(CatalogueFilterOperator.Or, children);
        }

        public static CatalogueFilter Not(CatalogueFilter child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            return new CatalogueFilter(
                CatalogueFilterOperator.Not,
                null,
                string.Empty,
                string.Empty,
                new[] { child });
        }

        private static CatalogueFilter Leaf(
            CatalogueFilterOperator operation,
            CatalogueFieldId field,
            string firstValue,
            string secondValue)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            return new CatalogueFilter(
                operation,
                field,
                firstValue,
                secondValue,
                new CatalogueFilter[0]);
        }

        private static CatalogueFilter Group(
            CatalogueFilterOperator operation,
            IEnumerable<CatalogueFilter> children)
        {
            if (children == null)
            {
                throw new ArgumentNullException(nameof(children));
            }

            var copy = children.ToList();
            if (copy.Count < 2 || copy.Any(child => child == null))
            {
                throw new ArgumentException(
                    "A filter group requires at least two non-null children.",
                    nameof(children));
            }

            return new CatalogueFilter(
                operation,
                null,
                string.Empty,
                string.Empty,
                copy);
        }
    }
}
