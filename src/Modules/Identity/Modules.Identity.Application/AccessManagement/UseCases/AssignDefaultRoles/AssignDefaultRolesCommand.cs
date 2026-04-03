using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using FluentValidation;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Domain.AcessManagement.Errors;
using Modules.Identity.Domain.Identities.DomainEvents;
using Modules.Identity.Domain.Identities.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Identity.Application.AccessManagement.UseCases.AssignDefaultRoles
{
    public sealed record AssignDefaultRolesCommand(Guid IdentityId) : ICommand;

    internal sealed class AssignDefaultRolesCommandValidator : AbstractValidator<AssignDefaultRolesCommand>
    {
        public AssignDefaultRolesCommandValidator()
        {
            RuleFor(x => x.IdentityId)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidIdentityId.Code)
                    .WithMessage(AccessManagementErrors.InvalidIdentityId.Description);
        }
    }

    internal sealed class AssignDefaultRolesCommandHandler(
        IIdentityRepository identityRepository,
        IRoleRepository roleRepository,
        IDomainEventCollector domainEventCollector
        ) : ICommandHandler<AssignDefaultRolesCommand>
    {
        public async Task<Result> ExecuteAsync(AssignDefaultRolesCommand request, CancellationToken cancellationToken = default)
        {
            var identity = await identityRepository.GetByIdAsync(request.IdentityId, cancellationToken);
            if (identity is null)
            {
                return Result.Failure(IdentityErrors.IdentityNotFound(request.IdentityId));
            }

            var availableRoles = await roleRepository.GetDefaultRolesByIdentityTypeAsync(identity.Type, cancellationToken);
            if (availableRoles.Count == 0)
            {
                return Result.Failure(AccessManagementErrors.InvalidRoleForIdentityType(identity.Type));
            }

            var tasks = availableRoles.Select(c =>
            {
                identity.AddDomainEvent(RoleAssignedDomainEvent.Create(request.IdentityId, c.Name));
                return roleRepository.AssignToIdentityAsync(c.Name, request.IdentityId);
            });

            await Task.WhenAll(tasks).WaitAsync(cancellationToken);

            domainEventCollector.Collect(identity);

            return Result.Success();
        }
    }
}