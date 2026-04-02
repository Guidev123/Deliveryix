using FluentValidation;

namespace Modules.Identity.Application.Identities.UseCases.Create
{
    internal sealed class CreateIdentityCommandValidator : AbstractValidator<CreateIdentityCommand>
    {
        public CreateIdentityCommandValidator()
        {
        }
    }
}