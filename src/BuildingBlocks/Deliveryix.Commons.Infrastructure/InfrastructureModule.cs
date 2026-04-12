using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Deliveryix.Commons.Application.Cache;
using Deliveryix.Commons.Application.EventBus;
using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Application.Outbox.Repositories;
using Deliveryix.Commons.Infrastructure.Cache;
using Deliveryix.Commons.Infrastructure.EventBus;
using Deliveryix.Commons.Infrastructure.Factories;
using Deliveryix.Commons.Infrastructure.Outbox.Options;
using Deliveryix.Commons.Infrastructure.Outbox.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Deliveryix.Commons.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddCommonsConfigurations(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);
            services.AddHttpContextAccessor();

            return services;
        }

        public static IServiceCollection AddEventCollector(this IServiceCollection services)
        {
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

        public static IServiceCollection AddServiceBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.SectionName));

            var section = configuration.GetSection("ServiceBus");
            var fullyQualifiedNamespace = section["FullyQualifiedNamespace"];
            var connectionString = section["ConnectionString"];

            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpTcp,
                RetryOptions = new ServiceBusRetryOptions
                {
                    Mode = ServiceBusRetryMode.Exponential,
                    MaxRetries = 3,
                    Delay = TimeSpan.FromMilliseconds(800),
                    MaxDelay = TimeSpan.FromSeconds(60)
                }
            };

            services.AddSingleton(_ =>
            {
                if (!string.IsNullOrWhiteSpace(fullyQualifiedNamespace))
                    return new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential(), clientOptions);

                if (!string.IsNullOrWhiteSpace(connectionString))
                    return new ServiceBusClient(connectionString, clientOptions);

                throw new InvalidOperationException(
                    "Configure 'ServiceBus:FullyQualifiedNamespace' (Managed Identity) " +
                    "or 'ServiceBus:ConnectionString' (dev local) on appsettings.json.");
            });

            services.AddSingleton<IEventBus, AzureServiceBus>();

            return services;
        }

        public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));

            services.AddScoped<IOutboxRepository, OutboxRepository>();

            return services;
        }

        public static IServiceCollection AddOutbox<TOutboxRepository>(this IServiceCollection services, IConfiguration configuration)
            where TOutboxRepository : class, IOutboxRepository
        {
            services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));

            services.AddScoped<IOutboxRepository, TOutboxRepository>();

            return services;
        }
    }
}