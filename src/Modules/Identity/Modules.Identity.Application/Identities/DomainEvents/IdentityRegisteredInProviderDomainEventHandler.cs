using Deliveryix.Commons.Domain.DomainObjects;
using MidR.Interfaces;
using Modules.Identity.Application.Identities.Create;
using Modules.Identity.Domain.Identities.DomainEvents;

namespace Modules.Identity.Application.Identities.DomainEvents
{
    internal sealed class IdentityRegisteredInProviderDomainEventHandler(ISender sender) : INotificationHandler<IdentityRegisteredInProviderDomainEvent>
    {
        public async Task ExecuteAsync(IdentityRegisteredInProviderDomainEvent notification, CancellationToken cancellationToken)
        {
            var result = await sender.SendAsync(new CreateIdentityCommand(
                notification.Email,
                notification.Document,
                notification.Phone
                ), cancellationToken);

            if (result.IsFailure)
            {
                throw new DeliveryixException(nameof(CreateIdentityCommand), result.Error);
            }
        }
    }
}