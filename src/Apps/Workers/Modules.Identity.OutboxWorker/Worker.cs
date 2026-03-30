using Dapper;
using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Infrastructure.Factories;
using Deliveryix.Commons.Infrastructure.Outbox.Models;
using Deliveryix.Commons.Infrastructure.Serializer;
using Microsoft.Extensions.Options;
using MidR.Interfaces;
using Modules.Identity.Infrastructure.Database;
using Modules.Identity.OutboxWorker.Options;
using Newtonsoft.Json;
using System.Data;

namespace Modules.Identity.OutboxWorker
{
    public class Worker(
        IPublisher publisher,
        SqlConnectionFactory sqlConnectionFactory,
        TimeProvider timeProvider,
        IOptions<OutboxOptions> options,
        ILogger<Worker> logger
        ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("[Identity] Beginning to process outbox messages");

            using var connection = sqlConnectionFactory.Create();
            await connection.OpenAsync(stoppingToken);

            using var transaction = await connection.BeginTransactionAsync(stoppingToken);

            var outboxMessages = await GetOutboxMessagesAsync(connection, transaction, options.Value.BatchSize, Schemas.DefaultSchemaName, stoppingToken);

            foreach (var outboxMessage in outboxMessages)
            {
                Exception? exception = null;

                try
                {
                    var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, JsonSerializerSettingsExtensions.Instance)!;

                    await publisher.PublishToBusAsync(domainEvent, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[Identity] Exception while processing outbox message {MessageId}", outboxMessage.Id);
                    exception = ex;
                }

                await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception, Schemas.DefaultSchemaName, stoppingToken);
            }

            await transaction.CommitAsync(stoppingToken);

            logger.LogInformation("[Identity] Completed process outbox messages");
        }

        private static async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int batchSize,
            string schema,
            CancellationToken cancellationToken
            )
        {
            var sql = @$"
                SELECT TOP ({batchSize})
                    Id,
                    Content
                FROM {schema}.OutboxMessages WITH (UPDLOCK, ROWLOCK)
                WHERE ProcessedOn IS NULL
                ORDER BY OccurredOn";

            var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction).WaitAsync(cancellationToken);

            return outboxMessages.ToList();
        }

        private async Task UpdateOutboxMessageAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            OutboxMessageResponse outboxMessage,
            Exception? exception,
            string schema,
            CancellationToken cancellationToken
            )
        {
            var sql = @$"
                UPDATE {schema}.OutboxMessages
                SET ProcessedOn = @ProcessedOn,
                    Error = @Error
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, new
            {
                outboxMessage.Id,
                ProcessedOnUtc = timeProvider.GetUtcNow(),
                Error = GetExceptionMessage(exception)
            }, transaction: transaction).WaitAsync(cancellationToken);
        }

        private static string? GetExceptionMessage(Exception? exception)
        {
            if (exception is null)
                return null;

            return exception switch
            {
                DeliveryixException dvEx when dvEx.Error?.Description is not null => dvEx.Error.Description,
                _ when exception.InnerException?.Message is not null => exception.InnerException.Message,
                _ => exception.Message
            };
        }
    }
}