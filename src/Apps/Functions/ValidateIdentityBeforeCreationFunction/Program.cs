using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Infrastructure;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IModuleInfo, ModuleInfo>();

builder.Services
    .AddCommonsConfigurations()
    .AddEventCollector()
    .AddIdentityPersistence(builder.Configuration)
    .AddOutbox(builder.Configuration);

builder.Build().Run();