using Deliveryix.Commons.Domain.DomainObjects;

namespace Deliveryix.Commons.Application.Extensions
{
    public static class ExceptionExtensions
    {
        public static string? GetExceptionMessage(this Exception? exception)
        {
            if (exception is null)
                return null;

            return exception switch
            {
                DeliveryixException dvEx when dvEx.Error?.Description is not null => dvEx.Error.Description,
                _ when exception.InnerException?.Message is not null => exception.InnerException.Message,
                _ => exception.Message
            };
        }
    }
}