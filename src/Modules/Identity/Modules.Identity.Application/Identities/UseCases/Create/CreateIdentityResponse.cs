namespace Modules.Identity.Application.Identities.UseCases.Create
{
    public sealed record CreateIdentityResponse(
        Guid Id,
        string IdentityProviderId
        );
}