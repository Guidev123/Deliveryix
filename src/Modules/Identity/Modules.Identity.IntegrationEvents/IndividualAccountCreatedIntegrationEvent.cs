using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.IntegrationEvents
{
    public sealed record IndividualAccountCreatedIntegrationEvent : IntegrationEvent
    {
        public static IndividualAccountCreatedIntegrationEvent Create(
            Guid identityId,
            string email,
            string document,
            string phone
            ) => new(identityId.ToString(), email, document, phone);

        private IndividualAccountCreatedIntegrationEvent(string userId, string email, string document, string phone)
            : base(nameof(IndividualAccountCreatedIntegrationEvent), ModuleExtensions.ModuleName)
        {
            IdentityId = userId;
            Email = email;
            Phone = phone;
            Document = document;
        }

        private IndividualAccountCreatedIntegrationEvent()
        { }

        public string Email { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string IdentityId { get; private set; } = null!;
        public static string Topic => "identity.individual-account-created";
    }
}