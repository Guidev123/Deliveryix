using Deliveryix.Commons.Domain.Results;

namespace Modules.Identity.Application.Identities.Create
{
    public interface IMicrosoftGraphService
    {
        Task<Result<IdentityProviderUserResponse>> GetIdentityProviderUserAsync(string email, CancellationToken cancellationToken);
    }
}