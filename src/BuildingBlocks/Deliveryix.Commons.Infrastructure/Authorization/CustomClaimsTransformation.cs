using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace Deliveryix.Commons.Infrastructure.Authorization
{
    internal sealed class CustomClaimsTransformation(IHttpContextAccessor httpContextAccessor) : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.HasClaim(c => c.Type == CustomClaims.SUB))
                return Task.FromResult(principal);

            var http = httpContextAccessor.HttpContext;
            if (http is null)
                return Task.FromResult(principal);

            var claimsIdentity = new ClaimsIdentity();

            if (http.Request.Headers.TryGetValue("X-Identity-Id", out var identityId))
                claimsIdentity.AddClaim(new(CustomClaims.SUB, identityId.ToString()));

            if (http.Request.Headers.TryGetValue("X-Roles", out var rolesHeader))
            {
                var roles = JsonSerializer.Deserialize<string[]>(rolesHeader.ToString());
                if (roles is not null)
                    foreach (var role in roles)
                        claimsIdentity.AddClaim(new(ClaimTypes.Role, role));
            }

            if (http.Request.Headers.TryGetValue("X-Permissions", out var permissionsHeader))
            {
                var permissions = JsonSerializer.Deserialize<string[]>(permissionsHeader.ToString());
                if (permissions is not null)
                    foreach (var permission in permissions)
                        claimsIdentity.AddClaim(new(CustomClaims.PERMISSIONS, permission));
            }

            principal.AddIdentity(claimsIdentity);
            return Task.FromResult(principal);
        }
    }
}