using Azure.Identity;
using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Application.Behaviors;
using Deliveryix.Commons.Infrastructure;
using Deliveryix.Commons.Infrastructure.EventBus;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using MidR.DependencyInjection;
using Modules.Identity.Application;
using Modules.Identity.Application.Identities.Behaviors;
using Modules.Identity.Application.Identities.Create;
using Modules.Identity.Application.Identities.Options;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Infrastructure.Database;
using Modules.Identity.Infrastructure.Database.Repositories;
using Modules.Identity.Infrastructure.Services;

namespace Modules.Identity.Infrastructure
{
    public static class IdentityModule
    {
        public const string SqlServerConnectionStringSectionName = "SqlServer";
        public const string RedisConnectionStringSectionName = "Redis";

        public static IServiceCollection AddIdentityFullInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                    .AddIdentityCore()
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
                .AddValidatorsFromAssembly(AssemblyReference.Assembly)
                .AddMidR(args: AssemblyReference.Assembly)
                .WithBehaviors(cfg =>
                {
                    cfg.AddBehavior(typeof(RequestLoggingBehavior<,>)).WithPriority(1);
                    cfg.AddBehavior(typeof(RequestValidationBehavior<,>)).WithPriority(2);
                    cfg.AddBehavior(typeof(RequestTransactionBehavior<,>)).WithPriority(3);

                    cfg.AddBehavior(typeof(NotificationLoggingBehavior<>)).WithPriority(1);
                    cfg.AddBehavior(typeof(OutboxIdempotencyBehavior<>)).WithPriority(2);
                });

            return services;
        }

        public static IServiceCollection AddIdentityPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(SqlServerConnectionStringSectionName)!;

            services.AddData(connectionString);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
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

            services.AddScoped<IMicrosoftGraphService, MicrosoftGraphService>();

            return services;
        }
    }
}