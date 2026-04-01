using Deliveryix.Commons.Domain.DomainObjects;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record IndividualAccountCreatedDomainEvent : DomainEvent
    {
        public static IndividualAccountCreatedDomainEvent Create(Guid aggregateId, string document)
            => new(aggregateId, document);

        private IndividualAccountCreatedDomainEvent(Guid aggregateId, string document)
            : base(aggregateId, nameof(IndividualAccountCreatedDomainEvent), ModuleExtensions.ModuleName)
        {
            Document = document;
        }

        private IndividualAccountCreatedDomainEvent()
        { }

        public string Document { get; set; } = null!;
    }
}