using Deliveryix.Commons.Application.Cache;
using Deliveryix.Commons.Domain.DomainObjects;
using Microsoft.Extensions.Options;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.Options;
using Modules.Identity.Application.AccessManagement.UseCases.GetPermissions;
using Modules.Identity.Domain.Identities.DomainEvents;

namespace Modules.Identity.Application.Identities.DomainEvents
{
    internal sealed class RoleUnassignedDomainEventHandler(
        ICacheService cacheService,
        IOptions<ProfileCacheOptions> cacheOptions,
        ISender sender) : INotificationHandler<RoleUnassignedDomainEvent>
    {
        public async Task ExecuteAsync(RoleUnassignedDomainEvent notification, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync(notification.IdentityId.ToString(), cancellationToken);

            var result = await sender.SendAsync(new GetIdentityPermissionsQuery(notification.IdentityId), cancellationToken);
            if (result.IsFailure)
            {
                throw new DeliveryixException(nameof(GetIdentityPermissionsQuery), result.Error);
            }

            await cacheService.SetAsync(
                notification.IdentityId.ToString(),
                result.Value,
                cacheOptions.Value.CacheExpirationTimeInMinutes,
                cancellationToken
                );
        }
    }
}