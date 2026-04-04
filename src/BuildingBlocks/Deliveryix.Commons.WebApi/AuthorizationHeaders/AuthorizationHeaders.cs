using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text.Json;

namespace Deliveryix.Commons.WebApi.AuthorizationHeaders
{
    public sealed record AuthorizationHeaders : IBindableFromHttpContext<AuthorizationHeaders>
    {
        public Guid IdentityId { get; set; }
        public string[] Roles { get; set; } = [];
        public string[] Permissions { get; set; } = [];

        public static ValueTask<AuthorizationHeaders?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            var identityId = context.Request.Headers["X-Identity-Id"].ToString();
            var roles = context.Request.Headers["X-Roles"].ToString();
            var permissions = context.Request.Headers["X-Permissions"].ToString();

            if (string.IsNullOrEmpty(identityId))
                return ValueTask.FromResult<AuthorizationHeaders?>(null);

            var result = new AuthorizationHeaders
            {
                IdentityId = Guid.Parse(identityId),
                Roles = JsonSerializer.Deserialize<string[]>(roles) ?? [],
                Permissions = JsonSerializer.Deserialize<string[]>(permissions) ?? []
            };

            return ValueTask.FromResult<AuthorizationHeaders?>(result);
        }
    }
}