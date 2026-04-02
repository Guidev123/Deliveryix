using FluentValidation;
using Modules.Identity.Domain.Identities.Errors;
using Modules.Identity.Domain.Identities.ValueObjects;

namespace Modules.Identity.Application.Identities.UseCases.Create
{
    internal sealed class CreateIdentityCommandValidator : AbstractValidator<CreateIdentityCommand>
    {
        public CreateIdentityCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithErrorCode(IdentityErrors.InvalidEmail().Code)
                    .WithMessage(IdentityErrors.InvalidEmail().Description)
                .MaximumLength(Email.MaxEmailLength)
                    .WithErrorCode(IdentityErrors.InvalidEmail().Code)
                    .WithMessage(IdentityErrors.InvalidEmail().Description)
                .EmailAddress()
                    .WithErrorCode(IdentityErrors.InvalidEmail().Code)
                    .WithMessage(IdentityErrors.InvalidEmail().Description);

            RuleFor(x => x.Document)
                .NotEmpty()
                    .WithErrorCode(IdentityErrors.InvalidDocument().Code)
                    .WithMessage(IdentityErrors.InvalidDocument().Description)
                .Must(d => d.Length is LegalDocument.CpfLength or LegalDocument.CnpjLength)
                    .WithErrorCode(IdentityErrors.InvalidDocument().Code)
                    .WithMessage(IdentityErrors.InvalidDocument().Description)
                .Must(d => d.Length is LegalDocument.CpfLength
                    ? LegalDocument.IsValidCpf(d)
                    : LegalDocument.IsValidCnpj(d))
                    .WithErrorCode(IdentityErrors.InvalidDocument().Code)
                    .WithMessage(IdentityErrors.InvalidDocument().Description);

            RuleFor(x => x.Phone)
                .NotEmpty()
                    .WithErrorCode(IdentityErrors.InvalidPhone().Code)
                    .WithMessage(IdentityErrors.InvalidPhone().Description)
                .Matches(@"^\+[1-9]\d{6,14}$")
                    .WithErrorCode(IdentityErrors.InvalidPhone().Code)
                    .WithMessage(IdentityErrors.InvalidPhone().Description);
        }
    }
}