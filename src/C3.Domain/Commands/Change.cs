using System;

namespace C3.Domain.Commands
{
    public enum ChangeKind
    {
        Created = 1,
        Updated = 2,
        Deleted = 3
    }

    public sealed class Change
    {
        public Change(string entityType, string entityId, ChangeKind kind)
        {
            if (string.IsNullOrWhiteSpace(entityType))
            {
                throw new ArgumentException("An entity type is required.", nameof(entityType));
            }

            if (string.IsNullOrWhiteSpace(entityId))
            {
                throw new ArgumentException("An entity identifier is required.", nameof(entityId));
            }

            if (!Enum.IsDefined(typeof(ChangeKind), kind))
            {
                throw new ArgumentOutOfRangeException(nameof(kind));
            }

            EntityType = entityType.Trim();
            EntityId = entityId.Trim();
            Kind = kind;
        }

        public string EntityType { get; }

        public string EntityId { get; }

        public ChangeKind Kind { get; }
    }
}
