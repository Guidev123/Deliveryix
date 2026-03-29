using Deliveryix.Commons.Domain.DomainObjects;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record UserRegisteredInProviderDomainEvent : IDomainEvent
    {
        public UserRegisteredInProviderDomainEvent(Guid aggregateId, string email, string document, string phone)
        {
            CorrelationId = Guid.NewGuid();
            OccurredOn = DateTimeOffset.UtcNow;
            Messagetype = GetType().Name;
            AggregateId = aggregateId;
            Email = email;
            Document = document;
            Phone = phone;
        }

        public Guid AggregateId { get; }
        public Guid CorrelationId { get; }
        public DateTimeOffset OccurredOn { get; }
        public string Messagetype { get; }
        public string Email { get; }
        public string Document { get; }
        public string Phone { get; }
    }
}