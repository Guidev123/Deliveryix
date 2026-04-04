using Deliveryix.Commons.Domain.DomainObjects;
using System.Security.Claims;

namespace Deliveryix.Commons.WebApi
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetEntraId(this ClaimsPrincipal claimsPrincipal)
        {
            var oid = claimsPrincipal.FindFirstValue("oid")
                ?? claimsPrincipal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
                ?? throw new InvalidOperationException("Claim 'oid' not found on ClaimsPrincipal.");

            return Guid.Parse(oid);
        }

        public static HashSet<string> GetPermissions(this ClaimsPrincipal claimsPrincipal)
        {
            var permissionClaims = claimsPrincipal?.FindAll("permissions")
                ?? throw new DeliveryixException("Permissions are unavaible");

            return permissionClaims.Select(c => c.Value).ToHashSet();
        }
    }
}