using Deliveryix.Commons.Domain.Results;
using Deliveryix.Commons.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MidR.Interfaces;
using Modules.Identity.Application.AccessManagement.UseCases.SetDefaultIdentityTypeRole;
using Modules.Identity.Domain.Identities.Enums;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Endpoints.AccessManagement
{
    internal sealed class SetDefaultIdentityTypeRoleEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/identity/access-management/roles/{name}/identity-type/defaut", async (
                string name,
                IdentityType identityType,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.SendAsync(new SetDefaultIdentityTypeRoleCommand(identityType, name), cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            }).WithTags(ModuleExtensions.ModuleName)
              .WithDescription("Assigns default roles to a specific account type");
        }
    }
}