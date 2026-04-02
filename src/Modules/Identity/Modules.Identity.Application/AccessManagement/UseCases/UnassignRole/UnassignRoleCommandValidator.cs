using FluentValidation;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.UnassignRole
{
    internal sealed class UnassignRoleCommandValidator : AbstractValidator<UnassignRoleCommand>
    {
        public UnassignRoleCommandValidator()
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