using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.AssignRole
{
    public sealed record AssignRoleCommand(
        Guid IdentityId,
        string RoleName
        ) : ICommand;
}