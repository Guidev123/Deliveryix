using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.RevokePermission
{
    internal sealed class RevokePermissionCommandHandler(IRoleRepository roleRepository) : ICommandHandler<RevokePermissionCommand>
    {
        public async Task<Result> ExecuteAsync(RevokePermissionCommand request, CancellationToken cancellationToken = default)
        {
            var roleExists = await roleRepository.RoleExistsAsync(request.RoleName, cancellationToken);
            if (!roleExists)
            {
                return Result.Failure(AccessManagementErrors.RoleNotFound(request.RoleName));
            }

            await roleRepository.RevokePermissionAsync(request.RoleName, request.PermissionCode, cancellationToken);

            return Result.Success();
        }
    }
}