using Deliveryix.Commons.Domain.Results;
using MidR.Interfaces;

namespace Deliveryix.Commons.Application.Messaging
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
          where TCommand : ICommand;

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>;
}