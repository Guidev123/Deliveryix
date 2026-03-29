namespace Deliveryix.Commons.Domain.DomainObjects
{
    public interface IDomainEvent : IEvent
    {
        Guid AggregateId { get; }
    }
}