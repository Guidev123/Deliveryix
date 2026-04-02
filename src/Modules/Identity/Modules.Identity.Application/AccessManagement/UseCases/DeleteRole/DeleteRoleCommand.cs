using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.DeleteRole
{
    public sealed record DeleteRoleCommand(string RoleName) : ICommand;
}