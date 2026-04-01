using Deliveryix.Commons.Domain.DomainObjects;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record BusinessAccountCreatedDomainEvent : DomainEvent
    {
        public static BusinessAccountCreatedDomainEvent Create(Guid aggregateId, string document)
            => new(aggregateId, document);

        private BusinessAccountCreatedDomainEvent(Guid aggregateId, string document)
            : base(aggregateId, nameof(BusinessAccountCreatedDomainEvent), ModuleExtensions.ModuleName)
        {
            Document = document;
        }

        private BusinessAccountCreatedDomainEvent()
        { }

        public string Document { get; set; } = null!;
    }
}