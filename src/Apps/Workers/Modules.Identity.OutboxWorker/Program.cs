using Modules.Identity.Infrastructure;
using Modules.Identity.OutboxWorker;
using Modules.Identity.OutboxWorker.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection(OutboxOptions.SectionName));

builder.Services.AddIdentityModule(builder.Configuration);

var host = builder.Build();
host.Run();