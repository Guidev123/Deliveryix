namespace Modules.Identity.Application.AccessManagement.UseCases.GetPermissions
{
    public sealed record IdentityPermission(Guid Id, string RoleName, string PermissionCode);
}