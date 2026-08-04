namespace C3.Domain.Identity
{
    public interface IEntityIdGenerator
    {
        EntityId<TAggregate> Next<TAggregate>();
    }
}
