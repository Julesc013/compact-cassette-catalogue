using C3.Domain.Catalogues;
using System;
using System.Collections.Generic;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueDocument
    {
        public CatalogueDocument(
            CatalogueSnapshot initialSnapshot,
            CatalogueResourceBudget budget)
        {
            CurrentSnapshot = initialSnapshot ??
                throw new ArgumentNullException(nameof(initialSnapshot));
            Budget = budget ?? throw new ArgumentNullException(nameof(budget));
            if (initialSnapshot.TotalEntities > budget.MaximumEntities)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(initialSnapshot),
                    "The initial snapshot exceeds its entity budget.");
            }
        }

        public CatalogueSnapshot CurrentSnapshot { get; }
        public CatalogueResourceBudget Budget { get; }

        public CatalogueTransaction BeginTransaction(
            ContentVersion expectedVersion,
            IEnumerable<CatalogueMutationIntent> intents)
        {
            if (!CurrentSnapshot.ContentVersion.Equals(expectedVersion))
            {
                throw new InvalidOperationException(
                    "The transaction expected a stale content version.");
            }

            return new CatalogueTransaction(expectedVersion, intents, Budget);
        }
    }
}
