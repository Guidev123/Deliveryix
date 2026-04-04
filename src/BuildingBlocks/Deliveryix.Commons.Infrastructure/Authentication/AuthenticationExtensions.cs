using Deliveryix.Commons.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace Deliveryix.Commons.Infrastructure.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthWithAzureEntraId(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthenticationWithAzureEntraId(configuration);
            services.AddAuthorizationWithAzureEntraId();

            return services;
        }

        private static void AddAuthenticationWithAzureEntraId(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(jwtOptions =>
                {
                    configuration.Bind("AzureAd", jwtOptions);

                    jwtOptions.TokenValidationParameters.NameClaimType = "name";
                    jwtOptions.TokenValidationParameters.RoleClaimType = "roles";
                    jwtOptions.TokenValidationParameters.ValidAudiences =
                    [
                        configuration["AzureAd:Audience"],
                        configuration["AzureAd:ClientId"]
                    ];
                    jwtOptions.TokenValidationParameters.ValidateLifetime = true;
                    jwtOptions.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(30);
                    jwtOptions.TokenValidationParameters.ValidateIssuer = true;

                    jwtOptions.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger("Authentication");

                            logger.LogWarning(
                                context.Exception,
                                "Authentication failed for request {Path}",
                                context.HttpContext.Request.Path
                                );

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            return Task.CompletedTask;
                        }
                    };
                },
                identityOptions =>
                {
                    configuration.Bind("AzureAd", identityOptions);
                });
        }

        private static void AddAuthorizationWithAzureEntraId(this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy("RequireAuthenticatedUser", policy =>
                    policy
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser());

            services.AddAuthorization(options =>
            {
                var defaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.DefaultPolicy = defaultPolicy;
                options.FallbackPolicy = defaultPolicy;
            });

            services.AddAuthorizationInternal();
        }
    }
}