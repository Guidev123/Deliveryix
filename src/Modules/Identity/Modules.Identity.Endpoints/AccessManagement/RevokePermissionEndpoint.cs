using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.WebApi;
using Deliveryix.Commons.WebApi.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.RevokePermission;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class RevokePermissionEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/v1/roles/{name}/permissions/{permissionCode}", async (
               string name,
               string permissionCode,
               ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(new RevokePermissionCommand(name, permissionCode), cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName)
              .WithDescription("Remove a permission from a role");
        }
    }
}