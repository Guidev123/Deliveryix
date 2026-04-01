using Deliveryix.Commons.Domain.ValueObjects;

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
        }
    }
}