using Deliveryix.Commons.Domain.Results;
using MidR.Interfaces;

namespace Deliveryix.Commons.Application.Messaging
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
      where TQuery : IQuery<TResponse>;
}