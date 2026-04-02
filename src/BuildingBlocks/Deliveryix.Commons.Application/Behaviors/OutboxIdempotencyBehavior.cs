using Deliveryix.Commons.Application.Outbox.Models;
using Deliveryix.Commons.Application.Outbox.Repositories;
using Deliveryix.Commons.Domain.DomainObjects;
using MidR.Behaviors;

namespace Deliveryix.Commons.Application.Behaviors
{
    public sealed class OutboxIdempotencyBehavior<TNotification>(
            IOutboxRepository outboxRepository
            )
            : INotificationBehavior<TNotification>
            where TNotification : DomainEvent
    {
        public async Task ExecuteAsync(TNotification notification, NotificationDelegate next, CancellationToken cancellationToken)
        {
            var outboxMessageConsumer = new OutboxMessageConsumer(notification.CorrelationId, notification.MessageType);

            var schema = notification.Module;

            var isProcessed = await outboxRepository.IsProcessedAsync(outboxMessageConsumer, schema, cancellationToken);
            if (isProcessed)
            {
                return;
            }

            await next();

            await outboxRepository.MarkAsProcessedAsync(outboxMessageConsumer, schema, cancellationToken);
        }
    }
}