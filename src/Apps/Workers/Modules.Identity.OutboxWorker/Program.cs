using Modules.Identity.Infrastructure;
using Modules.Identity.OutboxWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddIdentityModule(builder.Configuration);

var host = builder.Build();
host.Run();