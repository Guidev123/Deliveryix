using Deliveryix.Commons.Infrastructure;
using Modules.Identity.Infrastructure;
using Modules.Identity.OutboxWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services
    .AddIdentityCore()
    .AddIdentityPersistence(builder.Configuration)
    .AddIdentityCache(builder.Configuration)
    .AddOutbox(builder.Configuration)
    .AddServiceBus(builder.Configuration)
    .AddIdentityGraphClient(builder.Configuration);

var host = builder.Build();
host.Run();