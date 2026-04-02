using FluentValidation;

namespace Modules.Identity.Application.AccessManagement.UseCases.UnassignRole
{
    internal sealed class UnassignRoleCommandValidator : AbstractValidator<UnassignRoleCommand>
    {
        public UnassignRoleCommandValidator()
        {
        }
    }
}