using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.DeleteRole;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class DeleteRoleEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/v1/identity/access-management/roles/{name}", async (
                string name,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(new DeleteRoleCommand(name), cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName)
              .WithDescription("Delete a role");
        }
    }
}