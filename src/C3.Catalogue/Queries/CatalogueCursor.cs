using C3.Domain.Catalogues;
using System;

namespace C3.Catalogue.Queries
{
    public sealed class CatalogueCursor
    {
        public CatalogueCursor(
            DocumentSessionId sessionId,
            ContentVersion contentVersion,
            StateFingerprint queryFingerprint,
            string lastSortKey,
            string lastEntityId)
        {
            SessionId = sessionId ?? throw new ArgumentNullException(nameof(sessionId));
            ContentVersion = contentVersion ??
                throw new ArgumentNullException(nameof(contentVersion));
            QueryFingerprint = queryFingerprint ??
                throw new ArgumentNullException(nameof(queryFingerprint));
            if (lastSortKey == null)
            {
                throw new ArgumentNullException(nameof(lastSortKey));
            }
            if (string.IsNullOrWhiteSpace(lastEntityId))
            {
                throw new ArgumentException(
                    "A cursor requires its stable entity-ID tie-breaker.",
                    nameof(lastEntityId));
            }

            LastSortKey = lastSortKey;
            LastEntityId = lastEntityId.Trim();
        }

        public DocumentSessionId SessionId { get; }
        public ContentVersion ContentVersion { get; }
        public StateFingerprint QueryFingerprint { get; }
        public string LastSortKey { get; }
        public string LastEntityId { get; }
    }
}
