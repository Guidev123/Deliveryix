using Deliveryix.Commons.Domain.Results;

namespace Modules.Identity.Domain.Identities.Errors
{
    public sealed class IdentityErrors
    {
        public static Error IdentityNotFound(string email) => Error.NotFound(
            "Identities.IdentityNotFound",
            $"Identity with email {email} was not found");

        public static Error IdentityNotFound(Guid id) => Error.NotFound(
            "Identities.IdentityNotFound",
            $"Identity with identifier {id} was not found");

        public static Error GraphApiError(string? code, string? message) => Error.Problem(
            "Identities.GraphApiError",
            code is not null && message is not null
            ? $"Microsoft Graph returned an error. Code: {code}. Message: {message}"
            : "Something has failed during Graph request");

        public static Error AlreadyExists(string document) => Error.Conflict(
            "Identities.AlreadyExists",
            $"Identity with document {document} already exists");
    }
}