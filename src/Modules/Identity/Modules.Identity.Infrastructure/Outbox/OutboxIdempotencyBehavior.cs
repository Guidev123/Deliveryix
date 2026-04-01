using Deliveryix.Commons.Application.Outbox.Models;
using Deliveryix.Commons.Application.Outbox.Repositories;
using Deliveryix.Commons.Domain.DomainObjects;
using MidR.Behaviors;
using Modules.Identity.Infrastructure.Database;

namespace Modules.Identity.Infrastructure.Outbox
{
    public sealed class OutboxIdempotencyBehavior<TNotification>(
            IOutboxRepository outboxRepository
            )
            : INotificationBehavior<TNotification>
            where TNotification : DomainEvent
    {
        public async Task ExecuteAsync(TNotification notification, NotificationDelegate next, CancellationToken cancellationToken)
        {
            var outboxMessageConsumer = new OutboxMessageConsumer(notification.CorrelationId, notification.Messagetype);

            var schema = Schemas.DefaultSchemaName;

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