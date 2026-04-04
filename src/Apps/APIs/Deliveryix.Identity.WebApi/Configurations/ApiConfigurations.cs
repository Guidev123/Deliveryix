using Deliveryix.Commons.Infrastructure.Authentication;
using Deliveryix.Commons.WebApi;
using Modules.Identity.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deliveryix.Identity.WebApi.Configurations
{
    public static class ApiConfigurations
    {
        public static WebApplicationBuilder AddApiConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddOpenApi();

            builder.Services.AddProblemDetails();

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddSwaggerConfig();

            return builder;
        }

        public static WebApplicationBuilder AddSecurity(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthWithAzureEntraId(builder.Configuration);

            return builder;
        }

        public static WebApplicationBuilder AddIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentityFullInfrastructure(builder.Configuration);

            return builder;
        }

        public static WebApplication UseApiConfiguration(this WebApplication app)
        {
            app.UseExceptionHandler();

            app.MapOpenApi();

            app.UseSwaggerConfig();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapEndpoints();

            return app;
        }
    }
}