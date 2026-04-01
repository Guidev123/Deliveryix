using Deliveryix.Commons.Domain.DomainObjects;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record UserCreatedDomainEvent : DomainEvent
    {
        public static UserCreatedDomainEvent Create(Guid aggregateId)
            => new(aggregateId);

        private UserCreatedDomainEvent(Guid aggregateId)
            : base(aggregateId, nameof(UserCreatedDomainEvent), ModuleExtensions.ModuleName)
        {
        }

        private UserCreatedDomainEvent()
        { }
    }
}