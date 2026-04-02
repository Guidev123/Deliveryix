using Deliveryix.Commons.Domain.DomainObjects;
using System.Security.Claims;

namespace Deliveryix.Commons.WebApi
{
    public static class ClaimsPrincipalExtensions
    {
        private const string SUB = "sub";
        private const string PERMISSIONS = "permissions";

        public static Guid GetIdentityId(this ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal?.FindFirst(SUB)?.Value;
            return Guid.TryParse(userId, out var parsedUserId)
                ? parsedUserId
                : throw new DeliveryixException("Identity identifier is unavaible");
        }

        public static HashSet<string> GetPermissions(this ClaimsPrincipal claimsPrincipal)
        {
            var permissionClaims = claimsPrincipal?.FindAll(PERMISSIONS)
                ?? throw new DeliveryixException("Permissions are unavaible");

            return permissionClaims.Select(c => c.Value).ToHashSet();
        }
    }
}