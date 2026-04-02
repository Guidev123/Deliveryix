namespace Modules.Identity.Application.Identities.Options
{
    public sealed class MicrosoftGraphOptions
    {
        public const string SectionName = "MicrosoftGraph";

        public string TenantId { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string TenantDomain { get; set; } = null!;
        public string Scope { get; set; } = null!;
    }
}