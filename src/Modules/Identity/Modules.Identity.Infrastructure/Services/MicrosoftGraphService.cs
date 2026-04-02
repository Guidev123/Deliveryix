using Deliveryix.Commons.Domain.Results;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using Modules.Identity.Application.Identities.Exceptions;
using Modules.Identity.Application.Identities.Options;
using Modules.Identity.Application.Identities.UseCases.Create;
using Modules.Identity.Domain.Identities.Errors;
using Polly.Registry;

namespace Modules.Identity.Infrastructure.Services
{
    internal sealed class MicrosoftGraphService(
        GraphServiceClient client,
        IOptions<MicrosoftGraphOptions> options,
        ResiliencePipelineProvider<string> resiliencePipelineProvider
        ) : IMicrosoftGraphService
    {
        public async Task<Result<IdentityProviderUserResponse>> GetIdentityProviderUserAsync(string email, CancellationToken cancellationToken)
        {
            var pipeline = resiliencePipelineProvider.GetPipeline(MicrosoftGraphResilienceKeys.GetUser);
            try
            {
                var user = await pipeline.ExecuteAsync(async ct =>
                {
                    var users = await client.Users
                        .GetAsync(req =>
                        {
                            req.QueryParameters.Filter =
                                $"identities/any(c:c/issuerAssignedId eq '{email}' and c/issuer eq '{options.Value.TenantDomain}')";
                            req.QueryParameters.Select = ["id"];
                        }, ct);

                    return users?.Value?.SingleOrDefault() ?? throw new GraphIdentityNotFoundException(email);
                }, cancellationToken);

                return new IdentityProviderUserResponse(user.Id!);
            }
            catch (GraphIdentityNotFoundException)
            {
                return Result.Failure<IdentityProviderUserResponse>(IdentityErrors.IdentityNotFound(email));
            }
            catch (ODataError ex)
            {
                return Result.Failure<IdentityProviderUserResponse>(IdentityErrors.GraphApiError(ex.Error?.Code, ex.Error?.Message));
            }
        }
    }

    internal static class MicrosoftGraphResilienceKeys
    {
        public const string GetUser = "MicrosoftGraph.GetUser";
    }
}