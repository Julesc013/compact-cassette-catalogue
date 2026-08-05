using C3.Domain.Catalogues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueTransaction
    {
        public CatalogueTransaction(
            ContentVersion expectedVersion,
            IEnumerable<CatalogueMutationIntent> intents,
            CatalogueResourceBudget budget)
        {
            ExpectedVersion = expectedVersion ??
                throw new ArgumentNullException(nameof(expectedVersion));
            if (intents == null)
            {
                throw new ArgumentNullException(nameof(intents));
            }
            if (budget == null)
            {
                throw new ArgumentNullException(nameof(budget));
            }

            var copy = intents.ToList();
            if (copy.Count == 0 || copy.Any(item => item == null))
            {
                throw new ArgumentException(
                    "A transaction requires one or more non-null intents.",
                    nameof(intents));
            }
            if (copy.Count > budget.MaximumTransactionOperations)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(intents),
                    "The transaction exceeds its operation budget.");
            }

            Intents = new ReadOnlyCollection<CatalogueMutationIntent>(copy);
        }

        public ContentVersion ExpectedVersion { get; }
        public ReadOnlyCollection<CatalogueMutationIntent> Intents { get; }
    }
}
