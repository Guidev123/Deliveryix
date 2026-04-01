using Deliveryix.Commons.Domain.DomainObjects;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record UserCreatedDomainEvent : DomainEvent
    {
        public static UserCreatedDomainEvent Create(Guid aggregateId)
            => new(aggregateId);

        private UserCreatedDomainEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
            CorrelationId = Guid.NewGuid();
            OccurredOn = DateTimeOffset.UtcNow;
            Messagetype = GetType().Name;
        }

        private UserCreatedDomainEvent()
        { }
    }
}