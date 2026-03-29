using Deliveryix.Commons.Infrastructure.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deliveryix.Commons.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services, string sqlServerConnectionSring)
        {
            services.AddSingleton<SqlConnectionFactory>(_ =>
            {
                var connectionString = sqlServerConnectionSring;

                return new(connectionString);
            });

            return services;
        }
    }
}