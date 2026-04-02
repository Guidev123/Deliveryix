using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.GrantPermission
{
    internal sealed class GrantPermissionCommandHandler(IRoleRepository roleRepository) : ICommandHandler<GrantPermissionCommand>
    {
        public async Task<Result> ExecuteAsync(GrantPermissionCommand request, CancellationToken cancellationToken = default)
        {
            var roleExists = await roleRepository.RoleExistsAsync(request.RoleName, cancellationToken);
            if (!roleExists)
            {
                return Result.Failure(AccessManagementErrors.RoleNotFound(request.RoleName));
            }

            await roleRepository.GrantPermissionAsync(request.RoleName, request.PermissionCode, cancellationToken);

            return Result.Success();
        }
    }
}