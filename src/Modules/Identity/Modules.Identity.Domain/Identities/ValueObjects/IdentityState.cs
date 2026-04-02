using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Domain.ValueObjects;
using Modules.Identity.Domain.Identities.Enums;
using Modules.Identity.Domain.Identities.Errors;

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
            AssertionConcern.EnsureTrue(
                Enum.IsDefined(typeof(IdentityStatus), Status),
                IdentityErrors.InvalidIdentityStatus().Description);
        }
    }
}