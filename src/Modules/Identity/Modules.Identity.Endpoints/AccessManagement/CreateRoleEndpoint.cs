using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.CreateRole;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class CreateRoleEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/identity/access-management/roles", async (
                CreateRoleCommand request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(request, cancellationToken);

                return result.Match(
                    () => Results.Created($"api/v1/identity/access-management/roles/{request.Name}", request.Name),
                    ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName);
        }
    }
}