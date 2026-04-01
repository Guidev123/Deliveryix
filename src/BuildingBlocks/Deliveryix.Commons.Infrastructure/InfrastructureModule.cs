using Deliveryix.Commons.Application.Cache;
using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Application.Outbox.Repositories;
using Deliveryix.Commons.Infrastructure.Cache;
using Deliveryix.Commons.Infrastructure.Factories;
using Deliveryix.Commons.Infrastructure.Outbox.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Deliveryix.Commons.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);
            services.AddScoped<IDomainEventCollector, DomainEventCollector>();

            return services;
        }

        public static IServiceCollection AddData(this IServiceCollection services, string sqlServerConnectionSring)
        {
            services.AddSingleton<SqlConnectionFactory>(_ =>
            {
                var connectionString = sqlServerConnectionSring;

                return new(connectionString);
            });

            services.AddScoped<IOutboxRepository, OutboxRepository>();

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