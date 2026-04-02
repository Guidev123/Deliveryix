using MidR.Interfaces;
using Modules.Identity.Domain.Identities.DomainEvents;

namespace Modules.Identity.Application.Identities.DomainEvents
{
    internal sealed class AssignIndividualAccountRolesDomainEventHandler : INotificationHandler<IndividualAccountCreatedDomainEvent>
    {
        public Task ExecuteAsync(IndividualAccountCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class AssignBusinessAccountRolesDomainEventHandler : INotificationHandler<BusinessAccountCreatedDomainEvent>
    {
        public Task ExecuteAsync(BusinessAccountCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}