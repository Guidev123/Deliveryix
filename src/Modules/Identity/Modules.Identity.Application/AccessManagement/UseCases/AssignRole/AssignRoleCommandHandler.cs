using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Domain.AcessManagement.Errors;
using Modules.Identity.Domain.Identities.DomainEvents;
using Modules.Identity.Domain.Identities.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.AssignRole
{
    internal sealed class AssignRoleCommandHandler(
        IIdentityRepository identityRepository,
        IRoleRepository roleRepository,
        IDomainEventCollector domainEventCollector
        ) : ICommandHandler<AssignRoleCommand>
    {
        public async Task<Result> ExecuteAsync(AssignRoleCommand request, CancellationToken cancellationToken = default)
        {
            var roleExists = await roleRepository.RoleExistsAsync(request.RoleName, cancellationToken);
            if (!roleExists)
            {
                return Result.Failure(AccessManagementErrors.RoleNotFound(request.RoleName));
            }

            var identity = await identityRepository.GetByIdAsync(request.IdentityId, cancellationToken);
            if (identity is null)
            {
                return Result.Failure(IdentityErrors.IdentityNotFound(request.IdentityId));
            }

            var availableRoles = await roleRepository.GetDefaultRolesByIdentityTypeAsync(identity.Type, cancellationToken);
            if (!availableRoles.Any(c =>
                string.Equals(c.Name, request.RoleName, StringComparison.OrdinalIgnoreCase)))
            {
                return Result.Failure(AccessManagementErrors.InvalidRoleForIdentityType(identity.Type));
            }

            await roleRepository.AssignToIdentityAsync(request.RoleName, request.IdentityId, cancellationToken);

            identity.AddDomainEvent(RoleAssignedDomainEvent.Create(request.IdentityId, request.RoleName));

            domainEventCollector.Collect(identity);

            return Result.Success();
        }
    }
}