using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.GrantPermission
{
    public sealed record GrantPermissionCommand(string RoleName, string PermissionCode) : ICommand;
}