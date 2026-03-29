using Deliveryix.Commons.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Infrastructure.Database;

namespace Modules.Identity.Infrastructure
{
    public static class IdentityModule
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCommonInfrastructure(configuration);

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(InfrastructureModule.SqlServerConnectionStringSectionName)!);
            });

            return services;
        }
    }
}