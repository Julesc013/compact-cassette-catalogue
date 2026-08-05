using C3.Domain.Catalogues;
using C3.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueChangeSet
    {
        public CatalogueChangeSet(
            ContentVersion versionBefore,
            ContentVersion versionAfter,
            StateFingerprint fingerprintBefore,
            StateFingerprint fingerprintAfter,
            IEnumerable<Change> changes)
        {
            VersionBefore = versionBefore ??
                throw new ArgumentNullException(nameof(versionBefore));
            VersionAfter = versionAfter ??
                throw new ArgumentNullException(nameof(versionAfter));
            FingerprintBefore = fingerprintBefore ??
                throw new ArgumentNullException(nameof(fingerprintBefore));
            FingerprintAfter = fingerprintAfter ??
                throw new ArgumentNullException(nameof(fingerprintAfter));
            if (versionBefore.Value == long.MaxValue ||
                versionAfter.Value != versionBefore.Value + 1)
            {
                throw new ArgumentException(
                    "A committed change set must advance the content version exactly once.",
                    nameof(versionAfter));
            }
            if (fingerprintBefore.Equals(fingerprintAfter))
            {
                throw new ArgumentException(
                    "A committed change set must change semantic state.",
                    nameof(fingerprintAfter));
            }
            if (changes == null)
            {
                throw new ArgumentNullException(nameof(changes));
            }

            var copy = changes.ToList();
            if (copy.Count == 0 || copy.Any(change => change == null))
            {
                throw new ArgumentException(
                    "A committed change set requires non-null changes.",
                    nameof(changes));
            }

            Changes = new ReadOnlyCollection<Change>(copy);
        }

        public ContentVersion VersionBefore { get; }
        public ContentVersion VersionAfter { get; }
        public StateFingerprint FingerprintBefore { get; }
        public StateFingerprint FingerprintAfter { get; }
        public ReadOnlyCollection<Change> Changes { get; }
    }
}
