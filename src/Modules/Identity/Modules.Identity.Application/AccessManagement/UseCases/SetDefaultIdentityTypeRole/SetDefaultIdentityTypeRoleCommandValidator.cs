using FluentValidation;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.SetDefaultIdentityTypeRole
{
    internal sealed class SetDefaultIdentityTypeRoleCommandValidator : AbstractValidator<SetDefaultIdentityTypeRoleCommand>
    {
        public SetDefaultIdentityTypeRoleCommandValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidRoleName.Code)
                    .WithMessage(AccessManagementErrors.InvalidRoleName.Description);
        }
    }
}