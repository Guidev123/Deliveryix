using Deliveryix.Commons.Domain.DomainObjects;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record UserCreatedDomainEvent : IDomainEvent
    {
        public UserCreatedDomainEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
            CorrelationId = Guid.NewGuid();
            OccurredOn = DateTimeOffset.UtcNow;
            Messagetype = GetType().Name;
        }

        public Guid AggregateId { get; }

        public Guid CorrelationId { get; }

        public DateTimeOffset OccurredOn { get; }

        public string Messagetype { get; }
    }
}