namespace ValidateIdentityBeforeCreationFunction.Models
{
    public sealed record EntraApiConnectorRequest(
        string Email,
        string? DisplayName,
        string? GivenName,
        string? Surname,
        List<EntraIdentity>? Identities
        );

    public sealed record EntraIdentity(
        string SignInType,
        string Issuer,
        string IssuerAssignedId
    );

    public sealed record ContinuationResponse(
        string Version = "1.0.0",
        string Action = "Continue"
    );

    public sealed record BlockingResponse(
        string Version = "1.0.0",
        string Action = "ShowBlockPage",
        string UserMessage = ""
    );
}