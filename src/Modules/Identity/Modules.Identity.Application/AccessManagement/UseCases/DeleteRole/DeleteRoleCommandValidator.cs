using FluentValidation;

namespace Modules.Identity.Application.AccessManagement.UseCases.DeleteRole
{
    internal sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
        }
    }
}