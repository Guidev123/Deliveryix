using FluentValidation;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.CreatePermission
{
    internal sealed class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
    {
        public CreatePermissionCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidPermissionCode.Code)
                    .WithMessage(AccessManagementErrors.InvalidPermissionCode.Description)
                .Matches(@"^[^:]+:[^:]+:[^:]+$")
                    .WithErrorCode(AccessManagementErrors.InvalidPermissionCode.Code)
                    .WithMessage(AccessManagementErrors.InvalidPermissionCode.Description);
        }
    }
}