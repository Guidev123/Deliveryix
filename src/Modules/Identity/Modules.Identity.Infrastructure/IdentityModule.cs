using Azure.Identity;
using Deliveryix.Commons.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Modules.Identity.Application.Identities.Create;
using Modules.Identity.Application.Identities.Options;
using Modules.Identity.Infrastructure.Database;
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
            ArgumentException.ThrowIfNullOrWhiteSpace(sqlServerConnection);
            ArgumentException.ThrowIfNullOrWhiteSpace(redisConnection);

            services.AddCommonInfrastructure(sqlServerConnection, redisConnection);

            services.AddServices(configuration);

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