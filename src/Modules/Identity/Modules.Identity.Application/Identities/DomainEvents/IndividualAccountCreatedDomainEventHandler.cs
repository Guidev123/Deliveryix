using Deliveryix.Commons.Application.EventBus;
using Deliveryix.Commons.Domain.DomainObjects;
using MidR.Interfaces;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Domain.Identities.DomainEvents;
using Modules.Identity.Domain.Identities.Errors;
using Modules.Identity.IntegrationEvents;

namespace Modules.Identity.Application.Identities.DomainEvents
{
    internal sealed class IndividualAccountCreatedDomainEventHandler(
        IEventBus eventBus,
        IIdentityRepository identityRepository
        ) : INotificationHandler<IndividualAccountCreatedDomainEvent>
    {
        public async Task ExecuteAsync(IndividualAccountCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var identity = await identityRepository.GetByIdAsync(notification.AggregateId, cancellationToken);

            if (identity is null)
            {
                throw new DeliveryixException(nameof(identityRepository.GetByIdAsync), IdentityErrors.IdentityNotFound(notification.AggregateId));
            }

            var @event = IndividualAccountCreatedIntegrationEvent.Create(
                identity.Id,
                identity.Email.Address,
                identity.Document.Number,
                identity.Phone.Number);

            await eventBus.PublishAsync(
                IndividualAccountCreatedIntegrationEvent.Topic,
                IntegrationEnvelope.FromEvent(@event),
                cancellationToken);
        }
    }
}