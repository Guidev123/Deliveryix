using Deliveryix.Commons.Domain.Results;
using MidR.Interfaces;

namespace Deliveryix.Commons.Application.Messaging
{
    public interface IQuery<TR> : IRequest<Result<TR>>;
}