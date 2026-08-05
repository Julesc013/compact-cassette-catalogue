using System;

namespace C3.Catalogue.Canonical
{
    public sealed class CatalogueMutationIntent
    {
        public CatalogueMutationIntent(
            string operationCode,
            CatalogueEntityKind entityKind,
            string entityId)
        {
            if (string.IsNullOrWhiteSpace(operationCode))
            {
                throw new ArgumentException(
                    "A stable operation code is required.",
                    nameof(operationCode));
            }
            if (!Enum.IsDefined(typeof(CatalogueEntityKind), entityKind))
            {
                throw new ArgumentOutOfRangeException(nameof(entityKind));
            }

            OperationCode = operationCode.Trim();
            EntityKind = entityKind;
            EntityId = (entityId ?? string.Empty).Trim();
        }

        public string OperationCode { get; }
        public CatalogueEntityKind EntityKind { get; }
        public string EntityId { get; }
    }
}
