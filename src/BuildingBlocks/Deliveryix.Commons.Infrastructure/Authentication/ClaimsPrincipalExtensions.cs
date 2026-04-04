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
    }
}