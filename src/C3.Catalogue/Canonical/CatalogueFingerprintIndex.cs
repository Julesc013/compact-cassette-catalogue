using C3.Domain.Catalogues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueFingerprintIndex
    {
        public CatalogueFingerprintIndex(
            IEnumerable<CatalogueFingerprintEntry> entries,
            StateFingerprint root)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }
            Root = root ?? throw new ArgumentNullException(nameof(root));

            var copy = entries.OrderBy(entry => entry == null ? null : entry.Key).ToList();
            if (copy.Any(entry => entry == null))
            {
                throw new ArgumentException(
                    "A fingerprint index cannot contain null entries.",
                    nameof(entries));
            }
            if (copy.GroupBy(entry => entry.Key).Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "A fingerprint index cannot contain duplicate entity keys.",
                    nameof(entries));
            }

            Entries = new ReadOnlyCollection<CatalogueFingerprintEntry>(copy);
        }

        public ReadOnlyCollection<CatalogueFingerprintEntry> Entries { get; }
        public StateFingerprint Root { get; }
    }
}
