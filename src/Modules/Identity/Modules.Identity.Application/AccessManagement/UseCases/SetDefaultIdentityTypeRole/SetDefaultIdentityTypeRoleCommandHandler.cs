using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;

namespace Modules.Identity.Application.AccessManagement.UseCases.SetDefaultIdentityTypeRole
{
    internal sealed class SetDefaultIdentityTypeRoleCommandHandler(IRoleRepository roleRepository) : ICommandHandler<SetDefaultIdentityTypeRoleCommand>
    {
        public async Task<Result> ExecuteAsync(SetDefaultIdentityTypeRoleCommand request, CancellationToken cancellationToken = default)
        {
            await roleRepository.AddDefaultRoleForIdentityTypeAsync(request.RoleName, request.IdentityType, cancellationToken);

            return Result.Success();
        }
    }
}