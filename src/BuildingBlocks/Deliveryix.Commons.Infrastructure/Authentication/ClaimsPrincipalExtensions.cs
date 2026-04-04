using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Infrastructure.Authorization;
using System.Security.Claims;

namespace Deliveryix.Commons.Infrastructure.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static HashSet<string> GetPermissions(this ClaimsPrincipal claimsPrincipal)
        {
            var permissionClaims = claimsPrincipal?.FindAll(CustomClaims.PERMISSIONS)
                ?? throw new DeliveryixException("Permissions are unavaible");

            return permissionClaims.Select(c => c.Value).ToHashSet();
        }

        public static Guid GetEntraId(this ClaimsPrincipal claimsPrincipal)
        {
            var oid = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaims.OID)?.Value
                ?? throw new InvalidOperationException("Claim 'oid' not found on ClaimsPrincipal.");

            return Guid.Parse(oid);
        }

        public static Guid GetIdentityId(this ClaimsPrincipal claimsPrincipal)
        {
            var identityId = claimsPrincipal.FindFirstValue(CustomClaims.SUB)
                ?? throw new DeliveryixException("Claim 'sub' (IdentityId) not found on ClaimsPrincipal.");

            return Guid.Parse(identityId);
        }
    }
}