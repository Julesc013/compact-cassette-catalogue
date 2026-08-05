using C3.Domain.Catalogues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueSnapshot
    {
        public CatalogueSnapshot(
            DocumentSessionId sessionId,
            ContentVersion contentVersion,
            StateFingerprint fingerprint,
            IEnumerable<CatalogueEntityCount> entityCounts)
        {
            SessionId = sessionId ?? throw new ArgumentNullException(nameof(sessionId));
            ContentVersion = contentVersion ??
                throw new ArgumentNullException(nameof(contentVersion));
            Fingerprint = fingerprint ?? throw new ArgumentNullException(nameof(fingerprint));
            if (entityCounts == null)
            {
                throw new ArgumentNullException(nameof(entityCounts));
            }

            var copy = entityCounts.OrderBy(item => item == null ? -1 : (int)item.Kind).ToList();
            if (copy.Any(item => item == null))
            {
                throw new ArgumentException(
                    "Entity counts cannot contain null entries.",
                    nameof(entityCounts));
            }
            if (copy.GroupBy(item => item.Kind).Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "Each entity kind may have only one count.",
                    nameof(entityCounts));
            }

            var total = copy.Aggregate(0L, (sum, item) => checked(sum + item.Count));
            EntityCounts = new ReadOnlyCollection<CatalogueEntityCount>(copy);
            TotalEntities = total;
        }

        public DocumentSessionId SessionId { get; }
        public ContentVersion ContentVersion { get; }
        public StateFingerprint Fingerprint { get; }
        public ReadOnlyCollection<CatalogueEntityCount> EntityCounts { get; }
        public long TotalEntities { get; }
    }
}
