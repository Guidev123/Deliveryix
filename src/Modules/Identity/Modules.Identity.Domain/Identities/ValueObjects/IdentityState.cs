using Deliveryix.Commons.Domain.ValueObjects;
using Modules.Identity.Domain.Identities.Enums;

namespace Modules.Identity.Domain.Identities.ValueObjects
{
    public sealed record IdentityState : ValueObject
    {
        private IdentityState(IdentityStatus status)
        {
            Status = status;
            DeletedOn = !IsActive ? DateTimeOffset.UtcNow : null;
            Validate();
        }

        private IdentityState()
        { }

        public IdentityStatus Status { get; }
        public DateTimeOffset? DeletedOn { get; }
        public bool IsActive =>
            Status == IdentityStatus.Active
            && DeletedOn is not null;

        public static implicit operator IdentityState(IdentityStatus status)
            => new(status);

        protected override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}