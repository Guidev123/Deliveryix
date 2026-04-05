using Deliveryix.Commons.Application.Cache;
using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Microsoft.Extensions.Options;
using Modules.Identity.Application.AccessManagement.Options;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.GetPermissions
{
    internal sealed class GetIdentityPermissionsQueryHandler(
        IRoleRepository roleRepository,
        IOptions<ProfileCacheOptions> options,
        ICacheService cacheService) : IQueryHandler<GetIdentityPermissionsQuery, GetIdentityPermissionsResponse>
    {
        public async Task<Result<GetIdentityPermissionsResponse>> ExecuteAsync(GetIdentityPermissionsQuery request, CancellationToken cancellationToken = default)
        {
            var cachedResponse = await cacheService.GetAsync<GetIdentityPermissionsResponse>(request.IdentityProviderId.ToString(), cancellationToken);
            if (cachedResponse is not null)
            {
                return cachedResponse;
            }

            var permissions = await roleRepository.GetIdentityPermissionsAsync(request.IdentityProviderId, cancellationToken);

            if (permissions is null)
            {
                return Result.Failure<GetIdentityPermissionsResponse>(AccessManagementErrors.PermissionsNotFoundForIdentity(request.IdentityProviderId));
            }

            await cacheService.SetAsync(
                request.IdentityProviderId.ToString(),
                permissions,
                options.Value.CacheExpirationTimeInMinutes,
                cancellationToken
                );

            return permissions;
        }
    }
}