using Deliveryix.Commons.Domain.DomainObjects;
using Microsoft.Extensions.Logging;
using MidR.Behaviors;
using MidR.Interfaces;
using Serilog.Context;
using System.Diagnostics;

namespace Deliveryix.Commons.Application.Behaviors
{
    public sealed class NotificationLoggingBehavior<TNotification>(ILogger<NotificationLoggingBehavior<TNotification>> logger) : INotificationBehavior<TNotification>
            where TNotification : DomainEvent

    {
        public async Task ExecuteAsync(TNotification notification, NotificationDelegate next, CancellationToken cancellationToken)
        {
            var notificationName = typeof(TNotification).Name;
            var notificationModule = GetRequestModule(typeof(TNotification).FullName!);

            Activity.Current?.SetTag("notification.module", notificationModule);
            Activity.Current?.SetTag("notification.name", notificationName);

            var stopwatch = Stopwatch.StartNew();
            using (LogContext.PushProperty("Module", notificationModule))
            {
                try
                {
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Processing notification: {NotificationName}", notificationName);
                    }

                    await next();

                    stopwatch.Stop();
                    var executionTime = stopwatch.ElapsedMilliseconds;

                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Notification handlers for {NotificationName} processed successfully in {ExecutionTimeInMilliseconds}ms",
                            notificationName, executionTime);
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    var executionTime = stopwatch.ElapsedMilliseconds;

                    logger.LogError(ex, "Notification handlers for {NotificationName} failed in {ExecutionTimeInMilliseconds}ms with unhandled exception",
                        notificationName,
                        executionTime);
                }
            }
        }

        private static string GetRequestModule(string requestName) => requestName.Split('.')[1];
    }
}