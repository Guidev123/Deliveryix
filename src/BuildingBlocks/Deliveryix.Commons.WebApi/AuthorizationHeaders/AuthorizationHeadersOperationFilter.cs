using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Deliveryix.Commons.WebApi.AuthorizationHeaders
{
    public sealed class AuthorizationHeadersOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasSkip = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<SkipAuthorizationHeadersAttribute>()
                .Any();

            if (hasSkip)
                return;

            operation.Parameters ??= [];

            foreach (var header in new[] { "X-Identity-Id", "X-Roles", "X-Permissions" })
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = header,
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                });
            }
        }
    }
}