using Deliveryix.Commons.Domain.Results;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using Modules.Identity.Application.Identities.Create;
using Modules.Identity.Application.Identities.Options;
using Modules.Identity.Domain.Identities.Errors;

namespace Modules.Identity.Infrastructure.Services
{
    internal sealed class MicrosoftGraphService(
        GraphServiceClient client,
        IOptions<MicrosoftGraphOptions> options
        ) : IMicrosoftGraphService
    {
        public async Task<Result<IdentityProviderUserResponse>> GetIdentityProviderUserAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                var users = await client.Users
                    .GetAsync(req =>
                    {
                        req.QueryParameters.Filter = $"identities/any(c:c/issuerAssignedId eq '{email}' and c/issuer eq '{options.Value.TenantDomain}')";
                        req.QueryParameters.Select = ["id"];
                    }, cancellationToken);

                var user = users?.Value?.SingleOrDefault();

                if (user is null)
                {
                    return Result.Failure<IdentityProviderUserResponse>(IdentityErrors.UserNotFound(email));
                }

                return new IdentityProviderUserResponse(user.Id!);
            }
            catch (ODataError ex)
            {
                return Result.Failure<IdentityProviderUserResponse>(IdentityErrors.GraphApiError(ex.Error?.Code, ex.Error?.Message));
            }
        }
    }
}