using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.WebApi;
using Deliveryix.Commons.WebApi.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.UnassignRole;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class UnassignRoleEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/v1/roles/{name}/identities/{identityId}", async (
                string name,
                Guid identityId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(new UnassignRoleCommand(identityId, name), cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName)
              .WithDescription("Remove a role from an identity");
        }
    }
}