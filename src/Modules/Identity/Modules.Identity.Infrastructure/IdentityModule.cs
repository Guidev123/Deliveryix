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
        public const string FullSqlServerConnectionStringSectionName = "ConnectionStrings_SqlServer";

        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlServerConnection = configuration.GetConnectionString(SqlServerConnectionStringSectionName)
                ?? configuration[FullSqlServerConnectionStringSectionName];

            ArgumentException.ThrowIfNullOrWhiteSpace(sqlServerConnection);

            services.AddCommonInfrastructure(sqlServerConnection);

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(sqlServerConnection);
            });

            return services;
        }
    }
}