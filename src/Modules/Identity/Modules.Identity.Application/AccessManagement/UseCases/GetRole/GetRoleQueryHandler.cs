using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.GetRole
{
    internal sealed class GetRoleQueryHandler(IRoleRepository roleRepository) : IQueryHandler<GetRoleQuery, GetRoleResponse>
    {
        public async Task<Result<GetRoleResponse>> ExecuteAsync(GetRoleQuery request, CancellationToken cancellationToken = default)
        {
            var roleAndPermissions = await roleRepository.GetByNameAsync(request.Name, cancellationToken);

            if (roleAndPermissions is null)
            {
                return Result.Failure<GetRoleResponse>(AccessManagementErrors.RoleNotFound(request.Name));
            }

            return roleAndPermissions;
        }
    }
}