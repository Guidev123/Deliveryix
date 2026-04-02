using FluentValidation;

namespace Modules.Identity.Application.AccessManagement.UseCases.CreateRole
{
    internal sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
        }
    }
}