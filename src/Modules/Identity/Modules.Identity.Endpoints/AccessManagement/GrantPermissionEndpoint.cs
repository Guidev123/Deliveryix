using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.WebApi;
using Deliveryix.Commons.WebApi.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.GrantPermission;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class GrantPermissionEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/identity/access-management/roles/{name}/permissions", async (
                string name,
               [FromBody] string permissionCode,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(new GrantPermissionCommand(name, permissionCode), cancellationToken);

                return result.Match(Results.Created, ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName)
              .WithDescription("Adds a permission for a role");
        }
    }
}