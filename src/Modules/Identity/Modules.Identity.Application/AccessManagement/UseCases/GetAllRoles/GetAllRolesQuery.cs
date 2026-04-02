using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Domain.AcessManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Identity.Application.AccessManagement.UseCases.GetAllRoles
{
    public sealed record GetAllRolesQuery(
        int PageSize = 10,
        int PageNumber = 1
        ) : IQuery<PagedResult<Role>>;
}