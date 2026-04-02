using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.AssignRole;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class AssignRoleEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/identity/access-management/roles/identities", async (
                AssignRoleCommand request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(request, cancellationToken);

                return result.Match(() => Results.Created(), ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName);
        }
    }
}