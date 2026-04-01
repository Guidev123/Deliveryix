namespace Deliveryix.Commons.Domain.DomainObjects
{
    internal interface IDomainEvent : IEvent
    {
        Guid AggregateId { get; }
    }
}