using System;

namespace C3.Domain.Identity
{
    public sealed class RandomEntityIdGenerator : IEntityIdGenerator
    {
        public EntityId<TAggregate> Next<TAggregate>()
        {
            return new EntityId<TAggregate>(Guid.NewGuid());
        }
    }
}
