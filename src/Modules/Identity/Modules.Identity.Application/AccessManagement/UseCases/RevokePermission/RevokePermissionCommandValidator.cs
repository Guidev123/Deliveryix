using FluentValidation;

namespace Modules.Identity.Application.AccessManagement.UseCases.RevokePermission
{
    internal sealed class RevokePermissionCommandValidator : AbstractValidator<RevokePermissionCommand>
    {
        public RevokePermissionCommandValidator()
        {
        }
    }
}