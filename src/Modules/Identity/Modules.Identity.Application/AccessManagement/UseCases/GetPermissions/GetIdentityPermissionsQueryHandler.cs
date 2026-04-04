using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.GetPermissions
{
    internal sealed class GetIdentityPermissionsQueryHandler(IRoleRepository roleRepository) : IQueryHandler<GetIdentityPermissionsQuery, GetIdentityPermissionsResponse>
    {
        public async Task<Result<GetIdentityPermissionsResponse>> ExecuteAsync(GetIdentityPermissionsQuery request, CancellationToken cancellationToken = default)
        {
            var permissions = await roleRepository.GetIdentityPermissionsAsync(request.IdentityProviderId, cancellationToken);

            if (permissions is null)
            {
                return Result.Failure<GetIdentityPermissionsResponse>(AccessManagementErrors.PermissionsNotFoundForIdentity(request.IdentityProviderId));
            }

            return permissions;
        }
    }
}