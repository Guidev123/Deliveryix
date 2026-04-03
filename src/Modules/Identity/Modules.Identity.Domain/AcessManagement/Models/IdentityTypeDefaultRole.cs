using Modules.Identity.Domain.Identities.Enums;

namespace Modules.Identity.Domain.AcessManagement.Models
{
    public sealed class IdentityTypeDefaultRole
    {
        public IdentityType IdentityType { get; private set; }
        public string RoleName { get; private set; } = null!;

        public IdentityTypeDefaultRole(IdentityType identityType, string roleName)
        {
            IdentityType = identityType;
            RoleName = roleName;
        }
    }
}