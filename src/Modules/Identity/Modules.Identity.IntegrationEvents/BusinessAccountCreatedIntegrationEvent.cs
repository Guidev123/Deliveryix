using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.IntegrationEvents
{
    public sealed record BusinessAccountCreatedIntegrationEvent : IntegrationEvent
    {
        public static BusinessAccountCreatedIntegrationEvent Create(
            Guid identityId,
            string email,
            string document,
            string phone
            ) => new(identityId.ToString(), email, document, phone);

        private BusinessAccountCreatedIntegrationEvent(string userId, string email, string document, string phone)
            : base(nameof(IndividualAccountCreatedIntegrationEvent), ModuleExtensions.ModuleName)
        {
            IdentityId = userId;
            Email = email;
            Phone = phone;
            Document = document;
        }

        private BusinessAccountCreatedIntegrationEvent()
        { }

        public string Email { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string IdentityId { get; private set; } = null!;
        public static string Topic => "identity.business-account-created";
    }
}