using FluentValidation;

namespace Modules.Identity.Application.AccessManagement.UseCases.CreatePermission
{
    internal sealed class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
    {
        public CreatePermissionCommandValidator()
        {
        }
    }
}