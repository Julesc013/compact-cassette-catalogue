using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueFingerprintDelta
    {
        public CatalogueFingerprintDelta(
            IEnumerable<CatalogueFingerprintEntry> upserts,
            IEnumerable<CatalogueEntityKey> removals)
        {
            if (upserts == null)
            {
                throw new ArgumentNullException(nameof(upserts));
            }
            if (removals == null)
            {
                throw new ArgumentNullException(nameof(removals));
            }

            var upsertCopy = upserts.ToList();
            var removalCopy = removals.ToList();
            if (upsertCopy.Any(item => item == null) ||
                removalCopy.Any(item => item == null))
            {
                throw new ArgumentException(
                    "Fingerprint deltas cannot contain null entries.");
            }
            if (upsertCopy.GroupBy(item => item.Key).Any(group => group.Count() > 1) ||
                removalCopy.GroupBy(item => item).Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "Fingerprint deltas cannot repeat an entity key.");
            }

            var removalsSet = new HashSet<CatalogueEntityKey>(removalCopy);
            if (upsertCopy.Any(item => removalsSet.Contains(item.Key)))
            {
                throw new ArgumentException(
                    "A fingerprint delta cannot upsert and remove the same entity.");
            }
            if (upsertCopy.Count == 0 && removalCopy.Count == 0)
            {
                throw new ArgumentException(
                    "A fingerprint delta requires an upsert or removal.");
            }

            Upserts = new ReadOnlyCollection<CatalogueFingerprintEntry>(
                upsertCopy.OrderBy(item => item.Key).ToList());
            Removals = new ReadOnlyCollection<CatalogueEntityKey>(
                removalCopy.OrderBy(item => item).ToList());
        }

        public ReadOnlyCollection<CatalogueFingerprintEntry> Upserts { get; }
        public ReadOnlyCollection<CatalogueEntityKey> Removals { get; }
    }
}
