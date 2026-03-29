namespace ValidateIdentityBeforeCreationFunction.Models
{
    public sealed record EntraAttributeCollectionRequest(
        string Type,
        EntraAttributeCollectionData Data
    );

    public sealed record EntraAttributeCollectionData(
        EntraUserSignUpInfo UserSignUpInfo
    );

    public sealed record EntraUserSignUpInfo(
        Dictionary<string, EntraAttributeValue> Attributes,
        List<EntraIdentity>? Identities
    );

    public sealed record EntraAttributeValue(
        string? Value,
        string? AttributeType
    );

    public sealed record EntraIdentity(
        string SignInType,
        string Issuer,
        string IssuerAssignedId
    );
}