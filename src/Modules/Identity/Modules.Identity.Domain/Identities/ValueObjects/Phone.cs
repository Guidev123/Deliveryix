using Deliveryix.Commons.Domain.ValueObjects;

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
            throw new NotImplementedException();
        }
    }
}