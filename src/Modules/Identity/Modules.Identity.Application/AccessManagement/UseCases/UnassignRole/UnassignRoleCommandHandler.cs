using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;

namespace Modules.Identity.Application.AccessManagement.UseCases.UnassignRole
{
    internal sealed class UnassignRoleCommandHandler(IRoleRepository roleRepository) : ICommandHandler<UnassignRoleCommand>
    {
        public async Task<Result> ExecuteAsync(UnassignRoleCommand request, CancellationToken cancellationToken = default)
        {
            await roleRepository.UnassignFromIdentityAsync(request.RoleName, request.IdentityId, cancellationToken);

            return Result.Success();
        }
    }
}