using Deliveryix.Commons.Domain.DomainObjects;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Domain.Identities.DomainEvents
{
    public sealed record RoleAssignedDomainEvent : DomainEvent
    {
        public static RoleAssignedDomainEvent Create(Guid identityId, string roleName)
            => new(identityId, roleName);

        public Guid IdentityId { get; }
        public string RoleName { get; } = null!;
        private RoleAssignedDomainEvent(Guid identityId, string roleName)
            : base(identityId, nameof(RoleAssignedDomainEvent), ModuleExtensions.ModuleName)
        {
            IdentityId = identityId;
            RoleName = roleName;
        }

        private RoleAssignedDomainEvent()
        { }
    }
}