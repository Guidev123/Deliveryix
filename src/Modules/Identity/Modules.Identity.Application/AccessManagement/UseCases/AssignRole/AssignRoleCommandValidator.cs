using FluentValidation;

namespace Modules.Identity.Application.AccessManagement.UseCases.AssignRole
{
    internal sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
        }
    }
}