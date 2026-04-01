using Azure.Identity;
using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Application.Behaviors;
using Deliveryix.Commons.Infrastructure;
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

        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlServerConnection = configuration.GetConnectionString(SqlServerConnectionStringSectionName)!;
            var redisConnection = configuration.GetConnectionString(RedisConnectionStringSectionName)!;

            if (!string.IsNullOrEmpty(sqlServerConnection))
            {
                services.AddData(sqlServerConnection);
            }

            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddCacheService(redisConnection);
            }

            services.AddCommonInfrastructure();

            services.AddServices(configuration);

            services
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityRepository, IdentityRepository>();

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(sqlServerConnection);
            });

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MicrosoftGraphOptions>(configuration.GetSection(MicrosoftGraphOptions.SectionName));

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MicrosoftGraphOptions>>();
                var graphOptions = options.Value;

                var credential = new ClientSecretCredential(graphOptions.TenantId, graphOptions.ClientId, graphOptions.ClientSecret);

                return new GraphServiceClient(credential, scopes: [graphOptions.Scope]);
            });

            services.AddScoped<IMicrosoftGraphService, MicrosoftGraphService>();

            return services;
        }
    }
}