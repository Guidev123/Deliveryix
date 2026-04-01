using Deliveryix.Commons.Domain.DomainObjects;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record UserRegisteredInProviderDomainEvent : DomainEvent
    {
        public static UserRegisteredInProviderDomainEvent Create(
             Guid aggregateId,
             string email,
             string document,
             string phone
            ) => new(aggregateId, email, document, phone);

        private UserRegisteredInProviderDomainEvent(
            Guid aggregateId, string email, string document, string phone)
            : base(aggregateId, nameof(UserRegisteredInProviderDomainEvent))
        {
            Email = email;
            Document = document;
            Phone = phone;
        }

        private UserRegisteredInProviderDomainEvent()
        { }

        public string Email { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}