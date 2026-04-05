using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Domain.Identities.DomainEvents;
using Modules.Identity.Domain.Identities.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.UnassignRole
{
    internal sealed class UnassignRoleCommandHandler(
        IRoleRepository roleRepository,
        IIdentityRepository identityRepository,
        IDomainEventCollector domainEventCollector) : ICommandHandler<UnassignRoleCommand>
    {
        public async Task<Result> ExecuteAsync(UnassignRoleCommand request, CancellationToken cancellationToken = default)
        {
            var identity = await identityRepository.GetByIdAsync(request.IdentityId, cancellationToken);
            if (identity is null)
            {
                return Result.Failure(IdentityErrors.IdentityNotFound(request.IdentityId));
            }

            await roleRepository.UnassignFromIdentityAsync(request.RoleName, request.IdentityId, cancellationToken);

            identity.AddDomainEvent(RoleUnassignedDomainEvent.Create(identity.Id, request.RoleName));

            domainEventCollector.Collect(identity);

            return Result.Success();
        }
    }
}