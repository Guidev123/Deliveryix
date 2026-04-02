using Deliveryix.WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiConfiguration()
    .AddIdentity();

var app = builder.Build();

app.UseApiConfiguration();

app.Run();