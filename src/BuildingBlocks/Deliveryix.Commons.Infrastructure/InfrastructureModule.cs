using Deliveryix.Commons.Application.Cache;
using Deliveryix.Commons.Infrastructure.Cache;
using Deliveryix.Commons.Infrastructure.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Deliveryix.Commons.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddCommonInfrastructure(
            this IServiceCollection services,
            string sqlServerConnectionSring,
            string redisConnectionString
            )
        {
            services.AddData(sqlServerConnectionSring);
            services.AddCacheService(redisConnectionString);

            return services;
        }

        public static IServiceCollection AddData(this IServiceCollection services, string sqlServerConnectionSring)
        {
            services.AddSingleton<SqlConnectionFactory>(_ =>
            {
                var connectionString = sqlServerConnectionSring;

                return new(connectionString);
            });

            return services;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services, string redisConnectionString)
        {
            try
            {
                IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
                services.TryAddSingleton(connectionMultiplexer);

                services.AddStackExchangeRedisCache(options =>
                {
                    options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
                });

                services.TryAddSingleton<ICacheService, CacheService>();
            }
            catch
            {
                services.TryAddSingleton<ICacheService, CacheService>();
                services.AddDistributedMemoryCache();
            }

            return services;
        }
    }
}