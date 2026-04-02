using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.CreatePermission
{
    public sealed record CreatePermissionCommand(string Code) : ICommand;
}