using FluentValidation;

namespace Modules.Identity.Application.AccessManagement.UseCases.GrantPermission
{
    internal sealed class GrantPermissionCommandValidator : AbstractValidator<GrantPermissionCommand>
    {
        public GrantPermissionCommandValidator()
        {
        }
    }
}