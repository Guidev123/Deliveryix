using Deliveryix.Commons.Application.EventBus;
using Deliveryix.Commons.Domain.DomainObjects;
using MidR.Interfaces;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Domain.Identities.DomainEvents;
using Modules.Identity.Domain.Identities.Errors;
using Modules.Identity.IntegrationEvents;

namespace Modules.Identity.Application.Identities.DomainEvents
{
    internal sealed class BusinessAccountCreatedDomainEventHandler(
        IEventBus eventBus,
        IIdentityRepository identityRepository
        ) : INotificationHandler<BusinessAccountCreatedDomainEvent>
    {
        public async Task ExecuteAsync(BusinessAccountCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
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

            await eventBus.PublishAsync(@event.Topic, IntegrationEnvelope.FromEvent(@event), cancellationToken);
        }
    }
}