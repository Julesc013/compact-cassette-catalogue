using C3.Catalogue.Canonical;
using C3.Domain.Profiles;
using System;

namespace C3.Infrastructure.CatalogueFiles.Canonical
{
    public sealed class CanonicalShadowProjection
    {
        public CanonicalShadowProjection(
            CatalogueProfileCapabilities sourceProfile,
            CatalogueState state,
            CatalogueSnapshot snapshot,
            CatalogueFingerprintIndex fingerprints)
        {
            SourceProfile = sourceProfile ??
                throw new ArgumentNullException(nameof(sourceProfile));
            State = state ?? throw new ArgumentNullException(nameof(state));
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            Fingerprints = fingerprints ??
                throw new ArgumentNullException(nameof(fingerprints));
            if (!snapshot.Fingerprint.Equals(fingerprints.Root))
            {
                throw new ArgumentException(
                    "A shadow snapshot must carry its fingerprint-index root.",
                    nameof(snapshot));
            }
            if (fingerprints.Entries.Count != snapshot.TotalEntities + 1)
            {
                throw new ArgumentException(
                    "A shadow fingerprint index must cover metadata and every entity.",
                    nameof(fingerprints));
            }
            if (state.TotalEntities != snapshot.TotalEntities)
            {
                throw new ArgumentException(
                    "A shadow state and snapshot must cover the same entities.",
                    nameof(state));
            }
        }

        public CatalogueProfileCapabilities SourceProfile { get; }
        public CatalogueState State { get; }
        public CatalogueSnapshot Snapshot { get; }
        public CatalogueFingerprintIndex Fingerprints { get; }
    }
}
