using Modules.Identity.Infrastructure;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddIdentityFullInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();

app.Run();