using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Domain.Identities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Identity.Domain.AcessManagement.Errors
{
    public static class AccessManagementErrors
    {
        public static Error RoleNotFound(string roleName) => Error.NotFound(
            "AcessManagement.RoleNotFound",
            $"The role '{roleName}' was not found"
            );

        public static Error PermissionNotFound(string permission) => Error.NotFound(
            "AcessManagement.PermissionNotFound",
            $"The permission '{permission}' was not found"
            );

        public static readonly Error InvalidRoleName = Error.Problem(
          "AccessManagement.InvalidRoleName",
          "Role name must not be empty");

        public static readonly Error InvalidPermissionCode = Error.Problem(
            "AccessManagement.InvalidPermissionCode",
            "Permission code must follow the format 'resource:action:scope'");

        public static readonly Error InvalidIdentityId = Error.Problem(
            "AccessManagement.InvalidIdentityId",
            "Identity identifier must not be empty");

        public static Error InvalidRoleForIdentityType(IdentityType identityType) => Error.Problem(
            "AccessManagement.InvalidRoleForIdentityType",
            $"Invalid role for identity type {identityType}");
    }
}