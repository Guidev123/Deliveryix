using Deliveryix.Commons.Infrastructure.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deliveryix.Commons.Infrastructure
{
    public static class InfrastructureModule
    {
        public const string SqlServerConnectionStringSectionName = "SqlServer";

        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SqlConnectionFactory>(_ =>
            {
                var connectionString = configuration.GetConnectionString(SqlServerConnectionStringSectionName)!;

                return new(connectionString);
            });

            return services;
        }
    }
}