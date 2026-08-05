using System;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueResourceBudget
    {
        public CatalogueResourceBudget(
            int maximumEntities,
            int maximumTransactionOperations,
            int maximumQueryDepth,
            int maximumQueryTerms,
            int maximumPageSize,
            int maximumOpenSnapshots)
        {
            MaximumEntities = Positive(maximumEntities, nameof(maximumEntities));
            MaximumTransactionOperations = Positive(
                maximumTransactionOperations,
                nameof(maximumTransactionOperations));
            MaximumQueryDepth = Positive(maximumQueryDepth, nameof(maximumQueryDepth));
            MaximumQueryTerms = Positive(maximumQueryTerms, nameof(maximumQueryTerms));
            MaximumPageSize = Positive(maximumPageSize, nameof(maximumPageSize));
            MaximumOpenSnapshots = Positive(
                maximumOpenSnapshots,
                nameof(maximumOpenSnapshots));
        }

        public int MaximumEntities { get; }
        public int MaximumTransactionOperations { get; }
        public int MaximumQueryDepth { get; }
        public int MaximumQueryTerms { get; }
        public int MaximumPageSize { get; }
        public int MaximumOpenSnapshots { get; }

        private static int Positive(int value, string parameterName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }

            return value;
        }
    }
}
