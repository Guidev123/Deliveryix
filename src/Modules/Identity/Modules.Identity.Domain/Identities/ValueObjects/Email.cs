using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Domain.ValueObjects;
using Modules.Identity.Domain.Identities.Errors;

namespace Modules.Identity.Domain.Identities.ValueObjects
{
    public sealed record Email : ValueObject
    {
        public const int MaxEmailLength = 160;

        private Email(string address)
        {
            Address = address;
            Validate();
        }

        private Email()
        { }

        public string Address { get; } = null!;

        public static implicit operator Email(string address) => new(address);

        protected override void Validate()
        {
            AssertionConcern.EnsureNotEmpty(Address, IdentityErrors.InvalidEmail().Description);
            AssertionConcern.EnsureMaxLength(Address, MaxEmailLength, IdentityErrors.InvalidEmail().Description);
            AssertionConcern.EnsureMatchesPattern(
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                Address,
                IdentityErrors.InvalidEmail().Description);
        }
    }
}