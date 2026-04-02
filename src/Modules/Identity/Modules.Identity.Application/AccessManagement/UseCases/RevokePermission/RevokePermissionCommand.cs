using Deliveryix.Commons.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Identity.Application.AccessManagement.UseCases.RevokePermission
{
    public sealed record RevokePermissionCommand(string RoleName, string PermissionCode) : ICommand;
}