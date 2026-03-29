using Deliveryix.Commons.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Infrastructure.Database;

namespace Modules.Identity.Infrastructure
{
    public static class IdentityModule
    {
        public const string SqlServerConnectionStringSectionName = "SqlServer";
        public const string RedisConnectionStringSectionName = "Redis";

        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlServerConnection = configuration.GetConnectionString(SqlServerConnectionStringSectionName)!;
            var redisConnection = configuration.GetConnectionString(RedisConnectionStringSectionName)!;
            ArgumentException.ThrowIfNullOrWhiteSpace(sqlServerConnection);
            ArgumentException.ThrowIfNullOrWhiteSpace(redisConnection);

            services.AddCommonInfrastructure(sqlServerConnection, redisConnection);

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(sqlServerConnection);
            });

            return services;
        }
    }
}