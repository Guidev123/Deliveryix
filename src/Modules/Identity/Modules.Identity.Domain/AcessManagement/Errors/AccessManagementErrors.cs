using Deliveryix.Commons.Domain.Results;
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
    }
}