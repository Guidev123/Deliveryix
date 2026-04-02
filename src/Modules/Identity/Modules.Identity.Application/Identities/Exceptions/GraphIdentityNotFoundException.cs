namespace Modules.Identity.Application.Identities.Exceptions
{
    public sealed class GraphIdentityNotFoundException(string email)
        : Exception($"Identity '{email}' not found in Microsoft Graph.")
    {
        public string Email { get; set; } = email;
    }
}