using Deliveryix.Commons.Domain.Results;

namespace Deliveryix.Commons.Domain.DomainObjects
{
    public class DeliveryixException : Exception
    {
        public DeliveryixException(string requestName, Error? error = default, Exception? innerException = default)
            : base("Application exception", innerException)
        {
            RequestName = requestName;
            Error = error;
        }

        public string RequestName { get; }

        public Error? Error { get; }
    }
}