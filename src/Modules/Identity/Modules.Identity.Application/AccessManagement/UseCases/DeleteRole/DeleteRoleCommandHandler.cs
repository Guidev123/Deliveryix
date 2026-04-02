using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;

namespace Modules.Identity.Application.AccessManagement.UseCases.DeleteRole
{
    internal sealed class DeleteRoleCommandHandler(IRoleRepository roleRepository) : ICommandHandler<DeleteRoleCommand>
    {
        public async Task<Result> ExecuteAsync(DeleteRoleCommand request, CancellationToken cancellationToken = default)
        {
            await roleRepository.DeleteAsync(request.RoleName, cancellationToken);

            return Result.Success();
        }
    }
}