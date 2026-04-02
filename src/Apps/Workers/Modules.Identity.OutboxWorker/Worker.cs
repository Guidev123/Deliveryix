using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Application.Extensions;
using Deliveryix.Commons.Application.Outbox.Repositories;
using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Infrastructure.Outbox.Options;
using Microsoft.Extensions.Options;
using MidR.Interfaces;
using Modules.Identity.Domain.Identities.Extensions;
using Newtonsoft.Json;

namespace Modules.Identity.OutboxWorker
{
    public class Worker(
        IServiceProvider serviceProvider,
        IOptions<OutboxOptions> options,
        ILogger<Worker> logger
        ) : BackgroundService
    {
        private readonly OutboxOptions _outboxOptions = options.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(_outboxOptions.IntervalInSeconds));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await ProcessOutboxAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[Identity] Unhandled exception in outbox worker");
                }
            }
        }

        private async Task ProcessOutboxAsync(CancellationToken stoppingToken)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            logger.LogInformation("[Identity] Beginning to process outbox messages");

            await unitOfWork.BeginTransactionAsync(stoppingToken);

            var outboxMessages = await outboxRepository.GetAsync(
                _outboxOptions.BatchSize,
                ModuleExtensions.ModuleName,
                stoppingToken);

            foreach (var outboxMessage in outboxMessages)
            {
                await using var messageScope = scope.ServiceProvider.CreateAsyncScope();

                Exception? exception = null;

                try
                {
                    var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(
                        outboxMessage.Content,
                        JsonSerializerSettingsExtensions.Instance)!;

                    var messagePublisher = messageScope.ServiceProvider.GetRequiredService<IPublisher>();
                    await messagePublisher.PublishAsync(domainEvent, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[Identity] Exception while processing outbox message {MessageId}", outboxMessage.Id);
                    exception = ex;
                }

                await outboxRepository.UpdateAsync(
                    ModuleExtensions.ModuleName,
                    exception,
                    outboxMessage,
                    stoppingToken);
            }

            await unitOfWork.CommitAsync(stoppingToken);

            logger.LogInformation("[Identity] Completed process outbox messages");
        }
    }
}