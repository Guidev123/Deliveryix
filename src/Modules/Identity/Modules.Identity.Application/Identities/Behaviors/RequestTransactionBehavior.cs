using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Application.Outbox.Repositories;
using Deliveryix.Commons.Domain.Results;
using MidR.Behaviors;
using MidR.Interfaces;

namespace Modules.Identity.Application.Identities.Behaviors
{
    public sealed class RequestTransactionBehavior<TRequest, TResponse>(
        IDomainEventCollector domainEventCollector,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork
        ) : IRequestBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IBaseCommand
        where TResponse : Result
    {
        private const string Schema = "identity";

        public async Task<TResponse> ExecuteAsync(TRequest request, RequestDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();

                if (response.IsFailure)
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return response;
                }

                var events = domainEventCollector.Flush();

                foreach (var domainEvent in events)
                {
                    await outboxRepository.InsertAsync(Schema, domainEvent, cancellationToken);
                }

                await unitOfWork.CommitAsync(cancellationToken);

                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}