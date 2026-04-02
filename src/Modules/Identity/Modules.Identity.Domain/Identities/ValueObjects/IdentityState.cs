using Deliveryix.Commons.Domain.ValueObjects;
using Modules.Identity.Domain.Identities.Enums;

namespace Modules.Identity.Domain.Identities.ValueObjects
{
    public sealed record IdentityState : ValueObject
    {
        private IdentityState(IdentityStatus status)
        {
            Status = status;
            Validate();
        }

        private IdentityState()
        { }

        public IdentityStatus Status { get; }
        public DateTimeOffset? DeletedOn { get; }

        public static implicit operator IdentityState(IdentityStatus status)
            => new(status);

        protected override void Validate()
        {
        }
    }
}