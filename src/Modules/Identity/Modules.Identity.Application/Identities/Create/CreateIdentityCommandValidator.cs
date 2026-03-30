using FluentValidation;

namespace Modules.Identity.Application.Identities.Create
{
    internal sealed class CreateIdentityCommandValidator : AbstractValidator<CreateIdentityCommand>
    {
        public CreateIdentityCommandValidator()
        {
        }
    }
}