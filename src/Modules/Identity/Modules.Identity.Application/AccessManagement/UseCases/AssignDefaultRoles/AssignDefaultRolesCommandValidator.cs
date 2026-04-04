using FluentValidation;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.AssignDefaultRoles
{
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
}