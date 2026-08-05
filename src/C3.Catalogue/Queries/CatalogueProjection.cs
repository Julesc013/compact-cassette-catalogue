using C3.Domain.Catalogues;
using C3.Domain.Values;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Catalogue.Queries
{
    public sealed class CatalogueProjection<T>
    {
        public CatalogueProjection(
            DocumentSessionId sessionId,
            ContentVersion contentVersion,
            IEnumerable<T> items,
            Optional<CatalogueCursor> nextCursor)
        {
            SessionId = sessionId ?? throw new ArgumentNullException(nameof(sessionId));
            ContentVersion = contentVersion ??
                throw new ArgumentNullException(nameof(contentVersion));
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var copy = items.ToList();
            if (copy.Any(item => ReferenceEquals(item, null)))
            {
                throw new ArgumentException(
                    "Projection items cannot contain null entries.",
                    nameof(items));
            }

            Items = new ReadOnlyCollection<T>(copy);
            NextCursor = nextCursor;
            if (nextCursor.HasValue &&
                (!sessionId.Equals(nextCursor.Value.SessionId) ||
                 !contentVersion.Equals(nextCursor.Value.ContentVersion)))
            {
                throw new ArgumentException(
                    "A continuation cursor must belong to the same document version.",
                    nameof(nextCursor));
            }
        }

        public DocumentSessionId SessionId { get; }
        public ContentVersion ContentVersion { get; }
        public ReadOnlyCollection<T> Items { get; }
        public Optional<CatalogueCursor> NextCursor { get; }
    }
}
