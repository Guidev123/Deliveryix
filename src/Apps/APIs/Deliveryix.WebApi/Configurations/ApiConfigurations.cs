using Deliveryix.Commons.WebApi;
using Microsoft.Graph.Models;
using Modules.Identity.Infrastructure;
using System.Text.Json;

namespace Deliveryix.WebApi.Configurations
{
    public static class ApiConfigurations
    {
        public static WebApplicationBuilder AddApiConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddOpenApi();

            builder.Services.AddProblemDetails();

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddSwaggerConfig();

            return builder;
        }

        public static WebApplicationBuilder AddIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentityFullInfrastructure(builder.Configuration);

            return builder;
        }

        public static Microsoft.AspNetCore.Builder.WebApplication UseApiConfiguration(this Microsoft.AspNetCore.Builder.WebApplication app)
        {
            app.UseExceptionHandler();

            app.MapOpenApi();

            app.MapEndpoints();

            app.UseSwaggerConfig();

            return app;
        }
    }
}