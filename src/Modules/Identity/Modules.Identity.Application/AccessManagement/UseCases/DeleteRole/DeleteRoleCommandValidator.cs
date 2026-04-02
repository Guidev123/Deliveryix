using FluentValidation;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.DeleteRole
{
    internal sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidRoleName.Code)
                    .WithMessage(AccessManagementErrors.InvalidRoleName.Description);
        }
    }
}