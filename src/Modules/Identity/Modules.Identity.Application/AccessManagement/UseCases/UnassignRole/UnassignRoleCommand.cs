using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.UnassignRole
{
    public sealed record UnassignRoleCommand(Guid IdentityId, string RoleName) : ICommand;
}