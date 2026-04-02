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

        public static Error InvalidEmail() => Error.Problem(
             "Identities.InvalidEmail",
             "Email address is invalid or exceeds maximum length");

        public static Error InvalidPhone() => Error.Problem(
            "Identities.InvalidPhone",
            "Phone number is invalid. Expected format: +[country code][number] (E.164)");

        public static Error InvalidDocument() => Error.Problem(
            "Identities.InvalidDocument",
            "Document number is invalid. Must be a valid CPF (11 digits) or CNPJ (14 digits)");

        public static Error InvalidIdentityStatus() => Error.Problem(
            "Identities.InvalidIdentityStatus",
            "Identity status is invalid");

        public static Error InvalidIdentityProviderId() => Error.Problem(
            "Identities.InvalidIdentityProviderId",
            "Identity provider identifier must not be empty");
    }
}