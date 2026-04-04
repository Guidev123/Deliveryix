using Deliveryix.Identity.WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiConfiguration()
    .AddSecurity()
    .AddIdentity();

var app = builder.Build();

app.UseApiConfiguration();

app.Run();