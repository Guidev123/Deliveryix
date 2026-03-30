namespace Modules.Identity.Application.Identities.Create
{
    public sealed record CreateIdentityResponse(
        Guid Id,
        string IdentityProviderId
        );
}