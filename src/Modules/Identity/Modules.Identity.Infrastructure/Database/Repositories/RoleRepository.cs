using Dapper;
using Deliveryix.Commons.Application.Abstractions;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Application.AccessManagement.UseCases.GetRole;
using Modules.Identity.Domain.AcessManagement.Models;
using Modules.Identity.Domain.Identities.Extensions;

namespace Modules.Identity.Infrastructure.Database.Repositories
{
    internal sealed class RoleRepository(IUnitOfWork unitOfWork) : IRoleRepository
    {
        private static readonly string Schema = ModuleExtensions.ModuleName;

        public Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                INSERT INTO [{Schema}].Roles (Name)
                VALUES (@Name)
            """;

            return unitOfWork.Connection.ExecuteAsync(sql, new
            {
                role.Name
            }, unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public Task DeleteAsync(string name, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                DELETE FROM [{Schema}].Roles
                WHERE Name = @Name
            """;

            return unitOfWork.Connection.ExecuteAsync(sql, new
            {
                Name = name
            }, unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public async Task<GetRoleResponse?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            var sql = $"""
                SELECT r.Name, p.Code AS PermissionCode
                FROM [{Schema}].Roles r
                LEFT JOIN [{Schema}].RolePermissions rp ON rp.RoleName = r.Name
                LEFT JOIN [{Schema}].Permissions p ON p.Code = rp.PermissionCode
                WHERE r.Name = @Name
            """;

            GetRoleResponse? result = null;
            var permissions = new List<string>();

            await unitOfWork.Connection.QueryAsync<string, string?, GetRoleResponse>(
                sql,
                (roleName, permissionCode) =>
                {
                    result ??= new GetRoleResponse(roleName, permissions);

                    if (permissionCode is not null)
                        permissions.Add(permissionCode);

                    return result;
                },
                new { Name = name },
                unitOfWork.Transaction,
                splitOn: "PermissionCode").WaitAsync(cancellationToken);

            return result;
        }

        public async Task<PagedResult<Role>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                SELECT Name
                FROM [{Schema}].Roles
                ORDER BY Name
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM [{Schema}].Roles;
            """;

            using var multi = await unitOfWork.Connection.QueryMultipleAsync(
                sql,
                new { Offset = (page - 1) * pageSize, PageSize = pageSize },
                unitOfWork.Transaction).WaitAsync(cancellationToken);

            var roles = (await multi.ReadAsync<Role>().WaitAsync(cancellationToken)).ToList();
            var total = await multi.ReadSingleAsync<int>().WaitAsync(cancellationToken);

            return new PagedResult<Role>(roles, total, page, pageSize);
        }

        public Task<bool> RoleExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM [{Schema}].Roles
                    WHERE Name = @Name
                ) THEN 1 ELSE 0 END AS BIT)
            """;

            return unitOfWork.Connection.ExecuteScalarAsync<bool>(
                sql,
                new { Name = name },
                unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public Task AssignToIdentityAsync(string roleName, Guid identityId, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                IF NOT EXISTS (
                    SELECT 1 FROM [{Schema}].IdentityRoles
                    WHERE IdentityId = @IdentityId AND RoleName = @RoleName
                )
                INSERT INTO [{Schema}].IdentityRoles (IdentityId, RoleName)
                VALUES (@IdentityId, @RoleName)
            """;

            return unitOfWork.Connection.ExecuteAsync(
                sql,
                new { IdentityId = identityId, RoleName = roleName },
                unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public Task UnassignFromIdentityAsync(string roleName, Guid identityId, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                DELETE FROM [{Schema}].IdentityRoles
                WHERE IdentityId = @IdentityId AND RoleName = @RoleName
            """;

            return unitOfWork.Connection.ExecuteAsync(
                sql,
                new { IdentityId = identityId, RoleName = roleName },
                unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public Task GrantPermissionAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                IF NOT EXISTS (
                    SELECT 1 FROM [{Schema}].RolePermissions
                    WHERE RoleName = @RoleName AND PermissionCode = @PermissionCode
                )
                INSERT INTO [{Schema}].RolePermissions (RoleName, PermissionCode)
                VALUES (@RoleName, @PermissionCode)
            """;

            return unitOfWork.Connection.ExecuteAsync(
                sql,
                new { RoleName = roleName, PermissionCode = permissionCode },
                unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public Task RevokePermissionAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                DELETE FROM [{Schema}].RolePermissions
                WHERE RoleName = @RoleName AND PermissionCode = @PermissionCode
            """;

            return unitOfWork.Connection.ExecuteAsync(
                sql,
                new { RoleName = roleName, PermissionCode = permissionCode },
                unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public Task<bool> RolePermissionExistsAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM [{Schema}].RolePermissions
                    WHERE RoleName = @RoleName AND PermissionCode = @PermissionCode
                ) THEN 1 ELSE 0 END AS BIT)
            """;

            return unitOfWork.Connection.ExecuteScalarAsync<bool>(
                sql,
                new { RoleName = roleName, PermissionCode = permissionCode },
                unitOfWork.Transaction).WaitAsync(cancellationToken);
        }

        public Task AddPermissionAsync(string code, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                IF NOT EXISTS (
                    SELECT 1 FROM [{Schema}].Permissions
                    WHERE Code = @Code
                )
                INSERT INTO [{Schema}].Permissions (Code)
                VALUES (@Code)
            """;

            return unitOfWork.Connection.ExecuteAsync(
                sql,
                new { Code = code },
                unitOfWork.Transaction
            ).WaitAsync(cancellationToken);
        }

        public Task<bool> PermissionExistsAsync(string code, CancellationToken cancellationToken = default)
        {
            var sql = $"""
                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM [{Schema}].Permissions
                    WHERE Code = @Code
                ) THEN 1 ELSE 0 END AS BIT)
            """;

            return unitOfWork.Connection.ExecuteScalarAsync<bool>(
                sql,
                new { Code = code },
                unitOfWork.Transaction
            ).WaitAsync(cancellationToken);
        }
    }
}