namespace Modules.Identity.Application.AccessManagement.UseCases.GetPermissions
{
    public sealed record GetIdentityPermissionsResponse(
        Guid IdentityId,
        IReadOnlyCollection<RolePermissions> Roles
    );

    public sealed record RolePermissions(
        string RoleName,
        HashSet<string> Permissions
    );
}