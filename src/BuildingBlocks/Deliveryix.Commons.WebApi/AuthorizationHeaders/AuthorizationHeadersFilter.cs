using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Deliveryix.Commons.WebApi.AuthorizationHeaders
{
    public sealed class AuthorizationHeadersFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var hasSkip = context.HttpContext.GetEndpoint()
                ?.Metadata
                .OfType<SkipAuthorizationHeadersAttribute>()
                .Any() ?? false;

            if (hasSkip)
                return await next(context);

            var http = context.HttpContext;

            var identityIdHeader = http.Request.Headers["X-Identity-Id"].ToString();
            var rolesHeader = http.Request.Headers["X-Roles"].ToString();
            var permissionsHeader = http.Request.Headers["X-Permissions"].ToString();

            if (string.IsNullOrEmpty(identityIdHeader))
                return Results.Problem("X-Identity-Id header missing.", statusCode: 401);

            http.Items["AuthorizationHeaders"] = new AuthorizationHeaders
            {
                IdentityId = Guid.Parse(identityIdHeader),
                Roles = JsonSerializer.Deserialize<string[]>(rolesHeader) ?? [],
                Permissions = JsonSerializer.Deserialize<string[]>(permissionsHeader) ?? []
            };

            return await next(context);
        }
    }
}