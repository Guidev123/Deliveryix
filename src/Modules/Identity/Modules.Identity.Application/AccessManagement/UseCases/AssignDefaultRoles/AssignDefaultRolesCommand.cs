using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.AssignDefaultRoles
{
    public sealed record AssignDefaultRolesCommand(Guid IdentityId) : ICommand;
}