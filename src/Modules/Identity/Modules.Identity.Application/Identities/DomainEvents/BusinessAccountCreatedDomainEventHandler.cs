using Deliveryix.Commons.Application.EventBus;
using Deliveryix.Commons.Domain.DomainObjects;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.AssignDefaultRoles;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Domain.Identities.DomainEvents;
using Modules.Identity.Domain.Identities.Errors;
using Modules.Identity.IntegrationEvents;

namespace Modules.Identity.Application.Identities.DomainEvents
{
    internal sealed class BusinessAccountCreatedDomainEventHandler(
        IEventBus eventBus,
        IIdentityRepository identityRepository,
        ISender sender
        ) : INotificationHandler<BusinessAccountCreatedDomainEvent>
    {
        public async Task ExecuteAsync(BusinessAccountCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var commandResult = await sender.SendAsync(new AssignDefaultRolesCommand(notification.AggregateId), cancellationToken);
            if (commandResult.IsFailure)
            {
                throw new DeliveryixException(nameof(AssignDefaultRolesCommand), commandResult.Error);
            }

            var identity = await identityRepository.GetByIdAsync(notification.AggregateId, cancellationToken);

            if (identity is null)
            {
                throw new DeliveryixException(nameof(identityRepository.GetByIdAsync), IdentityErrors.IdentityNotFound(notification.AggregateId));
            }

            var @event = BusinessAccountCreatedIntegrationEvent.Create(
                identity.Id,
                identity.Email.Address,
                identity.Document.Number,
                identity.Phone.Number);

            await eventBus.PublishAsync(
                BusinessAccountCreatedIntegrationEvent.Topic,
                IntegrationEnvelope.FromEvent(@event),
                cancellationToken);
        }
    }
}