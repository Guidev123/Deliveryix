using Deliveryix.Commons.Application.Messaging;
using Modules.Identity.Domain.Identities.Enums;

namespace Modules.Identity.Application.AccessManagement.UseCases.SetDefaultIdentityTypeRole
{
    public sealed record SetDefaultIdentityTypeRoleCommand(IdentityType IdentityType, string RoleName) : ICommand;
}