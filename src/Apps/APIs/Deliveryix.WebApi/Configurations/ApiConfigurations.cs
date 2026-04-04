using Deliveryix.Commons.WebApi.Configurations;
using Deliveryix.Commons.WebApi.Endpoints;
using Modules.Identity.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddSwaggerConfig();

            return builder;
        }

        public static WebApplication UseApiConfiguration(this WebApplication app)
        {
            app.UseExceptionHandler();

            app.MapOpenApi();

            app.MapEndpoints();

            app.UseSwaggerConfig();

            return app;
        }
    }
}