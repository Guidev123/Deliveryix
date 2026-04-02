namespace Modules.Identity.Application.AccessManagement.UseCases.GetRole
{
    public sealed record GetRoleResponse(
        string Name,
        IEnumerable<string> Permissions
        );
}