using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;

namespace Modules.Identity.Application.AccessManagement.UseCases.CreateRole
{
    internal sealed class CreateRoleCommandHandler(IRoleRepository roleRepository) : ICommandHandler<CreateRoleCommand>
    {
        public async Task<Result> ExecuteAsync(CreateRoleCommand request, CancellationToken cancellationToken = default)
        {
            await roleRepository.AddAsync(new(request.Name), cancellationToken);

            return Result.Success();
        }
    }
}