using C3.Catalogue.Canonical;
using C3.Domain.Catalogues;
using C3.Domain.Values;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Catalogue.Queries
{
    public sealed class CatalogueQuery
    {
        public CatalogueQuery(
            CatalogueEntityKind entityKind,
            Optional<CatalogueFilter> filter,
            IEnumerable<CatalogueSort> sort,
            int pageSize,
            StateFingerprint queryFingerprint,
            CatalogueResourceBudget budget)
        {
            if (!Enum.IsDefined(typeof(CatalogueEntityKind), entityKind))
            {
                throw new ArgumentOutOfRangeException(nameof(entityKind));
            }
            if (sort == null)
            {
                throw new ArgumentNullException(nameof(sort));
            }
            if (budget == null)
            {
                throw new ArgumentNullException(nameof(budget));
            }
            if (pageSize <= 0 || pageSize > budget.MaximumPageSize)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            var sortCopy = sort.ToList();
            if (sortCopy.Count == 0 || sortCopy.Any(item => item == null))
            {
                throw new ArgumentException(
                    "A query requires one or more non-null sort definitions.",
                    nameof(sort));
            }
            if (sortCopy.GroupBy(item => item.Field).Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "A query cannot sort the same field more than once.",
                    nameof(sort));
            }
            if (filter.HasValue)
            {
                var measurements = Measure(filter.Value);
                if (measurements.Item1 > budget.MaximumQueryDepth ||
                    measurements.Item2 > budget.MaximumQueryTerms)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(filter),
                        "The filter exceeds its depth or term budget.");
                }
            }

            EntityKind = entityKind;
            Filter = filter;
            Sort = new ReadOnlyCollection<CatalogueSort>(sortCopy);
            PageSize = pageSize;
            QueryFingerprint = queryFingerprint ??
                throw new ArgumentNullException(nameof(queryFingerprint));
        }

        public CatalogueEntityKind EntityKind { get; }
        public Optional<CatalogueFilter> Filter { get; }
        public ReadOnlyCollection<CatalogueSort> Sort { get; }
        public int PageSize { get; }
        public StateFingerprint QueryFingerprint { get; }

        private static Tuple<int, int> Measure(CatalogueFilter filter)
        {
            if (filter.Children.Count == 0)
            {
                return Tuple.Create(1, 1);
            }

            var childMeasurements = filter.Children.Select(Measure).ToList();
            return Tuple.Create(
                1 + childMeasurements.Max(item => item.Item1),
                childMeasurements.Sum(item => item.Item2));
        }
    }
}
