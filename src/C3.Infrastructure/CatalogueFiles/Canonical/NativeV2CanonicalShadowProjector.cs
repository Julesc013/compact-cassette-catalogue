using C3.Catalogue.Canonical;
using C3.Catalogue.Native;
using C3.Domain.Catalogues;
using C3.Domain.Profiles;
using System;
using System.Collections.Generic;

namespace C3.Infrastructure.CatalogueFiles.Canonical
{
    public sealed class NativeV2CanonicalShadowProjector
    {
        public CanonicalShadowProjection Project(
            NativeCatalogue source,
            DocumentSessionId sessionId,
            ContentVersion contentVersion,
            CatalogueResourceBudget budget)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (sessionId == null) throw new ArgumentNullException(nameof(sessionId));
            if (contentVersion == null) throw new ArgumentNullException(nameof(contentVersion));
            if (budget == null) throw new ArgumentNullException(nameof(budget));

            var state = new NativeV2ToCanonicalAdapter().Adapt(source);
            if (state.TotalEntities > budget.MaximumEntities)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(source),
                    "The native catalogue exceeds the canonical entity budget.");
            }
            var fingerprints = new CatalogueStateFingerprintProjector().Project(state);
            var snapshot = new CatalogueSnapshot(
                sessionId,
                contentVersion,
                fingerprints.Root,
                Counts(state));
            new CatalogueDocument(snapshot, budget);
            return new CanonicalShadowProjection(
                KnownCatalogueProfiles.NativeV2_0,
                state,
                snapshot,
                fingerprints);
        }

        private static IEnumerable<CatalogueEntityCount> Counts(
            CatalogueState state)
        {
            return new[]
            {
                new CatalogueEntityCount(
                    CatalogueEntityKind.Brand, state.Brands.Count),
                new CatalogueEntityCount(
                    CatalogueEntityKind.CassetteModel,
                    state.CassetteModels.Count),
                new CatalogueEntityCount(
                    CatalogueEntityKind.DeckModel, state.DeckModels.Count),
                new CatalogueEntityCount(
                    CatalogueEntityKind.DeckUnit, state.DeckUnits.Count),
                new CatalogueEntityCount(
                    CatalogueEntityKind.Tape, state.Tapes.Count),
                new CatalogueEntityCount(
                    CatalogueEntityKind.Recording, state.Recordings.Count)
            };
        }
    }
}
