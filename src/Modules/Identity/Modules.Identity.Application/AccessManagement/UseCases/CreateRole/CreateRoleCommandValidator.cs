using FluentValidation;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.CreateRole
{
    internal sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidRoleName.Code)
                    .WithMessage(AccessManagementErrors.InvalidRoleName.Description);
        }
    }
}