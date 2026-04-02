using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Domain.ValueObjects;
using Modules.Identity.Domain.Identities.Errors;

namespace Modules.Identity.Domain.Identities.ValueObjects
{
    public sealed record Phone : ValueObject
    {
        private Phone(string number)
        {
            Number = number;
        }

        private Phone()
        { }

        public string Number { get; } = null!;

        public static implicit operator Phone(string number) => new(number);

        protected override void Validate()
        {
            AssertionConcern.EnsureNotEmpty(Number, IdentityErrors.InvalidPhone().Description);
            AssertionConcern.EnsureMatchesPattern(
                @"^\+[1-9]\d{6,14}$",
                Number,
                IdentityErrors.InvalidPhone().Description);
        }
    }
}