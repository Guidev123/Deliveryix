using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.WebApi;
using Deliveryix.Commons.WebApi.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.GetAllRoles;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class GetAllRolesEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/identity/access-management/roles", async (
              [FromQuery] int pageNumber,
              [FromQuery] int pageSize,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(new GetAllRolesQuery(pageSize, pageNumber), cancellationToken);

                return result.Match(
                    role => Results.Ok(role),
                    ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName)
              .WithDescription("Get all roles");
        }
    }
}