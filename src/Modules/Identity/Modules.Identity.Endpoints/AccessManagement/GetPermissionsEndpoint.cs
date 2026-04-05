using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.Infrastructure.Authentication;
using Deliveryix.Commons.WebApi;
using Deliveryix.Commons.WebApi.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.GetPermissions;
using Modules.Identity.Domain.Identities.Extensions;
using System.Security.Claims;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class GetPermissionsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/identities/me", async (ClaimsPrincipal claimsPrincipal, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(new GetIdentityPermissionsQuery(claimsPrincipal.GetEntraId()), cancellationToken);

                return result.Match(() => Results.Ok(result.Value), ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName)
              .WithoutAuthorizationHeaders()
              .WithDescription("Get permissions of authenticated idenity");
        }
    }
}