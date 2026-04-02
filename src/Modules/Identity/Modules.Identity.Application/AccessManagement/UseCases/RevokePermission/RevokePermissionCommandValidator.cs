using FluentValidation;
using Modules.Identity.Domain.AcessManagement.Errors;

namespace Modules.Identity.Application.AccessManagement.UseCases.RevokePermission
{
    internal sealed class RevokePermissionCommandValidator : AbstractValidator<RevokePermissionCommand>
    {
        public RevokePermissionCommandValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidRoleName.Code)
                    .WithMessage(AccessManagementErrors.InvalidRoleName.Description);

            RuleFor(x => x.PermissionCode)
                .NotEmpty()
                    .WithErrorCode(AccessManagementErrors.InvalidPermissionCode.Code)
                    .WithMessage(AccessManagementErrors.InvalidPermissionCode.Description)
                .Matches(@"^[^:]+:[^:]+:[^:]+$")
                    .WithErrorCode(AccessManagementErrors.InvalidPermissionCode.Code)
                    .WithMessage(AccessManagementErrors.InvalidPermissionCode.Description);
        }
    }
}