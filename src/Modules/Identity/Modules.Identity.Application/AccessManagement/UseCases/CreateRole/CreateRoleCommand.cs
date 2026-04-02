using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.CreateRole
{
    public sealed record CreateRoleCommand(
        string Name
        ) : ICommand;
}