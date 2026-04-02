using FluentValidation;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.AssignRole
{
    internal sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
            RuleFor(x => x.IdentityId)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidIdentityId.Code)
                    .WithMessage(AccessManagementErrors.InvalidIdentityId.Description);

            RuleFor(x => x.RoleName)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidRoleName.Code)
                    .WithMessage(AccessManagementErrors.InvalidRoleName.Description);
        }
    }
}