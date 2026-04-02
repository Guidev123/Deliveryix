using Deliveryix.Commons.Application.Messaging;

namespace Modules.Identity.Application.AccessManagement.UseCases.GetRole
{
    public sealed record GetRoleQuery(string Name) : IQuery<GetRoleResponse>;
}