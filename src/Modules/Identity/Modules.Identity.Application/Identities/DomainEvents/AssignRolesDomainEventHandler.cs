using Deliveryix.Commons.Domain.DomainObjects;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.AssignDefaultRoles;
using Modules.Identity.Application.AccessManagement.UseCases.AssignRole;
using Modules.Identity.Domain.Identities.DomainEvents;

namespace Modules.Identity.Application.Identities.DomainEvents
{
    internal sealed class AssignRolesDomainEventHandler(ISender sender) : INotificationHandler<IndividualAccountCreatedDomainEvent>
    {
        public async Task ExecuteAsync(IndividualAccountCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var result = await sender.SendAsync(new AssignDefaultRolesCommand(notification.AggregateId), cancellationToken);

            if (result.IsFailure)
            {
                throw new DeliveryixException(nameof(AssignRoleCommand), result.Error);
            }
        }
    }

}