using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.UseCases.GetPermissions;
using Modules.Identity.Application.AccessManagement.UseCases.GetRole;
using Modules.Identity.Domain.AcessManagement.Models;
using Modules.Identity.Domain.Identities.Enums;

namespace Modules.Identity.Application.AccessManagement.Repositories
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role, CancellationToken cancellationToken = default);

        Task<GetRoleResponse?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<PagedResult<Role>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        Task<bool> RoleExistsAsync(string name, CancellationToken cancellationToken = default);

        Task DeleteAsync(string name, CancellationToken cancellationToken = default);

        Task AssignToIdentityAsync(string roleName, Guid identityId, CancellationToken cancellationToken = default);

        Task UnassignFromIdentityAsync(string roleName, Guid identityId, CancellationToken cancellationToken = default);

        Task GrantPermissionAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default);

        Task RevokePermissionAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default);

        Task<bool> RolePermissionExistsAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default);

        Task AddPermissionAsync(string code, CancellationToken cancellationToken = default);

        Task<bool> PermissionExistsAsync(string code, CancellationToken cancellationToken = default);

        Task AddDefaultRoleForIdentityTypeAsync(string roleName, IdentityType identityType, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<Role>> GetDefaultRolesByIdentityTypeAsync(IdentityType identityType, CancellationToken cancellationToken = default);

        Task<GetIdentityPermissionsResponse?> GetIdentityPermissionsAsync(Guid identityProviderId, CancellationToken cancellationToken = default);
    }
}