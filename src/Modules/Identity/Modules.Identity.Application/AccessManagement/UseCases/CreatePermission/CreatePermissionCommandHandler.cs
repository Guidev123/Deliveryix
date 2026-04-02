using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;

namespace Modules.Identity.Application.AccessManagement.UseCases.CreatePermission
{
    internal sealed class CreatePermissionCommandHandler(IRoleRepository roleRepository) : ICommandHandler<CreatePermissionCommand>
    {
        public async Task<Result> ExecuteAsync(CreatePermissionCommand request, CancellationToken cancellationToken = default)
        {
            await roleRepository.AddPermissionAsync(request.Code, cancellationToken);

            return Result.Success();
        }
    }
}