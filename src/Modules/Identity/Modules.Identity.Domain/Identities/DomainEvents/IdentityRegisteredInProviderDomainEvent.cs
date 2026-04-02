using Deliveryix.Commons.Domain.DomainObjects;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record IdentityRegisteredInProviderDomainEvent : DomainEvent
    {
        public static IdentityRegisteredInProviderDomainEvent Create(
             Guid aggregateId,
             string email,
             string document,
             string phone
            ) => new(aggregateId, email, document, phone);

        private IdentityRegisteredInProviderDomainEvent(
            Guid aggregateId, string email, string document, string phone)
            : base(aggregateId, nameof(IdentityRegisteredInProviderDomainEvent), ModuleExtensions.ModuleName)
        {
            Email = email;
            Document = document;
            Phone = phone;
        }

        private IdentityRegisteredInProviderDomainEvent()
        { }

        public string Email { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}