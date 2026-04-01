using Dapper;
using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Application.Extensions;
using Deliveryix.Commons.Application.Outbox.Models;
using Deliveryix.Commons.Application.Outbox.Repositories;
using Deliveryix.Commons.Domain.DomainObjects;
using Newtonsoft.Json;

namespace Deliveryix.Commons.Infrastructure.Outbox.Repositories
{
    internal sealed class OutboxRepository(
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider
        ) : IOutboxRepository
    {
        public async Task<IReadOnlyList<OutboxMessageResponse>> GetAsync(
            int batchSize,
            string schema,
            CancellationToken cancellationToken)
        {
            var sql = @$"
                SELECT TOP ({batchSize})
                    Id,
                    Content
                FROM [{schema}].OutboxMessages WITH (UPDLOCK, ROWLOCK)
                WHERE ProcessedOn IS NULL
                ORDER BY OccurredOn";

            var outboxMessages = await unitOfWork.Connection.QueryAsync<OutboxMessageResponse>(sql, transaction: unitOfWork.Transaction).WaitAsync(cancellationToken);

            return outboxMessages.ToList();
        }

        public Task InsertAsync(string schema, DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = domainEvent.CorrelationId,
                Type = domainEvent.MessageType,
                Content = JsonConvert.SerializeObject(domainEvent, JsonSerializerSettingsExtensions.Instance)!,
                OccurredOn = domainEvent.OccurredOn,
            };

            return InsertAsync(schema, outboxMessage, cancellationToken);
        }

        public Task InsertAsync(string schema, OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            string sql = $"""
                INSERT INTO [{schema}].OutboxMessages
                VALUES(@Id, @Type, @Content, @OccurredOn, @ProcessedOn, @Error)
            """;

            return unitOfWork.Connection.ExecuteAsync(sql, new
            {
                outboxMessage.Id,
                outboxMessage.Type,
                outboxMessage.Content,
                outboxMessage.OccurredOn,
                outboxMessage.ProcessedOn,
                outboxMessage.Error
            }, transaction: unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public Task<bool> IsProcessedAsync(OutboxMessageConsumer outboxMessageConsumer, string schema, CancellationToken cancellationToken)
        {
            var sql = $@"
                SELECT CASE
                    WHEN EXISTS(
                        SELECT 1
                        FROM [{schema}].OutboxMessageConsumers
                        WHERE Id = @Id
                        AND Name = @Name)
                   THEN 1
                   ELSE 0
                END";

            return unitOfWork.Connection.ExecuteScalarAsync<bool>(sql, outboxMessageConsumer).WaitAsync(cancellationToken);
        }

        public Task MarkAsProcessedAsync(OutboxMessageConsumer outboxMessageConsumer, string schema, CancellationToken cancellationToken)
        {
            var sql = $@"
                INSERT INTO [{schema}].OutboxMessageConsumers (Id, Name)
                VALUES (@Id, @Name)";

            return unitOfWork.Connection.ExecuteAsync(sql, outboxMessageConsumer).WaitAsync(cancellationToken);
        }

        public Task UpdateAsync(
            string schema,
            Exception? exception,
            OutboxMessageResponse outboxMessage,
            CancellationToken cancellationToken)
        {
            var sql = @$"
                UPDATE [{schema}].OutboxMessages
                SET ProcessedOn = @ProcessedOn,
                    Error = @Error
                WHERE Id = @Id";

            return unitOfWork.Connection.ExecuteAsync(sql, new
            {
                outboxMessage.Id,
                ProcessedOn = timeProvider.GetUtcNow(),
                Error = exception.GetExceptionMessage()
            }, transaction: unitOfWork.Transaction).WaitAsync(cancellationToken);
        }
    }
}