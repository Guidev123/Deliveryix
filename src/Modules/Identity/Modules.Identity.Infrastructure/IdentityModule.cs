using Azure.Identity;
using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Application.Behaviors;
using Deliveryix.Commons.Infrastructure;
using Deliveryix.Commons.WebApi;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using MidR.DependencyInjection;
using Modules.Identity.Application;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Application.Identities.Exceptions;
using Modules.Identity.Application.Identities.Options;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Application.Identities.UseCases.Create;
using Modules.Identity.Endpoints;
using Modules.Identity.Infrastructure.Database;
using Modules.Identity.Infrastructure.Database.Repositories;
using Modules.Identity.Infrastructure.Services;
using Polly;
using Polly.Retry;

namespace Modules.Identity.Infrastructure
{
    public static class IdentityModule
    {
        public const string SqlServerConnectionStringSectionName = "SqlServer";
        public const string RedisConnectionStringSectionName = "Redis";

        public static IServiceCollection AddIdentityFullInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                    .AddEventCollector()
                    .AddCommonsConfigurations()
                    .AddIdentityEndpoints()
                    .AddIdentityCore()
                    .AddOutbox(configuration)
                    .AddIdentityPersistence(configuration)
                    .AddIdentityCache(configuration)
                    .AddIdentityGraphClient(configuration)
                    .AddServiceBus(configuration);
        }

        public static IServiceCollection AddIdentityCore(this IServiceCollection services)
        {
            services
                .AddCommonsConfigurations()
                .AddEventCollector()
                .AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true)
                .AddMidR(args: AssemblyReference.Assembly)
                .WithBehaviors(cfg =>
                {
                    cfg.AddBehavior(typeof(RequestLoggingBehavior<,>)).WithPriority(1);
                    cfg.AddBehavior(typeof(RequestValidationBehavior<,>)).WithPriority(2);
                    cfg.AddBehavior(typeof(RequestTransactionBehavior<,>)).WithPriority(3);

                    cfg.AddBehavior(typeof(NotificationLoggingBehavior<>)).WithPriority(1);
                    cfg.AddBehavior(typeof(OutboxIdempotencyBehavior<>)).WithPriority(2);
                });

            services.AddSingleton<IModuleInfo, ModuleInfo>();

            return services;
        }

        public static IServiceCollection AddIdentityEndpoints(this IServiceCollection services)
        {
            services.AddEndpoints(typeof(EndpointsModule).Assembly);

            return services;
        }

        public static IServiceCollection AddIdentityPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(SqlServerConnectionStringSectionName)!;

            services.AddData(connectionString);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddIdentityCache(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConnection = configuration.GetConnectionString(RedisConnectionStringSectionName)!;
            services.AddCacheService(redisConnection);

            return services;
        }

        public static IServiceCollection AddIdentityGraphClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MicrosoftGraphOptions>(
                configuration.GetSection(MicrosoftGraphOptions.SectionName));

            services.AddSingleton(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<MicrosoftGraphOptions>>().Value;
                var credential = new ClientSecretCredential(
                    opts.TenantId, opts.ClientId, opts.ClientSecret);

                return new GraphServiceClient(credential, scopes: [opts.Scope]);
            });

            services.AddResiliencePipeline(
                MicrosoftGraphResilienceKeys.GetUser,
                builder => builder.AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder()
                        .Handle<GraphIdentityNotFoundException>()
                        .Handle<ODataError>(ex => ex.Error?.Code is "Request_ResourceNotFound" or "ServiceNotAvailable"),

                    MaxRetryAttempts = 5,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                }));

            services.AddScoped<IMicrosoftGraphService, MicrosoftGraphService>();

            return services;
        }
    }
}