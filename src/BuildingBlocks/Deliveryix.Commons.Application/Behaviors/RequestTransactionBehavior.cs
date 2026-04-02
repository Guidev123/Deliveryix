using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Application.Outbox.Repositories;
using Deliveryix.Commons.Domain.Results;
using Microsoft.Extensions.Logging;
using MidR.Behaviors;
using MidR.Interfaces;

namespace Deliveryix.Commons.Application.Behaviors
{
    public sealed class RequestTransactionBehavior<TRequest, TResponse>(
        IDomainEventCollector domainEventCollector,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork,
        IModuleInfo moduleInfo,
        ILogger<RequestTransactionBehavior<TRequest, TResponse>> logger
        ) : IRequestBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IBaseCommand
        where TResponse : Result
    {
        public async Task<TResponse> ExecuteAsync(TRequest request, RequestDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Transaction started for request {RequestType}", typeof(TRequest).Name);
            }

            try
            {
                var response = await next();

                if (response.IsFailure)
                {
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Transaction rollback performed due to handler failure");
                    }

                    await unitOfWork.RollbackAsync(cancellationToken);
                    return response;
                }

                var events = domainEventCollector.Flush();

                foreach (var domainEvent in events)
                {
                    await outboxRepository.InsertAsync(moduleInfo.Name, domainEvent, cancellationToken);
                }

                await unitOfWork.CommitAsync(cancellationToken);

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Transaction completed successfully");
                }

                return response;
            }
            catch
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Transaction rollback performed due to exception");
                }

                await unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}