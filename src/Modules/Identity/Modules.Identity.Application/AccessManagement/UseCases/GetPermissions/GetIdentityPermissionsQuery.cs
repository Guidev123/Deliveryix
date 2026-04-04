using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.GetPermissions
{
    public sealed record GetIdentityPermissionsQuery(Guid IdentityProviderId) : IQuery<GetIdentityPermissionsResponse>;
}