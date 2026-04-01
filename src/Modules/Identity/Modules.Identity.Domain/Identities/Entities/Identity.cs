using Deliveryix.Commons.Domain.DomainObjects;
using Modules.Identity.Domain.Identities.DomainEvents;
using Modules.Identity.Domain.Identities.Enums;
using Modules.Identity.Domain.Identities.Models;
using Modules.Identity.Domain.Identities.ValueObjects;

namespace Modules.Identity.Domain.Identities.Entities
{
    public sealed class Identity : Entity, IAggregateRoot
    {
        private readonly List<Role> _roles = [];

        private Identity(string identityProviderId, string document, string email, string phone)
        {
            IdentityProviderId = identityProviderId;
            Document = document;
            Email = email;
            Phone = phone;
            State = IdentityStatus.Active;
            Type = Document.IdentityType;
            Validate();
        }

        private Identity()
        { }

        public string IdentityProviderId { get; private set; } = null!;
        public LegalDocument Document { get; private set; } = null!;
        public IdentityState State { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public Phone Phone { get; private set; } = null!;
        public IdentityType Type { get; private set; }
        public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

        public static Identity Create(string identityProviderId, string document, string email, string phone)
        {
            var identity = new Identity(identityProviderId, document, email, phone);

            identity.AddDomainEvent(UserCreatedDomainEvent.Create(identity.Id));

            return identity;
        }

        protected override void Validate()
        {
        }
    }
}