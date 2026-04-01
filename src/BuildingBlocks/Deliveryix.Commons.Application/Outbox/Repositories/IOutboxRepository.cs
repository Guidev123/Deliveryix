using Deliveryix.Commons.Application.Outbox.Models;
using Deliveryix.Commons.Domain.DomainObjects;

namespace Deliveryix.Commons.Application.Outbox.Repositories
{
    public interface IOutboxRepository
    {
        Task InsertAsync(string schema, DomainEvent domainEvent, CancellationToken cancellationToken);

        Task InsertAsync(string schema, OutboxMessage outboxMessage, CancellationToken cancellationToken);

        Task UpdateAsync(
            string schema,
            Exception? exception,
            OutboxMessageResponse outboxMessage,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<OutboxMessageResponse>> GetAsync(
            int batchSize,
            string schema,
            CancellationToken cancellationToken);

        Task<bool> IsProcessedAsync(
           OutboxMessageConsumer outboxMessageConsumer,
           string schema,
           CancellationToken cancellationToken);

        Task MarkAsProcessedAsync(OutboxMessageConsumer outboxMessageConsumer,
            string schema,
            CancellationToken cancellationToken);
    }
}