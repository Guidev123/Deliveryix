namespace Modules.Identity.Application.AccessManagement.Options
{
    public sealed class ProfileCacheOptions
    {
        public const string SectionName = "ProfileCache";

        public int CacheExpirationTimeInMinutes { get; set; }
    }
}